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

/* <id name="PandoraDB – abstract class for all single reference edges" />
 * <copyright file="ASingleEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This abstract class should be implemented for all single reference edges. It will store just an ObjectUUID. The complementary of this for not reference types are all ADBBaseObject implementations.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This abstract class should be implemented for all single reference edges. It will store just an ObjectUUID. The complementary of this for not reference types are all ADBBaseObject implementations.
    /// </summary>

    public abstract class ASingleReferenceEdgeType : AEdgeType, IReferenceEdge
    {
        /// <summary>
        /// The ObjectUUID of the value
        /// </summary>
        /// <returns></returns>
        public abstract ObjectUUID GetUUID();

        /// <summary>
        /// Set the value with some optional parameters
        /// </summary>
        /// <param name="myValue">A ObjectUUID</param>
        /// <param name="myParameters">Some optional parameters</param>
        public abstract void Set(ObjectUUID myValue, params ADBBaseObject[] myParameters);

        /// <summary>
        /// Merge the current value with the value of mySingleEdgeType. In detail, overwrites the ObjectUUID and make some magic with the edge informations
        /// </summary>
        /// <param name="aSingleEdgeType"></param>
        public abstract void Merge(ASingleReferenceEdgeType mySingleEdgeType);

        /// <summary>
        /// Create the readout for the ObjectUUID.
        /// </summary>
        /// <param name="GetAllAttributesFromDBO">A delegate which will retriev the standard DBObjectReadout for a ObjectUUID</param>
        /// <returns></returns>
        public abstract DBObjectReadout GetReadout(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO);

        #region IReferenceEdge Members

        /// <summary>
        /// The ObjectUUID of the value
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<ObjectUUID> GetAllUUIDs();

        /// <summary>
        /// Get all uuids and their edge infos
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetEdges();

        /// <summary>
        /// removes a specific reference
        /// </summary>
        /// <param name="myObjectUUID">the object uuid of the object, that should remove</param>
        public abstract Boolean RemoveUUID(ObjectUUID myObjectUUID);

        /// <summary>
        /// remove some specifics references
        /// </summary>
        /// <param name="myObjectUUIDs">the object uuid's of the objects, that should remove</param>
        public abstract Boolean RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs);


        #endregion
    }
}
