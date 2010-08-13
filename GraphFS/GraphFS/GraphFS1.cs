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
 * GraphFS1
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphFS;
using sones.GraphFS.Caches;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Exceptions;
using sones.GraphFS.Errors;
using sones.GraphFS.InternalObjects;
using sones.GraphFS.Session;

using sones.StorageEngines;

using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones
{

    /// <summary>
    /// GraphFS1 is an memory-only implementation of the IGraphFS interface
    /// </summary>
    public sealed class GraphFS1 : AGraphFS, IGraphFS
    {


        //#region Data

        //private ObjectStore _ObjectCache;

        //#endregion

        #region Constructor

        #region GraphFS1()

        public GraphFS1()
            : base(new ObjectStore())
        {
          //  _ObjectCache       = new ObjectStore();
        }

        #endregion

        #endregion


        #region Information Methods

        #region IsPersistent

        public override Boolean IsPersistent { get { return false; } }

        #endregion

        #region isMounted

        /// <summary>
        /// Returns true if the file system was mounted correctly
        /// </summary>
        /// <returns>true if the file system was mounted correctly</returns>
        public override Boolean IsMounted
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region GetNumberOfBytes(SessionToken mySessionToken)

        public override UInt64 GetNumberOfBytes(SessionToken mySessionToken)
        {
            return 0UL;
        }

        #endregion

        #region GetNumberOfFreeBytes(SessionToken mySessionToken)

        public override UInt64 GetNumberOfFreeBytes(SessionToken mySessionToken)
        {
            return 0UL;
        }

        #endregion

        #endregion



        #region Make-/Grow-/ShrinkFileSystem

        #region MakeFileSystem(myStorageLocations, myDescription, myNumberOfBytes, myOverwriteExistingFileSystem, myAction, mySessionToken)

        /// <summary>
        /// This initialises a IGraphFS in a given device or file using the given sizes
        /// </summary>
        /// <param name="myStorageLocations">a device or filename where to store the file system data</param>
        /// <param name="myDescription">a distinguishable Name or description for the file system (can be changed later)</param>
        /// <param name="myNumberOfBytes">the size of the file system in byte</param>
        /// <param name="myOverwriteExistingFileSystem">overwrite an existing file system [yes|no]</param>
        /// <returns>the UUID of the new file system</returns>
        public override Exceptional<FileSystemUUID> MakeFileSystem(IEnumerable<String> myStorageLocations, String myDescription, UInt64 myNumberOfBytes, Boolean myOverwriteExistingFileSystem, Action<Double> myAction, SessionToken mySessionToken)
        {

            //#region Checks

            //if (IsMounted)
            //    throw new GraphFSException("You can not call MakeFileSystem(...) on a mounted file system!");

            //#endregion

            //lock (this)
            //{

            //    #region Generate new FileSystemUUID, if it was not given before

            //    if (FileSystemUUID.Length == 0)
            //        FileSystemUUID = new FileSystemUUID();

            //    #endregion

            //    #region Instantiate the NotificationDispatcher

            //    _NotificationDispatcher = new NotificationDispatcher(FileSystemUUID, _NotificationSettings);

            //    #endregion

            //}

            //return FileSystemUUID;

            return new Exceptional<FileSystemUUID>() { Value = FileSystemUUID };

        }

        #endregion

        #region GrowFileSystem(myNumberOfBytesToAdd, SessionToken mySessionToken)

        /// <summary>
        /// This enlarges the size of a IGraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        public override Exceptional<UInt64> GrowFileSystem(UInt64 myNumberOfBytesToAdd, SessionToken mySessionToken)
        {
            return new Exceptional<UInt64>();
        }

        #endregion

        #region ShrinkFileSystem(myNumberOfBytesToRemove, SessionToken mySessionToken)

        /// <summary>
        /// This reduces the size of a IGraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToRemove">the number of bytes to remove from the size of the current file system</param>
        public override Exceptional<UInt64> ShrinkFileSystem(UInt64 myNumberOfBytesToRemove, SessionToken mySessionToken)
        {
            return new Exceptional<UInt64>();
        }

        #endregion

        #endregion

        #region MountFileSystem

        #region MountFileSystem(myStorageLocation, myFSAccessMode, myNotificationDispatcher, SessionToken mySessionToken)

        public override Exceptional MountFileSystem(String myStorageLocation, AccessModeTypes myFSAccessMode, SessionToken mySessionToken)
        {

            Debug.Assert(_ObjectCache != null);

            var _RootDirectoryLocator = new ObjectLocator()
            {
                ObjectLocation = new ObjectLocation()
            };

            var _RootDirectoryObject = new DirectoryObject()
            {
                ObjectLocation = new ObjectLocation()
            };

            // Add special directories to the RootDirectory
            _RootDirectoryObject.AddObjectStream(FSConstants.DotLink,       FSConstants.VIRTUALDIRECTORY);
            _RootDirectoryObject.AddObjectStream(FSConstants.DotDotLink,    FSConstants.VIRTUALDIRECTORY);
            _RootDirectoryObject.AddObjectStream(FSConstants.DotMetadata,   FSConstants.VIRTUALDIRECTORY);
            _RootDirectoryObject.AddObjectStream(FSConstants.DotSystem,     FSConstants.VIRTUALDIRECTORY);
            _RootDirectoryObject.AddObjectStream(FSConstants.DotStreams,    FSConstants.VIRTUALDIRECTORY);
            _RootDirectoryObject.StoreInlineData(FSConstants.DotUUID,       _RootDirectoryObject.ObjectUUID.GetByteArray(), true);
            _RootDirectoryObject.AddObjectStream(FSConstants.DotFS,         FSConstants.VIRTUALDIRECTORY);
            _RootDirectoryObject.AddObjectStream(FSConstants.DotForest,     FSConstants.VIRTUALDIRECTORY);

            // Prepare the ObjectLocator
            _RootDirectoryLocator.Add(FSConstants.DIRECTORYSTREAM, null);
            _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM] = new ObjectStream();
            _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM].Add(FSConstants.DefaultEdition, null);
            _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition] = new ObjectEdition();
            _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition].Add(new ObjectRevisionID(FileSystemUUID), null);
            _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition][_RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition].LatestRevisionID] = new ObjectRevision(FSConstants.DIRECTORYSTREAM);
            
            var _CacheUUID = _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition].LatestRevision.CacheUUID;

            // Store both within the Lookuptable
            _ObjectCache.StoreObjectLocator(_RootDirectoryLocator);
            _ObjectCache.StoreAFSObject(_CacheUUID, _RootDirectoryObject);

            return new Exceptional();

        }

        #endregion

        

        #region UnmountFileSystem(SessionToken mySessionToken)

        public override Exceptional UnmountFileSystem(SessionToken mySessionToken)
        {

            lock (this)
            {

                var _Exceptional = base.UnmountFileSystem(mySessionToken);

                _ObjectCache.Clear();

                return _Exceptional;

            }

        }

        #endregion

        #endregion





        #region INode and ObjectLocator methods

        #region (protected) GetObjectLocator_protected(myObjectLocation)

        protected override Exceptional<ObjectLocator> GetObjectLocator_protected(ObjectLocation myObjectLocation)
        {

            var _Exceptional = _ObjectCache.GetObjectLocator(myObjectLocation);

            if (_Exceptional.Value == null)
                _Exceptional.PushT(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));

            return _Exceptional;
            
        }

        #endregion

        #endregion

        

        #region Object specific methods

        #region (protected) LoadAFSObject_protected(myObjectLocator, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myAFSObject)

        protected override Exceptional<AFSObject> LoadAFSObject_protected(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, AFSObject myAFSObject)
        {
            return new Exceptional<AFSObject>();
        }

        #endregion

        #region (protected) StoreAFSObject_Layer2_protected(myObjectLocation, myAFSObject, myAllowOverwritting, mySessionToken)

        protected override Exceptional StoreAFSObject_Layer2_protected(ObjectLocation myObjectLocation, AFSObject myAFSObject, Boolean myAllowOverwritting)
        {

            Debug.Assert(IsMounted);
            Debug.Assert(myAFSObject.INodeReference                         != null);
            Debug.Assert(myAFSObject.ObjectLocatorReference                 != null);
            Debug.Assert(myAFSObject.ObjectLocatorReference.ObjectLocation  != null);
            Debug.Assert(myAFSObject.ObjectStream                           != null);
            Debug.Assert(myAFSObject.ObjectEdition                          != null);
            Debug.Assert(myAFSObject.ObjectRevisionID                       != null);

            lock (this)
            {

                var _Exceptional    = new Exceptional();
                
                #region Write on TmpDisc!

                var _ObjectEdition1 = myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition];
                _ObjectCache.StoreObjectLocator(myAFSObject.ObjectLocatorReference);
                _ObjectCache.StoreAFSObject(_ObjectEdition1[myAFSObject.ObjectRevisionID].CacheUUID, myAFSObject);

                #endregion

                // Do a fake allocation to indicate that this "are" objects on a disc
                myAFSObject.ObjectLocatorReference.INodeReference.INodePositions.Add(new ExtendedPosition(0, 0));

                #region Remove obsolete ObjectRevisions

                //while (myAGraphObject.ObjectLocatorReference[myAGraphObject.ObjectStream][myAGraphObject.ObjectEdition].MaxNumberOfRevisions <
                //       myAGraphObject.ObjectLocatorReference[myAGraphObject.ObjectStream][myAGraphObject.ObjectEdition].GetMaxPathLength(
                //       myAGraphObject.ObjectLocatorReference[myAGraphObject.ObjectStream][myAGraphObject.ObjectEdition].LatestRevisionID))
                //{
                //    DeleteOldestObjectRevision(myAGraphObject.ObjectLocatorReference, myAGraphObject.ObjectStream, myAGraphObject.ObjectEdition, mySessionToken);
                //}

                while (_ObjectEdition1.ULongCount() > _ObjectEdition1.MaxNumberOfRevisions)
                {

                    // If the oldest and the second oldest object within the cache are different remove the oldest!
                    if (_ObjectEdition1.SecondOldestRevision != null)
                        if (_ObjectEdition1.SecondOldestRevision.CacheUUID != _ObjectEdition1.OldestRevision.CacheUUID)
                            CacheRemove(_ObjectEdition1.OldestRevision.CacheUUID);

                    _ObjectEdition1.Remove(_ObjectEdition1.OldestRevisionID);

                }

                #endregion

                #region Handle ParentIDirectoryObject

                return GetFSObject_protected<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), null, null, null, 0, false).
                    WhenFailed<DirectoryObject>(e => e.PushT(new GraphFSError_DirectoryObjectNotFound(myObjectLocation.Path))).
                    WhenSucceded<DirectoryObject>(_ParentDirectoryObject =>
                    {

                        #region Add ObjectStream to ParentDirectoryObject

                        if (!_ParentDirectoryObject.Value.ObjectStreamExists(myObjectLocation.Name, myAFSObject.ObjectStream))
                        {

                            #region Add a new ObjectRevision

                            var _ObjectEdition          = _ParentDirectoryObject.Value.ObjectLocatorReference[_ParentDirectoryObject.Value.ObjectStream][_ParentDirectoryObject.Value.ObjectEdition];
                            var _OldRevision            = _ObjectEdition.LatestRevision;
                            var _OldMinNumberOfCopies   = _OldRevision.MinNumberOfCopies;
                            var _OldMaxNumberOfCopies   = _OldRevision.MaxNumberOfCopies;

                            _ParentDirectoryObject.Value.ObjectRevisionID = new ObjectRevisionID(_ForestUUID);

                            var _NewRevision = new ObjectRevision(_ParentDirectoryObject.Value.ObjectStream)
                                                            {
                                                                MinNumberOfCopies = _OldMinNumberOfCopies,
                                                                MaxNumberOfCopies = _OldMaxNumberOfCopies,
                                                            };

                            _NewRevision.CacheUUID = _OldRevision.CacheUUID;

                            _ObjectEdition.Add(_ParentDirectoryObject.Value.ObjectRevisionID, _NewRevision);

                            #endregion

                            //_ParentIDirectoryObject.Value.IGraphFSReference = this;
                            if (myAFSObject.INodeReference.INodePositions.Count == 0)
                                Debug.Write("myAGraphObject.INodeReference.INodePositions.Count == 0");

                            _ParentDirectoryObject.Value.AddObjectStream(myObjectLocation.Name, myAFSObject.ObjectStream, myAFSObject.INodeReference.INodePositions);

                            #region Remove obsolete ObjectRevisions

                            while (_ObjectEdition.ULongCount() > _ObjectEdition.MaxNumberOfRevisions)
                            {

                                // If the oldest and the second oldest object within the cache are different remove the oldest!
                                if (_ObjectEdition.SecondOldestRevision != null)
                                    if (_ObjectEdition.SecondOldestRevision.CacheUUID != _ObjectEdition.OldestRevision.CacheUUID)
                                        CacheRemove(_ObjectEdition.OldestRevision.CacheUUID);

                                _ObjectEdition.Remove(_ObjectEdition.OldestRevisionID);

                            }

                            #endregion

                        }

                        #endregion

                        return _ParentDirectoryObject;

                    });

                #endregion

            }

        }

        #endregion

        #region (protected) RemoveAFSObject_protected(myObjectLocator, myObjectStream, myObjectEdition, myObjectRevisionID)

        protected override Exceptional RemoveAFSObject_protected(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID)
        {
            return Exceptional.OK;
        }

        #endregion

        #region (protected) EraseAFSObject_protected(myObjectLocator, myObjectStream, myObjectEdition, myObjectRevisionID)

        protected override Exceptional EraseAFSObject_protected(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID)
        {
            return Exceptional.OK;
        }

        #endregion

        #endregion


        #region (protected) CreateDirectoryObject_protected(myObjectLocation, myBlocksize)

        protected override Exceptional<IDirectoryObject> InitDirectoryObject_protected(ObjectLocation myObjectLocation, UInt64 myBlocksize)
        {

            Debug.Assert(IsMounted);
            Debug.Assert(myObjectLocation != null);

            var _Exceptional = new Exceptional<IDirectoryObject>();

            _Exceptional.Value = new DirectoryObject()
            {
                ObjectLocation = myObjectLocation,
                Blocksize = myBlocksize
            };

            return _Exceptional;
        
        }

        #endregion

        #region GraphStream methods

        #region OpenStream(mySessionToken, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy)

        public IGraphFSStream OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy)
        {
            return null;
        }

        #endregion

        #region OpenStream(mySessionToken, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy, myFileMode, myFileAccess, myFileShare, myFileOptions, myBufferSize)

        public IGraphFSStream OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy,
                                         FileMode myFileMode, FileAccess myFileAccess, FileShare myFileShare, FileOptions myFileOptions, UInt64 myBufferSize)
        {
            throw new NotImplementedException();
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


        //#region Transactions

        //public FSTransaction BeginTransaction(SessionToken mySessionToken)
        //{

        //    var newTransaction = new FSTransaction(mySessionToken);

        //    newTransaction.OnDispose += new TransactionDisposedHandler(transaction_OnDispose);

        //    lock (mySessionToken)
        //    {
        //        return (FSTransaction)mySessionToken.AddTransaction(newTransaction);
        //    }

        //}


        //private void transaction_OnDispose(object sender, TransactionDisposedEventArgs args)
        //{
        //    FSTransaction theDisposedTransaction = args.Transaction;
        //}


        //public void RollbackTransaction(SessionToken mySessionToken)
        //{
        //}

        //public void CommitTransaction(SessionToken mySessionToken)
        //{
        //}

        //#endregion


        #region IGraphFS Members

        public Exceptional<IMetadataObject<TValue>> CreateMetadataObject<TValue>(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public Exceptional RemoveMetadataObject(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public Exceptional<Trinary> MetadatumExists(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, string myUserMetadataKey)
        {
            throw new NotImplementedException();
        }

        public Exceptional RemoveMetadatum(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, string myUserMetadataKey)
        {
            throw new NotImplementedException();
        }

        public Exceptional RemoveMetadatum<TValue>(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, string myUserMetadataKey, TValue myObject)
        {
            throw new NotImplementedException();
        }

        public Exceptional SetMetadata<TValue>(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, IEnumerable<KeyValuePair<string, TValue>> myMetadata, IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GetMetadata<TValue>(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, string myUserMetadataKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GetMetadata<TValue>(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, Func<KeyValuePair<string, TValue>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, string myObjectStream, string myObjectEdition, IEnumerable<KeyValuePair<string, TValue>> myMetadata)
        {
            throw new NotImplementedException();
        }

        #endregion


    }

}

