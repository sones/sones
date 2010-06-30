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
using System.Text;
using sones.Lib.Serializer;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.DataStructures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Exceptions;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.ObjectManagement;
using sones.Lib.DataStructures.UUID;
using sones.GraphDB.Errors;
using sones.Lib;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This datastructure may hold a usual set of data as a weighted list
    /// </summary>
    
    public class EdgeTypeWeightedList : ASetReferenceEdgeType
    {

        public override String EdgeTypeName { get { return "WEIGHTED"; } }
        public override EdgeTypeUUID EdgeTypeUUID { get { return new EdgeTypeUUID(1001); } }

        private WeightedSet<ObjectUUID> weightedSet;
        private ADBBaseObject weightDataType = null;

        #region TypeCode
        public override UInt32 TypeCode { get { return 457; } }
        #endregion

        public EdgeTypeWeightedList()
        {
            weightedSet = new WeightedSet<ObjectUUID>(SortDirection.Desc);
        }
        
        #region AEdgeType Members

        public override AEdgeType GetNewInstance()
        {
            var edgeTypeWeightedList = new EdgeTypeWeightedList();
            edgeTypeWeightedList.weightedSet = new WeightedSet<ObjectUUID>(weightedSet.SortDirection);
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
            myValue.weightedSet.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeWeightedList myValue)
        {
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
            return weightedSet.GetAll();
        }

        public override System.Collections.IEnumerable GetTop(ulong myNumOfEntries)
        {
            return weightedSet.GetTop(myNumOfEntries);
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
                throw new ArgumentException("myAEdgeType is not of type EdgeTypeWeightedList");
        }

        public override void Distinction()
        {
        }

        #endregion

        #region AListReferenceEdgeType Members

        public void Add(ObjectUUID myValue, DBNumber myWeight)
        {
            if (!(myValue is ObjectUUID))
                throw new ArgumentException("myValue is not of type ObjectUUID");
            weightedSet.Add((ObjectUUID)myValue, myWeight);
        }

        public void Add(IEnumerable<ObjectUUID> myValues, DBNumber myWeight)
        {
            foreach (var val in myValues)
                weightedSet.Add(val, myWeight);
        }

        public override void Add(ObjectUUID myValue, params ADBBaseObject[] myParameters)
        {
            if (myParameters != null && myParameters.Count() > 0 && DBNumber.IsValid(myParameters[0].Value))
            {
                Add(myValue, (DBNumber)weightedSet.DefaultWeight.Clone(myParameters[0].Value));//new DBNumber(myParameters[0].Value));
            }
            else
            {
                Add(myValue, weightedSet.DefaultWeight);
            }
        }

        public override void Add(IEnumerable<ObjectUUID> myValue, params ADBBaseObject[] myParameters)
        {
            if (myParameters != null && myParameters.Count() > 0 && DBNumber.IsValid(myParameters[0].Value))
            {
                Add(myValue, (DBNumber)weightedSet.DefaultWeight.Clone(myParameters[0].Value));//new DBNumber(myParameters[0].Value));
            }
            else
            {
                Add(myValue, weightedSet.DefaultWeight);
            }
        }

        public override IEnumerable<ObjectUUID> GetAllUUIDs()
        {
            return weightedSet.GetAllValues();
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            foreach (var dbo in weightedSet.GetAll())
            {
                var readout = GetAllAttributesFromDBO(dbo.Key);

                yield return new DBWeightedObjectReadout(readout.Attributes, dbo.Value);
            }
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO, IEnumerable<Exceptional<DBObjectStream>> myObjectUUIDs)
        {
            foreach (var dbo in myObjectUUIDs)
            {

                if (dbo.Failed)
                    throw new GraphDBException(dbo.Errors);

                if (!weightedSet.Contains(dbo.Value.ObjectUUID))
                {
                    /* If we had a function like TOP(1) the GetAllAttributesFromDBO might not contains elements which are in myObjectUUIDs (which is coming from the where)*/
                    //throw new Exception("Fatal error during EdgeTypeWeightedList.GetReadouts - the ObjectUUID does not have an edge!");
                }
                else
                {
                    var weight = weightedSet.Get(dbo.Value.ObjectUUID);
                    var readout = GetAllAttributesFromDBO(dbo.Value.ObjectUUID);
                    yield return new DBWeightedObjectReadout(readout.Attributes, weight.Value);
                }
            }
        }

        public override void RemoveWhere(Predicate<ObjectUUID> match)
        {
            weightedSet.RemoveWhere(match);
        }

        public override ObjectUUID FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public override void AddRange(IEnumerable<ObjectUUID> hashSet, params ADBBaseObject[] myParameters)
        {
            if (!myParameters.IsNullOrEmpty())
            {
                if(weightedSet.DefaultWeight.IsValidValue(myParameters[0].Value))
                    weightedSet.AddRange(hashSet, (DBNumber)weightedSet.DefaultWeight.Clone(myParameters[0].Value));
                else
                    throw new GraphDBException(new Error_DataTypeDoesNotMatch(myParameters[0].Value.GetType().Name, weightedSet.DefaultWeight.ObjectName));
            }
            else
            {
                weightedSet.AddRange(hashSet, weightedSet.DefaultWeight);
            }
        }

        public override bool Contains(ObjectUUID myValue)
        {
            if (!(myValue is ObjectUUID))
                throw new ArgumentException("myValue is not of type ObjectUUID");

            return weightedSet.Contains((ObjectUUID)myValue);
        }

        public override Boolean RemoveUUID(ObjectUUID myValue)
        {
            if (!(myValue is ObjectUUID))
                throw new ArgumentException("myValue is not of type ObjectUUID");

            return weightedSet.Remove((ObjectUUID)myValue);
        }

        public override bool RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            if (!myObjectUUIDs.IsNullOrEmpty())
            {
                weightedSet.RemoveWhere(item => myObjectUUIDs.Contains(item));
                return false;
            }

            return false;
        }

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable)
        {
            var retEdge = new EdgeTypeWeightedList();

            foreach (var dbo in iEnumerable)
            {
                var vals = weightedSet.Get(dbo.Value.ObjectUUID);;
                retEdge.Add(vals.Key, vals.Value);
            }

            return retEdge;
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable)
        {
            var retEdge = new EdgeTypeWeightedList();

            foreach (var uuid in iEnumerable)
            {
                var vals = weightedSet.Get(uuid); ;
                retEdge.Add(vals.Key, vals.Value);
            }

            return retEdge;
        }

        public override IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetEdges()
        {
            foreach (var vals in weightedSet.GetAll())
            {
                yield return new Tuple<ObjectUUID, ADBBaseObject>(vals.Key, vals.Value);
            }
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
    }
}
