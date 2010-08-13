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
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;


namespace sones.GraphDB.Structures.EdgeTypes
{
    
    public class EdgeTypeListOfBaseObjects : AListOfBaseEdgeType
    {
        
        #region Data

        List<ADBBaseObject> _Objects;

        #endregion

        #region Ctors

        public EdgeTypeListOfBaseObjects()
        {
            _Objects = new List<ADBBaseObject>();
        }

        public EdgeTypeListOfBaseObjects(IEnumerable<ADBBaseObject> myObjects)
        {
            _Objects = new List<ADBBaseObject>(myObjects);
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

            return new EdgeTypeListOfBaseObjects(_Objects.Take((int)myNumOfEntries));
        }

        public override void UnionWith(IListOrSetEdgeType myAListEdgeType)
        {
            _Objects = new List<ADBBaseObject>(_Objects.Union((myAListEdgeType as EdgeTypeListOfBaseObjects)._Objects));
        }

        public override void Distinction()
        {
            _Objects = new List<ADBBaseObject>(_Objects.Distinct());
        }

        public override void Clear()
        {
            _Objects.Clear();
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
        }

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        public override void AddRange(IEnumerable<ADBBaseObject> myValue, params ADBBaseObject[] myParameters)
        {
            _Objects.AddRange(myValue);
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
            get { return "LIST"; }
        }

        public override EdgeTypeUUID EdgeTypeUUID
        {
            get { return new EdgeTypeUUID(101); }
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
            return String.Concat("LIST", "<", myGraphDBType.Name, ">");
        }

        public override IEdgeType GetNewInstance()
        {
            return new EdgeTypeListOfBaseObjects();
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

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeTypeListOfBaseObjects myValue)
        {
            mySerializationWriter.WriteUInt32((UInt32)myValue._Objects.Count);
            foreach (var obj in myValue._Objects)
            {
                obj.ID.Serialize(ref mySerializationWriter);
                obj.Serialize(ref mySerializationWriter);
            }
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeListOfBaseObjects myValue)
        {
            var count = mySerializationReader.ReadUInt32();
            myValue._Objects = new List<ADBBaseObject>();
            for (UInt32 i = 0; i < count; i++)
            {
                TypeUUID id = new TypeUUID();
                id.Deserialize(ref mySerializationReader);
                ADBBaseObject obj = GraphDBTypeMapper.GetADBBaseObjectFromUUID(id);
                obj.Deserialize(ref mySerializationReader);
                myValue._Objects.Add(obj);
            }

            return myValue;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            var thisObject = (EdgeTypeListOfBaseObjects)value;
            Serialize(ref writer, thisObject);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            var thisObject = (EdgeTypeListOfBaseObjects)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        public override UInt32 TypeCode { get { return 453; } }

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
            var otherEdge = obj as EdgeTypeListOfBaseObjects;
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

        public override string ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }

    }
}
