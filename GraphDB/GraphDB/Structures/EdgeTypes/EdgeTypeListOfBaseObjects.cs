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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;
using sones.Lib.Serializer;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.NewFastSerializer;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Structures.EdgeTypes
{
    
    public class EdgeTypeListOfBaseObjects : AListBaseEdgeType
    {

        List<ADBBaseObject> _Objects;

        public EdgeTypeListOfBaseObjects()
        {
            _Objects = new List<ADBBaseObject>();
        }

        #region TypeCode
        public override UInt32 TypeCode { get { return 453; } }
        #endregion

        #region AEdgeType Members

        public override string EdgeTypeName
        {
            get { return "LIST"; }
        }

        public override EdgeTypeUUID EdgeTypeUUID
        {
            get { return new EdgeTypeUUID(100); }
        }

        public override void ApplyParams(params EdgeTypeParamNode[] myParams)
        {

        }

        public override AEdgeType GetNewInstance()
        {
            return new EdgeTypeListOfBaseObjects();
        }

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable)
        {
            return GetNewInstance();
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable)
        {
            return GetNewInstance();
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return GetGDDL(myGraphDBType);
        }

        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return String.Concat(GraphQL.TERMINAL_LIST, GraphQL.TERMINAL_LT, myGraphDBType.Name, GraphQL.TERMINAL_GT);
        }

        #endregion


        #region AListEdgeType Members

        public override void Clear()
        {
            _Objects.Clear();
        }

        public override ulong Count()
        {
            if (_Objects != null)
                return (UInt64)_Objects.Count;
            else
                return 0;
        }

        public override System.Collections.IEnumerable GetAll()
        {
            return _Objects;
        }

        public override System.Collections.IEnumerable GetTop(ulong myNumOfEntries)
        {
            return _Objects.GetRange(0, (Int32)myNumOfEntries);
        }

        public override void UnionWith(AEdgeType myAListEdgeType)
        {
            _Objects = new List<ADBBaseObject>(_Objects.Union((myAListEdgeType as EdgeTypeListOfBaseObjects)._Objects));
        }


        public override void Distinction()
        {
            _Objects = new List<ADBBaseObject>(_Objects.Distinct());
        }

        public override IEnumerable<ADBBaseObject> GetBaseObjects()
        {
            return _Objects;
        }

        #endregion

        #region IBaseEdge Members

        public override void Add(ADBBaseObject myValue, params ADBBaseObject[] myParameters)
        {
            _Objects.Add((ADBBaseObject)myValue);
        }

        public override void AddRange(IEnumerable<ADBBaseObject> myValue, params ADBBaseObject[] myParameters)
        {
            _Objects.AddRange(myValue);
        }

        public override bool Contains(ADBBaseObject myValue)
        {
            return _Objects.Contains((ADBBaseObject)myValue);
        }

        public override Boolean Remove(ADBBaseObject myValue)
        {
            return _Objects.Remove((ADBBaseObject)myValue);
        }

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

        #region IEnumerable Members

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return _Objects.GetEnumerator();
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

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeTypeListOfBaseObjects myValue)
        {
            mySerializationWriter.WriteObject(myValue._Objects.Count);
            foreach (var obj in myValue._Objects)
            {
                obj.ID.Serialize(ref mySerializationWriter);
                obj.Serialize(ref mySerializationWriter);
            }
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeListOfBaseObjects myValue)
        {
            var count = (Int32)mySerializationReader.ReadObject();
            myValue._Objects = new List<ADBBaseObject>();
            for (Int32 i = 0; i < count; i++)
            {
                TypeUUID id = new TypeUUID();
                id.Deserialize(ref mySerializationReader);
                ADBBaseObject obj = GraphDBTypeMapper.GetADBBaseObjectFromUUID(id);
                obj.Deserialize(ref mySerializationReader);
                myValue._Objects.Add(obj);
            }

            return myValue;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            EdgeTypeListOfBaseObjects thisObject = (EdgeTypeListOfBaseObjects)value;
            Serialize(ref writer, thisObject);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            EdgeTypeListOfBaseObjects thisObject = (EdgeTypeListOfBaseObjects)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);            
        }

        #endregion

        public override string ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }

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

    }
}
