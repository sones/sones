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
 * IGraphFS
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.IO;
using System.Collections.Generic;

using sones.StorageEngines;

using sones.GraphFS;
using sones.GraphFS.Events;
using sones.GraphFS.Caches;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphFS.InternalObjects;

using sones.Notifications;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.GraphFS.Transactions;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones
{

    /// <summary>
    /// The interface for all Pandora file systems. Each method that gets exported to the WCF Administration Userland
    /// should be marked with . Please keep in mind that these methods should only take primitive
    /// parameters and return data structures that can be serialized and deserialized with .NET Serializers.
    /// (so preferably they also return just primitives)
    /// </summary>

    public interface IGraphFS
    {


        #region Properties

        FileSystemUUID          FileSystemUUID          { get; }
        NotificationSettings    NotificationSettings    { get; set; }
        ObjectCacheSettings     ObjectCacheSettings     { get; set; }
        NotificationDispatcher  NotificationDispatcher  { get; set; }

        #endregion

        #region Events

        event FSEventHandlers.OnLoadEventHandler    OnLoad;
        event FSEventHandlers.OnLoadedEventHandler  OnLoaded;
        event FSEventHandlers.OnSaveEventHandler    OnSave;
        event FSEventHandlers.OnSavedEventHandler   OnSaved;
        event FSEventHandlers.OnRemoveEventHandler  OnRemove;
        event FSEventHandlers.OnRemovedEventHandler OnRemoved;

        #endregion

        #region Information Methods

        #region IsPersistent

        Boolean IsPersistent { get; }

        #endregion

        #region isMounted

        /// <summary>
        /// Returns true if the file system was mounted correctly
        /// </summary>
        /// <returns>true if the file system was mounted correctly</returns>
        Boolean             IsMounted { get; }

        #endregion


        #region TraverseChildFSs(myFunc, myDepth, mySessionToken)

        IEnumerable<Object> TraverseChildFSs(Func<IGraphFS, UInt64, IEnumerable<Object>> myFunc, UInt64 myDepth, SessionToken mySessionToken);

        #endregion


        #region GetFileSystemUUID(...)

        /// <summary>
        /// Returns the UUID of this file system
        /// </summary>
        /// <returns>The UUID of this file system</returns>
        FileSystemUUID              GetFileSystemUUID(SessionToken mySessionToken);

        /// <summary>
        /// Returns the UUID of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The UUID of the file system at the given ObjectLocation</returns>
        FileSystemUUID              GetFileSystemUUID(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        /// <summary>
        /// Returns a recursive list of FileSystemUUIDs of all mounted file systems
        /// </summary>
        /// <param name="myDepth">Depth</param>
        /// <returns>A (recursive) list of FileSystemUUIDs of all mounted file systems</returns>
        IEnumerable<FileSystemUUID> GetFileSystemUUIDs(UInt64 myDepth, SessionToken mySessionToken);

        #endregion

        #region GetFileSystemDescription(...)

        /// <summary>
        /// Returns the name or a description of this file system.
        /// </summary>
        /// <returns>The name or a description of this file system</returns>
        String              GetFileSystemDescription(SessionToken mySessionToken);

        /// <summary>
        /// Returns the name or a description of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The name or a description of the file system at the given ObjectLocation</returns>
        String              GetFileSystemDescription(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        /// <summary>
        /// Returns a (recursive) list of file system descriptions of all mounted file systems
        /// </summary>
        /// <param name="myRecursiveOperation">Recursive operation?</param>
        /// <returns>A (recursive) list of file system descriptions of all mounted file systems</returns>
        IEnumerable<String> GetFileSystemDescriptions(UInt64 myDepth, SessionToken mySessionToken);

        #endregion

        #region SetFileSystemDescription(...)

        /// <summary>
        /// Sets the Name or a description of this file system.
        /// </summary>
        /// <param name="myFileSystemDescription">the Name or a description of this file system</param>
        Boolean SetFileSystemDescription(String myFileSystemDescription, SessionToken mySessionToken);

        /// <summary>
        /// Sets the Name or a description of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myFileSystemDescription">the Name or a description of the file system at the given ObjectLocation</param>
        Boolean SetFileSystemDescription(ObjectLocation myObjectLocation, String myFileSystemDescription, SessionToken mySessionToken);

        #endregion

        #region GetNumberOfBytes(...)

        /// <summary>
        /// Returns the size (number of bytes) of this file system
        /// </summary>
        /// <returns>The size (number of bytes) of this file system</returns>
        UInt64              GetNumberOfBytes(SessionToken mySessionToken);

        /// <summary>
        /// Returns the size (number of bytes) of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The size (number of bytes) of the file system at the given ObjectLocation</returns>
        UInt64              GetNumberOfBytes(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        /// <summary>
        /// Returns a (recursive) list of the number of bytes within all mounted file systems
        /// </summary>
        /// <param name="myRecursiveOperation">Recursive operation?</param>
        /// <returns>A (recursive) list of the number of bytes within all mounted file systems</returns>
        IEnumerable<UInt64> GetNumberOfBytes(Boolean myRecursiveOperation, SessionToken mySessionToken);

        #endregion

        #region GetNumberOfFreeBytes(...)

        /// <summary>
        /// Returns the number of free bytes of this file system
        /// </summary>
        /// <returns>The number of free bytes of this file system</returns>
        UInt64              GetNumberOfFreeBytes(SessionToken mySessionToken);

        /// <summary>
        /// Returns the number of free bytes of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The number of free bytes of the file system at the given ObjectLocation</returns>
        UInt64              GetNumberOfFreeBytes(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        /// <summary>
        /// Returns a (recursive) list of the number of free bytes within all mounted file systems
        /// </summary>
        /// <param name="myRecursiveOperation">Recursive operation?</param>
        /// <returns>A (recursive) list of the number of free bytes within all mounted file systems</returns>
        IEnumerable<UInt64> GetNumberOfFreeBytes(Boolean myRecursiveOperation, SessionToken mySessionToken);

        #endregion

        #region GetAccessMode(...)

        /// <summary>
        /// Returns the access mode of this file system
        /// </summary>
        /// <returns>The access mode of this file system</returns>
        AccessModeTypes        GetAccessMode(SessionToken mySessionToken);

        /// <summary>
        /// Returns the access mode of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The access mode of the file system at the given ObjectLocation</returns>
        AccessModeTypes        GetAccessMode(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        /// <summary>
        /// Returns a (recursive) list of the access modes within all mounted file systems
        /// </summary>
        /// <param name="myRecursiveOperation">Recursive operation?</param>
        /// <returns>A (recursive) list of the access modes within all mounted file systems</returns>
        IEnumerable<AccessModeTypes> GetAccessModes(Boolean myRecursiveOperation, SessionToken mySessionToken);

        #endregion

        #region ParentFileSystem/ChildFileSystems

        IGraphFS ParentFileSystem { get; set; }
        IGraphFS GetChildFileSystem(ObjectLocation myObjectLocation, Boolean myRecursive, SessionToken mySessionToken);
        IEnumerable<ObjectLocation> GetChildFileSystemMountpoints(Boolean myRecursiveOperation, SessionToken mySessionToken);

        #endregion

        #endregion


        #region Make-/Grow-/ShrinkFileSystem

        /// <summary>
        /// This initialises a GraphFS in a given device or file using the given sizes
        /// </summary>
        /// <param name="myIStorageEngines">a device or filename where to store the file system data</param>
        /// <param name="myDescription">a distinguishable Name or description for the file system (can be changed later)</param>
        /// <param name="myNumberOfBytes">the size of the file system in byte</param>
        /// <param name="myOverwriteExistingFileSystem">overwrite an existing file system [yes|no]</param>
        /// <returns>the UUID of the new file system</returns>
        Exceptional<FileSystemUUID> MakeFileSystem(IEnumerable<IStorageEngine> myIStorageEngines, String myDescription, Boolean myOverwriteExistingFileSystem, Action<Double> myAction, SessionToken mySessionToken);

        /// <summary>
        /// This enlarges the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        Exceptional<UInt64> GrowFileSystem(UInt64 myNumberOfBytesToAdd, SessionToken mySessionToken);

        /// <summary>
        /// This reduces the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToRemove">the number of bytes to remove from the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        Exceptional<UInt64> ShrinkFileSystem(UInt64 myNumberOfBytesToRemove, SessionToken mySessionToken);

        #endregion

        #region MountFileSystem

        /// <summary>
        /// Mounts a GraphFS from a device or filename serving the file system
        /// </summary>
        /// <param name="myStorage">a device or filename serving the file system</param>
        /// <param name="myFSAccessMode">the access mode of this file system (read/write, read-only, ...)</param>
        Exceptional MountFileSystem(String myStorageLocation, AccessModeTypes myFSAccessMode, SessionToken mySessionToken);


        Exceptional MountFileSystem(IGraphFS myIGraphFS, ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode, SessionToken mySessionToken);

        /// <summary>
        /// This method will mount the file system from a StorageLocation serving
        /// the file system into the given ObjectLocation using the given file system
        /// access mode. If the mountpoint is located within another file system this
        /// file system will be called to process this request in a recursive way.
        /// </summary>
        /// <param name="myStorageLocation">A StorageLocation (device or filename) the file system can be read from</param>
        /// <param name="myMountPoint">The location the file system should be mounted at</param>
        /// <param name="myFSAccessMode">The access mode of the file system to mount</param>
        Exceptional MountFileSystem(String myStorageLocation, ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode, SessionToken mySessionToken);


        /// <summary>
        /// Remounts a GraphFS
        /// </summary>
        /// <param name="myFSAccessMode">the mode the file system should be opend (see AccessModeTypes)</param>
        Exceptional RemountFileSystem(AccessModeTypes myFSAccessMode, SessionToken mySessionToken);

        /// <summary>
        /// Remounts a GraphFS
        /// </summary>
        /// <param name="myMountPoint">the location of the file system within the virtual file system</param>
        /// <param name="myFSAccessMode">the mode the file system should be opend (see AccessModeTypes)</param>
        Exceptional RemountFileSystem(ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode, SessionToken mySessionToken);


        /// <summary>
        /// Unmounts a GraphFS by flushing all caches and shutting down all managers, finally closing the file system
        /// </summary>
        Exceptional UnmountFileSystem(SessionToken mySessionToken);

        /// <summary>
        /// Unmounts a GraphFS by flushing all caches and shutting down all managers, finally closing the file
        /// </summary>
        /// <param name="myMountPoint">the location of the file system within the virtual file system</param>
        Exceptional UnmountFileSystem(ObjectLocation myMountPoint, SessionToken mySessionToken);

        /// <summary>
        /// Unmounts all GraphFSs by flushing all caches and shutting down all managers, finally closing all files
        /// </summary>
        Exceptional UnmountAllFileSystems(SessionToken mySessionToken);


        /// <summary>
        /// Restricts the access to this file system to the given "/ChangeRootPrefix".
        /// This might be of interesst for security and safety purposes.
        /// </summary>
        /// <param name="myChangeRootPrefix">the location of this object (ObjectPath and ObjectName) of the new file system root</param>
        Exceptional ChangeRootDirectory(String myChangeRootPrefix, SessionToken mySessionToken);

        #endregion


        #region INode and ObjectLocator

        /// <summary>
        /// Exports the INode of the given ObjectLocation.
        /// For security reasons only a copy/clone of the INode will be exported!
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested INode within the file system</param>
        /// <returns>An INode copy/clone of the given ObjectLocation</returns>
        Exceptional<INode> GetINode(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        /// <summary>
        /// Exports the ObjectLocator of the given ObjectLocation.
        /// For security reasons only a copy/clone of the ObjectLocator will be exported!
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested ObjectLocator within the file system</param>
        /// <returns>An ObjectLocator copy/clone of the given ObjectLocation</returns>
        Exceptional<ObjectLocator> GetObjectLocator(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        #endregion

        #region GraphObject specific methods

        Exceptional LockFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, ObjectLocks myObjectLock, ObjectLockTypes myObjectLockType, UInt64 myLockingTime, SessionToken mySessionToken);

        Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, SessionToken mySessionToken) where PT : AFSObject, new();
        Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, Func<PT> myFunc, SessionToken mySessionToken) where PT : AFSObject;
        Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, SessionToken mySessionToken) where PT : AFSObject, new();
        Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, UInt64 myObjectCopy, Boolean myIgnoreIntegrityCheckFailures, Func<PT> myFunc, SessionToken mySessionToken) where PT : AFSObject;

        Exceptional StoreFSObject(ObjectLocation myObjectLocation, AFSObject myAPandoraObject, Boolean myAllowOverwritting, SessionToken mySessionToken);

        Exceptional<Trinary> ObjectExists         (ObjectLocation myObjectLocation, SessionToken mySessionToken);
        Exceptional<Trinary> ObjectStreamExists   (ObjectLocation myObjectLocation, String myObjectStream, SessionToken mySessionToken);
        Exceptional<Trinary> ObjectEditionExists  (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, SessionToken mySessionToken);
        Exceptional<Trinary> ObjectRevisionExists (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, SessionToken mySessionToken);

        Exceptional<IEnumerable<String>>     GetObjectStreams     (ObjectLocation myObjectLocation, SessionToken mySessionToken);
        Exceptional<IEnumerable<String>>     GetObjectEditions    (ObjectLocation myObjectLocation, String myObjectStream, SessionToken mySessionToken);
        Exceptional<IEnumerable<RevisionID>> GetObjectRevisionIDs (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, SessionToken mySessionToken);

        Exceptional RenameObject(ObjectLocation myObjectLocation, String myNewObjectName, SessionToken mySessionToken);
        Exceptional RemoveFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, SessionToken mySessionToken);
        Exceptional EraseFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID, SessionToken mySessionToken);

        #endregion

        #region Symlink Maintenance

        /// <summary>
        /// Adds a symlink to another object within the file system
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the symlink within the file system</param>
        /// <param name="myTargetLocation">the location of this object (ObjectPath and ObjectName) of the symlink target within the file system</param>
        Exceptional AddSymlink(ObjectLocation myObjectLocation, ObjectLocation myTargetLocation, SessionToken mySessionToken);

        /// <summary>
        /// Checks the existence of a symlink at the given file system location
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the symlink within the file system</param>        
        /// <returns>exists(true) or not exists(false)</returns>
        Exceptional<Trinary> isSymlink(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        /// <summary>
        /// Returns the target of a symlink
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the symlink within the file system</param>        
        /// <returns>a string representing the locaction of another file system object within the file system</returns>
        Exceptional<ObjectLocation> GetSymlink(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        /// <summary>
        /// This method removes a symlink from the parent directory.
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        Exceptional RemoveSymlink(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        #endregion

        #region DirectoryObject Methods

        /// <summary>
        /// Creates a directory in the given file system location reserving the given number of bytes 
        /// </summary>
        /// <param name="myObjectLocation"></param>
        /// <param name="myBlocksize"></param>
        /// <param name="mySessionToken"></param>
        /// <returns></returns>
        Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation, UInt64 myBlocksize, SessionToken mySessionToken);

        /// <summary>
        /// Creates a directory in the given file system location, it will recursively all not existing subdirectories
        /// </summary>
        /// <param name="myObjectLocation"></param>
        /// <param name="myBlocksize"></param>
        /// <param name="myRecursive"></param>
        /// <param name="mySessionToken"></param>
        /// <returns></returns>
        Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation, UInt64 myBlocksize, Boolean myRecursive, SessionToken mySessionToken);


        /// <summary>
        /// Checks the existence of a directory in the given file system location
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested directory within the file system</param>        
        /// <returns>exists(true) or not exists(false)</returns>        
        Exceptional<Trinary> isIDirectoryObject(ObjectLocation myObjectLocation, SessionToken mySessionToken);


        /// <summary>
        /// Returns all directory entries at the given object location as a list of strings.
        /// </summary>
        /// <returns>List of strings containing a list of all directory entries</returns>
        Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation, Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc, SessionToken mySessionToken);

        /// <summary>
        /// Returns all directory entries at the given object location as a list of strings.
        /// Additionally filters may be applied to the output.
        /// </summary>
        /// <returns>List of strings containing a filtered list of all directory entries</returns>
        Exceptional<IEnumerable<String>> GetFilteredDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime, SessionToken mySessionToken);


        /// <summary>
        /// Returns all directory entries at the given object location as a list of dictionaries.
        /// </summary>
        /// <returns>List of dictionaries containing a list of all directory entries</returns>
        Exceptional<IEnumerable<DirectoryEntryInformation>> GetExtendedDirectoryListing(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        Exceptional<IEnumerable<DirectoryEntryInformation>> GetExtendedDirectoryListing(ObjectLocation myObjectLocation, Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc, SessionToken mySessionToken);

        /// <summary>
        /// Returns all directory entries at the given object location as a list of dictionaries.
        /// Additionally filters may be applied to the output.
        /// </summary>
        /// <returns>List of dictionaries containing a filtered list of all directory entries</returns>
        Exceptional<IEnumerable<DirectoryEntryInformation>> GetFilteredExtendedDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime, SessionToken mySessionToken);


        /// <summary>
        /// This method removes a DirectoryObject from the parent directory objectstream. The Directory and it content is not physically deleted.
        /// As long as there is a revision containing this ObjectStream, you can restore the Directory
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the directory which will be deleted</param>        
        /// <param name="myRemoveRecursive">remove the directory recursive</param>        
        Exceptional RemoveDirectoryObject(ObjectLocation myObjectLocation, Boolean removeRecursive, SessionToken mySessionToken);

        /// <summary>
        /// Erases a directory in the given file system location
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the directory within the file system</param>
        /// <param name="myRemoveRecursive">erase the directory recursive</param>        
        Exceptional EraseDirectoryObject(ObjectLocation myObjectLocation, Boolean eradeRecursive, SessionToken mySessionToken);

        #endregion

        #region MetadataObject Methods

        Exceptional SetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myUserMetadataKey, TValue myMetadatum, IndexSetStrategy myIndexSetStrategy, SessionToken mySessionToken);
        Exceptional SetMetadata<TValue> (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata, IndexSetStrategy myIndexSetStrategy, SessionToken mySessionToken);

        Exceptional<Trinary> MetadatumExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myUserMetadataKey, TValue myMetadatum, SessionToken mySessionToken);
        Exceptional<Trinary> MetadataExists<TValue> (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myUserMetadataKey, SessionToken mySessionToken);

        Exceptional<IEnumerable<TValue>>                       GetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, SessionToken mySessionToken);
        Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue> (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, SessionToken mySessionToken);
        Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue> (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myMinKey, String myMaxKey, SessionToken mySessionToken);
        Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue> (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc, SessionToken mySessionToken);

        Exceptional RemoveMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myUserMetadataKey, TValue myMetadatum, SessionToken mySessionToken);
        Exceptional RemoveMetadata<TValue> (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myUserMetadataKey, SessionToken mySessionToken);
        Exceptional RemoveMetadata<TValue> (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata, SessionToken mySessionToken);
        Exceptional RemoveMetadata<TValue> (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc, SessionToken mySessionToken);

        #endregion


        //#region AccessControlObject Maintenance

        ///// <summary>
        ///// This method adds a RIGTHSSTREAM to an Object.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myRightsObject">The AccessControlObject that should be added to the Object.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddRightsStreamToObject(ObjectLocation myObjectLocation, AccessControlObject myRightsObject, SessionToken mySessionToken);

        ///// <summary>
        ///// Adds an entity to the AllowACL of a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The right that should be granted.</param>
        ///// <param name="myEntitiyUUID">The guid of the entity.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddEntityToRightsStreamAllowACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// Adds an entity to the DenyACL of a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The right that should be denied.</param>
        ///// <param name="myEntitiyUUID">The guid of the entity.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddEntityToRightsStreamDenyACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// Removes an entity from a ACL that is related to a right within the allowACL.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The right which the entity should not be allowed for anymore.</param>
        ///// <param name="myEntitiyUUID">The guid of the entity.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveEntityFromRightsStreamAllowACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// Removes an entity from a ACL that is related to a right within the denyACL.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The right which the entity should not be denied of anymore.</param>
        ///// <param name="myEntitiyUUID">The guid of the entity.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveEntityFromRightsStreamDenyACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// Changes the AllowOverDeny property of a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myAllowOverDeny">The new value the AllowOverDeny property</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool ChangeAllowOverDenyOfRightsStream(ObjectLocation myObjectLocation, DefaultRuleTypes myDefaultRule, SessionToken mySessionToken);

        ///// <summary>
        ///// Adds an alert to a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myAlert">The alert that should be added.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddAlertToPandoraRightsAlertHandlingList(ObjectLocation myObjectLocation, NHAccessControlObject myAlert, SessionToken mySessionToken);

        ///// <summary>
        ///// Removes an alert from a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myAlert">The alert that should be removed.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveAlertFromPandoraRightsAlertHandlingList(ObjectLocation myObjectLocation, NHAccessControlObject myAlert, SessionToken mySessionToken);

        //#endregion

        //#region Evaluation of rights

        //List<Right> EvaluateRightsForEntity(ObjectLocation myObjectLocation, EntityUUID myEntityGuid, AccessControlObject myRightsObject, SessionToken mySessionToken);

        //#endregion

        //#region Index Maintenance

        ////void CreateIndexObject<T1, T2, T3>(ObjectLocation myObjectLocation) where T1 : APandoraObject, IIndexObject<T2, T3>, new() where T2 : IComparable;
        //IIndexObject<TKey, TValue> CreateIndexObject<TKey, TValue>(ObjectLocation myObjectLocation, IndexObjectTypes myIndexObjectType, SessionToken mySessionToken) where TKey : IComparable;

        //IIndexObject<TKey, TValue> CreateIndex<TKey, TValue>(ObjectLocation myObjectLocation, IndexObjectTypes myIndexObjectType, Boolean myScaleableIndex, SessionToken mySessionToken) where TKey : IComparable;

        ///// <summary>
        ///// Stores any object identified by the given metadata key within a user MetadataObject at the given file system location 
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <param name="myKey">the key to identify the object/metadatum to store</param>
        ///// <param name="myValue">an object</param>
        //void InsertIntoIndex<TKey, TValue>(ObjectLocation myObjectLocation, TKey myKey, List<TValue> myValue, SessionToken mySessionToken) where TKey : IComparable;

        ///// <summary>
        ///// Stores a list of <String, Object>-KeyValuePairs within a user MetadataObject at the given file system location 
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <param name="myIndexList">a list of KeyValuePairs to store</param>
        //void InsertIntoIndex<TKey, TValue>(ObjectLocation myObjectLocation, List<KeyValuePair<TKey, List<TValue>>> myIndexList, SessionToken mySessionToken) where TKey : IComparable;

        ///// <summary>
        ///// Stores a dictionary of type <String, Object> within a user MetadataObject at the given file system location 
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <param name="myIndexDictionary">a dictionary of type <String, Object> to store</param>
        //void InsertIntoIndex<TKey, TValue>(ObjectLocation myObjectLocation, Dictionary<TKey, List<TValue>> myIndexDictionary, SessionToken mySessionToken) where TKey : IComparable;


        ///// <summary>
        ///// Returns the requested user metadatum indexed by the object location and the metadata key.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadata object within the file system</param>        
        ///// <param name="myKey">the key or identifier of the requested metadatum</param>
        ///// <returns>the requested metadatum</returns>
        //List<TValue> GetValueFromIndex<TKey, TValue>(ObjectLocation myObjectLocation, TKey myUserMetadataKey, SessionToken mySessionToken) where TKey : IComparable;

        ///// <summary>
        ///// Returns a list of key-value-pairs of all user metadata indexed by the object location.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <returns>the requested list of all metadata</returns>
        //List<KeyValuePair<TKey, TValue>> GetIndexAsList<TKey, TValue>(ObjectLocation myObjectLocation, SessionToken mySessionToken) where TKey : IComparable;

        ///// <summary>
        ///// Returns a dictionary of all user metadata indexed by the object location.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <returns>the requested dictionary of all metadata</returns>
        //Dictionary<TKey, List<TValue>> GetIndexAsDictionary<TKey, TValue>(ObjectLocation myObjectLocation, SessionToken mySessionToken) where TKey : IComparable;


        ///// <summary>
        ///// Deletes the entire user metadata object at the given object location.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the metadatum within the file system</param>        
        //void DeleteIndexObject(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        ///// <summary>
        ///// Deletes a user metadatum at the given object location indexed by the metadata key.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the metadatum within the file system</param>        
        ///// <param name="myUserMetadataKey">the key or identifier of the metadatum</param>
        //void DeleteKeyFromIndex<TKey, TValue>(ObjectLocation myObjectLocation, TKey myKey, SessionToken mySessionToken) where TKey : IComparable;


        ///// <summary>
        ///// Checks if a specific user metadata object exists at the given object location.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the metadatum within the file system</param>        
        ///// <returns>exists(true) or not exists(false)</returns>
        //Boolean IndexObjectExists(ObjectLocation myObjectLocation, String nameOfKeyClass, SessionToken mySessionToken);

        ///// <summary>
        ///// Checks if a specific user metadata key indexed by the given object location exists.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the metadatum within the file system</param>        
        ///// <param name="myUserMetadataKey">the key or identifier of the metadatum</param>
        ///// <returns>exists(true) or not exists(false)</returns>
        //Boolean IndexHasKey<TKey, TValue>(ObjectLocation myObjectLocation, TKey myKey, SessionToken mySessionToken) where TKey : IComparable;

        //#endregion

        //#region StringListObject Maintenance

        //void CreateListOfStrings(ObjectLocation myObjectLocation, SessionToken mySessionToken);
        //List<String> GetListOfStrings(ObjectLocation myObjectLocation, SessionToken mySessionToken);
        //void AddToListOfStrings(ObjectLocation myObjectLocation, String myNewString, SessionToken mySessionToken);
        //void RemoveFromListOfStrings(ObjectLocation myObjectLocation, String myToBeRemovedString, SessionToken mySessionToken);
        //Boolean ListOfStringsExist(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        //#endregion

        //#region EntitiesObject Maintenance

        ///// <summary>
        ///// This method tries to add an Entity to the EntitiesObject.
        ///// It is added if it has a correct aMemberDefinition list (correct means, that 
        ///// the Entities, which are referenced by the IDs have to exist) 
        ///// and a not empty string as Name.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myLogin">The Name of the entity, should not be empty or null.</param>
        ///// <param name="myPasswordHash">The hash value of the password of the entity.</param>
        ///// <param name="myPublicKey">The public key of the entity.</param>
        ///// <param name="myMemberDefinition">A list of memberships.</param>
        ///// <returns>The EntityUUID of the added entity</returns>
        //EntityUUID AddEntity(ObjectLocation myObjectLocation, String myLogin, String myRealname, String myDescription, Dictionary<ContactTypes, List<String>> myContacts, String myPassword, List<PublicKey> myPublicKeyList, HashSet<EntityUUID> myMembership, SessionToken mySessionToken);

        ///// <summary>
        ///// This method checks if a given entity, which is referenced by its guid, is 
        ///// present in the current FS.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="_RightUUID">The guid of the entity.</param>
        ///// <returns>Returns true on a valid guid. False else.</returns>
        //Trinary EntityExists(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// This method returns the the guid for a given entity Name.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="aName">The Name of an entity.</param>
        ///// <returns>The guid of the entity or null if it does not exist</returns>
        //EntityUUID GetEntityUUID(ObjectLocation myObjectLocation, String aName, SessionToken mySessionToken);

        ///// <summary>
        ///// This method removes a given Guid (references an Entity) from the EntitiesObject.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="_RightUUID">The Guid of the Entity that should be removed.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveEntity(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// This mehtod returns a list of memberships concerning a given Guid (references an Entity).
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="_RightUUID">The Guid of the Entity.</param>
        ///// <returns>A list of Guids if the given Guid exists or otherwise null.</returns>
        //HashSet<EntityUUID> GetMemberships(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, Boolean myRecursion, SessionToken mySessionToken);

        ///// <summary>
        ///// This method changes the password hash to a given Guid, which references an Entity.
        ///// If the old password hash parameter matches the entities password hash, it is replaced by the new one.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The Guid of the Entity, whose password hash should be changed.</param>
        ///// <param name="myOldPassword">The old password hash.</param>
        ///// <param name="myNewPassword">The new password hash.</param>
        ///// <returns>True for success, or otherwise false.</returns>
        //void ChangeEntityPassword(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, String myOldPassword, String myNewPassword, SessionToken mySessionToken);

        ///// <summary>
        ///// This method changes the public key of an Entity, which is referenced by its Guid.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The UUID of the entity.</param>
        ///// <param name="myPassword">the entity password</param>
        ///// <param name="myNewPublicKeyList">The new list of public keys</param>
        //void ChangeEntityPublicKeyList(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, String myPassword, List<PublicKey> myNewPublicKeyList, SessionToken mySessionToken);

        ///// <summary>
        ///// This method adds a new membership for an Entity which is referenced by the Guid parameter.
        ///// This operation can only succeed, if the Guid of the new membership really exists.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The UUID of the Entity.</param>
        ///// <param name="myNewMembershipUUID">The UUID of the new membership.</param>
        //void ChangeEntityAddMembership(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, EntityUUID myNewMembershipUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// This method removes a membership for an Entity which is referenced by the Guid parameter.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The Guid of the Entity.</param>
        ///// <param name="myNewMembershipUUID">The UUID of the new membership.</param>
        //void ChangeEntityRemoveMembership(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, EntityUUID myNewMembershipUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// This method returns the public key concerning a Entity, which is referenced by its Guid.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The Guid that references the Entity.</param>
        ///// <returns>A list of public keys</returns>
        //List<PublicKey> GetEntityPublicKeyList(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, SessionToken mySessionToken);

        //#endregion

        //#region Rights ObjectIndex Maintenance
        
        ///// <summary>
        ///// This method adds a Right to the GraphFS. All 
        ///// Rights that are added via this method are treated as 
        ///// userdefined rights. Additionally it is possible to add 
        ///// a validation script which is evaluated while trying to 
        ///// use an object with this constraint.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myLogin">The Name of the right. Cannot be null or empty.</param>
        ///// <param name="myValidationScript">The validation script for evaluating the access of an entity. Can be null or empty.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddPandoraRight(ObjectLocation myObjectLocation, String Name, String ValidationScript, SessionToken mySessionToken);

        ///// <summary>
        ///// This method removes a Right from the GraphFS. It is 
        ///// not possible to remove a non userdefined right.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The UUID of the right.</param>
        ///// <returns>True for success or otherwise false</returns>
        //bool RemovePandoraRight(ObjectLocation myObjectLocation, RightUUID myRightUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// Returns a Right correspondig to its Name.
        ///// </summary>
        ///// <param name="RightName">The Name of the Right.</param>
        ///// <returns>The Right.</returns>
        //Right GetRightByName(String RightName, SessionToken mySessionToken);

        //Boolean ContainsRightUUID(RightUUID myRightUUID, SessionToken mySessionToken);

        //#endregion


        #region FlushObjectLocationNew(myObjectLocation, SessionToken mySessionToken);

        void FlushObjectLocationNew(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        #endregion

        #region StorageEngine Maintenance

        #region StorageLocations(...)

        /// <summary>
        /// Returns a list of all StorageLocations associated with this file system
        /// </summary>
        /// <returns>A list of all StorageLocations associated with this file system</returns>
        IEnumerable<String> StorageLocations(SessionToken mySessionToken);

        /// <summary>
        /// Returns a list of all StorageLocations associated with the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>A list of all StorageLocations associated with the file system at the given ObjectLocation</returns>
        IEnumerable<String> StorageLocations(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        #endregion

        #region StorageUUIDs(...)

        /// <summary>
        /// Returns a list of all StorageUUIDs associated with this file system
        /// </summary>
        /// <returns>A list of all StorageUUIDs associated with this file system</returns>
        IEnumerable<StorageUUID> StorageUUIDs(SessionToken mySessionToken);

        /// <summary>
        /// Returns a list of all StorageUUIDs associated with the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>A list of all StorageUUIDs associated with the file system at the given ObjectLocation</returns>
        IEnumerable<StorageUUID> StorageUUIDs(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        #endregion

        #region StorageDescriptions(...)

        /// <summary>
        /// Returns a list of descriptions for the storage engines associated with this file system
        /// </summary>
        /// <returns>A list of descriptions for the storage engines associated with this file system</returns>
        IEnumerable<String> StorageDescriptions(SessionToken mySessionToken);

        /// <summary>
        /// Returns a list of descriptions for the storage engines associated with the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>A list of descriptions for the storage engines associated with the file system at the given ObjectLocation</returns>
        IEnumerable<String> StorageDescriptions(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        #endregion

        #endregion

        #region Transactions

        FSTransaction BeginTransaction(SessionToken mySessionToken, Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null);

        #endregion

        #region Stream

        Exceptional<IGraphFSStream> OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevision, UInt64 myObjectCopy);

        Exceptional<IGraphFSStream> OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevision, UInt64 myObjectCopy,
                                    FileMode myFileMode, FileAccess myFileAccess, FileShare myFileShare, FileOptions myFileOptions, UInt64 myBufferSize);

        #endregion

        #region ObjectCache

        /// <summary>
        /// Returns the ObjectCache settings of this file system
        /// </summary>
        /// <returns>The ObjectCache settings of this file system</returns>
        ObjectCacheSettings GetObjectCacheSettings(SessionToken mySessionToken);

        /// <summary>
        /// Returns the ObjectCache settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The ObjectCache settings of the file system at the given ObjectLocation</returns>
        ObjectCacheSettings GetObjectCacheSettings(ObjectLocation myObjectLocation, SessionToken mySessionToken);


        /// <summary>
        /// Sets the ObjectCache settings of this file system
        /// </summary>
        /// <param name="myNotificationSettings">A ObjectCacheSettings object</param>
        void SetObjectCacheSettings(ObjectCacheSettings myObjectCacheSettings, SessionToken mySessionToken);

        /// <summary>
        /// Sets the ObjectCache settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myNotificationSettings">A ObjectCacheSettings object</param>
        void SetObjectCacheSettings(ObjectLocation myObjectLocation, ObjectCacheSettings myObjectCacheSettings, SessionToken mySessionToken);

        #endregion

        #region NotificationDispatcher

        // The NotificationDispatcher handles all kind of notification between system parts or other dispatchers.
        // Use register to get notified as recipient.
        // Use SendNotification to send a notification to all subscribed recipients.

        #region GetNotificationDispatcher(...)

        /// <summary>
        /// Returns the NotificationDispatcher of this file system
        /// </summary>
        /// <returns>The NotificationDispatcher of this file system</returns>
        NotificationDispatcher GetNotificationDispatcher(SessionToken mySessionToken);

        /// <summary>
        /// Returns the NotificationDispatcher of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The NotificationDispatcher of the file system at the given ObjectLocation</returns>
        NotificationDispatcher GetNotificationDispatcher(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        #endregion

        #region GetNotificationSettings(...)

        /// <summary>
        /// Returns the NotificationDispatcher settings of this file system
        /// </summary>
        /// <returns>The NotificationDispatcher settings of this file system</returns>
        NotificationSettings GetNotificationSettings(SessionToken mySessionToken);

        /// <summary>
        /// Returns the NotificationDispatcher settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The NotificationDispatcher settings of the file system at the given ObjectLocation</returns>
        NotificationSettings GetNotificationSettings(ObjectLocation myObjectLocation, SessionToken mySessionToken);

        #endregion


        #region SetNotificationDispatcher(..., myNotificationDispatcher)

        /// <summary>
        /// Sets the NotificationDispatcher of this file system.
        /// </summary>
        /// <param name="myNotificationDispatcher">A NotificationDispatcher object</param>
        void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher, SessionToken mySessionToken);

        /// <summary>
        /// Sets the NotificationDispatcher of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myNotificationDispatcher">A NotificationDispatcher object</param>
        void SetNotificationDispatcher(ObjectLocation myObjectLocation, NotificationDispatcher myNotificationDispatcher, SessionToken mySessionToken);

        #endregion

        #region SetNotificationSettings(..., myNotificationSettings)

        /// <summary>
        /// Sets the NotificationDispatcher settings of this file system
        /// </summary>
        /// <param name="myNotificationSettings">A NotificationSettings object</param>
        void SetNotificationSettings(NotificationSettings myNotificationSettings, SessionToken mySessionToken);

        /// <summary>
        /// Sets the NotificationDispatcher settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myNotificationSettings">A NotificationSettings object</param>
        void SetNotificationSettings(ObjectLocation myObjectLocation, NotificationSettings myNotificationSettings, SessionToken mySessionToken);

        #endregion

        #endregion


    }

}
