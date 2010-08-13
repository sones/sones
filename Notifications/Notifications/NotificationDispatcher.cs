/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* PandoraLib - NotificationDispatcher
 * (c) Stefan Licht, 2009
 * 
 * The NotificationDispatcher handles all kind of notification between system parts or other dispatchers.
 * Use register to get notified as recipient.
 * Us SendNotification to send a notification to all subscribed recipients.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using sones.Lib.DataStructures.UUID;
using sones.Notifications.Exceptions;
using sones.Notifications.Messages;
using sones.Notifications.NotificationTypes;
using sones.Lib;
using sones.Lib.Threading;
using sones.Lib.DataStructures.Timestamp;

namespace sones.Notifications
{
    /// <summary>
    /// The NotificationDispatcher handles all kind of notification between system parts or other dispatchers.
    /// Use register to get notified as recipient.
    /// Us SendNotification to send a notification to all subscribed recipients.
    /// </summary>
    public class NotificationDispatcher : IDisposable
    {

        #region Data definition

        #region Consts

        private const Int32 MAX_RETRIES_HandleNotification = 10;

        #endregion

        #region Structs

        /// <summary>
        /// This struct combines the Notification-reference-class with a NotificationType
        /// Used for _Recipients
        /// </summary>
        private struct RecipientsEntry : IComparable<RecipientsEntry>
        {
            public INotification INotification;
            public ANotificationType ANotificationType;

            public RecipientsEntry(INotification myINotification, ANotificationType myANotificationType)
            {
                INotification = myINotification;
                ANotificationType = myANotificationType;
            }

            #region IComparable<Notification_NotificationType> Members

            public int CompareTo(RecipientsEntry other)
            {
                if (INotification == other.INotification && ANotificationType.GetType() == other.ANotificationType.GetType())
                    return 0;

                return -1;
            }

            public override bool Equals(object obj)
            {
                if (CompareTo((RecipientsEntry)obj) == 0)
                    return true;

                return false;
            }

            public override int GetHashCode()
            {
                return INotification.GetHashCode();
            }

            #endregion

        }

        /// <summary>
        /// This struct is the key of the MessageQueue which stores all messages to be send through the dispatcher
        /// </summary>
        private struct MessageQueueEntry : IComparable
        {
            NotificationPriority Priority;
            public NotificationMessage Message;
            public INotification NotificationReference;

            public MessageQueueEntry(NotificationMessage myMessage, INotification myNotificationReference)
            {
                Message = myMessage;
                NotificationReference = myNotificationReference;
                Priority = Message.Priority;
            }

            #region IComparable<MessageQueueEntry> Members

            public int CompareTo(Object other)
            {

                if (Priority > ((MessageQueueEntry)other).Priority)
                    return -1;

                if (Priority < ((MessageQueueEntry)other).Priority)
                    return 1;

                return 0;

            }

            public override bool Equals(object obj)
            {
                return CompareTo((MessageQueueEntry)obj) == 0;
            }

            public override int GetHashCode()
            {
                return Message.GetHashCode();
            }

            #endregion

        }

        #endregion

        #region Fields

        /// <summary>
        /// Locks the _Recipients datastructure and all access/modification on it
        /// </summary>
        private ReaderWriterLockSlim _Locker = null;
        
        /// <summary>
        /// Locks the _ConntectedDispatchers datastructure and all access/modification on it
        /// </summary>
        private ReaderWriterLockSlim _LockerConnectedDispatchers = null;
        
        /// <summary>
        /// Locks the _MessageQueue datastructure and all access/modification on it.
        /// Since we use it for Montior, we can't use the ReaderWriterLockSlim. The Monitor is used to stop the
        /// SendMessageThread as long as there are no entries in the MessageQueue
        /// </summary>
        private Object _MessageQueueThreadLock = new Object();


        /// <summary>
        /// Stores all connected NotificationDispatcher
        /// </summary>
        private List<NotificationDispatcher> _ConnectedDispatchers;

        /// <summary>
        /// Stores a List of Recipients for a particular NotificationType
        /// INotificationType, []{ Guid, INotification Reference, Params }
        /// </summary>
        private Dictionary<Type, List<RecipientsEntry>> _Recipients;

        /// <summary>
        /// This is the NotificationBridge. If it is null, than there is no started bridge. If it has an valid instance, than all notifications
        /// will be send via multicast.
        /// </summary>
        private NotificationBridge _Bridge;

        /// <summary>
        /// The MessageQueue stores all Messages which will be send in the SendMessageThread.
        /// The Key is based (and sorted) by the priority and Created_TimeStamp
        /// </summary>
        //private SortedList<NotificationMessage,INotification> _MessageQueue;
        //private BStarTreeList _MessageQueue;
        private SortedList<NotificationPriority, SortedList<NotificationMessage,INotification>> _MessageQueues;
        
        /// <summary>
        /// The total number of message queue entries of ALL priorities
        /// </summary>
        private Int32 _TotalMessageQueueEntries = 0;

        /// <summary>
        /// Identifies, whether or not the dispatcher is started.
        /// </summary>
        private Boolean _IsStarted = false;

        /// <summary>
        /// Identifies, whether or not the dispatcher is suspended.
        /// </summary>
        private Boolean _IsSuspended = false;

        private Boolean _IsStopRequested = false;

        private Boolean _Disposed = false;

        /// <summary>
        /// This Lookuptable holds an empty (ctor) instance of all NotificationTypes. The key is the name of the NotificationType.
        /// </summary>
        public Dictionary<String, ANotificationType> NotificationTypesNamesLookupTable;

        private Dictionary<String,Type> _NotificationTypesLookupTable;

        private GraphThreadPool _BridgeSendThreadPool;
        //private SmartThreadPool _BridgeSendThreadPool;
        private GraphThreadPool _SendMessageThreadPool;
        //private SmartThreadPool _SendMessageThreadPool;

        /// <summary>
        /// Thie holds the messages sent with this priority. If the _SentPriorityThresholds with this priority is reached, 
        /// we will switch to the next priority to give them a chance
        /// </summary>
        private Dictionary<byte, Int32> _SentPriorities = new Dictionary<byte, Int32>();

        /// <summary>
        /// Since we can't make sure, that the priority enum have consecutive numbers, this holds the lowest priority
        /// </summary>
        private Byte _LowestPriority = 0;

        private NotificationSettings _NotificationSettings;
            
        #endregion

        #region Properties
        /// <summary>
        /// Each Dispatcher has a unique id. This will help the bridge to identify whether or not the incoming multicast message comes from a different
        /// Dispatcher. If it comes from the same, than the message should be discarded.
        /// </summary>
        public String Uuid
        {
            get
            {
                return _Uuid;
            }
        }
        private String _Uuid;

        /// <summary>
        /// The dispatcher sender ID. If RestrictedSenderID is set, only messages with this senderID will be accepted
        /// </summary>
        public UUID SenderID
        {
            get { return _SenderID; }
            set { _SenderID = value; }
        }
        private UUID _SenderID;

        /// <summary>
        /// Identifies, whether or not the bridge is started.
        /// </summary>
        public Boolean BridgeStarted
        {
            get { return (_Bridge != null); }
        }

        /// <summary>
        /// Identifies, whether or not the dispatcher is started and NOT suspended.
        /// </summary>
        public Boolean DispatcherRunning
        {
            get { return (_IsStarted && !_IsSuspended); }
        }

        /// <summary>
        /// Only accept multicast notifications send from this Sender ID
        /// TODO: accept a list of restricted Sender IDs
        /// </summary>
        public String RestrictedSenderID { get; set; }

        public Int32 MulticastTTL
        {
            get { return _NotificationSettings.BridgeMulticastTTL; }
            set { _NotificationSettings.BridgeMulticastTTL = value; }
        }

        #endregion

        #endregion

        #region Constructor
 
        /// <summary>
        /// Create a new Instance of a NotificationDispatcher with a name
        /// </summary>
        /// <param name="mySenderID">The sender identification</param>
        /// <param name="myDispatcherName">The name of the NotificationDispatcher</param>
        public NotificationDispatcher(UUID mySenderID)
            : this (mySenderID, new NotificationSettings())
        { }

        /// <summary>
        /// Create a new Instance of a NotificationDispatcher with a name
        /// </summary>
        /// <param name="mySenderID">The sender identification</param>
        /// <param name="myDispatcherName">The name of the NotificationDispatcher</param>
        /// <param name="myNotificationSettings">The NotificationSettings</param>
        public NotificationDispatcher(UUID mySenderID, NotificationSettings myNotificationSettings)
        {

            _Locker                     = new ReaderWriterLockSlim();
            _LockerConnectedDispatchers = new ReaderWriterLockSlim();

            _ConnectedDispatchers      = new List<NotificationDispatcher>();
            _Recipients                 = new Dictionary<Type, List<RecipientsEntry>>();
            //_MessageQueue               = new SortedList<NotificationMessage, INotification>();
            _MessageQueues              = new SortedList<NotificationPriority,SortedList<NotificationMessage,INotification>>();
            //_MessageQueue = new BStarTreeList(10);

            _NotificationSettings       = myNotificationSettings;

            if (_NotificationSettings.StartBrigde)
            {
                _BridgeSendThreadPool = new GraphThreadPool("BridgeSendThreadPool <" + Encoding.UTF8.GetString(mySenderID.GetByteArray()) + ">");
                //_BridgeSendThreadPool       = new SmartThreadPool(1000, 2, Environment.ProcessorCount);
                _SendMessageThreadPool = new GraphThreadPool("SendMessageThreadPool <" + Encoding.UTF8.GetString(mySenderID.GetByteArray()) + ">");
                //_SendMessageThreadPool      = new SmartThreadPool(1000, 2, Environment.ProcessorCount);

                _BridgeSendThreadPool.OnWorkerThreadException += new WorkerThreadExceptionHandler(ThreadPool_OnWorkerThreadException);
                _SendMessageThreadPool.OnWorkerThreadException += new WorkerThreadExceptionHandler(ThreadPool_OnWorkerThreadException);
            }

            _Uuid                       = Guid.NewGuid().ToString();
            _SenderID                   = mySenderID;
            RestrictedSenderID           = String.Empty;

            foreach (byte val in Enum.GetValues(typeof(NotificationPriority)))
            {
                _SentPriorities.Add(val, 0);
                if (val > _LowestPriority)
                    _LowestPriority = val;
            }

            FindAndFillNotificationTypesLookupTable();

            if (myNotificationSettings.StartDispatcher)
                StartDispatcher();

            if (myNotificationSettings.StartBrigde)
                StartBridge(myNotificationSettings.BridgePort);

        }

        void ThreadPool_OnWorkerThreadException(object mySender, Exception myException)
        {
            System.Diagnostics.Debug.WriteLine("[NotificationDispatcher] ThreadPool_OnWorkerThreadException: " + myException.Message + " " + myException.StackTrace);
        }

        #endregion

        #region Settings

        public void ChangeSettings(NotificationSettings myNotificationSettings)
        {
            _NotificationSettings = myNotificationSettings;
        }

        public NotificationSettings NotificationSettings
        {
            get
            {
                return _NotificationSettings;
            }
        }


        #endregion

        #region Helpers

        /// <summary>
        /// Returns an new instance of an ANotificationType class defined by the FullBaseName.
        /// You can get the FullBaseName with the extension '[Type]&gt;[TheBasType]&lt;FullBaseName()' for a Type.
        /// </summary>
        /// <param name="myFullBaseName">The FullBaseName</param>
        /// <returns>A new Instance of a ANotificationType defined by FullBaseName</returns>
        public ANotificationType GetEmptyNotificationTypeFromFullBaseName(String myFullBaseName)
        {
            if (NotificationTypesNamesLookupTable.ContainsKey(myFullBaseName))
                return NotificationTypesNamesLookupTable[myFullBaseName];

            throw new NotificationException_InvalidNotificationType("Could not find " + myFullBaseName + " in Lookup table");
        }

        private void FindAndFillNotificationTypesLookupTable()
        {
            NotificationTypesNamesLookupTable = new Dictionary<string, ANotificationType>();
            _NotificationTypesLookupTable = new Dictionary<string, Type>();

            foreach (string fileOn in Directory.GetFiles("."))
            {

                FileInfo file = new FileInfo(fileOn);

                //Preliminary check, must be .dll
                if ( (file.Extension.Equals(".dll")) || (file.Extension.Equals(".exe")))
                {

                    try
                    {
                        Type[] allTypes = Assembly.LoadFrom(file.FullName).GetTypes();
                        foreach (Type type in allTypes)
                        {
                            String fullTypeName = type.Name;

                            Type tempType = type;
                            Int32 steps = 0;
                            while (tempType != null && steps++ < 5 && tempType != typeof(Object) && tempType.BaseType != typeof(ANotificationType) && !NotificationTypesNamesLookupTable.ContainsKey(tempType.Name))
                            {
                                tempType = tempType.BaseType;
                                if (tempType != null) fullTypeName = String.Concat(tempType.Name, ".", fullTypeName);
                            }

                            if ((tempType != null) && 
                                   ( (tempType.BaseType == typeof(ANotificationType)) // we reached the correct basetype
                                || ( (NotificationTypesNamesLookupTable.ContainsKey(tempType.Name)) && (!NotificationTypesNamesLookupTable.ContainsKey(type.Name)))) // the current basetype is already added, so add this NotificationType
                               )
                            {
                                ANotificationType NotificationType = (ANotificationType)Activator.CreateInstance(type);

                                _NotificationTypesLookupTable.Add(fullTypeName, type);

                                NotificationTypesNamesLookupTable.Add(fullTypeName, NotificationType);
                            }

                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(file.Name + " failed: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Changes the multicast IP:Port for a NotificationType and all their derived NotificationTypes.
        /// </summary>
        /// <param name="myNotificationType">The NotificationType</param>
        /// <param name="myIPAddress">Valid multicast IPAddress is Class D - 224.0.0.0 to 239.255.255.255</param>
        /// <param name="myPort">A valid Port</param>
        public void SetNotificationTypeMulticastInfo(Type myNotificationType, IPAddress myIPAddress, Int32 myPort)
        {

            #region Check UDP IP Address and Port

            //224.0.0.0 to 239.255.255.255
            String[] parts = myIPAddress.ToString().Split(new char[] { '.' });
            if (Int32.Parse(parts[0]) < 224 || Int32.Parse(parts[0]) > 239)
                throw new NotificationException_InvalidMulticastIP("Invalid multicast IP Address");

            if (myPort < 0 || myPort > 65535)
                throw new NotificationException_InvalidMulticastPort(myPort + " is not a valid port (must between 0 and )");

            #endregion

            String fullBaseName = myNotificationType.FullBaseName<ANotificationType>();
            if (!NotificationTypesNamesLookupTable.ContainsKey(fullBaseName))
                throw new NotificationException_InvalidNotificationType("Could not find " + myNotificationType.FullBaseName<ANotificationType>());

            // Since we have the full basename we need to change all starting with this: 
            List<String> foundKeys = new List<String>();
            foreach (String key in NotificationTypesNamesLookupTable.Keys)
            {
                // the full basename until the real base ANotificationType
                if (key.StartsWith(fullBaseName))
                {
                    ANotificationType nt = NotificationTypesNamesLookupTable[key];

                    if (_Bridge != null)
                    {

                        _Bridge.ChangeBridgeTypeConnection(nt, myIPAddress, myPort);

                    }

                    nt.MulticastIPAddress = myIPAddress;
                    nt.MulticastPort = myPort;
                }
            }

        }

        public Int32 GetNumberOfSubscribers(Type myNotificationType)
        {
            if (!_Recipients.ContainsKey(myNotificationType)) 
                return 0;

            return _Recipients[myNotificationType].Count;
        }

        #endregion

        #region Start/Suspend/Resume Distpatcher

        /// <summary>
        /// Start the Dispatcher if it is not already started.
        /// </summary>
        public void StartDispatcher()
        {
            
            if (!_IsStarted)
            {

                if (_SendMessageThreadPool == null)
                {
                    _SendMessageThreadPool = new GraphThreadPool("SendMessageThreadPool <" + "__" + ">");
                    //_SendMessageThreadPool      = new SmartThreadPool(1000, 2, Environment.ProcessorCount);

                    _SendMessageThreadPool.OnWorkerThreadException += new WorkerThreadExceptionHandler(ThreadPool_OnWorkerThreadException);
                }

                Thread sendMessageThreadInstance = new Thread(new ThreadStart(SendMessageThread));
                sendMessageThreadInstance.Name = "NotificationDispatcher.SendMessageThread() ["+_Uuid+"]";
                sendMessageThreadInstance.IsBackground = true;
                sendMessageThreadInstance.Start();

                _IsSuspended = false;

                // wait for thread start completed
                while (!_IsStarted)
                    Thread.Sleep(1);
            }

        }

        /// <summary>
        /// Suspends the dispatcher but send all queued messages before suspend
        /// </summary>
        public void SuspendDispatcher()
        {
            if (_IsStarted)
            {
                _IsSuspended = true;
            }
        }

        /// <summary>
        /// Resume a prior suspended dispatcher or start the dispatcher
        /// </summary>
        public void ResumeOrStartDispatcher()
        {
            if (_IsStarted)
            {
                _IsSuspended = false;
            }
            else
            {
                StartDispatcher();
            }
        }

        #endregion
        
        #region (Dis)ConnectDispatcher

        /// <summary>
        /// Connect a second NotificationDispatcher with this NotificationDispatcher
        /// </summary>
        /// <param name="myNotificationDispatcher"></param>
        public void ConnectDispatcher(NotificationDispatcher myNotificationDispatcher)
        {

            if (myNotificationDispatcher == null)
                throw new ArgumentNullException();

            _LockerConnectedDispatchers.EnterWriteLock();

            try
            {
                if (!_ConnectedDispatchers.Contains(myNotificationDispatcher))
                    _ConnectedDispatchers.Add(myNotificationDispatcher);
            }
            catch (Exception e)
            {
                _LockerConnectedDispatchers.ExitWriteLock();
                throw e;
            }

            _LockerConnectedDispatchers.ExitWriteLock();

        }

        /// <summary>
        /// Disconnect a second NotificationDispatcher from this NotificationDispatcher
        /// </summary>
        /// <param name="myNotificationDispatcher"></param>
        public void DisconnectDispatcher(NotificationDispatcher myNotificationDispatcher)
        {

            if (myNotificationDispatcher == null)
                throw new ArgumentNullException();

            _LockerConnectedDispatchers.EnterWriteLock();
            try
            {
                if (_ConnectedDispatchers.Contains(myNotificationDispatcher))
                    _ConnectedDispatchers.Remove(myNotificationDispatcher);
            }
            catch (Exception e)
            {
                _LockerConnectedDispatchers.ExitWriteLock();
                throw e;
            }

            _LockerConnectedDispatchers.ExitWriteLock();

        }

        #endregion

        #region Start/Stop Bridge

                /// <summary>
        /// Connect a NotificationBridge with this NotificationDispatcher. There could be only one bridge per dispatcher.
        /// But you can always change the port with this method. The default port is 55555
        /// </summary>
        public void StartBridge()
        {
            StartBridge(_NotificationSettings.BridgePort);
        }

        /// <summary>
        /// Connect a NotificationBridge with this NotificationDispatcher. There could be only one bridge per dispatcher.
        /// But you can always change the port with this method.
        /// </summary>
        /// <param name="myPort">The Port on which the Multicastmessages will be send</param>
        public void StartBridge(Int32 myPort)
        {

            if (_Bridge != null)
                throw new NotificationException_BridgeAlreadyConnected("You must disconnect the bridge before connecting a new one!");

            if (_BridgeSendThreadPool == null)
            {
                _BridgeSendThreadPool = new GraphThreadPool("BridgeSendThreadPool <" + "__" + ">");
                //_BridgeSendThreadPool       = new SmartThreadPool(1000, 2, Environment.ProcessorCount);
                _BridgeSendThreadPool.OnWorkerThreadException += new WorkerThreadExceptionHandler(ThreadPool_OnWorkerThreadException);
            }

            _Bridge = new NotificationBridge(this, myPort);
            _Bridge.MulticastTTL = _NotificationSettings.BridgeMulticastTTL;


        }

        /// <summary>
        /// Disconnect the Bridge from the Dispatcher. All notification will send only to all recipients of this dispatcher and
        /// all connected dispatchers.
        /// </summary>
        public void StopBridge()
        {

            _Bridge.Close();
            _Bridge = null;

        }


        #endregion

        #region (Un)RegisterRecipient
        /// <summary>
        /// Subscribe to a specific NotificationType as a recipient (client).
        /// The client must implement INotification.
        /// </summary>
        /// <param name="myNotificationTypeName">The Type name of the NotificationType for which you subscribe.</param>
        /// <param name="myClassReference">The reference to the subscriber client. The method HandleNotification will be invoked if a notification appears.</param>
        public void RegisterRecipient(String myNotificationTypeName, INotification myClassReference)
        {
            // finds the NotificationType according to the given Name
            

            ANotificationType _type = NotificationTypesNamesLookupTable[myNotificationTypeName];

            if (_type != null)
                RegisterRecipient(_type, myClassReference);
            else
                throw new NotificationException_InvalidNotificationType("Notificationtype " + myNotificationTypeName + " not found!");
        }


        /// <summary>
        /// Subscribe to a specific NotificationType as a recipient (client).
        /// The client must implement INotification.
        /// </summary>
        /// <param name="myNotificationType">The NotificationType for which you subscribe.</param>
        /// <param name="myClassReference">The reference to the subscriber client. The method HandleNotification will be invoked if a notification appears.</param>
        public void RegisterRecipient(ANotificationType myNotificationType, INotification myClassReference)
        {

            if (myNotificationType == null || myClassReference == null)
                throw new ArgumentNullException();

            Type currentNotificationType = myNotificationType.GetType();
            String guid = Guid.NewGuid().ToString();
            RecipientsEntry nnt = new RecipientsEntry(myClassReference, myNotificationType);

            _Locker.EnterWriteLock();

            try
            {
                // did we already registered this type
                if (!_Recipients.ContainsKey(currentNotificationType))
                {
                    _Recipients.Add(currentNotificationType, new List<RecipientsEntry>());
                    if (_Bridge != null)
                        _Bridge.AddBridgeTypeConnection(myNotificationType);
                }
                else
                {
                    // check whether or not this Class is already registered for this type
                    if (_Recipients[currentNotificationType].Contains(nnt))
                        throw new Exception("This class is already registered for a Notification of type " + myNotificationType.GetType().FullBaseName<ANotificationType>());
                }
                _Recipients[currentNotificationType].Add(nnt);
            }
            catch (Exception e)
            {
                _Locker.ExitWriteLock();
                throw e;
            }

            _Locker.ExitWriteLock();

        }

        /// <summary>
        /// Checks if there's already a Subscription for this NotificationType, without checking for recipient match
        /// </summary>
        /// <param name="myNotificationTypeName">the Name of the NotificationType</param>
        /// <returns>true if subscribed, false if not subscribes</returns>
        public Boolean Subscribed(String myNotificationTypeName, INotification myClassReference)
        {
            // finds the NotificationType according to the given Name

            ANotificationType _type = NotificationTypesNamesLookupTable[myNotificationTypeName];

            if (_type != null)
                return Subscribed(_type, myClassReference);
            else
                throw new NotificationException_InvalidNotificationType("Notificationtype " + myNotificationTypeName + " not found!");
        }
        
        /// <summary>
        /// Checks if there's already a Subscription for this NotificationType, without checking for recipient match
        /// </summary>
        /// <param name="myNotificationTypeName">the Name of the NotificationType</param>
        /// <returns>true if subscribed, false if not subscribes</returns>
        public Boolean Subscribed(ANotificationType myNotificationType, INotification myClassReference)
        {

            if (_Recipients.ContainsKey(myNotificationType.GetType()))
                    return _Recipients[myNotificationType.GetType()].Contains(new RecipientsEntry(myClassReference, myNotificationType));

                /*
            foreach (Type type in _Recipients.Keys)
            {
                //if (myNotificationType.GetType().FullBaseName<ANotificationType>().StartsWith(type.FullBaseName<ANotificationType>()))
                if (myNotificationType.GetType().FullBaseName<ANotificationType>() == type.FullBaseName<ANotificationType>())
                {
                }
            }
                 * */
            return false;

        }

        ///// <summary>
        ///// Check whether or not the _Recipients dictionary contains any parent type of myNotificationType and subscribed to
        ///// </summary>
        ///// <param name="myNotificationType">The Type</param>
        ///// <returns>True, if the is already a parent existing</returns>
        //private Boolean RecipientsContains(ANotificationType myNotificationType, RecipientsEntry myRecipientsEntry, ref Type myParentType)
        //{
        //    foreach (Type type in _Recipients.Keys)
        //    {
        //        if (myNotificationType.GetType().FullBaseName<ANotificationType>().StartsWith(type.FullBaseName<ANotificationType>())
        //            && _Recipients[type].Contains(myRecipientsEntry))
        //        {
        //            myParentType = type;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// Check whether or not the _Recipients dictionary contains any type which are childs from the myParentNotificationType
        ///// </summary>
        ///// <param name="myParentNotificationType">The parent Type</param>
        ///// <returns>True, if the is already a parent existing</returns>
        //private Boolean RecipientsArePartOf(ANotificationType myParentNotificationType)
        //{
        //    foreach (Type type in _Recipients.Keys)
        //    {
        //        if (type.GetType().FullBaseName<ANotificationType>().StartsWith(myParentNotificationType.FullBaseName<ANotificationType>()))
        //            return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Unregister a subscribed client for a particular NotificationType
        /// </summary>
        /// <param name="myNotificationTypeName">The NotificationTypeName you want to unsubscribe</param>
        /// <param name="myClassReference">The reference to the subscriber client.</param>
        public void UnRegisterRecipient(String myNotificationTypeName, INotification myClassReference)
        {
            // finds the NotificationType according to the given Name
            ANotificationType _type = NotificationTypesNamesLookupTable[myNotificationTypeName];

            if (_type != null)
                UnRegisterRecipient(_type, myClassReference);
            else
                throw new NotificationException_InvalidNotificationType("Notificationtype " + myNotificationTypeName + " not found!");

        }

        /// <summary>
        /// Unregister a subscribed client for a particular NotificationType
        /// </summary>
        /// <param name="myNotificationType">The NotificationType you want to unsubscribe</param>
        /// <param name="myClassReference">The reference to the subscriber client.</param>
        public void UnRegisterRecipient(ANotificationType myNotificationType, INotification myClassReference)
        {

            if (myNotificationType == null || myClassReference == null)
                throw new ArgumentNullException();

            RecipientsEntry nnt = new RecipientsEntry(myClassReference, myNotificationType);

            _Locker.EnterWriteLock();

            try
            {
                Type currentNotificationType = myNotificationType.GetType();
                if (_Recipients.ContainsKey(currentNotificationType))
                {
                    // find the NotificationType for which this recipient has subscribed
                    if (_Recipients[currentNotificationType].Contains(nnt))
                    {
                        _Recipients[currentNotificationType].Remove(nnt);
                        if (_Bridge != null)
                            _Bridge.RemoveBridgeTypeConnection(myNotificationType);

                        if (_Recipients[currentNotificationType].Count == 0)
                            _Recipients.Remove(currentNotificationType);
                    }
                }
            }
            catch (Exception e)
            {
                _Locker.ExitWriteLock();
                throw e;
            }

            _Locker.ExitWriteLock();

        }

        #endregion

        #region SendNotification

        #region Method overrides

        /// <summary>
        /// Sends a Notification with PriorityTypes.Normal and Multicast send
        /// </summary>
        /// <param name="myNotificationType">The type of the notification</param>
        /// <param name="myNotificationArguments">The typed arguments </param>
        public void SendNotification(Type myNotificationType, INotificationArguments myNotificationArguments)
        {
            SendNotification(myNotificationType, myNotificationArguments, NotificationPriority.Normal);
        }

        /// <summary>
        /// Sends a Notification with Multicast send
        /// </summary>
        /// <param name="myNotificationType">The type of the notification</param>
        /// <param name="myNotificationArguments">The typed arguments </param>
        /// <param name="myPriority">The priority </param>
        public void SendNotification(Type myNotificationType, INotificationArguments myNotificationArguments, NotificationPriority myPriority)
        {
            SendNotification(myNotificationType, myNotificationArguments, myPriority, true);
        }

        /// <summary>
        /// Sends a Notification
        /// </summary>
        /// <param name="myNotificationType">The type of the notification</param>
        /// <param name="myNotificationArguments">The typed arguments </param>
        /// <param name="myPriority">The priority </param>
        /// <param name="mySendMulticast">Set to False, to not send the notification via the bridge (in addition to the usual way)</param>
        public void SendNotification(Type myNotificationType, INotificationArguments myNotificationArguments, NotificationPriority myPriority, Boolean mySendMulticast)
        {

            if (myNotificationType == null || myNotificationArguments == null)
                throw new ArgumentNullException();

            var message = new NotificationMessage(myPriority, (Int64) TimestampNonce.Ticks, myNotificationArguments, myNotificationType);

            SendNotification(message, mySendMulticast);

        }

        #endregion

        #region ResolveType
        public Type ResolvedNotificationTypeByName(String myNotificationTypeName)
        {
            return _NotificationTypesLookupTable[myNotificationTypeName];          
        }
        #endregion

        /// <summary>
        /// Sends a Notification
        /// </summary>
        /// <param name="myNotificationType">The type of the notification</param>
        /// <param name="myNotificationArguments">The typed arguments </param>
        /// <param name="myPriority">A priority</param>
        public void SendNotification(NotificationMessage message, Boolean mySendMulticast)
        {

            if (message == null)
                throw new ArgumentNullException("The NotificationMessage must not be null!");

            // The Dispatcher is not startet, so do not handle any notifications
            if (!_IsStarted || _IsSuspended || _IsStopRequested) return;

            // avoid cycles from connected dispatchers
            if (message.IsHandledByDispatcher(_Uuid)) return;

            message.HandleByDispatcher(_Uuid);

            _Locker.EnterReadLock();

            try
            {
                // Do we have any recipients?
                if (_Recipients.Count > 0)
                {

                    Type curType = ResolvedNotificationTypeByName(message.NotificationType);
                    // find the first type for which someone has subscribed (could be the NotificationType itself or some parent TypeGroup like NObjectCache....
                    while (curType != null && curType.BaseType != typeof(ANotificationType) && !_Recipients.ContainsKey(curType))
                        curType = curType.BaseType;

                    if (curType == null)
                        throw new NotificationException_InvalidNotificationType("No valid notification type!");

                    // send message for current type and all parent TypeGroups until ANotificationType is reached
                    while (curType != null && curType != typeof(ANotificationType))
                    {
                        // It could be, that the current type has no subscriber but maybe any of the parent TypeGroups
                        if (!_Recipients.ContainsKey(curType))
                        {
                            curType = curType.BaseType;
                            continue;
                        }

                        // send notification to each subscribed recipient
                        foreach (var _RecipientsEntry in _Recipients[curType])
                        {
                            // execute the validate method of this notification type
                            if (_RecipientsEntry.ANotificationType.Validate(message.Arguments))
                            {
                                lock (_MessageQueueThreadLock)
                                {
                                    // check if this message was already added by any parent or child notificationtype
                                    //if (_MessageQueue.ContainsKey(message))
                                    if (_MessageQueues.ContainsKey(message.Priority) && _MessageQueues[message.Priority].ContainsKey(message))
                                    {
                                        // the message exist and has the same handler
                                        if (_MessageQueues[message.Priority][message] == _RecipientsEntry.INotification)
                                            continue;
                                        // the message already exist but has a nother handler INotification reference
                                        else
                                            message = message.Copy();
                                    }

                                    //_MessageQueue.Add(new MessageQueueEntry(message, nnt.INotification));
                                    if (!_MessageQueues.ContainsKey(message.Priority))
                                        _MessageQueues.Add(message.Priority, new SortedList<NotificationMessage, INotification>());

                                    _MessageQueues[message.Priority].Add(message, _RecipientsEntry.INotification);
                                    Interlocked.Increment(ref _TotalMessageQueueEntries);

                                    //System.Diagnostics.Debug.WriteLine("[NotificationDispatcher] adding " + message.NotificationType.Name + " at pos " + _MessageQueues[message.Priority].Count);

                                    Monitor.Pulse(_MessageQueueThreadLock);
                                }

                            }
                        }
                        curType = curType.BaseType;
                    }

                }
            }
            catch (Exception e)
            {
                _Locker.ExitReadLock();
                throw e;
            }

            _Locker.ExitReadLock();

            #region Redirect message to connected dispatchers

            _LockerConnectedDispatchers.EnterReadLock();

            try
            {
                foreach (var _NotificationDispatcher in _ConnectedDispatchers)
                {
                    _NotificationDispatcher.SendNotification(message, false);
                }
            }
            catch (Exception e)
            {
                _LockerConnectedDispatchers.ExitReadLock();
                throw e;
            }

            _LockerConnectedDispatchers.ExitReadLock();

            #endregion

            #region Send Message via NotificationBridge

            if (mySendMulticast && _Bridge != null && NotificationTypesNamesLookupTable.ContainsKey(message.NotificationType))
            {
                var sendTupel = new StefanTuple<NotificationMessage, ANotificationType>(message, NotificationTypesNamesLookupTable[message.NotificationType]);
                _BridgeSendThreadPool.QueueWorkItem(new GraphThreadPool.ThreadPoolEntry(new WaitCallback(_Bridge.Send), sendTupel));
                //_BridgeSendThreadPool.QueueWorkItem(new Action<Object>(_Bridge.Send), sendTupel);
                //Console.WriteLine("Send multicast!");
            }
            
            #endregion

        }


        #endregion

        #region SendNotification thread

        private void SendNotificationCallback(Object state)
        {

            MessageQueueEntry currentMessageEntry = ((MessageQueueEntry)state);
            var currentMessage = currentMessageEntry.Message;

            var result = currentMessageEntry.NotificationReference.HandleNotification(currentMessage);

            if (!result && (currentMessage.Retries < MAX_RETRIES_HandleNotification))
            {
                lock (_MessageQueueThreadLock)
                {
                    currentMessage.Retries++;
                    
                    if (!_MessageQueues.ContainsKey(currentMessageEntry.Message.Priority))
                        _MessageQueues.Add(currentMessageEntry.Message.Priority, new SortedList<NotificationMessage, INotification>());
                    
                    _MessageQueues[currentMessageEntry.Message.Priority].Add(currentMessageEntry.Message, currentMessageEntry.NotificationReference);
                    Interlocked.Increment(ref _TotalMessageQueueEntries);
                    
                    //System.Diagnostics.Debug.WriteLine("[NotificationDispatcher] HandleNotification failed "+currentMessage.Retries+" times! Adding " + currentMessage.NotificationType.Name + " at pos " + _MessageQueues.Count + " again");
                    Monitor.Pulse(_MessageQueueThreadLock);
                }
            }

        }

        
        private void SendMessageThread()
        {


            _IsStarted = true;

            byte lastPriority = 0;
 
            while (true)
            {

                Monitor.Enter(_MessageQueueThreadLock);

                try
                {
                    #region Wait for messages

                    // only suspend until all messages are sent
                    while (!_IsStopRequested && ((_TotalMessageQueueEntries == 0) || (_TotalMessageQueueEntries == 0 && _IsSuspended)))
                    {
                        lastPriority = 0;
                        Monitor.Wait(_MessageQueueThreadLock);
                    }
                    if ((_IsStopRequested && _TotalMessageQueueEntries == 0) || _Disposed) break;

                    #endregion

                    #region Find the next best queue with the highest priority or the next highest priority if the threshold was reched

                    // find a priority for which there is at least one item in the queue
                    // Since we start always with priority 0 we switch to a lower priority only in the case, that the threshold is reached.
                    while (true)
                    {

                        // the threshold is reached, reset counter and switch to the next priority
                        if (_SentPriorities[lastPriority] >= _NotificationSettings.GetNumberOfSentPrioritiesThreshold((NotificationPriority)lastPriority))
                        {
                            _SentPriorities[lastPriority] = 0;
                        }
                        else
                        {
                            // if we found a message for this priority, than proceed
                            if (_MessageQueues.ContainsKey((NotificationPriority)lastPriority) && _MessageQueues[(NotificationPriority)lastPriority].Count != 0)
                                break;
                        }

                        lastPriority = (byte)((byte)(lastPriority + 1) % (byte)(_LowestPriority + 1));
                    }

                    _SentPriorities[lastPriority]++;

                    #endregion

                    #region Get the message and invoke the delegate

                    //MessageQueueEntry currentMessageEntry = (MessageQueueEntry)_MessageQueue[0];
                    MessageQueueEntry currentMessageEntry = new MessageQueueEntry(_MessageQueues[(NotificationPriority)lastPriority].ElementAt(0).Key, _MessageQueues[(NotificationPriority)lastPriority].ElementAt(0).Value);

                    //String deb = "";
                    //for (byte i=0; i <= 4; i++)
                    //    deb += " sp: " + _SentPriorities[i] + " mq: " + ((_MessageQueues.ContainsKey((PriorityTypes)i) ? _MessageQueues[(PriorityTypes)i].Count.ToString() : "\t"));
                    //System.Diagnostics.Debug.WriteLine("QueueWorkItem prio: " + currentMessageEntry.Message.Priority + deb);

                    Boolean addedSuccessfully = _SendMessageThreadPool.QueueWorkItem(new GraphThreadPool.ThreadPoolEntry(new ParameterizedThreadStart(SendNotificationCallback), currentMessageEntry));
                    //Boolean addedSuccessfully = true;
                    //_SendMessageThreadPool.QueueWorkItem(new Action<Object>(SendNotificationCallback), currentMessageEntry);

                    if (addedSuccessfully)
                    {
                        // TODO: Retry only a couple of time, than discard the message.
                        //_MessageQueue.Remove(currentMessageEntry);
                        if (_MessageQueues[(NotificationPriority)lastPriority].Remove(currentMessageEntry.Message))
                            Interlocked.Decrement(ref _TotalMessageQueueEntries);

                        if (_MessageQueues[(NotificationPriority)lastPriority].Count == 0)
                            _MessageQueues.Remove((NotificationPriority)lastPriority);

                        lastPriority = 0;
                    }
                    else
                    {
                        _MessageQueues[(NotificationPriority)lastPriority].Remove(currentMessageEntry.Message);

                        currentMessageEntry.Message.Retries++;

                        if (currentMessageEntry.Message.Retries < MAX_RETRIES_HandleNotification)
                            _MessageQueues[(NotificationPriority)lastPriority].Add(currentMessageEntry.Message, currentMessageEntry.NotificationReference);
                    }

                    #endregion
                }
                catch (Exception e)
                {
                    Monitor.Exit(_MessageQueueThreadLock);
                    throw e;
                }

                Monitor.Exit(_MessageQueueThreadLock);

            }

        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            if (_Disposed) return;

            _IsStopRequested = true;

            if (_Bridge != null)
                StopBridge();

            lock (_MessageQueueThreadLock)
            {
                Monitor.Pulse(_MessageQueueThreadLock);
            }

            if (_BridgeSendThreadPool != null)
                _BridgeSendThreadPool.Dispose();

            if (_SendMessageThreadPool != null)
                _SendMessageThreadPool.Dispose();

            _Disposed = true;
        }

        #endregion

    }
}
