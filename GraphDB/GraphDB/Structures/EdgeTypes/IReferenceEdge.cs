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


#region usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Structures.EdgeTypes
{
    public interface IReferenceEdge
    {
        
        /// <summary>
        /// Get all added objectUUIDs
        /// </summary>
        /// <returns></returns>
        IEnumerable<ObjectUUID> GetAllReferenceIDs();

        /// <summary>
        /// Get all uuids and their edge infos
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetAllReferenceIDsWeighted();

        /// <summary>
        /// Get all destinations of an edge
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache);

        /// <summary>
        /// Get all weighted destinations of an edge
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<Exceptional<DBObjectStream>, ADBBaseObject>> GetAllEdgeDestinationsWeighted(DBObjectCache dbObjectCache);

        /// <summary>
        /// removes a specific reference
        /// </summary>
        /// <param name="myObjectUUID">the object uuid of the object, that should remove</param>
        /// <returns></returns>
        Boolean RemoveUUID(ObjectUUID myObjectUUID);

        /// <summary>
        /// remove some specifics references
        /// </summary>
        /// <param name="myObjectUUIDs">the object uuid's of the objects, that should remove</param>
        /// <returns></returns>
        Boolean RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs);

        /// <summary>
        /// Returns the typeUUID of the objects within the reference edge
        /// </summary>
        /// <returns></returns>
        TypeUUID GetTypeUUIDOfReferences();
    }
}
