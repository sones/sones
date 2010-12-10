/* GraphFS - IDictionaryObject
 * (c) Achim Friedland, 2008 - 2009
 * 
 * The interface for all Graph directory objects and virtual
 * directory objects.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;


using sones.Lib.DataStructures;

using sones.StorageEngines;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.Lib.DataStructures.WeakReference;
using sones.GraphFS.InternalObjects;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface for all Graph directory objects and virtual
    /// directory objects.
    /// </summary>

    public interface IDirectoryObject : IDirectoryListing //: IQueryable<DirectoryEntry>
    {

        #region Members of AGraphHeader

        Boolean       isNew                   { get; set; }
        ObjectUUID    ObjectUUID              { get; }

        #endregion


        #region Members of IFastSerialize

        Boolean       isDirty                 { get; set; }

        #endregion


        WeakReference<IGraphFS> IGraphFSReference { get; set; }

        /// <summary>
        /// Adds a complete directory entry to the directory object
        /// </summary>
        /// <param name="entryName">The name of the directory entry</param>
        /// <param name="aDirectoryEntry">The directory entry itself</param>
        void AddDirectoryEntry(String entryName, DirectoryEntry aDirectoryEntry);

        /// <summary>
        /// Renames an IDirectoryObject
        /// </summary>
        /// <param name="myObjectName">The old name of the DirectoryEntry</param>
        /// <param name="myNewObjectName">The new name of the DirectoryEntry</param>
        /// <returns></returns>
        Boolean RenameDirectoryEntry(String myObjectName, String myNewObjectName);

        /// <summary>
        /// Removes an ObjectLocation
        /// </summary>
        /// <param name="myObjectName">The name of the DirectoryEntry that is going to be deleted</param>
        /// <param name="myForce"></param>
        void RemoveObjectLocation(String myObjectName, Boolean myForce = false);

        /// <summary>
        /// Wipe the directory object
        /// </summary>
        /// <returns></returns>
        Exceptional Wipe();

        #region Members of IDictionaryObject

        #region Object Stream Maintenance

        /// <summary>
        /// This method adds a new ObjectStream to an object in the _DirectoryTree
        /// </summary>
        /// <param name="myObjectName">the Name of the object</param>
        /// <param name="myObjectStream">the ObjectStream of the object</param>
        /// <param name="myINodePositions">the filesystem positions of the corresponding streams</param>
        void AddObjectStream(String myObjectName, String myObjectStream);

        /// <summary>
        /// This method adds a new ObjectStream to an object in the _DirectoryTree
        /// </summary>
        /// <param name="myObjectName">the Name of the object</param>
        /// <param name="myObjectStream">the ObjectStream of the object</param>
        /// <param name="myINodePositions">the filesystem positions of the corresponding streams</param>
        void AddObjectStream(String myObjectName, String myObjectStream, IEnumerable<ExtendedPosition> myINodePositions);

        /// <summary>
        /// This method deletes an ObjectStream from an object in the _DirectoryTree.
        /// It will also remove the entire object if no ObjectStream is left.
        /// </summary>
        /// <param name="myObjectName">the Name of the object</param>
        /// <param name="myObjectStream">the ObjectStream of the object</param>
        void RemoveObjectStream(String myObjectName, String myObjectStream);

        #endregion

        #region InlineData Maintenance

        /// <summary>
        /// Adds inline data which will be stored within the directory object
        /// </summary>
        /// <param name="myObjectName">the Name of the inline data object</param>
        /// <param name="myInlineData">the online data as array of bytes</param>
        /// <param name="myAllowOverwritting">allows overwritting</param>
        void StoreInlineData(String myObjectName, Byte[] myInlineData, Boolean myAllowOverwritting);

        /// <summary>
        /// Removes inline data stored within the directory object
        /// </summary>
        /// <param name="myObjectName">the Name of the inline data</param>
        void DeleteInlineData(String myObjectName);

        #endregion

        #region Symlink Maintenance

        /// <summary>
        /// Adds a symlink to another object within the filesystem
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink</param>
        /// <param name="myTargetLocation">the myPath to another object within the filesystem</param>
        void AddSymlink(String myObjectName, ObjectLocation myTargetLocation);

        /// <summary>
        /// Removes a symlink
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink</param>
        void RemoveSymlink(String myObjectName);

        #endregion

        #region Clone()

        //AGraphObject Clone();

        #endregion

        #endregion

    }

}