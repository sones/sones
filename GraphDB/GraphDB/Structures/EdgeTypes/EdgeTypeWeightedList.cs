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

/* <id name="sones GraphDB – EdgeTypeWeightedList<T>" />
 * <copyright file="EdgeTypeWeightedList.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>
 *  This datastructure may hold a usual set of data as a weighted list. You have to pass as first parameter the DataType ('Integer', 'Double')
 *  E.g: CREATE TYPE ... (LIST<WEIGHTED(Double, DEFAULT=1.5, SORTED=DESC)<User>> Friends)
 *  
 * All other parameters are optional in number and occurence
 * </summary>
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
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This datastructure may hold a usual set of data as a weighted list
    /// </summary>
    
    public class EdgeTypeWeightedList : ASetReferenceEdgeType
    {

        public override String EdgeTypeName { get { return "WEIGHTED"; } }
        public override EdgeTypeUUID EdgeTypeUUID { get { return new EdgeTypeUUID(1001); } }

        private WeightedSet<Reference> weightedSet;
        private ADBBaseObject weightDataType = null;
        private TypeUUID _typeOfDBObjects = null;

        #region TypeCode
        public override UInt32 TypeCode { get { return 457; } }
        #endregion

#warning do not use this one
        public EdgeTypeWeightedList()
        {
            weightedSet = new WeightedSet<Reference>(SortDirection.Desc);
        }

        public EdgeTypeWeightedList(TypeUUID typeOfObjects)
        {
            weightedSet = new WeightedSet<Reference>(SortDirection.Desc);
            _typeOfDBObjects = typeOfObjects;
        }
        
        #region AEdgeType Members

        public override AEdgeType GetNewInstance()
        {
            var edgeTypeWeightedList = new EdgeTypeWeightedList(_typeOfDBObjects);
            edgeTypeWeightedList.weightedSet = new WeightedSet<Reference>(weightedSet.SortDirection);
            edgeTypeWeightedList.weightedSet.SetWeightedDefaultValue(weightedSet.DefaultWeight);

            return edgeTypeWeightedList;
        }

        public override void ApplyParams(params EdgeTypeParamNode[] myParams)
        {

            if (myParams.Count() == 0)
                throw new ArgumentException("EdgeTypeWeightedList: Expected at least 1 parameter for edge type WeightedList!");

            // The first parameter has to be the type
            if (myParams[0].Type != EdgeTypeParamNode.ParamType.PandoraType)
            {
                throw new ArgumentException("EdgeTypeWeightedList: The first parameter has to be the type 'Integer', 'Double, etc");
            }
            else
            {
                weightDataType = myParams[0].Param as ADBBaseObject;
            }

            #region Get default node if exists

            if (myParams.Any(p => p.Type == EdgeTypeParamNode.ParamType.DefaultValueDef))
            {
                var def = (from p in myParams where p.Type == EdgeTypeParamNode.ParamType.DefaultValueDef select p).First();
                weightDataType.SetValue(def.Param);
            }

            #endregion

            weightedSet.SetWeightedDefaultValue((DBNumber)weightDataType);

            #region Get sort if exists

            if (myParams.Any(p => p.Type == EdgeTypeParamNode.ParamType.Sort))
            {
                var sort = (from p in myParams where p.Type == EdgeTypeParamNode.ParamType.Sort select p).First();
                weightedSet.SetSortDirection((SortDirection)sort.Param);
            }

            #endregion
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return String.Concat(GraphQL.TERMINAL_SET, GraphQL.TERMINAL_LT, EdgeTypeName.ToUpper(), GraphQL.TERMINAL_BRACKET_LEFT, weightDataType.ObjectName, ", ", "DEFAULT=", weightDataType.Value.ToString(), ", ", weightedSet.SortDirection.ToString(), GraphQL.TERMINAL_BRACKET_RIGHT, GraphQL.TERMINAL_LT, myGraphDBType.Name, GraphQL.TERMINAL_GT, GraphQL.TERMINAL_GT);
        }

        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return String.Concat(GraphQL.TERMINAL_SET, GraphQL.TERMINAL_LT, EdgeTypeName.ToUpper(), GraphQL.TERMINAL_BRACKET_LEFT, weightDataType.ObjectName, ", ", "DEFAULT=", weightDataType.Value.ToString(), ", ", weightedSet.SortDirection.ToString(), GraphQL.TERMINAL_BRACKET_RIGHT, GraphQL.TERMINAL_LT, myGraphDBType.Name, GraphQL.TERMINAL_GT, GraphQL.TERMINAL_GT);
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

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeTypeWeightedList myValue)
        {
            mySerializationWriter.WriteObject(_typeOfDBObjects);
            myValue.weightedSet.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeWeightedList myValue)
        {
            myValue._typeOfDBObjects = (TypeUUID)mySerializationReader.ReadObject();
            myValue.weightedSet.Deserialize(ref mySerializationReader);
            return myValue;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {            
            Serialize(ref writer, (EdgeTypeWeightedList)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            EdgeTypeWeightedList thisObject = (EdgeTypeWeightedList)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        #region IEnumerable Members

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return weightedSet.GetEnumerator();
        }

        #endregion

        #region AListEdgeType Members

        public override ulong Count()
        {
            return weightedSet.Count;
        }

        public override void Clear()
        {
            weightedSet.Clear();
        }

        public override System.Collections.IEnumerable GetAll()
        {
            return weightedSet.GetAll().Select(item => new KeyValuePair<ObjectUUID, ADBBaseObject>(item.Key.ObjectUUID, item.Value));
        }

        public override System.Collections.IEnumerable GetTop(ulong myNumOfEntries)
        {
            return weightedSet.GetTop(myNumOfEntries).Select(item => item.ObjectUUID);
        }

        public override void UnionWith(AEdgeType myAEdgeType)
        {
            if (myAEdgeType is EdgeTypeWeightedList)
            {
                foreach (var e in ((EdgeTypeWeightedList)myAEdgeType).weightedSet.GetAll())
                {
                    weightedSet.Add(e.Key, e.Value);
                }
            }
            else
            {
                throw new ArgumentException("myAEdgeType is not of type EdgeTypeWeightedList");
            }
        }

        public override void Distinction()
        {
        }

        #endregion

        #region AListReferenceEdgeType Members

        public void Add(Reference myValue, DBNumber myWeight)
        {
            weightedSet.Add(myValue, myWeight);
        }

        public override void Add(ObjectUUID myValue, TypeUUID typeOfDBObjects, params ADBBaseObject[] myParameters)
        {
            if (myParameters != null && myParameters.Count() > 0 && DBNumber.IsValid(myParameters[0].Value))
            {
                Add(new Reference(myValue, typeOfDBObjects), (DBNumber)weightedSet.DefaultWeight.Clone(myParameters[0].Value));//new DBNumber(myParameters[0].Value));
            }
            else
            {
                Add(new Reference(myValue, typeOfDBObjects), weightedSet.DefaultWeight);
            }
        }

        public override IEnumerable<ObjectUUID> GetAllReferenceIDs()
        {
            return weightedSet.GetAllValues().Select(aReference => aReference.ObjectUUID);
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            foreach (var dbo in weightedSet.GetAll())
            {
                var readout = GetAllAttributesFromDBO(dbo.Key.ObjectUUID);

                yield return new DBWeightedObjectReadout(readout.Attributes, dbo.Value);
            }
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO, IEnumerable<Exceptional<DBObjectStream>> myObjectUUIDs)
        {
            foreach (var dbo in myObjectUUIDs)
            {

                if (dbo.Failed)
                {
                    throw new GraphDBException(dbo.Errors);
                }

                var lookupReference = new Reference(dbo.Value.ObjectUUID, dbo.Value.TypeUUID);

                if (weightedSet.Contains(lookupReference))
                {
                    var weight = weightedSet.Get(lookupReference);
                    var readout = GetAllAttributesFromDBO(dbo.Value.ObjectUUID);
                    yield return new DBWeightedObjectReadout(readout.Attributes, weight.Value);
                }
            }
        }

        public override ObjectUUID FirstOrDefault()
        {
            return weightedSet.GetAll().FirstOrDefault().Key.ObjectUUID;
        }

        public override void AddRange(IEnumerable<ObjectUUID> hashSet, TypeUUID typeOfDBObjects, params ADBBaseObject[] myParameters)
        {
            if (!myParameters.IsNullOrEmpty())
            {
                if (weightedSet.DefaultWeight.IsValidValue(myParameters[0].Value))
                {
                    weightedSet.AddRange(hashSet.Select(item => new Reference(item, typeOfDBObjects)), (DBNumber)weightedSet.DefaultWeight.Clone(myParameters[0].Value));
                }
                else
                {
                    throw new GraphDBException(new Error_DataTypeDoesNotMatch(myParameters[0].Value.GetType().Name, weightedSet.DefaultWeight.ObjectName));
                }
            }
            else
            {
                weightedSet.AddRange(hashSet.Select(item => new Reference(item, typeOfDBObjects)), weightedSet.DefaultWeight);
            }
        }

        public override bool Contains(ObjectUUID myValue)
        {
            return weightedSet.Contains(new Reference(myValue, null));
        }

        public override Boolean RemoveUUID(ObjectUUID myValue)
        {
            return weightedSet.Remove(new Reference(myValue, null));
        }

        public override bool RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            if (!myObjectUUIDs.IsNullOrEmpty())
            {
                weightedSet.RemoveWhere(item => myObjectUUIDs.Contains(item.ObjectUUID));
                return false;
            }

            return false;
        }

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable, TypeUUID typeOfObjects)
        {
            var retEdge = new EdgeTypeWeightedList(typeOfObjects);

            foreach (var dbo in iEnumerable)
            {
                var vals = weightedSet.Get(new Reference(dbo.Value.ObjectUUID, typeOfObjects));
                retEdge.Add(vals.Key, vals.Value);
            }

            return retEdge;
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfObjects)
        {
            var retEdge = new EdgeTypeWeightedList(typeOfObjects);

            foreach (var uuid in iEnumerable)
            {
                var vals = weightedSet.Get(new Reference(uuid, typeOfObjects)); ;
                retEdge.Add(vals.Key, vals.Value);
            }

            return retEdge;
        }

        public override IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetAllReferenceIDsWeighted()
        {
            foreach (var vals in weightedSet.GetAll())
            {
                yield return new Tuple<ObjectUUID, ADBBaseObject>(vals.Key.ObjectUUID, vals.Value);
            }

            yield break;
        }

        #endregion

        public override string ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }
        
        public DBNumber DefaultWeight
        {
            get
            {
                return weightedSet.DefaultWeight;
            }
        }

        public DBNumber GetMaxWeight()
        {
            return weightedSet.GetMaxWeight();
        }

        public ASetReferenceEdgeType GetTopAsWeightedSet(ulong myNumOfEntries)
        {
            EdgeTypeWeightedList edge = (EdgeTypeWeightedList)GetNewInstance();
            edge.weightedSet = weightedSet.GetTopAsWeightedSet(myNumOfEntries);
            return edge;
        }

        #region Equals

        public override bool Equals(object obj)
        {
            var otherEdge = obj as EdgeTypeWeightedList;
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

            if (!DefaultWeight.Equals(otherEdge.DefaultWeight))
            {
                return false;
            }
            if (weightDataType == null || otherEdge.weightDataType == null)
            {
                return weightDataType == otherEdge.weightDataType;
            }
            else if (!weightDataType.Equals(otherEdge.weightDataType))
            {
                return false;
            }

            if (weightedSet == null || otherEdge.weightedSet == null)
            {
                return weightedSet == otherEdge.weightedSet;
            }
            else if (!weightedSet.Equals(otherEdge.weightedSet))
            {
                return false;
            }

            return true;
        }

        #endregion

        public override IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache)
        {
            foreach (var aReference in weightedSet.GetAll())
            {
                yield return aReference.Key.GetDBObjectStream(dbObjectCache);
            }

            yield break;
        }

        public override IEnumerable<Tuple<Exceptional<DBObjectStream>, ADBBaseObject>> GetAllEdgeDestinationsWeighted(DBObjectCache dbObjectCache)
        {
            foreach (var vals in weightedSet.GetAll())
            {
                yield return new Tuple<Exceptional<DBObjectStream>, ADBBaseObject>(vals.Key.GetDBObjectStream(dbObjectCache), vals.Value);
            }

            yield break;
        }

        public override TypeUUID GetTypeUUIDOfReferences()
        {
            return _typeOfDBObjects;
        }
    }
}
