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
using sones.GraphDB.TypeManagement;
using sones.Lib;
using System.Collections;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.Serializer;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.ObjectManagement;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage;
using sones.GraphDB.Errors;

namespace sones.GraphDB.Structures.EdgeTypes
{
    
    public class EdgeTypeSetOfReferences : ASetReferenceEdgeType
    {
        private HashSet<ObjectUUID> _Objects;

        #region TypeCode
        public override UInt32 TypeCode { get { return 452; } }
        #endregion

        public EdgeTypeSetOfReferences()
        {
            _Objects = new HashSet<ObjectUUID>();
        }

        public EdgeTypeSetOfReferences(IEnumerable<ObjectUUID> dbos)
        {
            _Objects = new HashSet<ObjectUUID>(dbos);
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

        public override void ApplyParams(params EdgeTypeParamNode[] myParams)
        {

        }

        public override AEdgeType GetNewInstance()
        {
            return new EdgeTypeSetOfReferences();
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return GetGDDL(myGraphDBType);
        }

        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return String.Concat(GraphQL.TERMINAL_SET, GraphQL.TERMINAL_LT, myGraphDBType.Name, GraphQL.TERMINAL_GT);
        }

        #endregion

        #region AListEdgeType Members

        public override ulong Count()
        {
            return _Objects.ULongCount();
        }

        public override IEnumerable GetAll()
        {
            return _Objects;
        }

        public override IEnumerable GetTop(ulong myNumOfEntries)
        {
            return _Objects.Take((Int32)myNumOfEntries);
        }

        public override void Clear()
        {
            _Objects.Clear();
        }

        public override void UnionWith(AEdgeType myAEdgeType)
        {
            if (myAEdgeType is ASetReferenceEdgeType)
                _Objects.UnionWith(((ASetReferenceEdgeType)myAEdgeType).GetAllUUIDs());
            else
                throw new ArgumentException("myValue is not of type ObjectUUID");
        }

        public override bool Contains(ObjectUUID myValue)
        {
            if (!(myValue is ObjectUUID))
                throw new ArgumentException("myValue is not of type ObjectUUID");

            return _Objects.Contains((ObjectUUID)myValue);
        }

        public override void Distinction()
        {
        }

        #endregion

        #region AListReferenceEdgeType Members

        public override IEnumerable<ObjectUUID> GetAllUUIDs()
        {
            return _Objects;
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            foreach (var dbo in _Objects)
                //result.Add(GetAllAttributesFromDBO(dbo));
                yield return GetAllAttributesFromDBO(dbo);

            //return result;
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO, IEnumerable<Exceptional<DBObjectStream>> myObjectUUIDs)
        {
            foreach (var dbo in myObjectUUIDs)
            {

                if (dbo.Failed)
                    throw new GraphDBException(dbo.Errors);
                /* If we had a function like TOP(1) the GetAllAttributesFromDBO might not contains elements which are in myObjectUUIDs (which is coming from the where)*/
                if (_Objects.Contains(dbo.Value.ObjectUUID))
                {
                    //throw new Exception("Fatal error during EdgeTypeList.GetReadouts - the ObjectUUID does not have an edge!");

                    yield return GetAllAttributesFromDBO(dbo.Value.ObjectUUID);
                }
            }
        }

        public override void RemoveWhere(Predicate<ObjectUUID> match)
        {
            _Objects.RemoveWhere(match);
        }

        public override ObjectUUID FirstOrDefault()
        {
            return _Objects.FirstOrDefault();
        }

        public override void AddRange(IEnumerable<ObjectUUID> hashSet, params ADBBaseObject[] myParameters)
        {
            _Objects.UnionWith(hashSet);
        }

        public override void Add(ObjectUUID myValue, params ADBBaseObject[] myParameters)
        {
            if (!(myValue is ObjectUUID))
                throw new GraphDBException(new Error_UnknownDBError("myValue is not of type ObjectUUID"));

            _Objects.Add((ObjectUUID)myValue);
        }

        public override void Add(IEnumerable<ObjectUUID> myValue, params ADBBaseObject[] myParameters)
        {
            _Objects.AddRange(myValue);
        }

        public override Boolean RemoveUUID(ObjectUUID myValue)
        {
            if (!(myValue is ObjectUUID))
                throw new GraphDBException(new Error_UnknownDBError("myValue is not of type ObjectUUID"));

            return _Objects.Remove((ObjectUUID)myValue);
        }

        public override Boolean RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            if (!myObjectUUIDs.IsNullOrEmpty())
            {
                _Objects.RemoveWhere(item => myObjectUUIDs.Contains(item));
                return true;
            }

            return false;
        }

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable)
        {
            var retEdge = new EdgeTypeSetOfReferences();
            foreach (var dbo in iEnumerable)
                retEdge.Add(dbo.Value.ObjectUUID);

            return retEdge;
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable)
        {
            var retEdge = new EdgeTypeSetOfReferences();
            foreach (var aUUID in iEnumerable)
                retEdge.Add(aUUID);

            return retEdge;
        }

        public override IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetEdges()
        {
            foreach (var obj in _Objects)
            {
                yield return new Tuple<ObjectUUID, ADBBaseObject>(obj, null);
            }
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
            mySerializationWriter.WriteObject(myValue._Objects.Count);
            foreach (var obj in myValue._Objects)
            {
                obj.Serialize(ref mySerializationWriter);
            }
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeSetOfReferences myValue)
        {
            var count = (Int32)mySerializationReader.ReadObject();
            for (Int32 i = 0; i < count; i++)
            {
                ObjectUUID obj = new ObjectUUID();
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

        #region IEnumerable Members

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return _Objects.GetEnumerator();
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

            if (_Objects.Count != otherEdge._Objects.Count)
            {
                return false;
            }

            foreach (var uuid in _Objects)
            {
                if (!otherEdge._Objects.Contains(uuid))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}
