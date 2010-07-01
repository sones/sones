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


/* <id name="sones GraphDB – SelectResultManager" />
 * <copyright file="SelectResultManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This will create the result of any kind of select - working on an IExpressionGraph.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Session;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.Lib;
using sones.Lib.ErrorHandling;
using System.Text;
using sones.Lib.Session;
using sones.GraphDB;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select
{

    #region GroupingKey and GroupingStructure

    public class GroupingKey : IComparable<Dictionary<AttributeUUID, ADBBaseObject>>
    {
        Dictionary<AttributeUUID, ADBBaseObject> _Values;
        Dictionary<AttributeUUID, PandoraTypeInformation> _GroupingAttr;

        public Dictionary<AttributeUUID, ADBBaseObject> Values
        {
            get { return _Values; }
            set { _Values = value; }
        }

        public GroupingKey(Dictionary<AttributeUUID, ADBBaseObject> myValues, Dictionary<AttributeUUID, PandoraTypeInformation> myGroupingAttr)
        {
            _Values = myValues;
            _GroupingAttr = myGroupingAttr;
        }

        #region IComparable<ADBBaseObject> Members

        public int CompareTo(Dictionary<AttributeUUID, ADBBaseObject> other)
        {

            // it could happen, that not all grouped aatributes are existing in all DBOs, so use the group by from the select to check
            foreach (AttributeUUID attr in _GroupingAttr.Keys)
            {
                if (_Values.ContainsKey(attr) && other.ContainsKey(attr))
                    if (_Values[attr].CompareTo(other[attr]) != 0)
                        return -1;
            }

            return 0;
        }

        #endregion

        public override int GetHashCode()
        {
            Int32 retVal = -1;
            foreach (KeyValuePair<AttributeUUID, ADBBaseObject> keyValPair in _Values)
            {
                if (retVal == -1)
                    retVal = keyValPair.Value.Value.GetHashCode();
                else
                    retVal = retVal ^ keyValPair.Value.Value.GetHashCode();
            }

            return retVal;
        }

        public override bool Equals(object obj)
        {
            return CompareTo(((GroupingKey)obj).Values) == 0;
        }
    }

    public class GroupingStructure
    {

        private Dictionary<AttributeUUID, PandoraTypeInformation> _GroupingAttr;

        // The attribute value of each Group BY, Set of DBOs which have exactly these values
        private Dictionary<GroupingKey, HashSet<DBObjectReadout>> _GroupsAndAggregates;
        public Dictionary<GroupingKey, HashSet<DBObjectReadout>> GroupsAndAggregates
        {
            get { return _GroupsAndAggregates; }
            set { _GroupsAndAggregates = value; }
        }

        public GraphDBType DBTypeStream
        {
            get;
            set;
        }

        public GroupingStructure(Dictionary<AttributeUUID, PandoraTypeInformation> myGroupingAttr)
        {
            _GroupsAndAggregates = new Dictionary<GroupingKey, HashSet<DBObjectReadout>>();
            _GroupingAttr = myGroupingAttr;
        }

        public void Add(DBObjectReadout myDBObjectReadout, Dictionary<AttributeUUID, ADBBaseObject> myValues)
        {
            GroupingKey curKey = new GroupingKey(myValues, _GroupingAttr);
            if (!_GroupsAndAggregates.ContainsKey(curKey))
            {
                _GroupsAndAggregates.Add(curKey, new HashSet<DBObjectReadout>());
            }
            _GroupsAndAggregates[curKey].Add(myDBObjectReadout);
        }
    }

    #endregion

    public class SelectResultManager
    {

        #region Data

        #region DBObjectCache

        DBObjectCache _DBObjectCache;
        public DBObjectCache DBObjectCache
        {
            set
            {
                _DBObjectCache = value;
            }
        }

        #endregion

        #region SessionToken

        SessionSettings _SessionToken;

        public SessionSettings SessionToken
        {
            set
            {
                _SessionToken = value;
            }
        }
        
        #endregion

        #region ExpressionGraph

        IExpressionGraph _ExpressionGraph;
        public IExpressionGraph ExpressionGraph
        {
            set
            {
                _ExpressionGraph = value;
            }
        } 

        #endregion

        #region Some private fields

        DBContext _DBContext;
        SelectSettingCache _SettingCache;
        IEnumerable<DBObjectReadout> _DBOsEnumerable;

        /// <summary>Dictionary of the base type and the selections of this type: e.g. U.Name and U.Cars.Color are both of basetype User</summary>
        /// The Level is the Level of LevelKey: e.g. U.Name == 0 and U.Cars.Color == 1
        /// If the User selected U.Name, U.Age than we have to SelectionElements in level 0 of type User
        /// reference [LevelKey, [selection]]
        Dictionary<String, Dictionary<LevelKey, List<SelectionElement>>> _Selections;


        List<SelectionElement> _SelectionElementsTypeIndependend;

        /// <summary>
        /// the selection element of _Selections , the aggregate selection element
        /// </summary>
        List<SelectionElement> _Aggregates;
        List<SelectionElement> _Groupings;

        Dictionary<String, GroupingStructure> _GroupingStructure;
        BinaryExpressionNode _HavingExpression;

        #endregion

        #endregion

        #region Ctor

        public SelectResultManager(DBContext dbContext, List<ATypeNode> myFromTypes)
        {
            _DBContext = dbContext;
            _SettingCache = new SelectSettingCache();

            _Selections = new Dictionary<string, Dictionary<LevelKey, List<SelectionElement>>>();
            _SelectionElementsTypeIndependend = new List<SelectionElement>();
            _GroupingStructure = new Dictionary<string,GroupingStructure>();

            _Aggregates = new List<SelectionElement>();
            _Groupings = new List<SelectionElement>();

        }

        #endregion

        #region Adding elements to selection

        /// <summary>
        /// Adds the typeNode as an asterisk *
        /// </summary>
        /// <param name="typeNode"></param>
        public void AddAsteriskToSelection(ATypeNode typeNode)
        {
            var selElem = new SelectionElement() { IsAsterisk = true };
        
            if (!_Selections.ContainsKey(typeNode.Reference))
                _Selections.Add(typeNode.Reference, new Dictionary<LevelKey, List<SelectionElement>>());

            var level = new LevelKey(new EdgeKey(typeNode.DBTypeStream.UUID, null));
            if (!_Selections[typeNode.Reference].ContainsKey(level))
                _Selections[typeNode.Reference].Add(level, new List<SelectionElement>());

            if (!_Selections[typeNode.Reference][level].Exists(item => item.IsAsterisk))
            {
                _Selections[typeNode.Reference][level].Add(selElem);
            }

        }
        
        /// <summary>
        /// Single IDNode selection attribute
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="alias"></param>
        /// <param name="myAStructureNode"></param>
        /// <param name="myPandoraType"></param>
        public void AddElementToSelection(string alias, String reference, IDNode myIDNode, Boolean isGroupedOrAggregated)
        {
            SelectionElement lastElem = null;

            var curLevel = new LevelKey();

            foreach (var nodeEdgeKey in myIDNode.IDNodeParts)
            {
                if (nodeEdgeKey is IDNodeEdge)
                {
                    var selElem = new SelectionElement() { Alias = alias, ContainingIDNodes = new List<IDNode>(new[] { myIDNode }), IsGroupedOrAggregated = isGroupedOrAggregated };

                    LevelKey preLevel;

                    var edgeKey = (nodeEdgeKey as IDNodeEdge).EdgeKey;
                    selElem.Element = _DBContext.DBTypeManager.GetTypeAttributeByEdge(edgeKey);

                    if (String.IsNullOrEmpty(selElem.Alias))
                        selElem.Alias = _DBContext.DBTypeManager.GetTypeAttributeByEdge(edgeKey).Name;

                    curLevel += edgeKey;
                    preLevel = curLevel.GetPredecessorLevel();

                    if (!_Selections.ContainsKey(reference))
                        _Selections.Add(reference, new Dictionary<LevelKey,List<SelectionElement>>());


                    if (!_Selections[reference].ContainsKey(preLevel))
                        _Selections[reference].Add(preLevel, new List<SelectionElement>());

                    if (_Selections[reference][preLevel].Exists(item => item.Alias == selElem.Alias) && !isGroupedOrAggregated)
                    {
                        throw new GraphDBException(new Error_DuplicateAttributeSelection(selElem.Alias));
                    }

                    _Selections[reference][preLevel].Add(selElem);

                    lastElem = selElem;
                }
                else if (nodeEdgeKey is IDNodeFunc)
                {

                    #region Type independent functions

                    if (reference == null)
                    {
                        _SelectionElementsTypeIndependend.Add(new SelectionElement() { Alias = alias, StructureNode = (nodeEdgeKey as IDNodeFunc).FuncCallNode });
                    }

                    #endregion

                    else
                    {
                        lastElem.StructureNode = (nodeEdgeKey as IDNodeFunc).FuncCallNode;
                    }
                }
            }

        }

        /// <summary>
        /// Multi IDNode selection attribute
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="alias"></param>
        /// <param name="myAStructureNode"></param>
        /// <param name="myPandoraType"></param>
        public void AddAggregateElementToSelection(string alias, string reference, IDNode tempIDNode, AStructureNode structureNode)
        {
            
            var selElem = new SelectionElement() { Alias = alias, StructureNode = structureNode };
            var edges = new List<EdgeKey>();

            if (structureNode is AggregateNode)
            {

                selElem.ContainingIDNodes = (structureNode as AggregateNode).ContainingIDNodes;
                selElem.StructureNode = structureNode;
                edges = (structureNode as AggregateNode).Edges;

                #region Check for index aggregate
                
                foreach (var edge in (structureNode as AggregateNode).Edges)
                {
                    
                    #region COUNT(*)
                    
                    if (edge.AttrUUID == null)
                    {
                        selElem.IndexAggregate = _DBContext.DBTypeManager.GetTypeByUUID(edge.TypeUUID).GetUUIDIndex(_DBContext.DBTypeManager);
                        selElem.Element = _DBContext.DBTypeManager.GetGUIDTypeAttribute();
                    }

                    #endregion

                    else if (_DBContext.DBTypeManager.GetTypeAttributeByEdge(edge).EdgeType is ASetReferenceEdgeType)
                    {
                        throw new GraphDBException(new Error_AggregateIsNotValidOnThisAttribute(_DBContext.DBTypeManager.GetTypeAttributeByEdge(edge).Name));
                    }

                    else
                    {
                        // if the GetAttributeIndex did not return null we will pass this as the aggregate operation value
                        selElem.IndexAggregate = _DBContext.DBTypeManager.GetTypeByUUID(edge.TypeUUID).GetAttributeIndex(edge.AttrUUID, null).Value;
                        selElem.Element = _DBContext.DBTypeManager.GetTypeAttributeByEdge(edge);
                    }
                }

                #endregion

                _Aggregates.Add(selElem);
                if (selElem.IndexAggregate == null)
                {
                    AddElementToSelection(null, reference, tempIDNode, true);
                }
            }

            
        }

        /// <summary>
        /// Adds a group element to the selection and validat it
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="iDNode"></param>
        public void AddGroupElementToSelection(string reference, IDNode iDNode)
        {
            // if the grouped attr is not selected
            if (!_Selections.ContainsKey(reference) || !_Selections[reference].Any(l => l.Value.Any(se => se.Element != null && se.Element == iDNode.LastAttribute )))
                throw new GraphDBException(new Error_GroupedAttributeIsNotSelected(iDNode.LastAttribute));
            else
                _Groupings.Add(new SelectionElement() { Alias = reference, Element = iDNode.LastAttribute, ContainingIDNodes = new List<IDNode>() { iDNode } });
        }

        /// <summary>
        /// Adds a having to the selection
        /// </summary>
        /// <param name="myHavingExpression"></param>
        public void AddHavingToSelection(BinaryExpressionNode myHavingExpression)
        {
            _HavingExpression = myHavingExpression;
        }

        #endregion

        #region Examine

        /// <summary>
        /// Examine a TypeNode to a specific <paramref name="myResolutionDepth"/> by using the underlying graph or the type guid index
        /// </summary>
        /// <param name="myResolutionDepth">The depth to which the reference attributes should be resolved</param>
        /// <param name="myTypeNode">The type node which should be examined on selections</param>
        /// <param name="myUsingGraph">True if there is a valid where expression of this typeNode</param>
        /// <returns>True if succeeded, false if there was nothing to select for this type</returns>
        public Exceptional<Boolean> Examine(Int64 myResolutionDepth, ATypeNode myTypeNode, Boolean myUsingGraph, DBObjectCache myDBObjectCache, SessionSettings mySessionToken, ref IEnumerable<DBObjectReadout> dbosEnumerable)
        {
            #region Return false if no selections were found for this typenode

            if (!_Selections.ContainsKey(myTypeNode.Reference) || !_Selections[myTypeNode.Reference].ContainsKey(new LevelKey(myTypeNode.DBTypeStream)))
            {
                
                #region Aggregates on index wont be in the _Selections

                if (_Aggregates.IsNullOrEmpty())
                {
                    return new Exceptional<bool>(false);
                }
                else
                {
                    if (myUsingGraph) // Due to we have a where expression we can't use the index - so add all AggrergateIndices to the selection
                    {
                        foreach(var aggr in _Aggregates.Where(a => a.IndexAggregate != null))
                        {
                            AddElementToSelection(null, myTypeNode.Reference, aggr.ContainingIDNodes.First(), true);
                        }
                    }
                    else
                    {
                        return new Exceptional<bool>(true);
                    }
                } 
                
                #endregion

            }

            #endregion

            var levelKey = new LevelKey(myTypeNode.DBTypeStream);
            //_DBOsEnumerable = dbosEnumerable;

            #region For groupings we cant create a enumerable

            if (!_Groupings.IsNullOrEmpty())
            {
                CreateGroupings(myResolutionDepth, myTypeNode, levelKey, myUsingGraph, myDBObjectCache, mySessionToken);
            }
            else
            {
                _DBOsEnumerable = dbosEnumerable;
                dbosEnumerable = ExamineDBO(myResolutionDepth, myTypeNode, levelKey, myUsingGraph, myDBObjectCache, mySessionToken);
            }

            #endregion

            return new Exceptional<Boolean>(true);

        }

        /// <summary>
        /// This is the main function. It will check all selections on this type and will create the readouts
        /// </summary>
        /// <param name="dbos"></param>
        /// <param name="myResolutionDepth"></param>
        /// <param name="myTypeNode"></param>
        /// <param name="levelKey"></param>
        /// <param name="myUsingGraph">True if for all selects the graph will be asked for DBOs</param>
        /// <param name="myDBObjectCache"></param>
        /// <param name="mySessionToken"></param>
        /// <returns></returns>
        private IEnumerable<DBObjectReadout> ExamineDBO(long myResolutionDepth, ATypeNode myTypeNode, LevelKey levelKey, bool myUsingGraph, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {

            #region Get all selection for this reference, type and level
            
            var selections = getAttributeSelections(myTypeNode.Reference, myTypeNode.DBTypeStream, levelKey);

            #endregion

            if (!selections.IsNullOrEmpty() && selections.All(s => s.IsReferenceToSkip(levelKey) ))
            {

                #region If there are only references in this level, we will skip this level (and add the attribute as placeholder) and step to the next one

                var Attributes = new Dictionary<String, Object>();

                foreach (var sel in selections)
                {
                    var edgeKey = new EdgeKey(sel.Element.RelatedPandoraTypeUUID, sel.Element.UUID);
                    Attributes.Add(sel.Alias, new Edge(ExamineDBO(myResolutionDepth, myTypeNode, levelKey + edgeKey, myUsingGraph, myDBObjectCache, mySessionToken), sel.Element.GetDBType(_DBContext.DBTypeManager).Name));
                }
                
                yield return new DBObjectReadout(Attributes);

                #endregion

            }
            else
            {

                #region Otherwise load all dbos until this level and return them

                #region Get dbos enumerable of the first level - this need to be changed if the graph is used but the first (0) level is not selected

                IEnumerable<Exceptional<DBObjectStream>> dbos;
                if (myUsingGraph)
                {
                    dbos = _ExpressionGraph.Select(levelKey, null, true);
                }
                else // using GUID index
                {
                    dbos = _DBObjectCache.SelectDBObjectsForLevelKey(levelKey, _DBContext);
                }

                #endregion

                foreach (var aDBObject in dbos)
                {
                    if (aDBObject.Failed)
                    {
                        // since we are in an yield we can do nothing else than throw a exception
                        throw new GraphDBException(aDBObject.Errors);
                    }

                    #region Create a readoutObject for this DBO and yield it: on an failure throw a exception

                    var examineResult = CreateReadOutObject(aDBObject.Value, myResolutionDepth, myTypeNode, levelKey, myUsingGraph, myDBObjectCache, mySessionToken);
                    if (examineResult.Failed)
                    {
                        // since we are in an yield we can do nothing else than throw a exception
                        throw new GraphDBException(examineResult.Errors);
                    }

                    if (examineResult.Value != null) // could happens, if the DBO has no selected attributes
                        yield return examineResult.Value;

                    #endregion

                }

                #endregion

            }

        }

        /// <summary>
        /// Create readouts of all attributes of a specific type <paramref name="myTypeOfDBObject"/>
        /// </summary>
        /// <param name="myDBObject">The DBObject which is examined</param>
        /// <param name="myDepth">The Depth</param>
        /// <param name="myTypeOfDBObject">The Type</param>
        /// <param name="myLevelKey">The levelKey</param>
        /// <returns>Successful or Failed for any error</returns>
        private Exceptional<DBObjectReadout> CreateReadOutObject(DBObjectStream myDBObject, Int64 myDepth, ATypeNode typeNode, LevelKey myLevelKey, Boolean myUsingGraph, DBObjectCache myObjectCache, SessionSettings mySessionToken)
        {

            Dictionary<string, object> Attributes = new Dictionary<string, object>();
            Int64 Depth;

            var minDepth = 0;

            var type = typeNode.DBTypeStream;
            var attributeSelections = getAttributeSelections(typeNode.Reference, type, myLevelKey);

            if (attributeSelections == null)
            {
                throw new GraphDBException(new Error_UnknownDBError("Should never happen!!! attributeSelections is null in CreateReadOutObject"));
            }

            foreach (var attrSel in attributeSelections)
            {
                if (attrSel.IsAsterisk)
                {

                    #region Asterisk (*) selection

                    Depth = GetDepth(myDepth, 0, type, null, _SessionToken);

                    AddAttributesByDBO(ref Attributes, type, myDBObject, Depth, myLevelKey, typeNode.Reference, myUsingGraph);

                    #endregion

                }
                else if (attrSel.StructureNode is FuncCallNode)
                {

                    #region Function

                    #region Get the FunctionNode and validate the Element

                    FuncCallNode fcn = ((FuncCallNode)attrSel.StructureNode);
                    fcn.CallingAttribute = attrSel.Element;

                    if (attrSel.Element == null)
                        continue;

                    #endregion

                    #region Get the CallingObject

                    if (myUsingGraph && (attrSel.Element.GetDBType(_DBContext.DBTypeManager).IsUserDefined || attrSel.Element.GetDBType(_DBContext.DBTypeManager).IsBackwardEdge))
                    {
                        var edge = GetAttributeValue(typeNode.DBTypeStream, attrSel.Element, myDBObject, myLevelKey);
                        if (edge is AEdgeType)
                            fcn.CallingObject = (edge as AEdgeType).GetNewInstance(_ExpressionGraph.SelectUUIDs(myLevelKey + new EdgeKey(attrSel.Element), myDBObject, true));
                        else
                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                    else
                    {
                        fcn.CallingObject = GetAttributeValue(typeNode.DBTypeStream, attrSel.Element, myDBObject, myLevelKey) as AObject;
                    }

                    #endregion

                    #region CallingDBObjectStream

                    fcn.CallingDBObjectStream = myDBObject;

                    #endregion

                    if (fcn.CallingObject == null) // DBObject does not have the attribute
                        continue;

                    #region Execute the function

                    var res = fcn.Execute(typeNode.DBTypeStream, myDBObject, typeNode.Reference, _DBContext, typeNode.DBTypeStream, _DBObjectCache, mySessionToken);
                    if (res.Failed)
                    {
                        return new Exceptional<DBObjectReadout>(res);
                    }
                    else
                    {
                        if (res.Value.Value == null)
                            continue; // no result for this object because of not set attribute value

                        //TODO: change the type of result to a more convinient thing
                        //if (fcn.Function.TypeOfResult == TypesOfOperatorResult.SetOfDBObjects || fcn.Function.TypeOfResult == TypesOfOperatorResult.NotABasicType)
                        if (res.Value.Value is IReferenceEdge)
                        {
                            Depth = GetDepth(myDepth, minDepth, type, null, _SessionToken);

                            if (Depth > myLevelKey.Level)
                            {

                                //if (fcn.Function.TypeOfResult == TypesOfOperatorResult.NotABasicType)
                                    myUsingGraph = false;

                                #region Resolve DBReferences

                                Attributes.Add(attrSel.Alias,
                                    ResolveAttributeValue(((FuncParameter)res.Value).TypeAttribute,((FuncParameter)res.Value).Value,Depth - 1, myLevelKey, myDBObject, typeNode.Reference, myUsingGraph));

                                #endregion

                            }
                            else
                            {

                                Attributes.Add(attrSel.Alias, null);

                            }

                        }
                        else
                        {
                            Attributes.Add(attrSel.Alias, ((FuncParameter)res.Value).Value.GetReadoutValue());
                        }
                    }


                    #endregion

                    #endregion

                }
                else if (attrSel.Element is TypeAttribute)
                {

                    #region Attribute selection

                    minDepth = (attrSel.ContainingIDNodes != null) ? attrSel.ContainingIDNodes[0].Edges.Count - 1 : 0;

                    var alias = (attrSel.Alias == null) ? (attrSel.Element as TypeAttribute).Name : attrSel.Alias;

                    if (myLevelKey.Level > 0 && !(attrSel.Element is ASpecialTypeAttribute)) // use the related type instead
                        Depth = GetDepth(myDepth, minDepth, (attrSel.Element as TypeAttribute).GetRelatedType(_DBContext.DBTypeManager), (attrSel.Element as TypeAttribute), _SessionToken);
                    else
                        Depth = GetDepth(myDepth, minDepth, type, (attrSel.Element as TypeAttribute), _SessionToken);

                    Object attrValue = null;

                    if (!Attributes.ContainsKey(alias))
                    {
                        if (GetAttributeValueAndResolve(typeNode.DBTypeStream, attrSel.Element as TypeAttribute, myDBObject, Depth, myLevelKey, typeNode.Reference, myUsingGraph, out attrValue))
                        {
                            Attributes.Add(alias, attrValue);
                        }
                    }

                    #endregion

                }
            }

            if (!Attributes.IsNullOrEmpty())
            {
                return new Exceptional<DBObjectReadout>(new DBObjectReadout(Attributes));
            }
            else
            {
                // we found no attributes, so we return null because currently we do not want to add empty attribute readouts
                return new Exceptional<DBObjectReadout>((DBObjectReadout)null);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myDepth">The target depth (defined by the select)</param>
        /// <param name="minDepth">The minimum depth. If the depth of the setting or mydepth is lower then minDepth will be returned</param>
        /// <param name="typeUUID"></param>
        /// <param name="attributeUUID">May be NULL, than only the depth settings of the type will be checked</param>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        private long GetDepth(long myDepth, long minDepth, GraphDBType type, TypeAttribute attribute, SessionSettings sessionToken)
        {
            Int64 Depth = 0;

            #region Get the depth for this type or from select

            /// This results of all selected attributes and their LevelKey. If U.Friends.Friends.Name is selected, than the MinDepth is 3

            if (attribute != null)
            {

                if (myDepth > -1)
                    Depth = Math.Max(minDepth, myDepth);
                else
                    Depth = Math.Max(minDepth, (Int64)_SettingCache.GetValue(type, attribute, SettingDepth.UUID, _DBContext).Value);

            }
            else
            {
            
                if (myDepth > -1)
                    Depth = Math.Max(minDepth, myDepth);
                else
                    Depth = Math.Max(minDepth, (Int64)(_DBContext.DBSettingsManager.GetSetting(SettingDepth.UUID, _DBContext, TypesSettingScope.TYPE, type).Value.Value).Value);

            }

            #endregion

            return Depth;
        }

        /// <summary>
        /// Get all selections on this <paramref name="reference"/>, <paramref name="type"/> and <paramref name="myLevelKey"/>
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="type"></param>
        /// <param name="myLevelKey"></param>
        /// <returns></returns>
        private List<SelectionElement> getAttributeSelections(String reference, GraphDBType type, LevelKey myLevelKey)
        {
            if (_Selections.ContainsKey(reference) && (_Selections[reference].ContainsKey(myLevelKey)))
            {
                return _Selections[reference][myLevelKey];
            }
            else
            {
                return null;
            }

        }

        #endregion

        #region Grouping and Aggregates

        /// <summary>
        /// Initialize the grouping and aggregate and validate them
        /// </summary>
        /// <param name="typeNode"></param>
        /// <returns></returns>
        public Exceptional<bool> InitGroupingOrAggregate(ATypeNode typeNode)
        {
            #region if we have grouping or aggregate the must be NO attribute in the select list which is not covert by an aggregate or grouping

            // all selections which are NOT from an aggregate
            var selectionsWithoutGroupOrAggregate = ((_Selections.ContainsKey(typeNode.Reference)) ? _Selections[typeNode.Reference].Aggregate(0, (result, el) => result + el.Value.Count) : 0) - _Aggregates.Count(e => e.IndexAggregate == null) - _Groupings.Count;

            //var selectionsWithoutGroupOrAggregate = ((_Selections.ContainsKey(typeNode.Reference)) ? _Selections[typeNode.Reference].Count : 0) - _Aggregates.Count(e => e.IndexAggregate == null) - _Groupings.Count;

            #region If we have defined grouping, all selects must be groups or aggregates

            if (_Groupings.Count > 0 || _Aggregates.Count > 0)
            {

                if (selectionsWithoutGroupOrAggregate > 0)
                    return new Exceptional<bool>(new Error_NoGroupingArgument());

            }

            #endregion

            #region Store the List of aggregated uuids

            if (!_Aggregates.IsNullOrEmpty())
            {
                foreach (var attr in _Aggregates)
                {

                    AggregateNode aggrNode = (AggregateNode)attr.StructureNode;

                    if (aggrNode.Expressions.Count > 1)
                        return new Exceptional<bool>(new Error_AggregateOnMultiAttributesNotAllowed(aggrNode.Expressions.Aggregate(new StringBuilder(), (ret, source) =>
                        {
                            ret.Append(source as IDNode); ret.Append(","); return ret;
                        }).ToString()));

                    // We checked prio in GetContent of SelectNode that we have exactly one expression
                    IDNode aAggrIDNode = aggrNode.Expressions[0] as IDNode;

                    if (aAggrIDNode == null)
                    {
                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                    else if (aAggrIDNode.Level > 1)
                    {
                        return new Exceptional<bool>(new Error_AggregateIsNotValidOnThisAttribute(aAggrIDNode.ToString()));
                    }
                }
            }

            #endregion

            if (!_Groupings.IsNullOrEmpty())
            {

                Dictionary<AttributeUUID, PandoraTypeInformation> groupingAttr = new Dictionary<AttributeUUID, PandoraTypeInformation>();
                foreach (var intAttr in _Groupings)
                {
                    groupingAttr.Add((intAttr.Element as TypeAttribute).UUID, new PandoraTypeInformation(((IDNode)intAttr.ContainingIDNodes[0])));
                    break;
                }
                _GroupingStructure.Add(typeNode.Reference, new GroupingStructure(groupingAttr) { DBTypeStream = typeNode.DBTypeStream });
                /*
                #region Check that eacht selected attribute is either is aggregated or grouped

                foreach (var intAttr in _Selections)
                {
                    if (!groupingAttr.ContainsKey(((IDNode)intAttr.AttributeIDNode).LastAttribute.UUID))
                    {
                        return new Exceptional<bool>(new Error_InvalidAttributeSelection("Attribute '" + intAttr.AttributeName + "' is invalid in the select list because it is not contained in either an aggregate function or the GROUP BY clause."));
                    }
                }

                #endregion
                */
            }
            #endregion

            return new Exceptional<bool>();
        }

        private void CreateGroupings(long myResolutionDepth, ATypeNode myTypeNode, LevelKey levelKey, bool myUsingGraph, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            IEnumerable<Exceptional<DBObjectStream>> dbos;

            #region Get dbos enumerable of the first level - this need to be changed if the graph is used but the first (0) level is not selected

            if (myUsingGraph)
            {
                dbos = _ExpressionGraph.Select(levelKey, null, true);
            }
            else // using GUID index
            {
                dbos = _DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(myTypeNode.DBTypeStream), _DBContext);
            }

            #endregion


            var interestingAttributes = GetInterestingAttributes(myTypeNode.Reference, myTypeNode.DBTypeStream);

            foreach (var dbo in dbos)
            {

                var myDBObject = dbo.Value;
                var groupingVals = new Dictionary<AttributeUUID, ADBBaseObject>();

                foreach (var _InterestingAttribute in _Groupings)
                {

                    var typeAttr = _InterestingAttribute.Element as TypeAttribute;
                    // some grouping

                    if (myDBObject.HasAttribute(typeAttr.UUID, myTypeNode.DBTypeStream, mySessionToken))
                    {
                        groupingVals.Add(typeAttr.UUID,
                            PandoraTypeMapper.GetPandoraObjectFromTypeName(
                            typeAttr.GetDBType(_DBContext.DBTypeManager).Name, ResolveAttributeValue(_InterestingAttribute.Element as TypeAttribute, myDBObject.GetAttribute(typeAttr.UUID, myTypeNode.DBTypeStream, _DBContext),
                            myResolutionDepth, levelKey, myDBObject, myTypeNode.Reference, myUsingGraph)
                            ));
                    }
                }
                
                //_GroupingStructure[myTypeNode.Reference].Add(new DBObjectReadout(myDBObject.GetAttributes(), myTypeNode.DBTypeStream), groupingVals);
                _GroupingStructure[myTypeNode.Reference].Add(new DBObjectReadout(GetInterestingAttributeValues(myDBObject, interestingAttributes, myTypeNode.DBTypeStream, mySessionToken)), groupingVals);
            }

        }

        private Dictionary<string, Object> GetInterestingAttributeValues(DBObjectStream myDBObject, HashSet<TypeAttribute> interestingAttributes, GraphDBType graphDBType, SessionSettings mySessionToken)
        {
            Dictionary<string, Object> result = new Dictionary<string, Object>();

            foreach (var aTypeAttribute in interestingAttributes)
            {
                result.Add(aTypeAttribute.Name, ((AObject)myDBObject.GetAttribute(aTypeAttribute, graphDBType, _DBContext)).GetReadoutValue());
            }

            return result;
        }

        private HashSet<TypeAttribute> GetInterestingAttributes(String myReference, GraphDBType graphDBType)
        {
            HashSet<TypeAttribute> result = new HashSet<TypeAttribute>();

            foreach (var aAggregate in _Aggregates)
            {
                result.Add(aAggregate.Element);
            }

            foreach (var aGrouping  in _Groupings.Where(item => item.Alias == myReference))
            {
                result.Add(aGrouping.Element);
            }

            foreach (var aSelection in _Selections.Where(item => item.Key == myReference).Select(aMatchingElement => aMatchingElement.Value))
            {
                foreach (var aLevelKey in aSelection.Select(item => item.Value))
                {
                    foreach (var aInnerSelection in aLevelKey)
                    {
                        result.Add(aInnerSelection.Element);
                    }
                }
            }

            return result;
        }

        #endregion

        #region GetReadouts - from a SetReferenceEdgeType

        /// <summary>
        /// Resolve a AListReferenceEdgeType to a DBObjectReadouts. This will resolve each edge target using the 'GetAllAttributesFromDBO' method
        /// </summary>
        /// <param name="typeOfAttribute"></param>
        /// <param name="dbos"></param>
        /// <param name="myDepth"></param>
        /// <param name="currentLvl"></param>
        /// <returns></returns>
        private IEnumerable<DBObjectReadout> GetReadouts(GraphDBType typeOfAttribute, ASetReferenceEdgeType dbos, IEnumerable<Exceptional<DBObjectStream>> myObjectUUIDs, Int64 myDepth, LevelKey myLevelKey, String reference, Boolean myUsingGraph)
        {
            foreach (var aReadOut in dbos.GetReadouts(a =>
            {
                var dbStream = _DBObjectCache.LoadDBObjectStream(typeOfAttribute, a);
                if (dbStream.Failed)
                {
                    throw new GraphDBException(dbStream.Errors);
                }

                return new DBObjectReadout(GetAllAttributesFromDBO(dbStream.Value, typeOfAttribute, myDepth, myLevelKey, reference, myUsingGraph));
            }, myObjectUUIDs))
            {
                yield return aReadOut;
            }

            yield break;
        }

        /// <summary>
        /// Resolve a AListReferenceEdgeType to a List of DBObjectReadout. This will resolve each edge target using the 'GetAllAttributesFromDBO' method
        /// </summary>
        /// <param name="typeOfAttribute"></param>
        /// <param name="dbos"></param>
        /// <param name="myDepth"></param>
        /// <param name="currentLvl"></param>
        /// <returns></returns>
        private IEnumerable<DBObjectReadout> GetReadouts(GraphDBType typeOfAttribute, ASetReferenceEdgeType dbos, Int64 myDepth, LevelKey myLevelKey, String reference, Boolean myUsingGraph)
        {
            SettingInvalidReferenceHandling invalidReferenceSetting = null;
            GraphDBType currentType = null;

            foreach (var aReadOut in dbos.GetReadouts(a =>
            {
                var dbStream = _DBObjectCache.LoadDBObjectStream(typeOfAttribute, a);
                if (!dbStream.Success)
                {
                    #region error

                    #region get setting

                    if (invalidReferenceSetting == null)
                    {
                        currentType = _DBContext.DBTypeManager.GetTypeByUUID(myLevelKey.LastEdge.TypeUUID);

                        if (myLevelKey.LastEdge.AttrUUID != null)
                        {
                            invalidReferenceSetting = (SettingInvalidReferenceHandling)_DBContext.DBSettingsManager.GetSetting(SettingInvalidReferenceHandling.UUID, _DBContext, TypesSettingScope.ATTRIBUTE, currentType, currentType.GetTypeAttributeByUUID(myLevelKey.LastEdge.AttrUUID)).Value;
                        }
                        else
                        {
                            invalidReferenceSetting = (SettingInvalidReferenceHandling)_DBContext.DBSettingsManager.GetSetting(SettingInvalidReferenceHandling.UUID, _DBContext, TypesSettingScope.TYPE, currentType).Value;
                        }
                    }

                    #endregion

                    switch (invalidReferenceSetting.Behaviour)
                    {
                        case BehaviourOnInvalidReference.ignore:
                            #region ignore

                            return GenerateUndefindedDBReadout(a, currentType);

                            #endregion
                        case BehaviourOnInvalidReference.log:
                        
                        default:

                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
                    }

                    #endregion
                }

                return new DBObjectReadout(GetAllAttributesFromDBO(dbStream.Value, typeOfAttribute, myDepth, myLevelKey, reference, myUsingGraph));               
            }))
            {
                if (aReadOut != null)
                {
                    yield return aReadOut;
                }
            }

            yield break;
        }

        #endregion

        #region AddAttribute and get the values

        /// <summary>
        /// Instead of using the Type with the attributes it will show ALL attributes of the <paramref name="myDBObject"/>
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="type"></param>
        /// <param name="myDBObject"></param>
        /// <param name="depth"></param>
        /// <param name="myLevelKey"></param>
        /// <param name="reference"></param>
        /// <param name="myUsingGraph"></param>
        private void AddAttributesByDBO(ref Dictionary<string, object> attributes, GraphDBType type, DBObjectStream myDBObject, long depth, LevelKey myLevelKey, string reference, bool myUsingGraph)
        {

            #region Get all attributes which are stored at the DBO

            foreach (var attr in myDBObject.GetAttributes())
            {

                #region Check whether the attribute is still exist in the type - if not, continue

                var typeAttr = type.GetTypeAttributeByUUID(attr.Key);
                if (typeAttr == null)
                    continue;

                #endregion

                if (attr.Value is ADBBaseObject)
                {
                    attributes.Add(typeAttr.Name, (attr.Value as ADBBaseObject).GetReadoutValue());
                }
                else if (attr.Value is AListBaseEdgeType)
                {
                    attributes.Add(typeAttr.Name, (attr.Value as AListBaseEdgeType).GetReadoutValues());
                }
                else if (attr.Value is ASetReferenceEdgeType || attr.Value is ASingleReferenceEdgeType)
                {
                    // Since we can define special depth (via setting) for attributes we need to check them now
                    depth = GetDepth(-1, depth, type, typeAttr, _SessionToken);
                    if (depth > 0)
                    {
                        attributes.Add(typeAttr.Name, ResolveAttributeValue(typeAttr, attr.Value, depth - 1, myLevelKey, myDBObject, reference, myUsingGraph));
                    }
                    else
                    {
                        attributes.Add(typeAttr.Name, GetNotResolvedReferenceAttributeValue(myDBObject, typeAttr, type, _DBContext));
                    }
                }
                else
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

            }

            #endregion

            #region Get all backwardEdge attributes

            foreach (var beAttr in GetBackwardEdgeAttributes(type))
            {
                if (depth > 0)
                {
                    var bes = myDBObject.GetBackwardEdges(beAttr.BackwardEdgeDefinition, _DBContext, _DBObjectCache, beAttr.GetDBType(_DBContext.DBTypeManager));

                    if (bes.Failed)
                        throw new GraphDBException(bes.Errors);

                    if (bes.Value != null) // otherwise the DBO does not have any
                        attributes.Add(beAttr.Name, ResolveAttributeValue(beAttr, bes.Value, depth - 1, myLevelKey, myDBObject, reference, myUsingGraph));
                }
                else
                {
                    var notResolvedBEs = GetNotResolvedBackwardEdgeReferenceAttributeValue(myDBObject, beAttr, beAttr.BackwardEdgeDefinition, _DBContext);
                    if (notResolvedBEs != null)
                    {
                        attributes.Add(beAttr.Name, notResolvedBEs);
                    }
                }
            }

            #endregion

            #region Get all untyped attributes from DBO - to be done

            var undefAttrException = myDBObject.GetUndefinedAttributes(_DBContext.DBObjectManager);

            if (undefAttrException.Failed)
                throw new GraphDBException(undefAttrException.Errors);

            foreach (var undefAttr in undefAttrException.Value)
                attributes.Add(undefAttr.Key, undefAttr.Value.GetReadoutValue());

            #endregion

            #region Add special attributes

            foreach (var specialAttr in GetSpecialAttributes(type))
            {
                if (!attributes.ContainsKey(specialAttr.Name))
                {
                    var result = (specialAttr as ASpecialTypeAttribute).ExtractValue(myDBObject, type, _DBContext);
                    if (result.Failed)
                    {
                        throw new GraphDBException(result.Errors);
                    }

                    attributes.Add(specialAttr.Name, result.Value.GetReadoutValue());
                }
            }

            #endregion
        }

        Dictionary<GraphDBType, List<TypeAttribute>> _BackwardEdgeAttributesByType = new Dictionary<GraphDBType, List<TypeAttribute>>();
        private IEnumerable<TypeAttribute> GetBackwardEdgeAttributes(GraphDBType type)
        {
            if (!_BackwardEdgeAttributesByType.ContainsKey(type))
            {
                _BackwardEdgeAttributesByType.Add(type, type.GetAllAttributes(t => t.IsBackwardEdge, _DBContext).ToList());
            }
            return _BackwardEdgeAttributesByType[type];
        }
        
        Dictionary<GraphDBType, List<TypeAttribute>> _SpecialAttributesByType = new Dictionary<GraphDBType, List<TypeAttribute>>();
        private IEnumerable<TypeAttribute> GetSpecialAttributes(GraphDBType type)
        {
            if (!_SpecialAttributesByType.ContainsKey(type))
            {
                _SpecialAttributesByType.Add(type, type.GetAllAttributes(t => t is ASpecialTypeAttribute, _DBContext).ToList());
            }
            return _SpecialAttributesByType[type];
        }

        /// <summary>   Gets an attribute value. </summary>
        ///
        /// <remarks>   Stefan, 16.04.2010. </remarks>
        ///
        /// <param name="myType">           Type. </param>
        /// <param name="myTypeAttribute">  my type attribute. </param>
        /// <param name="myDBObject">       my database object. </param>
        /// <param name="myDepth">          Depth of my. </param>
        /// <param name="myLevelKey">       my level key. </param>
        /// <param name="reference">        The reference. </param>
        /// <param name="myUsingGraph">     true to my using graph. </param>
        /// <param name="attributeValue">   [out] The attribute value. </param>
        ///
        /// <returns>   true if it succeeds, false if the DBO does not have the attribute. </returns>
        private Boolean GetAttributeValueAndResolve(GraphDBType myType, TypeAttribute myTypeAttribute, DBObjectStream myDBObject, Int64 myDepth, LevelKey myLevelKey, String reference, Boolean myUsingGraph, out Object attributeValue)
        {
            if (myTypeAttribute.TypeCharacteristics.IsBackwardEdge)
            {
                #region IsBackwardEdge

                EdgeKey edgeKey = myTypeAttribute.BackwardEdgeDefinition;
                var contBackwardExcept = myDBObject.ContainsBackwardEdge(edgeKey, _DBContext, _DBObjectCache, myType);

                if (contBackwardExcept.Failed)
                    throw new GraphDBException(contBackwardExcept.Errors);

                if (contBackwardExcept.Value)
                {
                    if (myDepth > 0)
                    {
                        var dbos = myDBObject.GetBackwardEdges(edgeKey, _DBContext, _DBObjectCache, myTypeAttribute.GetDBType(_DBContext.DBTypeManager));

                        if (dbos.Failed)
                            throw new GraphDBException(dbos.Errors);

                        if (dbos.Value != null)
                        {
                            attributeValue = ResolveAttributeValue(myTypeAttribute, dbos.Value, myDepth - 1, myLevelKey, myDBObject, reference, myUsingGraph);
                            return true;
                        }
                    }
                    else
                    {
                        attributeValue = GetNotResolvedBackwardEdgeReferenceAttributeValue(myDBObject, myTypeAttribute, edgeKey, _DBContext);
                        return true;

                    }
                }

                #endregion

            }


            else if (myDBObject.HasAttribute(myTypeAttribute.UUID, myType, _SessionToken))
            {

                #region ELSE (!IsBackwardEdge)

                #region not a reference attribute value

                if (!myTypeAttribute.GetDBType(_DBContext.DBTypeManager).IsUserDefined)
                {
                    var attrVal = myDBObject.GetAttribute(myTypeAttribute.UUID, myType, _DBContext);
                    // currently, we do not want to return a ADBBaseObject but the real value
                    if (attrVal is ADBBaseObject)
                        attributeValue = (attrVal as ADBBaseObject).GetReadoutValue();
                    else if (attrVal is AListBaseEdgeType)
                        attributeValue = (attrVal as AListBaseEdgeType).GetReadoutValues();
                    else
                        attributeValue = attrVal;

                    return true;
                }

                #endregion

                #region ELSE Reference attribute value

                else
                {
                    if (myDepth > 0)
                    {
                        var attrValue = myDBObject.GetAttribute(myTypeAttribute.UUID, myType, _DBContext);
                        attributeValue = ResolveAttributeValue(myTypeAttribute, attrValue, myDepth - 1, myLevelKey, myDBObject, reference, myUsingGraph);
                        return true;
                    }
                    else
                    {
                        attributeValue = GetNotResolvedReferenceAttributeValue(myDBObject, myTypeAttribute, myType, _DBContext);
                        return true;
                    }
                }

                #endregion

                #endregion

            }

            attributeValue = null;
            return false;
        }

        private Edge GetNotResolvedReferenceAttributeValue(DBObjectStream myDBObject, TypeAttribute myTypeAttribute, GraphDBType myType, DBContext _DBContext)
        {
            var attrValue = myDBObject.GetAttribute(myTypeAttribute.UUID, myType, _DBContext);

            if (attrValue == null)
            {
                return null;
            }
            else if (!(attrValue is IReferenceEdge))
            {
                throw new GraphDBException(new Error_InvalidEdgeType(attrValue.GetType(), typeof(IReferenceEdge)));
            }

            List<DBObjectReadout> readouts = new List<DBObjectReadout>();
            var typeName = myTypeAttribute.GetDBType(_DBContext.DBTypeManager).Name;

            if (attrValue is ASetReferenceEdgeType)
            {
                return new Edge((attrValue as ASetReferenceEdgeType).GetReadouts((uuid) => GenerateUndefindedDBReadout(uuid, myTypeAttribute.GetDBType(_DBContext.DBTypeManager))), typeName);
            }
            else if (attrValue is ASingleReferenceEdgeType)
            {
                return new Edge((attrValue as ASingleReferenceEdgeType).GetReadout((uuid) => GenerateUndefindedDBReadout(uuid, myTypeAttribute.GetDBType(_DBContext.DBTypeManager))), typeName);
            }
            else
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }

        private DBObjectReadout GenerateUndefindedDBReadout(ObjectUUID reference, GraphDBType myType)
        {
            var specialAttributes = new Dictionary<string, object>();
            specialAttributes.Add(SpecialTypeAttribute_UUID.AttributeName, reference);
            specialAttributes.Add(SpecialTypeAttribute_TYPE.AttributeName, myType);

            return new DBObjectReadout(specialAttributes);
        }

        private object GetNotResolvedBackwardEdgeReferenceAttributeValue(DBObjectStream myDBObject, TypeAttribute myTypeAttribute, EdgeKey edgeKey, DBContext _DBContext)
        {
            var attrValue = myDBObject.GetBackwardEdges(edgeKey, _DBContext, _DBObjectCache, myTypeAttribute.GetDBType(_DBContext.DBTypeManager));
            if (attrValue.Failed)
            {
                throw new GraphDBException(attrValue.Errors);
            }

            if (attrValue.Value == null)
            {
                return null;
            }
            else if (!(attrValue.Value is IReferenceEdge))
            {
                throw new GraphDBException(new Error_InvalidEdgeType(attrValue.GetType(), typeof(IReferenceEdge)));
            }

            List<DBObjectReadout> readouts = new List<DBObjectReadout>();
            var typeName = _DBContext.DBTypeManager.GetTypeByUUID(edgeKey.TypeUUID).Name;
            foreach (var reference in (attrValue.Value as IReferenceEdge).GetAllUUIDs())
            {
                var specialAttributes = new Dictionary<string, object>();
                specialAttributes.Add(SpecialTypeAttribute_UUID.AttributeName, reference);
                specialAttributes.Add(SpecialTypeAttribute_TYPE.AttributeName, typeName);

                readouts.Add(new DBObjectReadout(specialAttributes));
            }


            return new Edge(readouts, _DBContext.DBTypeManager.GetTypeAttributeByEdge(edgeKey).GetDBType(_DBContext.DBTypeManager).Name);
        }

        private Object GetAttributeValue(GraphDBType myType, TypeAttribute myTypeAttribute, DBObjectStream myDBObject, LevelKey myLevelKey)
        {
            if (myTypeAttribute.TypeCharacteristics.IsBackwardEdge)
            {

                #region IsBackwardEdge

                EdgeKey edgeKey = myTypeAttribute.BackwardEdgeDefinition;
                var contBackwardExcept = myDBObject.ContainsBackwardEdge(edgeKey, _DBContext, _DBObjectCache, myType);

                if (contBackwardExcept.Failed)
                    throw new GraphDBException(contBackwardExcept.Errors);

                if (contBackwardExcept.Value)
                {
                    var getBackwardExcept = myDBObject.GetBackwardEdges(edgeKey, _DBContext, _DBObjectCache, myTypeAttribute.GetDBType(_DBContext.DBTypeManager));

                    if (getBackwardExcept.Failed)
                        throw new GraphDBException(getBackwardExcept.Errors);

                    return getBackwardExcept.Value;
                }

                #endregion

            }
            else if (myDBObject.HasAttribute(myTypeAttribute.UUID, myType, _SessionToken))
            {

                #region ELSE (!IsBackwardEdge)

                return myDBObject.GetAttribute(myTypeAttribute.UUID, myType, _DBContext);

                #endregion

            }

            return null;
        }

        /// <summary>
        /// Resolves an attribute 
        /// </summary>
        /// <param name="attrDefinition">The TypeAttribute</param>
        /// <param name="attributeValue">The attribute value, either a AListReferenceEdgeType or ASingleEdgeType or a basic object (Int, String, ...)</param>
        /// <param name="myDepth">The current depth defined by a setting or in the select</param>
        /// <param name="currentLvl">The current level (for recursive resolve)</param>
        /// <returns>List&lt;DBObjectReadout&gt; for user defined list attributes; DBObjectReadout for reference attributes, Object for basic type values </returns>
        private object ResolveAttributeValue(TypeAttribute attrDefinition, object attributeValue, Int64 myDepth, LevelKey myLevelKey, DBObjectStream mySourceDBObject, String reference, Boolean myUsingGraph)
        {

            #region attributeValue is not a reference type just return the value

            if (!((attributeValue is ASetReferenceEdgeType) || (attributeValue is ASingleReferenceEdgeType)))
            {
                return attributeValue;
            }

            #endregion

            #region get typeOfAttribute

            var typeOfAttribute = attrDefinition.GetDBType(_DBContext.DBTypeManager);

            // For backwardEdges, the baseType is the type of the DBObjects!
            if (attrDefinition.TypeCharacteristics.IsBackwardEdge)
                typeOfAttribute = _DBContext.DBTypeManager.GetTypeAttributeByEdge(attrDefinition.BackwardEdgeDefinition).GetRelatedType(_DBContext.DBTypeManager);

            #endregion

            #region Get levelKey and UsingGraph

            if (!(attrDefinition is ASpecialTypeAttribute))
            {
                if (myLevelKey.Level == 0)
                {
                    myLevelKey = new LevelKey(new EdgeKey(attrDefinition.RelatedPandoraTypeUUID, attrDefinition.UUID));
                }
                else
                {
                    myLevelKey += new EdgeKey(attrDefinition.RelatedPandoraTypeUUID, attrDefinition.UUID);
                }
            }

            // at some deeper level we could get into graph independend results. From this time, we can use the GUID index rather than asking the graph all the time
            if (myUsingGraph)
                myUsingGraph = _ExpressionGraph.IsGraphRelevant(myLevelKey, mySourceDBObject);

            #endregion

            if (attributeValue is ASetReferenceEdgeType)
            {

                #region SetReference attribute

                IEnumerable<DBObjectReadout> resultList = null;

                var edge = ((ASetReferenceEdgeType)attributeValue);

                if (myUsingGraph)
                {
                    var dbos = _ExpressionGraph.Select(myLevelKey, mySourceDBObject, true);

                    resultList = GetReadouts(typeOfAttribute, edge, dbos, myDepth, myLevelKey, reference, myUsingGraph);
                }
                else
                {
                    resultList = GetReadouts(typeOfAttribute, edge, myDepth, myLevelKey, reference, myUsingGraph);
                }

                return new Edge(resultList, typeOfAttribute.Name);

                #endregion

            }
            else if (attributeValue is ASingleReferenceEdgeType)
            {

                #region Single reference

                attributeValue = new Edge((attributeValue as ASingleReferenceEdgeType).GetReadout(a =>
                {
                    var dbStream = _DBObjectCache.LoadDBObjectStream(typeOfAttribute, a);
                    if (dbStream.Failed)
                    {
                        throw new GraphDBException(dbStream.Errors);
                    }

                    return new DBObjectReadout(GetAllAttributesFromDBO(dbStream.Value, typeOfAttribute, myDepth, myLevelKey, reference, myUsingGraph));
                }), typeOfAttribute.Name);

                return attributeValue;

                #endregion

            }
            else
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }

        /// <summary>
        /// Gets all selected attributes of an <paramref name="aDBObject"/> or on asterisk all attributes
        /// </summary>
        /// <param name="aDBObject"></param>
        /// <param name="typeOfAttribute"></param>
        /// <param name="myDepth"></param>
        /// <param name="myLevelKey"></param>
        /// <param name="reference"></param>
        /// <param name="myUsingGraph"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetAllAttributesFromDBO(DBObjectStream aDBObject, GraphDBType typeOfAttribute, Int64 myDepth, LevelKey myLevelKey, String reference, Boolean myUsingGraph)
        {

            Dictionary<string, object> Attributes = new Dictionary<string, object>();

            var selections = getAttributeSelections(reference, typeOfAttribute, myLevelKey);

            if (selections.IsNullOrEmpty() && myLevelKey.Level > 0) // 
            {

                #region Get all attributes from the DBO if nothing special was selected

                AddAttributesByDBO(ref Attributes, typeOfAttribute, aDBObject, myDepth, myLevelKey, reference, myUsingGraph);

                #endregion

            }
            else
            {

                #region Get all attribute values from DBO by the selections

                foreach (var sel in selections)
                {
                    TypeAttribute typeAttribute = sel.Element;

                    if (aDBObject.HasAttribute(typeAttribute.UUID, typeOfAttribute, _SessionToken))
                    {
                        Object attributeVal = null;
                        if (GetAttributeValueAndResolve(typeOfAttribute, typeAttribute, aDBObject, myDepth, myLevelKey, reference, myUsingGraph, out attributeVal))
                            Attributes.Add(typeAttribute.Name, attributeVal);
                    }
                }

                #endregion

            }

            return Attributes;
        }

        #endregion

        /// <summary>
        /// This will return the result of all examined DBOs, including calculated aggregates and groupings.
        /// The Value of the returned PandoraResult is of type IEnumerable&lt;DBObjectReadout&gt;
        /// </summary>
        /// <param name="typeNode">The current typeNode</param>
        /// <param name="dbObjectReadouts">The precalculated return objects</param>
        /// <returns>A PandoraResult including the ResultType with the occured errors.</returns>
        public IEnumerable<DBObjectReadout> GetResult(ATypeNode typeNode, IEnumerable<DBObjectReadout> dbObjectReadouts, Boolean isWhereExpressionDependent = false)
        {

            if (!_Aggregates.IsNullOrEmpty() && _Groupings.IsNullOrEmpty())
            //if (_AggregateNodes != null && _GroupingAttr == null)
            
            #region We have only aggregates!

            {

                var Attributes = new Dictionary<String, Object>();

                foreach (var intAttr in _Aggregates)
                {

                    var aggrNode = (AggregateNode)intAttr.StructureNode;
                    if (intAttr.IndexAggregate != null && !isWhereExpressionDependent)
                    {
                        #region Aggregate on index and no where expression

                        var res = aggrNode.Aggregate.Aggregate(intAttr.IndexAggregate, aggrNode.ContainingIDNodes.First().Reference.Item2, _DBContext, _DBObjectCache, _SessionToken);
                        if (res.Failed)
                        {
                            throw new GraphDBException(res.Errors);
                        }
                        Attributes.Add(aggrNode.Alias, res.Value);

                        #endregion
                    }
                    else
                    {
                        #region Aggregate on the dbObjectReadouts created by the select run

                        var res = aggrNode.Aggregate.Aggregate(dbObjectReadouts, (aggrNode.Expressions[0] as IDNode).LastAttribute, _DBContext, _DBObjectCache, _SessionToken);
                        if (res.Failed)
                        {
                            throw new GraphDBException(res.Errors);
                        }

                        Attributes.Add(aggrNode.Alias, res.Value);

                        #endregion
                    }

                }

                //_DBOs = new IEnumerable<DBObjectReadout>();
                //_DBOs.Add(new DBObjectReadoutGroup(Attributes, new DBObjectSystemData()));

                yield return new DBObjectReadoutGroup(Attributes);

            }

            #endregion

            else //if (_AggregateNodes != null || _GroupingAttr != null)
                if (!_Aggregates.IsNullOrEmpty() || !_Groupings.IsNullOrEmpty())
                #region We have aggregates and/or groupings
                {

                    foreach (var keyValPair in _GroupingStructure[typeNode.Reference].GroupsAndAggregates)
                    {

                        var Attributes = new Dictionary<string, object>();

                        foreach (var groupKeyValPair in keyValPair.Key.Values)
                        {

                            var theAttribute = _GroupingStructure[typeNode.Reference].DBTypeStream.GetTypeAttributeByUUID(groupKeyValPair.Key);
                            if (theAttribute != null)
                            {
                                Attributes.Add(theAttribute.Name, groupKeyValPair.Value.GetReadoutValue());
                            }
                            else
                            {
                                var Setting = _DBContext.DBSettingsManager.GetSetting(new AttributeUUID(groupKeyValPair.Key.ToString()), _DBContext, TypesSettingScope.TYPE, _GroupingStructure[typeNode.Reference].DBTypeStream).Value;
                                if (Setting != null)
                                    Attributes.Add(Setting.Name, groupKeyValPair.Value.GetReadoutValue());
                            }
                        }

                        #region Calculate aggregate

                        if (!_Aggregates.IsNullOrEmpty())
                        {
                            foreach (var intAttr in _Aggregates)
                            {
                                var aggrNode = (AggregateNode)intAttr.StructureNode;
                                var res = aggrNode.Aggregate.Aggregate(keyValPair.Value, (aggrNode.Expressions[0] as IDNode).LastAttribute, _DBContext, _DBObjectCache, _SessionToken);
                                if (res.Failed)
                                {
                                    //return new Exceptional<IEnumerable<DBObjectReadout>>(res.Errors);
                                }
                                Attributes.Add(aggrNode.Alias, res.Value);
                            }
                        }

                        #endregion

                        var _DBObjectReadoutGroup = new DBObjectReadoutGroup(Attributes);
                        _DBObjectReadoutGroup.CorrespondingDBObjects = keyValPair.Value;

                        #region Check for having expressions and evaluate them

                        if (_HavingExpression == null)
                        {
                            //_DBOs.Add(readoutGroup);
                            yield return _DBObjectReadoutGroup;
                        }
                        else
                        {
                            var res = _HavingExpression.IsSatisfyHaving(_DBObjectReadoutGroup, _DBContext);
                            if (res.Failed)
                                throw new GraphDBException(res.Errors);
                            else if (res.Value)
                                yield return _DBObjectReadoutGroup;
                        }

                        #endregion

                    }

                }

                #endregion

                //return _DBOs;
                else
                {
                    foreach (var dbo in dbObjectReadouts)
                        yield return dbo;
                }

        }

        public IEnumerable<DBObjectReadout> GetTypeIndependendResult()
        {

            //_DBOs = new IEnumerable<DBObjectReadout>();
            var Attributes = new Dictionary<string, object>();

            #region Go through all _SelectionElementsTypeIndependend

            foreach (var selection in _SelectionElementsTypeIndependend)
            {
                if (selection.StructureNode is FuncCallNode)
                {
                    FuncCallNode fcn = ((FuncCallNode)selection.StructureNode);
                    var res = fcn.Execute(null, null, null, _DBContext, null, _DBObjectCache, _SessionToken);
                    if (res.Success)
                    {
                        if (res.Value.Value == null)
                            continue; // no result for this object because of not set attribute value

                        Attributes.Add(fcn.Alias, ((FuncParameter)res.Value).Value.GetReadoutValue());
                    }
                    else
                    {
                        //return new Exceptional<IEnumerable<DBObjectReadout>>(res.Errors);
                        throw new GraphDBException(res.Errors);
                    }

                }
            }

            #endregion

            if (!Attributes.IsNullOrEmpty())
            {
                DBObjectReadout tempROObject = new DBObjectReadout(Attributes);
                //_DBOs.Add(tempROObject);
                yield return tempROObject;
            }

        }

        public void Clear()
        {
            _DBOsEnumerable = new List<DBObjectReadout>();
        }

        public Dictionary<String, String> GetSelectedAttributesList()
        {

            var retVal = new Dictionary<String, String>();
            
            foreach (var selType in _Selections)
            {
                foreach (var sel in selType.Value)
                {
                    foreach (var selElem in sel.Value)
                    {
                        if (selElem.Element != null && !selElem.IsGroupedOrAggregated)
                        {
                            if (!retVal.ContainsKey(selElem.Element.Name))
                                retVal.Add(selElem.Element.Name, selElem.Alias);
                        }
                        else if (selElem.IsAsterisk)
                        {
                            foreach (var attr in _DBContext.DBTypeManager.GetTypeByUUID(sel.Key.Edges[0].TypeUUID).GetAllAttributes(_DBContext))
                            {
                                if (!retVal.ContainsKey(attr.Name))
                                    retVal.Add(attr.Name, attr.Name);
                            }
                        }
                       
                    }
                }
            }

            #region Aggregates

            if (!_Aggregates.IsNullOrEmpty())
            {
                foreach (var selElem in _Aggregates)
                {
                    retVal.Add((selElem.StructureNode as AggregateNode).SourceParsedString, selElem.Alias);
                }
            }


            #endregion

            return retVal;
        }
    }
}
