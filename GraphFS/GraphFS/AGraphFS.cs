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
/*
 * AGraphFS
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using sones.GraphFS;
using sones.GraphFS.Events;
using sones.GraphFS.Caches;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Errors;
using sones.GraphFS.Exceptions;
using sones.GraphFS.InternalObjects;
using sones.GraphFS.Notification;
using sones.GraphFS.Objects;
using sones.GraphFS.Transactions;

using sones.Lib;
using sones.Lib.Session;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Indices;

using sones.Notifications;

using sones.StorageEngines;

#endregion

namespace sones
{

    /// <summary>
    /// This is an abstract implementation of the IGraphFS interface and
    /// many other file system will extend this implementation.
    /// </summary>

    public abstract class AGraphFS : IGraphFS
    {


        #region Data

        protected           ForestUUID              _ForestUUID;
        protected           String                  _ChangedRootDirectoryPrefix;
        protected readonly  Regex                   _MoreThanOnePathSeperatorRegExpr;
        protected readonly  MountpointLookup        _GraphFSLookuptable;
        protected const     UInt64                  NUMBER_OF_DEFAULT_DIRECTORYENTRIES = 6;
        protected readonly  IObjectCache            _ObjectCache;

        #endregion

        #region Properties

        #region FileSystemDescription

        public String FileSystemDescription { get; set; }

        #endregion

        #region NotificationSettings

        public NotificationSettings NotificationSettings { get; set; }

        #endregion

        #region NotificationDispatcher

        public NotificationDispatcher NotificationDispatcher { get; set; }

        #endregion

        #endregion

        #region Events

        #region OnExceptionOccurred(myEventArgs)

        public event ExceptionOccuredEvent OnExceptionOccurred;

        protected void OnExceptionOccured(Object mySender, ExceptionOccuredEventArgs myEventArgs)
        {
            Debug.WriteLine("[!CRITICAL Exception][" + mySender.GetType().FullName + "] " + myEventArgs.Exception.Message + Environment.NewLine + myEventArgs.Exception.StackTrace);
            // if (OnExceptionOccurred != null)
            OnExceptionOccurred(mySender, myEventArgs);
        }

        #endregion


        #region AFSObject handling

        #region OnLoad/OnLoadEvent(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID)

        /// <summary>
        /// An event to be notified whenever an AFSObject
        /// is ready to be loaded.
        /// </summary>
        public event GraphFSEventHandlers.OnLoadEventHandler OnLoad;

        /// <summary>
        /// Invoke the OnLoad event, called whenever an AFSObject
        /// is ready to be loaded.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnLoadEvent(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID)
        {
            if (OnLoad != null)
                OnLoad(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID);
        }

        #endregion

        #region OnLoaded/OnLoadedEvent(myObjectLocator, myAFSObject)

        /// <summary>
        /// An event to be notified whenever an AFSObject
        /// was successfully loaded.
        /// </summary>
        public event GraphFSEventHandlers.OnLoadedEventHandler OnLoaded;

        /// <summary>
        /// Invoke the OnLoaded event, called whenever an AFSObject
        /// was successfully loaded.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnLoadedEvent(ObjectLocator myObjectLocator, AFSObject myAFSObject)
        {
            if (OnLoaded != null)
                OnLoaded(myObjectLocator, myAFSObject);
        }

        #endregion

        #region OnLoadedAsync/OnLoadedAsyncEvent(myObjectLocator, myAFSObject)

        /// <summary>
        /// An event to be notified asynchronously whenever an
        /// AFSObject was successfully loaded.
        /// </summary>
        public event GraphFSEventHandlers.OnLoadedAsyncEventHandler OnLoadedAsync;

        /// <summary>
        /// Invoke the OnLoadedAsync event, called whenever an
        /// AFSObject was successfully loaded.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnLoadedAsyncEvent(ObjectLocator myObjectLocator, AFSObject myAFSObject)
        {
            if (OnLoadedAsync != null)
                OnLoadedAsync(myObjectLocator, myAFSObject);
        }

        #endregion


        #region OnSave/OnSaveEvent(myObjectLocation, myObjectStream, myObjectEdition)

        /// <summary>
        /// An event to be notified whenever an AFSObject
        /// is ready to be saved.
        /// </summary>
        public event GraphFSEventHandlers.OnSaveEventHandler OnSave;

        /// <summary>
        /// Invoke the OnSave event, called whenever an AFSObject
        /// is ready to be saved.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnSaveEvent(ObjectLocation myObjectLocation, AFSObject myAFSObject)
        {
            if (OnSave != null)
                OnSave(myObjectLocation, myAFSObject);
        }

        #endregion

        #region OnSaved/OnSavedEvent(myObjectLocator, myAFSObject, myOldObjectRevisionID)

        /// <summary>
        /// An event to be notified whenever an AFSObject
        /// was successfully saved on disc.
        /// </summary>
        public event GraphFSEventHandlers.OnSavedEventHandler OnSaved;

        /// <summary>
        /// Invoke the OnSaved event, called whenever an AFSObject
        /// was successfully saved on disc.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnSavedEvent(ObjectLocator myObjectLocator, AFSObject myAFSObject, ObjectRevisionID myOldObjectRevisionID)
        {
            if (OnSaved != null)
                OnSaved(myObjectLocator, myAFSObject, myOldObjectRevisionID);
        }

        #endregion

        #region OnSavedAsync/OnSavedAsyncEvent(myObjectLocator, myAFSObject, myOldObjectRevisionID)

        /// <summary>
        /// An event to be notified asynchronously whenever an
        /// AFSObject was successfully saved on disc.
        /// </summary>
        public event GraphFSEventHandlers.OnSavedAsyncEventHandler OnSavedAsync;

        /// <summary>
        /// Invoke the OnSavedAsync event, called whenever an
        /// AFSObject was successfully saved on disc.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnSavedAsyncEvent(ObjectLocator myObjectLocator, AFSObject myAFSObject, ObjectRevisionID myOldObjectRevisionID)
        {
            if (OnSavedAsync != null)
                OnSavedAsync(myObjectLocator, myAFSObject, myOldObjectRevisionID);
        }

        #endregion


        #region OnRemove/OnRemoveEvent(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID)

        /// <summary>
        /// An event to be notified whenever an AFSObject
        /// is ready to be removed.
        /// </summary>
        public event GraphFSEventHandlers.OnRemoveEventHandler OnRemove;

        /// <summary>
        /// Invoke the OnSave event, called whenever an AFSObject
        /// is ready to be removed.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnRemoveEvent(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID)
        {
            if (OnRemove != null)
                OnRemove(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID);
        }

        #endregion

        #region OnRemoved/OnRemovedEvent(myObjectLocator, myObjectStream, myObjectEdition, myRevisionID)

        /// <summary>
        /// An event to be notified whenever an AFSObject
        /// was successfully removed.
        /// </summary>
        public event GraphFSEventHandlers.OnRemovedEventHandler OnRemoved;

        /// <summary>
        /// Invoke the OnRemoved event, called whenever an AFSObject
        /// was successfully removed.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnRemovedEvent(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID)
        {
            if (OnRemoved != null)
                OnRemoved(myObjectLocator, myObjectStream, myObjectEdition, myRevisionID);
        }

        #endregion

        #region OnRemovedAsync/OnRemovedAsyncEvent(myObjectLocator, myObjectStream, myObjectEdition, myRevisionID)

        /// <summary>
        /// An event to be notified asynchronously whenever an
        /// AFSObject was successfully removed.
        /// </summary>
        public event GraphFSEventHandlers.OnRemovedAsyncEventHandler OnRemovedAsync;

        /// <summary>
        /// Invoke the OnRemovedAsync event, called whenever
        /// an AFSObject was successfully removed.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnRemovedAsyncEvent(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID)
        {
            if (OnRemovedAsync != null)
                OnRemovedAsync(myObjectLocator, myObjectStream, myObjectEdition, myRevisionID);
        }

        #endregion

        #endregion

        #region Transaction handling

        #region OnTransactionStart/OnTransactionStartEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified whenever a transaction starts.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionStartEventHandler OnTransactionStart;

        /// <summary>
        /// Invoke the OnTransactionStart event, called whenever
        /// transaction starts.
        /// </summary>
        public virtual void OnTransactionStartEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionStart != null)
                OnTransactionStart(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion

        #region OnTransactionStarted/OnTransactionStartedEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified whenever a transaction started.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionStartedEventHandler OnTransactionStarted;

        /// <summary>
        /// Invoke the OnTransactionStart event, called whenever
        /// transaction started.
        /// </summary>
        public virtual void OnTransactionStartedEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionStarted != null)
                OnTransactionStarted(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion

        #region OnTransactionStartedAsync/OnTransactionStartedAsyncEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified asynchronously whenever a
        /// transaction started.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionStartedAsyncEventHandler OnTransactionStartedAsync;

        /// <summary>
        /// Invoke the OnTransactionStart event, called whenever
        /// transaction started.
        /// </summary>
        public virtual void OnTransactionStartedAsyncEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionStartedAsync != null)
                OnTransactionStartedAsync(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion


        #region OnTransactionCommit/OnTransactionCommitEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified whenever a transaction
        /// will be commited.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionCommitEventHandler OnTransactionCommit;

        /// <summary>
        /// Invoke the OnTransactionStart event, called whenever
        /// transaction will be commited.
        /// </summary>
        public virtual void OnTransactionCommitEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionCommit != null)
                OnTransactionCommit(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion

        #region OnTransactionCommitted/OnTransactionCommittedEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified whenever a transaction
        /// was committed.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionCommittedEventHandler OnTransactionCommitted;

        /// <summary>
        /// Invoke the OnTransactionCommitted event, called whenever
        /// a transaction was committed.
        /// </summary>
        public virtual void OnTransactionCommittedEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionCommitted != null)
                OnTransactionCommitted(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion

        #region OnTransactionCommittedAsync/OnTransactionCommittedEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified asynchronously whenever a
        /// transaction was committed.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionCommittedAsyncEventHandler OnTransactionCommittedAsync;

        /// <summary>
        /// Invoke the OnTransactionCommitted event, called whenever
        /// a transaction was committed.
        /// </summary>
        public virtual void OnTransactionCommittedAsyncEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionCommittedAsync != null)
                OnTransactionCommittedAsync(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion


        #region OnTransactionRollback/OnTransactionRollbackEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified whenever a transaction
        /// will be rollbacked.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionRollbackEventHandler OnTransactionRollback;

        /// <summary>
        /// Invoke the OnTransactionStart event, called whenever
        /// transaction will be rollbacked.
        /// </summary>
        public virtual void OnTransactionRollbackEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionRollback != null)
                OnTransactionRollback(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion

        #region OnTransactionRollbacked/OnTransactionRollbackedEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified whenever a transaction
        /// was rollbacked.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionRollbackedEventHandler OnTransactionRollbacked;

        /// <summary>
        /// Invoke the OnTransactionRollbacked event, called whenever
        /// a transaction was rollbacked.
        /// </summary>
        public virtual void OnTransactionRollbackedEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionRollbacked != null)
                OnTransactionRollbacked(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion

        #region OnTransactionRollbackedAsync/OnTransactionRollbackedAsyncEvent(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp)

        /// <summary>
        /// An event to be notified asynchronously whenever a
        /// transaction was rollbacked.
        /// </summary>
        public event GraphFSEventHandlers.OnTransactionRollbackedAsyncEventHandler OnTransactionRollbackedAsync;

        /// <summary>
        /// Invoke the OnTransactionRollbacked event, called whenever
        /// a transaction was rollbacked.
        /// </summary>
        public virtual void OnTransactionRollbackedAsyncEvent(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp)
        {
            if (OnTransactionRollbackedAsync != null)
                OnTransactionRollbackedAsync(myDistributed, myLongRunning, myIsolationLevel, myName, myTimestamp);
        }

        #endregion

        #endregion
        
        #endregion

        #region Constructor(s)

        #region AGraphFS()

        public AGraphFS()
        {
            _ForestUUID                         = ForestUUID.NewUUID;
            FileSystemUUID                      = FileSystemUUID.NewUUID;
            FileSystemDescription               = "";
            AccessMode                          = AccessModeTypes.rw;
            ParentFileSystem                    = null;
            _GraphFSLookuptable                 = new MountpointLookup();
            NotificationSettings                = new NotificationSettings();
            _MoreThanOnePathSeperatorRegExpr    = new Regex("\\" + FSPathConstants.PathDelimiter + "\\" + FSPathConstants.PathDelimiter);
        }

        public AGraphFS(IObjectCache myIObjectCache)
            : this()
        {
            _ObjectCache                        = myIObjectCache;
        }

        #endregion

        #endregion


        #region Information Methods

        #region FileSystemUUID

        public FileSystemUUID FileSystemUUID { get; protected set; }

        #endregion

        #region AccessMode

        public AccessModeTypes AccessMode { get; protected set; }

        #endregion

        #region IsMounted

        /// <summary>
        /// Returns true if the file system was mounted correctly
        /// </summary>
        /// <returns>true if the file system was mounted correctly</returns>
        public abstract Boolean IsMounted { get; }

        #endregion

        #region IsPersistent

        public abstract Boolean IsPersistent { get; }

        #endregion


        #region TraverseChildFSs(myFunc, myDepth, mySessionToken)

        public IEnumerable<Object> TraverseChildFSs(Func<IGraphFS, UInt64, IEnumerable<Object>> myFunc, UInt64 myDepth, SessionToken mySessionToken)
        {

            var _List = new List<Object>();

            foreach (var _ChildFS in _GraphFSLookuptable.ChildFSs)
                _List.AddRange(myFunc(_ChildFS, myDepth - 1));

            return _List;

        }

        #endregion


        #region GetFileSystemUUID(mySessionToken)

        /// <summary>
        /// Returns the UUID of this file system
        /// </summary>
        /// <returns>The UUID of this file system</returns>
        public FileSystemUUID GetFileSystemUUID(SessionToken mySessionToken)
        {

            // Will cause exceptions!
            //if (!IsMounted)
            //    throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            return FileSystemUUID;

        }

        #endregion

        #region GetFileSystemUUID(myObjectLocation, mySessionToken)

        /// <summary>
        /// Returns the UUID of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The UUID of the file system at the given ObjectLocation</returns>
        public FileSystemUUID GetFileSystemUUID(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            return GetChildFileSystem(myObjectLocation, true, mySessionToken).GetFileSystemUUID(mySessionToken);

        }

        #endregion

        #region GetFileSystemUUIDs(myDepth, mySessionToken)

        /// <summary>
        /// Returns a recursive list of FileSystemUUIDs of all mounted file systems
        /// </summary>
        /// <param name="myDepth">Depth</param>
        /// <returns>A (recursive) list of FileSystemUUIDs of all mounted file systems</returns>
        public IEnumerable<FileSystemUUID> GetFileSystemUUIDs(UInt64 myDepth, SessionToken mySessionToken)
        {

            var _List = new List<FileSystemUUID>() { GetFileSystemUUID(mySessionToken) };

            if (myDepth > 0)
                foreach (var _Items in TraverseChildFSs((ChildFS, Depth) => ChildFS.GetFileSystemUUIDs(Depth, mySessionToken), myDepth, mySessionToken))
                    _List.Add(_Items as FileSystemUUID);

            return _List;

        }

        #endregion


        #region GetFileSystemDescription(mySessionToken)

        /// <summary>
        /// Returns the Name or a description of this file system.
        /// </summary>
        /// <returns>The Name or a description of this file system</returns>
        public String GetFileSystemDescription(SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            return FileSystemDescription;

        }

        #endregion

        #region GetFileSystemDescription(myObjectLocation, mySessionToken)

        /// <summary>
        /// Returns the Name or a description of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The Name or a description of the file system at the given ObjectLocation</returns>
        public String GetFileSystemDescription(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            return GetChildFileSystem(myObjectLocation, true, mySessionToken).GetFileSystemDescription(mySessionToken);

        }

        #endregion

        #region GetFileSystemDescriptions(myDepth, mySessionToken)

        /// <summary>
        /// Returns a recursive list of FileSystemDescriptions of all mounted file systems
        /// </summary>
        /// <param name="myDepth">Depth</param>
        /// <returns>A (recursive) list of FileSystemDescriptions of all mounted file systems</returns>
        public IEnumerable<String> GetFileSystemDescriptions(UInt64 myDepth, SessionToken mySessionToken)
        {

            var _List = new List<String>() { GetFileSystemDescription(mySessionToken) };

            if (myDepth > 0)
                foreach (var _Items in TraverseChildFSs((ChildFS, Depth) => ChildFS.GetFileSystemDescriptions(Depth, mySessionToken), myDepth, mySessionToken))
                    _List.Add(_Items as String);

            return _List;

        }

        #endregion


        #region SetFileSystemDescription(myFileSystemDescription, mySessionToken)

        /// <summary>
        /// Sets the Name or a description of this file system.
        /// </summary>
        /// <param name="myFileSystemDescription">the Name or a description of this file system</param>
        public void SetFileSystemDescription(String myFileSystemDescription, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            FileSystemDescription = myFileSystemDescription;

        }

        #endregion

        #region SetFileSystemDescription(myObjectLocation, myFileSystemDescription, mySessionToken)

        /// <summary>
        /// Sets the Name or a description of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myFileSystemDescription">the Name or a description of the file system at the given ObjectLocation</param>
        public void SetFileSystemDescription(ObjectLocation myObjectLocation, String myFileSystemDescription, SessionToken mySessionToken)
        {
            GetChildFileSystem(myObjectLocation, true, mySessionToken).SetFileSystemDescription(myFileSystemDescription, mySessionToken);
        }

        #endregion


        #region GetNumberOfBytes(mySessionToken)

        public abstract UInt64 GetNumberOfBytes(SessionToken mySessionToken);

        #endregion

        #region GetNumberOfBytes(myObjectLocation, mySessionToken)

        public UInt64 GetNumberOfBytes(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            return GetChildFileSystem(myObjectLocation, true, mySessionToken).GetNumberOfBytes(mySessionToken);

        }

        #endregion

        #region GetNumberOfBytes(myRecursiveOperation, mySessionToken)

        public IEnumerable<UInt64> GetNumberOfBytes(Boolean myRecursiveOperation, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            var _ListOfNumberOfBytes = new List<UInt64>();

            if (myRecursiveOperation)
                foreach (var _IGraphFS in _GraphFSLookuptable.ChildFSs)
                {

                    if (_IGraphFS == this)
                        _ListOfNumberOfBytes.Add(GetNumberOfBytes(mySessionToken));

                    else
                        foreach (var _ListOfRecursiveNumberOfBytes in _IGraphFS.GetNumberOfBytes(myRecursiveOperation, mySessionToken))
                            _ListOfNumberOfBytes.Add(_ListOfRecursiveNumberOfBytes);

                }

            else
                foreach (var _IGraphFS in _GraphFSLookuptable.ChildFSs)
                    _ListOfNumberOfBytes.Add(_IGraphFS.GetNumberOfBytes(mySessionToken));

            return _ListOfNumberOfBytes;

        }

        #endregion


        #region GetNumberOfFreeBytes(mySessionToken)

        public abstract UInt64 GetNumberOfFreeBytes(SessionToken mySessionToken);

        #endregion

        #region GetNumberOfFreeBytes(myObjectLocation, mySessionToken)

        public UInt64 GetNumberOfFreeBytes(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            return GetChildFileSystem(myObjectLocation, true, mySessionToken).GetNumberOfFreeBytes(mySessionToken);

        }

        #endregion

        #region GetNumberOfFreeBytes(myRecursiveOperation, mySessionToken)

        public IEnumerable<UInt64> GetNumberOfFreeBytes(Boolean myRecursiveOperation, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            var _ListOfNumberOfFreeBytes = new List<UInt64>();

            if (myRecursiveOperation)
                foreach (var _IGraphFS in _GraphFSLookuptable.ChildFSs)
                {

                    if (_IGraphFS == this)
                        _ListOfNumberOfFreeBytes.Add(GetNumberOfFreeBytes(mySessionToken));

                    else
                        foreach (var _ListOfRecursiveNumberOfFreeBytes in _IGraphFS.GetNumberOfFreeBytes(myRecursiveOperation, mySessionToken))
                            _ListOfNumberOfFreeBytes.Add(_ListOfRecursiveNumberOfFreeBytes);

                }

            else
                foreach (var _IGraphFS in _GraphFSLookuptable.ChildFSs)
                    _ListOfNumberOfFreeBytes.Add(_IGraphFS.GetNumberOfFreeBytes(mySessionToken));

            return _ListOfNumberOfFreeBytes;

        }

        #endregion


        #region GetAccessMode(mySessionToken)

        public AccessModeTypes GetAccessMode(SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            return AccessMode;

        }

        #endregion

        #region GetAccessMode(myObjectLocation, mySessionToken)

        public AccessModeTypes GetAccessMode(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            return GetChildFileSystem(myObjectLocation, true, mySessionToken).GetAccessMode(mySessionToken);

        }

        #endregion

        #region GetAccessModes(myRecursiveOperation, mySessionToken)

        public IEnumerable<AccessModeTypes> GetAccessModes(Boolean myRecursiveOperation, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            var _ListOfAccessModes = new List<AccessModeTypes>();

            if (myRecursiveOperation)
                foreach (var _IGraphFS in _GraphFSLookuptable.ChildFSs)
                {

                    if (_IGraphFS == this)
                        _ListOfAccessModes.Add(GetAccessMode(mySessionToken));

                    else
                        foreach (var _ListOfRecursiveAccessModes in _IGraphFS.GetAccessModes(myRecursiveOperation, mySessionToken))
                            _ListOfAccessModes.Add(_ListOfRecursiveAccessModes);

                }

            else
                foreach (var _IGraphFS in _GraphFSLookuptable.ChildFSs)
                    _ListOfAccessModes.Add(_IGraphFS.GetAccessMode(mySessionToken));


            return _ListOfAccessModes;

        }

        #endregion


        #region ParentFileSystem

        public IGraphFS ParentFileSystem { get; set; }

        #endregion

        #region GetChildFileSystem(myObjectLocation, myRecursive, mySessionToken)

        public IGraphFS GetChildFileSystem(ObjectLocation myObjectLocation, Boolean myRecursive, SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            var _PathLength = Int32.MinValue;
            IGraphFS _ChildIGraphFS = this;

            foreach (var __Mountpoint_IGraphFS in _GraphFSLookuptable.MountedFSs)
            {

                if (myObjectLocation.StartsWith(__Mountpoint_IGraphFS.Key) &&
                    (_PathLength < __Mountpoint_IGraphFS.Key.Length))
                {
                    _PathLength = __Mountpoint_IGraphFS.Key.Length;
                    _ChildIGraphFS = __Mountpoint_IGraphFS.Value;
                }

            }

            if (myRecursive && _ChildIGraphFS != this)
                _ChildIGraphFS = _ChildIGraphFS.GetChildFileSystem(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), true, mySessionToken);

            return _ChildIGraphFS;

        }

        #endregion

        #region GetChildFileSystemMountpoints(myRecursiveOperation, SessionToken mySessionToken)

        public IEnumerable<ObjectLocation> GetChildFileSystemMountpoints(Boolean myRecursiveOperation, SessionToken mySessionToken)
        {

            return new List<ObjectLocation>();

            //if (!isMounted)
            //    throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            //var _ListOfChildFileSystemMountpoints = new List<ObjectLocation>();

            //if (myRecursiveOperation)
            //    foreach (var __MountPoint_IGraphFS in _Lookuptable.MountedFSs)
            //    {

            //        if (__MountPoint_IGraphFS.Value == this)
            //            foreach (var _Mountpoint in _Lookuptable.Mountpoints)
            //            {
            //                if (!_Mountpoint.Equals(FSPathConstants.PathDelimiter))
            //                    _ListOfChildFileSystemMountpoints.Add(_Mountpoint);
            //            }

            //        else
            //            foreach (var _ChildMountpoints in __MountPoint_IGraphFS.Value.GetChildFileSystemMountpoints(myRecursiveOperation, mySessionToken))
            //                _ListOfChildFileSystemMountpoints.Add(new ObjectLocation(__MountPoint_IGraphFS.Key + _ChildMountpoints));

            //    }

            //else
            //    foreach (var _Mountpoint in _Lookuptable.Mountpoints)
            //    {
            //        if (!_Mountpoint.Equals(FSPathConstants.PathDelimiter))
            //            _ListOfChildFileSystemMountpoints.Add(_Mountpoint);
            //    }

            //return _ListOfChildFileSystemMountpoints;

        }

        #endregion

        #region (protected) GetChildFileSystemMountpoint(myObjectLocation, SessionToken mySessionToken)

        protected ObjectLocation GetChildFileSystemMountpoint(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {


            Int32  _PathLength       = Int32.MinValue;
            String _ChildMountpoint  = FSPathConstants.PathDelimiter;

            foreach (var __Mountpoint_IGraphFS in _GraphFSLookuptable.MountedFSs)
            {

                if (myObjectLocation.StartsWith(__Mountpoint_IGraphFS.Key) &&
                    (_PathLength < __Mountpoint_IGraphFS.Key.Length))
                {
                    _PathLength       = __Mountpoint_IGraphFS.Key.Length;
                    _ChildMountpoint  = __Mountpoint_IGraphFS.Key;
                }

            }

            return new ObjectLocation(_ChildMountpoint);

        }

        #endregion

        #region (protected) GetObjectLocationOnChildFileSystem(myObjectLocation, SessionToken mySessionToken)

        protected ObjectLocation GetObjectLocationOnChildFileSystem(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            var newObjectLocation = myObjectLocation.Substring(GetChildFileSystemMountpoint(myObjectLocation, mySessionToken).Length);

            if (!newObjectLocation.StartsWith(FSPathConstants.PathDelimiter))
                newObjectLocation = String.Concat(FSPathConstants.PathDelimiter, newObjectLocation);

            return new ObjectLocation(newObjectLocation);

        }

        #endregion

        #endregion


        #region Make-/Grow-/ShrinkFileSystem

        public abstract Exceptional<FileSystemUUID> MakeFileSystem(IEnumerable<String> myStorageLocations, String myDescription, UInt64 myNumberOfBytes, Boolean myOverwriteExistingFileSystem, Action<Double> myAction, SessionToken mySessionToken);
        public abstract Exceptional<UInt64> GrowFileSystem(UInt64 myNumberOfBytesToAdd, SessionToken mySessionToken);
        public abstract Exceptional<UInt64> ShrinkFileSystem(UInt64 myNumberOfBytesToRemove, SessionToken mySessionToken);

        #endregion

        #region MountFileSystem

        public abstract Exceptional MountFileSystem(String myStorageLocation, AccessModeTypes myFSAccessMode, SessionToken mySessionToken);

        #region MountFileSystem(myStorageLocation, myMountPoint, myFSAccessMode, SessionToken mySessionToken)

        /// <summary>
        /// This method will mount the file system from a StorageLocation serving
        /// the file system into the given ObjectLocation using the given file system
        /// access mode. If the mountpoint is located within another file system this
        /// file system will be called to process this request in a recursive way.
        /// </summary>
        /// <param name="myStorageLocation">A StorageLocation (device or filename) the file system can be read from</param>
        /// <param name="myMountPoint">The location the file system should be mounted at</param>
        /// <param name="myFSAccessMode">The access mode of the file system to mount</param>
        public Exceptional MountFileSystem(IGraphFS myIGraphFS, ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode, SessionToken mySessionToken)
        {

            #region Pre-Mounting checks

            // Check if the root filesystem is mounted
            if (!IsMounted)
            {

                //if (myMountPoint.Equals(FSPathConstants.PathDelimiter))
                //    return MountFileSystem(myStorageLocation, myFSAccessMode, mySessionToken);

                throw new GraphFSException_MountFileSystemFailed("Please mount a (root) file system first!");

            }

            // Remove an ending FSPathConstants.PathDelimiter: "/Volumes/test/" -> "/Volumes/test"
            if ((myMountPoint.Length > FSPathConstants.PathDelimiter.Length) && (myMountPoint.EndsWith(FSPathConstants.PathDelimiter)))
                myMountPoint = new ObjectLocation(myMountPoint.Substring(0, myMountPoint.Length - FSPathConstants.PathDelimiter.Length));

            #endregion


            var _ChildIGraphFS = GetChildFileSystem(myMountPoint, false, mySessionToken);

            if (_ChildIGraphFS == this)
            {

                // Check if the _exact_ _MountPoint is already used by another filesystem
                foreach (var _Mountpoint in _GraphFSLookuptable.Mountpoints)
                    if (myMountPoint.Equals(_Mountpoint))
                        throw new GraphFSException_MountFileSystemFailed("This mountpoint is already in use!");

                // Check if the directory mentioned in the _MountPoint is existend


                //// Register this file system as parent file system
                myIGraphFS.ParentFileSystem = this;

                //// Register the mountedFS object in the list of ChildFileSystems
                _GraphFSLookuptable.Set(myMountPoint, myIGraphFS);


            }

            else
                _ChildIGraphFS.MountFileSystem(myIGraphFS, GetObjectLocationOnChildFileSystem(myMountPoint, mySessionToken), myFSAccessMode, mySessionToken);

            return new Exceptional();

        }

        #endregion

        #region MountFileSystem(myStorageLocation, myMountPoint, myFSAccessMode, SessionToken mySessionToken)

        /// <summary>
        /// This method will mount the file system from a StorageLocation serving
        /// the file system into the given ObjectLocation using the given file system
        /// access mode. If the mountpoint is located within another file system this
        /// file system will be called to process this request in a recursive way.
        /// </summary>
        /// <param name="myStorageLocation">A StorageLocation (device or filename) the file system can be read from</param>
        /// <param name="myMountPoint">The location the file system should be mounted at</param>
        /// <param name="myFSAccessMode">The access mode of the file system to mount</param>
        public Exceptional MountFileSystem(String myStorageLocation, ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode, SessionToken mySessionToken)
        {

            #region Pre-Mounting checks

            // Check if the root filesystem is mounted
            if (!IsMounted)
            {

                if (myMountPoint.Equals(FSPathConstants.PathDelimiter))
                    return MountFileSystem(myStorageLocation, myFSAccessMode, mySessionToken);

                throw new GraphFSException_MountFileSystemFailed("Please mount a (root) file system first!");

            }

            // Remove an ending FSPathConstants.PathDelimiter: "/Volumes/test/" -> "/Volumes/test"
            if ((myMountPoint.Length > FSPathConstants.PathDelimiter.Length) && (myMountPoint.EndsWith(FSPathConstants.PathDelimiter)))
                myMountPoint = new ObjectLocation(myMountPoint.Substring(0, myMountPoint.Length - FSPathConstants.PathDelimiter.Length));

            #endregion

            var _ChildIGraphFS = GetChildFileSystem(myMountPoint, false, mySessionToken);

            if (_ChildIGraphFS == this)
            {

                //if (myMountPoint.Equals(FSPathConstants.PathDelimiter))
                //    MountFileSystem(myStorageLocation, myFSAccessMode, mySessionToken);

                //else
                //{

                    #region Checks against other mounted filesystems

                    // Check if the _exact_ _MountPoint is already used by another filesystem
                    foreach (var _Mountpoint in _GraphFSLookuptable.Mountpoints)
                    {

                        if (myMountPoint.Equals(_Mountpoint))
                            throw new GraphFSException_MountFileSystemFailed("This mountpoint is already in use!");

                        //// Check if the filesystem is already mounted != readonly
                        //if (MountedFileSystems[_Filesystem].StorageIDs.Equals(myStorageLocation))
                        //{
                        //    switch (MountedFileSystems[_Filesystem].AccessMode)
                        //    {
                        //        case AccessModeTypes.rw: throw new GraphFSException_MountFileSystemFailed("File system is already mounted read/write");
                        //        case AccessModeTypes.ap: throw new GraphFSException_MountFileSystemFailed("File system is already mounted appendable");
                        //        case AccessModeTypes.metarw: throw new GraphFSException_MountFileSystemFailed("File system is already mounted meta-read/write");
                        //        case AccessModeTypes.metaap: throw new GraphFSException_MountFileSystemFailed("File system is already mounted meta-appendable");
                        //    }
                        //}

                    }

                    // Check if the directory mentioned in the _MountPoint is existend

                    #endregion

                    #region Mount and register the new file system

                    //IGraphFS newFSObject = new TmpFS1();
                    ////newFSObject.SetNotificationDispatcher(_NotificationDispatcher, mySessionToken);
                    //newFSObject.MountFileSystem(myStorageLocation, myFSAccessMode, mySessionToken);

                    ////ParentMountedFS = FindMountedFileSystemByPath(myMountPoint);

                    //// Register this file system as parent file system
                    //newFSObject.ParentFileSystem = this;

                    //// Register the mountedFS object in the list of ChildFileSystems
                    //_GraphFSLookuptable.Set(myMountPoint, newFSObject);

                    #endregion

                //}

            }

            else
                _ChildIGraphFS.MountFileSystem(myStorageLocation, GetObjectLocationOnChildFileSystem(myMountPoint, mySessionToken), myFSAccessMode, mySessionToken);

            return new Exceptional();

        }

        #endregion

        #region RemountFileSystem(myFSAccessMode, mySessionToken)

        public Exceptional RemountFileSystem(AccessModeTypes myFSAccessMode, SessionToken mySessionToken)
        {
            return RemountFileSystem(ObjectLocation.Root, myFSAccessMode, mySessionToken);
        }

        #endregion

        #region RemountFileSystem(myMountPoint, myFSAccessMode, mySessionToken)

        public Exceptional RemountFileSystem(ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region UnmountFileSystem(mySessionToken)

        public virtual Exceptional UnmountFileSystem(SessionToken mySessionToken)
        {

            if (!IsMounted)
                throw new GraphFSException("Please mount a file system first!");

            // There is at least one (this file system itself) file system!
            if (_GraphFSLookuptable.ChildFSs.LongCount() > 1)
                throw new GraphFSException_UnmountFileSystemFailed("There are still mounted child file systems!");

            // Reset all global variables
            _GraphFSLookuptable.Clear();
            ParentFileSystem        = null;
            FileSystemUUID          = new FileSystemUUID(0);
            FileSystemDescription   = null;
            //_NotificationDispatcher.Dispose();

            return Exceptional.OK;

        }

        #endregion

        #region UnmountFileSystem(myMountPoint, mySessionToken)

        public Exceptional UnmountFileSystem(ObjectLocation myMountPoint, SessionToken mySessionToken)
        {
            var _ChildIGraphFS = GetChildFileSystem(myMountPoint, false, mySessionToken);
            return _ChildIGraphFS.UnmountFileSystem(mySessionToken);
        }

        #endregion

        #region UnmountAllFileSystems(mySessionToken)

        /// <summary>
        /// Will recursively unmount all file systems
        /// </summary>
        /// <param name="mySessionToken"></param>
        /// <returns></returns>
        public Exceptional UnmountAllFileSystems(SessionToken mySessionToken)
        {

            var _Exceptional = new Exceptional();

            // Loop till there is only one (this file system itself) left!
            while (_GraphFSLookuptable.ChildFSs.LongCount() > 1)
            {

                var _ListOfChildIGraphFSs = new List<KeyValuePair<ObjectLocation, IGraphFS>>();

                foreach (var __ChildMountpoint_IGraphFS in _GraphFSLookuptable.MountedFSs)
                    if (__ChildMountpoint_IGraphFS.Value != this)
                        _ListOfChildIGraphFSs.Add(__ChildMountpoint_IGraphFS);

                if (_ListOfChildIGraphFSs.Count > 0)
                {
                    _ListOfChildIGraphFSs[0].Value.UnmountAllFileSystems(mySessionToken);
                    _GraphFSLookuptable.Remove(_ListOfChildIGraphFSs[0].Key);
                }

            }

            return UnmountFileSystem(mySessionToken);

        }

        #endregion

        #region ChangeRootDirectory(myChangeRootPrefix, mySessionToken)

        /// <summary>
        /// Restricts the access to this file system to the given "/ChangeRootPrefix".
        /// This might be of interesst for security and safety purposes.
        /// </summary>
        /// <param name="myChangeRootPrefix">the location of this object (ObjectPath and ObjectName) of the new file system root</param>
        /// <param name="mySessionToken"></param>
        public Exceptional ChangeRootDirectory(String myChangeRootPrefix, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        
        #region INode and ObjectLocator methods

        #region (protected) GetINode_protected(myObjectLocation)

        protected Exceptional<INode> GetINode_protected(ObjectLocation myObjectLocation)
        {

            var _Exceptional = new Exceptional<INode>();
            var _ObjectLocatorExceptional = GetObjectLocator_protected(myObjectLocation);

            if (_ObjectLocatorExceptional.Success() && _ObjectLocatorExceptional.Value != null && _ObjectLocatorExceptional.Value.INodeReference != null)
                _Exceptional.Value = _ObjectLocatorExceptional.Value.INodeReference;

            else
                return _ObjectLocatorExceptional.Convert<INode>().PushT(new GraphFSError_INodeCouldNotBeLoaded(myObjectLocation));

            return _Exceptional;

        }

        #endregion

        #region GetINode(myObjectLocation, mySessionToken)

        public Exceptional<INode> GetINode(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {
            return GetINode_protected(myObjectLocation);
        }

        #endregion

        #region (private) GetObjectLocator_protected(myObjectLocation)

        protected abstract Exceptional<ObjectLocator> GetObjectLocator_protected(ObjectLocation myObjectLocation);

        #endregion

        #region GetObjectLocator(myObjectLocation, mySessionToken)

        public Exceptional<ObjectLocator> GetObjectLocator(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            return GetObjectLocator_protected(myObjectLocation);

            //var _Exceptional = new Exceptional<ObjectLocator>();

            //_Exceptional.Value = _Lookuptable.GetObjectLocator(myObjectLocation);

            //if (_Exceptional.Value == null)
            //    _Exceptional.Add(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));

            //return _Exceptional;
            
        }

        #endregion

        #endregion

        #region (protected) ObjectCache handling

        #region ObjectCacheSettings

        public ObjectCacheSettings ObjectCacheSettings
        {

            get
            {
                return _ObjectCache.ObjectCacheSettings;
            }

            set
            {
                if (_ObjectCache != null)
                    _ObjectCache.ObjectCacheSettings = value;
                else
                    Debug.WriteLine("Could not set _ObjectCache.ObjectCacheSettings as _ObjectCache == null!");
            }

        }

        #endregion

        #region GetObjectCacheSettings(mySessionToken)

        public Exceptional<ObjectCacheSettings> GetObjectCacheSettings(SessionToken mySessionToken)
        {
            return new Exceptional<ObjectCacheSettings>(_ObjectCache.ObjectCacheSettings);
        }

        #endregion

        #region GetObjectCacheSettings(myObjectLocation, mySessionToken)

        public Exceptional<ObjectCacheSettings> GetObjectCacheSettings(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region SetObjectCacheSettings(myObjectCacheSettings, mySessionToken)

        public Exceptional SetObjectCacheSettings(ObjectCacheSettings myObjectCacheSettings, SessionToken mySessionToken)
        {

            Debug.Assert(myObjectCacheSettings != null);

            _ObjectCache.ObjectCacheSettings = myObjectCacheSettings;

            return Exceptional.OK;

        }

        #endregion

        #region SetObjectCacheSettings(myObjectLocation, myObjectCacheSettings, mySessionToken)

        public Exceptional SetObjectCacheSettings(ObjectLocation myObjectLocation, ObjectCacheSettings myObjectCacheSettings, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region (protected) CacheAdd(myObjectLocator, myIsPinned = false)

        protected Exceptional CacheAdd(ObjectLocator myObjectLocator, Boolean myIsPinned = false)
        {

            Debug.Assert(myObjectLocator != null);
            Debug.Assert(myObjectLocator.ObjectLocation != null);
            Debug.Assert(_ObjectCache != null);

            return _ObjectCache.StoreObjectLocator(myObjectLocator);

        }

        #endregion

        #region (protected) CacheAdd(myCacheUUID, myAFSObject, myIsPinned = false)

        protected Exceptional CacheAdd(CacheUUID myCacheUUID, AFSObject myAFSObject, Boolean myIsPinned = false)
        {

            Debug.Assert(myAFSObject != null);
            Debug.Assert(myAFSObject.ObjectLocation != null);
            Debug.Assert(_ObjectCache != null);

            return _ObjectCache.StoreAFSObject(myCacheUUID, myAFSObject, myIsPinned);

        }

        #endregion


        #region (protected) CacheGet<PT>(myCacheUUID)

        protected Exceptional<PT> CacheGet<PT>(CacheUUID myCacheUUID)
            where PT : AFSObject
        {

            Debug.Assert(_ObjectCache != null, "_ObjectCache == null! No root file system mounted?");

            var _Exceptional = _ObjectCache.GetAFSObject<PT>(myCacheUUID);

            if (_Exceptional.Value == null)
                _Exceptional.PushT(new GraphFSError("Could not get object with UUID '" + myCacheUUID.ToString() + "' from ObjectCache!"));

            return _Exceptional;

        }

        #endregion


        #region (protected) CacheCopy(mySourceLocation, myTargetLocation, myRecursion = false)

        protected Exceptional CacheCopy(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation, Boolean myRecursion = false)
        {

            Debug.Assert(_ObjectCache != null, "_ObjectCache == null! No root file system mounted?");

            return _ObjectCache.Copy(mySourceLocation, myTargetLocation, myRecursion);

        }

        #endregion

        #region (protected) CacheMove(mySourceLocation, myTargetLocation, myRecursion = false)

        protected Exceptional CacheMove(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation, Boolean myRecursion = false)
        {

            Debug.Assert(_ObjectCache != null, "_ObjectCache == null! No root file system mounted?");

            return _ObjectCache.Move(mySourceLocation, myTargetLocation, myRecursion);

        }

        #endregion


        #region (protected) CacheRemove(myObjectLocation, myRecursion = false)

        protected Exceptional CacheRemove(ObjectLocation myObjectLocation, Boolean myRecursion = false)
        {

            Debug.Assert(_ObjectCache != null, "_ObjectCache == null! No root file system mounted?");

            return _ObjectCache.RemoveObjectLocation(myObjectLocation, myRecursion);

        }

        #endregion

        #region (protected) CacheRemove(myCacheUUID)

        protected Exceptional CacheRemove(CacheUUID myCacheUUID)
        {

            Debug.Assert(_ObjectCache != null, "_ObjectCache == null! No root file system mounted?");

            return _ObjectCache.RemoveAFSObject(myCacheUUID);

        }

        #endregion

        #endregion



        #region AFSObject specific methods

        protected abstract Exceptional            StoreAFSObject_Layer2_protected (ObjectLocation myObjectLocation, AFSObject myAFSObject, Boolean myAllowOverwritting);
        protected abstract Exceptional<AFSObject> LoadAFSObject_protected  (ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, AFSObject myAFSObject);
        protected abstract Exceptional            RemoveAFSObject_protected(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID);
        protected abstract Exceptional            EraseAFSObject_protected (ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID);


        #region LockObject_protected(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectLock, myObjectLockType, myLockingTime)

        protected Boolean LockObject_protected(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, ObjectLocks myObjectLock, ObjectLockTypes myObjectLockType, UInt64 myLockingTime)
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            return false;

        }

        #endregion

        #region LockObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectLock, myObjectLockType, myLockingTime, mySessionToken)

        public Boolean LockObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, ObjectLocks myObjectLock, ObjectLockTypes myObjectLockType, UInt64 myLockingTime, SessionToken mySessionToken)
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            return LockObject_protected(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectLock, myObjectLockType, myLockingTime);

        }

        #endregion



        #region GetOrCreateFSObject_protected<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures)

        protected Exceptional<PT> GetOrCreateFSObject_protected<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures) where PT : AFSObject, new()
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            return GetOrCreateFSObject_protected<PT>(myObjectLocation,
                                           myObjectStream,
                                           myObjectEdition,
                                           myObjectRevisionID,
                                           myObjectCopy,
                                           myIgnoreIntegrityCheckFailures,
                                           new Func<PT>(delegate { return new PT(); }));

        }

        #endregion

        #region GetOrCreateFSObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, mySessionToken)

        public Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, SessionToken mySessionToken) where PT : AFSObject, new()
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            return GetOrCreateFSObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, new Func<PT>(delegate { return new PT(); }), mySessionToken);

        }

        #endregion

        #region GetOrCreateObject_protected<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myIgnoreIntegrityCheckFailures)

        protected Exceptional<PT> GetOrCreateFSObject_protected<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, Func<PT> myFunc) where PT : AFSObject
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision
            Debug.Assert(myFunc             != null);

            lock (this)
            {

                #region Get an existing Object...

                var _Exceptional = GetFSObject_protected<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myFunc);

                #endregion

                #region ...or create a new one!

                try
                {

                    if (_Exceptional.Value == null)
                    {

                        _Exceptional       = new Exceptional<PT>();
                        //_Exceptional.Value = new PT() { ObjectLocation = myObjectLocation };
                        _Exceptional.Value = myFunc();
                        _Exceptional.Value.ObjectLocation = myObjectLocation;

                        if (myObjectStream != null && myObjectStream.Length > 0)
                            _Exceptional.Value.ObjectStream = myObjectStream;

                        if (myObjectEdition != null && myObjectEdition.Length > 0)
                            _Exceptional.Value.ObjectEdition = myObjectEdition;

                        if (myObjectRevisionID != null && myObjectRevisionID.Timestamp != 0 && myObjectRevisionID.UUID != null)
                            _Exceptional.Value.ObjectRevisionID = myObjectRevisionID;

                    }

                }

                catch (Exception e)
                {
                    _Exceptional.PushT(new GraphFSError(e.Message));
                }

                #endregion

                return _Exceptional;
            
            }

        }

        #endregion

        #region GetOrCreateObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myIgnoreIntegrityCheckFailures, mySessionToken)

        public Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, Func<PT> myFunc, SessionToken mySessionToken) where PT : AFSObject
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            //ToDo: Check mySessionToken!

            return GetOrCreateFSObject_protected<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myFunc);

        }

        #endregion



        #region GetFSObject_protected<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures)

        protected Exceptional<PT> GetFSObject_protected<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures) where PT : AFSObject, new()
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            return GetFSObject_protected<PT>(myObjectLocation,
                                             myObjectStream,
                                             myObjectEdition,
                                             myObjectRevisionID,
                                             myObjectCopy,
                                             myIgnoreIntegrityCheckFailures,
                                             new Func<PT>(delegate { return new PT(); }));

        }

        #endregion

        #region GetFSObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, mySessionToken)

        public Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, SessionToken mySessionToken) where PT : AFSObject, new()
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            return GetFSObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, new Func<PT>(delegate { return new PT(); }), mySessionToken);

        }

        #endregion

        #region GetFSObject_protected<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures)

        protected Exceptional<PT> GetFSObject_protected<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, Func<PT> myFunc) where PT : AFSObject
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision
            Debug.Assert(myFunc             != null);

            lock (this)
            {


                PT newT = myFunc();

                #region Input validation

                using (var _Exceptional2 = new Exceptional<PT>().NotNullOrEmptyMsg<PT>("The myObjectLocation must not be null or its length zero!", myObjectLocation))
                {
                    if (_Exceptional2.Failed())
                        return _Exceptional2;
                }

                if (myObjectStream == null)
                {
                    
                    if (newT.ObjectStream == null)
                        return new Exceptional<PT>(new GraphFSError("newT.ObjectStream == null!"));

                    myObjectStream = newT.ObjectStream;

                }

                if (myObjectEdition == null)
                    myObjectEdition = FSConstants.DefaultEdition;

                ObjectStream   _ObjectStream   = null;
                ObjectEdition  _ObjectEdition  = null;
                ObjectRevision _ObjectRevision = null;

                #endregion

                var _Exceptional = new Exceptional<PT>();

                try
                {

                    var _ObjectLocatorExceptional = GetObjectLocator_protected(myObjectLocation).
                        WhenFailed<ObjectLocator>(e => e.PushT(new GraphFSError_ObjectLocatorNotFound(myObjectLocation)));

                    if (_ObjectLocatorExceptional.Failed())
                        return _ObjectLocatorExceptional.Convert<PT>();

                    if (_ObjectLocatorExceptional.Success() && _ObjectLocatorExceptional.Value != null)
                    {

                        #region Resolve ObjectStream, -Edition and -RevisionID

                        // Will use tryget(...) internally!
                        _ObjectStream = _ObjectLocatorExceptional.Value[myObjectStream];
                        if (_ObjectStream == null) return new Exceptional<PT>(new GraphFSError("ObjectStream '" + myObjectStream + "' not found!"));

                        // Will use tryget(...) internally!
                        _ObjectEdition = _ObjectStream[myObjectEdition];
                        if (_ObjectEdition == null) return new Exceptional<PT>(new GraphFSError("ObjectEdition '" + myObjectEdition + "' not found!"));

                        if (_ObjectEdition.IsDeleted)
                            return new Exceptional<PT>(new GraphFSError_ObjectNotFound(myObjectLocation));

                        // If nothing specified => Return the LatestRevision
                        if (myObjectRevisionID == null || myObjectRevisionID.UUID == null)
                        {
                            _ObjectRevision    = _ObjectEdition.LatestRevision;
                            myObjectRevisionID = _ObjectEdition.LatestRevisionID;
                        }

                        else
                        {
                            // Will use tryget(...) internally!
                            _ObjectRevision = _ObjectEdition[myObjectRevisionID];
                            if (_ObjectRevision == null) return new Exceptional<PT>(new GraphFSError("ObjectRevision '" + myObjectRevisionID + "' not found!"));
                        }

                        //if (_ObjectLocatorExceptional.Value.ContainsKey(myObjectStream))
                        //{

                        //    _ObjectStream = _ObjectLocatorExceptional.Value[myObjectStream];

                        //    if (_ObjectStream != null)
                        //    {

                        //        if (_ObjectStream.ContainsKey(myObjectEdition))
                        //        {

                        //            _ObjectEdition = _ObjectStream[myObjectEdition];

                        //            if (_ObjectEdition != null)
                        //            {

                        //                if (_ObjectEdition.IsDeleted)
                        //                    return new Exceptional<PT>(new GraphFSError_ObjectNotFound(myObjectLocation));

                        //                // If nothing specified => Return the LatestRevision
                        //                if (myObjectRevisionID == null || myObjectRevisionID.UUID == null)
                        //                {
                        //                    _ObjectRevision    = _ObjectEdition.LatestRevision;
                        //                    myObjectRevisionID = _ObjectEdition.LatestRevisionID;
                        //                }

                        //                else
                        //                {
                        //                    _ObjectRevision = _ObjectEdition[myObjectRevisionID];
                        //                }

                        //            }
                        //            else
                        //                return new Exceptional<PT>(new GraphFSError_NoObjectRevisionsFound(myObjectLocation, myObjectStream, myObjectEdition));

                        //        }
                        //        else
                        //            return new Exceptional<PT>(new GraphFSError_ObjectEditionNotFound(myObjectLocation, myObjectEdition, myObjectStream));

                        //    }
                        //    else
                        //        return new Exceptional<PT>(new GraphFSError_NoObjectEditionsFound(myObjectLocation, myObjectStream));

                        //}
                        //else
                        //    return new Exceptional<PT>(new GraphFSError_ObjectStreamNotFound(myObjectLocation, myObjectStream));

                        #endregion

                        if (_ObjectRevision != null && _ObjectRevision.CacheUUID != null)
                        {

                            #region Try to get the object from the internal cache...

                            var _GetObjectFromCacheExceptional = CacheGet<PT>(_ObjectRevision.CacheUUID);

                            if (_GetObjectFromCacheExceptional.Success() && _GetObjectFromCacheExceptional.Value != null)
                            {
                                
                                _Exceptional.Value       = _GetObjectFromCacheExceptional.Value;

                                if (_Exceptional.Value.ObjectLocatorReference == null)
                                    _Exceptional.Value.ObjectLocatorReference = _ObjectLocatorExceptional.Value;

                                if (_Exceptional.Value.INodeReference == null)
                                    _Exceptional.Value.INodeReference = _ObjectLocatorExceptional.Value.INodeReference;

                                _Exceptional.Value.isNew = false;

                                return _Exceptional;

                            }

                            #endregion

                            #region ...or try to load the object via the IGraphFS internal loading-mechanism!

                            else
                            {

                                if (_ObjectRevision.Count > 2)
                                {
                                    Debug.WriteLine("_ObjectRevision.Count > 2");
                                }

                                //var _LoadObjectExceptional = LoadObject_protected<PT>(_ObjectLocatorExceptional.Value, _ObjectRevision, myIgnoreIntegrityCheckFailures);
                                var _LoadObjectExceptional = LoadAFSObject_protected(_ObjectLocatorExceptional.Value, myObjectStream, myObjectEdition, myObjectRevisionID, 0, myIgnoreIntegrityCheckFailures, newT);

                                if (_LoadObjectExceptional.Success() && _LoadObjectExceptional.Value != null)
                                {

                                    _LoadObjectExceptional.Value.ObjectStream      = myObjectStream;
                                    _LoadObjectExceptional.Value.ObjectStream      = myObjectStream;
                                    _LoadObjectExceptional.Value.ObjectEdition     = myObjectEdition;
                                    _LoadObjectExceptional.Value.ObjectRevisionID    = myObjectRevisionID;

                                    // Cache the loaded object
                                    CacheAdd(_ObjectRevision.CacheUUID, _LoadObjectExceptional.Value, false);

                                    //_Exceptional.Value = _LoadObjectExceptional.Value;
                                    _Exceptional.Value = _LoadObjectExceptional.Value as PT;

                                }

                                else
                                {
                                    // ErrorHandling!
                                    return _Exceptional.PushT(new GraphFSError_AllObjectCopiesFailed(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID));
                                }

                            }

                            #endregion

                        }

                        else
                            _Exceptional.PushT(new GraphFSError("Could not find a valid CacheUUID for this object!"));

                    }

                }

                catch (Exception e)
                {
                    return new Exceptional<PT>(new GraphFSError(e.Message));
                }

                return _Exceptional;

            }

        }

        #endregion

        #region GetFSObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myFunc, mySessionToken)

        public Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, Func<PT> myFunc, SessionToken mySessionToken)
            where PT : AFSObject
        {

            Debug.Assert(myObjectLocation   != null);
            //Debug.Assert(myObjectStream     != null); => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            //ToDo: Check mySessionToken!

            #region Call OnLoadEvent on this file system

            OnLoadEvent(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID);

            #endregion

            var _PTExceptional = GetFSObject_protected<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myFunc);

            #region Call OnSavedEvents on this file system and the given AFSObject

            if (!_PTExceptional.Failed())
            {
                OnLoadedEvent(_PTExceptional.Value.ObjectLocatorReference, _PTExceptional.Value);
                _PTExceptional.Value.OnLoadedEvent(_PTExceptional.Value.ObjectLocatorReference, _PTExceptional.Value);
            }

            #endregion

            return _PTExceptional;

        }

        #endregion



        #region StoreAFSObject_protected(myObjectLocation, myAGraphObject, myAllowOverwritting)

        protected Exceptional StoreAFSObject_protected(ObjectLocation myObjectLocation, AFSObject myAFSObject, Boolean myAllowOverwritting)
        {

            var _Exceptional = new Exceptional();

            #region Check/Set myAFSObject.ObjectStream/-Edition/-RevisionID

            if (myAFSObject.ObjectEdition == null)
                myAFSObject.ObjectEdition = FSConstants.DefaultEdition;

            // Use the _ForestUUID to distinguish ObjectRevisions in a distributed environment!
            if (myAFSObject.ObjectRevisionID == null)
                myAFSObject.ObjectRevisionID = new ObjectRevisionID(_ForestUUID);

            #endregion

            #region Check/Set the ObjectLocator and the INode

            if (myAFSObject.ObjectLocatorReference == null)
            {

                // Will load the ParentIDirectoryObject
                var _AFSObjectLocator = GetObjectLocator_protected(myObjectLocation);

                if (_AFSObjectLocator.Failed())
                {

                    var _ParentIDirectoryObjectLocator = GetObjectLocator_protected(new ObjectLocation(myObjectLocation.Path));

                    if (_ParentIDirectoryObjectLocator.Failed())
                    {
                        //return _ParentIDirectoryObjectLocator.Push(new GraphFSError_ObjectLocatorNotFound(new ObjectLocation(myObjectLocation.Path)));
                        throw new GraphFSException("ObjectLocator for parent ObjectLocation '" + myObjectLocation.Path + "' was not found!");
                    }

                }

                myAFSObject.ObjectLocatorReference = _AFSObjectLocator.Value;

                if (myAFSObject.ObjectLocatorReference == null)
                {

                    myAFSObject.ObjectLocatorReference = new ObjectLocator(myAFSObject.ObjectLocation, myAFSObject.ObjectUUID);
                    myAFSObject.ObjectLocatorReference.INodeReference = new INode(myAFSObject.ObjectUUID);
                    myAFSObject.ObjectLocatorReference.INodeReference.LastModificationTime = myAFSObject.ObjectRevisionID.Timestamp;

                    myAFSObject.ObjectLocatorReference.INodeReference.ObjectLocatorReference = myAFSObject.ObjectLocatorReference;

                }

                myAFSObject.INodeReference = myAFSObject.ObjectLocatorReference.INodeReference;

            }

            if (myAFSObject.INodeReference.ObjectLocatorReference == null)
                myAFSObject.INodeReference.ObjectLocatorReference = myAFSObject.ObjectLocatorReference;

            if (myAFSObject.ObjectLocatorReference.INodeReference == null)
                myAFSObject.ObjectLocatorReference.INodeReference = myAFSObject.INodeReference;

            #endregion

            #region Check/Set the ObjectStream and ObjectEdition

            // Add ObjectStreamName
            if (myAFSObject.ObjectLocatorReference == null ||
                myAFSObject.ObjectLocatorReference.ContainsKey(myAFSObject.ObjectStream) == false)
                myAFSObject.ObjectLocatorReference.Add(myAFSObject.ObjectStream, null);


            // Add ObjectStream
            if (myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream] == null)
                myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream] = new ObjectStream(myAFSObject.ObjectStream, myAFSObject.ObjectEdition, null);

            // Add ObjectEditionName
            else if (!myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream].ContainsKey(myAFSObject.ObjectEdition))
                myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream].Add(myAFSObject.ObjectEdition, null);


            // Add ObjectEdition
            if (myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition] == null)
                myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition] = new ObjectEdition(myAFSObject.ObjectEdition, myAFSObject.ObjectRevisionID, null)
                {
                    IsDeleted = false,
                    MinNumberOfRevisions = FSConstants.MIN_NUMBER_OF_REVISIONS,
                    MaxNumberOfRevisions = FSConstants.MAX_NUMBER_OF_REVISIONS,
                    MinRevisionDelta = FSConstants.MIN_REVISION_DELTA,
                    MaxRevisionAge = FSConstants.MAX_REVISION_AGE
                };

            else

                // The ObjectEdition might be marked as deleted before => remove this mark and store another ObjectRevision
                if (myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].IsDeleted)
                    myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].IsDeleted = false;

            #endregion

            #region ObjectRevision

            #region Create the first ObjectRevision

            if (myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].Count() == 0)
            {
                myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].
                    Add(myAFSObject.ObjectRevisionID, new ObjectRevision(myAFSObject.ObjectStream)
                {
                    MinNumberOfCopies = FSConstants.MIN_NUMBER_OF_COPIES,
                    MaxNumberOfCopies = FSConstants.MAX_NUMBER_OF_COPIES
                });
            }

            #endregion

            // Unknown := newer or older but not first ObjectRevision
            else
            {

                #region Try to overwrite the latest ObjectRevision

                if (myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].LatestRevisionID == myAFSObject.ObjectRevisionID)
                {

                    if (myAllowOverwritting)
                    {

                        myAFSObject.ObjectRevisionID = new ObjectRevisionID(_ForestUUID);

                        myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].
                            Add(myAFSObject.ObjectRevisionID, new ObjectRevision(myAFSObject.ObjectStream)
                        {
                            MinNumberOfCopies = FSConstants.MIN_NUMBER_OF_COPIES,
                            MaxNumberOfCopies = FSConstants.MAX_NUMBER_OF_COPIES
                        });

                    }

                    else
                    {
                        return _Exceptional.Push(new GraphFSError_CouldNotOverwriteRevision(myAFSObject.ObjectLocation, myAFSObject.ObjectStream, myAFSObject.ObjectEdition, myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].LatestRevisionID));
                    }

                }

                #endregion

                #region Try to add a very old ObjectRevision

                else if (myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].LatestRevisionID > myAFSObject.ObjectRevisionID)
                {
                    if (myAllowOverwritting)
                    {

                        myAFSObject.ObjectRevisionID = new ObjectRevisionID(_ForestUUID);

                        myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].
                            Add(myAFSObject.ObjectRevisionID, new ObjectRevision(myAFSObject.ObjectStream)
                        {
                            MinNumberOfCopies = FSConstants.MIN_NUMBER_OF_COPIES,
                            MaxNumberOfCopies = FSConstants.MAX_NUMBER_OF_COPIES
                        });

                    }

                    else
                    {
                        return _Exceptional.Push(new GraphFSError_CouldNotOverwriteRevision(myAFSObject.ObjectLocation, myAFSObject.ObjectStream, myAFSObject.ObjectEdition, myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition].LatestRevisionID));
                    }
                }

                #endregion

                #region Add a very new ObjectRevision

                else// if (myAGraphObject.ObjectLocatorReference[myAGraphObject.ObjectStream][myAGraphObject.ObjectEdition][myAGraphObject.ObjectRevision] == null)
                {
                    myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition][myAFSObject.ObjectRevisionID] =
                        new ObjectRevision(myAFSObject.ObjectStream)
                    {
                        MinNumberOfCopies = FSConstants.MIN_NUMBER_OF_COPIES,
                        MaxNumberOfCopies = FSConstants.MAX_NUMBER_OF_COPIES
                    };
                }

                #endregion

            }

            //// Check if there is already an existing revision
            //if (myAGraphObject.ObjectLocatorReference[myAGraphObject.ObjectStream][myAGraphObject.ObjectEdition].ULongCount() > 0 && !myAllowOverwritting)
            //{
            //    _Exceptional.Add(new GraphFSError_CouldNotOverwriteRevision(myAGraphObject.ObjectLocation, myAGraphObject.ObjectStream, myAGraphObject.ObjectEdition, myAGraphObject.ObjectLocatorReference[myAGraphObject.ObjectStream][myAGraphObject.ObjectEdition].LatestRevisionID));
            //    //return _Exceptional;
            //    throw new GraphFSException_ObjectStreamAlreadyExists(myAGraphObject.ObjectStream + " '" + myObjectLocation + "' already exists!");
            //}

            //else if (!myAGraphObject.ObjectLocatorReference[myAGraphObject.ObjectStream][myAGraphObject.ObjectEdition].ContainsKey(myAGraphObject.ObjectRevisionID))
            //    myAGraphObject.ObjectLocatorReference[myAGraphObject.ObjectStream][myAGraphObject.ObjectEdition].Add(myAGraphObject.ObjectRevisionID, null);

            #endregion

            StoreAFSObject_Layer2_protected(myObjectLocation, myAFSObject, myAllowOverwritting);

            return _Exceptional;

        }

        #endregion

        #region StoreAFSObject(myObjectLocation, myAFSObject, myAllowOverwritting, mySessionToken)

        public Exceptional StoreAFSObject(ObjectLocation myObjectLocation, AFSObject myAFSObject, Boolean myAllowOverwritting, SessionToken mySessionToken)
        {

            Debug.Assert(myObjectLocation       != null);
            Debug.Assert(myAFSObject            != null);
            Debug.Assert(myAFSObject.ObjectStream     != null);// => PT.ObjectStream
            //Debug.Assert(myObjectEdition    != null); => DefaultEdition
            //Debug.Assert(myObjectRevisionID != null); => LatestRevision

            var _OldObjectRevisionID = myAFSObject.ObjectRevisionID;

            lock (this)
            {

                var _Exceptional = new Exceptional();

                #region Sanity Checks

                _Exceptional.NotNullMsg("AFSObject must not be null!", myAFSObject);
                _Exceptional.NotNullOrEmptyMsg("The ObjectStream must not be null or its length zero!", myAFSObject.ObjectStream);

                if (_Exceptional.Failed())
                    return _Exceptional;

                #endregion

                #region Call OnSaveEvents on this file system and the given AFSObject

                OnSaveEvent(myObjectLocation, myAFSObject);
                myAFSObject.OnSaveEvent(myObjectLocation, myAFSObject);

                #endregion

                StoreAFSObject_protected(myObjectLocation, myAFSObject, myAllowOverwritting);

                #region Call OnSavedEvents on this file system and the given AFSObject

                OnSavedEvent(myAFSObject.ObjectLocatorReference, myAFSObject, _OldObjectRevisionID);
                myAFSObject.OnSavedEvent(myAFSObject.ObjectLocatorReference, myAFSObject, _OldObjectRevisionID);

                #endregion

                return new Exceptional();

            }

        }

        #endregion


        #region ObjectExists(myObjectLocation)

        public Exceptional<Trinary> ObjectExists(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            #region Resolve all symlinks and call myself on a possible ChildFileSystem...

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            //myObjectLocation = ResolveObjectLocation(myObjectLocation, false, mySessionToken);

            if (myObjectLocation == null)
                return new Exceptional<Trinary>(Trinary.FALSE);

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.ObjectExists(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), mySessionToken);

            #endregion

            var _Exceptional = new Exceptional<Trinary>();
            var _ParentDirectoryObjectExceptional = GetFSObject_protected<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), FSConstants.DIRECTORYSTREAM, null, null, 0, false);

            if (_ParentDirectoryObjectExceptional.Success() && _ParentDirectoryObjectExceptional.Value != null)
                _Exceptional.Value = _ParentDirectoryObjectExceptional.Value.ObjectExists(myObjectLocation.Name);

            else
            {
                _Exceptional.PushT(new GraphFSError_DirectoryObjectNotFound(myObjectLocation.Path));
//                _Exceptional.Add(_ParentDirectoryObjectExceptional.Errors);
                _Exceptional.Value = Trinary.FALSE;
            }

            return _Exceptional;

        }

        #endregion

        #region ObjectStreamExists(myObjectLocation, myObjectStream, mySessionToken)

        public Exceptional<Trinary> ObjectStreamExists(ObjectLocation myObjectLocation, String myObjectStream, SessionToken mySessionToken)
        {

            #region Resolve all symlinks and call myself on a possible ChildFileSystem...

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            //myObjectLocation = ResolveObjectLocation(myObjectLocation, false, mySessionToken);

            //if (myObjectLocation == null)
            //    return Trinary.FALSE;

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.ObjectStreamExists(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myObjectStream, mySessionToken);

            #endregion

            var _Exceptional = new Exceptional<Trinary>();
            var _ParentDirectoryObjectExceptional = GetFSObject<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), FSConstants.DIRECTORYSTREAM, null, null, 0, false, mySessionToken);

            if (_ParentDirectoryObjectExceptional.Success() && _ParentDirectoryObjectExceptional.Value != null)
            {

                // Get DirectoryEntry
                var _DirectoryEntry = _ParentDirectoryObjectExceptional.Value.GetDirectoryEntry(myObjectLocation.Name);

                if (_DirectoryEntry != null && _DirectoryEntry.ObjectStreamsList != null)
                    _Exceptional.Value = _DirectoryEntry.ObjectStreamsList.Contains(myObjectStream);

                else _Exceptional.Value = Trinary.FALSE;

            }

            else
            {
                _Exceptional = new Exceptional<Trinary>(_ParentDirectoryObjectExceptional);
                _Exceptional.Push(new GraphFSError_DirectoryObjectNotFound(myObjectLocation));
                _Exceptional.Value = Trinary.FALSE;
            }

            return _Exceptional;

        }

        #endregion

        #region ObjectEditionExists(myObjectLocation, myObjectStream, myObjectEdition, mySessionToken)

        public Exceptional<Trinary> ObjectEditionExists(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, SessionToken mySessionToken)
        {

            #region Resolve all symlinks and call myself on a possible ChildFileSystem...

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            //myObjectLocation = ResolveObjectLocation(myObjectLocation, false, mySessionToken);

            if (myObjectLocation == null)
                return new Exceptional<Trinary>() { Value = Trinary.FALSE };

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.ObjectEditionExists(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myObjectStream, myObjectEdition, mySessionToken);

            #endregion

            var _Exceptional = new Exceptional<Trinary>();
            var _ObjectLocatorExceptional = GetObjectLocator(myObjectLocation, mySessionToken);

            if (_ObjectLocatorExceptional.Success() && _ObjectLocatorExceptional.Value != null)
            {

                // Get ObjectStream
                var _ObjectStream = _ObjectLocatorExceptional.Value[myObjectStream];

                if (_ObjectStream != null)
                    _Exceptional.Value = _ObjectStream.ContainsKey(myObjectEdition);

                else
                {
                    _Exceptional.PushT(new GraphFSError_ObjectStreamNotFound(myObjectLocation, myObjectStream));
                    _Exceptional.Value = Trinary.FALSE;
                }

            }

            else
            {
                _Exceptional = new Exceptional<Trinary>(_ObjectLocatorExceptional);
                _Exceptional.PushT(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));
                _Exceptional.Value = Trinary.FALSE;
            }

            return _Exceptional;

        }

        #endregion

        #region ObjectRevisionExists(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, mySessionToken)

        public Exceptional<Trinary> ObjectRevisionExists(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, SessionToken mySessionToken)
        {

            #region Resolve all symlinks and call myself on a possible ChildFileSystem...

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            //myObjectLocation = ResolveObjectLocation(myObjectLocation, false, mySessionToken);

            if (myObjectLocation == null)
                return new Exceptional<Trinary>() { Value = Trinary.FALSE };

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.ObjectRevisionExists(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myObjectStream, myObjectEdition, myObjectRevisionID, mySessionToken);

            #endregion

            var _Exceptional = new Exceptional<Trinary>();
            var _ObjectLocatorExceptional = GetObjectLocator(myObjectLocation, mySessionToken);

            if (_ObjectLocatorExceptional.Success() && _ObjectLocatorExceptional.Value != null)
            {

                // Get ObjectStream
                var _ObjectStream = _ObjectLocatorExceptional.Value[myObjectStream];

                if (_ObjectStream != null && _ObjectStream.ContainsKey(myObjectEdition))
                {

                    // Get ObjectEdition
                    var _ObjectEdition = _ObjectStream[myObjectEdition];

                    if (_ObjectEdition != null)
                        _Exceptional.Value = _ObjectEdition.ContainsKey(myObjectRevisionID);
                    
                    else
                    {
                        _Exceptional.PushT(new GraphFSError_ObjectEditionNotFound(myObjectLocation, myObjectStream, myObjectEdition));
                        _Exceptional.Value = Trinary.FALSE;
                    }

                }

                else
                {
                    _Exceptional.PushT(new GraphFSError_ObjectStreamNotFound(myObjectLocation, myObjectStream));
                    _Exceptional.Value = Trinary.FALSE;
                }

            }

            else
            {
                _Exceptional = new Exceptional<Trinary>(_ObjectLocatorExceptional);
                _Exceptional.PushT(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));
                _Exceptional.Value = Trinary.FALSE;
            }

            return _Exceptional;

        }

        #endregion


        #region GetObjectStreams(myObjectLocation, mySessionToken)

        public Exceptional<IEnumerable<String>> GetObjectStreams(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            lock (this)
            {

                var _Exceptional = new Exceptional<IEnumerable<String>>();

                #region Special handling of the root directory as we can not ask its parent directory!

                if (myObjectLocation.Equals(FSPathConstants.PathDelimiter))
                {

                    var _ObjectLocatorExceptional = GetObjectLocator(myObjectLocation, mySessionToken);

                    if (_ObjectLocatorExceptional.Success() && _ObjectLocatorExceptional.Value != null)
                        _Exceptional.Value = _ObjectLocatorExceptional.Value.Keys;

                    else
                    {
                        _Exceptional = _ObjectLocatorExceptional.Convert<IEnumerable<String>>().
                            PushT(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));
                    }

                }

                #endregion

                #region Just ask the parent directory, as this avoids unnecessary loading of the INode and ObjectLocator!

                else
                {

                    var _ParentDirectoryObjectExceptional = GetFSObject<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), FSConstants.DIRECTORYSTREAM, null, null, 0, false, mySessionToken);

                    if (_ParentDirectoryObjectExceptional.Success() && _ParentDirectoryObjectExceptional.Value != null)
                    {

                        // Get DirectoryEntry
                        var _DirectoryEntry = _ParentDirectoryObjectExceptional.Value.GetDirectoryEntry(myObjectLocation.Name);

                        if (_DirectoryEntry != null && _DirectoryEntry.ObjectStreamsList != null)
                            _Exceptional.Value = _DirectoryEntry.ObjectStreamsList;

                        else
                            _Exceptional.PushT(new GraphFSError_CouldNotGetObjectStreams(myObjectLocation));

                    }

                    else
                    {
                        _Exceptional = _ParentDirectoryObjectExceptional.Convert<IEnumerable<String>>().
                            PushT(new GraphFSError_DirectoryObjectNotFound(myObjectLocation));
                    }

                }

                #endregion

                return _Exceptional;

            }

        }

        #endregion

        #region GetObjectEditions(myObjectLocation, myObjectStream, mySessionToken)

        public Exceptional<IEnumerable<String>> GetObjectEditions(ObjectLocation myObjectLocation, String myObjectStream, SessionToken mySessionToken)
        {

            var _Exceptional = new Exceptional<IEnumerable<String>>();
            var _ObjectLocatorExceptional = GetObjectLocator(myObjectLocation, mySessionToken);

            if (_ObjectLocatorExceptional.Success() && _ObjectLocatorExceptional.Value != null)
            {

                // Get ObjectStream
                var _ObjectStream = _ObjectLocatorExceptional.Value[myObjectStream];

                if (_ObjectStream != null)
                    _Exceptional.Value = _ObjectStream.Keys;

                else
                    _Exceptional.PushT(new GraphFSError_ObjectStreamNotFound(myObjectLocation, myObjectStream));

            }

            else
            {
                _Exceptional = _ObjectLocatorExceptional.Convert<IEnumerable<String>>().
                    PushT(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));
            }

            return _Exceptional;

        }

        #endregion

        #region GetObjectRevisionIDs(myObjectLocation, myObjectStream, myObjectEdition, mySessionToken)

        public Exceptional<IEnumerable<ObjectRevisionID>> GetObjectRevisionIDs(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, SessionToken mySessionToken)
        {

            var _Exceptional = new Exceptional<IEnumerable<ObjectRevisionID>>();
            var _ObjectLocatorExceptional = GetObjectLocator(myObjectLocation, mySessionToken);

            if (_ObjectLocatorExceptional.Success() && _ObjectLocatorExceptional.Value != null)
            {

                // Get ObjectStream
                var _ObjectStream = _ObjectLocatorExceptional.Value[myObjectStream];

                if (_ObjectStream != null && _ObjectStream.ContainsKey(myObjectEdition))
                {

                    // Get ObjectEdition
                    var _ObjectEdition = _ObjectStream[myObjectEdition];

                    if (_ObjectEdition != null)
                        _Exceptional.Value = _ObjectEdition.Keys;

                    else
                        _Exceptional.PushT(new GraphFSError_ObjectEditionNotFound(myObjectLocation, myObjectStream, myObjectEdition));

                }

                else
                    _Exceptional.PushT(new GraphFSError_ObjectStreamNotFound(myObjectLocation, myObjectStream));

            }

            else
            {
                _Exceptional = _ObjectLocatorExceptional.Convert<IEnumerable<ObjectRevisionID>>().
                    PushT(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));
            }

            return _Exceptional;

        }

        #endregion


        #region RenameObject_protected(myObjectLocation, myNewObjectName)

        protected Exceptional RenameObject_protected(ObjectLocation myObjectLocation, String myNewObjectName)
        {

            var _Exceptional = new Exceptional();

            var _ParentDirectoryObjectExceptional = GetFSObject_protected<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), FSConstants.DIRECTORYSTREAM, null, null, 0, false);

            if (_ParentDirectoryObjectExceptional.Success() && _ParentDirectoryObjectExceptional.Value != null)
            {

                if (!_ParentDirectoryObjectExceptional.Value.RenameDirectoryEntry(myObjectLocation.Name, myNewObjectName))
                    _Exceptional.Push(new GraphFSError_ObjectNotFound(myObjectLocation));

                var _GetObjectLocatorExceptional = GetObjectLocator_protected(myObjectLocation);
                if (_GetObjectLocatorExceptional.Failed() || _GetObjectLocatorExceptional.Value == null)
                    _Exceptional.Push(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));

                else
                {
                    CacheMove(myObjectLocation, new ObjectLocation(myObjectLocation.Path, myNewObjectName), true);
                }

                return _Exceptional;

            }

            else
            {
                _Exceptional = _ParentDirectoryObjectExceptional.Convert<IEnumerable<ObjectRevisionID>>().
                    PushT(new GraphFSError_DirectoryObjectNotFound(myObjectLocation.Path));
            }

            return _Exceptional;

        }

        #endregion

        #region RenameObject(myObjectLocation, myNewObjectName, mySessionToken)

        public Exceptional RenameObject(ObjectLocation myObjectLocation, String myNewObjectName, SessionToken mySessionToken)
        {

            #region Resolve all symlinks and call myself on a possible ChildFileSystem...

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            //myObjectLocation = ResolveObjectLocation(myObjectLocation, false, mySessionToken);

            if (myObjectLocation == null)
                throw new ArgumentNullException("myObjectLocation must not be null!");

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.RenameObject(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myNewObjectName, mySessionToken);

            #endregion

            return RenameObject_protected(myObjectLocation, myNewObjectName);

        }

        #endregion


        #region RemoveObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, mySessionToken)

        public Exceptional RemoveFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, SessionToken mySessionToken)
        {

            #region Call OnRemoveEvent on this file system

            OnRemoveEvent(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID);

            #endregion


            #region GetObjectLocator

            var _Exceptional = new Exceptional<IEnumerable<String>>();
            var _ObjectLocatorExceptional = GetObjectLocator(myObjectLocation, mySessionToken);

            if (_ObjectLocatorExceptional.Failed() || _ObjectLocatorExceptional.Value == null)
                return _Exceptional.Push(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));

            #endregion

            #region Check the ObjectOntology

            var _ObjectLocator = _ObjectLocatorExceptional.Value;

            // ObjectStream
            if (myObjectStream == null || _ObjectLocator.ContainsKey(myObjectStream) == false)
                return new Exceptional().Push(new GraphFSError("Invalid ObjectStream '" + myObjectStream + "'!"));

            var _ObjectStream = _ObjectLocator[myObjectStream];

            // ObjectEdition
            if (myObjectEdition == null || _ObjectStream.ContainsKey(myObjectEdition) == false)
                return new Exceptional().Push(new GraphFSError("Invalid ObjectEdition '" + myObjectEdition + "'!"));

            var _ObjectEdition = _ObjectStream[myObjectEdition];

            if (_ObjectEdition.IsDeleted)
                return Exceptional.OK;

            // ObjectRevision
            if (myObjectRevisionID == null || _ObjectEdition.ContainsKey(myObjectRevisionID) == false)
                return new Exceptional().Push(new GraphFSError("Invalid ObjectRevisionID '" + myObjectRevisionID + "'!"));

            var _ObjectRevision = _ObjectEdition[myObjectRevisionID];

            #endregion

            #region Remove AFSObject from ObjectCache

            try
            {
                // ErrorHandling?!
                CacheRemove(_ObjectRevision.CacheUUID);
            }
            catch (Exception)
            { }

            #endregion

            #region Mark ObjectEdition as deleted

            _ObjectEdition.IsDeleted = true;
            RemoveAFSObject_protected(_ObjectLocator, myObjectStream, myObjectEdition, myObjectRevisionID);

            //var _success = _ObjectEdition.Remove(myObjectRevisionID);

            //// If the last ObjectRevision was removed => Remove the ObjectEdition
            //if (_ObjectEdition.Count == 0)
            //{

            //    _ObjectStream.Remove(myObjectEdition);

            //    // If the last ObjectEdition was removed => Remove the ObjectEdition
            //    if (_ObjectStream.Count == 0)
            //    {

            //        _ObjectLocator.Remove(myObjectStream);

            //        if (_ObjectLocator.Count == 0)
            //            CacheRemove(myObjectLocation, false);

            //    }

            //}

            #endregion

            #region Remove ObjectStream from ParentIDirectoryObject

            var _Exceptional2 = GetFSObject<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), null, null, null, 0, false, mySessionToken).
                WhenFailed<DirectoryObject>(e => e.PushT(new GraphFSError_DirectoryObjectNotFound(myObjectLocation.Path))).
                WhenSucceded<DirectoryObject>(e =>
                {

                    e.Value.IGraphFSReference = this;
                    e.Value.RemoveObjectStream(myObjectLocation.Name, myObjectStream);

                    var _ObjectStreamsList = e.Value.GetObjectStreamsList(myObjectLocation.Name);

                    if (_ObjectStreamsList.Contains(FSConstants.ACCESSCONTROLSTREAM) &&
                        _ObjectStreamsList.CountIs(2))
                        e.Value.RemoveObjectStream(myObjectLocation.Name, FSConstants.ACCESSCONTROLSTREAM);

                    return e;

                }
                );

            #endregion


            #region Call OnRemovedEvent on this file system

            if (!_Exceptional2.Failed())
            {
                OnRemovedEvent(_ObjectLocatorExceptional.Value, myObjectStream, myObjectEdition, myObjectRevisionID);
            }

            #endregion

            return _Exceptional2;

        }

        #endregion

        #region EraseObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, mySessionToken)

        public Exceptional EraseFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Symlink methods

        #region AddSymlink_protected(myObjectLocation, myTargetLocation, mySessionToken)

        protected Exceptional AddSymlink_protected(ObjectLocation myObjectLocation, ObjectLocation myTargetLocation)
        {

            lock (this)
            {

                var _Exceptional = new Exceptional();

                // Add symlink to ParentIDirectoryObject
                var _ParentIDirectoryObject = GetFSObject_protected<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), null, null, null, 0, false);

                if (_ParentIDirectoryObject.Failed())
                {
                    return _ParentIDirectoryObject.Push(new GraphFSError_DirectoryObjectNotFound(myObjectLocation.Path));
                }

                if (!_ParentIDirectoryObject.Value.ObjectExists(myObjectLocation.Name))
                {
                    _ParentIDirectoryObject.Value.IGraphFSReference = this;
                    _ParentIDirectoryObject.Value.AddSymlink(myObjectLocation.Name, myTargetLocation);
                }

                return _Exceptional;

            }

        }

        #endregion

        #region AddSymlink(myObjectLocation, myTargetLocation, mySessionToken)

        public Exceptional AddSymlink(ObjectLocation myObjectLocation, ObjectLocation myTargetLocation, SessionToken mySessionToken)
        {

            #region Resolve all symlinks...

            //myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(ResolveObjectLocation(new ObjectLocation(myObjectLocation.Path), true, mySessionToken),
            myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(new ObjectLocation(myObjectLocation.Path),
                                                  DirectoryHelper.GetObjectName(myObjectLocation)));

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
            {
                _ChildIGraphFS.AddSymlink(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myTargetLocation, mySessionToken);
                return new Exceptional();
            }

            #endregion

            return AddSymlink_protected(myObjectLocation, myTargetLocation);

        }

        #endregion

        #region isSymlink(myObjectLocation, SessionToken mySessionToken)

        public Exceptional<Trinary> isSymlink(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            #region Resolve all symlinks and call myself on a possible ChildFileSystem...

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            //var myIParentDirectoryLocation = ResolveObjectLocation(new ObjectLocation(myObjectLocation.Path), true, mySessionToken);
            var myIParentDirectoryLocation = new ObjectLocation(myObjectLocation.Path);
            if (myIParentDirectoryLocation.Length == 0)
                return new Exceptional<Trinary>(Trinary.FALSE);

            myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(myIParentDirectoryLocation, myObjectLocation.Name));

            IGraphFS _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.isSymlink(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), mySessionToken);

            #endregion

            lock (this)
            {

                var _Exceptional = new Exceptional<Trinary>();

                // Add symlink to ParentIDirectoryObject
                var _ParentIDirectoryObject = GetFSObject<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), null, null, null, 0, false, mySessionToken);

                if (_ParentIDirectoryObject.Failed())
                {
                    return _ParentIDirectoryObject.Convert<Trinary>().PushT(new GraphFSError_DirectoryObjectNotFound(myObjectLocation.Path));
                }

                _Exceptional.Value = _ParentIDirectoryObject.Value.isSymlink(myObjectLocation.Name);

                return _Exceptional;

            }

        }

        #endregion

        #region GetSymlink(myObjectLocation, SessionToken mySessionToken)

        public Exceptional<ObjectLocation> GetSymlink(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            #region Resolve all symlinks...

            //myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(ResolveObjectLocation(new ObjectLocation(myObjectLocation.Path), true, mySessionToken),
            myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(new ObjectLocation(myObjectLocation.Path),
                                                       DirectoryHelper.GetObjectName(myObjectLocation)));

            IGraphFS _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.GetSymlink(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), mySessionToken);

            #endregion

            lock (this)
            {

                var _Exceptional = new Exceptional<ObjectLocation>();

                // Add symlink to ParentIDirectoryObject
                var _ParentIDirectoryObject = GetFSObject<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), null, null, null, 0, false, mySessionToken);

                if (_ParentIDirectoryObject.Failed())
                {
                    return _ParentIDirectoryObject.Convert<ObjectLocation>().PushT(new GraphFSError_DirectoryObjectNotFound(myObjectLocation.Path));
                }

                _Exceptional.Value = _ParentIDirectoryObject.Value.GetSymlink(myObjectLocation.Name);

                return _Exceptional;

            }

        }

        #endregion

        #region RemoveSymlink(myObjectLocation, SessionToken mySessionToken)

        /// <summary>
        /// This method removes a symlink from the parent directory.
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        public Exceptional RemoveSymlink(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            #region Check if the ObjectLocation is a symlink

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            if (isSymlink(myObjectLocation, mySessionToken).Value != Trinary.TRUE)
                throw new GraphFSException_SymlinkCouldNotBeRemoved("Only symlinks can be removed by this method!");

            #endregion

            #region Resolve all symlinks and call myself on a possible ChildFileSystem...

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            // Will throw an exception if the ObjectLocation could not be resolved
            //myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(ResolveObjectLocation(new ObjectLocation(myObjectLocation), true, mySessionToken),
            myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(new ObjectLocation(myObjectLocation),
                                                       DirectoryHelper.GetObjectName(myObjectLocation)));

            IGraphFS _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.RemoveSymlink(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), mySessionToken);

            #endregion


            //if (mySessionToken.Transaction == null)
            //    using (FSTransaction transaction = beginTransaction(mySessionToken, true))
            //    {
            //        RemoveAGraphObject_private(myObjectLocation, FSConstants.SYMLINK, null, null, mySessionToken);
            //        CommitTransaction(mySessionToken);
            //    }
            //else
            return RemoveFSObject(myObjectLocation, FSConstants.SYMLINK, null, null, mySessionToken);

        }

        #endregion

        #endregion

        #region DirectoryObject Methods

        protected abstract Exceptional<IDirectoryObject> InitDirectoryObject_protected(ObjectLocation myObjectLocation, UInt64 myBlocksize);

        #region (protected) CreateDirectoryObject(myObjectLocation, myBlocksize, mySessionToken)

        protected Exceptional<IDirectoryObject> CreateDirectoryObject_protected(ObjectLocation myObjectLocation, UInt64 myBlocksize)
        {

            //if (mySessionToken.Transaction == null)
            //    BeginTransaction(mySessionToken, true);

            var _Exceptional = new Exceptional<IDirectoryObject>();

            try
            {                

                #region Initialize a new DirectoryObject

                // Just create a DirectoryObject within the IGraphFS implementation
                var _IDirectoryObject = InitDirectoryObject_protected(myObjectLocation, myBlocksize).Value;

                // Add standard subdirectories to the new DirectoryObject
                _IDirectoryObject.AddObjectStream(FSConstants.DotLink,     FSConstants.DIRECTORYSTREAM, new List<ExtendedPosition> { new ExtendedPosition(0, 0) });
                _IDirectoryObject.AddObjectStream(FSConstants.DotDotLink,  FSConstants.DIRECTORYSTREAM, new List<ExtendedPosition> { new ExtendedPosition(0, 0) });
                _IDirectoryObject.AddObjectStream(FSConstants.DotMetadata, FSConstants.VIRTUALDIRECTORY);
                _IDirectoryObject.AddObjectStream(FSConstants.DotSystem,   FSConstants.VIRTUALDIRECTORY);
                _IDirectoryObject.AddObjectStream(FSConstants.DotStreams,  FSConstants.VIRTUALDIRECTORY);
                _IDirectoryObject.StoreInlineData(FSConstants.DotUUID,     _IDirectoryObject.ObjectUUID.GetByteArray(), true);

                #endregion

                #region Store the new DirectoryObject

                var _AFSObject = _IDirectoryObject as AFSObject;

                if (_AFSObject == null)
                    throw new GraphFSException("'_AFSObject = _IDirectoryObject as AFSObject' failed!");

                using (var _StoreDirectoryObjectExceptional = StoreAFSObject_protected(myObjectLocation, _AFSObject, true))
                {
                    if (_StoreDirectoryObjectExceptional.Failed())
                        return _StoreDirectoryObjectExceptional.Convert<IDirectoryObject>().PushT(new GraphFSError_CreateDirectoryFailed(myObjectLocation));
                }

                //_DirectoryObject.IGraphFSReference = this;

                #endregion

                _Exceptional.Value = _IDirectoryObject;

            }

            catch (Exception)
            {
                _Exceptional.PushT(new GraphFSError_CreateDirectoryFailed(myObjectLocation));
            }

            return _Exceptional;            

        }

        #endregion

        #region CreateDirectoryObject(myObjectLocation, myBlocksize, mySessionToken)

        public Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation, UInt64 myBlocksize, SessionToken mySessionToken)
        {

            #region Resolve all symlinks and call myself on a possible ChildFileSystem...

            if (!IsMounted)
                throw new GraphFSException_FileSystemNotMounted("Please mount a file system first!");

            //myObjectLocation = ResolveObjectLocation(myObjectLocation, false, mySessionToken);

            if (myObjectLocation == null)
                throw new ArgumentNullException("myObjectLocation must not be null!");

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.CreateDirectoryObject(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myBlocksize, mySessionToken);

            #endregion

            return CreateDirectoryObject_protected(myObjectLocation, myBlocksize);

        }

        #endregion

        #region CreateDirectoryObject(myObjectLocation, myBlocksize, myRecursive, mySessionToken)

        public Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation, UInt64 myBlocksize, Boolean myRecursive, SessionToken mySessionToken)
        {

            throw new NotImplementedException();

            //var _ObjectPath = new ObjectLocation(myObjectLocation.Path);
            //var _ObjectName = myObjectLocation.Name;

            //IEnumerable<String> _ObjectStreams  = new List<String>();
            //IDirectoryObject    _ParentDir      = null;
            //IGraphFS            _IGraphFS       = this;

            //while (!ResolveObjectLocation(ref _ObjectPath, out _ObjectStreams, out _ObjectPath, out _ObjectName, out _ParentDir, out _IGraphFS, mySessionToken))
            //{
            //    _IGraphFS.CreateDirectoryObject(new ObjectLocation(DirectoryHelper.Combine(_ParentDir.ObjectLocation, _ObjectName)), 0, mySessionToken);
            //    _ObjectPath = new ObjectLocation(myObjectLocation.Path);
            //    _ObjectName = myObjectLocation.Name;
            //}

            ////myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(ResolveObjectLocation(new ObjectLocation(myObjectLocation.Path), true, mySessionToken),
            //myObjectLocation = new ObjectLocation(DirectoryHelper.Combine(new ObjectLocation(myObjectLocation.Path),
            //                                      DirectoryHelper.GetObjectName(myObjectLocation)));

            //return CreateDirectoryObject(myObjectLocation, myBlocksize, mySessionToken);

        }

        #endregion


        #region isIDirectoryObject(myObjectLocation, mySessionToken)

        public Exceptional<Trinary> isIDirectoryObject(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            lock (this)
            {

                return GetObjectStreams(myObjectLocation, mySessionToken).
                           ConvertWithFunc<IEnumerable<String>, Trinary>(v => v.Contains(FSConstants.DIRECTORYSTREAM)).
                           WhenFailed<Trinary>(e => new Exceptional<Trinary>(Trinary.FALSE));

                ////ToDo: Resolve SymLinks!

                //var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

                //if (_ChildIGraphFS != this)
                //    return _ChildIGraphFS.isIDirectoryObject(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), mySessionToken);

                //// Get ParentDirectoryObject, check ObjectStream and return Trinary.FALSE on any error!
                //return GetObject<DirectoryObject>(myObjectLocation.Path, FSConstants.DIRECTORYSTREAM, FSConstants.DefaultEdition, null, 0, false, mySessionToken).
                //    Convert<DirectoryObject, Trinary>(v => v.ObjectStreamExists(myObjectLocation.Name, FSConstants.DIRECTORYSTREAM)).
                //    WhenFailed<Trinary>(e => new Exceptional<Trinary>(Trinary.FALSE));

            }

        }

        #endregion


        #region GetDirectoryListing(myObjectLocation, mySessionToken)

        public Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            lock (this)
            {

                //ToDo: Resolve SymLinks!

                var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

                if (_ChildIGraphFS != this)
                    return _ChildIGraphFS.GetDirectoryListing(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), mySessionToken);

                // Get DirectoryObject and return the directory listing
                return GetFSObject<DirectoryObject>(myObjectLocation, FSConstants.DIRECTORYSTREAM, FSConstants.DefaultEdition, null, 0, false, mySessionToken).
                       ConvertWithFunc<DirectoryObject, IEnumerable<String>>(v => v.GetDirectoryListing());

            }

        }

        #endregion

        #region GetDirectoryListing(myObjectLocation, myFunc, mySessionToken)

        public Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation, Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc, SessionToken mySessionToken)
        {

            lock (this)
            {

                //ToDo: Resolve SymLinks!

                var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

                if (_ChildIGraphFS != this)
                    return _ChildIGraphFS.GetDirectoryListing(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myFunc, mySessionToken);

                // Get DirectoryObject and return the directory listing
                return GetFSObject<DirectoryObject>(myObjectLocation, FSConstants.DIRECTORYSTREAM, FSConstants.DefaultEdition, null, 0, false, mySessionToken).
                       ConvertWithFunc<DirectoryObject, IEnumerable<String>>(v => v.GetDirectoryListing(myFunc));

            }

        }

        #endregion

        #region GetFilteredDirectoryListing(myObjectLocation, myLogin, myIgnoreName, myRegExpr, myObjectStream, myIgnoreObjectStreamType, mySize, myCreationTime, myModificationTime, myLastAccessTime, myDeletionTime, mySessionToken)

        public Exceptional<IEnumerable<String>> GetFilteredDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime, SessionToken mySessionToken)
        {

            //ToDo: Resolve SymLinks!

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.GetFilteredDirectoryListing(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams, mySize, myCreationTime, myLastModificationTime, myLastAccessTime, myDeletionTime, mySessionToken);

            var     _Output = new List<String>();
            INode   _INode  = null;
            Boolean _AddEntry;

            // Get DirectoryObject and return the filtered directory listing
            return GetFSObject<DirectoryObject>(myObjectLocation, FSConstants.DIRECTORYSTREAM, FSConstants.DefaultEdition, null, 0, false, mySessionToken).
                   ConvertWithFunc<DirectoryObject, IEnumerable<String>>(v => v.GetDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)).
                   WhenSucceded<IEnumerable<String>>(_Exceptional =>
                   {

                       #region Apply additional filters

                       if (mySize                   != null ||
                           myCreationTime           != null ||
                           myLastModificationTime   != null ||
                           myLastAccessTime         != null ||
                           myDeletionTime           != null)
                       {

                           foreach (var _ObjectName in _Exceptional.Value)
                           {

                               _AddEntry = false;

                               #region Load the INode via this methods in order to make use of the object cache

                               if (myObjectLocation.Equals(FSPathConstants.PathDelimiter))
                                   _INode = GetINode(new ObjectLocation(_ObjectName), mySessionToken).Value;

                               else
                                   _INode = GetINode(new ObjectLocation(myObjectLocation, _ObjectName), mySessionToken).Value;

                               #endregion

                               #region Parameter --size=[<|=|>]NumberOfBytes

                               if (mySize != null)
                                   try
                                   {
                                       foreach (var _Size in mySize)
                                           switch (_Size[0])
                                           {
                                               case '<': if (_INode.ObjectSize < UInt64.Parse(_Size.Substring(1))) _AddEntry = true; break;
                                               case '=': if (_INode.ObjectSize == UInt64.Parse(_Size.Substring(1))) _AddEntry = true; break;
                                               case '>': if (_INode.ObjectSize > UInt64.Parse(_Size.Substring(1))) _AddEntry = true; break;
                                               default: throw new GraphFSException_ParsedSizeIsInvalid("The parameter 'size' could not be parsed!");
                                           }

                                   }
                                   catch (Exception)
                                   {
                                       throw new GraphFSException_ParsedSizeIsInvalid("The parameter 'size' could not be parsed!");
                                   }

                               #endregion

                               #region Parameter --CreationTime=[<|=|>]Timestamp

                               if (myCreationTime != null)
                                   try
                                   {
                                       foreach (var _CreationTime in myCreationTime)
                                           switch (_CreationTime[0])
                                           {
                                               case '<': if (_INode.CreationTime < UInt64.Parse(_CreationTime.Substring(1))) _AddEntry = true; break;
                                               case '=': if (_INode.CreationTime == UInt64.Parse(_CreationTime.Substring(1))) _AddEntry = true; break;
                                               case '>': if (_INode.CreationTime > UInt64.Parse(_CreationTime.Substring(1))) _AddEntry = true; break;
                                               default: throw new GraphFSException_ParsedCreationTimeIsInvalid("The parameter 'CreationTime' could not be parsed!");
                                           }

                                   }
                                   catch (Exception)
                                   {
                                       throw new GraphFSException_ParsedCreationTimeIsInvalid("The parameter 'CreationTime' could not be parsed!");
                                   }

                               #endregion

                               #region Parameter --LastAccessTime=[<|=|>]Timestamp

                               if (myLastAccessTime != null)
                                   try
                                   {
                                       foreach (var _LastAccessTime in myLastAccessTime)
                                           switch (_LastAccessTime[0])
                                           {
                                               case '<': if (_INode.LastAccessTime < UInt64.Parse(_LastAccessTime.Substring(1))) _AddEntry = true; break;
                                               case '=': if (_INode.LastAccessTime == UInt64.Parse(_LastAccessTime.Substring(1))) _AddEntry = true; break;
                                               case '>': if (_INode.LastAccessTime > UInt64.Parse(_LastAccessTime.Substring(1))) _AddEntry = true; break;
                                               default: throw new GraphFSException_ParsedLastAccessTimeIsInvalid("The parameter 'LastAccessTime' could not be parsed!");
                                           }

                                   }
                                   catch (Exception)
                                   {
                                       throw new GraphFSException_ParsedLastAccessTimeIsInvalid("The parameter 'LastAccessTime' could not be parsed!");
                                   }

                               #endregion

                               #region Parameter --LastModificationTime=[<|=|>]Timestamp

                               if (myLastModificationTime != null)
                                   try
                                   {
                                       foreach (var _LastModificationTime in myLastModificationTime)
                                           switch (_LastModificationTime[0])
                                           {
                                               case '<': if (_INode.CreationTime < UInt64.Parse(_LastModificationTime.Substring(1))) _AddEntry = true; break;
                                               case '=': if (_INode.CreationTime == UInt64.Parse(_LastModificationTime.Substring(1))) _AddEntry = true; break;
                                               case '>': if (_INode.CreationTime > UInt64.Parse(_LastModificationTime.Substring(1))) _AddEntry = true; break;
                                               default: throw new GraphFSException_ParsedLastModificationTimeIsInvalid("The parameter 'LastModificationTime' could not be parsed!");
                                           }

                                   }
                                   catch (Exception)
                                   {
                                       throw new GraphFSException_ParsedLastModificationTimeIsInvalid("The parameter 'LastModificationTime' could not be parsed!");
                                   }

                               #endregion

                               #region Parameter --DeletionTime=[<|=|>]Timestamp

                               if (myDeletionTime != null)
                                   try
                                   {
                                       foreach (var _DeletionTime in myDeletionTime)
                                           switch (_DeletionTime[0])
                                           {
                                               case '<': if (_INode.DeletionTime < UInt64.Parse(_DeletionTime.Substring(1))) _AddEntry = true; break;
                                               case '=': if (_INode.DeletionTime == UInt64.Parse(_DeletionTime.Substring(1))) _AddEntry = true; break;
                                               case '>': if (_INode.DeletionTime > UInt64.Parse(_DeletionTime.Substring(1))) _AddEntry = true; break;
                                               default: throw new GraphFSException_ParsedDeletionTimeIsInvalid("The parameter 'DeletionTime' could not be parsed!");
                                           }

                                   }
                                   catch (Exception)
                                   {
                                       throw new GraphFSException_ParsedDeletionTimeIsInvalid("The parameter 'DeletionTime' could not be parsed!");
                                   }

                               #endregion


                               if (_AddEntry) _Output.Add(_ObjectName);

                           }

                       }

                       else
                           _Output.AddRange(_Exceptional.Value);

                       #endregion

                       return new Exceptional<IEnumerable<string>>(_Output);

                    });

        }

        #endregion


        #region GetExtendedDirectoryListing(myObjectLocation, mySessionToken)

        public Exceptional<IEnumerable<DirectoryEntryInformation>> GetExtendedDirectoryListing(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            lock (this)
            {

                // Resolve SymLinks!

                var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

                if (_ChildIGraphFS != this)
                    return _ChildIGraphFS.GetExtendedDirectoryListing(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), mySessionToken);

                // Get DirectoryObject and return the extended directory listing
                return GetFSObject<DirectoryObject>(myObjectLocation, FSConstants.DIRECTORYSTREAM, FSConstants.DefaultEdition, null, 0, false, mySessionToken).
                       ConvertWithFunc<DirectoryObject, IEnumerable<DirectoryEntryInformation>>(v => v.GetExtendedDirectoryListing());

            }

        }

        #endregion

        #region GetExtendedDirectoryListing(myObjectLocation, myFunc, mySessionToken)

        public Exceptional<IEnumerable<DirectoryEntryInformation>> GetExtendedDirectoryListing(ObjectLocation myObjectLocation, Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc, SessionToken mySessionToken)
        {

            lock (this)
            {

                // Resolve SymLinks!

                var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

                if (_ChildIGraphFS != this)
                    return _ChildIGraphFS.GetExtendedDirectoryListing(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myFunc, mySessionToken);

                // Get DirectoryObject and return the extended directory listing
                return GetFSObject<DirectoryObject>(myObjectLocation, FSConstants.DIRECTORYSTREAM, FSConstants.DefaultEdition, null, 0, false, mySessionToken).
                       ConvertWithFunc<DirectoryObject, IEnumerable<DirectoryEntryInformation>>(v => v.GetExtendedDirectoryListing(myFunc));

            }

        }

        #endregion

        #region GetFilteredExtendedDirectoryListing(myObjectLocation, myLogin, myIgnoreName, myRegExpr, myObjectStream, myIgnoreObjectStreamType, mySize, myCreationTime, myModificationTime, myLastAccessTime, myDeletionTime, mySessionToken)

        public Exceptional<IEnumerable<DirectoryEntryInformation>> GetFilteredExtendedDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreamTypes, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime, SessionToken mySessionToken)
        {

            #region Resolve all symlinks...

            //myObjectLocation = new ObjectLocation(ResolveObjectLocation(myObjectLocation, true, mySessionToken));

            #endregion

            var _ChildIGraphFS = GetChildFileSystem(myObjectLocation, false, mySessionToken);

            if (_ChildIGraphFS != this)
                return _ChildIGraphFS.GetFilteredExtendedDirectoryListing(GetObjectLocationOnChildFileSystem(myObjectLocation, mySessionToken), myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreamTypes, mySize, myCreationTime, myLastModificationTime, myLastAccessTime, myDeletionTime, mySessionToken);


            #region _IDirectoryObject.GetExtendedDirectoryListing(...)

            var _IDirectoryObject = (IDirectoryObject) GetFSObject<DirectoryObject>(myObjectLocation, FSConstants.DIRECTORYSTREAM, null, null, 0, false, mySessionToken).Value;
            var _DirectoryListing = _IDirectoryObject.GetExtendedDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreamTypes);
            var _Output = new List<DirectoryEntryInformation>(); ;

            INode _INode = null;
            Boolean _AddEntry;
            HashSet<String> _ObjectStreamTypes;
            DirectoryEntryInformation __ExtendedDirectoryListing;

            #endregion

            #region Additional Filters

            foreach (var _ExtendedDirectoryListing in _DirectoryListing)
            {

                _AddEntry = false;

                _ObjectStreamTypes = _ExtendedDirectoryListing.Streams;

                if (
                    (_ObjectStreamTypes.Contains(FSConstants.INLINEDATA)) ||
                    (_ObjectStreamTypes.Contains(FSConstants.SYMLINK)) ||
                    (_ObjectStreamTypes.Contains(FSConstants.VIRTUALDIRECTORY)) ||
                    (_ExtendedDirectoryListing.Name.Equals(FSConstants.DotLink)) ||
                    (_ExtendedDirectoryListing.Name.Equals(FSConstants.DotDotLink))
                   )
                {
                    __ExtendedDirectoryListing = _ExtendedDirectoryListing;
                    __ExtendedDirectoryListing.Size = 0;
                    __ExtendedDirectoryListing.Timestamp = 0;
                    _Output.Add(__ExtendedDirectoryListing);
                }

                else
                {

                    #region Load the INode via this methods in order to make use of the object cache

                    if (myObjectLocation.Equals(FSPathConstants.PathDelimiter))
                        _INode = GetINode(new ObjectLocation(_ExtendedDirectoryListing.Name), mySessionToken).Value;

                    else
                        _INode = GetINode(new ObjectLocation(myObjectLocation, _ExtendedDirectoryListing.Name), mySessionToken).Value;

                    #endregion


                    if (mySize != null ||
                        myCreationTime != null ||
                        myLastModificationTime != null ||
                        myLastAccessTime != null ||
                        myDeletionTime != null)
                    {

                        #region Parameter --size=[<|=|>]NumberOfBytes

                        if (mySize != null)
                            try
                            {
                                foreach (String _Size in mySize)
                                    switch (_Size[0])
                                    {
                                        case '<': if (_INode.ObjectSize < UInt64.Parse(_Size.Substring(1))) _AddEntry = true; break;
                                        case '=': if (_INode.ObjectSize == UInt64.Parse(_Size.Substring(1))) _AddEntry = true; break;
                                        case '>': if (_INode.ObjectSize > UInt64.Parse(_Size.Substring(1))) _AddEntry = true; break;
                                        default: throw new GraphFSException_ParsedSizeIsInvalid("The parameter 'size' could not be parsed!");
                                    }

                            }
                            catch (Exception)
                            {
                                throw new GraphFSException_ParsedSizeIsInvalid("The parameter 'size' could not be parsed!");
                            }

                        #endregion

                        #region Parameter --CreationTime=[<|=|>]Timestamp

                        if (myCreationTime != null)
                            try
                            {
                                foreach (var _CreationTime in myCreationTime)
                                    switch (_CreationTime[0])
                                    {
                                        case '<': if (_INode.CreationTime < UInt64.Parse(_CreationTime.Substring(1))) _AddEntry = true; break;
                                        case '=': if (_INode.CreationTime == UInt64.Parse(_CreationTime.Substring(1))) _AddEntry = true; break;
                                        case '>': if (_INode.CreationTime > UInt64.Parse(_CreationTime.Substring(1))) _AddEntry = true; break;
                                        default: throw new GraphFSException_ParsedCreationTimeIsInvalid("The parameter 'CreationTime' could not be parsed!");
                                    }

                            }
                            catch (Exception)
                            {
                                throw new GraphFSException_ParsedCreationTimeIsInvalid("The parameter 'CreationTime' could not be parsed!");
                            }

                        #endregion

                        #region Parameter --LastAccessTime=[<|=|>]Timestamp

                        if (myLastAccessTime != null)
                            try
                            {
                                foreach (var _LastAccessTime in myLastAccessTime)
                                    switch (_LastAccessTime[0])
                                    {
                                        case '<': if (_INode.LastAccessTime < UInt64.Parse(_LastAccessTime.Substring(1))) _AddEntry = true; break;
                                        case '=': if (_INode.LastAccessTime == UInt64.Parse(_LastAccessTime.Substring(1))) _AddEntry = true; break;
                                        case '>': if (_INode.LastAccessTime > UInt64.Parse(_LastAccessTime.Substring(1))) _AddEntry = true; break;
                                        default: throw new GraphFSException_ParsedLastAccessTimeIsInvalid("The parameter 'LastAccessTime' could not be parsed!");
                                    }

                            }
                            catch (Exception)
                            {
                                throw new GraphFSException_ParsedLastAccessTimeIsInvalid("The parameter 'LastAccessTime' could not be parsed!");
                            }

                        #endregion

                        #region Parameter --LastModificationTime=[<|=|>]Timestamp

                        if (myLastModificationTime != null)
                            try
                            {
                                foreach (var _LastModificationTime in myLastModificationTime)
                                    switch (_LastModificationTime[0])
                                    {
                                        case '<': if (_INode.CreationTime < UInt64.Parse(_LastModificationTime.Substring(1))) _AddEntry = true; break;
                                        case '=': if (_INode.CreationTime == UInt64.Parse(_LastModificationTime.Substring(1))) _AddEntry = true; break;
                                        case '>': if (_INode.CreationTime > UInt64.Parse(_LastModificationTime.Substring(1))) _AddEntry = true; break;
                                        default: throw new GraphFSException_ParsedLastModificationTimeIsInvalid("The parameter 'LastModificationTime' could not be parsed!");
                                    }

                            }
                            catch (Exception)
                            {
                                throw new GraphFSException_ParsedLastModificationTimeIsInvalid("The parameter 'LastModificationTime' could not be parsed!");
                            }

                        #endregion

                        #region Parameter --DeletionTime=[<|=|>]Timestamp

                        if (myDeletionTime != null)
                            try
                            {
                                foreach (var _DeletionTime in myDeletionTime)
                                    switch (_DeletionTime[0])
                                    {
                                        case '<': if (_INode.DeletionTime < UInt64.Parse(_DeletionTime.Substring(1))) _AddEntry = true; break;
                                        case '=': if (_INode.DeletionTime == UInt64.Parse(_DeletionTime.Substring(1))) _AddEntry = true; break;
                                        case '>': if (_INode.DeletionTime > UInt64.Parse(_DeletionTime.Substring(1))) _AddEntry = true; break;
                                        default: throw new GraphFSException_ParsedDeletionTimeIsInvalid("The parameter 'DeletionTime' could not be parsed!");
                                    }

                            }
                            catch (Exception)
                            {
                                throw new GraphFSException_ParsedDeletionTimeIsInvalid("The parameter 'DeletionTime' could not be parsed!");
                            }

                        #endregion


                        if (_AddEntry)
                        {
                            __ExtendedDirectoryListing = _ExtendedDirectoryListing;
                            __ExtendedDirectoryListing.Size = _INode.ObjectSize;
                            __ExtendedDirectoryListing.Timestamp = _INode.LastModificationTime;
                            _Output.Add(__ExtendedDirectoryListing);
                        }

                    }

                    else
                    {
                        __ExtendedDirectoryListing = _ExtendedDirectoryListing;
                        __ExtendedDirectoryListing.Size = _INode.ObjectSize;
                        __ExtendedDirectoryListing.Timestamp = _INode.LastModificationTime;
                        _Output.Add(__ExtendedDirectoryListing);
                    }

                }

            }

            #endregion

            var _Exceptional = new Exceptional<IEnumerable<DirectoryEntryInformation>>();
            _Exceptional.Value = _Output;
            return _Exceptional;

        }

        #endregion


        #region RemoveDirectoryObject(myObjectLocation, myRemoveRecursive, mySessionToken)

        public Exceptional RemoveDirectoryObject(ObjectLocation myObjectLocation, Boolean myRemoveRecursive, SessionToken mySessionToken)
        {

            #region Initial Checks

            if (myObjectLocation == null)
                throw new ArgumentNullException("Parameter myObjectLocation must not be null!");
            
            if (mySessionToken == null)
                throw new ArgumentNullException("Parameter mySessionToken must not be null!");

            var _Exceptional = new Exceptional();

            #endregion

            #region Get a listing of the DirectoryObject

            var _DirectoryListingExceptional = GetExtendedDirectoryListing(myObjectLocation, mySessionToken);

            if (_DirectoryListingExceptional.Failed() || _DirectoryListingExceptional.Value == null)
            {
                return _DirectoryListingExceptional.Push(new GraphFSError_CouldNotGetDirectoryListing(myObjectLocation));
            }

            #endregion

            #region If the directory is not empty...

            if (_DirectoryListingExceptional.Value.ULongCount() > NUMBER_OF_DEFAULT_DIRECTORYENTRIES)
            {

                #region Fail if myRemoveRecursive == false

                if (!myRemoveRecursive)
                {
                    return _Exceptional.Push(new GraphFSError_DirectoryIsNotEmpty(myObjectLocation));
                }

                #endregion

                #region Remove all sub elements

                foreach (var _DirectoryEntryInformation in _DirectoryListingExceptional.Value)
                {

                    #region check if standard dir

                    // Hack: This should be done dynamically. 
                    if (!((_DirectoryEntryInformation.Name == FSConstants.DotLink)     ||
                          (_DirectoryEntryInformation.Name == FSConstants.DotDotLink)  ||
                          (_DirectoryEntryInformation.Name == FSConstants.DotMetadata) ||
                          (_DirectoryEntryInformation.Name == FSConstants.DotSystem)   ||
                          (_DirectoryEntryInformation.Name == FSConstants.DotStreams)))
                    {

                        #region Get a copy of all ObjectStreams
                        
                        var _ListOfStreams = new List<String>();

                        foreach (var _Stream in _DirectoryEntryInformation.Streams)
                            _ListOfStreams.Add(_Stream);

                        #endregion

                        foreach (var _StreamType in _ListOfStreams)
                        {

                            if (_StreamType == FSConstants.DIRECTORYSTREAM)
                            {
                                var _RemoveSubobjectExceptional = RemoveDirectoryObject(new ObjectLocation(myObjectLocation, _DirectoryEntryInformation.Name), true, mySessionToken);

                                // if (

                            }

                            else if (_StreamType == FSConstants.INLINEDATA)
                            {
                                // do nothing!
                            }

                            else
                            {

                                #region Get list of ObjectEditions and ObjectRevisionID to remove

                                var _ListOfObjectEditionsAndRevisionIDs = new List<KeyValuePair<String, ObjectRevisionID>>();

                                var _SubobjectLocatorExceptional = GetObjectLocator_protected(new ObjectLocation(myObjectLocation, _DirectoryEntryInformation.Name));

                                if (_SubobjectLocatorExceptional.Failed() || _SubobjectLocatorExceptional.Value == null)
                                {
                                    return _SubobjectLocatorExceptional.Push(new GraphFSError_CouldNotGetObjectLocator(new ObjectLocation(myObjectLocation, _DirectoryEntryInformation.Name)));
                                }

                                foreach (var _ObjectEditionKeyValuePair in _SubobjectLocatorExceptional.Value[_StreamType])
                                {
                                    foreach (var _ObjectRevisionKeyValuePair in _ObjectEditionKeyValuePair.Value)
                                    {
                                        _ListOfObjectEditionsAndRevisionIDs.Add(new KeyValuePair<String, ObjectRevisionID>(_ObjectEditionKeyValuePair.Key, _ObjectRevisionKeyValuePair.Key));
                                    }
                                }

                                #endregion

                                #region Finally remove them all...

                                foreach (var _ObjectEditionAndRevisionID in _ListOfObjectEditionsAndRevisionIDs)
                                {

                                    var _RemoveSubobjectExceptional = RemoveFSObject(new ObjectLocation(myObjectLocation, _DirectoryEntryInformation.Name), _StreamType, _ObjectEditionAndRevisionID.Key, _ObjectEditionAndRevisionID.Value, mySessionToken);

                                    if (_RemoveSubobjectExceptional.Failed())
                                    {
                                        return _RemoveSubobjectExceptional;
                                    }

                                }

                                #endregion

                            }                            

                        }

                    }

                    #endregion

                }

                #endregion
            
            }

            #endregion

            #region Remove the DirectoryObject itself!

            // To ensure optimistic concurrency: First get the latest RevisionID
            var _ObjectLocatorExceptional = GetObjectLocator_protected(myObjectLocation);

            if (_ObjectLocatorExceptional.Failed() || _ObjectLocatorExceptional.Value == null)
            {
                return _ObjectLocatorExceptional.Push(new GraphFSError_CouldNotGetObjectLocator(myObjectLocation));
            }

            // Remove the $DefaultEditon and $LatestRevision
            var _RemoveObjectExceptional = RemoveFSObject(myObjectLocation, FSConstants.DIRECTORYSTREAM, _ObjectLocatorExceptional.Value[FSConstants.DIRECTORYSTREAM].DefaultEditionName, _ObjectLocatorExceptional.Value[FSConstants.DIRECTORYSTREAM].DefaultEdition.LatestRevisionID, mySessionToken);

            if (_RemoveObjectExceptional.Failed())
            {
                return _RemoveObjectExceptional.Push(new GraphFSError_CouldNotRemoveDirectoryObject(myObjectLocation));
            }

            return _Exceptional;

            #endregion

        }

        #endregion

        #region EraseDirectoryObject(myObjectLocation, myEraseRecursive, mySessionToken)

        public Exceptional EraseDirectoryObject(ObjectLocation myObjectLocation, Boolean myEraseRecursive, SessionToken mySessionToken)
        {

            #region Erase recursive

            if (myEraseRecursive)
            {

                #region really erase recursive?

                if (GetDirectoryListing(myObjectLocation, mySessionToken).Value.LongCount().Equals((Int64)NUMBER_OF_DEFAULT_DIRECTORYENTRIES))
                {
                    return EraseFSObject(myObjectLocation, FSConstants.DIRECTORYSTREAM, null, null, mySessionToken);
                }

                #endregion

                else//yes, erase recursive!
                {

                    #region get directory information

                    var dirEntries = GetExtendedDirectoryListing(myObjectLocation, mySessionToken);

                    if (dirEntries == null)
                        return new Exceptional<Boolean>(new GraphFSError_DirectoryListingFailed(myObjectLocation));

                    #endregion

                    #region erase all sub elements

                    foreach (var aDirInformation in dirEntries.Value)
                    {

                        #region check if standard dir

                        //Hack: This should be done dynamically. 
                        if (!(aDirInformation.Name.Equals(FSConstants.DotLink) ||
                            aDirInformation.Name.Equals(FSConstants.DotDotLink) ||
                            aDirInformation.Name.Equals(FSConstants.DotMetadata) ||
                            aDirInformation.Name.Equals(FSConstants.DotUUID) ||
                            aDirInformation.Name.Equals(FSConstants.DotSystem) ||
                            aDirInformation.Name.Equals(FSConstants.DotStreams)))
                        {

                        #endregion

                            #region get deeper

                            if (aDirInformation.Streams.Contains(FSConstants.DIRECTORYSTREAM))
                            {

                                var _ChildIGraphFS = GetChildFileSystem(new ObjectLocation(myObjectLocation, aDirInformation.Name), false, mySessionToken);

                                if (_ChildIGraphFS != this)
                                {
                                    throw new GraphFSException_DeviceBusy("Device at mointpoint \"" + myObjectLocation + FSPathConstants.PathDelimiter + aDirInformation.Name + "\" busy!");
                                }

                                //if (!EraseDirectoryObject(new ObjectLocation(myObjectLocation, aDirInformation.Name), true, mySessionToken))
                                //{
                                //    throw new GraphFSException_DirectoryObjectIsNotEmpty("The directory \"" + myObjectLocation + FSPathConstants.PathDelimiter + aDirInformation.Name + "\" could not be erased.");
                                //}

                            }

                            #endregion

                            else
                            {
                                #region erase

                                foreach (var aStreamType in aDirInformation.Streams)
                                {
                                    if (!aStreamType.Equals(FSConstants.INLINEDATA))
                                    {
                                        EraseFSObject(new ObjectLocation(myObjectLocation, aDirInformation.Name), aStreamType, null, null, mySessionToken);
                                    }
                                }

                                #endregion
                            }
                        }
                    }

                    #endregion

                    #region erase the directory itself

                    List<String> streamTypes = new List<String>();

                    streamTypes.AddRange(GetObjectStreams(myObjectLocation, mySessionToken).Value);

                    foreach (String aStreamType in streamTypes)
                    {
                        EraseFSObject(myObjectLocation, aStreamType, null, null, mySessionToken);
                    }

                    #endregion
                }
            }
            #endregion

            // non-recursive
            else
            {
                if (GetDirectoryListing(myObjectLocation, mySessionToken).Value.LongCount() > (Int64)NUMBER_OF_DEFAULT_DIRECTORYENTRIES)
                    throw new GraphFSException_DirectoryObjectIsNotEmpty("This DirectoryObject is not empty!");

                return EraseFSObject(myObjectLocation, FSConstants.DIRECTORYSTREAM, null, null, mySessionToken);

            }

            return new Exceptional<Boolean>();


        }

        #endregion

        #endregion

        #region MetadataObject Methods

        #region SetMetadatum<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, myValue, myIndexSetStrategy, mySessionToken)

        public Exceptional SetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy, SessionToken mySessionToken)
        {

            #region Get or create MetadataObject

            var _MetadataObjectExceptional = GetOrCreateFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            if (_MetadataObjectExceptional.Failed() || _MetadataObjectExceptional.Value == null)
            {
                return _MetadataObjectExceptional.Push(new GraphFSError_CouldNotSetMetadata(myObjectLocation, myObjectStream, myObjectEdition));
            }

            #endregion

            #region If new, store the MetadataObject explicitly

            if (_MetadataObjectExceptional.Value.isNew)
            {

                _MetadataObjectExceptional.Value.Set(myKey, myValue, myIndexSetStrategy);
//                _MetadataObjectExceptional.Value.Save();

                using (var _StoreObjectExceptional = StoreAFSObject(myObjectLocation, _MetadataObjectExceptional.Value, true, mySessionToken))
                {
                    if (_StoreObjectExceptional.Failed())
                    {
                        return _MetadataObjectExceptional.Push(new GraphFSError_CouldNotStoreObject(myObjectLocation, myObjectStream, myObjectEdition));
                    }
                }

            }

            #endregion

            _MetadataObjectExceptional.Value.Set(myKey, myValue, myIndexSetStrategy);
            //_MetadataObjectExceptional.Value.Save();

            return StoreAFSObject(myObjectLocation, _MetadataObjectExceptional.Value, true, mySessionToken).
                       WhenFailed(e => e.Push(new GraphFSError_CouldNotStoreObject(myObjectLocation, myObjectStream, myObjectEdition)));

            //using (var _StoreObjectExceptional = StoreFSObject(myObjectLocation, _MetadataObjectExceptional.Value, true, mySessionToken))
            //{
            //    if (_StoreObjectExceptional == null || _StoreObjectExceptional.Failed)
            //    {
            //        var _Exceptional = new Exceptional();
            //        _Exceptional.Add(new GraphFSError_CouldNotStoreObject(myObjectLocation, myObjectStream, myObjectEdition));
            //        _Exceptional.Add(_MetadataObjectExceptional.Errors);
            //        return _Exceptional;
            //    }
            //}

            //return new Exceptional();

        }

        #endregion

        #region SetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myMetadata, myIndexSetStrategy, mySessionToken)

        public Exceptional SetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata, IndexSetStrategy myIndexSetStrategy, SessionToken mySessionToken)
        {


            var _MetadataObjectExceptional = GetOrCreateFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken).
                                                 WhenFailed(e => e.PushT(new GraphFSError_CouldNotSetMetadata(myObjectLocation, myObjectStream, myObjectEdition)));

            if (_MetadataObjectExceptional.Failed())
                return _MetadataObjectExceptional;



            //#region Get or create MetadataObject

            //var _MetadataObjectExceptional = GetOrCreateObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            //if (_MetadataObjectExceptional == null || _MetadataObjectExceptional.Failed || _MetadataObjectExceptional.Value == null)
            //{
            //    var _Exceptional = new Exceptional();
            //    _Exceptional.Add(new GraphFSError_CouldNotSetMetadata(myObjectLocation, myObjectStream, myObjectEdition));
            //    _Exceptional.Add(_MetadataObjectExceptional.Errors);
            //    return _Exceptional;
            //}

            //#endregion

            #region If new, store the MetadataObject within the cache

            // FIXME!
            _MetadataObjectExceptional.Value.Set(myMetadata, myIndexSetStrategy);

            if (_MetadataObjectExceptional.Value.isNew)
            {

                return StoreAFSObject(myObjectLocation, _MetadataObjectExceptional.Value, true, mySessionToken).
                           WhenFailed(e => e.Push(new GraphFSError_CouldNotStoreObject(myObjectLocation, myObjectStream, myObjectEdition)));


                //var _StoreObjectExceptional = StoreFSObject(myObjectLocation, _MetadataObjectExceptional.Value, true, mySessionToken);

                //if (_StoreObjectExceptional == null || _StoreObjectExceptional.Failed)
                //{
                //    var _Exceptional = new Exceptional();
                //    _Exceptional.Add(new GraphFSError_CouldNotStoreObject(myObjectLocation, myObjectStream, myObjectEdition));
                //    _Exceptional.Add(_MetadataObjectExceptional.Errors);
                //    return _Exceptional;
                //}

            }

            #endregion

            return new Exceptional();

        }

        #endregion


        #region MetadatumExists<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, myValue, mySessionToken)

        public Exceptional<Trinary> MetadatumExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue, SessionToken mySessionToken)
        {

            return GetFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken).
                       ConvertWithFunc(v => v.Contains(myKey, myValue)).
                       WhenFailed(e => e = new Exceptional<Trinary>(Trinary.FALSE));


            //var _MetadataObjectExceptional = GetObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            //if (_MetadataObjectExceptional == null || _MetadataObjectExceptional.Failed || _MetadataObjectExceptional.Value == null)
            //{
            //    _Exceptional.Value = Trinary.FALSE;
            //    _Exceptional.Add(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition));
            //    _Exceptional.Add(_MetadataObjectExceptional.Errors);
            //}

            //else
            //    _Exceptional.Value = _MetadataObjectExceptional.Value.Contains(myKey, myValue);

            //return _Exceptional;

        }

        #endregion

        #region MetadataExists<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, mySessionToken)

        public Exceptional<Trinary> MetadataExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, SessionToken mySessionToken)
        {

            return GetFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken).
                       ConvertWithFunc(v => v.ContainsKey(myKey)).
                       WhenFailed(e => e = new Exceptional<Trinary>(Trinary.FALSE));


            //var _Exceptional = new Exceptional<Trinary>();
            //var _MetadataObjectExceptional = GetObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            //if (_MetadataObjectExceptional == null || _MetadataObjectExceptional.Failed || _MetadataObjectExceptional.Value == null)
            //{
            //    _Exceptional.Value = Trinary.FALSE;
            //    _Exceptional.Add(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition));
            //    _Exceptional.Add(_MetadataObjectExceptional.Errors);
            //}

            //else
            //    _Exceptional.Value = _MetadataObjectExceptional.Value.ContainsKey(myKey);

            //return _Exceptional;

        }

        #endregion


        #region GetMetadatum<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, mySessionToken)

        public Exceptional<IEnumerable<TValue>> GetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, SessionToken mySessionToken)
        {

            return GetFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken).
                       ConvertWithFunc(v =>
                       {
                           var _ListOfValues = new List<TValue>();

                           foreach (var _item in v[myKey])
                           {
                               try
                               {
                                   _ListOfValues.Add((TValue)_item);
                               }
                               catch (Exception)
                               {
                               }
                           }

                           return (IEnumerable<TValue>) _ListOfValues;
                       }).
                       WhenFailed(e => e.PushT(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition)));



            //var _Exceptional = new Exceptional<IEnumerable<TValue>>();
            //var _MetadataObjectExceptional = GetObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            //if (_MetadataObjectExceptional == null || _MetadataObjectExceptional.Failed || _MetadataObjectExceptional.Value == null)
            //{
            //    _Exceptional.Push(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition));
            //}

            //else
            //{

            //    var _ListOfValues = new List<TValue>();

            //    foreach (var _item in _MetadataObjectExceptional.Value[myKey])
            //    {
            //        try
            //        {
            //            _ListOfValues.Add( (TValue) _item);
            //        }
            //        catch (Exception e)
            //        {
            //        }
            //    }

            //    _Exceptional.Value = _ListOfValues;

            //}

            //return _Exceptional;

        }

        #endregion

        #region GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, mySessionToken)

        public Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, SessionToken mySessionToken)
        {
            
            var _Exceptional = new Exceptional<IEnumerable<KeyValuePair<String, TValue>>>();
            var _MetadataObjectExceptional = GetFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            if (_MetadataObjectExceptional.Failed() || _MetadataObjectExceptional.Value == null)
            {
                _Exceptional = _MetadataObjectExceptional.Convert<IEnumerable<KeyValuePair<String,TValue>>>().
                                    PushT(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition));
            }

            else
            {

                var _ListOfKeyValuePairs = new List<KeyValuePair<String, TValue>>();

                foreach (var _KeyValuesPair in _MetadataObjectExceptional.Value)
                {
                    foreach (var _KeyValuePair in _KeyValuesPair.Value)
                    {
                        try
                        {
                            _ListOfKeyValuePairs.Add(new KeyValuePair<String, TValue>(_KeyValuesPair.Key, (TValue)_KeyValuePair));
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                _Exceptional.Value = _ListOfKeyValuePairs;

            }

            return _Exceptional;

        }

        #endregion

        #region GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myMinKey, myMaxKey, mySessionToken)

        public Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myMinKey, String myMaxKey, SessionToken mySessionToken)
        {

            var _Exceptional = new Exceptional<IEnumerable<KeyValuePair<String, TValue>>>();
            var _MetadataObjectExceptional = GetFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            if (_MetadataObjectExceptional.Failed() || _MetadataObjectExceptional.Value == null)
            {
                _Exceptional = _MetadataObjectExceptional.Convert<IEnumerable<KeyValuePair<String, TValue>>>().
                    PushT(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition));
            }

            else
            {

                var _ListOfKeyValuePairs = new List<KeyValuePair<String, TValue>>();

                foreach (var _KeyValuesPair in _MetadataObjectExceptional.Value)
                {
                    foreach (var _KeyValuePair in _KeyValuesPair.Value)
                    {
                        try
                        {
                            if (_KeyValuesPair.Key.CompareTo(myMinKey) >= 0 && _KeyValuesPair.Key.CompareTo(myMaxKey) <= 0)
                                _ListOfKeyValuePairs.Add(new KeyValuePair<String, TValue>(_KeyValuesPair.Key, (TValue) _KeyValuePair));
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                _Exceptional.Value = _ListOfKeyValuePairs;

            }

            return _Exceptional;

        }

        #endregion

        #region GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myFunc, mySessionToken)

        public Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region RemoveMetadatum<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, myValue, mySessionToken)

        public Exceptional RemoveMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue, SessionToken mySessionToken)
        {

            var _Exceptional = new Exceptional();
            var _MetadataObjectExceptional = GetFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            if (_MetadataObjectExceptional.Failed() || _MetadataObjectExceptional.Value == null)
            {
                _Exceptional = _MetadataObjectExceptional.Convert<IEnumerable<KeyValuePair<String, TValue>>>().
                    PushT(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition));
            }

            else
            {

                try
                {
                    _MetadataObjectExceptional.Value.Remove(myKey, myValue);
                }
                catch (Exception)
                {
                }

            }

            return _Exceptional;

        }

        #endregion

        #region RemoveMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, mySessionToken)

        public Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, SessionToken mySessionToken)
        {

            var _Exceptional = new Exceptional();
            var _MetadataObjectExceptional = GetFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            if (_MetadataObjectExceptional.Failed() || _MetadataObjectExceptional.Value == null)
            {
                _Exceptional = _MetadataObjectExceptional.Convert<IEnumerable<KeyValuePair<String, TValue>>>().
                    PushT(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition));
            }

            else
            {

                try
                {
                    _MetadataObjectExceptional.Value.Remove(myKey);
                }
                catch (Exception)
                {
                }

                var _StoreObjectExceptional = StoreAFSObject(myObjectLocation, _MetadataObjectExceptional.Value, true, mySessionToken);

                if (_StoreObjectExceptional.Failed())
                {
                    return _MetadataObjectExceptional.Convert<IEnumerable<KeyValuePair<String, TValue>>>().
                        PushT(new GraphFSError_CouldNotStoreObject(myObjectLocation, myObjectStream, myObjectEdition));
                }

            }

            return _Exceptional;

        }

        #endregion

        #region RemoveMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myMetadata, mySessionToken)

        public Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata, SessionToken mySessionToken)
        {

            var _Exceptional = new Exceptional<IEnumerable<KeyValuePair<String, TValue>>>();
            var _MetadataObjectExceptional = GetFSObject<MetadataObject<TValue>>(myObjectLocation, myObjectStream, myObjectEdition, null, 0, false, mySessionToken);

            if (_MetadataObjectExceptional.Failed() || _MetadataObjectExceptional.Value == null)
            {
                _Exceptional = _MetadataObjectExceptional.Convert<IEnumerable<KeyValuePair<String, TValue>>>().
                    PushT(new GraphFSError_MetadataObjectNotFound(myObjectLocation, myObjectStream, myObjectEdition));
            }

            else
            {

                foreach (var _KeyValuesPair in myMetadata)
                {   
                 
                    try
                    {
                        _MetadataObjectExceptional.Value.Remove(_KeyValuesPair.Key, _KeyValuesPair.Value);
                    }
                    catch (Exception)
                    {
                    }
                    
                }

            }

            return _Exceptional;

        }

        #endregion

        #region RemoveMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myFunc, mySessionToken)

        public Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion




        #region Stream methods

        #region OpenStream(mySessionToken, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy)

        public virtual Exceptional<IGraphFSStream> OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy)
        {
            throw new NotImplementedException("This method can not be used by this IGraphFS implementation!");
        }

        #endregion

        #region OpenStream(mySessionToken, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy, myFileMode, myFileAccess, myFileShare, myFileOptions, myBufferSize)

        public virtual Exceptional<IGraphFSStream> OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy,
                                           FileMode myFileMode, FileAccess myFileAccess, FileShare myFileShare, FileOptions myFileOptions, UInt64 myBufferSize)
        {
            throw new NotImplementedException("This method can not be used by this IGraphFS implementation!");
        }

        #endregion

        #endregion


        #region StorageEngine Maintenance

        #region StorageLocations(mySessionToken)

        public IEnumerable<String> StorageLocations(SessionToken mySessionToken)
        {
            return new List<String>();
        }

        #endregion

        #region StorageLocations(myObjectLocation, mySessionToken)

        public IEnumerable<String> StorageLocations(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {
            return new List<String>();
        }

        #endregion


        #region StorageUUIDs(mySessionToken)

        public IEnumerable<StorageUUID> StorageUUIDs(SessionToken mySessionToken)
        {
            return new List<StorageUUID>();
        }

        #endregion

        #region StorageUUIDs(myObjectLocation, mySessionToken)

        public IEnumerable<StorageUUID> StorageUUIDs(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {
            return new List<StorageUUID>();
        }

        #endregion


        #region StorageDescriptions(mySessionToken)

        public IEnumerable<String> StorageDescriptions(SessionToken mySessionToken)
        {
            return new List<String>();
        }

        #endregion

        #region StorageDescriptions(myObjectLocation, mySessionToken)

        public IEnumerable<String> StorageDescriptions(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {
            return new List<String>();
        }

        #endregion

        #endregion


        #region Transactions

        public FSTransaction BeginTransaction(SessionToken mySessionToken, Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null)
        {
            var _FSTransaction = new FSTransaction(myDistributed, myLongRunning, myIsolationLevel, myName);
            _FSTransaction.OnDispose += new TransactionDisposedHandler(TransactionOnDisposeHandler);
            return _FSTransaction;
        }

        // --------------------

        private void TransactionOnDisposeHandler(object sender, TransactionDisposedEventArgs args)
        {

            var _FSTransaction = args.Transaction;
            _FSTransaction.Rollback();//args.SessionToken);

        }

        //private void rollbackTransaction(FSTransaction myTransaction, SessionToken mySessionToken)
        //{

        //    //foreach (var keyValPair in myTransaction.TransactionDetails)
        //    //{

        //    //    var currentLocator = _ObjectCache.GetCachedObjectLocator(keyValPair.Key);

        //    //    #region If the ObjectLocator and TransactionUUID is not null, deallocate the ObjectLocator

        //    //    if (keyValPair.Value.ObjectLocator != null)
        //    //    {
        //    //        keyValPair.Value.ObjectLocator.TransactionUUID = null;
        //    //        _AllocationMap.Deallocate(ref keyValPair.Value.ObjectLocator.PreallocationTickets);

        //    //        if (ObjectExists(new ObjectLocation(keyValPair.Key), mySessionToken) == Trinary.TRUE)
        //    //        {
        //    //            if (currentLocator != null)
        //    //            {
        //    //                keyValPair.Value.ObjectLocator.CopyTo(currentLocator);
        //    //                keyValPair.Value.INode.CopyTo(currentLocator.INodeReference);
        //    //            }
        //    //            else
        //    //            {
        //    //                currentLocator = keyValPair.Value.ObjectLocator;
        //    //                _ObjectCache.StoreObjectLocator(keyValPair.Key, currentLocator);
        //    //            }
        //    //        }

        //    //    }
        //    //    if (keyValPair.Value.INode != null)
        //    //    {
        //    //        keyValPair.Value.INode.TransactionUUID = null;
        //    //        _AllocationMap.Deallocate(ref keyValPair.Value.INode.PreallocationTickets);
        //    //    }

        //    //    #endregion

        //    //    if (currentLocator == null) continue;

        //    //    #region Go through all streams and editions to deallocate the TransactionRevision

        //    //    foreach (KeyValuePair<String, ObjectStream> editionKeyValPair in currentLocator)
        //    //    {
        //    //        ObjectStream edition = editionKeyValPair.Value;
        //    //        foreach (ObjectEdition revision in edition.Values)
        //    //        {
        //    //            if (revision.HoldTransaction)
        //    //            {

        //    //                #region Deallocate the TransactionRevision

        //    //                AGraphObject GraphObject = ((StreamCacheEntry)_ObjectCache.RemoveTempEntry(revision.TransactionRevision.CacheUUID)).CachedAGraphObject;
        //    //                _AllocationMap.Deallocate(ref GraphObject.PreallocationTickets);

        //    //                #endregion

        //    //                GraphObject.TransactionUUID = null;

        //    //                #region Rollback the TransactionRevision

        //    //                revision.RollbackTransaction();

        //    //                #endregion

        //    //            }
        //    //        }

        //    //    }

        //    //    #endregion

        //    //    keyValPair.Value.ObjectLocator = null;
        //    //    keyValPair.Value.INode = null;

        //    //}

        //}

        ///// <summary>
        ///// Rolls back the current transaction
        ///// </summary>
        ///// <param name="mySessionToken"></param>
        //public void RollbackTransaction(SessionToken mySessionToken)
        //{

        //    //#region Get the current Transaction

        //    //FSTransaction lastFSTransaction;
        //    //lock (mySessionToken)
        //    //{
        //    //    lastFSTransaction = (FSTransaction)mySessionToken.RemoveTransaction();
        //    //}

        //    //#endregion

        //    //rollbackTransaction(lastFSTransaction, mySessionToken);

        //    //lastFSTransaction.Rollback();

        //}

        // /// <summary>
        ///// Commit the current transaction
        ///// </summary>
        ///// <param name="mySessionToken"></param>
        //public void CommitTransaction(SessionToken mySessionToken)
        //{

        //    //#region Get the current Transaction

        //    //FSTransaction lastFSTransaction;

        //    //lock (mySessionToken)
        //    //{
        //    //    lastFSTransaction = (FSTransaction) mySessionToken.RemoveTransaction();
        //    //}

        //    //if (lastFSTransaction == null)
        //    //    return;

        //    //#endregion

        //    //#region Go through all transactionDetails (locations)

        //    //foreach (var _TransactionDetail in lastFSTransaction.TransactionDetails)
        //    //{

        //    //    // put the ObjectLocator stored in Transaction into the cache
        //    //    var _CurrentObjectLocator = _ObjectCache.GetCachedObjectLocator(_TransactionDetail.Key);

        //    //    #region If the ObjectLocator and TransactionUUID is not null, the ObjectLocator changed in this transaction

        //    //    if (_TransactionDetail.Value.ObjectLocator != null && _TransactionDetail.Value.ObjectLocator.TransactionUUID != null)
        //    //    {

        //    //        _TransactionDetail.Value.INode.TransactionUUID = null;
        //    //        _TransactionDetail.Value.ObjectLocator.TransactionUUID = null;

        //    //        if (_CurrentObjectLocator == null)
        //    //        {
                        
        //    //            #region The ObjectLocator is new
                        
        //    //            _ObjectCache.StoreObjectLocator(_TransactionDetail.Key, _TransactionDetail.Value.ObjectLocator, DirectoryHelper.GetObjectPath(_TransactionDetail.Value.ObjectLocator.ObjectLocation));
        //    //            _CurrentObjectLocator = _TransactionDetail.Value.ObjectLocator;

        //    //            #endregion

        //    //        }
        //    //        else
        //    //        {

        //    //            #region The ObjectLocator already exist and need to be updated

        //    //            #region Deallocate currentLocator before overwrite it from Transactional locator

        //    //            _AllocationMap.Deallocate(ref _CurrentObjectLocator.PreallocationTickets);
        //    //            _AllocationMap.Deallocate(ref _CurrentObjectLocator.INodeReference.PreallocationTickets);

        //    //            #endregion

        //    //            _TransactionDetail.Value.ObjectLocator.CopyTo(_CurrentObjectLocator);
        //    //            _TransactionDetail.Value.INode.CopyTo(_CurrentObjectLocator.INodeReference);

        //    //            if (_TransactionDetail.Value.WasRemoved)
        //    //            {
        //    //                _ObjectCache.RemoveObjectLocator(_CurrentObjectLocator.ObjectLocation, true);
        //    //            }

        //    //            #endregion

        //    //        }
        //    //        //Debug.WriteLine("\t\t StoreObjectLocator[" + keyValPair.Key + "] " + currentLocator.ToString());

        //    //    }

        //    //    #endregion

        //    //    if (_CurrentObjectLocator.INodeReference.ObjectLocatorStates == ObjectLocatorStates.Erased)
        //    //    {

        //    //        foreach (var _ObjectEditions in _CurrentObjectLocator.Values)
        //    //        {
        //    //            foreach (var _ObjectRevision in _ObjectEditions.Values)
        //    //            {
        //    //                if (_ObjectRevision.HoldTransaction)
        //    //                {

        //    //                    AGraphObject _AGraphObject = _ObjectCache.GetAGraphObject(_ObjectRevision.TransactionRevision.CacheUUID);
        //    //                    _AllocationMap.Deallocate(ref _AGraphObject.PreallocationTickets);
        //    //                    _ObjectCache.RemoveTempEntry(_ObjectRevision.TransactionRevision.CacheUUID);
        //    //                    _AGraphObject = null;

        //    //                }
        //    //            }
        //    //        }

        //    //        _AllocationMap.Deallocate(ref _CurrentObjectLocator.PreallocationTickets);
        //    //        _CurrentObjectLocator = null;
        //    //        lastFSTransaction.Commit();
                 
        //    //        continue;

        //    //    }

        //    //    #region Go through all streams and editions to move the transactional ObjectStreams (GraphObjects) from temp to the cache

        //    //    foreach (KeyValuePair<String, ObjectStream> editionKeyValPair in _CurrentObjectLocator)
        //    //    {
        //    //        ObjectStream edition = editionKeyValPair.Value;
        //    //        foreach (ObjectEdition revision in edition.Values)
        //    //        {
        //    //            if (revision.HoldTransaction)
        //    //            {

        //    //                #region Commit the TransactionRevision and move the Object from Cache.temp to cache

        //    //                //Debug.WriteLine("\t\t commit cacheuuid " + revision.TransactionRevision.CacheUUID);
        //    //                revision.CommitTransaction();
        //    //                //Debug.WriteLine("\t\t copy to cache " + revision.LatestRevision.CacheUUID);
        //    //                AGraphObject GraphObject = ((StreamCacheEntry)_ObjectCache.MoveTempEntryToCache(revision.LatestRevision.CacheUUID)).CachedAGraphObject;
        //    //                GraphObject.TransactionUUID = null;
                            
        //    //                #endregion

        //    //            }
        //    //        }

        //    //        #region Delete old revision greater than MaxNumberOfRevisions

        //    //        while (edition.DefaultEdition.MaxNumberOfRevisions < edition.DefaultEdition.GetMaxPathLength(edition.DefaultEdition.LatestRevisionID))
        //    //        {
        //    //            DeleteOldestObjectRevision(_CurrentObjectLocator, editionKeyValPair.Key, edition.DefaultEditionName, mySessionToken);
        //    //        }

        //    //        #endregion
        //    //    }

        //    //    #endregion

        //    //    #region Update the global _AllocationMap (Locator, INode) if the Objectlocator contains the AllocationMap

        //    //    //TODO: change this if there are more than one AllocationMap

        //    //    if (_CurrentObjectLocator.ContainsKey(FSConstants.ALLOCATIONMAPSTREAM)
        //    //        && _TransactionDetail.Value.ObjectLocator != null && _TransactionDetail.Value.INode != null)
        //    //    {
        //    //        //if (_AllocationMapLocator != null)
        //    //        //{
        //    //        //    currentLocator.CopyTo(_AllocationMapLocator);
        //    //        //}
        //    //        //else
        //    //        //{
        //    //        _AllocationMapLocator = _CurrentObjectLocator;
        //    //            _AllocationMap.ObjectLocatorReference = _AllocationMapLocator;
        //    //        //}

        //    //        //if (_AllocationMapINode != null)
        //    //        //{
        //    //        //    keyValPair.Value.INode.CopyTo(_AllocationMapINode);
        //    //        //}
        //    //        //else
        //    //        //{
        //    //        _AllocationMapINode = _TransactionDetail.Value.INode;
        //    //            _AllocationMap.INodeReference = _AllocationMapINode;
        //    //        //}
        //    //    }

        //    //    #endregion

        //    //}

        //    //#endregion

        //    //lastFSTransaction.Commit();

        //}

        #endregion


        public void FlushObjectLocationNew(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }


        


        #region ResolveObjectLocation(myObjectLocation, ..., SessionToken mySessionToken)

        //public abstract ResolveTypes ResolveObjectLocation_Internal(ref ObjectLocation myObjectLocation, out IEnumerable<String> myObjectStreams, out ObjectLocation myObjectPath, out String myObjectName, out IDirectoryObject myIDirectoryObject, ref List<String> mySymlinkTargets, SessionToken mySessionToken);
        //public abstract ResolveTypes ResolveObjectLocationRecursive_Internal(ref ObjectLocation myObjectLocation, out IEnumerable<String> myObjectStreams, out ObjectLocation myObjectPath, out String myObjectName, out IDirectoryObject myIDirectoryObject, out IGraphFS myIGraphFS, ref List<String> mySymlinkTargets, SessionToken mySessionToken);
        //public abstract Trinary ResolveObjectLocation(ref ObjectLocation myObjectLocation, out IEnumerable<String> myObjectStreams, out ObjectLocation myObjectPath, out String myObjectName, out IDirectoryObject myIDirectoryObject, out IGraphFS myIGraphFS, SessionToken mySessionToken);
        //public abstract ObjectLocation ResolveObjectLocation(ObjectLocation myObjectLocation, Boolean myThrowObjectNotFoundException, SessionToken mySessionToken);

        #endregion


        #region NotificationDispatcher

        // The NotificationDispatcher handles all kind of notification between system parts or other dispatchers.
        // Use register to get notified as recipient.
        // Use SendNotification to send a notification to all subscribed recipients.

        #region GetNotificationDispatcher(SessionToken mySessionToken)

        /// <summary>
        /// Returns the NotificationDispatcher of this file system.
        /// </summary>
        /// <returns>The NotificationDispatcher of this file system</returns>
        public NotificationDispatcher GetNotificationDispatcher(SessionToken mySessionToken)
        {
            return NotificationDispatcher;
        }

        #endregion

        #region GetNotificationDispatcher(myObjectLocation, mySessionToken)

        /// <summary>
        /// Returns the NotificationDispatcher of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The NotificationDispatcher of the file system at the given ObjectLocation</returns>
        public NotificationDispatcher GetNotificationDispatcher(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {

            var _PathLength = Int32.MinValue;
            var _NotificationDispatcher = GetNotificationDispatcher(mySessionToken);

            foreach (var __Mountpoint_IGraphFS in _GraphFSLookuptable.MountedFSs)
            {

                if (myObjectLocation.StartsWith(__Mountpoint_IGraphFS.Key) &&
                    (_PathLength < __Mountpoint_IGraphFS.Key.Length))
                {
                    _PathLength = __Mountpoint_IGraphFS.Key.Length;
                    _NotificationDispatcher = __Mountpoint_IGraphFS.Value.GetNotificationDispatcher(mySessionToken);
                }

            }

            return _NotificationDispatcher;
        }

        #endregion


        #region SetNotificationDispatcher(myNotificationDispatcher, mySessionToken)

        /// <summary>
        /// Sets the NotificationDispatcher of this file system.
        /// </summary>
        /// <param name="myNotificationDispatcher">A NotificationDispatcher object</param>
        public void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher, SessionToken mySessionToken)
        {
            NotificationDispatcher = myNotificationDispatcher;
        }

        #endregion

        #region SetNotificationDispatcher(myObjectLocation, myNotificationDispatcher, SessionToken mySessionToken)

        /// <summary>
        /// Sets the NotificationDispatcher of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myNotificationDispatcher">A NotificationDispatcher object</param>
        public void SetNotificationDispatcher(ObjectLocation myObjectLocation, NotificationDispatcher myNotificationDispatcher, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region GetNotificationSettings(SessionToken mySessionToken)

        /// <summary>
        /// Returns the NotificationDispatcher settings of this file system
        /// </summary>
        /// <returns>The NotificationDispatcher settings of this file system</returns>
        public NotificationSettings GetNotificationSettings(SessionToken mySessionToken)
        {
            return NotificationSettings;
        }

        #endregion

        #region GetNotificationSettings(myObjectLocation, SessionToken mySessionToken)

        /// <summary>
        /// Returns the NotificationDispatcher settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The NotificationDispatcher settings of the file system at the given ObjectLocation</returns>
        public NotificationSettings GetNotificationSettings(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SetNotificationSettings(myNotificationSettings, SessionToken mySessionToken)

        /// <summary>
        /// Sets the NotificationDispatcher settings of this file system
        /// </summary>
        /// <param name="myNotificationSettings">A NotificationSettings object</param>
        public void SetNotificationSettings(NotificationSettings myNotificationSettings, SessionToken mySessionToken)
        {
            NotificationSettings = myNotificationSettings;
        }

        #endregion

        #region SetNotificationSettings(myObjectLocation, myNotificationSettings, SessionToken mySessionToken)

        /// <summary>
        /// Sets the NotificationDispatcher settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myNotificationSettings">A NotificationSettings object</param>
        public void SetNotificationSettings(ObjectLocation myObjectLocation, NotificationSettings myNotificationSettings, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


        #region IGraphFS Members

        bool IGraphFS.SetFileSystemDescription(string myFileSystemDescription, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        bool IGraphFS.SetFileSystemDescription(ObjectLocation myObjectLocation, string myFileSystemDescription, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        Exceptional IGraphFS.LockFSObject(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, ObjectRevisionID myObjectRevisionID, ObjectLocks myObjectLock, ObjectLockTypes myObjectLockType, ulong myLockingTime, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion


    }

}

