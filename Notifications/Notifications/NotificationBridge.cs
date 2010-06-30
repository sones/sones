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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

using sones.Lib;
using sones.Networking.UDPSocket;
using sones.Lib.NewFastSerializer;
using sones.Notifications.Exceptions;
using sones.Notifications.Messages;
using sones.Notifications.NotificationTypes;

namespace sones.Notifications
{
    public class NotificationBridge
    {

        #region Fields

        private ReaderWriterLockSlim _LockerBridgeTypeSockets = null;
        private Int32 _SenderPort;
        private Dictionary<BrigeSocketStruct, UDPSocketMulticast<NotificationBridgeConnection>> _BrigeTypeSockets;
        private Dictionary<BrigeSocketStruct, List<Type>> _BrigeSockets_NotificationTypes;
        private String _Hostname = Dns.GetHostName();

        #endregion

        #region Properties

        private NotificationDispatcher _Dispatcher;
        public NotificationDispatcher Dispatcher
        {
            get
            {
                return _Dispatcher;
            }
        }

        public Int32 MulticastTTL
        {
            get { return _MulticastTTL; }
            set { _MulticastTTL = value; }
        }
        private Int32 _MulticastTTL = 2;

        #endregion

        #region Structs

        /// <summary>
        /// This struct is needed for the key of _BrigeTypeSockets dictionary
        /// </summary>
        private struct BrigeSocketStruct : IComparable<BrigeSocketStruct>
        {
            //public Type NotificationType;
            public IPAddress IPAddress;
            public Int32 Port;

            public BrigeSocketStruct(IPAddress myIPAddress, Int32 myPort)
            {
                //NotificationType = myNotificationType;
                IPAddress = myIPAddress;
                Port = myPort;
            }

            #region IComparable<BrigeTypeStruct> Members

            public int CompareTo(BrigeSocketStruct other)
            {
                if (IPAddress.Equals(other.IPAddress) && Port == other.Port)
                    return 0;

                return -1;
            }

            public override bool Equals(object obj)
            {
                return CompareTo((BrigeSocketStruct)obj) == 0;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            #endregion

        }

        #endregion

        #region Constructors

        public NotificationBridge(NotificationDispatcher myDispatcher, Int32 mySenderPort)
        {

            if (mySenderPort < 0 || mySenderPort > 65535)
                throw new NotificationException_InvalidMulticastPort(mySenderPort + " is not a valid port (must between 0 and )");

            _Dispatcher = myDispatcher;
            _SenderPort = mySenderPort;
            _BrigeTypeSockets = new Dictionary<BrigeSocketStruct, UDPSocketMulticast<NotificationBridgeConnection>>();
            _BrigeSockets_NotificationTypes = new Dictionary<BrigeSocketStruct, List<Type>>();
            _LockerBridgeTypeSockets = new ReaderWriterLockSlim();

        }

        #endregion

        #region Add/Remove BridgeTypeConnection

        /// <summary>
        /// Add a new UDP multicast listener for this NotificationType
        /// </summary>
        /// <param name="myNotificationType"></param>
        public void AddBridgeTypeConnection(ANotificationType myNotificationType)
        {

            if (myNotificationType == null)
                throw new ArgumentNullException();

            BrigeSocketStruct BrigeTypeStruct = new BrigeSocketStruct(myNotificationType.MulticastIPAddress, myNotificationType.MulticastPort);
            
            _LockerBridgeTypeSockets.EnterUpgradeableReadLock();

            if (!_BrigeTypeSockets.ContainsKey(BrigeTypeStruct))
            {
                UDPSocketMulticast<NotificationBridgeConnection> UDPSocketMulticast = new UDPSocketMulticast<NotificationBridgeConnection>(myNotificationType.MulticastIPAddress, myNotificationType.MulticastPort);
                UDPSocketMulticast.MulticastTTL = _MulticastTTL;
                UDPSocketMulticast.SetStateObject(this);
                UDPSocketMulticast.OnExceptionOccured += new UDPSocketExceptionOccuredHandler(UDPSocketMulticast_OnExceptionOccured);
                UDPSocketMulticast.StartListener();

                // wait until listener thread is spawned
                while (!UDPSocketMulticast.IsRunning)
                    Thread.Sleep(1);

                _LockerBridgeTypeSockets.EnterWriteLock();

                _BrigeTypeSockets.Add(BrigeTypeStruct, UDPSocketMulticast);

                if (!_BrigeSockets_NotificationTypes.ContainsKey(BrigeTypeStruct))
                    _BrigeSockets_NotificationTypes.Add(BrigeTypeStruct, new List<Type>());
                _BrigeSockets_NotificationTypes[BrigeTypeStruct].Add(myNotificationType.GetType());

                _LockerBridgeTypeSockets.ExitWriteLock();
            }
            else
            {

                _LockerBridgeTypeSockets.EnterWriteLock();

                _BrigeSockets_NotificationTypes[BrigeTypeStruct].Add(myNotificationType.GetType());
                
                _LockerBridgeTypeSockets.ExitWriteLock();

            }
            /*
            else
            {
                _LockerBridgeTypeSockets.ExitUpgradeableReadLock();

                throw new Exception("There is already NotificationType registered for this bridge (" + myNotificationType.MulticastIPAddress + ":" + myNotificationType.MulticastPort + ")");
            }*/

            _LockerBridgeTypeSockets.ExitUpgradeableReadLock();
            
        }

        void UDPSocketMulticast_OnExceptionOccured(object mySender, Exception myException)
        {
            System.Diagnostics.Debug.WriteLine("[NotificationBridge] " + mySender.ToString() + " Exception: " + myException.ToString());
        }

        /// <summary>
        /// Remove an existing UDP multicast listener for this NotificationType
        /// </summary>
        /// <param name="myNotificationType"></param>
        public void RemoveBridgeTypeConnection(ANotificationType myNotificationType)
        {

            if (myNotificationType == null)
                throw new ArgumentNullException();

            BrigeSocketStruct BrigeTypeStruct = new BrigeSocketStruct(myNotificationType.MulticastIPAddress, myNotificationType.MulticastPort);

            _LockerBridgeTypeSockets.EnterUpgradeableReadLock();

            if (_BrigeTypeSockets.ContainsKey(BrigeTypeStruct))
            {

                _BrigeTypeSockets[BrigeTypeStruct].StopListener();

                _LockerBridgeTypeSockets.EnterWriteLock();

                #region Remove the type from Socket list or close the socket

                // remove this type for this BridgeSocket
                _BrigeSockets_NotificationTypes[BrigeTypeStruct].Remove(myNotificationType.GetType());

                // if the socketList is empty now, we can stop the listener and remove the socket
                if (_BrigeSockets_NotificationTypes[BrigeTypeStruct].Count == 0)
                {
                    _BrigeTypeSockets[BrigeTypeStruct].StopListener();
                    _BrigeTypeSockets.Remove(BrigeTypeStruct);
                    _BrigeSockets_NotificationTypes.Remove(BrigeTypeStruct);
                }

                #endregion


                _LockerBridgeTypeSockets.ExitWriteLock();

            }

            _LockerBridgeTypeSockets.ExitUpgradeableReadLock();

        }
        
        public void ChangeBridgeTypeConnection(ANotificationType myOldNotificationType, IPAddress myIPAddress, Int32 myPort)
        {

            if (myOldNotificationType == null || myIPAddress == null)
                throw new ArgumentNullException();

            BrigeSocketStruct oldBrigeTypeStruct = new BrigeSocketStruct(myOldNotificationType.MulticastIPAddress, myOldNotificationType.MulticastPort);

            _LockerBridgeTypeSockets.EnterUpgradeableReadLock();

            if (_BrigeTypeSockets.ContainsKey(oldBrigeTypeStruct))
            {

                BrigeSocketStruct newBrigeTypeStruct = new BrigeSocketStruct(myIPAddress, myPort);
                
                #region _LockerBridgeTypeSockets.EnterWriteLock();

                _LockerBridgeTypeSockets.EnterWriteLock();

                #region Remove the type from Socket list or close the socket

                // remove this type for this BridgeSocket
                _BrigeSockets_NotificationTypes[oldBrigeTypeStruct].Remove(myOldNotificationType.GetType());

                // if the socketList is empty now, we can stop the listener and remove the socket
                if (_BrigeSockets_NotificationTypes[oldBrigeTypeStruct].Count == 0)
                {
                    _BrigeTypeSockets[oldBrigeTypeStruct].StopListener();
                    _BrigeTypeSockets.Remove(oldBrigeTypeStruct);
                    _BrigeSockets_NotificationTypes.Remove(oldBrigeTypeStruct);
                }

                #endregion

                #region Add a new socket group if not already exist

                // If the Multicast group does not exist already
                if (!_BrigeTypeSockets.ContainsKey(newBrigeTypeStruct))
                {

                    UDPSocketMulticast<NotificationBridgeConnection> UDPSocketMulticast = new UDPSocketMulticast<NotificationBridgeConnection>(newBrigeTypeStruct.IPAddress, newBrigeTypeStruct.Port);
                    UDPSocketMulticast.MulticastTTL = _MulticastTTL;
                    UDPSocketMulticast.SetStateObject(this);
                    UDPSocketMulticast.StartListener();

                    _BrigeTypeSockets.Add(newBrigeTypeStruct, UDPSocketMulticast);

                    if (!_BrigeSockets_NotificationTypes.ContainsKey(newBrigeTypeStruct))
                        _BrigeSockets_NotificationTypes.Add(newBrigeTypeStruct, new List<Type>());
                }

                #endregion

                // add the type to the socket list
                _BrigeSockets_NotificationTypes[newBrigeTypeStruct].Add(myOldNotificationType.GetType());

                _LockerBridgeTypeSockets.ExitWriteLock();

                #endregion

            }

            _LockerBridgeTypeSockets.ExitUpgradeableReadLock();

        }
        
        #endregion

        #region Send multicast message

        /// <summary>
        /// Send an Object of type NotificationMessage
        /// </summary>
        /// <param name="myNotificationMessage"></param>
        public void Send(Object myNotificationMessage)
        {

            if (myNotificationMessage == null)
                throw new ArgumentNullException();

            if (!(myNotificationMessage is StefanTuple<NotificationMessage, ANotificationType>))
                throw new ArgumentException("Argument is not of type NotificationMessage");

            var sendTupel = (StefanTuple<NotificationMessage, ANotificationType>)myNotificationMessage;
            NotificationMessage message = sendTupel.Item1;
            ANotificationType notificationType = sendTupel.Item2;

            UDPSocketMulticast<Object> UDPSocketMulticast = new UDPSocketMulticast<object>(IPAddress.Any, _SenderPort);
            byte[] bytesToSend = CreateMulticastMessage(message);
            /*
            System.IO.Compression.CompressionMode comprMode = System.IO.Compression.CompressionMode.Compress;
            MemoryStream memStream = new MemoryStream();

            System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(memStream, comprMode, true);
            gzipStream.Write(bytesToSend, 0, bytesToSend.Length);
            gzipStream.Close();

            Array.C

            memStream.Position = 0;

            byte[] comprBytes = new byte[memStream.Length];
            memStream.Read(comprBytes, 0, comprBytes.Length);
            memStream.Close();

            memStream = new MemoryStream(comprBytes);
            gzipStream = new System.IO.Compression.GZipStream(memStream, System.IO.Compression.CompressionMode.Decompress, true);
            gzipStream.Read(bytesToSend, 0, bytesToSend.Length);
            */
            UDPSocketMulticast.MulticastTTL = _MulticastTTL;
            UDPSocketMulticast.Send(notificationType.MulticastIPAddress, notificationType.MulticastPort, bytesToSend);

        }

        /// <summary>
        /// Create the Multicast message containing a header and the body with the NotificationMessage
        /// </summary>
        /// <param name="myNotificationMessage"></param>
        /// <returns></returns>
        private Byte[] CreateMulticastMessage(NotificationMessage myNotificationMessage)
        {

            if (myNotificationMessage == null)
                throw new ArgumentNullException();

            StringBuilder HeaderString = new StringBuilder();
            HeaderString.Append("Dispatcher: "); HeaderString.AppendLine(_Dispatcher.Uuid);
            HeaderString.Append("NotificationType: "); HeaderString.AppendLine(myNotificationMessage.NotificationType.GetType().FullBaseName<ANotificationType>());

            Byte[] serializedMessage = myNotificationMessage.Serialize();

            SerializationWriter writer = new SerializationWriter();
            writer.WriteObject("Host: " + _Hostname + Environment.NewLine);
            writer.WriteObject("SenderID: " + _Dispatcher.SenderID.ToString() + Environment.NewLine);
            foreach(String disp in myNotificationMessage.HandledDispatchers)
                writer.WriteObject("Dispatcher: " + disp + Environment.NewLine);

            writer.WriteObject("NotificationType: " + myNotificationMessage.NotificationType + Environment.NewLine);
            writer.WriteObject("ContentLength: " + serializedMessage.ToArray().Length);
            writer.WriteObject(Environment.NewLine);
            writer.WriteObject(serializedMessage);
            
            Byte[] body = writer.ToArray();
            return body;
        }

        #endregion

        #region Close

        /// <summary>
        /// Close the bridge and stop all BridgeSocket listeners
        /// </summary>
        public void Close()
        {

            _LockerBridgeTypeSockets.EnterWriteLock();
            
            foreach (KeyValuePair<BrigeSocketStruct,  UDPSocketMulticast<NotificationBridgeConnection>> keyValPair in _BrigeTypeSockets)
            {
                keyValPair.Value.StopListener();
            }

            _LockerBridgeTypeSockets.ExitWriteLock();

        }

        #endregion

    }
}
