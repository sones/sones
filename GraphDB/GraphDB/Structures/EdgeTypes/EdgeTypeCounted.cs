
#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Result;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Structures.EdgeTypes
{
    
    public class EdgeTypeCounted : ASingleReferenceWithInfoEdgeType
    {

        private Reference _Reference;
        private ADBBaseObject _Count;
        private ADBBaseObject _CountBy;

        #region TypeCode 
        public override UInt32 TypeCode { get { return 451; } }
        #endregion

        public EdgeTypeCounted()
        {
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

        public override void ApplyParams(params EdgeTypeParamDefinition[] myParams)
        {

            if (myParams.Count() == 0)
                throw new GraphDBException(new Error_EdgeParameterCountMismatch(EdgeTypeName, 0, 1));

            // The first parameter has to be the type
            if (myParams[0].Type != ParamType.GraphType)
            {
                throw new GraphDBException(new Error_DataTypeDoesNotMatch(myParams[0].Type.ToString(), "BaseType like 'Integer', 'Double, etc"));
            }
            else
            {
                _CountBy = myParams[0].Param as ADBBaseObject;
            }

            _Count = _CountBy.Clone();
            _Count.SetValue(DBObjectInitializeType.Default);

            #region Get default node if exists

            if (myParams.Any(p => p.Type == ParamType.DefaultValueDef))
            {
                var def = (from p in myParams where p.Type == ParamType.DefaultValueDef select p).First();
                if (_CountBy.IsValidValue(def.Param))
                {
                    _CountBy.SetValue(def.Param);
                }
                else
                {
                    throw new GraphDBException(new Error_DataTypeDoesNotMatch(_CountBy.ObjectName, def.Param.GetType().Name));
                }
            }
            else
            {
                _CountBy.SetValue(1);
            }

            #endregion

        }

        public override IEdgeType GetNewInstance()
        {
            var edgeTypeCounted = new EdgeTypeCounted();
            if (_Count != null)
                edgeTypeCounted._Count = _Count.Clone();
            if (_CountBy != null)
                edgeTypeCounted._CountBy = _CountBy.Clone();
            return edgeTypeCounted;
        }

        public override IReferenceEdge GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable)
        {
            var edgeTypeCounted = new EdgeTypeCounted();
            if (_Count != null)
                edgeTypeCounted._Count = _Count.Clone();
            if (_CountBy != null)
                edgeTypeCounted._CountBy = _CountBy.Clone();
            return edgeTypeCounted;
        }

        public override IReferenceEdge GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfDBObjects)
        {
            var edgeTypeCounted = new EdgeTypeCounted();
            if (_Count != null)
                edgeTypeCounted._Count = _Count.Clone();
            if (_CountBy != null)
                edgeTypeCounted._CountBy = _CountBy.Clone();
            return edgeTypeCounted;
        }

        public override Vertex GetVertex(Func<ObjectUUID, Vertex> GetAllAttributesFromDBO)
        {
            return (Vertex) new Vertex_WeightedEdges(GetAllAttributesFromDBO(_Reference.ObjectUUID).ObsoleteAttributes, _Count.Value, _Count.Type.ToString());
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return GetGDDL(myGraphDBType);
        }


        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return String.Concat(EdgeTypeName.ToUpper(), "(", _Count.ObjectName, ", ", "DEFAULT=", _CountBy.Value.ToString(), ")", "<", myGraphDBType.Name, ">");
        }

        #endregion

        #region ASingleEdgeType Members

        public override ObjectUUID GetUUID()
        {
            return _Reference.ObjectUUID;
        }
        
        public override IEnumerable<Reference> GetAllReferences()
        {
            yield return _Reference;

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


        public override void Merge(ASingleReferenceEdgeType mySingleEdgeType)
        {

            if (!(mySingleEdgeType is EdgeTypeCounted))
                throw new ArgumentException("mySingleEdgeType is not of type EdgeTypeCounted");

            var reference = mySingleEdgeType.GetAllReferences().FirstOrDefault();

            _Reference = reference;

            _Count.Add((mySingleEdgeType as EdgeTypeCounted)._Count);
        }

        /// <summary>
        /// Get all uuids and their edge infos
        /// </summary>
        /// <returns></returns>
        public override Tuple<ObjectUUID, ADBBaseObject> GetReferenceIDWeighted()
        {
            return new Tuple<ObjectUUID, ADBBaseObject>(_Reference.ObjectUUID, _Count);
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
            mySerializationWriter.WriteObject(myValue._Reference);
            mySerializationWriter.WriteObject(myValue._Count);
            mySerializationWriter.WriteObject(myValue._CountBy);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeCounted myValue)
        {
            myValue._Reference = (Reference)mySerializationReader.ReadObject();
            myValue._Count = (ADBBaseObject)mySerializationReader.ReadObject();
            myValue._CountBy = (ADBBaseObject)mySerializationReader.ReadObject();
            
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

        public override Tuple<Exceptional<DBObjectStream>, ADBBaseObject> GetEdgeDestinationWeighted(DBObjectCache dbObjectCache)
        {
            return new Tuple<Exceptional<DBObjectStream>, ADBBaseObject>(_Reference.GetDBObjectStream(dbObjectCache), _Count);
        }


        #region IComparable Members

        public override int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
