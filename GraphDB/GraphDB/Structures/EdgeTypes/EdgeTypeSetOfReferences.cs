#region Usings

using System;
using System.Collections;
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


#endregion

namespace sones.GraphDB.Structures.EdgeTypes
{
    
    public class EdgeTypeSetOfReferences : ASetOfReferencesEdgeType
    {
        private Dictionary<ObjectUUID, Reference> _ObjectUUIDs = null;

        private UInt64          _estimatedSize  = 0;


        #region TypeCode
        public override UInt32 TypeCode { get { return 452; } }
        #endregion

        public EdgeTypeSetOfReferences()
        {
            _ObjectUUIDs = new Dictionary<ObjectUUID, Reference>();
        }

        public EdgeTypeSetOfReferences(IEnumerable<ObjectUUID> dbos, TypeUUID typeOfDBObjects)
        {
            Debug.Assert(typeOfDBObjects != null);

            _ObjectUUIDs = new Dictionary<ObjectUUID, Reference>();

            if (dbos != null)
            {
                foreach (var aUUID in dbos)
                {
                    _ObjectUUIDs.Add(aUUID, new Reference(aUUID, typeOfDBObjects));
                }
            }

            CalcEstimatedSize(this);
        }

        public EdgeTypeSetOfReferences(IEnumerable<KeyValuePair<ObjectUUID, Reference>> dbos)
        {

            _ObjectUUIDs = new Dictionary<ObjectUUID, Reference>();
            foreach (var dbo in dbos)
            {
                _ObjectUUIDs.Add(dbo.Key, dbo.Value);
            }

            CalcEstimatedSize(this);            
        }

        #region AEdgeType Members

        public override string EdgeTypeName
        {
            get { return "SET"; }
        }

        public override EdgeTypeUUID EdgeTypeUUID
        {
            get { return new EdgeTypeUUID(1000); }
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
            return new EdgeTypeSetOfReferences();
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return GetGDDL(myGraphDBType);
        }

        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return String.Concat("SET", "<", myGraphDBType.Name, ">");
        }

        #endregion

        #region AListEdgeType Members

        public override ulong Count()
        {
            return _ObjectUUIDs.ULongCount();
        }

        //public override IEnumerable GetAll()
        //{
        //    return new HashSet<ObjectUUID>(_ObjectUUIDs.Select(item => item.Key));
        //}

        //public override IEnumerable GetTop(ulong myNumOfEntries)
        //{
        //    return _ObjectUUIDs.Take((Int32)myNumOfEntries).Select(item => item.Key);
        //}

        public override IListOrSetEdgeType GetTopAsEdge(ulong myNumOfEntries)
        {

            if (_ObjectUUIDs.Count < (int)myNumOfEntries)
            {
                myNumOfEntries = (ulong)_ObjectUUIDs.Count;
            }

            return new EdgeTypeSetOfReferences(_ObjectUUIDs.Take((int)myNumOfEntries));

        }

        public override void Clear()
        {
            _ObjectUUIDs.Clear();

            CalcEstimatedSize(this);
        }

        public override void UnionWith(IListOrSetEdgeType myAEdgeType)
        {
            var anotherEdge = myAEdgeType as ASetOfReferencesEdgeType;

            if (anotherEdge != null)
            {
                foreach (var aReference in anotherEdge.GetAllReferences())
                {
                    if (!_ObjectUUIDs.ContainsKey(aReference.ObjectUUID))
                    {
                        _ObjectUUIDs.Add(aReference.ObjectUUID, aReference);
                    }
                }
            }
            else
            {
                throw new ArgumentException("myValue is not of type ObjectUUID");
            }

            CalcEstimatedSize(this);

        }

        public override bool Contains(ObjectUUID myValue)
        {
            return _ObjectUUIDs.ContainsKey(myValue);
        }

        public override void Distinction()
        {
        }

        #endregion

        #region AListReferenceEdgeType Members

        public override IEnumerable<ObjectUUID> GetAllReferenceIDs()
        {
            return _ObjectUUIDs.Select(aKV => aKV.Key);
        }

        public override IEnumerable<Vertex> GetVertices(Func<ObjectUUID, Vertex> GetAllAttributesFromDBO)
        {
            foreach (var dbo in _ObjectUUIDs)
            {
                yield return GetAllAttributesFromDBO(dbo.Key);
            }

            yield break;
        }

        public override IEnumerable<Vertex> GetReadouts(Func<ObjectUUID, Vertex> GetAllAttributesFromDBO, IEnumerable<Exceptional<DBObjectStream>> myObjectUUIDs)
        {
            foreach (var dbo in myObjectUUIDs)
            {

                if (dbo.Failed())
                    throw new GraphDBException(dbo.IErrors);
                /* If we had a function like TOP(1) the GetAllAttributesFromDBO might not contains elements which are in myObjectUUIDs (which is coming from the where)*/
                if (_ObjectUUIDs.ContainsKey(dbo.Value.ObjectUUID))
                {
                    //throw new Exception("Fatal error during EdgeTypeList.GetReadouts - the ObjectUUID does not have an edge!");

                    yield return GetAllAttributesFromDBO(dbo.Value.ObjectUUID);
                }
            }

            yield break;
        }

        public override ObjectUUID FirstOrDefault()
        {
            return _ObjectUUIDs.FirstOrDefault().Key;
        }

        public override void AddRange(IEnumerable<ObjectUUID> hashSet, TypeUUID typeOfDBObjects, params ADBBaseObject[] myParameters)
        {
            foreach (var aUUID in hashSet)
            {
                if (!_ObjectUUIDs.ContainsKey(aUUID))
                {
                    _ObjectUUIDs.Add(aUUID, new Reference(aUUID, typeOfDBObjects));
                }
            }

            CalcEstimatedSize(this);

        }

        public override void Add(ObjectUUID myValue, TypeUUID typeOfDBObjects, params ADBBaseObject[] myParameters)
        {
            if (!_ObjectUUIDs.ContainsKey(myValue))
            {
                var aReference = new Reference(myValue, typeOfDBObjects);

                _estimatedSize += aReference.GetEstimatedSize() + EstimatedSizeConstants.CalcUUIDSize(myValue);

                _ObjectUUIDs.Add(myValue, aReference);
            }
        }

        public override Boolean RemoveUUID(ObjectUUID myValue)
        {
            return _ObjectUUIDs.Remove(myValue);
        }

        public override Boolean RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            if (!myObjectUUIDs.IsNullOrEmpty())
            {
                foreach (var aUUID in myObjectUUIDs)
                {
                    _ObjectUUIDs.Remove(aUUID);
                }

                return true;
            }

            return false;
        }

        public override IReferenceEdge GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable)
        {
            var newEdge = new EdgeTypeSetOfReferences();

            foreach (var aDBO in iEnumerable)
            {
                newEdge.Add(aDBO.Value.ObjectUUID, aDBO.Value.TypeUUID);
            }
            
            return newEdge;
        }

        public override IReferenceEdge GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfObjects)
        {
            return new EdgeTypeSetOfReferences(iEnumerable, typeOfObjects);
        }

        #endregion

        public override string ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }

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

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeTypeSetOfReferences myValue)
        {
            mySerializationWriter.WriteInt32(myValue._ObjectUUIDs.Count);
            foreach (var obj in myValue._ObjectUUIDs)
            {
                obj.Value.Serialize(ref mySerializationWriter);
            }
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeSetOfReferences myValue)
        {
            var count = mySerializationReader.ReadInt32();
            for (Int32 i = 0; i < count; i++)
            {
                Reference aRef = new Reference();
                aRef.Deserialize(ref mySerializationReader);
                myValue._ObjectUUIDs.Add(aRef.ObjectUUID, aRef);
            }

            CalcEstimatedSize(myValue);

            return myValue;
        }

        #region IFastSerializationTypeSurrogate

        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter mySerializationWriter, object value)
        {
            EdgeTypeSetOfReferences thisObject = (EdgeTypeSetOfReferences)value;

            Serialize(ref mySerializationWriter, thisObject);
        }

        public override object Deserialize(SerializationReader mySerializationReader, Type type)
        {
            EdgeTypeSetOfReferences thisObject = (EdgeTypeSetOfReferences)Activator.CreateInstance(type);

            return Deserialize(ref mySerializationReader, thisObject);
        }

        #endregion


        #region Equals

        public override bool Equals(object obj)
        {
            var otherEdge = obj as EdgeTypeSetOfReferences;
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

            if (_ObjectUUIDs.Count != otherEdge._ObjectUUIDs.Count)
            {
                return false;
            }

            foreach (var uuid in _ObjectUUIDs)
            {
                if (!otherEdge._ObjectUUIDs.Contains(uuid))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region IComparable Members

        public override int CompareTo(object obj)
        {
            if (!(obj is EdgeTypeSetOfReferences))
            {
                return -1;
            }

            var otherEdge = (obj as EdgeTypeSetOfReferences);

            if (_ObjectUUIDs.Count != otherEdge._ObjectUUIDs.Count)
            {
                return _ObjectUUIDs.Count.CompareTo(otherEdge._ObjectUUIDs.Count);
            }

            var intersected = _ObjectUUIDs.Intersect(otherEdge._ObjectUUIDs).Count();

            if (intersected != _ObjectUUIDs.Count)
            {
                return intersected.CompareTo(_ObjectUUIDs.Count);
            }

            return 0;

        }

        #endregion


        public override IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache)
        {
            return _ObjectUUIDs.Select(kv => kv.Value.GetDBObjectStream(dbObjectCache));
        }

        public override IEnumerable<Reference> GetAllReferences()
        {
            return _ObjectUUIDs.Select(kv => kv.Value);
        }

        #region IObject

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(EdgeTypeSetOfReferences myTypeAttribute)
        {
            //Dictionary<ObjectUUID, Reference> + base size
            _estimatedSize = base.GetBaseSize();

            if (_ObjectUUIDs != null)
            {
                _estimatedSize += EstimatedSizeConstants.Dictionary;

                foreach (var aKV in _ObjectUUIDs)
                {
                    //key
                    _estimatedSize += EstimatedSizeConstants.CalcUUIDSize(aKV.Key);

                    //Value
                    _estimatedSize += aKV.Value.GetEstimatedSize();
                }
            }
        }

        #endregion
    }
}
