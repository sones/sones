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
 * GraphFS1
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphFS;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Errors;
using sones.GraphFS.InternalObjects;
using sones.GraphFS.Session;

using sones.StorageEngines;

using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// GraphFS1 is an memory-only implementation of the IGraphFS interface
    /// </summary>
    public sealed class GraphFS1 : AGraphFS, IGraphFS
    {


        #region Constructor(s)

        #region GraphFS1()

        public GraphFS1()
            : base(new ObjectStore())
        {
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
                return !_ObjectCache.IsEmpty;
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


        #region Make-/Grow-/Shrink-/WipeFileSystem

        #region MakeFileSystem(mySessionToken, myDescription, myNumberOfBytes, myOverwriteExistingFileSystem, myAction)

        /// <summary>
        /// This initialises a IGraphFS in a given device or file using the given sizes
        /// </summary>
        /// <param name="myStorageLocations">a device or filename where to store the file system data</param>
        /// <param name="myDescription">a distinguishable Name or description for the file system (can be changed later)</param>
        /// <param name="myNumberOfBytes">the size of the file system in byte</param>
        /// <param name="myOverwriteExistingFileSystem">overwrite an existing file system [yes|no]</param>
        /// <returns>the UUID of the new file system</returns>
        public override Exceptional<FileSystemUUID> MakeFileSystem(SessionToken mySessionToken, String myDescription, UInt64 myNumberOfBytes, Boolean myOverwriteExistingFileSystem, Action<Double> myAction)
        {
            FileSystemUUID = FileSystemUUID.NewUUID;
            return new Exceptional<FileSystemUUID>(FileSystemUUID);
        }

        #endregion

        #region GrowFileSystem(mySessionToken, myNumberOfBytesToAdd)

        /// <summary>
        /// This enlarges the size of a IGraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        public override Exceptional<UInt64> GrowFileSystem(SessionToken mySessionToken, UInt64 myNumberOfBytesToAdd)
        {
            return new Exceptional<UInt64>();
        }

        #endregion

        #region ShrinkFileSystem(mySessionToken, myNumberOfBytesToRemove)

        /// <summary>
        /// This reduces the size of a IGraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToRemove">the number of bytes to remove from the size of the current file system</param>
        public override Exceptional<UInt64> ShrinkFileSystem(SessionToken mySessionToken, UInt64 myNumberOfBytesToRemove)
        {
            return new Exceptional<UInt64>();
        }

        #endregion

        #region WipeFileSystem(mySessionToken)

        /// <summary>
        /// This wipes this file system
        /// </summary>
        public override Exceptional WipeFileSystem(SessionToken mySessionToken)
        {
            return Exceptional.OK;
        }

        #endregion

        #endregion

        #region MountFileSystem(mySessionToken, myAccessMode)

        public override Exceptional MountFileSystem(SessionToken mySessionToken, AccessModeTypes myAccessMode)
        {

            Debug.Assert(_ObjectCache   != null);
            Debug.Assert(mySessionToken != null);

            var _RootDirectoryLocator = new ObjectLocator()
            {
                ObjectLocationSetter = new ObjectLocation()
            };

            var _RootDirectoryObject = new DirectoryObject()
            {
                ObjectLocatorReference  = _RootDirectoryLocator,
                ObjectLocation          = new ObjectLocation()
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
            _RootDirectoryObject.ObjectRevisionID = _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition].LatestRevisionID;
            _RootDirectoryLocator.INodeReferenceSetter = new INode(); // an Locator without an valid INode may break the AGraphFS.StoreAFSObject_protected() method
            _RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition][_RootDirectoryLocator[FSConstants.DIRECTORYSTREAM][FSConstants.DefaultEdition].LatestRevisionID]
                        = new ObjectRevision(_RootDirectoryObject.ObjectRevisionID, FSConstants.DIRECTORYSTREAM);

            // Store both within the ObjectCache
            _ObjectCache.StoreObjectLocator(_RootDirectoryLocator);
            _ObjectCache.StoreAFSObject(_RootDirectoryObject);

            AccessMode = myAccessMode;

            return new Exceptional();

        }

        #endregion


        #region AFSObject specific methods

        #region (protected) LoadAFSObject_protected(myObjectLocator, myObjectStream, myObjectEdition, myObjectRevisionID, myObjectCopy, myIgnoreIntegrityCheckFailures, myAFSObject)

        protected override Exceptional<AFSObject> LoadAFSObject_protected(ObjectLocator myObjectLocator, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, AFSObject myAFSObject)
        {
            return new Exceptional<AFSObject>();
        }

        #endregion

        #region (protected) StoreAFSObject_protected(myObjectLocation, myAFSObject, myAllowToOverwrite = false)

        protected override Exceptional StoreAFSObject_protected(ObjectLocation myObjectLocation, AFSObject myAFSObject, Boolean myAllowToOverwrite = false)
        {

            lock (this)
            {

                #region Initial checks

                Debug.Assert(IsMounted);
                Debug.Assert(myObjectLocation != null);
                Debug.Assert(myAFSObject      != null);

                #endregion

                // Call StoreAFSObject_protected on AGraphFS in order to check the
                // ObjectHierachy and cache the AFSObject.
                var _Exceptional =  base.StoreAFSObject_protected(myObjectLocation,
                                                                  myAFSObject,
                                                                  myAllowToOverwrite);

                #region Additional Checks

                if (_Exceptional.Failed())
                    return _Exceptional;

                Debug.Assert(myAFSObject.INodeReference                        != null);
                Debug.Assert(myAFSObject.ObjectLocatorReference                != null);
                Debug.Assert(myAFSObject.ObjectLocatorReference.ObjectLocation != null);
                Debug.Assert(myAFSObject.ObjectStream                          != null);
                Debug.Assert(myAFSObject.ObjectEdition                         != null);
                Debug.Assert(myAFSObject.ObjectRevisionID                      != null);

                #endregion


                // Do a fake allocation to indicate that this "are" objects on a disc
                myAFSObject.ObjectLocatorReference.INodeReference.INodePositions.Add(new ExtendedPosition(0, 0));

                #region Handle ParentIDirectoryObject

                return GetAFSObject_protected<DirectoryObject>(new ObjectLocation(myObjectLocation.Path), null, null, null, 0, false).
                    WhenFailed<DirectoryObject>(e => e.PushIErrorT(new GraphFSError_DirectoryObjectNotFound(myObjectLocation.Path))).
                    WhenSucceded<DirectoryObject>(_ParentDirectoryObjectExceptional =>
                    {

                        #region Add ObjectStream to ParentDirectoryObject

                        if (!_ParentDirectoryObjectExceptional.Value.ObjectStreamExists(myObjectLocation.Name, myAFSObject.ObjectStream))
                        {

                            #region Add a new ObjectRevision

                            var _ObjectEdition        = _ParentDirectoryObjectExceptional.Value.ObjectLocatorReference[_ParentDirectoryObjectExceptional.Value.ObjectStream][_ParentDirectoryObjectExceptional.Value.ObjectEdition];
                            var _OldRevision          = _ObjectEdition.LatestRevision;
                            var _OldMinNumberOfCopies = _OldRevision.MinNumberOfCopies;
                            var _OldMaxNumberOfCopies = _OldRevision.MaxNumberOfCopies;

                            _ParentDirectoryObjectExceptional.Value.ObjectRevisionID = new ObjectRevisionID(_ForestUUID);

                            var _NewRevision = new ObjectRevision(_ParentDirectoryObjectExceptional.Value.ObjectRevisionID, _ParentDirectoryObjectExceptional.Value.ObjectStream)
                            {
                                MinNumberOfCopies = _OldMinNumberOfCopies,
                                MaxNumberOfCopies = _OldMaxNumberOfCopies,
                            };

                            _NewRevision.CacheUUID = _OldRevision.CacheUUID;

                            _ObjectEdition.Add(_ParentDirectoryObjectExceptional.Value.ObjectRevisionID, _NewRevision);

                            #endregion

                            _ParentDirectoryObjectExceptional.Value.AddObjectStream(myObjectLocation.Name, myAFSObject.ObjectStream, myAFSObject.INodeReference.INodePositions);

                            #region Remove obsolete ObjectRevisions

                            while (_ObjectEdition.ULongCount() > _ObjectEdition.MaxNumberOfRevisions)
                            {

                                // If the oldest and the second oldest object within the cache are different remove the oldest!
                                if (_ObjectEdition.SecondOldestRevision != null)
                                    if (_ObjectEdition.SecondOldestRevision.CacheUUID != _ObjectEdition.OldestRevision.CacheUUID)
                                        _ObjectCache.RemoveAFSObject(_ObjectEdition.OldestRevision.CacheUUID);

                                _ObjectEdition.Remove(_ObjectEdition.OldestRevisionID);

                            }

                            #endregion

                        }

                        #endregion

                        return _ParentDirectoryObjectExceptional;

                    });

                #endregion

            }

        }

        #endregion

        #region (protected) InitDirectoryObject_protected(myObjectLocation, myBlocksize)

        protected override Exceptional<IDirectoryObject> InitDirectoryObject_protected(ObjectLocation myObjectLocation, UInt64 myBlocksize)
        {

            Debug.Assert(IsMounted);
            Debug.Assert(myObjectLocation != null);

            var _Exceptional = new Exceptional<IDirectoryObject>();

            _Exceptional.Value = new DirectoryObject()
            {
                ObjectLocation  = myObjectLocation,
                Blocksize       = myBlocksize
            };

            return _Exceptional;
        
        }

        #endregion

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

