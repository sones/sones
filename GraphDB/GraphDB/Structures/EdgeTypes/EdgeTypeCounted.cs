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

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.Structures.EdgeTypes
{
    
    public class EdgeTypeCounted : ASingleReferenceEdgeType
    {

        private Reference _Reference;
        private ADBBaseObject _Count;
        private ADBBaseObject _CountBy;
        private TypeUUID _typeOfDBObject = null;

        #region TypeCode 
        public override UInt32 TypeCode { get { return 451; } }
        #endregion

        public EdgeTypeCounted()
        {
        }

        public EdgeTypeCounted(TypeUUID typeOfDBObject)
        {
            _typeOfDBObject = typeOfDBObject;
        }

        #region AEdgeType Members

        public override string EdgeTypeName
        {
            get { return "COUNTED"; }
        }

        public override EdgeTypeUUID EdgeTypeUUID
        {
            get { return new EdgeTypeUUID(11); }
        }

        public override void ApplyParams(params EdgeTypeParamNode[] myParams)
        {

            if (myParams.Count() == 0)
                throw new ArgumentException("EdgeTypeCounted: Expected at least 1 parameter for edge type counted!");

            // The first parameter has to be the type
            if (myParams[0].Type != EdgeTypeParamNode.ParamType.PandoraType)
            {
                throw new ArgumentException("EdgeTypeCounted: The first parameter has to be the type 'Integer', 'Double, etc");
            }
            else
            {
                _CountBy = myParams[0].Param as ADBBaseObject;
            }

            _Count = _CountBy.Clone();
            _Count.SetValue(DBObjectInitializeType.Default);

            #region Get default node if exists

            if (myParams.Any(p => p.Type == EdgeTypeParamNode.ParamType.DefaultValueDef))
            {
                var def = (from p in myParams where p.Type == EdgeTypeParamNode.ParamType.DefaultValueDef select p).First();
                _CountBy.SetValue(def.Param);
            }
            else
            {
                _CountBy.SetValue(1);
            }

            #endregion

        }

        public override AEdgeType GetNewInstance()
        {
            var edgeTypeCounted = new EdgeTypeCounted();
            if (_Count != null)
                edgeTypeCounted._Count = _Count.Clone();
            if (_CountBy != null)
                edgeTypeCounted._CountBy = _CountBy.Clone();
            return edgeTypeCounted;
        }

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable, TypeUUID typeOfDBObjects)
        {
            var edgeTypeCounted = new EdgeTypeCounted(typeOfDBObjects);
            if (_Count != null)
                edgeTypeCounted._Count = _Count.Clone();
            if (_CountBy != null)
                edgeTypeCounted._CountBy = _CountBy.Clone();
            return edgeTypeCounted;
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfDBObjects)
        {
            var edgeTypeCounted = new EdgeTypeCounted(typeOfDBObjects);
            if (_Count != null)
                edgeTypeCounted._Count = _Count.Clone();
            if (_CountBy != null)
                edgeTypeCounted._CountBy = _CountBy.Clone();
            return edgeTypeCounted;
        }

        public override DBObjectReadout GetReadout(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            return (DBObjectReadout) new DBWeightedObjectReadout(GetAllAttributesFromDBO(_Reference.ObjectUUID).Attributes, _Count);
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return GetGDDL(myGraphDBType);
        }


        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return String.Concat(EdgeTypeName.ToUpper(), GraphQL.TERMINAL_BRACKET_LEFT, _Count.ObjectName, ", ", "DEFAULT=", _CountBy.Value.ToString(), GraphQL.TERMINAL_BRACKET_RIGHT, GraphQL.TERMINAL_LT, myGraphDBType.Name, GraphQL.TERMINAL_GT);
        }

        #endregion

        #region ASingleEdgeType Members

        public override ObjectUUID GetUUID()
        {
            return _Reference.ObjectUUID;
        }

        public override IEnumerable<ObjectUUID> GetAllReferenceIDs()
        {
            yield return _Reference.ObjectUUID;

            yield break;
        }

        public override void Set(ObjectUUID myValue, TypeUUID typeOfDBObjects, params ADBBaseObject[] myParameters)
        {
            if (myValue == null)
            {
                _Reference = null;
                _Count.SetValue(DBObjectInitializeType.Default);
                return;
            }

            if (_typeOfDBObject == null)
            {
                _typeOfDBObject = typeOfDBObjects;
            }

            _Reference = new Reference(myValue, typeOfDBObjects);

            if (myParameters != null && myParameters.Count() > 0)
            {
                if (!_Count.IsValidValue(myParameters[0].Value))
                {
                    throw new GraphDBException(new Error_EdgeParameterTypeMismatch(myParameters[0], _Count));
                }
                _Count.Add(myParameters[0]);
            }
            else
            {
                _Count.Add(_CountBy);
            }
        }

        public override Boolean RemoveUUID(ObjectUUID myObjectUUID)
        {
            if (myObjectUUID == _Reference.ObjectUUID)
            {
                _Reference = null;
                return true;
            }

            return false;
        }

        public override bool RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            if (!myObjectUUIDs.IsNullOrEmpty())
            {
                if (myObjectUUIDs.Contains(_Reference.ObjectUUID))
                {
                    _Reference = null;
                    return true;
                }

                return false;
            }

            return false;
        }

        public override void Merge(ASingleReferenceEdgeType mySingleEdgeType)
        {

            if (!(mySingleEdgeType is EdgeTypeCounted))
                throw new ArgumentException("mySingleEdgeType is not of type EdgeTypeCounted");

            _Reference = new Reference(mySingleEdgeType.GetUUID(), mySingleEdgeType.GetTypeUUIDOfReferences());

            _Count.Add((mySingleEdgeType as EdgeTypeCounted)._Count);
        }

        /// <summary>
        /// Get all uuids and their edge infos
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetAllReferenceIDsWeighted()
        {
            yield return new Tuple<ObjectUUID, ADBBaseObject>(_Reference.ObjectUUID, _Count);
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

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeTypeCounted myValue)
        {
            mySerializationWriter.WriteObject(_typeOfDBObject);
            
            //if (myValue._typeOfDBObject != null)
            //{
            //    myValue._typeOfDBObject.Serialize(ref mySerializationWriter);
            //}
            //else
            //{
            //    mySerializationWriter.WriteObject(null);
            //}

            mySerializationWriter.WriteObject(myValue._Reference);
            //if (myValue._Reference != null)
            //{
            //    myValue._Reference.Serialize(ref mySerializationWriter);
            //}
            //else
            //{
            //    mySerializationWriter.WriteObject(null);
            //}

            myValue._Count.ID.Serialize(ref mySerializationWriter);
            myValue._Count.Serialize(ref mySerializationWriter);
            myValue._CountBy.ID.Serialize(ref mySerializationWriter);
            myValue._CountBy.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeCounted myValue)
        {
            myValue._typeOfDBObject = new TypeUUID(ref mySerializationReader);
            myValue._Reference = (Reference)mySerializationReader.ReadObject();
            TypeUUID countType = new TypeUUID(ref mySerializationReader);
            myValue._Count = GraphDBTypeMapper.GetADBBaseObjectFromUUID(countType);
            myValue._Count.Deserialize(ref mySerializationReader);
            TypeUUID countByType = new TypeUUID(ref mySerializationReader);
            myValue._CountBy = GraphDBTypeMapper.GetADBBaseObjectFromUUID(countByType);
            myValue._CountBy.Deserialize(ref mySerializationReader);

            return myValue;
        }

        public override string ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            EdgeTypeCounted thisObject = (EdgeTypeCounted)value;
            Serialize(ref writer, thisObject);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            EdgeTypeCounted thisObject = (EdgeTypeCounted)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion
        
        #region Equals

        public override bool Equals(object obj)
        {
            var otherEdge = obj as EdgeTypeCounted;
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
            if (!_Count.Equals(otherEdge._Count))
            {
                return false;
            }
            if (!_CountBy.Equals(otherEdge._CountBy))
            {
                return false;
            }

            return true;
        }

        #endregion

        public override IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache)
        {
            yield return _Reference.GetDBObjectStream(dbObjectCache);

            yield break;
        }

        public override IEnumerable<Tuple<Exceptional<DBObjectStream>, ADBBaseObject>> GetAllEdgeDestinationsWeighted(DBObjectCache dbObjectCache)
        {
            yield return new Tuple<Exceptional<DBObjectStream>, ADBBaseObject>(_Reference.GetDBObjectStream(dbObjectCache), _Count);

            yield break;
        }

        public override TypeUUID GetTypeUUIDOfReferences()
        {
            return _typeOfDBObject;
        }
    }
}
