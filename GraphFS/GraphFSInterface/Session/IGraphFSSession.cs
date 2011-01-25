/*
 * IGraphFSSession
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.IO;
using System.ServiceModel;
using System.Collections.Generic;

using sones.GraphFS.Caches;
using sones.GraphFS.Events;
using sones.GraphFS.Objects;
using sones.GraphFS.Transactions;
using sones.GraphFS.DataStructures;
using sones.GraphFS.InternalObjects;

using sones.GraphFS.Session;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Indices;

using sones.Notifications;

#endregion

namespace sones.GraphFS.Session
{

    /// <summary>
    /// The session interface for all Graph file systems
    /// </summary>

    public interface IGraphFSSession
    {


        #region Properties

        IGraphFS     IGraphFS      { get; }
        SessionToken SessionToken  { get; }
        String       Implementation { get; }

        #endregion

        #region Events
        
        // AFSObject handling
        event GraphFSEventHandlers.OnLoadEventHandler                       OnLoad;
        event GraphFSEventHandlers.OnLoadedEventHandler                     OnLoaded;
        event GraphFSEventHandlers.OnLoadedAsyncEventHandler                OnLoadedAsync;

        event GraphFSEventHandlers.OnSaveEventHandler                       OnSave;
        event GraphFSEventHandlers.OnSavedEventHandler                      OnSaved;
        event GraphFSEventHandlers.OnSavedAsyncEventHandler                 OnSavedAsync;
        
        event GraphFSEventHandlers.OnRemoveEventHandler                     OnRemove;
        event GraphFSEventHandlers.OnRemovedEventHandler                    OnRemoved;
        event GraphFSEventHandlers.OnRemovedAsyncEventHandler               OnRemovedAsync;

        // Transaction handling
        event GraphFSEventHandlers.OnTransactionStartEventHandler           OnTransactionStart;
        event GraphFSEventHandlers.OnTransactionStartedEventHandler         OnTransactionStarted;
        event GraphFSEventHandlers.OnTransactionStartedAsyncEventHandler    OnTransactionStartedAsync;

        event GraphFSEventHandlers.OnTransactionCommitEventHandler          OnTransactionCommit;
        event GraphFSEventHandlers.OnTransactionCommittedEventHandler       OnTransactionCommitted;
        event GraphFSEventHandlers.OnTransactionCommittedAsyncEventHandler  OnTransactionCommittedAsync;

        event GraphFSEventHandlers.OnTransactionRollbackEventHandler        OnTransactionRollback;
        event GraphFSEventHandlers.OnTransactionRollbackedEventHandler      OnTransactionRollbacked;
        event GraphFSEventHandlers.OnTransactionRollbackedAsyncEventHandler OnTransactionRollbackedAsync;

        #endregion

        #region Session specific Members

        IGraphFSSession CreateNewSession(String myUsername);

        #endregion

        #region Information Methods

        #region IsMounted

        /// <summary>
        /// Returns true if the file system was mounted correctly
        /// </summary>
        /// <returns>true if the file system was mounted correctly</returns>
        Boolean IsMounted { get; }

        #endregion

        #region IsPersistent

        Boolean IsPersistent { get; }

        #endregion


        #region TraverseChildFSs(myFunc, myDepth)

        IEnumerable<Object> TraverseChildFSs(Func<IGraphFS, UInt64, IEnumerable<Object>> myFunc, UInt64 myDepth);

        #endregion


        #region GetFileSystemUUID(...)

        /// <summary>
        /// Returns the UUID of this file system
        /// </summary>
        /// <returns>The UUID of this file system</returns>
        FileSystemUUID GetFileSystemUUID();

        /// <summary>
        /// Returns the UUID of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The UUID of the file system at the given ObjectLocation</returns>
        FileSystemUUID GetFileSystemUUID(ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns a recursive list of FileSystemUUIDs of all mounted file systems
        /// </summary>
        /// <param name="myDepth">Depth</param>
        /// <returns>A (recursive) list of FileSystemUUIDs of all mounted file systems</returns>
        IEnumerable<FileSystemUUID> GetFileSystemUUIDs(UInt64 myDepth);

        #endregion

        #region GetFileSystemDescription(...)

        /// <summary>
        /// Returns the name or a description of this file system.
        /// </summary>
        /// <returns>The name or a description of this file system</returns>
        String GetFileSystemDescription();

        /// <summary>
        /// Returns the name or a description of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The name or a description of the file system at the given ObjectLocation</returns>
        String GetFileSystemDescription(ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns a (recursive) list of file system descriptions of all mounted file systems
        /// </summary>
        /// <param name="myRecursiveOperation">Recursive operation?</param>
        /// <returns>A (recursive) list of file system descriptions of all mounted file systems</returns>
        IEnumerable<String> GetFileSystemDescriptions(UInt64 myDepth);

        #endregion

        #region SetFileSystemDescription(...)

        /// <summary>
        /// Sets the Name or a description of this file system.
        /// </summary>
        /// <param name="myFileSystemDescription">the Name or a description of this file system</param>
        void SetFileSystemDescription(String myFileSystemDescription);

        /// <summary>
        /// Sets the Name or a description of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myFileSystemDescription">the Name or a description of the file system at the given ObjectLocation</param>
        void SetFileSystemDescription(ObjectLocation myObjectLocation, String myFileSystemDescription);

        #endregion

        #region GetNumberOfBytes(...)

        /// <summary>
        /// Returns the size (number of bytes) of this file system
        /// </summary>
        /// <returns>The size (number of bytes) of this file system</returns>
        [OperationContract(Name = "GetNumberOfBytes1")]
        UInt64 GetNumberOfBytes();

        /// <summary>
        /// Returns the size (number of bytes) of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The size (number of bytes) of the file system at the given ObjectLocation</returns>
        [OperationContract(Name = "GetNumberOfBytes2")]
        UInt64 GetNumberOfBytes(ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns a (recursive) list of the number of bytes within all mounted file systems
        /// </summary>
        /// <param name="myRecursiveOperation">Recursive operation?</param>
        /// <returns>A (recursive) list of the number of bytes within all mounted file systems</returns>
        [OperationContract(Name = "GetNumberOfBytes3")]
        IEnumerable<UInt64> GetNumberOfBytes(Boolean myRecursiveOperation);

        #endregion

        #region GetNumberOfFreeBytes(...)

        /// <summary>
        /// Returns the number of free bytes of this file system
        /// </summary>
        /// <returns>The number of free bytes of this file system</returns>
        [OperationContract(Name = "GetNumberOfFreeBytes1")]
        UInt64 GetNumberOfFreeBytes();

        /// <summary>
        /// Returns the number of free bytes of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The number of free bytes of the file system at the given ObjectLocation</returns>
        [OperationContract(Name = "GetNumberOfFreeBytes2")]
        UInt64 GetNumberOfFreeBytes(ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns a (recursive) list of the number of free bytes within all mounted file systems
        /// </summary>
        /// <param name="myRecursiveOperation">Recursive operation?</param>
        /// <returns>A (recursive) list of the number of free bytes within all mounted file systems</returns>
        [OperationContract(Name = "GetNumberOfFreeBytes3")]
        IEnumerable<UInt64> GetNumberOfFreeBytes(Boolean myRecursiveOperation);

        #endregion

        #region GetAccessMode(...)

        /// <summary>
        /// Returns the access mode of this file system
        /// </summary>
        /// <returns>The access mode of this file system</returns>
        AccessModeTypes GetAccessMode();

        /// <summary>
        /// Returns the access mode of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The access mode of the file system at the given ObjectLocation</returns>
        AccessModeTypes GetAccessMode(ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns a (recursive) list of the access modes within all mounted file systems
        /// </summary>
        /// <param name="myRecursiveOperation">Recursive operation?</param>
        /// <returns>A (recursive) list of the access modes within all mounted file systems</returns>
        IEnumerable<AccessModeTypes> GetAccessModes(Boolean myRecursiveOperation);

        #endregion

        #region ParentFileSystem

        IGraphFS ParentFileSystem { get; set; }

        #endregion

        #region ChildFileSystems

        //Dictionary<String, IGraphFS> ChildFileSystems { get; }
        IEnumerable<ObjectLocation> GetChildFileSystemMountpoints(Boolean myRecursiveOperation);

        IGraphFS GetChildFileSystem(ObjectLocation myObjectLocation, Boolean myRecursive);

        #endregion

        #endregion

        #region ObjectCache

        /// <summary>
        /// Returns the ObjectCache settings of this file system
        /// </summary>
        /// <returns>The ObjectCache settings of this file system</returns>
        Exceptional<ObjectCacheSettings> GetObjectCacheSettings();

        /// <summary>
        /// Returns the ObjectCache settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The ObjectCache settings of the file system at the given ObjectLocation</returns>
        Exceptional<ObjectCacheSettings> GetObjectCacheSettings(ObjectLocation myObjectLocation);


        /// <summary>
        /// Sets the ObjectCache settings of this file system
        /// </summary>
        /// <param name="myNotificationSettings">A ObjectCacheSettings object</param>
        Exceptional SetObjectCacheSettings(ObjectCacheSettings myObjectCacheSettings);

        /// <summary>
        /// Sets the ObjectCache settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myNotificationSettings">A ObjectCacheSettings object</param>
        Exceptional SetObjectCacheSettings(ObjectLocation myObjectLocation, ObjectCacheSettings myObjectCacheSettings);

        #endregion


        #region Make-/Grow-/Shrink-/WipeFileSystem

        /// <summary>
        /// This initialises a IGraphFS in a given device or file using the given sizes
        /// </summary>
        /// <param name="myStorageLocations">a device or filename where to store the file system data</param>
        /// <param name="myDescription">a distinguishable Name or description for the file system (can be changed later)</param>
        /// <param name="myNumberOfBytes">the size of the file system in byte</param>
        /// <param name="myOverwriteExistingFileSystem">overwrite an existing file system [yes|no]</param>
        /// <returns>the UUID of the new file system</returns>
        Exceptional<FileSystemUUID> MakeFileSystem(String myDescription, UInt64 myNumberOfBytes, Boolean myOverwriteExistingFileSystem, Action<Double> myAction);

        /// <summary>
        /// This enlarges the size of a IGraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        Exceptional<UInt64> GrowFileSystem(UInt64 myNumberOfBytesToAdd);

        /// <summary>
        /// This reduces the size of a IGraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToRemove">the number of bytes to remove from the size of the current file system</param>
        Exceptional<UInt64> ShrinkFileSystem(UInt64 myNumberOfBytesToRemove);

        /// <summary>
        /// Wipe the file system
        /// </summary>
        Exceptional WipeFileSystem();

        #endregion

        #region Mount-/Remount-/UnmountFileSystem

        /// <summary>
        /// Mounts a IGraphFS from a device or filename serving the file system with an existing Notification dispatcher
        /// </summary>
        /// <param name="myStorage">a device or filename serving the file system</param>
        /// <param name="myFSAccessMode">the access mode of this file system (read/write, read-only, ...)</param>
        /// <param name="myNotificationDispatcher">the notification dispatcher with already registered recipients to get notifications during mount</param>
        Exceptional MountFileSystem(AccessModeTypes myFSAccessMode);

        Exceptional MountFileSystem(ObjectLocation myMountPoint, IGraphFSSession myIGraphFSSession, AccessModeTypes myFSAccessMode);


        /// <summary>
        /// Remounts a IGraphFS
        /// </summary>
        /// <param name="myFSAccessMode">the mode the file system should be opend (see AccessModeTypes)</param>
        Exceptional RemountFileSystem(AccessModeTypes myFSAccessMode);

        /// <summary>
        /// Remounts a IGraphFS
        /// </summary>
        /// <param name="myMountPoint">the location of the file system within the virtual file system</param>
        /// <param name="myFSAccessMode">the mode the file system should be opend (see AccessModeTypes)</param>
        Exceptional RemountFileSystem(ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode);


        /// <summary>
        /// Unmounts a IGraphFS by flushing all caches and shutting down all managers, finally closing the file system
        /// </summary>
        Exceptional UnmountFileSystem();

        /// <summary>
        /// Unmounts a IGraphFS by flushing all caches and shutting down all managers, finally closing the file
        /// </summary>
        /// <param name="myMountPoint">the location of the file system within the virtual file system</param>
        Exceptional UnmountFileSystem(ObjectLocation myMountPoint);

        /// <summary>
        /// Unmounts all GraphFSs by flushing all caches and shutting down all managers, finally closing all files
        /// </summary>
        Exceptional UnmountAllFileSystems();


        /// <summary>
        /// Restricts the access to this file system to the given "/ChangeRootPrefix".
        /// This might be of interesst for security and safety purposes.
        /// </summary>
        /// <param name="myChangeRootPrefix">the location of this object (ObjectPath and ObjectName) of the new file system root</param>
        Exceptional ChangeRootDirectory(String myChangeRootPrefix);

        #endregion


        #region Get INode and ObjectLocator

        /// <summary>
        /// Exports the INode of the given ObjectLocation.
        /// For security reasons only a copy/clone of the INode will be exported!
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested INode within the file system</param>
        /// <returns>An INode copy/clone of the given ObjectLocation</returns>
        Exceptional<INode> GetINode(ObjectLocation myObjectLocation);

        /// <summary>
        /// Exports the ObjectLocator of the given ObjectLocation.
        /// For security reasons only a copy/clone of the ObjectLocator will be exported!
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested ObjectLocator within the file system</param>
        /// <returns>An ObjectLocator copy/clone of the given ObjectLocation</returns>
        Exceptional<ObjectLocator> GetObjectLocator(ObjectLocation myObjectLocation);

        #endregion

        #region Object/ObjectStream/ObjectEdition/ObjectRevision infos

        Exceptional<Trinary> ObjectExists(ObjectLocation myObjectLocatio);
        Exceptional<Trinary> ObjectStreamExists(ObjectLocation myObjectLocation, String myObjectStream);
        Exceptional<Trinary> ObjectEditionExists(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition);
        Exceptional<Trinary> ObjectRevisionExists(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null);

        Exceptional<IEnumerable<String>> GetObjectStreams(ObjectLocation myObjectLocation);
        Exceptional<IEnumerable<String>> GetObjectEditions(ObjectLocation myObjectLocation, String myObjectStream);
        Exceptional<IEnumerable<ObjectRevisionID>> GetObjectRevisionIDs(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition);

        #endregion

        #region AFSObject specific methods

        Exceptional LockFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, ObjectLocks myObjectLock, ObjectLockTypes myObjectLockType, UInt64 myLockingTime);

        Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation) where PT : AFSObject, new();
        Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false) where PT : AFSObject, new();
        Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, Func<PT> myFunc, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false) where PT : AFSObject;

        Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation) where PT : AFSObject, new();
        Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures= false) where PT : AFSObject, new();
        Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, Func<PT> myFunc, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false) where PT : AFSObject;

        Exceptional StoreFSObject(AFSObject myAGraphObject, Boolean myAllowOverwritting, Boolean myPinObjectLocationInCache = false);

        Exceptional RenameFSObject(ObjectLocation myObjectLocation, String myNewObjectName);
        Exceptional RemoveFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null);
        Exceptional EraseFSObject (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null);

        /// <summary>
        /// Moves an objectlocation from A to B
        /// </summary>
        /// <param name="myFromLocation">The location that should be moved.</param>
        /// <param name="myToLocation">The place where it should be moved to.</param>
        Exceptional MoveObjectLocation(ObjectLocation myFromLocation, ObjectLocation myToLocation);

        #endregion


        #region Symlink Maintenance

        /// <summary>
        /// Adds a symlink to another object within the file system
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the symlink within the file system</param>        
        /// <param name="myTargetLocation">the location of this object (ObjectPath and ObjectName) of the symlink target within the file system</param>
        Exceptional AddSymlink(ObjectLocation myObjectLocation, ObjectLocation myTargetLocation);

        /// <summary>
        /// Adds a symlink to another object within the file system
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the symlink within the file system</param>        
        /// <param name="myTargetAFSObject">an object providing the the target ObjectLocation</param>
        Exceptional AddSymlink(ObjectLocation myObjectLocation, AFSObject myTargetAFSObject);

        /// <summary>
        /// Checks the existence of a symlink at the given file system location
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the symlink within the file system</param>        
        /// <returns>exists(true) or not exists(false)</returns>
        Exceptional<Trinary> isSymlink(ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns the target of a symlink
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the symlink within the file system</param>        
        /// <returns>a string representing the locaction of another file system object within the file system</returns>
        Exceptional<ObjectLocation> GetSymlink(ObjectLocation myObjectLocation);

        /// <summary>
        /// This method removes a symlink from the parent directory.
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        Exceptional RemoveSymlink(ObjectLocation myObjectLocation);

        #endregion

        #region Directory Maintenance

        /// <summary>
        /// Creates a directory in the given file system location
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the new directory within the file system
        /// Multiple strings will be concatenated to a valid ObjectLocation.</param>
        Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation, UInt64 myBlocksize = 0, Boolean myRecursive = false);

        /// <summary>
        /// Checks the existence of a directory in the given file system location
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested directory within the file system</param>        
        /// <returns>exists(true) or not exists(false)</returns>
        Exceptional<Trinary> isIDirectoryObject(ObjectLocation myObjectLocation);


        #region GetDirectoryListing(myObjectLocation, ...)

        /// <summary>
        /// Returns all directory entries at the given object location as a list of strings.
        /// </summary>
        /// <returns>List of strings containing a list of all directory entries</returns>
        Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation);


        Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation, Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc);

        /// <summary>
        /// Returns all directory entries at the given object location as a list of strings.
        /// Additionally filters may be applied to the output.
        /// </summary>
        /// <returns>List of strings containing a filtered list of all directory entries</returns>
        Exceptional<IEnumerable<String>> GetFilteredDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime);

        #endregion

        #region GetExtendedDirectoryListing(myObjectLocation, ...)

        /// <summary>
        /// Returns all directory entries at the given object location as a list of dictionaries.
        /// </summary>
        /// <returns>List of dictionaries containing a list of all directory entries</returns>
        Exceptional<IEnumerable<DirectoryEntryInformation>> GetExtendedDirectoryListing(ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns all directory entries at the given object location as a list of dictionaries.
        /// Additionally filters may be applied to the output.
        /// </summary>
        /// <returns>List of dictionaries containing a filtered list of all directory entries</returns>
        Exceptional<IEnumerable<DirectoryEntryInformation>> GetFilteredExtendedDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime);

        #endregion

        /// <summary>
        /// Removes a directory object
        /// </summary>
        /// <param name="myObjectLocation">The location of the directory object</param>
        /// <param name="removeRecursive">Remove recursive?</param>
        /// <returns></returns>
        Exceptional RemoveDirectoryObject(ObjectLocation myObjectLocation, Boolean removeRecursive);

        /// <summary>
        /// Erase a directory object
        /// </summary>
        /// <param name="myObjectLocation">The location of the directory object</param>
        /// <param name="eradeRecursive">Erase recursive?</param>
        /// <returns></returns>
        Exceptional EraseDirectoryObject(ObjectLocation myObjectLocation, Boolean eradeRecursive);

        /// <summary>
        /// Gets a IDirectoryObject
        /// </summary>
        /// <param name="objectLocation">The location of the IDirectoryObject</param>
        /// <returns>An IDirectoryObject</returns>
        Exceptional<IDirectoryObject> GetDirectoryObject(ObjectLocation objectLocation);

        #endregion

        #region Metadata Maintenance

        Exceptional SetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy);
        Exceptional SetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata, IndexSetStrategy myIndexSetStrategy);

        Exceptional<Trinary> MetadatumExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue);
        Exceptional<Trinary> MetadataExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey);

        Exceptional<IEnumerable<TValue>> GetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey);
        Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition);
        Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myMinKey, String myMaxKey);
        Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc);

        Exceptional RemoveMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue);
        Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey);
        Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata);
        Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc);

        #endregion

        #region UserMetadata Maintenance

        /// <summary>
        /// Sets a single user metadatum
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>
        /// <param name="myUserMetadataKey">the key to identify the object/metadatum to store</param>
        /// <param name="myObject">an object</param>
        /// <param name="myIndexSetStrategy"></param>
        /// <returns></returns>
        Exceptional SetUserMetadatum(ObjectLocation myObjectLocation, String myKey, Object myObject, IndexSetStrategy myIndexSetStrategy);

        /// <summary>
        /// Sets multiple user metadata pairs
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>
        /// <param name="myUserMetadata"></param>
        /// <param name="myIndexSetStrategy"></param>
        /// <returns></returns>
        Exceptional SetUserMetadata(ObjectLocation myObjectLocation, IEnumerable<KeyValuePair<String, Object>> myUserMetadata, IndexSetStrategy myIndexSetStrategy);


        /// <summary>
        /// Checks if the given combination of a user metadatakey and value exists
        /// </summary>
        /// <param name="myObjectLocation"></param>
        /// <param name="myUserMetadataKey"></param>
        /// <param name="myMetadatum"></param>
        /// <returns></returns>
        Exceptional<Trinary> UserMetadatumExists(ObjectLocation myObjectLocation, String myKey, Object myMetadatum);
        
        /// <summary>
        /// Checks if the given user metadatakey exists
        /// </summary>
        /// <param name="myObjectLocation"></param>
        /// <param name="myUserMetadataKey"></param>
        /// <returns></returns>
        Exceptional<Trinary> UserMetadataExists(ObjectLocation myObjectLocation, String myKey);



        Exceptional<IEnumerable<Object>> GetUserMetadatum(ObjectLocation myObjectLocation, String myKey);

        Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation);

        Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation, String myMinKey, String myMaxKey);

        Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation, Func<KeyValuePair<String, Object>, Boolean> myFunc);



        Exceptional RemoveUserMetadatum(ObjectLocation myObjectLocation, String myKey, Object myObject);

        Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, String myKey);

        Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, IEnumerable<KeyValuePair<String, Object>> myMetadata);

        Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, Func<KeyValuePair<String, Object>, Boolean> myFunc);

        #endregion


        #region File Maintenance

        Exceptional<FileObject> GetFileObject(ObjectLocation myObjectLocation);
        Exceptional<FileObject> GetFileObject(ObjectLocation myObjectLocation, ObjectRevisionID myRevisionID);

        Exceptional StoreFileObject(ObjectLocation myObjectLocation, Byte[] myData, Boolean myAllowToOverwrite = false);
        Exceptional StoreFileObject(ObjectLocation myObjectLocation, String myData, Boolean myAllowToOverwrite = false);

        #endregion


        #region AccessControlObject Maintenance

        ///// <summary>
        ///// This method adds a RIGTHSSTREAM to an Object.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myRightsObject">The AccessControlObject that should be added to the Object.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddRightsStreamToObject(ObjectLocation myObjectLocation, AccessControlObject myRightsObject);

        ///// <summary>
        ///// Adds an entity to the AllowACL of a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The right that should be granted.</param>
        ///// <param name="myEntitiyUUID">The guid of the entity.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddEntityToRightsStreamAllowACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID);

        ///// <summary>
        ///// Adds an entity to the DenyACL of a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The right that should be denied.</param>
        ///// <param name="myEntitiyUUID">The guid of the entity.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddEntityToRightsStreamDenyACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID);

        ///// <summary>
        ///// Removes an entity from a ACL that is related to a right within the allowACL.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The right which the entity should not be allowed for anymore.</param>
        ///// <param name="myEntitiyUUID">The guid of the entity.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveEntityFromRightsStreamAllowACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID);

        ///// <summary>
        ///// Removes an entity from a ACL that is related to a right within the denyACL.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The right which the entity should not be denied of anymore.</param>
        ///// <param name="myEntitiyUUID">The guid of the entity.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveEntityFromRightsStreamDenyACL(ObjectLocation myObjectLocation, RightUUID myRightUUID, EntityUUID myEntitiyUUID);

        ///// <summary>
        ///// Changes the AllowOverDeny property of a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myAllowOverDeny">The new value the AllowOverDeny property</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool ChangeAllowOverDenyOfRightsStream(ObjectLocation myObjectLocation, DefaultRuleTypes myDefaultRule);

        ///// <summary>
        ///// Adds an alert to a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myAlert">The alert that should be added.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddAlertToGraphRightsAlertHandlingList(ObjectLocation myObjectLocation, NHAccessControlObject myAlert);

        ///// <summary>
        ///// Removes an alert from a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myAlert">The alert that should be removed.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveAlertFromGraphRightsAlertHandlingList(ObjectLocation myObjectLocation, NHAccessControlObject myAlert);

        #endregion

        #region Evaluation of rights

        //List<Right> EvaluateRightsForEntity(ObjectLocation myObjectLocation, EntityUUID myEntityGuid, AccessControlObject myRightsObject);

        #endregion

        #region Index Maintenance

        ////void CreateIndexObject<T1, T2, T3>(ObjectLocation myObjectLocation) where T1 : AGraphObject, IIndexObject<T2, T3>, new() where T2 : IComparable;
        //IIndexObject<TKey, TValue> CreateIndexObject<TKey, TValue>(ObjectLocation myObjectLocation, IndexObjectTypes myIndexObjectType) where TKey : IComparable;

        ///// <summary>
        ///// Stores any object identified by the given metadata key within a user MetadataObject at the given file system location 
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <param name="myKey">the key to identify the object/metadatum to store</param>
        ///// <param name="myValue">an object</param>
        //void InsertIntoIndex<TKey, TValue>(ObjectLocation myObjectLocation, TKey myKey, List<TValue> myValue) where TKey : IComparable;

        ///// <summary>
        ///// Stores a list of <String, Object>-KeyValuePairs within a user MetadataObject at the given file system location 
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <param name="myIndexList">a list of KeyValuePairs to store</param>
        //void InsertIntoIndex<TKey, TValue>(ObjectLocation myObjectLocation, List<KeyValuePair<TKey, List<TValue>>> myIndexList) where TKey : IComparable;

        ///// <summary>
        ///// Stores a dictionary of type <String, Object> within a user MetadataObject at the given file system location 
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <param name="myIndexDictionary">a dictionary of type <String, Object> to store</param>
        //void InsertIntoIndex<TKey, TValue>(ObjectLocation myObjectLocation, Dictionary<TKey, List<TValue>> myIndexDictionary) where TKey : IComparable;


        ///// <summary>
        ///// Returns the requested user metadatum indexed by the object location and the metadata key.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadata object within the file system</param>        
        ///// <param name="myKey">the key or identifier of the requested metadatum</param>
        ///// <returns>the requested metadatum</returns>
        //List<TValue> GetValueFromIndex<TKey, TValue>(ObjectLocation myObjectLocation, TKey myUserMetadataKey) where TKey : IComparable;

        ///// <summary>
        ///// Returns a list of key-value-pairs of all user metadata indexed by the object location.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <returns>the requested list of all metadata</returns>
        //List<KeyValuePair<TKey, TValue>> GetIndexAsList<TKey, TValue>(ObjectLocation myObjectLocation) where TKey : IComparable;

        ///// <summary>
        ///// Returns a dictionary of all user metadata indexed by the object location.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested metadatum within the file system</param>        
        ///// <returns>the requested dictionary of all metadata</returns>
        //Dictionary<TKey, List<TValue>> GetIndexAsDictionary<TKey, TValue>(ObjectLocation myObjectLocation) where TKey : IComparable;


        ///// <summary>
        ///// Deletes the entire user metadata object at the given object location.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the metadatum within the file system</param>        
        //void DeleteIndexObject(ObjectLocation myObjectLocation);

        ///// <summary>
        ///// Deletes a user metadatum at the given object location indexed by the metadata key.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the metadatum within the file system</param>        
        ///// <param name="myUserMetadataKey">the key or identifier of the metadatum</param>
        //void DeleteKeyFromIndex<TKey, TValue>(ObjectLocation myObjectLocation, TKey myKey) where TKey : IComparable;


        ///// <summary>
        ///// Checks if a specific user metadata object exists at the given object location.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the metadatum within the file system</param>        
        ///// <returns>exists(true) or not exists(false)</returns>
        //Boolean IndexObjectExists(ObjectLocation myObjectLocation, String nameOfKeyClass);

        ///// <summary>
        ///// Checks if a specific user metadata key indexed by the given object location exists.
        ///// </summary>
        ///// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the metadatum within the file system</param>        
        ///// <param name="myUserMetadataKey">the key or identifier of the metadatum</param>
        ///// <returns>exists(true) or not exists(false)</returns>
        //Boolean IndexHasKey<TKey, TValue>(ObjectLocation myObjectLocation, TKey myKey) where TKey : IComparable;

        #endregion

        #region StringListObject Maintenance

        //void CreateListOfStrings(ObjectLocation myObjectLocation);
        //List<String> GetListOfStrings(ObjectLocation myObjectLocation);
        //void AddToListOfStrings(ObjectLocation myObjectLocation, String myNewString);
        //void RemoveFromListOfStrings(ObjectLocation myObjectLocation, String myToBeRemovedString);
        //Boolean ListOfStringsExist(ObjectLocation myObjectLocation);

        #endregion

        #region EntitiesObject Maintenance

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
        //EntityUUID AddEntity(ObjectLocation myObjectLocation, String myLogin, String myRealname, String myDescription, Dictionary<ContactTypes, List<String>> myContacts, String myPassword, List<PublicKey> myPublicKeyList, HashSet<EntityUUID> myMembership);

        ///// <summary>
        ///// This method checks if a given entity, which is referenced by its guid, is 
        ///// present in the current FS.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="_RightUUID">The guid of the entity.</param>
        ///// <returns>Returns true on a valid guid. False else.</returns>
        //Trinary EntityExists(ObjectLocation myObjectLocation, EntityUUID myEntityUUID);

        ///// <summary>
        ///// This method returns the the guid for a given entity Name.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="aName">The Name of an entity.</param>
        ///// <returns>The guid of the entity or null if it does not exist</returns>
        //EntityUUID GetEntityUUID(ObjectLocation myObjectLocation, String aName);

        ///// <summary>
        ///// This method removes a given Guid (references an Entity) from the EntitiesObject.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="_RightUUID">The Guid of the Entity that should be removed.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveEntity(ObjectLocation myObjectLocation, EntityUUID myEntityUUID);

        ///// <summary>
        ///// This mehtod returns a list of memberships concerning a given Guid (references an Entity).
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="_RightUUID">The Guid of the Entity.</param>
        ///// <returns>A list of Guids if the given Guid exists or otherwise null.</returns>
        //HashSet<EntityUUID> GetMemberships(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, Boolean myRecursion);

        ///// <summary>
        ///// This method changes the password hash to a given Guid, which references an Entity.
        ///// If the old password hash parameter matches the entities password hash, it is replaced by the new one.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The Guid of the Entity, whose password hash should be changed.</param>
        ///// <param name="myOldPassword">The old password hash.</param>
        ///// <param name="myNewPassword">The new password hash.</param>
        ///// <returns>True for success, or otherwise false.</returns>
        //void ChangeEntityPassword(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, String myOldPassword, String myNewPassword);

        ///// <summary>
        ///// This method changes the public key of an Entity, which is referenced by its Guid.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The UUID of the entity.</param>
        ///// <param name="myPassword">the entity password</param>
        ///// <param name="myNewPublicKeyList">The new list of public keys</param>
        //void ChangeEntityPublicKeyList(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, String myPassword, List<PublicKey> myNewPublicKeyList);

        ///// <summary>
        ///// This method adds a new membership for an Entity which is referenced by the Guid parameter.
        ///// This operation can only succeed, if the Guid of the new membership really exists.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The UUID of the Entity.</param>
        ///// <param name="myNewMembershipUUID">The UUID of the new membership.</param>
        //void ChangeEntityAddMembership(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, EntityUUID myNewMembershipUUID);

        ///// <summary>
        ///// This method removes a membership for an Entity which is referenced by the Guid parameter.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The Guid of the Entity.</param>
        ///// <param name="myNewMembershipUUID">The UUID of the new membership.</param>
        //void ChangeEntityRemoveMembership(ObjectLocation myObjectLocation, EntityUUID myEntityUUID, EntityUUID myNewMembershipUUID);

        ///// <summary>
        ///// This method returns the public key concerning a Entity, which is referenced by its Guid.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myEntityUUID">The Guid that references the Entity.</param>
        ///// <returns>A list of public keys</returns>
        //List<PublicKey> GetEntityPublicKeyList(ObjectLocation myObjectLocation, EntityUUID myEntityUUID);

        #endregion

        #region Rights ObjectIndex Maintenance

        ///// <summary>
        ///// This method adds a Right to the IGraphFS. All 
        ///// Rights that are added via this method are treated as 
        ///// userdefined rights. Additionally it is possible to add 
        ///// a validation script which is evaluated while trying to 
        ///// use an object with this constraint.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myLogin">The Name of the right. Cannot be null or empty.</param>
        ///// <param name="myValidationScript">The validation script for evaluating the access of an entity. Can be null or empty.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool AddGraphRight(ObjectLocation myObjectLocation, String Name, String ValidationScript);

        ///// <summary>
        ///// This method removes a Right from the IGraphFS. It is 
        ///// not possible to remove a non userdefined right.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The UUID of the right.</param>
        ///// <returns>True for success or otherwise false</returns>
        //bool RemoveGraphRight(ObjectLocation myObjectLocation, RightUUID myRightUUID);

        ///// <summary>
        ///// Returns a Right correspondig to its Name.
        ///// </summary>
        ///// <param name="RightName">The Name of the Right.</param>
        ///// <returns>The Right.</returns>
        //Right GetRightByName(String RightName);

        //Boolean ContainsRightUUID(RightUUID myRightUUID);

        #endregion


        #region Transactions

        FSTransaction BeginFSTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null);

        #endregion


        Exceptional<IGraphFSStream> OpenStream(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy);

        Exceptional<IGraphFSStream> OpenStream(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy,
                                    FileMode myFileMode, FileAccess myFileAccess, FileShare myFileShare, FileOptions myFileOptions, UInt64 myBufferSize);


        UInt64 NumberOfSpecialDirectories { get; }
    }
}
