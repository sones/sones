/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/*
 * GraphFSSession
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.IO;
using System.Collections.Generic;

using sones.Notifications;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;

using sones.StorageEngines;
using sones.GraphFS.Caches;
using sones.GraphFS.InternalObjects;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Errors;
using sones.Lib.Session;
using sones.GraphFS.Transactions;
using System.Text;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.WeakReference;

#endregion

namespace sones.GraphFS.Session
{

    /// <summary>
    /// The session interface for all graph file systems
    /// </summary>

    public class GraphFSSession : IGraphFSSession
    {

        #region Data

        private IGraphFS _IGraphFS;

        #endregion

        #region Implemenation

        public String Implemenation
        {
            get
            {
                return _IGraphFS.GetType().Name;
            }
        }

        #endregion

        #region SessionToken

        private SessionToken _SessionToken;

        public SessionToken SessionToken
        {
            get
            {
                return _SessionToken;
            }
        }

        #endregion


        #region Constructors

        #region GraphFSSession(myPandoraFS, myUsername)

        /// <summary>
        /// This will create a new PandoraFS session on an existing PandoraFS and will verify the given credentials
        /// </summary>
        /// <param name="myPandoraFS"></param>
        /// <param name="myUsername"></param>
        public GraphFSSession(IGraphFS myIPandoraFS, String myUsername)
        {

            _IGraphFS = myIPandoraFS;
            var sessionInfo = new FSSessionInfo(myUsername);            
            _SessionToken   = new SessionToken(sessionInfo);

        }

        #endregion

        #endregion


        #region Session specific Members

        public IGraphFSSession CreateNewSession(String myUsername)
        {
            return new GraphFSSession(_IGraphFS, myUsername);
        }

        #endregion


        #region IGraphFSSession Members

        #region Information Methods

        #region IsPersistent

        public Boolean IsPersistent { get { return _IGraphFS.IsPersistent; } }

        #endregion

        public Boolean isMounted
        {
            get { return _IGraphFS.isMounted; }
        }

        public FileSystemUUID GetFileSystemUUID()
        {
            return _IGraphFS.GetFileSystemUUID(_SessionToken);
        }

        public FileSystemUUID GetFileSystemUUID(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetFileSystemUUID(myObjectLocation, _SessionToken);
        }

        public IEnumerable<FileSystemUUID> GetFileSystemUUIDs(Boolean myRecursiveOperation)
        {
            return _IGraphFS.GetFileSystemUUIDs(myRecursiveOperation, _SessionToken);
        }

        public String GetFileSystemDescription()
        {
            return _IGraphFS.GetFileSystemDescription(_SessionToken);
        }

        public String GetFileSystemDescription(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetFileSystemDescription(myObjectLocation, _SessionToken);
        }

        public IEnumerable<String> GetFileSystemDescriptions(Boolean myRecursiveOperation)
        {
            return _IGraphFS.GetFileSystemDescriptions(myRecursiveOperation, _SessionToken);
        }

        public void SetFileSystemDescription(String myFileSystemDescription)
        {
            _IGraphFS.SetFileSystemDescription(myFileSystemDescription, _SessionToken);
        }

        public void SetFileSystemDescription(ObjectLocation myObjectLocation, String myFileSystemDescription)
        {
            _IGraphFS.SetFileSystemDescription(myObjectLocation, myFileSystemDescription, _SessionToken);
        }

        public UInt64 GetNumberOfBytes()
        {
            return _IGraphFS.GetNumberOfBytes(_SessionToken);
        }

        public UInt64 GetNumberOfBytes(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetNumberOfBytes(myObjectLocation, _SessionToken);
        }

        public IEnumerable<UInt64> GetNumberOfBytes(Boolean myRecursiveOperation)
        {
            return _IGraphFS.GetNumberOfBytes(myRecursiveOperation, _SessionToken);
        }

        public UInt64 GetNumberOfFreeBytes()
        {
            return _IGraphFS.GetNumberOfFreeBytes(_SessionToken);
        }

        public UInt64 GetNumberOfFreeBytes(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetNumberOfFreeBytes(myObjectLocation, _SessionToken);
        }

        public IEnumerable<UInt64> GetNumberOfFreeBytes(Boolean myRecursiveOperation)
        {
            return _IGraphFS.GetNumberOfFreeBytes(myRecursiveOperation, _SessionToken);
        }

        public AccessModeTypes GetAccessMode()
        {
            return _IGraphFS.GetAccessMode(_SessionToken);
        }

        public AccessModeTypes GetAccessMode(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetAccessMode(myObjectLocation, _SessionToken);
        }

        public IEnumerable<AccessModeTypes> GetAccessModes(Boolean myRecursiveOperation)
        {
            return _IGraphFS.GetAccessModes(myRecursiveOperation, _SessionToken);
        }

#warning "Change to method"
        public IGraphFS ParentFileSystem
        {
            get
            {
                return _IGraphFS.ParentFileSystem;
            }
            set
            {
                _IGraphFS.ParentFileSystem = value;
            }
        }

        public IEnumerable<ObjectLocation> GetChildFileSystemMountpoints(Boolean myRecursiveOperation)
        {
            return _IGraphFS.GetChildFileSystemMountpoints(myRecursiveOperation, _SessionToken);
        }

        public IGraphFS GetChildFileSystem(ObjectLocation myObjectLocation, Boolean myRecursive)
        {
            return _IGraphFS.GetChildFileSystem(myObjectLocation, myRecursive, _SessionToken);
        }

        public NotificationDispatcher GetNotificationDispatcher()
        {
            return _IGraphFS.GetNotificationDispatcher(_SessionToken);
        }

        public NotificationDispatcher GetNotificationDispatcher(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetNotificationDispatcher(myObjectLocation, _SessionToken);
        }

        public NotificationSettings GetNotificationSettings()
        {
            return _IGraphFS.GetNotificationSettings(_SessionToken);
        }

        public NotificationSettings GetNotificationSettings(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetNotificationSettings(myObjectLocation, _SessionToken);
        }

        public void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher)
        {
            _IGraphFS.SetNotificationDispatcher(myNotificationDispatcher, _SessionToken);
        }

        public void SetNotificationDispatcher(ObjectLocation myObjectLocation, NotificationDispatcher myNotificationDispatcher)
        {
            _IGraphFS.SetNotificationDispatcher(myObjectLocation, myNotificationDispatcher, _SessionToken);
        }

        public void SetNotificationSettings(NotificationSettings myNotificationSettings)
        {
            _IGraphFS.SetNotificationSettings(myNotificationSettings, _SessionToken);
        }

        public void SetNotificationSettings(ObjectLocation myObjectLocation, NotificationSettings myNotificationSettings)
        {
            _IGraphFS.SetNotificationSettings(myObjectLocation, myNotificationSettings, _SessionToken);
        }

        #endregion

        #region ObjectCache

        public ObjectCacheSettings GetObjectCacheSettings()
        {
            return _IGraphFS.GetObjectCacheSettings(_SessionToken);
        }

        public ObjectCacheSettings GetObjectCacheSettings(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetObjectCacheSettings(myObjectLocation, _SessionToken);
        }

        public void SetObjectCacheSettings(ObjectCacheSettings myObjectCacheSettings)
        {
            _IGraphFS.SetObjectCacheSettings(myObjectCacheSettings, _SessionToken);
        }

        public void SetObjectCacheSettings(ObjectLocation myObjectLocation, ObjectCacheSettings myObjectCacheSettings)
        {
            _IGraphFS.SetObjectCacheSettings(myObjectLocation, myObjectCacheSettings, _SessionToken);
        }

        #endregion

        #region MakeFileSystem

        public Exceptional<FileSystemUUID> MakeFileSystem(String myStorageLocation, String myDescription, UInt64 myNumberOfBytes, Boolean myOverwriteExistingFileSystem, Action<Double> myAction)
        {

            var _MKFSBufferSize  = 1 * 1024 * 1024u;
            var _IStorageEngines = new List<IStorageEngine>();

            if (myOverwriteExistingFileSystem)
                _IStorageEngines.Add(StorageEngineFactory.Instance.CreateIStorageEngine(myStorageLocation, myNumberOfBytes, _MKFSBufferSize, myOverwriteExistingFileSystem, myAction));
            
            else
                _IStorageEngines.Add(StorageEngineFactory.Instance.ActivateIStorageEngine(myStorageLocation).Value);

            var _Exceptional = _IGraphFS.MakeFileSystem(_IStorageEngines, myDescription, myOverwriteExistingFileSystem, myAction, _SessionToken);

            foreach (var _IStorageEngine in _IStorageEngines)
                _IStorageEngine.DetachStorage();

            return _Exceptional;

        }

        public void GrowFileSystem(UInt64 myNumberOfBytesToAdd)
        {
            _IGraphFS.GrowFileSystem(myNumberOfBytesToAdd, _SessionToken);
        }

        public void ShrinkFileSystem(UInt64 myNumberOfBytesToRemove)
        {
            _IGraphFS.ShrinkFileSystem(myNumberOfBytesToRemove, _SessionToken);
        }

        #endregion

        #region Mount

        public void MountFileSystem(String myStorageLocation, AccessModeTypes myFSAccessMode)
        {
            _IGraphFS.MountFileSystem(myStorageLocation, myFSAccessMode, _SessionToken);
        }

        public void MountFileSystem(String myStorageLocation, ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode)
        {
            _IGraphFS.MountFileSystem(myStorageLocation, myMountPoint, myFSAccessMode, _SessionToken);
        }


        public void RemountFileSystem(AccessModeTypes myFSAccessMode)
        {
            _IGraphFS.RemountFileSystem(myFSAccessMode, _SessionToken);
        }

        public void RemountFileSystem(ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode)
        {
            _IGraphFS.RemountFileSystem(myMountPoint, myFSAccessMode, _SessionToken);
        }


        public void UnmountFileSystem()
        {
            _IGraphFS.UnmountFileSystem(_SessionToken);
        }

        public void UnmountFileSystem(ObjectLocation myMountPoint)
        {
            _IGraphFS.UnmountFileSystem(myMountPoint, _SessionToken);
        }

        public void UnmountAllFileSystems()
        {
            _IGraphFS.UnmountAllFileSystems(_SessionToken);
        }

        #endregion

        public void ChangeRootDirectory(String myChangeRootPrefix)
        {
            _IGraphFS.ChangeRootDirectory(myChangeRootPrefix, _SessionToken);
        }


        public Trinary ResolveObjectLocation(ref ObjectLocation myObjectLocation, out IEnumerable<String> myObjectStreams, out ObjectLocation myObjectPath, out String myObjectName, out IDirectoryObject myIDirectoryObject, out IGraphFS myIPandoraFS)
        {
            return _IGraphFS.ResolveObjectLocation(ref myObjectLocation, out myObjectStreams, out myObjectPath, out myObjectName, out myIDirectoryObject, out myIPandoraFS, _SessionToken);
        }

        public String ResolveObjectLocation(ObjectLocation myObjectLocation, Boolean myThrowObjectNotFoundException)
        {
            return _IGraphFS.ResolveObjectLocation(myObjectLocation, myThrowObjectNotFoundException, _SessionToken);
        }



        #region INode and ObjectLocator

        #region GetINode(myObjectLocation)

        public Exceptional<INode> GetINode(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetINode(myObjectLocation, _SessionToken);
        }

        #endregion

        #region GetObjectLocator(ObjectLocation)

        public Exceptional<ObjectLocator> GetObjectLocator(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetObjectLocator(myObjectLocation, _SessionToken);
        }

        #endregion

        #endregion

        #region Object specific methods

        #region LockObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectLock, myObjectLockType, myLockingTime)

        public Exceptional LockObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, ObjectLocks myObjectLock, ObjectLockTypes myObjectLockType, UInt64 myLockingTime)
        {
            return _IGraphFS.LockObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectLock, myObjectLockType, myLockingTime, _SessionToken);
        }

        #endregion


        #region GetOrCreateObject<PT>(myObjectLocation)

        public Exceptional<PT> GetOrCreateObject<PT>(ObjectLocation myObjectLocation) where PT : AFSObject, new()
        {
            var _APandoraObject = _IGraphFS.GetOrCreateObject<PT>(myObjectLocation, null, null, null, 0, false, _SessionToken);
            if (_APandoraObject != null && _APandoraObject.Value != null)
                _APandoraObject.Value.IGraphFSSessionReference = this;
            return _APandoraObject;
        }

        #endregion

        #region GetOrCreateObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures)

        public Exceptional<PT> GetOrCreateObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, RevisionID myObjectRevisionID = null, ulong myObjectCopy = 0, bool myIgnoreIntegrityCheckFailures = false) where PT : AFSObject, new()
        {

            var _APandoraObject = _IGraphFS.GetOrCreateObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, _SessionToken);

            if (_APandoraObject != null && _APandoraObject.Value != null)
            {
                _APandoraObject.Value.IGraphFSSessionReference = new WeakReference<IGraphFSSession>(this);
                _APandoraObject.Value.IGraphFSReference        = new WeakReference<IGraphFS>(_IGraphFS);
            }

            return _APandoraObject;

        }

        #endregion

        #region GetOrCreateObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myFunc, _SessionToken)

        public Exceptional<PT> GetOrCreateObject<PT>(ObjectLocation myObjectLocation, string myObjectStream, Func<PT> myFunc, string myObjectEdition = FSConstants.DefaultEdition, RevisionID myObjectRevisionID = null, ulong myObjectCopy = 0, bool myIgnoreIntegrityCheckFailures = false) where PT : AFSObject
        {
            var _APandoraObject = _IGraphFS.GetOrCreateObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myFunc, _SessionToken);
            if (_APandoraObject != null && _APandoraObject.Value != null)
                _APandoraObject.Value.IGraphFSSessionReference = new WeakReference<IGraphFSSession>(this);
            return _APandoraObject;
        }

        #endregion


        #region GetObject<PT>(myObjectLocation)

        public Exceptional<PT> GetObject<PT>(ObjectLocation myObjectLocation) where PT : AFSObject, new()
        {
            var _APandoraObject = _IGraphFS.GetObject<PT>(myObjectLocation, null, null, null, 0, false, _SessionToken);
            if (_APandoraObject != null && _APandoraObject.Value != null)
                _APandoraObject.Value.IGraphFSSessionReference = new WeakReference<IGraphFSSession>(this);
            return _APandoraObject;
        }

        #endregion

        #region GetObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures)

        public Exceptional<PT> GetObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, RevisionID myObjectRevisionID = null, ulong myObjectCopy = 0, bool myIgnoreIntegrityCheckFailures = false) where PT : AFSObject, new()
        {
            var _APandoraObject = _IGraphFS.GetObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, _SessionToken);
            if (_APandoraObject != null && _APandoraObject.Value != null)
                _APandoraObject.Value.IGraphFSSessionReference = new WeakReference<IGraphFSSession>(this);
            return _APandoraObject;
        }

        #endregion

        #region GetObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myFunc, _SessionToken)

        public Exceptional<PT> GetObject<PT>(ObjectLocation myObjectLocation, string myObjectStream, Func<PT> myFunc, string myObjectEdition = FSConstants.DefaultEdition, RevisionID myObjectRevisionID = null, ulong myObjectCopy = 0, bool myIgnoreIntegrityCheckFailures = false) where PT : AFSObject
        {
            var _APandoraObject = _IGraphFS.GetObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myFunc, _SessionToken);
            if (_APandoraObject != null && _APandoraObject.Value != null)
                _APandoraObject.Value.IGraphFSSessionReference = new WeakReference<IGraphFSSession>(this);
            return _APandoraObject;
        }

        #endregion


        #region StoreObject(myAPandoraObject, myAllowOverwritting)

        public Exceptional StoreFSObject(AFSObject myAPandoraObject, Boolean myAllowOverwritting = false)
        {

            myAPandoraObject.IGraphFSSessionReference = new WeakReference<IGraphFSSession>(this);

            return _IGraphFS.StoreFSObject(myAPandoraObject.ObjectLocation, myAPandoraObject, myAllowOverwritting, _SessionToken);

        }

        #endregion


        #region ObjectExists(params myObjectLocation)

        public Exceptional<Trinary> ObjectExists(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.ObjectExists(myObjectLocation, _SessionToken);
        }

        #endregion

        #region ObjectStreamExists(ObjectLocation, myObjectStream)

        public Exceptional<Trinary> ObjectStreamExists(ObjectLocation myObjectLocation, String myObjectStream)
        {
            return _IGraphFS.ObjectStreamExists(myObjectLocation, myObjectStream, _SessionToken);
        }

        #endregion

        #region ObjectEditionExists(myObjectLocation, myObjectStream, myObjectEdition)

        public Exceptional<Trinary> ObjectEditionExists(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition)
        {
            return _IGraphFS.ObjectEditionExists(myObjectLocation, myObjectStream, myObjectEdition, _SessionToken);
        }

        #endregion

        #region ObjectRevisionExists(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID)

        public Exceptional<Trinary> ObjectRevisionExists(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, RevisionID myObjectRevisionID = null)
        {
            return _IGraphFS.ObjectRevisionExists(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, _SessionToken);
        }

        #endregion


        #region GetObjectStreams(myObjectLocation)

        public Exceptional<IEnumerable<String>> GetObjectStreams(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetObjectStreams(myObjectLocation, _SessionToken);
        }

        #endregion

        #region GetObjectEditions(myObjectLocation, myObjectStream)

        public Exceptional<IEnumerable<String>> GetObjectEditions(ObjectLocation myObjectLocation, String myObjectStream)
        {
            return _IGraphFS.GetObjectEditions(myObjectLocation, myObjectStream, _SessionToken);
        }

        #endregion

        #region GetObjectRevisionIDs(myObjectLocation, myObjectStream, myObjectEdition)

        public Exceptional<IEnumerable<RevisionID>> GetObjectRevisionIDs(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition)
        {
            return _IGraphFS.GetObjectRevisionIDs(myObjectLocation, myObjectStream, myObjectEdition, _SessionToken);
        }

        #endregion


        #region RenameObject(myObjectLocation, myNewObjectName)

        public Exceptional RenameObject(ObjectLocation myObjectLocation, String myNewObjectName)
        {
            return _IGraphFS.RenameObject(myObjectLocation, myNewObjectName, _SessionToken);
        }

        #endregion

        #region RemoveObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID)

        public Exceptional RemoveObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, RevisionID myObjectRevisionID = null)
        {

            lock (this)
            {

                var _Exceptional = new Exceptional();

                if (myObjectEdition == null || myObjectRevisionID == null)
                {

                    return _IGraphFS.GetObjectLocator(myObjectLocation, _SessionToken).
                        WhenFailed(e => e.PushT(new GraphFSError_CouldNotGetObjectLocator(myObjectLocation))).
                        WhenSucceded<ObjectLocator>(e =>
                        {
                            if (myObjectEdition == null)
                                myObjectEdition = e.Value[myObjectStream].DefaultEditionName;

                            if (myObjectRevisionID == null)
                                myObjectRevisionID = e.Value[myObjectStream].DefaultEdition.LatestRevisionID;

                            return _IGraphFS.RemoveObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, _SessionToken).
                                       Convert<ObjectLocator>();

                        });

                }

                return _IGraphFS.RemoveObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, _SessionToken);

            }

        }

        #endregion

        #region EraseObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID)

        public Exceptional EraseObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID)
        {
            return _IGraphFS.EraseObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, _SessionToken);
        }

        #endregion

        #endregion


        #region Symlink

        public Exceptional AddSymlink(ObjectLocation myObjectLocation, ObjectLocation myTargetLocation)
        {
            return _IGraphFS.AddSymlink(myObjectLocation, myTargetLocation, _SessionToken);
        }

        public Exceptional AddSymlink(ObjectLocation myObjectLocation, AFSObject myTargetAFSObject)
        {
            return _IGraphFS.AddSymlink(myObjectLocation, myTargetAFSObject.ObjectLocation, _SessionToken);
        }

        public Exceptional<Trinary> isSymlink(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.isSymlink(myObjectLocation, _SessionToken);
        }

        public Exceptional<ObjectLocation> GetSymlink(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetSymlink(myObjectLocation, _SessionToken);
        }

        public Exceptional RemoveSymlink(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.RemoveSymlink(myObjectLocation, _SessionToken);
        }

        #endregion

        #region DirectoryObject

        #region CreateDirectoryObject(myObjectLocation)

        public Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation)
        {

            var _DirectoryObject = _IGraphFS.CreateDirectoryObject(myObjectLocation, 0, _SessionToken);
            //_DirectoryObject.IGraphFSSessionReference = this;

            return _DirectoryObject;

        }

        #endregion

        #region CreateDirectoryObject(myObjectLocation, myBlocksize)

        public Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation, UInt64 myBlocksize)
        {

            var _DirectoryObject = _IGraphFS.CreateDirectoryObject(myObjectLocation, myBlocksize, _SessionToken);
            //_DirectoryObject.IGraphFSSessionReference = this;

            return _DirectoryObject;

        }

        #endregion

        #region CreateDirectoryObjectRecursive(myObjectLocation, myBlocksize, myRecursive)

        public Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation, UInt64 myBlocksize, Boolean myRecursive)
        {

            var _DirectoryObject = _IGraphFS.CreateDirectoryObject(myObjectLocation, myBlocksize, myRecursive, _SessionToken);            
            //_DirectoryObject.IGraphFSSessionReference = this;

            return _DirectoryObject;

        }

        #endregion

        #region isIDirectoryObject(params myObjectLocation)

        public Exceptional<Trinary> isIDirectoryObject(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.isIDirectoryObject(myObjectLocation, _SessionToken);
        }

        #endregion

        #region GetDirectoryListing(myObjectLocation, ...)

        public Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetDirectoryListing(myObjectLocation, _SessionToken);
        }

        public Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation, Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            return _IGraphFS.GetDirectoryListing(myObjectLocation, myFunc, _SessionToken);
        }

        public Exceptional<IEnumerable<String>> GetFilteredDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime)
        {
            return _IGraphFS.GetFilteredDirectoryListing(myObjectLocation, myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams, mySize, myCreationTime, myLastModificationTime, myLastAccessTime, myDeletionTime, _SessionToken);
        }

        #endregion

        #region GetExtendedDirectoryListing(params myObjectLocation)

        public Exceptional<IEnumerable<DirectoryEntryInformation>> GetExtendedDirectoryListing(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetExtendedDirectoryListing(myObjectLocation, _SessionToken);
        }

        #endregion

        #region GetFilteredExtendedDirectoryListing(myObjectLocation, myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams, mySize, myCreationTime, myLastModificationTime, myLastAccessTime, myDeletionTime)

        public Exceptional<IEnumerable<DirectoryEntryInformation>> GetFilteredExtendedDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime)
        {
            return _IGraphFS.GetFilteredExtendedDirectoryListing(myObjectLocation, myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams, mySize, myCreationTime, myLastModificationTime, myLastAccessTime, myDeletionTime, _SessionToken);
        }

        #endregion

        #region RemoveDirectoryObject(myObjectLocation, MyRemoveRecursive)

        public Exceptional RemoveDirectoryObject(ObjectLocation myObjectLocation, Boolean MyRemoveRecursive)
        {
            return _IGraphFS.RemoveDirectoryObject(myObjectLocation, MyRemoveRecursive, _SessionToken);
        }

        #endregion

        #region EraseDirectoryObject(myObjectLocation, MyEraseRecursive)

        public Exceptional EraseDirectoryObject(ObjectLocation myObjectLocation, Boolean MyEraseRecursive)
        {
            return _IGraphFS.EraseDirectoryObject(myObjectLocation, MyEraseRecursive, _SessionToken);
        }

        #endregion

        #endregion

        #region Metadata Maintenance

        #region SetMetadatum(myObjectLocation, myObjectStream, myObjectEdition, myKey, myValue, myIndexSetStrategy)

        public Exceptional SetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {
            return _IGraphFS.SetMetadatum<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, myValue, myIndexSetStrategy, _SessionToken);
        }

        #endregion

        #region SetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myMetadata, myIndexSetStrategy)

        public Exceptional SetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata, IndexSetStrategy myIndexSetStrategy)
        {
            return _IGraphFS.SetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myMetadata, myIndexSetStrategy, _SessionToken);
        }

        #endregion


        #region MetadatumExists<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, myValue)

        public Exceptional<Trinary> MetadatumExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue)
        {
            return _IGraphFS.MetadatumExists<TValue>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myKey, myValue, _SessionToken);
        }

        #endregion

        #region MetadataExists<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myUserMetadataKey)

        public Exceptional<Trinary> MetadataExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey)
        {
            return _IGraphFS.MetadataExists<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, _SessionToken);
        }

        #endregion


        #region GetMetadatum<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey)

        public Exceptional<IEnumerable<TValue>> GetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey)
        {
            return _IGraphFS.GetMetadatum<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, _SessionToken);
        }

        #endregion

        #region GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition)

        public Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
        {
            return _IGraphFS.GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, _SessionToken);
        }

        #endregion

        #region GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myMinKey, myMaxKey)

        public Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myMinKey, String myMaxKey)
        {
            return _IGraphFS.GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myMinKey, myMaxKey, _SessionToken);
        }

        #endregion

        #region GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myFunc)

        public Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc)
        {
            return _IGraphFS.GetMetadata<TValue>(myObjectLocation, myObjectStream, myObjectStream, myFunc, _SessionToken); ;
        }

        #endregion


        #region RemoveMetadatum<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, myValue)

        public Exceptional RemoveMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue)
        {
            return _IGraphFS.RemoveMetadatum<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, myValue, _SessionToken);
        }

        #endregion

        #region RemoveMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey)

        public Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey)
        {
            return _IGraphFS.RemoveMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myKey, _SessionToken);
        }

        #endregion

        #region RemoveMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myMetadata)

        public Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata)
        {
            return _IGraphFS.RemoveMetadata(myObjectLocation, myObjectStream, myObjectEdition, myMetadata, _SessionToken);
        }

        #endregion

        #region RemoveMetadata<TValue>(myObjectLocation, myObjectStream, myObjectEdition, myFunc)

        public Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc)
        {
            return _IGraphFS.RemoveMetadata(myObjectLocation, myObjectStream, myObjectEdition, myFunc, _SessionToken);
        }

        #endregion

        #endregion

        #region UserMetadatum Maintenance

        #region SetMetadatum(myObjectLocation, myKey, myObject, myIndexSetStrategy)

        public Exceptional SetUserMetadatum(ObjectLocation myObjectLocation, String myKey, Object myObject, IndexSetStrategy myIndexSetStrategy)
        {
            return _IGraphFS.SetMetadatum<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myKey, myObject, IndexSetStrategy.MERGE, _SessionToken);
        }

        #endregion

        #region SetUserMetadata(myObjectLocation, myUserMetadata, myIndexSetStrategy)

        public Exceptional SetUserMetadata(ObjectLocation myObjectLocation, IEnumerable<KeyValuePair<String, Object>> myUserMetadata, IndexSetStrategy myIndexSetStrategy)
        {
            return _IGraphFS.SetMetadata<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myUserMetadata, IndexSetStrategy.MERGE, _SessionToken);
        }

        #endregion


        #region UserMetadatumExists(myObjectLocation, myKey, myObject)

        public Exceptional<Trinary> UserMetadatumExists(ObjectLocation myObjectLocation, String myKey, Object myObject)
        {
            return _IGraphFS.MetadatumExists<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myKey, myObject, _SessionToken);
        }

        #endregion

        #region UserMetadataExists(myObjectLocation, myKey)

        public Exceptional<Trinary> UserMetadataExists(ObjectLocation myObjectLocation, String myKey)
        {
            return _IGraphFS.MetadataExists<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myKey, _SessionToken);
        }

        #endregion


        #region GetUserMetadatum(myObjectLocation, myKey)

        public Exceptional<IEnumerable<Object>> GetUserMetadatum(ObjectLocation myObjectLocation, String myKey)
        {

            var _GetMetadata = _IGraphFS.GetMetadatum<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myKey, _SessionToken);

            if (_GetMetadata != null && _GetMetadata.Success && _GetMetadata.Value != null)
                return new Exceptional<IEnumerable<Object>>() { Value = _GetMetadata.Value };

            else
                return new Exceptional<IEnumerable<Object>>(_GetMetadata);

        }

        #endregion

        #region GetUserMetadata(myObjectLocation)

        public Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation)
        {

            var _GetMetadata = _IGraphFS.GetMetadata<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, _SessionToken);

            if (_GetMetadata != null && _GetMetadata.Success && _GetMetadata.Value != null)
                return new Exceptional<IEnumerable<KeyValuePair<String, Object>>>() { Value = _GetMetadata.Value };

            else
                return new Exceptional<IEnumerable<KeyValuePair<String, Object>>>(_GetMetadata);

        }

        #endregion

        #region GetUserMetadata(myObjectLocation, myMinKey, myMaxKey)

        public Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation, String myMinKey, String myMaxKey)
        {

            var _GetMetadata = _IGraphFS.GetMetadata<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myMinKey, myMaxKey, _SessionToken);

            if (_GetMetadata != null && _GetMetadata.Success && _GetMetadata.Value != null)
                return new Exceptional<IEnumerable<KeyValuePair<String, Object>>>() { Value = _GetMetadata.Value };

            else
                return new Exceptional<IEnumerable<KeyValuePair<String, Object>>>(_GetMetadata);

        }

        #endregion

        #region GetUserMetadata(myObjectLocation, myFunc)

        public Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation, Func<KeyValuePair<String, Object>, Boolean> myFunc)
        {

            var _GetMetadata = _IGraphFS.GetMetadata<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myFunc, _SessionToken); ;

            if (_GetMetadata != null && _GetMetadata.Success && _GetMetadata.Value != null)
                return new Exceptional<IEnumerable<KeyValuePair<String, Object>>>() { Value = _GetMetadata.Value };

            else
                return new Exceptional<IEnumerable<KeyValuePair<String, Object>>>(_GetMetadata);

        }

        #endregion


        #region RemoveUserMetadatum(myObjectLocation, myKey, myObject)

        public Exceptional RemoveUserMetadatum(ObjectLocation myObjectLocation, String myKey, Object myObject)
        {
            return _IGraphFS.RemoveMetadatum<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myKey, myObject, _SessionToken);
        }

        #endregion

        #region RemoveUserMetadata(myObjectLocation, myKey)

        public Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, String myKey)
        {
            return _IGraphFS.RemoveMetadata<Object>(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myKey, _SessionToken);
        }

        #endregion

        #region RemoveUserMetadata(myObjectLocation, myMetadata)

        public Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, IEnumerable<KeyValuePair<String, Object>> myMetadata)
        {
            return _IGraphFS.RemoveMetadata(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myMetadata, _SessionToken);
        }

        #endregion

        #region RemoveUserMetadata(myObjectLocation, myFunc)

        public Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, Func<KeyValuePair<String, Object>, Boolean> myFunc)
        {
            return _IGraphFS.RemoveMetadata(myObjectLocation, FSConstants.USERMETADATASTREAM, FSConstants.DefaultEdition, myFunc, _SessionToken);
        }

        #endregion

        #endregion


        #region FileObject

        #region GetFileObject(myObjectLocation)

        public Exceptional<FileObject> GetFileObject(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.GetObject<FileObject>(myObjectLocation, FSConstants.FILESTREAM, FSConstants.DefaultEdition, null, 0, false, _SessionToken);
        }

        #endregion

        #region GetFileObject(myObjectLocation, myRevisionID)

        public Exceptional<FileObject> GetFileObject(ObjectLocation myObjectLocation, RevisionID myRevisionID)
        {
            return _IGraphFS.GetObject<FileObject>(myObjectLocation, FSConstants.FILESTREAM, FSConstants.DefaultEdition, myRevisionID, 0, false, _SessionToken);
        }

        #endregion

        #region StoreFileObject(myObjectLocation, myData, myAllowOverwritte)

        public Exceptional StoreFileObject(ObjectLocation myObjectLocation, Byte[] myData, Boolean myAllowOverwritte)
        {
            return _IGraphFS.StoreFSObject(myObjectLocation, new FileObject() { ObjectLocation = myObjectLocation, ObjectData = myData }, myAllowOverwritte, _SessionToken);
        }

        #endregion

        #region StoreFileObject(myObjectLocation, myStringData, myAllowOverwritte)

        public Exceptional StoreFileObject(ObjectLocation myObjectLocation, String myStringData, Boolean myAllowOverwritte)
        {
            return _IGraphFS.StoreFSObject(myObjectLocation, new FileObject() { ObjectLocation = myObjectLocation, ObjectData = UTF8Encoding.UTF8.GetBytes(myStringData) }, myAllowOverwritte, _SessionToken);
        }

        #endregion

        #endregion

        //#region Rights

        //public Boolean AddRightsStreamToObject(ObjectLocation myObjectLocation, AccessControlObject myRightsObject)
        //{
        //    return _IGraphFS.AddRightsStreamToObject(myObjectLocation, myRightsObject, _SessionToken);
        //}

        //public Boolean AddEntityToRightsStreamAllowACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID)
        //{
        //    return _IGraphFS.AddEntityToRightsStreamAllowACL(myObjectLocation, myRightUUID, myEntitiyUUID, _SessionToken);
        //}

        //public Boolean AddEntityToRightsStreamDenyACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID)
        //{
        //    return _IGraphFS.AddEntityToRightsStreamDenyACL(myObjectLocation, myRightUUID, myEntitiyUUID, _SessionToken);
        //}

        //public Boolean RemoveEntityFromRightsStreamAllowACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID)
        //{
        //    return _IGraphFS.RemoveEntityFromRightsStreamAllowACL(myObjectLocation, myRightUUID, myEntitiyUUID, _SessionToken);
        //}

        //public Boolean RemoveEntityFromRightsStreamDenyACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID)
        //{
        //    return _IGraphFS.RemoveEntityFromRightsStreamDenyACL(myObjectLocation, myRightUUID, myEntitiyUUID, _SessionToken);
        //}

        //public Boolean ChangeAllowOverDenyOfRightsStream(ObjectLocation myObjectLocation, DefaultRuleTypes myDefaultRule)
        //{
        //    return _IGraphFS.ChangeAllowOverDenyOfRightsStream(myObjectLocation, myDefaultRule, _SessionToken);
        //}

        //public Boolean AddAlertToPandoraRightsAlertHandlingList(ObjectLocation myObjectLocation, NHAccessControlObject myAlert)
        //{
        //    return _IGraphFS.AddAlertToPandoraRightsAlertHandlingList(myObjectLocation, myAlert, _SessionToken);
        //}

        //public Boolean RemoveAlertFromPandoraRightsAlertHandlingList(ObjectLocation myObjectLocation, NHAccessControlObject myAlert)
        //{
        //    return _IGraphFS.RemoveAlertFromPandoraRightsAlertHandlingList(myObjectLocation, myAlert, _SessionToken);
        //}

        //public List<Right> EvaluateRightsForEntity(ObjectLocation myObjectLocation, EntityUUID myEntityGuid, AccessControlObject myRightsObject)
        //{
        //    return _IGraphFS.EvaluateRightsForEntity(myObjectLocation, myEntityGuid, myRightsObject, _SessionToken);
        //}

        //#endregion
       
        //#region Enitity

        //public EntityUUID AddEntity(ObjectLocation myObjectLocation, String myLogin, String myRealname, String myDescription, Dictionary<ContactTypes, List<String>> myContacts, String myPassword, List<PublicKey> myPublicKeyList, HashSet<EntityUUID> myMembership)
        //{
        //    return _IGraphFS.AddEntity(myObjectLocation, myLogin, myRealname, myDescription, myContacts, myPassword, myPublicKeyList, myMembership, _SessionToken);
        //}

        //public Trinary EntityExists(ObjectLocation myObjectLocation, EntityUUID myEntityUUID)
        //{
        //    return _IGraphFS.EntityExists(myObjectLocation, myEntityUUID, _SessionToken);
        //}

        //public EntityUUID GetEntityUUID(ObjectLocation myObjectLocation, String aName)
        //{
        //    return _IGraphFS.GetEntityUUID(myObjectLocation, aName, _SessionToken);
        //}

        //public Boolean RemoveEntity(ObjectLocation myObjectLocation, EntityUUID myEntityUUID)
        //{
        //    return _IGraphFS.RemoveEntity(myObjectLocation, myEntityUUID, _SessionToken);
        //}

        //public HashSet<EntityUUID> GetMemberships(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, Boolean myRecursion)
        //{
        //    return _IGraphFS.GetMemberships(myObjectLocation, myEntityUUID, myRecursion, _SessionToken);
        //}

        //public void ChangeEntityPassword(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, String myOldPassword, String myNewPassword)
        //{
        //    _IGraphFS.ChangeEntityPassword(myObjectLocation, myEntityUUID, myOldPassword, myNewPassword, _SessionToken);
        //}

        //public void ChangeEntityPublicKeyList(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, String myPassword, List<PublicKey> myNewPublicKeyList)
        //{
        //    _IGraphFS.ChangeEntityPublicKeyList(myObjectLocation, myEntityUUID, myPassword, myNewPublicKeyList, _SessionToken);
        //}

        //public void ChangeEntityAddMembership(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, EntityUUID myNewMembershipUUID)
        //{
        //     _IGraphFS.ChangeEntityAddMembership(myObjectLocation, myEntityUUID, myNewMembershipUUID, _SessionToken);
        //}

        //public void ChangeEntityRemoveMembership(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, EntityUUID myNewMembershipUUID)
        //{
        //    _IGraphFS.ChangeEntityRemoveMembership(myObjectLocation, myEntityUUID, myNewMembershipUUID, _SessionToken);
        //}

        //public List<PublicKey> GetEntityPublicKeyList(ObjectLocation myObjectLocation, EntityUUID myEntityUUID)
        //{
        //    return _IGraphFS.GetEntityPublicKeyList(myObjectLocation, myEntityUUID, _SessionToken);
        //}

        //public Boolean AddPandoraRight(ObjectLocation myObjectLocation, String Name, String ValidationScript)
        //{
        //    return _IGraphFS.AddPandoraRight(myObjectLocation, Name, ValidationScript, _SessionToken);
        //}

        //public Boolean RemovePandoraRight(ObjectLocation myObjectLocation, RightUUID myRightUUID)
        //{
        //    return _IGraphFS.RemovePandoraRight(myObjectLocation, myRightUUID, _SessionToken);
        //}

        //public Right GetRightByName(String RightName)
        //{
        //    return _IGraphFS.GetRightByName(RightName, _SessionToken);
        //}

        //public Boolean ContainsRightUUID(RightUUID myRightUUID)
        //{
        //    return _IGraphFS.ContainsRightUUID(myRightUUID, _SessionToken);
        //}

        //#endregion

        #region FlushObjectLocation(myObjectLocation)

        public void FlushObjectLocation(ObjectLocation myObjectLocation)
        {
            _IGraphFS.FlushObjectLocationNew(myObjectLocation, _SessionToken);
        }

        #endregion


        #region Stream

        public Exceptional<IGraphFSStream> OpenStream(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevision, UInt64 myObjectCopy)
        {
            return _IGraphFS.OpenStream(_SessionToken, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy);
        }

        public Exceptional<IGraphFSStream> OpenStream(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevision, UInt64 myObjectCopy,
                                           FileMode myFileMode, FileAccess myFileAccess, FileShare myFileShare, FileOptions myFileOptions, UInt64 myBufferSize)
        {
            return _IGraphFS.OpenStream(_SessionToken, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy,
                                          myFileMode, myFileAccess, myFileShare, myFileOptions, myBufferSize);
        }

        #endregion

        #region Transactions

        public FSTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myTimeStamp = null)
        {

            //FSTransaction _FSTransaction = null;

            //if (CurrentFSTransaction.Transaction == null)
            //{
            //    _FSTransaction = CurrentFSTransaction.Transaction = new FSTransaction(myDistributed, myLongRunning, myIsolationLevel, myName, myTimeStamp);
            //    //_FSTransaction.OnDispose += new TransactionDisposedHandler(TransactionOnDisposeHandler);
            //}

            //else
            //{
                //_FSTransaction = new FSTransaction(new GraphFSError_TransactionAlreadyRunning());
            //}

                return new FSTransaction(myDistributed, myLongRunning, myIsolationLevel, myName, myTimeStamp); ;

            //return _IGraphFS.BeginTransaction(_SessionToken, myDistributed, myLongRunning, myIsolationLevel, myName, myTimeStamp);

        }

        #endregion


        #region StorageEngine Maintenance

        public IEnumerable<StorageUUID> StorageUUIDs()
        {
            return _IGraphFS.StorageUUIDs(_SessionToken);
        }

        public IEnumerable<StorageUUID> StorageUUIDs(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.StorageUUIDs(myObjectLocation, _SessionToken);
        }


        public IEnumerable<String> StorageDescriptions()
        {
            return _IGraphFS.StorageDescriptions(_SessionToken);
        }

        public IEnumerable<String> StorageDescriptions(ObjectLocation myObjectLocation)
        {
            return _IGraphFS.StorageDescriptions(myObjectLocation, _SessionToken);
        }

        #endregion


        #endregion

    }

}
