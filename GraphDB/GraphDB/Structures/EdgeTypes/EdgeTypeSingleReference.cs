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

/* <id name="sones GraphDB – EdgeTypeSingle" />
 * <copyright file="EdgeTypeSingle.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Holds a single ObjectUUID.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.Lib.Serializer;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.NewFastSerializer;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.Lib;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// Holds a single ObjectUUID
    /// </summary>
    
    public class EdgeTypeSingleReference : ASingleReferenceEdgeType
    {
        private ObjectUUID _ObjectUUID;
        
        #region TypeCode
        public override UInt32 TypeCode { get { return 455; } }
        #endregion

        #region AEdgeType Members

        public override string EdgeTypeName
        {
            get { return "SINGLE"; }
        }

        public override EdgeTypeUUID EdgeTypeUUID
        {
            get { return new EdgeTypeUUID(10); }
        }

        public override void ApplyParams(params EdgeTypeParamNode[] myParams)
        {

        }

        public override AEdgeType GetNewInstance()
        {
            return new EdgeTypeSingleReference();
        }

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable)
        {
            if (iEnumerable.FirstOrDefault() == null || _ObjectUUID != iEnumerable.First().Value.ObjectUUID)
            {
                throw new GraphDBException(new Errors.Error_InvalidEdgeType(typeof(EdgeTypeSetOfReferences)));
            }
            else
            {
                var newInst = GetNewInstance() as EdgeTypeSingleReference;
                newInst.Set(_ObjectUUID);
                return newInst;
            }
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable)
        {
            if (iEnumerable.FirstOrDefault() == null || _ObjectUUID != iEnumerable.First())
            {
                throw new GraphDBException(new Errors.Error_InvalidEdgeType(typeof(EdgeTypeSetOfReferences)));
            }
            else
            {
                var newInst = GetNewInstance() as EdgeTypeSingleReference;
                newInst.Set(_ObjectUUID);
                return newInst;
            }
        }

        #endregion

        #region ASingleEdgeType Members

        public override ObjectUUID GetUUID()
        {
            return _ObjectUUID;
        }

        public override IEnumerable<ObjectUUID> GetAllUUIDs()
        {
            yield return _ObjectUUID;

            yield break;
        }

        public override bool RemoveUUID(ObjectUUID myObjectUUID)
        {
            if (_ObjectUUID == myObjectUUID)
            {
                _ObjectUUID = null;
                return true;
            }

            return false;
        }

        public override bool RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            if (!myObjectUUIDs.IsNullOrEmpty())
            {
                if (myObjectUUIDs.Contains(_ObjectUUID))
                {
                    _ObjectUUID = null;
                    return true;
                }

                return false;
            }

            return false;
        }

        public override void Set(ObjectUUID myValue, params ADBBaseObject[] myParameters)
        {
            _ObjectUUID = myValue;
        }

        public override void Merge(ASingleReferenceEdgeType mySingleEdgeType)
        {
            _ObjectUUID = mySingleEdgeType.GetUUID();
        }

        public override DBObjectReadout GetReadout(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            return GetAllAttributesFromDBO(_ObjectUUID);
        }

        public override IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetEdges()
        {
            yield return new Tuple<ObjectUUID, ADBBaseObject>(_ObjectUUID, null);
        }

        #endregion

        #region IFastSerialize Members

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            Serialize(ref mySerializationWriter, this);
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            Deserialize(ref mySerializationReader, this);
        }

        #endregion

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeTypeSingleReference myValue)
        {
            if (myValue._ObjectUUID != null)
                //myValue._ObjectUUID.Serialize(ref mySerializationWriter);
                mySerializationWriter.WriteObject(myValue._ObjectUUID.GetByteArray());
            else
                mySerializationWriter.WriteObject(null);

        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeSingleReference myValue)
        {
            var value = (Byte[])mySerializationReader.ReadObject();
            if (value == null)
            {
                myValue._ObjectUUID = null;
            }
            else
            {
                myValue._ObjectUUID = new ObjectUUID(value);
            }

            //myValue._ObjectUUID = new ObjectUUID();
            //myValue._ObjectUUID.Deserialize(ref mySerializationReader);
            return myValue;
        }

        #region IFastSerializationTypeSurrogate 
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            EdgeTypeSingleReference thisObject = (EdgeTypeSingleReference)value;
            Serialize(ref writer, thisObject);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            EdgeTypeSingleReference thisObject = (EdgeTypeSingleReference)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }
        #endregion

        public override String ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return GetGDDL(myGraphDBType);
        }

        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return myGraphDBType.Name;
        }

        #region Equals

        public override bool Equals(object obj)
        {
            var otherEdge = obj as EdgeTypeSingleReference;
            if (otherEdge == null)
            {
                return false;
            }

            if (EdgeTypeName != otherEdge.EdgeTypeName)
            {
                return false;
            }
            if (EdgeTypeUUID != otherEdge.EdgeTypeUUID)
            {
                return false;
            }
            if (_ObjectUUID != otherEdge._ObjectUUID)
            {
                return false;
            }

            return true;
        }

        #endregion


    }
}

