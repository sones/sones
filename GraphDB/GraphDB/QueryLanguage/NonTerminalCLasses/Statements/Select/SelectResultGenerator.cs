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

///* <id name="PandoraDB – SelectResultGenerator" />
// * <copyright file="SelectResultGenerator.cs"
// *            company="sones GmbH">
// * Copyright (c) sones GmbH. All rights reserved.
// * </copyright>
// * <developer>Stefan Licht</developer>
// * <summary>This will do all evaluations and calculations on selected DB objects. Just create an instance, examine some DB objects and get the result.</summary>
// */

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using sones.GraphDB.ObjectManagement;
//using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
//using sones.GraphDB.QueryLanguage.Result;
//using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
//using sones.GraphDB.TypeManagement;

//using sones.GraphDB.TypeManagement.PandoraTypes;
//using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;

//using sones.GraphDB.Settings;
//using sones.GraphDB.Structures;
//using sones.GraphDB.Structures.EdgeTypes;
//using sones.GraphDB.QueryLanguage.Enums;
//using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select;
//using sones.Lib.ErrorHandling;
//using sones.GraphDB.Exceptions;
//using sones.Lib.DataStructures.UUID;
//using sones.GraphDB.Errors;
//using sones.GraphFS.DataStructures;
//using sones.GraphFS.Session;
//using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
//using sones.Lib.Session;

//namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements
//{

//    public class GroupingKey : IComparable<Dictionary<AttributeUUID, ADBBaseObject>>
//    {
//        Dictionary<AttributeUUID, ADBBaseObject> _Values;
//        Dictionary<AttributeUUID, PandoraTypeInformation> _GroupingAttr;

//        public Dictionary<AttributeUUID, ADBBaseObject> Values
//        {
//            get { return _Values; }
//            set { _Values = value; }
//        }

//        public GroupingKey(Dictionary<AttributeUUID, ADBBaseObject> myValues, Dictionary<AttributeUUID, PandoraTypeInformation> myGroupingAttr)
//        {
//            _Values = myValues;
//            _GroupingAttr = myGroupingAttr;
//        }

//        #region IComparable<ADBBaseObject> Members

//        public int CompareTo(Dictionary<AttributeUUID, ADBBaseObject> other)
//        {

//            // it could happen, that not all grouped aatributes are existing in all DBOs, so use the group by from the select to check
//            foreach (AttributeUUID attr in _GroupingAttr.Keys)
//            {
//                if (_Values.ContainsKey(attr) && other.ContainsKey(attr))
//                    if (_Values[attr].CompareTo(other[attr]) != 0)
//                        return -1;
//            }

//            return 0;
//        }

//        #endregion

//        public override int GetHashCode()
//        {
//            Int32 retVal = -1;
//            foreach (KeyValuePair<AttributeUUID, ADBBaseObject> keyValPair in _Values)
//            {
//                if (retVal == -1)
//                    retVal = keyValPair.Value.Value.GetHashCode();
//                else
//                    retVal = retVal ^ keyValPair.Value.Value.GetHashCode();
//            }

//            return retVal;
//        }

//        public override bool Equals(object obj)
//        {
//            return CompareTo(((GroupingKey)obj).Values) == 0;
//        }
//    }

//    public class GroupingStructure
//    {

//        private Dictionary<AttributeUUID, PandoraTypeInformation> _GroupingAttr;

//        // The attribute value of each Group BY, Set of DBOs which have exactly these values
//        private Dictionary<GroupingKey, HashSet<DBObjectReadout>> _GroupsAndAggregates;
//        public Dictionary<GroupingKey, HashSet<DBObjectReadout>> GroupsAndAggregates
//        {
//            get { return _GroupsAndAggregates; }
//            set { _GroupsAndAggregates = value; }
//        }

//        public GraphDBType DBTypeStream
//        {
//            get;
//            set; 
//        }

//        public GroupingStructure(Dictionary<AttributeUUID, PandoraTypeInformation> myGroupingAttr)
//        {
//            _GroupsAndAggregates = new Dictionary<GroupingKey, HashSet<DBObjectReadout>>();
//            _GroupingAttr = myGroupingAttr;
//        }

//        public void Add(DBObjectReadout myDBObjectReadout, Dictionary<AttributeUUID, ADBBaseObject> myValues)
//        {
//            GroupingKey curKey = new GroupingKey(myValues, _GroupingAttr);
//            if (!_GroupsAndAggregates.ContainsKey(curKey))
//            {
//                _GroupsAndAggregates.Add(curKey, new HashSet<DBObjectReadout>());
//            }
//            _GroupsAndAggregates[curKey].Add(myDBObjectReadout);
//        }
//    }

//    public class SelectResultGenerator : ISelect
//    {

//        /// <summary>
//        /// The selected attributes, including Aggregates and single selects
//        /// </summary>
//        private InterestingAttributes _InterestingAttributes;

//        /// <summary>
//        /// The Hashset of the result DB objects, will be filled during ExamineDBO and GetResult (for grouping and aggregates)
//        /// </summary>
//        private HashSet<DBObjectReadout> _DBOs;
//        private BinaryExpressionNode _HavingExpression;
//        private GraphDBType _TypeOfDBObject;

//        private GroupingStructure _Groupings;

//        private SessionInfos _SessionToken;

//        private SelectSettingCache _SettingCache;

//        private IDBObjectCache _IDBObjectCache;

//        /// <summary>
//        /// Initialize the SelectResultGenerator with all information about a select.
//        /// </summary>
//        /// <param name="myTypeOfDBObject">The Type of the selected attributes</param>
//        /// <param name="myInterestingAttributes">The interesting attributes of the select</param>
//        /// <param name="myFuncCallNodes">All functions of the select</param>
//        /// <param name="myAggregateNodes">All aggregate nodes</param>
//        /// <param name="myGroupingAttr">All grouping attributes of GROUP BY</param>
//        /// <param name="myHavingExpression">The having expressions</param>
//        public SelectResultGenerator(GraphDBType myTypeOfDBObject, InterestingAttributes myInterestingAttributes, BinaryExpressionNode myHavingExpression, SessionInfos mySessionToken, IDBObjectCache myIDBObjectCache, SelectSettingCache mySettingCache)
//        {
//            _InterestingAttributes = myInterestingAttributes;
//            _DBOs = new HashSet<DBObjectReadout>();
//            _TypeOfDBObject = myTypeOfDBObject;
//            _HavingExpression = myHavingExpression;
//            _SessionToken = mySessionToken;
//            _IDBObjectCache = myIDBObjectCache;
//            _SettingCache = mySettingCache;

//        }

//        public Exceptional<bool> InitGroupingOrAggregate(TypeManager myTypeManager)
//        {
//            #region if we have grouping or aggregate the must be NO attribute in the select list which is not covert by an aggregate or grouping
            
//            #region Store the List of aggregated uuids

//            List<UUID> _AggregateAttr = new List<UUID>();
//            if (_InterestingAttributes.AggregateNodes != null && _InterestingAttributes.AggregateNodes.Count > 0)
//            {
//                if (_InterestingAttributes.InterestingAttribts != null && _InterestingAttributes.InterestingAttribts.Count > 0)
//                {
//                    if (_InterestingAttributes.GroupNodes == null || _InterestingAttributes.GroupNodes.Count == 0)
//                    {
//                        return new Exceptional<bool>(new Error_NoGroupingArgument());
//                    }
//                }                
                
//                foreach (InterestingAttribute attr in _InterestingAttributes.AggregateNodes)
//                {
//                    AggregateNode aggrNode = (AggregateNode)attr.InterestingNode;
//                    // We checked prio in GetContent of SelectNode that we have exactly one expression
//                    IDNode aAggrIDNode = aggrNode.Expressions[0] as IDNode;

//                    if (aAggrIDNode == null)
//                    {
//                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
//                    }
//                    else
//                    {
//                        _AggregateAttr.Add(aAggrIDNode.LastAttribute.UUID);
//                    }
//                }
//            }

//            #endregion            

//            if (_InterestingAttributes.GroupNodes != null && _InterestingAttributes.GroupNodes.Count > 0)
//            {

//                Dictionary<AttributeUUID, PandoraTypeInformation> groupingAttr = new Dictionary<AttributeUUID, PandoraTypeInformation>();
//                foreach (InterestingAttribute intAttr in _InterestingAttributes.GroupNodes)
//                {
//                    groupingAttr.Add(((IDNode)intAttr.AttributeIDNode).LastAttribute.UUID, new PandoraTypeInformation(((IDNode)intAttr.AttributeIDNode)));
//                    break;
//                }
//                _Groupings = new GroupingStructure(groupingAttr);

//                #region Check that eacht selected attribute is either is aggregated or grouped

//                foreach (InterestingAttribute intAttr in _InterestingAttributes.InterestingAttribts)
//                {
//                    if (!groupingAttr.ContainsKey(((IDNode)intAttr.AttributeIDNode).LastAttribute.UUID))
//                    {
//                        return new Exceptional<bool>(new Error_InvalidAttributeSelection("Attribute '" + intAttr.AttributeName + "' is invalid in the select list because it is not contained in either an aggregate function or the GROUP BY clause."));
//                    }
//                }

//                #endregion
//            }
//            #endregion

//            return new Exceptional<bool>();
//        }
        
//        /// <summary>
//        /// Examines a DBObject, execute the functions, resolve all reference attributes to a specified <paramref name="myDepth"/> and store the values in a DBReadout object.
//        /// All aggregate and grouping calculations will be prepared. You need to call <see cref="GetResult"/> to get the final result!
//        /// </summary>
//        /// <param name="myDBObject">The DBO to be examined</param>
//        /// <param name="myDepth">The depth to resolve all reference types</param>
//        /// <param name="myTypeOfDBObject">The type of the DB object</param>
//        /// <param name="myReference">The reference of the type defined in the FROM</param>
//        /// <param name="myTypeManager">The TypeManager reference</param>
//        /// <param name="myRouteToDBObjects">The route to the DB object attribute</param>
//        public Exceptional<ResultType> ExamineDBO(DBObjectStream myDBObject, Int64 myDepth, GraphDBType myTypeOfDBObject, String myReference, TypeManager myTypeManager, RouteToDBObjects myRouteToDBObjects, SessionInfos myToken, IDBObjectCache myDBObjectCache)
//        {

//            Dictionary<string, object>  Attributes = new Dictionary<string, object>();
//            Int64 Depth = 0;

//            #region Fill up function evaluated attributes

//            if (_InterestingAttributes.FuncCallNode != null && _InterestingAttributes.FuncCallNode.Count > 0)
//            {
//                foreach (InterestingAttribute intAttr in _InterestingAttributes.FuncCallNode)
//                {
//                    FuncCallNode fcn = ((FuncCallNode)intAttr.InterestingNode);
//                    fcn.CallingAttribute = null;
//                    var res = fcn.Execute(myTypeOfDBObject, myDBObject, myReference, myTypeManager, myTypeOfDBObject, myDBObjectCache, myToken);
//                    if (res.Success)
//                    {
//                        if (res.Value.Value == null)
//                            continue; // no result for this object because of not set attribute value

//                        if (fcn.Function.TypeOfResult == TypesOfOperatorResult.ListOfDBObjects)
//                        {
//                            #region Resolve DBReferences

//                            if (myDepth > -1)
//                                Depth = myDepth;    
//                            else
//                                Depth = intAttr.Depth;
                            
//                            Attributes.Add(fcn.Alias, 
//                                GetAttributeValue(
//                                    myTypeOfDBObject, 
//                                    ((FuncParameter)res.Value).TypeAttribute, 
//                                    ((FuncParameter)res.Value).TypeAttribute.Name, 
//                                    ((FuncParameter)res.Value).Value, 
//                                    Depth, myRouteToDBObjects.Route[myReference], 0, myTypeManager, myToken, myDBObjectCache));

//                            #endregion
//                        }
//                        else if (fcn.Function.TypeOfResult == TypesOfOperatorResult.NotABasicType)
//                        {
//                            #region Resolve DBReferences

//                            if (myDepth > -1)
//                                Depth = myDepth;
//                            else
//                                Depth = intAttr.Depth;

//                            IEnumerable<DBObjectReadout> readOutPath = new List<DBObjectReadout>();

//                            if (((FuncParameter)res.Value).Value is ASetReferenceEdgeType)
//                            {
//                                readOutPath = (((FuncParameter)res.Value).Value as ASetReferenceEdgeType).GetReadouts(a =>
//                                { 
//                                    var dbo = myDBObjectCache.LoadDBObjectStream(myTypeOfDBObject, a);
//                                    if (dbo.Failed)
//                                    {
//                                        throw new NotImplementedException();
//                                    }

//                                    return new DBObjectReadout(GetAllAttributesFromDBO(myTypeManager, dbo.Value, myTypeOfDBObject, myDepth, myRouteToDBObjects.Route[myReference], 0, myToken, myDBObjectCache));
//                                });
//                            }
//                            /*
//                            var paths = (((FuncParameter)res.Value).Value as IEnumerable<List<ObjectUUID>>);

//                            var readOutPath = new DBObjectReadoutPath(myDBObject, myTypeOfDBObject);

//                            foreach (var path in paths)
//                            {
//                                var readoutList = new IEnumerable<DBObjectReadout>();
//                                foreach (var dbo in myDBObjectCache.LoadListOfDBObjectStreams(myTypeOfDBObject, path))
//                                {
//                                    readoutList.Add(new DBObjectReadout(dbo.GetAttributes(), new DBObjectSystemData(dbo), myTypeOfDBObject));
//                                }
//                                readOutPath.DBObjectPaths.Add(readoutList);
//                            }
//                            */
//                            Attributes.Add(fcn.Alias, readOutPath);

//                            #endregion
//                        }
//                        else
//                        {
//                            Attributes.Add(fcn.Alias, ((FuncParameter)res.Value).Value);
//                        }
//                    }
//                    else
//                    {
//                        return new Exceptional<ResultType>(ResultType.Failed);
//                    }

//                }
//            }

//            #endregion

//            #region AggreateNodesOfListReferences

//            if (_InterestingAttributes.AggreateNodesOfListReferences != null && _InterestingAttributes.AggreateNodesOfListReferences.Count > 0)
//            {
//                foreach (InterestingAttribute intAttr in _InterestingAttributes.AggreateNodesOfListReferences)
//                {                    
//                    if (myDBObject.HasAttribute(intAttr.AttributeUUID, myTypeOfDBObject, myToken))
//                    {

//                        if (myDepth > -1)
//                            Depth = myDepth;
//                        else
//                            Depth = intAttr.Depth;

//                        Exceptional<Object> res = new Exceptional<object>();
//                        if(intAttr.AttributeIDNode.LastAttribute.KindOfType == KindsOfType.SetOfReferences)
//                        {
//                            res = ((AggregateNode)intAttr.InterestingNode).Aggregate.Aggregate((AListEdgeType)myDBObject.GetAttribute(intAttr.AttributeUUID, myTypeOfDBObject, myToken, myTypeManager), intAttr.AttributeIDNode.LastAttribute, myTypeManager, myDBObjectCache, myToken);
//                        }
//                        else
//                        {
//                            res = ((AggregateNode)intAttr.InterestingNode).Aggregate.Aggregate(
//                            GetAttributeValue(
//                                intAttr.AttributeIDNode.LastAttribute.DBType, 
//                                intAttr.AttributeIDNode.LastAttribute, 
//                                intAttr.AttributeName, 
//                                myDBObject.GetAttribute(intAttr.AttributeUUID, myTypeOfDBObject, myToken, myTypeManager), 
//                                Depth, myRouteToDBObjects.Route[myReference], 1, myTypeManager, myToken, myDBObjectCache)
//                            , intAttr.AttributeIDNode.LastAttribute, myTypeManager, myDBObjectCache, myToken);
//                        }

//                        if (res.Success)
//                        {
//                            Attributes.Add(intAttr.Alias, res.Value);
//                        }
//                        else
//                        {
//                            return new Exceptional<ResultType>(res);
//                        }
//                    }
//                    else
//                    {
//                        #region Check for BackwardEdge

//                        var contExcept = myDBObject.ContainsBackwardEdge(myTypeOfDBObject.GetTypeAttributeByName(intAttr.AttributeName).BackwardEdgeDefinition, myTypeManager, myDBObjectCache, myTypeOfDBObject);

//                        if (contExcept.Failed)
//                            return new Exceptional<ResultType>(contExcept);
                        
//                        if (myTypeOfDBObject.GetTypeAttributeByName(intAttr.AttributeName).TypeCharacteristics.IsBackwardEdge
//                            && contExcept.Value)
//                        {
//                            EdgeKey edgeKey = myTypeOfDBObject.GetTypeAttributeByName(intAttr.AttributeName).BackwardEdgeDefinition;
//                            TypeAttribute typeAttribute = myTypeOfDBObject.GetTypeAttributeByName(intAttr.AttributeName);
//                            var be = myDBObject.GetBackwardEdges(myTypeOfDBObject.GetTypeAttributeByName(intAttr.AttributeName).BackwardEdgeDefinition, myTypeManager, typeAttribute, myDBObjectCache);

//                            if (be.Failed)
//                                return new Exceptional<ResultType>(be);

//                            var res = ((AggregateNode)intAttr.InterestingNode).Aggregate.Aggregate(
//                                 GetAttributeValue(myTypeManager.GetTypeByUUID(edgeKey.TypeUUID), typeAttribute, typeAttribute.Name, be.Value, myDepth, myRouteToDBObjects.Route[myReference], 1, myTypeManager, myToken, myDBObjectCache)
//                                 , intAttr.AttributeIDNode.LastAttribute, myTypeManager, myDBObjectCache, myToken);
//                            if (res.Success)
//                            {
//                                Attributes.Add(intAttr.Alias, res.Value);
//                            }
//                            else
//                            {
//                                return new Exceptional<ResultType>(res);
//                            }
//                        }

//                        #endregion
//                    }
//                }
//            }

//            #endregion

//            #region AggregateNodes - will be handled the same way like A usual selected Attribute (IDNode)

//            if (_InterestingAttributes.AggregateNodes != null && _InterestingAttributes.AggregateNodes.Count > 0)
//            {
//                foreach (InterestingAttribute intAttr in _InterestingAttributes.AggregateNodes)
//                {
//                    if (myDepth > -1)
//                        Depth = myDepth;
//                    else
//                        Depth = intAttr.Depth;

//                    AddAttribute(ref Attributes, intAttr.AttributeName, intAttr.AttributeName, myDBObject, intAttr.AttributeIDNode, myTypeManager, Depth, myRouteToDBObjects, myReference, myToken, myDBObjectCache);
//                }
//            }

//            #endregion

//            #region A usual selected Attribute (IDNode)

//            if (_InterestingAttributes.InterestingAttribts != null && _InterestingAttributes.InterestingAttribts.Count > 0)
//            {
//                foreach (InterestingAttribute intAttr in _InterestingAttributes.InterestingAttribts)
//                {
//                    if (myDepth > -1)
//                        Depth = myDepth;
//                    else
//                        Depth = intAttr.Depth;
//                    AddAttribute(ref Attributes, intAttr.AttributeName, intAttr.Alias, myDBObject, intAttr.AttributeIDNode, myTypeManager, Depth, myRouteToDBObjects, myReference, myToken, myDBObjectCache);
//                }
//            }

//            #endregion

//            #region Asterisk: SELECT *

//            if (_InterestingAttributes.IsAsteriskSelection) // SELECT *
//            {

//                #region UUID
//                var uuidSetting = myTypeOfDBObject.GetSettingValue(new SettingShowUUID().Name, null);
//                Object uuid = null;
//                if (myTypeOfDBObject.GetSettingValue(new SettingShowUUID().Name, null) != null && (myTypeOfDBObject.GetSettingValue(new SettingShowUUID().Name, null) as ADBShowSetting).IsShown)
//                {
//                    //if we have a SettingUUID, we have to find out in which way 
//                    //we have to create the objectUUID

//                    String settingEncoding = (String)myTypeOfDBObject.GetSettingValue(DBConstants.SettingUUIDEncoding, myToken).Value.Value;

//                    if (settingEncoding == null)
//                    {
//                        throw new GraphDBException(new Error_ArgumentNullOrEmpty("settingEncoding"));
//                    }

//                    //uuid = (new SettingShowUUID()).DoExtract(myDBObject, settingEncoding);
//                }


//                if (uuid != null)
//                {
//                    Attributes.Add(uuidSetting.Name, uuid);
//                }

//                #endregion

//                foreach(var type in myTypeManager.GetAllParentTypes(myTypeOfDBObject, true, false))
//                {
//                    foreach (KeyValuePair<AttributeUUID, TypeAttribute> aSelectedAttribute in type.Attributes)
//                    {
//                        if (myDepth > -1)
//                            Depth = myDepth;
//                        else
//                            Depth = (Int64)_SettingCache.GetValue(aSelectedAttribute.Value.RelatedPandoraTypeUUID, aSelectedAttribute.Value.UUID, "DEPTH", _SessionToken).Value;

//                        AddAttribute(ref Attributes, myTypeOfDBObject, aSelectedAttribute.Value, aSelectedAttribute.Value.Name, myDBObject, myTypeManager, Depth, myRouteToDBObjects, myReference, myToken, myDBObjectCache);
//                    }
//                }

//            }

//            #endregion

//            #region Grouping : add attributes to the special _Groupings GroupingStructure

//            if (_InterestingAttributes.GroupNodes != null && _InterestingAttributes.GroupNodes.Count > 0)
//            {

//                var groupingVals = new Dictionary<AttributeUUID, ADBBaseObject>();

//                foreach (var _InterestingAttribute in _InterestingAttributes.GroupNodes)
//                {
//                    if (myDepth > -1)
//                        Depth = myDepth;
//                    else
//                        Depth = _InterestingAttribute.Depth;
//                    // some grouping

//                    if (myDBObject.HasAttribute(_InterestingAttribute.AttributeUUID, myTypeOfDBObject, myToken))
//                    {
//                        groupingVals.Add(_InterestingAttribute.AttributeIDNode.LastAttribute.UUID,
//                            PandoraTypeMapper.GetPandoraObjectFromTypeName(
//                            _InterestingAttribute.AttributeIDNode.LastAttribute.DBType.Name,
//                            GetAttributeValue(myTypeOfDBObject, _InterestingAttribute.AttributeIDNode.LastAttribute,
//                            _InterestingAttribute.AttributeName,
//                            myDBObject.GetAttribute(_InterestingAttribute.AttributeUUID, myTypeOfDBObject, myToken, myTypeManager),
//                            Depth, myRouteToDBObjects.Route[myReference],
//                            0, myTypeManager, myToken, myDBObjectCache)
//                            ));
//                    }
//                }

//                _Groupings.Add(new DBObjectReadout(myDBObject.GetAttributes(), new DBObjectSystemData(myDBObject), myTypeOfDBObject), groupingVals);

//            }

//            #endregion

//            #region Add the attributes to the DBOs if we have no grouping

//            if (_InterestingAttributes.GroupNodes == null || _InterestingAttributes.GroupNodes.Count == 0)
//            {
//                DBObjectReadout tempROObject = new DBObjectReadout(Attributes, new DBObjectSystemData(myDBObject));
//                _DBOs.Add(tempROObject);
//            }

//            #endregion

//            return new Exceptional<ResultType>(ResultType.Successful);

//        }

//        private void AddAttribute(ref Dictionary<string, object> myAttributes, GraphDBType myType, ADBShowSetting mySetting, string myAlias, DBObjectStream myDBObject, TypeManager myTypeManager, Int64 myDepth, RouteToDBObjects myRouteToDBObjects, string myReference, SessionInfos myToken)
//        {
//            if (myDBObject.HasAttribute(new AttributeUUID(mySetting.ID), myType, myToken))
//            {
//                if (mySetting is SettingShowUUID)
//                {
//                    //if we have a SettingUUID, we have to find out in which way 
//                    //we have to create the objectUUID

//                    String settingEncoding = (String)myType.GetSettingValue(DBConstants.SettingUUIDEncoding, myToken).Value.Value;

//                    if (settingEncoding == null)
//                    {
//                        throw new GraphDBException(new Error_ArgumentNullOrEmpty("settingEncoding"));
//                    }

//                    //myAttributes.Add(mySetting.Name, mySetting.DoExtract(myDBObject, settingEncoding));
//                }
//                else
//                {
//                    //myAttributes.Add(mySetting.Name, mySetting.DoExtract(myDBObject));
//                }
//            }
//         }
        
        
//        /// <summary>
//        /// Adds an attribute with its values (to the defined depth)
//        /// </summary>
//        /// <param name="myAttributes">The referenced Attribute dictionary where the attribute will be added</param>
//        /// <param name="myType">The Type</param>
//        /// <param name="myTypeAttribute">The Type of the attribute</param>
//        /// <param name="myAlias">The alias - this will be the key of the myAttributes dictionary </param>
//        /// <param name="myDBObject">The DBO containing the values</param>
//        /// <param name="myTypeManager">The TypeManager reference</param>
//        /// <param name="myDepth">The depth to resolve references</param>
//        /// <param name="myRouteToDBObjects">The RouteToDBObjects</param>
//        /// <param name="myReference"></param>
//        private void AddAttribute(ref Dictionary<string, object> myAttributes, GraphDBType myType, TypeAttribute myTypeAttribute, string myAlias, DBObjectStream myDBObject, TypeManager myTypeManager, Int64 myDepth, RouteToDBObjects myRouteToDBObjects, string myReference, SessionInfos myToken, IDBObjectCache dbObjectCache)
//        {
//            if (myTypeAttribute.TypeCharacteristics.IsBackwardEdge)
//            {
//                #region backward edges

//                EdgeKey edgeKey = myTypeAttribute.BackwardEdgeDefinition;
//                var contBackwardExcept = myDBObject.ContainsBackwardEdge(edgeKey, myTypeManager, dbObjectCache, myType);

//                if (contBackwardExcept.Failed)
//                    throw new GraphDBException(contBackwardExcept.Errors);

//                if (contBackwardExcept.Value)
//                {
//                    if (myDepth > 0)
//                    {
//                        var dbos = myDBObject.GetBackwardEdges(edgeKey, myTypeManager, myTypeAttribute, dbObjectCache);

//                        if (dbos.Failed)
//                            throw new GraphDBException(dbos.Errors);

//                        myAttributes.Add(myAlias, GetAttributeValue(myTypeManager.GetTypeByUUID(edgeKey.TypeUUID), myTypeAttribute, myTypeAttribute.Name, dbos.Value, myDepth - 1, myRouteToDBObjects.Route[myReference], 1, myTypeManager, myToken,dbObjectCache));
//                    }
//                    else
//                    {
//                        myAttributes.Add(myAlias, null);
//                    }
//                }

//                #endregion
//            }
//            else
//            {
//                if (myDBObject.HasAttribute(myTypeAttribute.UUID, myType, myToken))
//                {
//                    if (!myTypeAttribute.DBType.IsUserDefined)
//                    {
//                        var attrVal = myDBObject.GetAttribute(myTypeAttribute.UUID, myType, myToken, myTypeManager);
//                        // currently, we do not want to return a ADBBaseObject but the real value
//                        if (attrVal is ADBBaseObject)
//                            myAttributes.Add(myAlias, (attrVal as ADBBaseObject).Value);
//                        else if (attrVal is AListBaseEdgeType)
//                            myAttributes.Add(myAlias, (attrVal as AListBaseEdgeType).GetReadoutValues());
//                        else
//                            myAttributes.Add(myAlias, attrVal);
//                    }
//                    else
//                    {
//                        if (myDepth > 0)
//                        {
//                            myAttributes.Add(myAlias, GetAttributeValue(myTypeManager.GetTypeByUUID(myTypeAttribute.DBTypeUUID), myTypeAttribute, myTypeAttribute.Name, myDBObject.GetAttribute(myTypeAttribute.UUID, myType, myToken, myTypeManager), myDepth - 1, myRouteToDBObjects.Route[myReference], 1, myTypeManager, myToken,dbObjectCache));
//                        }
//                        else
//                        {
//                            myAttributes.Add(myAlias, null);
//                        }
//                    }
//                }
//            }
//        }

//        private void AddAttribute(ref Dictionary<string, object> myAttributes, string myAttributeName, string myAlias, DBObjectStream myDBObject, IDNode myIDNode, TypeManager myTypeManager, Int64 myDepth, RouteToDBObjects myRouteToDBObjects, string myReference, SessionInfos myToken, IDBObjectCache dbObjectCache)
//        {
//            AddAttribute(ref myAttributes, myIDNode.LastType, myIDNode.LastAttribute, myAlias, myDBObject, myTypeManager, myDepth, myRouteToDBObjects, myReference, myToken, dbObjectCache);
//        }

//        /// <summary>
//        /// This will return the result of all examined DBOs, including calculated aggregates and groupings.
//        /// The Value of the returned PandoraResult is of type HashSet&lt;DBObjectReadout&gt;
//        /// </summary>
//        /// <param name="myTypeManager">The TypeManager reference</param>
//        /// <param name="myToken">The current session token</param>
//        /// <returns>A PandoraResult including the ResultType with the occured errors.</returns>
//        public Exceptional<HashSet<DBObjectReadout>> GetResult(TypeManager myTypeManager, SessionInfos myToken, IDBObjectCache myDBObjectCache)
//        {

//            if ((_InterestingAttributes.AggregateNodes != null && _InterestingAttributes.AggregateNodes.Count > 0)
//                && (_InterestingAttributes.GroupNodes == null || _InterestingAttributes.GroupNodes.Count == 0))
//            //if (_AggregateNodes != null && _GroupingAttr == null)
//            #region We have only aggregates!
//            {
//                Dictionary<string, object> Attributes = new Dictionary<string, object>();
//                foreach (InterestingAttribute intAttr in _InterestingAttributes.AggregateNodes)
//                {
//                    AggregateNode aggrNode = (AggregateNode)intAttr.InterestingNode;
//                    var res = aggrNode.Aggregate.Aggregate(_DBOs, (aggrNode.Expressions[0] as IDNode).LastAttribute, myTypeManager, myDBObjectCache, myToken);
//                    if (res.Failed)
//                    {
//                        return new Exceptional<HashSet<DBObjectReadout>>(res);
//                    }
//                    Attributes.Add(aggrNode.Alias, res.Value);
//                }
//                _DBOs = new HashSet<DBObjectReadout>();
//                _DBOs.Add(new DBObjectReadoutGroup(Attributes, new DBObjectSystemData()));
//            }
//            #endregion
//            else //if (_AggregateNodes != null || _GroupingAttr != null)
//                if ((_InterestingAttributes.AggregateNodes != null && _InterestingAttributes.AggregateNodes.Count > 0)
//                    || (_InterestingAttributes.GroupNodes != null && _InterestingAttributes.GroupNodes.Count > 0))
//            #region We have aggregates and/or groupings
//            {

//                foreach (KeyValuePair<GroupingKey, HashSet<DBObjectReadout>> keyValPair in _Groupings.GroupsAndAggregates)
//                {
//                    Dictionary<string, object> Attributes = new Dictionary<string, object>();
//                    foreach (KeyValuePair<AttributeUUID, ADBBaseObject> groupKeyValPair in keyValPair.Key.Values)
//                    {

//                        TypeAttribute theAttribute = _TypeOfDBObject.GetTypeAttributeByUUID(groupKeyValPair.Key);
//                        if (theAttribute != null)
//                            Attributes.Add(theAttribute.Name, groupKeyValPair.Value.Value);
//                        else
//                        {
//                            ADBShowSetting Setting = (ADBShowSetting)_TypeOfDBObject.GetSettingValueByID(new AttributeUUID(groupKeyValPair.Key), myToken);
//                            if(Setting != null)
//                                Attributes.Add(Setting.Name, groupKeyValPair.Value.Value);
//                        }
//                    }

//                    #region Calculate aggregate

//                    if (_InterestingAttributes.AggregateNodes != null && _InterestingAttributes.AggregateNodes.Count > 0)
//                    {
//                        foreach (InterestingAttribute intAttr in _InterestingAttributes.AggregateNodes)
//                        {
//                            AggregateNode aggrNode = (AggregateNode)intAttr.InterestingNode;
//                            var res = aggrNode.Aggregate.Aggregate(keyValPair.Value, (aggrNode.Expressions[0] as IDNode).LastAttribute, myTypeManager, myDBObjectCache, myToken);
//                            if (res.Failed)
//                            {
//                                return new Exceptional<HashSet<DBObjectReadout>>(res);
//                            }
//                            Attributes.Add(aggrNode.Alias, res.Value);
//                        }
//                    }

//                    #endregion

//                    DBObjectReadoutGroup readoutGroup = new DBObjectReadoutGroup(Attributes, new DBObjectSystemData());
//                    readoutGroup.CorrespondingDBObjects = keyValPair.Value;

//                    #region Check for having expressions and evaluate them

//                    if ((_HavingExpression == null) || (_HavingExpression != null && _HavingExpression.IsSatisfyHaving(readoutGroup).Value))
//                    {
//                        _DBOs.Add(readoutGroup);
//                    }

//                    #endregion
//                }
//            }
//            #endregion

//            return new Exceptional<HashSet<DBObjectReadout>>(_DBOs);

//        }

//        private Dictionary<string, object> GetAllAttributesFromDBO(TypeManager myTypeManager, DBObjectStream aDBObject, GraphDBType typeOfAttribute, Int64 myDepth, Dictionary<int, List<PartialSelectResult>> routeForReference, int currentLvl, SessionInfos myToken, IDBObjectCache dbObjectCache)
//        {

//            Dictionary<string, object> Attributes = new Dictionary<string, object>();

//            #region UUID
//            var uuidSetting = typeOfAttribute.GetSettingValue(new SettingShowUUID().Name, myToken);
//            Object uuid = null;

//            if (uuidSetting != null)
//            {
//                String settingEncoding = (String)typeOfAttribute.GetSettingValue(DBConstants.SettingUUIDEncoding, myToken).Value.Value;

//                if (settingEncoding == null)
//                {
//                    throw new GraphDBException(new Error_ArgumentNullOrEmpty("settingEncoding"));
//                }

//                //uuid = ((ADBShowSetting)uuidSetting).DoExtract(aDBObject, settingEncoding);
//            }

//            if (uuid != null)
//            {
//                Attributes.Add(uuidSetting.Name, uuid);
//            }

//            #endregion

//            #region fill up attributes

//            foreach (KeyValuePair<AttributeUUID, AObject> aAttr in aDBObject.GetAttributes())
//            {

//                if (aAttr.Key != SpecialTypeAttribute_UUID.AttributeUUID)
//                {
//                    TypeAttribute typeAttribute = typeOfAttribute.GetTypeAttributeByUUID(aAttr.Key);
//                    if (!typeAttribute.DBType.IsUserDefined)
//                    {
//                        // currently, we do not want to return a ADBBaseObject but the real value
//                        if (aAttr.Value is ADBBaseObject)
//                            Attributes.Add(typeAttribute.Name, ((ADBBaseObject)aAttr.Value).Value);
//                        else
//                            Attributes.Add(typeAttribute.Name, aAttr.Value);

//                    }
//                    else
//                    {
//                        if (myDepth > 0)
//                        {
//                            Attributes.Add(typeAttribute.Name, GetAttributeValue(typeOfAttribute, typeAttribute, typeAttribute.Name, aDBObject.GetAttribute(aAttr.Key, typeOfAttribute, myToken, myTypeManager), myDepth - 1, routeForReference, currentLvl + 1, myTypeManager, myToken,dbObjectCache));
//                        }
//                        else
//                        {
//                            Attributes.Add(typeAttribute.Name, null);
//                        }
//                    }
//                }

//            }

//            #endregion

//            return Attributes;
//        }

//        private IEnumerable<DBObjectReadout> GetReadouts(TypeManager myTypeManager, GraphDBType typeOfAttribute, ASetReferenceEdgeType dbos, Int64 myDepth, Dictionary<int, List<PartialSelectResult>> routeForReference, int currentLvl, SessionInfos myToken, IDBObjectCache dbObjectCache)
//        {
//            List<DBObjectReadout> resultList = new List<DBObjectReadout>();

//            resultList.AddRange(dbos.GetReadouts(a => 
//                {
//                    var dbStream = _IDBObjectCache.LoadDBObjectStream(typeOfAttribute, a);
//                    if (dbStream.Failed)
//                    {
//                        throw new NotImplementedException();
//                    }

//                    return new DBObjectReadout(GetAllAttributesFromDBO(myTypeManager, dbStream.Value, typeOfAttribute, myDepth, routeForReference, currentLvl, myToken, dbObjectCache), new DBObjectSystemData(dbStream.Value));
//                }));

//            return resultList;
//        }

//        private object GetAttributeValue(GraphDBType baseType, TypeAttribute attrDefinition, string attributeName, object attributeValue, Int64 myDepth, Dictionary<int, List<PartialSelectResult>> routeForReference, int currentLvl, TypeManager myTypeManager, SessionInfos myToken, IDBObjectCache dbObjectCache)
//        {
//            #region Data

//            //TypeAttribute attrDefinition = myTypeManager.GetAttribute(baseType, attributeName);
//            GraphDBType typeOfAttribute = attrDefinition.DBType;
//            Dictionary<string, object> Attributes = null;

//            // For backwardEdges, the baseType is the type of the DBObjects!
//            if (attrDefinition.TypeCharacteristics.IsBackwardEdge)
//                typeOfAttribute = myTypeManager.GetTypeAttributeByEdge(attrDefinition.BackwardEdgeDefinition).RelatedPandoraType;

//            #endregion


//            if (attrDefinition.KindOfType == KindsOfType.SetOfReferences)
//            {
//                IEnumerable<DBObjectReadout> resultList = null;

//                var objects = (ASetReferenceEdgeType)attributeValue;
                
                
//                resultList = objects.GetReadouts(a => 
//                {
//                    var dbStream = _IDBObjectCache.LoadDBObjectStream(typeOfAttribute, a);
//                    if (dbStream.Failed)
//                    {
//                        throw new GraphDBException(dbStream.Errors);
//                    }

//                    return new DBObjectReadout(GetAllAttributesFromDBO(myTypeManager, dbStream.Value, typeOfAttribute, myDepth, routeForReference, currentLvl, myToken, dbObjectCache), new DBObjectSystemData(dbStream.Value));
//                });

//                return resultList;
//            }
//            else if (typeOfAttribute.IsUserDefined)
//            {
//                // For backwardEdges, we have always hashsets, even if the edge points to a single attribute!
//                if (attrDefinition.TypeCharacteristics.IsBackwardEdge)
//                {
//                    attributeValue = ((ASetReferenceEdgeType)attributeValue).FirstOrDefault();

//                    var aTempDBObject = dbObjectCache.LoadDBObjectStream(typeOfAttribute, (ObjectUUID)attributeValue);
//                    if (aTempDBObject.Failed)
//                    {
//                        throw new NotImplementedException();
//                    }

//                    #region fill up attributes

//                    Attributes = GetAllAttributesFromDBO(myTypeManager, aTempDBObject.Value, typeOfAttribute, myDepth, routeForReference, currentLvl, myToken,dbObjectCache);

//                    #endregion

//                    attributeValue = new DBObjectReadout(Attributes, new DBObjectSystemData(aTempDBObject.Value));
//                }
//                else
//                {
//                    attributeValue = (attributeValue as ASingleReferenceEdgeType).GetReadout(a =>
//                    {
//                        var dbStream = _IDBObjectCache.LoadDBObjectStream(typeOfAttribute, a);
//                        if (dbStream.Failed)
//                        {
//                            throw new NotImplementedException();
//                        }

//                        return new DBObjectReadout(GetAllAttributesFromDBO(myTypeManager, dbStream.Value, typeOfAttribute, myDepth, routeForReference, currentLvl, myToken,dbObjectCache), new DBObjectSystemData(dbStream.Value));
//                    });
//                }

//                return attributeValue;
//            }
//            else // this is a basic type - no need to resolve
//            {
//                return attributeValue;
//            }
//        }

//    }
//}
