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

/* <id name="sones GraphDB – abstract class for all reference list edges" />
 * <copyright file="AListReferenceEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This abstract class should be implemented for all reference list edges. It provides the base methods which are needed from the Database to retrieve all reference related values like ObjectUUID etc.</summary>
 */

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This abstract class should be implemented for all reference list edges. It provides the base methods which are needed from the Database to retrieve all reference related values like ObjectUUID etc.
    /// </summary>

    public abstract class ASetReferenceEdgeType : AListEdgeType, IReferenceEdge
    {
        

        /// <summary>
        /// Get a list of readouts. Additional to the standard readout (generated from <paramref name="GetAllAttributesFromDBO"/>)
        /// some special information will be added (like Weight etc)
        /// </summary>
        /// <param name="GetAllAttributesFromDBO">A delegate which will retriev the standard DBObjectReadout for a ObjectUUID</param>
        /// <returns>The standard readouts with some additional infos</returns>
        public abstract IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO);

        /// <summary>
        /// Extracts a list of readouts defined by <paramref name="myDBObjectStreams"/> from the edge. 
        /// Additional to the standard readout (generated from <paramref name="GetAllAttributesFromDBO"/>)
        /// some special information will be added (like Weight etc)
        /// If we had a function like TOP(1) the GetAllAttributesFromDBO might not contains elements which are in myObjectUUIDs (which is coming from the where)
        /// </summary>
        /// <param name="GetAllAttributesFromDBO">A delegate which will retriev the standard DBObjectReadout for a ObjectUUID</param>
        /// <param name="myDBObjectStreams">The Objects which should be extracted from the edge</param>
        /// <returns>The standard readouts with some additional infos</returns>
        public abstract IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO, IEnumerable<Exceptional<DBObjectStream>> myDBObjectStreams);

        /// <summary>
        /// This is just a helper for BackwardEdges
        /// </summary>
        /// <returns></returns>
        public abstract ObjectUUID FirstOrDefault();

        /// <summary>
        /// Adds a set of ObjectUUID with parameters
        /// </summary>
        /// <param name="hashSet"></param>
        /// <param name="myParameters"></param>
        public abstract void AddRange(IEnumerable<ObjectUUID> hashSet, TypeUUID typeOfObjects, params ADBBaseObject[] myParameters);

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        public abstract void Add(ObjectUUID myValue, TypeUUID typeOfObjects, params ADBBaseObject[] myParameters);      

        /// <summary>
        /// Check for a containing element
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public abstract Boolean Contains(ObjectUUID myValue);


        #region IReferenceEdge Members

        /// <summary>
        /// Get all added objectUUIDs
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<ObjectUUID> GetAllReferenceIDs();

        /// <summary>
        /// Get all added references
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Reference> GetAllReferences();
        
        /// <summary>
        /// Get all uuids and their edge infos
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetAllReferenceIDsWeighted();
        
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

        /// <summary>
        /// Get all destinations of an edge
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache);

        /// <summary>
        /// Get all weighted destinations of an edge
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<Exceptional<DBObjectStream>, ADBBaseObject>> GetAllEdgeDestinationsWeighted(DBObjectCache dbObjectCache);

        #endregion

    }
}
