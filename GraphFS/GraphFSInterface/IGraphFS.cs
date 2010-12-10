/*
 * IGraphFS
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.IO;
using System.Collections.Generic;

using sones.GraphFS;
using sones.GraphFS.Events;
using sones.GraphFS.Caches;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.GraphFS.Transactions;
using sones.GraphFS.DataStructures;
using sones.GraphFS.InternalObjects;

using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.Indices;
using sones.Lib.Settings;

#endregion

namespace sones
{

    #region IGraphFSVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphFS plugin versions. 
    /// Defines the min and max version for all IGraphFS implementations which will be activated used this IGraphFS.
    /// </summary>
    internal static class IGraphFSVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("1.1.1.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("1.1.1.0");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all Graph file systems.
    /// </summary>

    public interface IGraphFS
    {


        #region Properties

        FileSystemUUID      FileSystemUUID          { get; }
        String              FileSystemDescription   { get; set; }
        ObjectCacheSettings ObjectCacheSettings     { get; set; }

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

        // Transactions
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


        #region Make-/Grow-/Shrink-/WipeFileSystem

        /// <summary>
        /// This initialises a GraphFS in a given device or file using the given sizes
        /// </summary>
        /// <param name="myDescription">a distinguishable Name or description for the file system (can be changed later)</param>
        /// <param name="myNumberOfBytes">the size of the file system in byte</param>
        /// <param name="myOverwriteExistingFileSystem">overwrite an existing file system [yes|no]</param>
        /// <returns>the UUID of the new file system</returns>
        Exceptional<FileSystemUUID> MakeFileSystem(SessionToken mySessionToken, String myDescription, UInt64 myNumberOfBytes, Boolean myOverwriteExistingFileSystem, Action<Double> myAction);

        /// <summary>
        /// This enlarges the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        Exceptional<UInt64> GrowFileSystem(SessionToken mySessionToken, UInt64 myNumberOfBytesToAdd);

        /// <summary>
        /// This reduces the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToRemove">the number of bytes to remove from the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        Exceptional<UInt64> ShrinkFileSystem(SessionToken mySessionToken, UInt64 myNumberOfBytesToRemove);

        /// <summary>
        /// Wipe the file system
        /// </summary>
        Exceptional WipeFileSystem(SessionToken mySessionToken);

        #endregion

        #region Mount-/Remount-/UnmountFileSystem

        /// <summary>
        /// Mounts this file system.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myAccessMode">The file system access mode, e.g. "read-write" or "read-only".</param>
        Exceptional MountFileSystem(SessionToken mySessionToken, AccessModeTypes myAccessMode);

        /// <summary>
        /// Mounts the given IGraphFS instance at the given mount point using the given access mode.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myMountPoint">An ObjectLocation used as mount point.</param>
        /// <param name="myIGraphFS">An file system instance implementing IGraphFS.</param>
        /// <param name="myAccessMode">The file system access mode, e.g. "read-write" or "read-only".</param>
        Exceptional MountFileSystem(SessionToken mySessionToken, ObjectLocation myMountPoint, IGraphFS myIGraphFS, AccessModeTypes myAccessMode);


        /// <summary>
        /// Remounts a file system in order to change its access mode.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myAccessMode">The file system access mode, e.g. "read-write" or "read-only".</param>
        Exceptional RemountFileSystem(SessionToken mySessionToken, AccessModeTypes myFSAccessMode);

        /// <summary>
        /// Remounts the file system at the given ObjectLocation in order to change its access mode.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myMountPoint">The mount point as ObjectLocation of the file system to remount.</param>
        /// <param name="myAccessMode">The file system access mode, e.g. "read-write" or "read-only".</param>
        Exceptional RemountFileSystem(SessionToken mySessionToken, ObjectLocation myMountPoint, AccessModeTypes myAccessMode);


        /// <summary>
        /// Flush all caches and unmount this file system.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        Exceptional UnmountFileSystem(SessionToken mySessionToken);

        /// <summary>
        /// Unmounts the file system at the given ObjectLocation.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myMountPoint">The mount point as ObjectLocation of the file system to unmount.</param>
        Exceptional UnmountFileSystem(SessionToken mySessionToken, ObjectLocation myMountPoint);

        /// <summary>
        /// Unmounts all file systems mounted via this file system.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        Exceptional UnmountAllFileSystems(SessionToken mySessionToken);


        /// <summary>
        /// Restricts the access to this file system to the given "/ChangeRootPrefix".
        /// This might be of interesst for security and safety purposes.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myChangeRootPrefix">The new file system root as ObjectLocation.</param>
        Exceptional ChangeRootDirectory(SessionToken mySessionToken, String myChangeRootPrefix);

        #endregion


        #region INode and ObjectLocator

        /// <summary>
        /// Returns the INode of the given ObjectLocation.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectLocation">The location of this object (ObjectPath and ObjectName) of the requested INode within the file system</param>
        Exceptional<INode> GetINode(SessionToken mySessionToken, ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns the ObjectLocator of the given ObjectLocation.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectLocation">The location of this object (ObjectPath and ObjectName) of the requested ObjectLocator within the file system</param>
        Exceptional<ObjectLocator> GetObjectLocator(SessionToken mySessionToken, ObjectLocation myObjectLocation);

        /// <summary>
        /// Stores an ObjectLocator
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectLocation">The location of this object (ObjectPath and ObjectName) of the requested ObjectLocator within the file system</param>
        Exceptional StoreObjectLocator(SessionToken mySessionToken, ObjectLocator myObjectLocator);

        #endregion

        #region Object/ObjectStream/ObjectEdition/ObjectRevision infos

        /// <summary>
        /// Checks if there is any object located at the given ObjectLocation.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectLocation">The location (ObjectPath and ObjectName) of the requested object within the file system.</param>
        Exceptional<Trinary> ObjectExists (SessionToken mySessionToken, ObjectLocation myObjectLocation);

        /// <summary>
        /// Checks if there the given ObjectStream located at the given ObjectLocation exists.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectStream">The ObjectStream.</param>
        /// <param name="myObjectLocation">The location (ObjectPath and ObjectName) of the requested object within the file system.</param>
        Exceptional<Trinary> ObjectStreamExists(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream);

        /// <summary>
        /// Checks if there the given ObjectEdition and ObjectStream located at the given ObjectLocation exists.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectStream">The ObjectStream.</param>
        /// <param name="myObjectEdition">The ObjectEdition.</param>
        /// <param name="myObjectLocation">The location (ObjectPath and ObjectName) of the requested object within the file system.</param>
        Exceptional<Trinary> ObjectEditionExists(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition);

        /// <summary>
        /// Checks if there the given ObjectRevision, ObjectEdition and ObjectStream located at the given ObjectLocation exists.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectStream">The ObjectStream.</param>
        /// <param name="myObjectEdition">The ObjectEdition.</param>
        /// <param name="myObjectRevisionID">The ObjectRevisionID.</param>
        /// <param name="myObjectLocation">The location (ObjectPath and ObjectName) of the requested object within the file system.</param>
        Exceptional<Trinary> ObjectRevisionExists(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID);


        /// <summary>
        /// Returns all ObjectStreams at the given ObjectLocation.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectLocation">The location (ObjectPath and ObjectName) of the requested ObjectStreams within the file system.</param>
        Exceptional<IEnumerable<String>> GetObjectStreams(SessionToken mySessionToken, ObjectLocation myObjectLocation);

        /// <summary>
        /// Returns all ObjectEditions at the given ObjectLocation having the requested ObjectStream.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectLocation">The location (ObjectPath and ObjectName) of the requested ObjectEditions within the file system.</param>
        /// <param name="myObjectStream">The ObjectStream.</param>
        Exceptional<IEnumerable<String>> GetObjectEditions(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream);

        /// <summary>
        /// Returns all ObjectEditions at the given ObjectLocation having the requested ObjectStream and ObjectEdition.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myObjectLocation">The location (ObjectPath and ObjectName) of the requested ObjectRevisions within the file system.</param>
        /// <param name="myObjectStream">The ObjectStream.</param>
        /// <param name="myObjectEdition">The ObjectEdition.</param>
        Exceptional<IEnumerable<ObjectRevisionID>> GetObjectRevisionIDs(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition);

        #endregion

        #region AFSObject specific methods

        Exceptional<Boolean> LockAFSObject(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, ObjectLocks myObjectLock, ObjectLockTypes myObjectLockType, UInt64 myLockingTime);

        Exceptional<PT> GetOrCreateAFSObject<PT> (SessionToken mySessionToken,                  ObjectLocation myObjectLocation, String myObjectStream = null, String myObjectEdition = null, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false) where PT : AFSObject, new();
        Exceptional<PT> GetOrCreateAFSObject<PT> (SessionToken mySessionToken, Func<PT> myFunc, ObjectLocation myObjectLocation, String myObjectStream = null, String myObjectEdition = null, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false) where PT : AFSObject;
        Exceptional<PT> GetAFSObject<PT>         (SessionToken mySessionToken,                  ObjectLocation myObjectLocation, String myObjectStream = null, String myObjectEdition = null, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false) where PT : AFSObject, new();
        Exceptional<PT> GetAFSObject<PT>         (SessionToken mySessionToken, Func<PT> myFunc, ObjectLocation myObjectLocation, String myObjectStream = null, String myObjectEdition = null, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false) where PT : AFSObject;

        Exceptional StoreAFSObject   (SessionToken mySessionToken, ObjectLocation myObjectLocation, AFSObject myAGraphObject, Boolean myAllowToOverwrite = false);

        Exceptional RenameAFSObjects (SessionToken mySessionToken, ObjectLocation myObjectLocation, String myNewObjectName);
        Exceptional RemoveAFSObject  (SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID);
        Exceptional EraseAFSObject   (SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID);

        /// <summary>
        /// Moves an objectlocation from A to B
        /// </summary>
        /// <param name="myFromLocation">The location that should be moved.</param>
        /// <param name="myToLocation">The place where it should be moved to.</param>
        /// <param name="mySessionToken">The current session token.</param>
        Exceptional MoveObjectLocation(ObjectLocation myFromLocation, ObjectLocation myToLocation, SessionToken mySessionToken);

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
        /// Creates a directory in the given file system location, it will recursively all not existing subdirectories
        /// </summary>
        Exceptional<IDirectoryObject> CreateDirectoryObject(SessionToken mySessionToken, ObjectLocation myObjectLocation, UInt64 myBlocksize = 0, Boolean myRecursive = false);


        /// <summary>
        /// Checks the existence of a directory in the given file system location
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested directory within the file system</param>        
        /// <returns>exists(true) or not exists(false)</returns>        
        Exceptional<Trinary> IsIDirectoryObject(ObjectLocation myObjectLocation, SessionToken mySessionToken);


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
        Exceptional RemoveDirectoryObject(ObjectLocation myObjectLocation, Boolean myRemoveRecursive, SessionToken mySessionToken);

        /// <summary>
        /// Erases a directory in the given file system location
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the directory within the file system</param>
        /// <param name="myRemoveRecursive">erase the directory recursive</param>        
        Exceptional EraseDirectoryObject(ObjectLocation myObjectLocation, Boolean myRemoveRecursive, SessionToken mySessionToken);

        /// <summary>
        /// Gets an IDirectoryObject
        /// </summary>
        /// <param name="SessionToken">The current session token</param>
        /// <param name="myObjectLocation">The location of the IDirectoryObject</param>
        /// <returns>An IDirectoryObject</returns>
        Exceptional<IDirectoryObject> GetDirectoryObject(SessionToken SessionToken, ObjectLocation myObjectLocation);

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


        #region AccessControlObject Maintenance (unmaintained!)

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
        //bool AddAlertToGraphRightsAlertHandlingList(ObjectLocation myObjectLocation, NHAccessControlObject myAlert, SessionToken mySessionToken);

        ///// <summary>
        ///// Removes an alert from a ACCESSCONTROLSTREAM.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="myAlert">The alert that should be removed.</param>
        ///// <returns>True for success or otherwise false.</returns>
        //bool RemoveAlertFromGraphRightsAlertHandlingList(ObjectLocation myObjectLocation, NHAccessControlObject myAlert, SessionToken mySessionToken);

        //#endregion

        //#region Evaluation of rights

        //List<Right> EvaluateRightsForEntity(ObjectLocation myObjectLocation, EntityUUID myEntityGuid, AccessControlObject myRightsObject, SessionToken mySessionToken);

        #endregion

        #region EntitiesObject Maintenance (unmaintained!)

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

        #endregion

        #region RightsObject Maintenance (unmaintained!)

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
        //bool AddGraphRight(ObjectLocation myObjectLocation, String Name, String ValidationScript, SessionToken mySessionToken);

        ///// <summary>
        ///// This method removes a Right from the GraphFS. It is 
        ///// not possible to remove a non userdefined right.
        ///// </summary>
        ///// <param name="myObjectLocation">The object location.</param>
        ///// <param name="RightUUID">The UUID of the right.</param>
        ///// <returns>True for success or otherwise false</returns>
        //bool RemoveGraphRight(ObjectLocation myObjectLocation, RightUUID myRightUUID, SessionToken mySessionToken);

        ///// <summary>
        ///// Returns a Right correspondig to its Name.
        ///// </summary>
        ///// <param name="RightName">The Name of the Right.</param>
        ///// <returns>The Right.</returns>
        //Right GetRightByName(String RightName, SessionToken mySessionToken);

        //Boolean ContainsRightUUID(RightUUID myRightUUID, SessionToken mySessionToken);

        #endregion


        #region Transactions

        FSTransaction BeginTransaction(SessionToken mySessionToken, Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null);

        #endregion

        #region Stream

        Exceptional<IGraphFSStream> OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy);

        Exceptional<IGraphFSStream> OpenStream(SessionToken mySessionToken, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy,
                                    FileMode myFileMode, FileAccess myFileAccess, FileShare myFileShare, FileOptions myFileOptions, UInt64 myBufferSize);

        #endregion

        #region ObjectCache

        /// <summary>
        /// Returns the ObjectCache settings of this file system
        /// </summary>
        /// <returns>The ObjectCache settings of this file system</returns>
        Exceptional<ObjectCacheSettings> GetObjectCacheSettings(SessionToken mySessionToken);

        /// <summary>
        /// Returns the ObjectCache settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <returns>The ObjectCache settings of the file system at the given ObjectLocation</returns>
        Exceptional<ObjectCacheSettings> GetObjectCacheSettings(ObjectLocation myObjectLocation, SessionToken mySessionToken);


        /// <summary>
        /// Sets the ObjectCache settings of this file system
        /// </summary>
        /// <param name="myNotificationSettings">A ObjectCacheSettings object</param>
        Exceptional SetObjectCacheSettings(ObjectCacheSettings myObjectCacheSettings, SessionToken mySessionToken);

        /// <summary>
        /// Sets the ObjectCache settings of the file system at the given ObjectLocation
        /// </summary>
        /// <param name="myObjectLocation">the ObjectLocation or path of interest</param>
        /// <param name="myNotificationSettings">A ObjectCacheSettings object</param>
        Exceptional SetObjectCacheSettings(ObjectLocation myObjectLocation, ObjectCacheSettings myObjectCacheSettings, SessionToken mySessionToken);

        #endregion
    }

}
