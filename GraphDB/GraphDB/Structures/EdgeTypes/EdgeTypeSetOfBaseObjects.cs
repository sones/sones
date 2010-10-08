using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using System.Collections;
using sones.GraphDB.TypeManagement;
using sones.Lib;


namespace sones.GraphDB.Structures.EdgeTypes
{

    public class EdgeTypeSetOfBaseObjects : ASetOfBaseEdgeType
    {

        #region Data

        HashSet<ADBBaseObject>  _Objects;
        private UInt64          _estimatedSize  = 0;

        #endregion

        #region Ctors

        public EdgeTypeSetOfBaseObjects()
        {
            _Objects = new HashSet<ADBBaseObject>();
        }

        public EdgeTypeSetOfBaseObjects(IEnumerable<ADBBaseObject> myObjects)
        {
            _Objects = new HashSet<ADBBaseObject>(myObjects);

            CalcEstimatedSize(this);
        }

        #endregion

        #region IListOrSetEdgeType Members

        public override ulong Count()
        {
            if (_Objects != null)
                return (UInt64)_Objects.Count;
            else
                return 0;
        }

        public override IListOrSetEdgeType GetTopAsEdge(ulong myNumOfEntries)
        {

            if (_Objects.Count < (int)myNumOfEntries)
            {
                myNumOfEntries = (ulong)_Objects.Count;
            }

            return new EdgeTypeSetOfBaseObjects(_Objects.Take((int)myNumOfEntries));
        }

        public override void UnionWith(IListOrSetEdgeType myAListEdgeType)
        {
            _Objects.UnionWith((myAListEdgeType as EdgeTypeSetOfBaseObjects)._Objects);

            CalcEstimatedSize(this);
        }

        public override void Distinction()
        {
            _Objects = new HashSet<ADBBaseObject>(_Objects.Distinct());
            CalcEstimatedSize(this);

        }

        public override void Clear()
        {
            _Objects.Clear();

            CalcEstimatedSize(this);
        }

        #endregion

        #region IBaseEdge Members

        public override IEnumerable<ADBBaseObject> GetBaseObjects()
        {
            return _Objects;
        }

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        public override void Add(ADBBaseObject myValue, params ADBBaseObject[] myParameters)
        {
            _Objects.Add(myValue);

            _estimatedSize += myValue.GetEstimatedSize();
        }

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        public override void AddRange(IEnumerable<ADBBaseObject> myValue, params ADBBaseObject[] myParameters)
        {
            _Objects.UnionWith(myValue);

            CalcEstimatedSize(this);
        }

        /// <summary>
        /// Remove a value
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public override Boolean Remove(ADBBaseObject myValue)
        {
            return _Objects.Remove((ADBBaseObject)myValue);
        }

        /// <summary>
        /// Check for a containing element
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public override bool Contains(ADBBaseObject myValue)
        {
            return _Objects.Contains((ADBBaseObject)myValue);
        }

        /// <summary>
        /// Returns all values. Use this for all not reference ListEdgeType.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Object> GetReadoutValues()
        {
            return (from o in _Objects select o.Value).ToList();
        }

        /// <summary>
        /// Get all data and their edge infos
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Tuple<ADBBaseObject, ADBBaseObject>> GetEdges()
        {
            foreach (var obj in _Objects)
            {
                yield return new Tuple<ADBBaseObject, ADBBaseObject>(obj, null);
            }
        }

        #endregion

        #region IEdgeType Members

        public override string EdgeTypeName
        {
            get { return "SetOfBaseObjects"; }
        }

        public override EdgeTypeUUID EdgeTypeUUID
        {
            get { return new EdgeTypeUUID(100); }
        }

        public override void ApplyParams(params EdgeTypeParamDefinition[] myParams)
        {

        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return "";
        }

        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return String.Concat("SET", "<", myGraphDBType.Name, ">");
        }

        public override IEdgeType GetNewInstance()
        {
            return new EdgeTypeSetOfBaseObjects();
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

        #region IFastSerializationTypeSurrogate Members

        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeTypeSetOfBaseObjects myValue)
        {
            mySerializationWriter.WriteUInt32((UInt32)myValue._Objects.Count);
            foreach (var obj in myValue._Objects)
            {
                mySerializationWriter.WriteObject(obj);
            }
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeSetOfBaseObjects myValue)
        {
            var count = mySerializationReader.ReadUInt32();
            myValue._Objects = new HashSet<ADBBaseObject>();
            for (UInt32 i = 0; i < count; i++)
            {
                myValue._Objects.Add((ADBBaseObject)mySerializationReader.ReadObject());
            }

            CalcEstimatedSize(myValue);

            return myValue;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            EdgeTypeSetOfBaseObjects thisObject = (EdgeTypeSetOfBaseObjects)value;
            Serialize(ref writer, thisObject);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            EdgeTypeSetOfBaseObjects thisObject = (EdgeTypeSetOfBaseObjects)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        public override UInt32 TypeCode { get { return 458; } }

        #endregion

        #region IEnumerable<ADBBaseObject> Members

        public override IEnumerator<ADBBaseObject> GetEnumerator()
        {
            return GetBaseObjects().GetEnumerator();
        }

        #endregion

        #region Equals

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var otherEdge = obj as EdgeTypeSetOfBaseObjects;
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

            if (_Objects.Count != otherEdge._Objects.Count)
            {
                return false;
            }

            foreach (var baseObj in _Objects)
            {
                if (!otherEdge._Objects.Contains(baseObj))
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
            throw new NotImplementedException();
        }

        #endregion

        public override string ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }

        #region IObject

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(EdgeTypeSetOfBaseObjects myTypeAttribute)
        {
            //_Objects + base size
            _estimatedSize = base.GetBaseSize();

            if (_Objects != null)
            {
                _estimatedSize += EstimatedSizeConstants.HashSet;

                foreach (var aADBBaseObject in _Objects)
                {
                    //key
                    _estimatedSize += aADBBaseObject.GetEstimatedSize();
                }
            }
        }

        #endregion

    }
}
