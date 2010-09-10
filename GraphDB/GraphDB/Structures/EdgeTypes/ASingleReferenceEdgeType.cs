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

/* <id name="GraphDB – abstract class for all single reference edges" />
 * <copyright file="ASingleEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This abstract class should be implemented for all single reference edges. It will store just an ObjectUUID. The complementary of this for not reference types are all ADBBaseObject implementations.</summary>
 */

#region usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib;
using sones.GraphDBInterface.Result;
using sones.GraphDBInterface.TypeManagement;


#endregion

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This abstract class should be implemented for all single reference edges. It will store just an ObjectUUID. The complementary of this for not reference types are all ADBBaseObject implementations.
    /// </summary>
    public abstract class ASingleReferenceEdgeType : IReferenceEdge
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
        public abstract void Set(ObjectUUID myValue, TypeUUID typeOfObjects, params ADBBaseObject[] myParameters);

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

        #region IReferenceEdge Members

        public bool Contains(ObjectUUID myValue)
        {
            return GetUUID() == myValue;
        }

        public ObjectUUID FirstOrDefault()
        {
            return GetUUID();
        }

        public IEnumerable<ObjectUUID> GetAllReferenceIDs()
        {
            yield return GetUUID();
        }

        public abstract IEnumerable<Reference> GetAllReferences();

        public abstract IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache);

        public abstract bool RemoveUUID(ObjectUUID myObjectUUID);

        public bool RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            var result = false;
            foreach (var uuid in myObjectUUIDs)
            {
                result = RemoveUUID(uuid);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public abstract IReferenceEdge GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable);

        public abstract IReferenceEdge GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfObjects);

        #endregion
        
        #region IComparable Members

        public abstract Int32 CompareTo(object obj);

        #endregion

    }

}
