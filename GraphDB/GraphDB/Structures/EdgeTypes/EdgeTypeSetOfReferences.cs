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
using System.Collections;
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
using System.Diagnostics;

namespace sones.GraphDB.Structures.EdgeTypes
{
    
    public class EdgeTypeSetOfReferences : ASetReferenceEdgeType
    {
        private Dictionary<ObjectUUID, Reference> _ObjectUUIDs = null;
        private TypeUUID _typeOfDBObjects;

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
            _typeOfDBObjects = typeOfDBObjects;

            if (dbos != null)
            {
                foreach (var aUUID in dbos)
                {
                    _ObjectUUIDs.Add(aUUID, new Reference(aUUID, typeOfDBObjects));
                }
            }
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
            return _ObjectUUIDs.ULongCount();
        }

        public override IEnumerable GetAll()
        {
            return new HashSet<ObjectUUID>(_ObjectUUIDs.Select(item => item.Key));
        }

        public override IEnumerable GetTop(ulong myNumOfEntries)
        {
            return _ObjectUUIDs.Take((Int32)myNumOfEntries).Select(item => item.Key);
        }

        public override void Clear()
        {
            _ObjectUUIDs.Clear();
        }

        public override void UnionWith(AEdgeType myAEdgeType)
        {
            var anotherEdge = myAEdgeType as ASetReferenceEdgeType;

            if (anotherEdge != null)
            {
                foreach (var aUUID in anotherEdge.GetAllReferenceIDs())
                {
                    if (!_ObjectUUIDs.ContainsKey(aUUID))
                    {
                        _ObjectUUIDs.Add(aUUID, new Reference(aUUID, _typeOfDBObjects));
                    }
                }
            }
            else
            {
                throw new ArgumentException("myValue is not of type ObjectUUID");
            }
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

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            foreach (var dbo in _ObjectUUIDs)
            {
                yield return GetAllAttributesFromDBO(dbo.Key);
            }

            yield break;
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO, IEnumerable<Exceptional<DBObjectStream>> myObjectUUIDs)
        {
            foreach (var dbo in myObjectUUIDs)
            {

                if (dbo.Failed)
                    throw new GraphDBException(dbo.Errors);
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
            if (_typeOfDBObjects == null)
            {
                _typeOfDBObjects = typeOfDBObjects;
            }

            foreach (var aUUID in hashSet)
            {
                if (!_ObjectUUIDs.ContainsKey(aUUID))
                {
                    _ObjectUUIDs.Add(aUUID, new Reference(aUUID, typeOfDBObjects));
                }
            }
        }

        public override void Add(ObjectUUID myValue, TypeUUID typeOfDBObjects, params ADBBaseObject[] myParameters)
        {
            if (_typeOfDBObjects == null)
            {
                _typeOfDBObjects = typeOfDBObjects;
            }

            if (!_ObjectUUIDs.ContainsKey(myValue))
            {
                _ObjectUUIDs.Add(myValue, new Reference(myValue, typeOfDBObjects));
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

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable, TypeUUID typeOfObjects)
        {
            return new EdgeTypeSetOfReferences(iEnumerable.Select(aDBO => aDBO.Value.ObjectUUID), typeOfObjects);
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfObjects)
        {
            return new EdgeTypeSetOfReferences(iEnumerable, typeOfObjects);
        }

        public override IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetAllReferenceIDsWeighted()
        {
            return _ObjectUUIDs.Select(aKV => new Tuple<ObjectUUID, ADBBaseObject>(aKV.Key, null));
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
            mySerializationWriter.WriteObject(myValue._typeOfDBObjects);
            mySerializationWriter.WriteInt32(myValue._ObjectUUIDs.Count);
            foreach (var obj in myValue._ObjectUUIDs)
            {
                obj.Key.Serialize(ref mySerializationWriter);
            }
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeSetOfReferences myValue)
        {
            myValue._typeOfDBObjects = (TypeUUID)mySerializationReader.ReadObject();
            var count = (Int32)mySerializationReader.ReadInt32();
            for (Int32 i = 0; i < count; i++)
            {
                ObjectUUID obj = new ObjectUUID();
                obj.Deserialize(ref mySerializationReader);
                myValue._ObjectUUIDs.Add(obj, new Reference(obj, myValue._typeOfDBObjects));
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
            return _ObjectUUIDs.GetEnumerator();
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


        public override IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache)
        {
            return _ObjectUUIDs.Select(kv => kv.Value.GetDBObjectStream(dbObjectCache));
        }

        public override IEnumerable<Tuple<Exceptional<DBObjectStream>, ADBBaseObject>> GetAllEdgeDestinationsWeighted(DBObjectCache dbObjectCache)
        {
            return _ObjectUUIDs.Select(kv => new Tuple<Exceptional<DBObjectStream>, ADBBaseObject>(kv.Value.GetDBObjectStream(dbObjectCache), null));
        }

        public override TypeUUID GetTypeUUIDOfReferences()
        {
            return _typeOfDBObjects;
        }
    }
}
