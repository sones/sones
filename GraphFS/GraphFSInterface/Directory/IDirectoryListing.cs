/*
 * IDictionaryListing
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;


using sones.Lib.DataStructures;

using sones.StorageEngines;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.InternalObjects;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface for all read-only DirectoryObjects
    /// </summary>

    public interface IDirectoryListing : IObjectLocation
    {

        #region Object Stream Maintenance

        /// <summary>
        /// Checks if an object exists within the object hashmap or binary search tree
        /// </summary>
        /// <param name="myObjectName">the Name of the requested object</param>
        /// <returns>true if found, false if not found</returns>
        Trinary ObjectExists(String myObjectName);

        /// <summary>
        /// Checks if an object and the given ObjectStream exists within the filesystem hierarchy
        /// </summary>
        /// <param name="myObjectName">the Name of the object in question</param>
        /// <param name="myObjectStream">the ObjectStream in question</param>
        /// <returns>true if found, false if not found</returns>
        Trinary ObjectStreamExists(String myObjectName, String myObjectStream);

        /// <summary>
        /// Returns a bitfield of available object streams
        /// </summary>
        /// <param name="myObjectName">the Name of the object in question</param>
        /// <returns>a list of available object streams</returns>
        IEnumerable<String> GetObjectStreamsList(String myObjectName);

        /// <summary>
        /// Returns the positions of the requested ObjectStream
        /// </summary>
        /// <param name="myObjectName">the Name of the object</param>
        /// <returns>a list of INode positions or an empty list if the object does not exist</returns>
        IEnumerable<ExtendedPosition> GetObjectINodePositions(String myObjectName);

        /// <summary>
        /// Returns an directory entry
        /// </summary>
        /// <param name="myObjectName">The Name of the entry</param>
        /// <returns>The DirectoryEntry or null if the entry could not be found</returns>
        DirectoryEntry GetDirectoryEntry(String myObjectName);

        #endregion

        #region InlineData Maintenance

        /// <summary>
        /// Returns the inline data stored within the directory object
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink</param>
        /// <returns>the array of bytes stored inline within the directory object</returns>
        Byte[] GetInlineData(String myObjectName);

        /// <summary>
        /// Returns if myObjectName has inline data
        /// </summary>
        /// <param name="myObjectName">the Name of the inline data object to probe</param>
        /// <returns>yes|no</returns>
        Trinary hasInlineData(String myObjectName);

        #endregion

        #region Symlink Maintenance

        /// <summary>
        /// Returns the target of a symlink
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink</param>
        /// <returns>a string representing the myPath to another object within the filesystem</returns>
        ObjectLocation GetSymlink(String myObjectName);

        /// <summary>
        /// Returns if myObjectName is a symlink
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink to probe</param>
        /// <returns>yes|no</returns>
        Trinary isSymlink(String myObjectName);

        #endregion

        #region GetDirectoryListings(...)

        /// <summary>
        /// Returns all directory entries as a list of strings.
        /// </summary>
        /// <returns>List of strings containing a list of all directory objects</returns>
        IEnumerable<String> GetDirectoryListing();

        IEnumerable<String> GetDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc);

        /// <summary>
        /// Returns all directory entries at the given object location as a list of strings.
        /// Additionally filters may be applied to the output.
        /// </summary>
        /// <returns>List of strings containing a filtered list of all directory entries</returns>
        IEnumerable<String> GetDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams);


        /// <summary>
        /// Returns all directory entries at the given object location as a list of dictionaries.
        /// </summary>
        /// <returns>List of dictionaries containing a list of all directory entries</returns>
        IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing();

        IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc);

        /// <summary>
        /// Returns all directory entries at the given object location as a list of dictionaries.
        /// Additionally filters may be applied to the output.
        /// </summary>
        /// <returns>List of dictionaries containing a filtered list of all directory entries</returns>
        IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams);

        #endregion

        #region Directory Counter Methods

        /// <summary>
        /// Returns the number of object within the directory.
        /// </summary>
        /// <returns></returns>
        UInt64 DirCount { get; }

        #endregion

        #region NotificationHandling

        /// <summary>
        /// Returns the NotificationHandling bitfield that indicates which
        /// notifications should be triggered.
        /// </summary>
        NHIDirectoryObject NotificationHandling { get; }

        /// <summary>
        /// This method adds the given NotificationHandling flags.
        /// </summary>
        /// <param name="myNotificationHandling">The NotificationHandlings to be added.</param>
        void SubscribeNotification(NHIDirectoryObject myNotificationHandling);

        /// <summary>
        /// This method removes the given NotificationHandling flags.
        /// </summary>
        /// <param name="myNotificationHandling">The NotificationHandlings to be removed.</param>
        void UnsubscribeNotification(NHIDirectoryObject myNotificationHandling);

        #endregion

    }

}