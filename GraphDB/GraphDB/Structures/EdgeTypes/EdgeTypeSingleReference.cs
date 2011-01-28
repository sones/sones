/* <id name="GraphDB – EdgeTypeSingle" />
 * <copyright file="EdgeTypeSingle.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Holds a single ObjectUUID.</summary>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.NewAPI;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// Holds a single ObjectUUID
    /// </summary>
    
    public class EdgeTypeSingleReference : ASingleReferenceEdgeType
    {

        private Tuple<ObjectUUID, Reference> _ObjectUUID;

        private UInt64          _estimatedSize  = 0;

        #region TypeCode
        public override UInt32 TypeCode { get { return 455; } }
        #endregion

        public EdgeTypeSingleReference()
        {
            _ObjectUUID = null;
        }

        public EdgeTypeSingleReference(ObjectUUID dbos, TypeUUID myTypeOfDBObject)
        {
            Debug.Assert(myTypeOfDBObject != null);
            if (dbos != null)
            {
                _ObjectUUID = new Tuple<ObjectUUID, Reference>(dbos, new Reference(dbos, myTypeOfDBObject));

                CalcEstimatedSize(this);
            }
        }

        #region AEdgeType Members

        public override string EdgeTypeName
        {
            get { return "SINGLE"; }
        }

        public override EdgeTypeUUID EdgeTypeUUID
        {
            get { return new EdgeTypeUUID(10); }
        }

        public override void ApplyParams(params EdgeTypeParamDefinition[] myParams)
        {

        }

        public override IEnumerable<EdgeTypeParamDefinition> GetParams()
        {
            yield break;
        }

        public override IEdgeType GetNewInstance()
        {
            return new EdgeTypeSingleReference();
        }

        public override IReferenceEdge GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable)
        {
            if (iEnumerable.FirstOrDefault() == null || _ObjectUUID.Item1 != iEnumerable.First().Value.ObjectUUID)
            {
                throw new GraphDBException(new Errors.Error_InvalidEdgeType(typeof(EdgeTypeSetOfReferences)));
            }
            else
            {
                var newInst = GetNewInstance() as EdgeTypeSingleReference;

                newInst.Set(_ObjectUUID.Item1, iEnumerable.First().Value.TypeUUID);

                return newInst;
            }
        }

        public override IReferenceEdge GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfObjects)
        {
            if (iEnumerable.FirstOrDefault() == null || _ObjectUUID.Item1 != iEnumerable.First())
            {
                throw new GraphDBException(new Errors.Error_InvalidEdgeType(typeof(EdgeTypeSetOfReferences)));
            }
            else
            {
                var newInst = GetNewInstance() as EdgeTypeSingleReference;
                newInst.Set(_ObjectUUID.Item1, typeOfObjects);

                return newInst;
            }
        }

        #endregion

        #region ASingleEdgeType Members

        public override ObjectUUID GetUUID()
        {
            return _ObjectUUID.Item1;
        }

        public override bool RemoveUUID(ObjectUUID myObjectUUID)
        {
            if (_ObjectUUID.Item1 == myObjectUUID)
            {
                _ObjectUUID = null;
                return true;
            }

            return false;
        }

        public override void Set(ObjectUUID myValue, TypeUUID typeOfDBObject, params ADBBaseObject[] myParameters)
        {
            _ObjectUUID = new Tuple<ObjectUUID, Reference>(myValue, new Reference(myValue, typeOfDBObject));

            CalcEstimatedSize(this);
        }

        public override void Merge(ASingleReferenceEdgeType mySingleEdgeType)
        {
            var aReference = mySingleEdgeType.GetAllReferences().FirstOrDefault();

            _ObjectUUID = new Tuple<ObjectUUID, Reference>(aReference.ObjectUUID, aReference);

            CalcEstimatedSize(this);

        }

        public override Vertex GetVertex(Func<ObjectUUID, Vertex> GetAllAttributesFromDBO)
        {
            return GetAllAttributesFromDBO(_ObjectUUID.Item1);
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
            {
                mySerializationWriter.WriteObject(myValue._ObjectUUID.Item2);
            }
            else
            {
                mySerializationWriter.WriteObject(null);

            }

        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeSingleReference myValue)
        {
            var reference = (Reference)mySerializationReader.ReadObject();

            if (reference != null)
            {
                myValue._ObjectUUID = new Tuple<ObjectUUID, Reference>(reference.ObjectUUID, reference);
            }

            CalcEstimatedSize(myValue);

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



        public Boolean Equals(EdgeTypeSingleReference p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            if (EdgeTypeName != p.EdgeTypeName)
            {
                return false;
            }
            if (EdgeTypeUUID != p.EdgeTypeUUID)
            {
                return false;
            }
            if (_ObjectUUID.Item2 != p._ObjectUUID.Item2)
            {
                return false;
            }

            return true;
        }

        public static Boolean operator ==(EdgeTypeSingleReference a, EdgeTypeSingleReference b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(EdgeTypeSingleReference a, EdgeTypeSingleReference b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EdgeTypeSingleReference))
            {
                return false;
            }

            var otherEdge = (EdgeTypeSingleReference)obj;

            if (EdgeTypeName != otherEdge.EdgeTypeName)
            {
                return false;
            }
            if (EdgeTypeUUID != otherEdge.EdgeTypeUUID)
            {
                return false;
            }

            if ((_ObjectUUID == null) && (otherEdge._ObjectUUID == null))
            {
                return true;
            }

            return _ObjectUUID.Equals(otherEdge._ObjectUUID);
        }

        #endregion

        #region IComparable Members

        public override int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache)
        {
            yield return _ObjectUUID.Item2.GetDBObjectStream(dbObjectCache);

            yield break;
        }

        public override IEnumerable<Reference> GetAllReferences()
        {
            yield return _ObjectUUID.Item2;

            yield break;
        }

        #region IObject

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(EdgeTypeSingleReference myTypeAttribute)
        {
            //Tuple<ObjectUUID, Reference> + base size
            _estimatedSize = base.GetBaseSize() + EstimatedSizeConstants.Tuple;

            if (_ObjectUUID != null)
            {
                if (_ObjectUUID.Item1 != null)
                {
                    _estimatedSize += EstimatedSizeConstants.CalcUUIDSize(_ObjectUUID.Item1);
                }

                if (_ObjectUUID.Item2 != null)
                {
                    _estimatedSize += _ObjectUUID.Item2.GetEstimatedSize();
                }
            }
        }

        #endregion

    }
}

