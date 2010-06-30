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


/* PandoraFS - IDictionaryObject
 * Achim Friedland, 2008 - 2009
 * 
 * The interface for all pandora directory objects and virtual
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

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface for all pandora directory objects and virtual
    /// directory objects.
    /// </summary>

    public interface IDirectoryObject : IDirectoryListing //: IQueryable<DirectoryEntry>
    {

        #region Members of APandoraHeader

        Boolean       isNew                   { get; set; }
        ObjectUUID    ObjectUUID              { get; }

        #endregion


        #region Members of IFastSerialize

        Boolean       isDirty                 { get; set; }

        #endregion


        WeakReference<IGraphFS> IGraphFSReference { get; set; }


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

        //APandoraObject Clone();

        #endregion

        #endregion


    }

}