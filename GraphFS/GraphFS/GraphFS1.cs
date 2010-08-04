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

using sones.StorageEngines;

using sones.TmpFS.Caches;
using sones.GraphFS.Caches;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Exceptions;
using sones.GraphFS.Errors;
using sones.GraphFS.InternalObjects;
using sones.GraphFS;
using sones.GraphFS.Session;

using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones
{

    /// <summary>
    /// GraphFS1 is an memory-only implementation of the IGraphFS interface
    /// </summary>
    public sealed class GraphFS1 : AGraphFS, IGraphFS
    {


        #region Data

        private TmpFSLookuptable _TmpFSLookuptable;

        #endregion

        #region Constructor

        #region TmpFS()

        public GraphFS1()
        {
            _TmpFSLookuptable       = new TmpFSLookuptable();
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


        #region (protected) ObjectCache handling

        #region GetObjectCacheSettings(mySessionToken)

        public override ObjectCacheSettings GetObjectCacheSettings(SessionToken mySessionToken)
        {
            return ObjectCacheSettings;
        }

        #endregion

        #region GetObjectCacheSettings(myObjectLocation, mySessionToken)

        public override ObjectCacheSettings GetObjectCacheSettings(ObjectLocation myObjectLocation, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region SetObjectCacheSettings(myObjectCacheSettings, mySessionToken)

        public override void SetObjectCacheSettings(ObjectCacheSettings myObjectCacheSettings, SessionToken mySessionToken)
        {
            ObjectCacheSettings = myObjectCacheSettings;
        }

        #endregion

        #region SetObjectCacheSettings(myObjectLocation, myObjectCacheSettings, mySessionToken)

        public override void SetObjectCacheSettings(ObjectLocation myObjectLocation, ObjectCacheSettings myObjectCacheSettings, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region (protected) CacheAdd(myObjectLocation, myObjectLocator, myIsPinned)

        protected override void CacheAdd(ObjectLocation myObjectLocation, ObjectLocator myObjectLocator, Boolean myIsPinned)
        {
            _TmpFSLookuptable.Set(myObjectLocation, myObjectLocator);
        }

        #endregion

        #region (protected) CacheAdd(myCacheUUID, myAPandoraObject, myIsPinned)

        protected override void CacheAdd(CacheUUID myCacheUUID, AFSObject myAPandoraObject, Boolean myIsPinned)
        {
            _TmpFSLookuptable.Set(myCacheUUID, myAPandoraObject);
        }

        #endregion

        #region (protected) CacheGet<PT>(myCacheUUID)

        protected override Exceptional<PT> CacheGet<PT>(CacheUUID myCacheUUID)
        {

            var _Exceptional = new Exceptional<PT>();
            _Exceptional.Value = (PT)_TmpFSLookuptable.GetAFSObject(myCacheUUID);

            if (_Exceptional == null || _Exceptional.Failed || _Exceptional.Value == null)
                _Exceptional.PushT(new GraphFSError("Could not get object with UUID '" + myCacheUUID.ToString() + "' from ObjectCache!"));

            return _Exceptional;

        }

        #endregion

        #region (protected) CacheMove(myOldObjectLocation, myNewObjectLocation, myRecursiveDecaching)

        protected override void CacheMove(ObjectLocation myOldObjectLocation, ObjectLocation myNewObjectLocation, Boolean myRecursiveDecaching)
        {
            lock (_TmpFSLookuptable)
            {
                _TmpFSLookuptable.Move(myOldObjectLocation, myNewObjectLocation, myRecursiveDecaching);
            }
        }

        #endregion

        #region (protected) CacheRemove(myObjectLocation, myRecursiveDecaching)

        protected override void CacheRemove(ObjectLocation myObjectLocation, Boolean myRecursiveDecaching)
        {
            lock (_TmpFSLookuptable)
            {
                _TmpFSLookuptable.Remove(myObjectLocation, myRecursiveDecaching);
            }
        }

        #endregion

        #region (protected) CacheRemove(myCacheUUID)

        protected override void CacheRemove(CacheUUID myCacheUUID)
        {
            _TmpFSLookuptable.Remove(myCacheUUID);
        }

        #endregion

        #endregion




        #region Make-/Grow-/ShrinkFileSystem

        #region MakeFileSystem(myIStorageEngines, myDescription, myOverwriteExistingFileSystem, myAction, SessionToken mySessionToken)

        /// <summary>
        /// This initialises a IPandoraFS in a given device or file using the given sizes
        /// </summary>
        /// <param name="myIStorageEngines">a device or filename where to store the file system data</param>
        /// <param name="myDescription">a distinguishable Name or description for the file system (can be changed later)</param>
        /// <param name="myNumberOfBytes">the size of the file system in byte</param>
        /// <param name="myOverwriteExistingFileSystem">overwrite an existing file system [yes|no]</param>
        /// <returns>the UUID of the new file system</returns>
        public override Exceptional<FileSystemUUID> MakeFileSystem(IEnumerable<IStorageEngine> myIStorageEngines, String myDescription, Boolean myOverwriteExistingFileSystem, Action<Double> myAction, SessionToken mySessionToken)
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
        /// This enlarges the size of a IPandoraFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        public override Exceptional<UInt64> GrowFileSystem(UInt64 myNumberOfBytesToAdd, SessionToken mySessionToken)
        {
            return new Exceptional<UInt64>();
        }

        #endregion

        #region ShrinkFileSystem(myNumberOfBytesToRemove, SessionToken mySessionToken)

        /// <summary>
        /// This reduces the size of a IPandoraFS
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

            if (_TmpFSLookuptable == null)
                _TmpFSLookuptable = new TmpFSLookuptable();

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
            _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition].Add(new RevisionID(FileSystemUUID), null);
            _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition][_RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition].LatestRevisionID] = new ObjectRevision(FSConstants.DIRECTORYSTREAM);
            
            var _CacheUUID = _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition].LatestRevision.CacheUUID;

            // Store both within the Lookuptable
            _TmpFSLookuptable.Set(_RootDirectoryLocator.ObjectLocation, _RootDirectoryLocator);
            _TmpFSLookuptable.Set(_CacheUUID, _RootDirectoryObject);

            return new Exceptional();

        }

        #endregion

        

        #region UnmountFileSystem(SessionToken mySessionToken)

        public override Exceptional UnmountFileSystem(SessionToken mySessionToken)
        {

            lock (this)
            {

                var _Exceptional = base.UnmountFileSystem(mySessionToken);

                _TmpFSLookuptable = null;

                return _Exceptional;

            }

        }

        #endregion

        #endregion





        #region INode and ObjectLocator methods

        #region (protected) GetObjectLocator_protected(myObjectLocation)

        protected override Exceptional<ObjectLocator> GetObjectLocator_protected(ObjectLocation myObjectLocation)
        {

            var _Exceptional = new Exceptional<ObjectLocator>();

            _Exceptional.Value = _TmpFSLookuptable.GetObjectLocator(myObjectLocation);

            if (_Exceptional.Value == null)
                _Exceptional.PushT(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));

            return _Exceptional;
            
        }

        #endregion

        #endregion

        

        #region Object specific methods

        //#region GetAPandoraObject<PT>(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, mySessionToken)

        //public Exceptional<PT> GetObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, SessionToken mySessionToken) where PT : APandoraObject, new()
        //{

        //    lock (this)
        //    {

        //        Exceptional<PT> _Exceptional = new Exceptional<PT>();

        //        try
        //        {

        //            #region Input validation

        //            if (myObjectLocation == null || myObjectLocation.Length == 0)
        //                throw new ArgumentNullException("The parameter myObjectLocator must not be null or its length zero!");

        //            if (myObjectStream == null)
        //            {
        //                var newT = new PT();
        //                myObjectStream = newT.ObjectStream;
        //            }

        //            if (myObjectEdition == null)
        //                myObjectEdition = FSConstants.DefaultEdition;

        //            #endregion

        //            #region Data

        //            INode           _INode          = null;
        //            ObjectStream    _ObjectStream   = null;
        //            ObjectEdition   _ObjectEdition  = null;
        //            ObjectRevision  _ObjectRevision = null;

        //            #endregion

        //            #region Resolve ObjectStream, -Edition and -RevisionID

        //            var _ObjectLocator = _TmpFSLookuptable.GetObjectLocator(myObjectLocation);

        //            if (_ObjectLocator != null)
        //            {

        //                if (_ObjectLocator.ContainsKey(myObjectStream))
        //                {

        //                    _ObjectStream = _ObjectLocator[myObjectStream];

        //                    if (_ObjectStream != null)
        //                    {

        //                        if (_ObjectStream.ContainsKey(myObjectEdition))
        //                        {

        //                            _ObjectEdition = _ObjectStream[myObjectEdition];

        //                            if (_ObjectEdition != null)
        //                            {

        //                                // If nothing specified => Return the LatestRevision
        //                                if (myObjectRevisionID == null || myObjectRevisionID.UUID == null)
        //                                {
        //                                    _ObjectRevision = _ObjectEdition.LatestRevision;
        //                                    myObjectRevisionID = _ObjectEdition.LatestRevisionID;
        //                                }

        //                                else
        //                                {
        //                                    _ObjectRevision = _ObjectEdition[myObjectRevisionID];
        //                                }

        //                            }
        //                            else
        //                                return new Exceptional<PT>(new GraphFSError_NoObjectRevisionsFound(myObjectLocation, myObjectStream, myObjectEdition));

        //                        }
        //                        else
        //                            return new Exceptional<PT>(new GraphFSError_ObjectEditionNotFound(myObjectLocation, myObjectEdition, myObjectStream));

        //                    }
        //                    else
        //                        return new Exceptional<PT>(new GraphFSError_NoObjectEditionsFound(myObjectLocation, myObjectStream));

        //                }
        //                else
        //                    return new Exceptional<PT>(new GraphFSError_ObjectStreamNotFound(myObjectLocation, myObjectStream));


        //                if (_ObjectRevision != null)
        //                {

        //                    // Get APandoraObject from Cache
        //                    _Exceptional.Value = (PT) _TmpFSLookuptable.GetAPandoraObject(_ObjectRevision.CacheUUID);

        //                }

        //            }

        //            #endregion
                    
        //            else
        //                _Exceptional.Add(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));

        //        }

        //        catch (Exception e)
        //        {
        //            _Exceptional.Add(new GraphFSError(e.Message));
        //            return _Exceptional.Value;
        //        }

        //        return _Exceptional.Value;

        //    }

        //}

        //#endregion

        protected override Exceptional<AFSObject> LoadAFSObject_protected(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, AFSObject myAFSObject)
        {
            return new Exceptional<AFSObject>();
        }


        #region StoreAFSObject_protected(myObjectLocation, myAFSObject, myAllowOverwritting, mySessionToken)

        protected override Exceptional StoreAFSObject_protected(ObjectLocation myObjectLocation, AFSObject myAFSObject, Boolean myAllowOverwritting, SessionToken mySessionToken)
        {

            lock (this)
            {

                var _Exceptional    = new Exceptional();
                var _ObjectEdition1 = myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition];

                #region Write on TmpDisc!

                _TmpFSLookuptable.Set(myObjectLocation, myAFSObject.ObjectLocatorReference);
                _TmpFSLookuptable.Set(_ObjectEdition1[myAFSObject.ObjectRevision].CacheUUID, myAFSObject);

                #endregion


                // Do a fake allocation to indicate that this "are" objects on a disc
                myAFSObject.ObjectLocatorReference.INodeReference.INodePositions.Add(new ExtendedPosition(0, 0));


                #region Remove obsolete ObjectRevisions

                //while (myAPandoraObject.ObjectLocatorReference[myAPandoraObject.ObjectStream][myAPandoraObject.ObjectEdition].MaxNumberOfRevisions <
                //       myAPandoraObject.ObjectLocatorReference[myAPandoraObject.ObjectStream][myAPandoraObject.ObjectEdition].GetMaxPathLength(
                //       myAPandoraObject.ObjectLocatorReference[myAPandoraObject.ObjectStream][myAPandoraObject.ObjectEdition].LatestRevisionID))
                //{
                //    DeleteOldestObjectRevision(myAPandoraObject.ObjectLocatorReference, myAPandoraObject.ObjectStream, myAPandoraObject.ObjectEdition, mySessionToken);
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

                return GetFSObject<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), null, null, null, 0, false, mySessionToken).
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

                            _ParentDirectoryObject.Value.ObjectRevision = new RevisionID(_ForestUUID);

                            var _NewRevision = new ObjectRevision(_ParentDirectoryObject.Value.ObjectStream)
                                                            {
                                                                MinNumberOfCopies = _OldMinNumberOfCopies,
                                                                MaxNumberOfCopies = _OldMaxNumberOfCopies,
                                                            };

                            _NewRevision.CacheUUID = _OldRevision.CacheUUID;

                            _ObjectEdition.Add(_ParentDirectoryObject.Value.ObjectRevision, _NewRevision);

                            #endregion

                            //_ParentIDirectoryObject.Value.IPandoraFSReference = this;
                            if (myAFSObject.INodeReference.INodePositions.Count == 0)
                                Debug.Write("myAPandoraObject.INodeReference.INodePositions.Count == 0");

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

        protected override Exceptional RemoveAFSObject_protected(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID)
        {
            return Exceptional.OK;
        }

        #region EraseObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID, mySessionToken)

        public Exceptional EraseFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


        #region (protected) CreateDirectoryObject_protected(myObjectLocation, myBlocksize)

        protected override IDirectoryObject CreateDirectoryObject_protected(ObjectLocation myObjectLocation, UInt64 myBlocksize)
        {
            return new DirectoryObject()
            {
                ObjectLocation = myObjectLocation,
                Blocksize      = myBlocksize
            };
        }

        #endregion

        #region PandoraStream methods

        #region OpenStream(mySessionToken, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy)

        public IGraphFSStream OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevision, UInt64 myObjectCopy)
        {
            return null;
        }

        #endregion

        #region OpenStream(mySessionToken, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy, myFileMode, myFileAccess, myFileShare, myFileOptions, myBufferSize)

        public IGraphFSStream OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevision, UInt64 myObjectCopy,
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

