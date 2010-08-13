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

/* <id name="sones GraphDB – abstract class for all not reference list edges" />
 * <copyright file="AListBaseEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This abstract class should be implemented for all not reference list edges. It provides the base methods which are needed from the Database to retrieve all values.</summary>
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This abstract class should be implemented for all not reference list edges. It provides the base methods which are needed from the Database to retrieve all values.
    /// </summary>    
    public abstract class AListOfBaseEdgeType : IBaseEdge
    {

        #region IListOrSetEdgeType Members

        public abstract ulong Count();

        public abstract IListOrSetEdgeType GetTopAsEdge(ulong myNumOfEntries);

        public abstract void UnionWith(IListOrSetEdgeType myAListEdgeType);

        public abstract void Distinction();

        public abstract void Clear();

        #endregion

        #region IBaseEdge Members

        public abstract IEnumerable<ADBBaseObject> GetBaseObjects();

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        public abstract void Add(ADBBaseObject myValue, params ADBBaseObject[] myParameters);

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        public abstract void AddRange(IEnumerable<ADBBaseObject> myValue, params ADBBaseObject[] myParameters);

        /// <summary>
        /// Remove a value
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public abstract Boolean Remove(ADBBaseObject myValue);


        /// <summary>
        /// Check for a containing element
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public abstract Boolean Contains(ADBBaseObject myValue);

        /// <summary>
        /// Returns all values. Use this for all not reference ListEdgeType.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Object> GetReadoutValues();

        /// <summary>
        /// Get all data and their edge infos
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<ADBBaseObject, ADBBaseObject>> GetEdges();

        #endregion

        #region IEdgeType Members

        public abstract string EdgeTypeName
        {
            get;
        }

        public abstract EdgeTypeUUID EdgeTypeUUID
        {
            get;
        }

        public abstract void ApplyParams(params Managers.Structures.EdgeTypeParamDefinition[] myParams);

        public abstract string GetDescribeOutput(GraphDBType myGraphDBType);

        public abstract string GetGDDL(GraphDBType myGraphDBType);

        public abstract IEdgeType GetNewInstance();

        #endregion

        #region IFastSerialize Members

        public bool isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public abstract void Serialize(ref Lib.NewFastSerializer.SerializationWriter mySerializationWriter);

        public abstract void Deserialize(ref Lib.NewFastSerializer.SerializationReader mySerializationReader);

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public abstract bool SupportsType(Type type);

        public abstract void Serialize(Lib.NewFastSerializer.SerializationWriter writer, object value);

        public abstract object Deserialize(Lib.NewFastSerializer.SerializationReader reader, Type type);

        public abstract uint TypeCode
        {
            get;
        }

        #endregion


        #region IEnumerable<ADBBaseObject> Members

        public abstract IEnumerator<ADBBaseObject> GetEnumerator();

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
