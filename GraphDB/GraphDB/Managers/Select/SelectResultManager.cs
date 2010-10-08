/* <id name="GraphDB – SelectResultManager" />
 * <copyright file="SelectResultManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This will create the result of any kind of select - working on an IExpressionGraph.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;

using sones.GraphFS.DataStructures;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Managers.TypeManagement.BasicTypes;
using sones.GraphDB.NewAPI;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.Managers.Select
{

    public class SelectResultManager
    {

        #region Data

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

        /// <summary>
        /// Dictionary of the base type and the selections of this type: e.g. U.Name and U.Cars.Color are both of basetype User.
        /// reference [LevelKey, [selection]]
        /// With that, we need to 'work' on each attribute only one time!
        /// </summary>
        /// The Level is the Level of LevelKey: e.g. U.Name == 0 and U.Cars.Color == 1
        /// If the User selected U.Name, U.Age than we have two SelectionElements in level 0 of type User
        /// In case of: "FROM User SELECT Friends.TOP(2).TOP(1).Friends.TOP(2).Name, Friends.TOP(2).TOP(1).Name, Name WHERE Name = 'Lila'"
        /// we have for reference 'User' the edgeList for Name, Friends (with Name and Friends) and Friends.Friends (with Name)
        Dictionary<String, Dictionary<EdgeList, List<SelectionElement>>> _Selections;


        List<SelectionElement> _SelectionElementsTypeIndependend;

        /// <summary>
        /// the selection element of _Selections , the aggregate selection element
        /// </summary>
        //List<SelectionElementAggregate> _Aggregates;
        Dictionary<String, Dictionary<EdgeList, List<SelectionElementAggregate>>> _Aggregates;
        Dictionary<String, List<SelectionElement>> _Groupings;

        BinaryExpressionDefinition _HavingExpression;

        /// <summary>
        /// Used to cache lookups
        /// </summary>
        Dictionary<GraphDBType, List<TypeAttribute>> _BackwardEdgeAttributesByType = new Dictionary<GraphDBType, List<TypeAttribute>>();

        /// <summary>
        /// Used to cache lookups
        /// </summary>
        Dictionary<GraphDBType, List<TypeAttribute>> _SpecialAttributesByType      = new Dictionary<GraphDBType, List<TypeAttribute>>();

        #endregion

        #endregion

        #region Ctor

        public SelectResultManager(DBContext dbContext)
        {

            _DBContext                        = dbContext;
            _SettingCache                     = new SelectSettingCache();
            _Aggregates                       = new Dictionary<string, Dictionary<EdgeList, List<SelectionElementAggregate>>>();
            _Groupings                        = new Dictionary<string, List<SelectionElement>>();
            _Selections                       = new Dictionary<string, Dictionary<EdgeList, List<SelectionElement>>>();
            _SelectionElementsTypeIndependend = new List<SelectionElement>();


        }

        #endregion

        #region Adding elements to selection

        /// <summary>
        /// Adds the typeNode as an asterisk *, rhomb # or minus - or ad
        /// </summary>
        /// <param name="typeNode"></param>
        public Exceptional AddSelectionType(String myReference, GraphDBType myType, TypesOfSelect mySelType, TypeUUID myTypeID = null)
        {
            var selElem = new SelectionElement(mySelType, myTypeID);

            if (!_Selections.ContainsKey(myReference))
                _Selections.Add(myReference, new Dictionary<EdgeList, List<SelectionElement>>());

            var level = new EdgeList(new EdgeKey(myType.UUID, null));

            if (!_Selections[myReference].ContainsKey(level))
                _Selections[myReference].Add(level, new List<SelectionElement>());

            if (!_Selections[myReference][level].Exists(item => item.Selection == mySelType))
            {
                _Selections[myReference][level].Add(selElem);
            }

            return Exceptional.OK;

        }
        
        /// <summary>
        /// Single IDNode selection attribute
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myAlias"></param>
        /// <param name="myAStructureNode"></param>
        /// <param name="myGraphType"></param>
        public Exceptional AddElementToSelection(string myAlias, String myReference, IDChainDefinition myIDChainDefinition, Boolean myIsGroupedOrAggregated, SelectValueAssignment mySelectValueAssignment = null)
        {
            SelectionElement lastElem = null;

            var curLevel = new EdgeList();
            EdgeList preLevel = null;

            if (myReference != null && _Selections.ContainsKey(myReference) && _Selections[myReference].Any(kv => kv.Value.Any(se => se.RelatedIDChainDefinition == myIDChainDefinition && se.Alias == myAlias)))
            {
                return new Exceptional(new Error_DuplicateAttributeSelection(myAlias));
            }

            foreach (var nodeEdgeKey in myIDChainDefinition)
            {

                if (nodeEdgeKey is ChainPartTypeOrAttributeDefinition)
                {

                    #region Usual attribute 

                    preLevel = null;

                    var selElem = new SelectionElement(myAlias, curLevel, myIsGroupedOrAggregated, myIDChainDefinition);

                    var typeOrAttr = (nodeEdgeKey as ChainPartTypeOrAttributeDefinition);

                    if (true || typeOrAttr.DBType != null && typeOrAttr.TypeAttribute != null)
                    {
                        #region defined

                        var edgeKey = typeOrAttr.EdgeKey;
                        selElem.Element = typeOrAttr.TypeAttribute; //_DBContext.DBTypeManager.GetTypeAttributeByEdge(edgeKey);

                        if (String.IsNullOrEmpty(selElem.Alias) || (nodeEdgeKey.Next != null && !(nodeEdgeKey.Next is ChainPartFuncDefinition)))
                        {
                            selElem.Alias = typeOrAttr.TypeAttribute.Name;//_DBContext.DBTypeManager.GetTypeAttributeByEdge(edgeKey).Name;
                        }

                        curLevel += edgeKey;
                        preLevel = curLevel.GetPredecessorLevel(); 

                        #endregion
                    }
                    else
                    {
                        #region undefined attribute
                        
                        if (myIDChainDefinition.Level == 0)
                        {
                            preLevel = new EdgeList(myIDChainDefinition.LastType.UUID);
                        }
                        else
                        {
                            var element = _Selections[myReference].Last();

                            preLevel = curLevel.GetPredecessorLevel();
                            //preLevel = curLevel;
                        }
                        selElem.Alias = typeOrAttr.TypeOrAttributeName;
                        selElem.Element = new UndefinedTypeAttribute(typeOrAttr.TypeOrAttributeName);

                        #endregion
                    }

                    #region Add to _Selections if valid

                    if (!_Selections.ContainsKey(myReference))
                    {
                        _Selections.Add(myReference, new Dictionary<EdgeList, List<SelectionElement>>());
                    }

                    if (!_Selections[myReference].ContainsKey(preLevel))
                    {
                        _Selections[myReference].Add(preLevel, new List<SelectionElement>());
                    }

                    ///
                    /// Duplicate AttributeSelection is: "U.Name, U.Name" or "U.Name.TOUPPER(), U.Name" but not "U.Friends.TOP(1).Name, U.Friends.TOP(1).Age"
                    if ((nodeEdgeKey.Next == null || (nodeEdgeKey.Next is ChainPartFuncDefinition && nodeEdgeKey.Next.Next == null))
                        //                                                        U.Name, U.Name                  U.Name.TOUPPER, U.Name
                        && _Selections[myReference][preLevel].Exists(item => item.Alias == selElem.Alias && selElem.EdgeList.Level == item.EdgeList.Level && item.RelatedIDChainDefinition.Depth == selElem.RelatedIDChainDefinition.Depth && item.Element != null) && !myIsGroupedOrAggregated)
                    {
                        return new Exceptional(new Error_DuplicateAttributeSelection(selElem.Alias));
                    }

                    // Do not add again if:
                    // - it is a defined attribute and there is an asterisk selection at this level
                    // - it is not the last part AND
                    // - there is already an item of this part with the same alias and the same Depth
                    if (nodeEdgeKey.Next == null || _Selections[myReference][preLevel].Count == 0 || _Selections[myReference][preLevel].Any(item => IsNewSelectionElement(item, selElem)))
                    {
                        _Selections[myReference][preLevel].Add(selElem);
                    }

                    #endregion

                    lastElem = selElem;

                    #endregion

                }
                else if (nodeEdgeKey is ChainPartFuncDefinition)
                {

                    var chainPartFuncDefinition = (nodeEdgeKey as ChainPartFuncDefinition);

                    #region Function

                    if (myReference == null)
                    {

                        #region Type independent functions

                        var selElem = new SelectionElement(myAlias, myIDChainDefinition);
                        if (String.IsNullOrEmpty(selElem.Alias))
                        {
                            selElem.Alias = chainPartFuncDefinition.SourceParsedString;
                        }
                        var funcElem = new SelectionElementFunction(selElem, chainPartFuncDefinition, chainPartFuncDefinition.Parameters);

                        if (lastElem is SelectionElementFunction)
                        {
                            (lastElem as SelectionElementFunction).AddFollowingFunction(funcElem);
                            lastElem = funcElem;
                        }
                        else
                        {
                            if (_SelectionElementsTypeIndependend.Any(se => se.Alias == funcElem.Alias))
                            {
                                return new Exceptional(new Error_DuplicateAttributeSelection(funcElem.Alias));
                            }

                            _SelectionElementsTypeIndependend.Add(funcElem);
                            lastElem = funcElem;
                        }

                        #endregion

                    }
                    else
                    {

                        #region Type dependent function

                        var funcElem = new SelectionElementFunction(lastElem, (nodeEdgeKey as ChainPartFuncDefinition), (nodeEdgeKey as ChainPartFuncDefinition).Parameters);
                        funcElem.RelatedIDChainDefinition = myIDChainDefinition;

                        if (!String.IsNullOrEmpty(myAlias) && nodeEdgeKey.Next == null)
                        {
                            funcElem.Alias = myAlias;
                        }

                        if (lastElem is SelectionElementFunction)
                        {
                            (lastElem as SelectionElementFunction).AddFollowingFunction(funcElem);
                            lastElem = funcElem;
                        }
                        else if (_Selections[myReference][preLevel].Contains(lastElem))
                        {

                            #region Add function to the last selection element (replace it)

                            _Selections[myReference][preLevel].Remove(lastElem);

                            //lastElem = new SelectionElementFunction(lastElem, (nodeEdgeKey as ChainPartFuncDefinition), (nodeEdgeKey as ChainPartFuncDefinition).Parameters);
                            //lastElem.RelatedIDChainDefinition = myIDChainDefinition;

                            //if (!String.IsNullOrEmpty(alias) && nodeEdgeKey.Next == null)
                            //{
                            //    lastElem.Alias = alias;
                            //}
                            lastElem = funcElem;

                            if (!_Selections[myReference][preLevel].Contains(lastElem)) // In case this Element with func is already in the selection list do nothing.
                            {

                                _Selections[myReference][preLevel].Add(lastElem);

                            }

                            #endregion

                        }
                        else if (!_Selections[myReference][preLevel].Contains(funcElem))
                        {

                            #region In this case we have a similar function but NOT THE SAME. Since we don't know what to do, return error.

                            return new Exceptional(new Error_InvalidAttributeSelection(myIDChainDefinition.ContentString));

                            #endregion

                        }

                        #endregion

                    }
                    
                    #endregion

                }

            }

            #region Set the SelectValueAssignment for the last element

            if (lastElem != null && mySelectValueAssignment != null)
            {

                #region Error handling

                System.Diagnostics.Debug.Assert(lastElem.Element != null);

                if (lastElem.Element.IsUserDefinedType(_DBContext.DBTypeManager))
                {
                    return new Exceptional(new Error_InvalidSelectValueAssignment(lastElem.Element.Name));
                }

                if (!(mySelectValueAssignment.TermDefinition is ValueDefinition))
                {
                    return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                #endregion

                #region Validate datatype if the attribute is a defined attribute

                if (!(lastElem.Element is UndefinedTypeAttribute))
                {

                    if (!lastElem.Element.GetADBBaseObjectType(_DBContext.DBTypeManager).IsValidValue((mySelectValueAssignment.TermDefinition as ValueDefinition).Value.Value))
                    {
                        return new Exceptional(new Error_SelectValueAssignmentDataTypeDoesNotMatch(lastElem.Element.GetADBBaseObjectType(_DBContext.DBTypeManager).ObjectName, (mySelectValueAssignment.TermDefinition as ValueDefinition).Value.ObjectName));
                    }
                    var typedValue = new ValueDefinition(lastElem.Element.GetADBBaseObjectType(_DBContext.DBTypeManager).Clone((mySelectValueAssignment.TermDefinition as ValueDefinition).Value.Value));
                    mySelectValueAssignment.TermDefinition = typedValue;
                }

                #endregion

                lastElem.SelectValueAssignment = mySelectValueAssignment;
            }


            #endregion

            return Exceptional.OK;

        }

        /// <summary>
        /// The element <paramref name="myNewElement"/> is new if
        /// - it is not a special type attribute and there is an asterisk selection at this level
        /// - it is not the last part AND there is already an item of this part with the same alias and the same Depth
        /// </summary>
        /// <param name="myExistingElement"></param>
        /// <param name="myNewElement"></param>
        /// <returns></returns>
        private Boolean IsNewSelectionElement(SelectionElement myExistingElement, SelectionElement myNewElement)
        {

            #region The existing is an asterisk and the new one is NOT a SpecialTypeAttribute
		
            if (myExistingElement.Selection == TypesOfSelect.Asterisk && !myNewElement.RelatedIDChainDefinition.IsSpecialTypeAttribute())
            {
                return false;
            }
 
	        #endregion

            #region The same level is already selected

            var item = myExistingElement;
            var selElem = myNewElement;

            if (item.Alias == selElem.Alias &&
                // same depth                                                                 or Undefined and one lower depth Friends.Undefined is Depth 1 and Friends.Age is Depth 2 but at the same selection level
                (item.RelatedIDChainDefinition.Depth == selElem.RelatedIDChainDefinition.Depth || (selElem.RelatedIDChainDefinition.IsUndefinedAttribute && item.RelatedIDChainDefinition.Depth == selElem.RelatedIDChainDefinition.Depth + 1))
                )
            {
                return false;
            }

            #endregion

            return true;

        }

        /// <summary>
        /// Adds an aggregate to the selection. It will check whether it is an index aggregate or not.
        /// Aggregates on attributes with level > 1 will return an error.
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myAlias"></param>
        /// <param name="myAStructureNode"></param>
        /// <param name="myGraphType"></param>
        public Exceptional AddAggregateElementToSelection(string myAlias, string myReference, SelectionElementAggregate mySelectionPartAggregate)
        {

            if (mySelectionPartAggregate.EdgeList.Level > 1)
            {
                return new Exceptional(new Error_AggregateIsNotValidOnThisAttribute(mySelectionPartAggregate.ToString()));
            }

            #region Check for index aggregate

            foreach (var edge in mySelectionPartAggregate.EdgeList.Edges)
            {

                #region COUNT(*)

                if (edge.AttrUUID == null)
                {
                    mySelectionPartAggregate.IndexAggregate = _DBContext.DBTypeManager.GetTypeByUUID(edge.TypeUUID).GetUUIDIndex(_DBContext.DBTypeManager);
                    mySelectionPartAggregate.Element = _DBContext.DBTypeManager.GetUUIDTypeAttribute();
                }

                #endregion

                else
                {
                    // if the GetAttributeIndex did not return null we will pass this as the aggregate operation value
                    mySelectionPartAggregate.IndexAggregate = _DBContext.DBTypeManager.GetTypeByUUID(edge.TypeUUID).GetAttributeIndex(edge.AttrUUID, null).Value;
                    mySelectionPartAggregate.Element = _DBContext.DBTypeManager.GetTypeAttributeByEdge(edge);
                }
            }

            #endregion

            if (!_Aggregates.ContainsKey(myReference))
            {
                _Aggregates.Add(myReference, new Dictionary<EdgeList,List<SelectionElementAggregate>>());
            }

            var level = mySelectionPartAggregate.EdgeList.GetPredecessorLevel();
            if (!_Aggregates[myReference].ContainsKey(level))
            {
                _Aggregates[myReference].Add(level, new List<SelectionElementAggregate>());
            }

            _Aggregates[myReference][level].Add(mySelectionPartAggregate);

            return Exceptional.OK;

        }

        /// <summary>
        /// Adds a group element to the selection and validat it
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myIDChainDefinition"></param>
        public Exceptional AddGroupElementToSelection(string myReference, IDChainDefinition myIDChainDefinition)
        {

            if (myIDChainDefinition.Edges.Count > 1)
            {
                return new Exceptional(new Error_InvalidGroupByLevel(myIDChainDefinition));
            }

            // if the grouped attr is not selected
            if ((!_Selections.ContainsKey(myReference) || !_Selections[myReference].Any(l => l.Value.Any(se => se.Element != null && se.Element == myIDChainDefinition.LastAttribute))) && !myIDChainDefinition.IsUndefinedAttribute)
            {
                return new Exceptional(new Error_GroupedAttributeIsNotSelected(myIDChainDefinition.LastAttribute));
            }
            else
            {
                if (!_Groupings.ContainsKey(myReference))
                {
                    _Groupings.Add(myReference, new List<SelectionElement>());
                }
                _Groupings[myReference].Add(new SelectionElement(myReference, new EdgeList(myIDChainDefinition.Edges), false, myIDChainDefinition, myIDChainDefinition.LastAttribute));
            }

            return Exceptional.OK;
        }

        /// <summary>
        /// Adds a having to the selection
        /// </summary>
        /// <param name="myHavingExpression"></param>
        public void AddHavingToSelection(BinaryExpressionDefinition myHavingExpression)
        {
            _HavingExpression = myHavingExpression;
        }

        #endregion

        #region Examines the DBOs

        /// <summary>
        /// Examine a TypeNode to a specific <paramref name="myResolutionDepth"/> by using the underlying graph or the type guid index
        /// </summary>
        /// <param name="myResolutionDepth">The depth to which the reference attributes should be resolved</param>
        /// <param name="myTypeNode">The type node which should be examined on selections</param>
        /// <param name="myUsingGraph">True if there is a valid where expression of this typeNode</param>
        /// <returns>True if succeeded, false if there was nothing to select for this type</returns>
        public Exceptional<Boolean> Examine(Int64 myResolutionDepth, String myReference, GraphDBType myReferencedDBType, Boolean myUsingGraph, ref IEnumerable<Vertex> myVertices)
        {

            if ((!_Selections.ContainsKey(myReference) || !_Selections[myReference].ContainsKey(new EdgeList(myReferencedDBType.UUID))) && _Aggregates.IsNullOrEmpty())
            {
                return new Exceptional<bool>(false);
            }

            var levelKey = new EdgeList(myReferencedDBType.UUID);

            myVertices  = ExamineVertex(myResolutionDepth, myReference, myReferencedDBType, levelKey, myUsingGraph);

            return new Exceptional<Boolean>(true);

        }

        /// <summary>
        /// This is the main function. It will check all selections on this type and will create the readouts
        /// </summary>
        /// <param name="dbos"></param>
        /// <param name="myResolutionDepth"></param>
        /// <param name="myTypeNode"></param>
        /// <param name="myLevelKey"></param>
        /// <param name="myUsingGraph">True if for all selects the graph will be asked for DBOs</param>
        /// <param name="myDBObjectCache"></param>
        /// <param name="mySessionToken"></param>
        /// <returns></returns>
        private IEnumerable<Vertex> ExamineVertex(long myResolutionDepth, String myReference, GraphDBType myReferencedDBType, EdgeList myLevelKey, bool myUsingGraph)
        {

            #region Get all selections and aggregates for this reference, type and level
            
            var _Selections = getAttributeSelections(myReference, myReferencedDBType, myLevelKey);
            var aggregates = getAttributeAggregates(myReference, myReferencedDBType, myLevelKey);

            #endregion

            if (
                (_Selections.IsNotNullOrEmpty() && _Selections.All(s => s.IsReferenceToSkip(myLevelKey)) && aggregates.IsNotNullOrEmpty() && aggregates.All(a => a.IsReferenceToSkip(myLevelKey)))
                || (_Selections.IsNotNullOrEmpty() && _Selections.All(s => s.IsReferenceToSkip(myLevelKey)) && aggregates.IsNullOrEmpty())
               )

            {

                #region If there are only references in this level, we will skip this level (and add the attribute as placeholder) and step to the next one

                var Attributes = new Dictionary<String, Object>();

                foreach (var _Selection in _Selections)
                {
                    
                    var edgeKey = new EdgeKey(_Selection.Element.RelatedGraphDBTypeUUID, _Selection.Element.UUID);

                    Attributes.Add(_Selection.Alias, new Edge(null,
                                                              ExamineVertex(myResolutionDepth, myReference, myReferencedDBType, myLevelKey + edgeKey, myUsingGraph))
                                                              { EdgeTypeName = _Selection.Element.GetDBType(_DBContext.DBTypeManager).Name });

                }

                yield return new Vertex(Attributes);

                #endregion

            }

            else
            {

                #region Otherwise load all dbos until this level and return them

                #region Get dbos enumerable of the first level - either from ExpressionGraph or via index

                IEnumerable<Exceptional<DBObjectStream>> dbos;
                if (myUsingGraph)
                {
                    dbos = _ExpressionGraph.Select(new LevelKey(myLevelKey.Edges, _DBContext.DBTypeManager), null, true);
                }
                else // using GUID index
                {
                    dbos = _DBContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(myLevelKey.Edges, _DBContext.DBTypeManager), _DBContext);
                }

                #endregion

                if (aggregates.IsNotNullOrEmpty())
                {
                    foreach (var val in ExamineDBO_Aggregates(dbos, aggregates, _Selections, myReferencedDBType, myUsingGraph))
                    {
                        if (val != null)
                        {
                            yield return val;
                        }
                    }
                }

                else if (_Groupings.IsNotNullOrEmpty())
                {
                    foreach (var val in ExamineDBO_Groupings(dbos, _Selections, myReferencedDBType))
                    {
                        if (val != null)
                        {
                            yield return val;
                        }
                    }
                }

                else if (!_Selections.IsNullOrEmpty())
                {

                    Boolean isRefTypeVertex = myReferencedDBType.UUID.Equals(DBVertex.UUID);
                    GraphDBType refType;
                    
                    #region Usually attribute selections

                    foreach (var aDBObject in dbos)
                    {
                        if (aDBObject.Failed())
                        {
                            // since we are in an yield we can do nothing else than throw a exception
                            throw new GraphDBException(aDBObject.IErrors);
                        }

                        #region Create a readoutObject for this DBO and yield it: on failure throw an exception

                        if (isRefTypeVertex)
                        {
                            refType = _DBContext.DBTypeManager.GetTypeByUUID(aDBObject.Value.TypeUUID);
                        }
                        else
                        {
                            refType = myReferencedDBType;
                        }

                        var Attributes = GetAllSelectedAttributesFromVertex(aDBObject.Value, refType, myResolutionDepth, myLevelKey, myReference, myUsingGraph);

                        if (Attributes.IsNotNullOrEmpty())
                        {
                            yield return new Vertex(Attributes);
                        }
                        //else
                        //{
                        //    // we found no attributes, so we return null because currently we do not want to add empty attribute readouts
                        //    yield return (Vertex)null;
                        //}

                        #endregion

                    }

                    #endregion

                }

                #endregion

            }

        }

        /// <summary>
        /// Go through each DBO and aggregate them
        /// </summary>
        /// <param name="myAggregates"></param>
        /// <param name="mySelections"></param>
        /// <param name="myDBOs"></param>
        /// <param name="myReferencedDBType"></param>
        /// <returns></returns>
        private IEnumerable<Vertex> ExamineDBO_Aggregates(IEnumerable<Exceptional<DBObjectStream>> myDBOs, List<SelectionElementAggregate> myAggregates, List<SelectionElement> mySelections, GraphDBType myReferencedDBType, Boolean myUsingGraph)
        {

            #region Aggregate

            if (mySelections.CountIsGreater(0))
            {

                #region Grouping - each selection is grouped (checked prior)

                var aggregatedGroupings = new Dictionary<GroupingKey, SelectionElementAggregate>();

                #region Create groupings using the ILookup

                var groupedDBOs = myDBOs.ToLookup((dbo) =>
                {
                    CheckLoadedDBObjectStream(dbo, myReferencedDBType);

                    #region Create GroupingKey based on the group values and attributes

                    Dictionary<GroupingValuesKey, IObject> groupingVals = new Dictionary<GroupingValuesKey, IObject>();
                    foreach (var selection in mySelections)
                    {
                        var attrValue = dbo.Value.GetAttribute(selection.Element, myReferencedDBType, _DBContext);
                        if (attrValue.Failed())
                        {
                            throw new GraphDBException(attrValue.IErrors);
                        }

                        groupingVals.Add(new GroupingValuesKey(selection.Element, selection.Alias), attrValue.Value);
                    }
                    GroupingKey groupingKey = new GroupingKey(groupingVals);

                    #endregion

                    return groupingKey;

                }, (dbo) =>
                {
                    CheckLoadedDBObjectStream(dbo, myReferencedDBType);

                    return dbo.Value;
                });
                
                #endregion

                foreach (var group in groupedDBOs)
                {

                    #region Create group readouts

                    var aggregatedAttributes = new Dictionary<String, Object>();

                    foreach (var aggr in myAggregates)
                    {
                        var aggrResult = aggr.Aggregate.Aggregate(group as IEnumerable<DBObjectStream>, aggr.Element, _DBContext);
                        if (aggrResult.Failed())
                        {
                            throw new GraphDBException(aggrResult.IErrors);
                        }
                        aggregatedAttributes.Add(aggr.Alias, aggrResult.Value.GetReadoutValue());
                    }

                    foreach (var groupingKeyVal in group.Key.Values)
                    {
                        aggregatedAttributes.Add(groupingKeyVal.Key.AttributeAlias, groupingKeyVal.Value.GetReadoutValue());
                    }

                    var dbObjectReadout = new VertexGroup(aggregatedAttributes, (group as IEnumerable<DBObjectStream>).Select(dbo => new Vertex(GetAllSelectedAttributesFromVertex(dbo, myReferencedDBType, -1, null, null, false, true))));

                    #endregion

                    #region Evaluate having if exist and yield return

                    if (_HavingExpression != null)
                    {
                        var res = _HavingExpression.IsSatisfyHaving(dbObjectReadout, _DBContext);
                        if (res.Failed())
                            throw new GraphDBException(res.IErrors);
                        else if (res.Value)
                            yield return dbObjectReadout;
                    }
                    else
                    {
                        yield return dbObjectReadout;
                    }

                    #endregion

                }
                
                yield break;

                #endregion

            }
            else
            {

                #region No grouping, just aggregates

                var aggregatedAttributes = new Dictionary<String, Object>();
                Vertex _Vertex;

                if (!myUsingGraph && myAggregates.All(a => a.IndexAggregate != null))
                {

                    #region Index aggregates

                    foreach (var aggr in myAggregates)
                    {
                        var idxAggrResult = aggr.Aggregate.Aggregate(aggr.IndexAggregate, myReferencedDBType, _DBContext);
                        if (idxAggrResult.Failed())
                        {
                            throw new GraphDBException(idxAggrResult.IErrors);
                        }
                        aggregatedAttributes.Add(aggr.Alias, idxAggrResult.Value.GetReadoutValue());
                    }

                    _Vertex = new Vertex(aggregatedAttributes);

                    #endregion

                }
                else
                {

                    #region OR Attribute aggregates

                    foreach (var aggr in myAggregates)
                    {
                        var curType = _DBContext.DBTypeManager.GetTypeByUUID(aggr.EdgeList.LastEdge.TypeUUID);
                        var curAttr = curType.GetTypeAttributeByUUID(aggr.EdgeList.LastEdge.AttrUUID);
                        var aggrResult = aggr.Aggregate.Aggregate(myDBOs.Select(dbo =>
                        {
                            CheckLoadedDBObjectStream(dbo, curType, curAttr);
                            return dbo.Value;
                        }), aggr.Element, _DBContext);

                        if (aggrResult.Failed())
                        {
                            throw new GraphDBException(aggrResult.IErrors);
                        }

                        aggregatedAttributes.Add(aggr.Alias, aggrResult.Value.GetReadoutValue());

                    }

                    _Vertex = new Vertex(aggregatedAttributes);

                    #endregion

                }

                #region Check having expression and yield return value

                if (_HavingExpression != null)
                {
                    var res = _HavingExpression.IsSatisfyHaving(_Vertex, _DBContext);
                    if (res.Failed())
                        throw new GraphDBException(res.IErrors);
                    else if (res.Value)
                        yield return _Vertex;
                }
                else
                {
                    yield return _Vertex;
                }

                #endregion

                yield break;

                #endregion

            }

            #endregion

        }

        /// <summary>
        /// This will check the exceptional for errors. Depending on the SettingInvalidReferenceHandling an expcetion will be thrown or false will be return on any load error.
        /// </summary>
        /// <param name="dbStream"></param>
        /// <param name="myLevelKey"></param>
        /// <returns></returns>
        private Boolean CheckLoadedDBObjectStream(Exceptional<DBObjectStream> dbStream, GraphDBType myDBType, TypeAttribute myTypeAttribute = null)
        {

            SettingInvalidReferenceHandling invalidReferenceSetting = null;
            GraphDBType currentType = myDBType;

            if (dbStream.Failed())
            {
                #region error

                #region get setting

                if (invalidReferenceSetting == null)
                {
                    //currentType = _DBContext.DBTypeManager.GetTypeByUUID(myLevelKey.LastEdge.TypeUUID);

                    //if (myLevelKey.LastEdge.AttrUUID != null)
                    if (myTypeAttribute != null)
                    {
                        invalidReferenceSetting = (SettingInvalidReferenceHandling)_DBContext.DBSettingsManager.GetSetting(SettingInvalidReferenceHandling.UUID, _DBContext, TypesSettingScope.ATTRIBUTE, currentType, myTypeAttribute).Value; // currentType.GetTypeAttributeByUUID(myLevelKey.LastEdge.AttrUUID)).Value;
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

                        return false;

                        #endregion
                    case BehaviourOnInvalidReference.log:

                    default:

                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
                }

                #endregion
            }

            return true;

        }

        /// <summary>
        ///  Group all DBOs and return the readouts
        /// </summary>
        /// <param name="myDBObjectStreams"></param>
        /// <param name="mySelections"></param>
        /// <param name="myReferencedDBType"></param>
        /// <returns></returns>
        private IEnumerable<Vertex> ExamineDBO_Groupings(IEnumerable<Exceptional<DBObjectStream>> myDBObjectStreams, List<SelectionElement> mySelections, GraphDBType myReferencedDBType)
        {

            #region Create groupings using the ILookup

            var _GroupedVertices = myDBObjectStreams.ToLookup((dbo) =>
            {

                CheckLoadedDBObjectStream(dbo, myReferencedDBType);

                #region Create GroupingKey based on the group values and attributes

                var groupingVals = new Dictionary<GroupingValuesKey, IObject>();

                foreach (var selection in mySelections)
                {

                    if (!dbo.Value.HasAttribute(selection.Element, _DBContext))
                    {
                        continue;
                    } 
                    
                    var attrValue = dbo.Value.GetAttribute(selection.Element, myReferencedDBType, _DBContext);
                    if (attrValue.Failed())
                    {
                        throw new GraphDBException(attrValue.IErrors);
                    }

                    groupingVals.Add(new GroupingValuesKey(selection.Element, selection.Alias), attrValue.Value);
                
                }
                
                GroupingKey groupingKey = new GroupingKey(groupingVals);

                #endregion

                return groupingKey;

            }, (dbo) =>
            {
                CheckLoadedDBObjectStream(dbo, myReferencedDBType);

                return dbo.Value;
            });

            #endregion

            foreach (var group in _GroupedVertices)
            {

                #region No valid grouping keys found

                if (group.Key.Values.IsNullOrEmpty())
                {
                    continue;
                }
                
                #endregion

                var groupedAttributes = new Dictionary<String, Object>();

                foreach (var groupingKeyVal in group.Key.Values)
                {
                    groupedAttributes.Add(groupingKeyVal.Key.AttributeAlias, groupingKeyVal.Value.GetReadoutValue());
                }

                var _VertexGroup = new VertexGroup(groupedAttributes,
                                                   group.Select(_DBObjectStream =>
                                                                new Vertex(GetAllSelectedAttributesFromVertex(_DBObjectStream, myReferencedDBType, -1, null, null, false, true))));
                
                #region Check having

                if (_HavingExpression != null)
                {
                    
                    var res = _HavingExpression.IsSatisfyHaving(_VertexGroup, _DBContext);
                    
                    if (res.Failed())
                        throw new GraphDBException(res.IErrors);
                    
                    else if (res.Value)
                        yield return _VertexGroup;

                }

                else
                {
                    yield return _VertexGroup;
                }

                #endregion

            }

        }

        #endregion

        #region ExecuteFunction

        /// <summary>
        /// Executes the function and return the result - for concatenated functions this will be done recursively
        /// </summary>
        /// <param name="mySelectionElementFunction"></param>
        /// <param name="myDBObject"></param>
        /// <param name="myCallingObject"></param>
        /// <param name="myDepth"></param>
        /// <param name="myReference"></param>
        /// <param name="myReferencedDBType"></param>
        /// <param name="myLevelKey"></param>
        /// <param name="myUsingGraph"></param>
        /// <returns></returns>
        private Exceptional<FuncParameter> ExecuteFunction(SelectionElementFunction mySelectionElementFunction, DBObjectStream myDBObject, IObject myCallingObject, Int64 myDepth, String myReference, GraphDBType myReferencedDBType, EdgeList myLevelKey, Boolean myUsingGraph)
        {

            #region Function

            if (myCallingObject == null) // DBObject does not have the attribute
            {

                if (mySelectionElementFunction.SelectValueAssignment != null)
                {
                    return new Exceptional<FuncParameter>(new FuncParameter((mySelectionElementFunction.SelectValueAssignment.TermDefinition as ValueDefinition).Value));
                }

                return null;
            }

            #region Get the FunctionNode and validate the Element

            var func = mySelectionElementFunction.Function;
            func.Function.CallingAttribute = mySelectionElementFunction.Element;

            if (mySelectionElementFunction.Element == null)
            {
                return null;
            }

            #endregion

            func.Function.CallingObject = myCallingObject;

            #region CallingDBObjectStream

            func.Function.CallingDBObjectStream = myDBObject;

            #endregion

            #region Execute the function

            var res = func.Execute(myReferencedDBType, myDBObject, myReference, _DBContext);
            if (res.Failed())
            {
                return new Exceptional<FuncParameter>(res);
            }
            else
            {
                if (res.Value.Value == null)
                {
                    return null; // no result for this object because of not set attribute value
                }

                if (mySelectionElementFunction.FollowingFunction != null)
                {
                    return ExecuteFunction(mySelectionElementFunction.FollowingFunction, myDBObject, res.Value.Value, myDepth, myReference, myReferencedDBType, myLevelKey, myUsingGraph);
                }
                else
                {
                    return res;
                }

            }


            #endregion

            #endregion
        }
        
        #endregion

        #region Some helper methods

        /// <summary>
        /// Returns the depth based on the parameters and settings
        /// </summary>
        /// <param name="myDepth">The target depth (defined by the select)</param>
        /// <param name="minDepth">The minimum depth. If the depth of the setting or mydepth is lower then minDepth will be returned</param>
        /// <param name="typeUUID"></param>
        /// <param name="attributeUUID">May be NULL, than only the depth settings of the type will be checked</param>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        private long GetDepth(long myDepth, long minDepth, GraphDBType myType, TypeAttribute myAttribute = null)
        {
            Int64 Depth = 0;

            #region Get the depth for this type or from select

            /// This results of all selected attributes and their LevelKey. If U.Friends.Friends.Name is selected, than the MinDepth is 3

            if (myAttribute != null)
            {

                if (myDepth > -1)
                    Depth = Math.Max(minDepth, myDepth);
                else
                    Depth = Math.Max(minDepth, (Int64)_SettingCache.GetValue(myType, myAttribute, SettingDepth.UUID, _DBContext).Value);

            }
            else
            {

                if (myDepth > -1)
                    Depth = Math.Max(minDepth, myDepth);
                else
                    Depth = Math.Max(minDepth, (Int64)(_DBContext.DBSettingsManager.GetSetting(SettingDepth.UUID, _DBContext, TypesSettingScope.TYPE, myType).Value.Value).Value);

            }

            #endregion

            return Depth;
        }

        /// <summary>
        /// Get all selections on this <paramref name="myReference"/>, <paramref name="myType"/> and <paramref name="myLevelKey"/>
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myType"></param>
        /// <param name="myLevelKey"></param>
        /// <returns></returns>
        private List<SelectionElement> getAttributeSelections(String myReference, GraphDBType myType, EdgeList myLevelKey)
        {
            if (_Selections.ContainsKey(myReference) && (_Selections[myReference].ContainsKey(myLevelKey)))
            {
                return _Selections[myReference][myLevelKey];
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Get all aggregates on this <paramref name="myReference"/>, <paramref name="myType"/> and <paramref name="myLevelKey"/>
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myType"></param>
        /// <param name="myLevelKey"></param>
        /// <returns></returns>
        private List<SelectionElementAggregate> getAttributeAggregates(String myReference, GraphDBType myType, EdgeList myLevelKey)
        {
            if (_Aggregates.ContainsKey(myReference) && (_Aggregates[myReference].ContainsKey(myLevelKey)))
            {
                return _Aggregates[myReference][myLevelKey];
            }
            else
            {
                return null;
            }

        }

        #endregion
        
        #region Validate Grouping, Selections and Aggregates

        /// <summary>
        /// Validates the groupings and aggregates
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myDBType"></param>
        /// <returns></returns>
        internal Exceptional ValidateGroupingAndAggregate(String myReference, GraphDBType myDBType)
        {

            #region No Aggregates nor Groupings

            if (_Aggregates.IsNullOrEmpty() && _Groupings.IsNullOrEmpty())
            {
                return Exceptional.OK;
            }

            #endregion

            #region No groupings <-> no selections

            if (_Groupings.IsNullOrEmpty() && _Selections.IsNullOrEmpty())
            {
                return Exceptional.OK;
            }
            
            #endregion

            if (!_Groupings.ContainsKey(myReference) && !_Selections.ContainsKey(myReference))
            {

                #region The reference is not existent in selections nor groupings

                return Exceptional.OK;

                #endregion

            }
            else if (_Selections.ContainsKey(myReference) && _Groupings.ContainsKey(myReference))
            {

                #region Verify that all selections match the groupings

                foreach (var selectionEdge in _Selections[myReference])
                {
                    foreach (var selection in selectionEdge.Value)
                    {
                        if (!_Groupings[myReference].Any(ge => ge.RelatedIDChainDefinition.ContentString == selection.RelatedIDChainDefinition.ContentString))
                        {
                            return new Exceptional<bool>(new Error_NoGroupingArgument(selection.RelatedIDChainDefinition.ContentString));
                        }
                    }
                }

                #endregion

            }
            else
            {
                #region Only one of the selections or the groupings contains the reference -> Error

                return new Exceptional<bool>(new Error_NoGroupingArgument(""));

                #endregion
            }

            #region Validate that aggregates and groups are of the same level

            if (_Selections.ContainsKey(myReference) && _Aggregates.ContainsKey(myReference))
            {
                foreach (var selectionEdge in _Selections[myReference])
                {
                    if (!_Aggregates[myReference].Any(a => a.Key.Level == selectionEdge.Key.Level))
                    {
                        return new Exceptional<bool>(new Error_AggregateDoesNotMatchGroupLevel(_Aggregates[myReference].Where(a => a.Key.Level != selectionEdge.Key.Level).First().Value.First().Alias));
                    } 
                }
            }

            #endregion

            return Exceptional.OK;

        }

        #endregion

        #region GetVertices - from a SetReferenceEdgeType

        /// <summary>
        /// Resolve a AListReferenceEdgeType to a DBObjectReadouts. This will resolve each edge target using the 'GetAllAttributesFromDBO' method
        /// </summary>
        /// <param name="myTypeOfAttribute"></param>
        /// <param name="myVertices"></param>
        /// <param name="myDepth"></param>
        /// <param name="currentLvl"></param>
        /// <returns></returns>
        internal IEnumerable<Vertex> GetVertices(GraphDBType myTypeOfAttribute, ASetOfReferencesEdgeType myVertices, IEnumerable<Exceptional<DBObjectStream>> myObjectUUIDs, Int64 myDepth, EdgeList myLevelKey, String myReference, Boolean myUsingGraph)
        {

            foreach (var _Vertex in myVertices.GetReadouts(_ObjectUUID => LoadAndResolveVertex(_ObjectUUID,
                                                                                               myTypeOfAttribute,
                                                                                               myDepth,
                                                                                               myLevelKey,
                                                                                               myReference,
                                                                                               myUsingGraph),
                                                           myObjectUUIDs))

            {
                yield return _Vertex;
            }

            yield break;

        }

        /// <summary>
        /// Resolve a AListReferenceEdgeType to a List of Vertex. This will resolve each edge target using the 'GetAllAttributesFromDBO' method
        /// </summary>
        /// <param name="typeOfAttribute"></param>
        /// <param name="myVertices"></param>
        /// <param name="myDepth"></param>
        /// <param name="currentLvl"></param>
        /// <returns></returns>
        internal IEnumerable<Vertex> GetVertices(GraphDBType myTypeOfAttribute, ASetOfReferencesEdgeType myVertices, Int64 myDepth, EdgeList myLevelKey, String myReference, Boolean myUsingGraph)
        {

            foreach (var _Vertex in myVertices.GetVertices(_ObjectUUID => LoadAndResolveVertex(_ObjectUUID,
                                                                                               myTypeOfAttribute,
                                                                                               myDepth,
                                                                                               myLevelKey,
                                                                                               myReference,
                                                                                               myUsingGraph)))
            {
                if (_Vertex != null)
                {
                    yield return _Vertex;
                }
            }

            yield break;

        }

        /// <summary>
        /// This will load the vertex (check for load errors) and get all selected attributes of this vertex
        /// </summary>
        /// <param name="myObjectUUID"></param>
        /// <param name="myTypeOfAttribute"></param>
        /// <param name="myDepth"></param>
        /// <param name="myLevelKey"></param>
        /// <param name="myReference"></param>
        /// <param name="myUsingGraph"></param>
        /// <returns></returns>
        private Vertex LoadAndResolveVertex(ObjectUUID myObjectUUID, GraphDBType myTypeOfAttribute, Int64 myDepth, EdgeList myLevelKey, String myReference, Boolean myUsingGraph)
        {
            
            var dbStream = _DBContext.DBObjectCache.LoadDBObjectStream(myTypeOfAttribute, myObjectUUID);

            var curType = _DBContext.DBTypeManager.GetTypeByUUID(myLevelKey.LastEdge.TypeUUID);
            var curAttr = curType.GetTypeAttributeByUUID(myLevelKey.LastEdge.AttrUUID);
            
            if (!CheckLoadedDBObjectStream(dbStream, curType, curAttr))
            {
                return GenerateNotResolvedVertex(myObjectUUID, _DBContext.DBTypeManager.GetTypeByUUID(myLevelKey.LastEdge.TypeUUID));
            }

            return new Vertex(GetAllSelectedAttributesFromVertex(dbStream.Value, myTypeOfAttribute, myDepth, myLevelKey, myReference, myUsingGraph));

        }

        #endregion

        #region Add all Attributes of a vertex (select *) and get the values

        /// <summary>
        /// This will add all attributes of <paramref name="myDBObject"/> to the <paramref name="myAttributes"/> reference. Reference attributes will be resolved to the <paramref name="myDepth"/>
        /// </summary>
        /// <param name="myAttributes"></param>
        /// <param name="myType"></param>
        /// <param name="myDBObject"></param>
        /// <param name="myDepth"></param>
        /// <param name="myEdgeList"></param>
        /// <param name="myReference"></param>
        /// <param name="myUsingGraph"></param>
        /// <param name="mySelType"></param>
        /// <param name="myTypeID"></param>
        private void AddAttributesByDBO(ref Dictionary<String, Object> myAttributes, GraphDBType myType, DBObjectStream myDBObject, Int64 myDepth, EdgeList myEdgeList, String myReference, Boolean myUsingGraph, TypesOfSelect mySelType, TypeUUID myTypeID = null)
        {

            #region Get all attributes which are stored at the DBO

            foreach (var attr in myDBObject.GetAttributes())
            {

                #region Check whether the attribute is still exist in the type - if not, continue

                var typeAttr = myType.GetTypeAttributeByUUID(attr.Key);

                if (typeAttr == null)
                {
                    continue;
                }

                #endregion

                #region Only attributes of the selected myTypeID (TypesOfSelect.Ad)

                if (mySelType == TypesOfSelect.Ad)
                {
                    if (myTypeID != typeAttr.GetDBType(_DBContext.DBTypeManager).UUID)
                    {
                        continue;
                    }
                }

                #endregion

                if (attr.Value is ADBBaseObject)
                {

                    #region Single base object

                    if (mySelType != TypesOfSelect.Minus && mySelType != TypesOfSelect.Gt && mySelType != TypesOfSelect.Lt)
                    {
                        myAttributes.Add(typeAttr.Name, (attr.Value as ADBBaseObject).GetReadoutValue());
                    }

                    #endregion

                }
                else if (attr.Value is IBaseEdge)
                {

                    #region List of base objects

                    if (mySelType != TypesOfSelect.Minus && mySelType != TypesOfSelect.Gt && mySelType != TypesOfSelect.Lt)
                    {
                        myAttributes.Add(typeAttr.Name, (attr.Value as IBaseEdge).GetReadoutValues());
                    }

                    #endregion

                }
                else if (attr.Value is IReferenceEdge)
                {

                    #region Reference edge

                    if (mySelType == TypesOfSelect.Minus || mySelType == TypesOfSelect.Asterisk || mySelType == TypesOfSelect.Ad || mySelType == TypesOfSelect.Gt)
                    {
                        // Since we can define special depth (via setting) for attributes we need to check them now
                        myDepth = GetDepth(-1, myDepth, myType, typeAttr);
                        if ((myDepth > 0))
                        {
                            myAttributes.Add(typeAttr.Name, ResolveAttributeValue(typeAttr, attr.Value, myDepth, myEdgeList, myDBObject, myReference, myUsingGraph));
                        }
                        else
                        {
                            myAttributes.Add(typeAttr.Name, GetNotResolvedReferenceAttributeValue(myDBObject, typeAttr, myType, myEdgeList, myUsingGraph, _DBContext));
                        }
                    }

                    #endregion

                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

            }

            #endregion

            #region Get all backwardEdge attributes

            if (mySelType == TypesOfSelect.Minus || mySelType == TypesOfSelect.Asterisk || mySelType == TypesOfSelect.Ad || mySelType == TypesOfSelect.Lt)
            {
                foreach (var beAttr in GetBackwardEdgeAttributes(myType))
                {
                    if (myDepth > 0)
                    {
                        if (mySelType == TypesOfSelect.Ad)
                        {
                            if (beAttr.BackwardEdgeDefinition.TypeUUID != myTypeID)
                            {
                                continue;
                            }
                        }

                        var bes = myDBObject.GetBackwardEdges(beAttr.BackwardEdgeDefinition, _DBContext, _DBContext.DBObjectCache, beAttr.GetDBType(_DBContext.DBTypeManager));
                        
                        if (bes.Failed())
                            throw new GraphDBException(bes.IErrors);

                        if (bes.Value != null) // otherwise the DBO does not have any
                            myAttributes.Add(beAttr.Name, ResolveAttributeValue(beAttr, bes.Value, myDepth, myEdgeList, myDBObject, myReference, myUsingGraph));
                    }
                    else
                    {
                        if (mySelType == TypesOfSelect.Ad)
                        {
                            if (beAttr.BackwardEdgeDefinition.TypeUUID != myTypeID)
                            {
                                continue;
                            }
                        }
                        
                        var notResolvedBEs = GetNotResolvedBackwardEdgeReferenceAttributeValue(myDBObject, beAttr, beAttr.BackwardEdgeDefinition, myEdgeList, myUsingGraph, _DBContext);
                        if (notResolvedBEs != null)
                        {
                            myAttributes.Add(beAttr.Name, notResolvedBEs);
                        }
                    }
                }
            }
            #endregion

            #region Get all undefined attributes from DBO

            if (mySelType == TypesOfSelect.Asterisk || mySelType == TypesOfSelect.Rhomb)
            {
                var undefAttrException = myDBObject.GetUndefinedAttributePayload(_DBContext.DBObjectManager);

                if (undefAttrException.Failed())
                    throw new GraphDBException(undefAttrException.IErrors);

                foreach (var undefAttr in undefAttrException.Value)
                    myAttributes.Add(undefAttr.Key, undefAttr.Value.GetReadoutValue());
            }

            #endregion

            #region Add special attributes

            if (mySelType == TypesOfSelect.Asterisk)
            {
                foreach (var specialAttr in GetSpecialAttributes(myType))
                {
                    if (!myAttributes.ContainsKey(specialAttr.Name))
                    {
                        var result = (specialAttr as ASpecialTypeAttribute).ExtractValue(myDBObject, myType, _DBContext);
                        if (result.Failed())
                        {
                            throw new GraphDBException(result.IErrors);
                        }

                        myAttributes.Add(specialAttr.Name, result.Value.GetReadoutValue());
                    }
                }
            }

            #endregion
        
        }

        private IEnumerable<TypeAttribute> GetBackwardEdgeAttributes(GraphDBType type)
        {

            if (!_BackwardEdgeAttributesByType.ContainsKey(type))
            {
                _BackwardEdgeAttributesByType.Add(type, type.GetAllAttributes(t => t.IsBackwardEdge, _DBContext).ToList());
            }

            return _BackwardEdgeAttributesByType[type];

        }
        
        private IEnumerable<TypeAttribute> GetSpecialAttributes(GraphDBType type)
        {

            if (!_SpecialAttributesByType.ContainsKey(type))
            {
                _SpecialAttributesByType.Add(type, type.GetAllAttributes(t => t is ASpecialTypeAttribute, _DBContext).ToList());
            }

            return _SpecialAttributesByType[type];

        }

        /// <summary>   Gets an attribute value - references will be resolved. </summary>
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
        private Boolean GetAttributeValueAndResolve(GraphDBType myType, SelectionElement mySelectionelement, DBObjectStream myDBObject, Int64 myDepth, EdgeList myLevelKey, String reference, Boolean myUsingGraph, out Object attributeValue, String myUndefAttrName = null)
        {

            var typeAttribute = mySelectionelement.Element;

            if (typeAttribute.TypeCharacteristics.IsBackwardEdge)
            {

                #region IsBackwardEdge

                EdgeKey edgeKey = typeAttribute.BackwardEdgeDefinition;
                var contBackwardExcept = myDBObject.ContainsBackwardEdge(edgeKey, _DBContext, _DBContext.DBObjectCache, myType);

                if (contBackwardExcept.Failed())
                    throw new GraphDBException(contBackwardExcept.IErrors);

                if (contBackwardExcept.Value)
                {
                    if (myDepth > 0)
                    {
                        var dbos = myDBObject.GetBackwardEdges(edgeKey, _DBContext, _DBContext.DBObjectCache, typeAttribute.GetDBType(_DBContext.DBTypeManager));

                        if (dbos.Failed())
                            throw new GraphDBException(dbos.IErrors);

                        if (dbos.Value != null)
                        {
                            attributeValue = ResolveAttributeValue(typeAttribute, dbos.Value, myDepth, myLevelKey, myDBObject, reference, myUsingGraph);
                            return true;
                        }
                    }
                    else
                    {
                        attributeValue = GetNotResolvedBackwardEdgeReferenceAttributeValue(myDBObject, typeAttribute, edgeKey, myLevelKey, myUsingGraph, _DBContext);
                        return true;

                    }
                }

                #endregion

            }
            else if (myDBObject.HasAttribute(typeAttribute, _DBContext))
            {

                #region SelectValueAssignment - kind of static assignment of selected attribute

                if (mySelectionelement.SelectValueAssignment != null && mySelectionelement.SelectValueAssignment.ValueAssignmentType == SelectValueAssignment.ValueAssignmentTypes.Always)
                {
                    // Currently the prior add SelectionElement verifies that TermDefinition is always a ValueDefinition - if others will become valid this must be changed!
                    attributeValue = (mySelectionelement.SelectValueAssignment.TermDefinition as ValueDefinition).Value.Value;
                    return true;
                }

                #endregion

                #region ELSE (!IsBackwardEdge)

                #region not a reference attribute value

                if (!typeAttribute.IsUserDefinedType(_DBContext.DBTypeManager))
                {
                    var attrVal = myDBObject.GetAttribute(typeAttribute, myType, _DBContext);

                    if (attrVal.Failed())
                    {
                        throw new GraphDBException(attrVal.IErrors);
                    }

                    // currently, we do not want to return a ADBBaseObject but the real value
                    if (attrVal.Value is ADBBaseObject)
                        attributeValue = (attrVal.Value as ADBBaseObject).GetReadoutValue();
                    else if (attrVal.Value is IBaseEdge)
                        attributeValue = (attrVal.Value as IBaseEdge).GetReadoutValues();
                    else
                        attributeValue = attrVal.Value;

                    return true;
                }

                #endregion

                #region ELSE Reference attribute value

                else
                {
                    if (myDepth > 0)
                    {
                        var attrValue = myDBObject.GetAttribute(typeAttribute.UUID, myType, _DBContext);
                        attributeValue = ResolveAttributeValue(typeAttribute, attrValue, myDepth, myLevelKey, myDBObject, reference, myUsingGraph);
                        return true;
                    }
                    else
                    {
                        attributeValue = GetNotResolvedReferenceAttributeValue(myDBObject, typeAttribute, myType, myLevelKey, myUsingGraph, _DBContext);
                        return true;
                    }
                }

                #endregion

                #endregion

            }
            else if (mySelectionelement.SelectValueAssignment != null)
            {
                // Currently the prior add SelectionElement verifies that TermDefinition is always a ValueDefinition - if others will become valid this must be changed!
                attributeValue = (mySelectionelement.SelectValueAssignment.TermDefinition as ValueDefinition).Value.Value;
                return true;
            }

            attributeValue = null;
            return false;

        }

        #region GetNotResolved edges

        /// <summary>
        /// Returns just UUID and Type for this edge
        /// </summary>
        /// <param name="referenceEdge"></param>
        /// <param name="typeAttribute"></param>
        /// <param name="myGraphDBType"></param>
        /// <param name="myDBContext"></param>
        /// <returns></returns>
        private Edge GetNotResolvedReferenceEdgeAttributeValue(IReferenceEdge referenceEdge, GraphDBType myGraphDBType, DBContext myDBContext)
        {

            if (referenceEdge is ASetOfReferencesEdgeType)
            {
                return new Edge(null,
                                (referenceEdge as ASetOfReferencesEdgeType).GetVertices((_ObjectUUID) => GenerateNotResolvedVertex(_ObjectUUID, myGraphDBType)))
                                { EdgeTypeName = myGraphDBType.Name};
            }

            else if (referenceEdge is ASingleReferenceEdgeType)
            {
                return new Edge(null,
                                (referenceEdge as ASingleReferenceEdgeType).GetVertex((_ObjectUUID) => GenerateNotResolvedVertex(_ObjectUUID, myGraphDBType)))
                                { EdgeTypeName = myGraphDBType.Name};
            }

            else
            {
                throw new GraphDBException(new Error_NotImplemented(new StackTrace(true)));
            }

        }

        /// <summary>
        /// Get the attribute value of <paramref name="myTypeAttribute"/> and calls GetNotResolvedReferenceEdgeAttributeValue with the edge
        /// </summary>
        /// <param name="myDBObjectStream"></param>
        /// <param name="myTypeAttribute"></param>
        /// <param name="myGraphDBType"></param>
        /// <param name="myCurrentEdgeList"></param>
        /// <param name="myUsingGraph"></param>
        /// <param name="myDBContext"></param>
        /// <returns></returns>
        private Edge GetNotResolvedReferenceAttributeValue(DBObjectStream myDBObjectStream, TypeAttribute myTypeAttribute, GraphDBType myGraphDBType, EdgeList myCurrentEdgeList, Boolean myUsingGraph, DBContext myDBContext)
        {

            IObject attrValue = null;

            if (myUsingGraph)
            {

                var interestingLevelKey = new LevelKey((myCurrentEdgeList + new EdgeKey(myGraphDBType.UUID, myTypeAttribute.UUID)).Edges, myDBContext.DBTypeManager);

                var interestingUUIDs = _ExpressionGraph.SelectUUIDs(interestingLevelKey, myDBObjectStream);

                attrValue = ((IReferenceEdge)myDBObjectStream.GetAttribute(myTypeAttribute.UUID, myGraphDBType, myDBContext)).GetNewInstance(interestingUUIDs, myGraphDBType.UUID);

            }

            else
            {
                attrValue = myDBObjectStream.GetAttribute(myTypeAttribute.UUID, myGraphDBType, myDBContext);
            }

            if (attrValue == null)
            {
                return null;
            }

            var typeName = myTypeAttribute.GetDBType(myDBContext.DBTypeManager).Name;

            return GetNotResolvedReferenceEdgeAttributeValue(attrValue as IReferenceEdge, myTypeAttribute.GetDBType(myDBContext.DBTypeManager), myDBContext);

        }

        /// <summary>
        /// The default DBReadout for not resolved edges
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="myType"></param>
        /// <returns></returns>
        private Vertex GenerateNotResolvedVertex(ObjectUUID reference, GraphDBType myType)
        {

            var specialAttributes = new Dictionary<String, Object>();
            specialAttributes.Add(SpecialTypeAttribute_UUID.AttributeName, reference);
            specialAttributes.Add(SpecialTypeAttribute_TYPE.AttributeName, myType.Name);

            return new Vertex(specialAttributes);

        }

        private Edge GetNotResolvedBackwardEdgeReferenceAttributeValue(DBObjectStream myDBObject, TypeAttribute myTypeAttribute, EdgeKey edgeKey, EdgeList currentEdgeList, Boolean myUsingGraph, DBContext _DBContext)
        {

            IObject attrValue = null;

            if (myUsingGraph)
            {
                var interestingLevelKey = new LevelKey((currentEdgeList + new EdgeKey(myTypeAttribute.RelatedGraphDBTypeUUID, myTypeAttribute.UUID)).Edges, _DBContext.DBTypeManager);

                attrValue = new EdgeTypeSetOfReferences(_ExpressionGraph.SelectUUIDs(interestingLevelKey, myDBObject), myTypeAttribute.DBTypeUUID);
            }

            else
            {
                var attrValueException = myDBObject.GetBackwardEdges(edgeKey, _DBContext, _DBContext.DBObjectCache, myTypeAttribute.GetDBType(_DBContext.DBTypeManager));
                if (attrValueException.Failed())
                {
                    throw new GraphDBException(attrValueException.IErrors);
                }

                attrValue = attrValueException.Value;
            }

            if (attrValue == null)
            {
                return null;
            }

            else if (!(attrValue is IReferenceEdge))
            {
                throw new GraphDBException(new Error_InvalidEdgeType(attrValue.GetType(), typeof(IReferenceEdge)));
            }

            var readouts = new List<Vertex>();
            var typeName = _DBContext.DBTypeManager.GetTypeByUUID(edgeKey.TypeUUID).Name;

            foreach (var reference in (attrValue as IReferenceEdge).GetAllReferenceIDs())
            {
                var specialAttributes = new Dictionary<string, object>();
                specialAttributes.Add(SpecialTypeAttribute_UUID.AttributeName, reference);
                specialAttributes.Add(SpecialTypeAttribute_TYPE.AttributeName, typeName);

                readouts.Add(new Vertex(specialAttributes));
            }

            return new Edge(null, readouts)
                    { EdgeTypeName = _DBContext.DBTypeManager.GetTypeAttributeByEdge(edgeKey).GetDBType(_DBContext.DBTypeManager).Name};

        }
        
        #endregion

        /// <summary>
        /// Resolves an attribute 
        /// </summary>
        /// <param name="attrDefinition">The TypeAttribute</param>
        /// <param name="attributeValue">The attribute value, either a AListReferenceEdgeType or ASingleEdgeType or a basic object (Int, String, ...)</param>
        /// <param name="myDepth">The current depth defined by a setting or in the select</param>
        /// <param name="currentLvl">The current level (for recursive resolve)</param>
        /// <returns>List&lt;Vertex&gt; for user defined list attributes; Vertex for reference attributes, Object for basic type values </returns>
        private object ResolveAttributeValue(TypeAttribute attrDefinition, object attributeValue, Int64 myDepth, EdgeList myEdgeList, DBObjectStream mySourceDBObject, String reference, Boolean myUsingGraph)
        {

            #region attributeValue is not a reference type just return the value

            if (!((attributeValue is ASetOfReferencesEdgeType) || (attributeValue is ASingleReferenceEdgeType)))
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
                if (myEdgeList.Level == 0)
                {
                    myEdgeList = new EdgeList(new EdgeKey(attrDefinition.RelatedGraphDBTypeUUID, attrDefinition.UUID));
                }
                else
                {
                    myEdgeList += new EdgeKey(attrDefinition.RelatedGraphDBTypeUUID, attrDefinition.UUID);
                }
            }

            // at some deeper level we could get into graph independend results. From this time, we can use the GUID index rather than asking the graph all the time
            if (myUsingGraph)
            {
                myUsingGraph = _ExpressionGraph.IsGraphRelevant(new LevelKey(myEdgeList.Edges, _DBContext.DBTypeManager), mySourceDBObject);
            }

            #endregion

            if (attributeValue is ASetOfReferencesEdgeType)
            {

                #region SetReference attribute -> return new Edge

                IEnumerable<Vertex> resultList = null;

                var edge = ((ASetOfReferencesEdgeType)attributeValue);

                if (myUsingGraph)
                {
                    var dbos = _ExpressionGraph.Select(new LevelKey(myEdgeList.Edges, _DBContext.DBTypeManager), mySourceDBObject, true);

                    resultList = GetVertices(typeOfAttribute, edge, dbos, myDepth, myEdgeList, reference, myUsingGraph);
                }
                else
                {
                    resultList = GetVertices(typeOfAttribute, edge, myDepth, myEdgeList, reference, myUsingGraph);
                }

                return new Edge(null, resultList) { EdgeTypeName = typeOfAttribute.Name };

                #endregion

            }
            else if (attributeValue is ASingleReferenceEdgeType)
            {

                #region Single reference

                attributeValue = new Edge(null,
                                          (attributeValue as ASingleReferenceEdgeType).GetVertex
                                          (a => LoadAndResolveVertex(a, typeOfAttribute, myDepth, myEdgeList, reference, myUsingGraph)))
                                          { EdgeTypeName = typeOfAttribute.Name };

                return attributeValue;

                #endregion

            }
            else
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }

        #endregion

        #region GetAllSelectedAttributesFromDBO

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
        private Dictionary<string, object> GetAllSelectedAttributesFromVertex(DBObjectStream myDBObject, GraphDBType myDBType, Int64 myDepth, EdgeList myLevelKey, String myReference, Boolean myUsingGraph, Boolean selectAllAttributes = false)
        {
            Dictionary<string, object> Attributes = new Dictionary<string, object>();
            Int64 Depth;

            var minDepth = 0;

            IEnumerable<SelectionElement> attributeSelections = null;

            if (!selectAllAttributes)
            {
                attributeSelections = getAttributeSelections(myReference, myDBType, myLevelKey);
            }

            if (attributeSelections.IsNullOrEmpty() || selectAllAttributes)// && myLevelKey.Level > 0)
            {

                #region Get all attributes from the DBO if nothing special was selected
                if ((myDepth == -1) || (myDepth >= myLevelKey.Level))
                {
                    AddAttributesByDBO(ref Attributes, myDBType, myDBObject, myDepth, myLevelKey, myReference, myUsingGraph, TypesOfSelect.Asterisk);
                }

                #endregion

            }
            else
            {

                foreach (var attrSel in attributeSelections)
                {

                    #region extract the selected infos

                    //myDepth = attrSel.EdgeList.Level;

                    #region Some kind of asterisk - return all attributes

                    if (attrSel.Selection != TypesOfSelect.None)
                    {

                        #region Asterisk (*), Rhomb (#), Minus (-), Ad (@) selection

                        Depth = GetDepth(myDepth, 0, myDBType);

                        AddAttributesByDBO(ref Attributes, myDBType, myDBObject, Depth, myLevelKey, myReference, myUsingGraph, attrSel.Selection, attrSel.TypeID);

                        #endregion

                        continue;

                    }
                    
                    #endregion

                    #region Alias

                    String alias = String.Empty;

                    if (attrSel.Element == null)
                    {
                        alias = (attrSel.Alias == null) ? attrSel.RelatedIDChainDefinition.UndefinedAttribute : attrSel.Alias;
                    }
                    else
                    {
                        alias = (attrSel.Alias == null) ? (attrSel.Element as TypeAttribute).Name : attrSel.Alias;
                    }

                    #endregion

                    if (Attributes.ContainsKey(alias))
                    {
                        // This is a bug in the attributeSelections add method. No attribute should be in the selected list twice. 
                        // If one attribute was selected more than one, these information will be stored in the next level.
                        //System.Diagnostics.Debug.Assert(false, "The attribute '" + alias + "' is selected twice in that level - this shouldnt!");
                        continue;
                    }

                    if (attrSel is SelectionElementFunction)
                    {

                        #region Select a function

                        var selectionElementFunction = (attrSel as SelectionElementFunction);

                        if (selectionElementFunction.SelectValueAssignment != null && selectionElementFunction.SelectValueAssignment.ValueAssignmentType == SelectValueAssignment.ValueAssignmentTypes.Always)
                        {
                            Attributes.Add(alias, (selectionElementFunction.SelectValueAssignment.TermDefinition as ValueDefinition).Value.GetReadoutValue());
                            continue;
                        }

                        #region Get the CallingObject

                        IObject callingObject = null;
                        var typeOfDBObjects = selectionElementFunction.Element.GetDBType(_DBContext.DBTypeManager);

                        if (myUsingGraph)
                        {
                            myUsingGraph = _ExpressionGraph.IsGraphRelevant(new LevelKey((myLevelKey + new EdgeKey(selectionElementFunction.Element)).Edges, _DBContext.DBTypeManager), myDBObject);
                        }

                        if (myUsingGraph && (typeOfDBObjects.IsUserDefined || typeOfDBObjects.IsBackwardEdge))
                        {
                            var edge = GetAttributeValue(myDBType, selectionElementFunction.Element, myDBObject, myLevelKey);
                            if (edge is IReferenceEdge)
                                callingObject = (edge as IReferenceEdge).GetNewInstance(_ExpressionGraph.SelectUUIDs(new LevelKey((myLevelKey + new EdgeKey(selectionElementFunction.Element)).Edges, _DBContext.DBTypeManager), myDBObject, true), typeOfDBObjects.UUID);
                            else if (edge == null)
                                callingObject = null;
                            else
                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }
                        else
                        {
                            callingObject = GetAttributeValue(myDBType, selectionElementFunction.Element, myDBObject, myLevelKey) as IObject;
                        }

                        #endregion

                        #region Execute the function

                        var res = ExecuteFunction(selectionElementFunction, myDBObject, callingObject, myDepth, myReference, myDBType, myLevelKey, myUsingGraph);

                        if (res == null)
                        {
                            continue;
                        }
                        if (res.Failed())
                        {
                            throw new GraphDBException(res.IErrors);
                        }

                        #endregion

                        #region Add function return value to attributes

                        if (res.Value.Value is IReferenceEdge)
                        {

                            #region Reference edge

                            //minDepth = attrSel.EdgeList.Level + 1; // depth should be at least the depth of the selected element
                            minDepth = (attrSel.RelatedIDChainDefinition != null) ? attrSel.RelatedIDChainDefinition.Edges.Count - 1 : 0;
                            myUsingGraph = false;

                            Depth = GetDepth(myDepth, minDepth, myDBType);

                            if (Depth > myLevelKey.Level || getAttributeSelections(myReference, myDBType, myLevelKey + new EdgeKey((attrSel as SelectionElementFunction).Element)).IsNotNullOrEmpty())
                            {

                                myUsingGraph = false;

                                #region Resolve DBReferences

                                Attributes.Add(alias,
                                    ResolveAttributeValue(((FuncParameter)res.Value).TypeAttribute, ((FuncParameter)res.Value).Value, Depth, myLevelKey, myDBObject, myReference, myUsingGraph));

                                #endregion

                            }
                            else
                            {

                                Attributes.Add(alias, GetNotResolvedReferenceEdgeAttributeValue(res.Value.Value as IReferenceEdge, res.Value.TypeAttribute.GetDBType(_DBContext.DBTypeManager), _DBContext));

                            }

                            #endregion

                        }
                        else
                        {

                            Attributes.Add(alias, ((FuncParameter)res.Value).Value.GetReadoutValue());

                        }

                        #endregion

                        #endregion

                    }
                    else if (attrSel.Element is UndefinedTypeAttribute)
                    {

                        #region undefined attribute selection

                        var undef_alias = attrSel.Alias;

                        if (!Attributes.ContainsKey(undef_alias))
                        {
                            Object attrValue = null;

                            if (GetAttributeValueAndResolve(myDBType, attrSel, myDBObject, 0, myLevelKey, myReference, myUsingGraph, out attrValue, undef_alias))
                            {
                                Attributes.Add(undef_alias, attrValue);
                            }                            
                        }                        

                        #endregion

                    }
                    else
                    {

                        #region Attribute selection

                        minDepth = (attrSel.RelatedIDChainDefinition != null) ? attrSel.RelatedIDChainDefinition.Edges.Count - 1 : 0;

                        //var alias = (attrSel.Alias == null) ? (attrSel.Element as TypeAttribute).Name : attrSel.Alias;

                        if (myLevelKey.Level > 0 && !(attrSel.Element is ASpecialTypeAttribute)) // use the related type instead
                            Depth = GetDepth(myDepth, minDepth, (attrSel.Element as TypeAttribute).GetRelatedType(_DBContext.DBTypeManager), (attrSel.Element as TypeAttribute));
                        else
                            Depth = GetDepth(myDepth, minDepth, myDBType, (attrSel.Element as TypeAttribute));

                        Object attrValue = null;

                        if (GetAttributeValueAndResolve(myDBType, attrSel, myDBObject, Depth, myLevelKey, myReference, myUsingGraph, out attrValue))
                        {
                            Attributes.Add(alias, attrValue);
                        }

                        #endregion

                    }

                    #endregion

                }

            }

            return Attributes;
        }

        /// <summary>
        /// Extracts the attribute from <paramref name="myDBObject"/>.
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myTypeAttribute"></param>
        /// <param name="myDBObject"></param>
        /// <param name="myLevelKey"></param>
        /// <returns></returns>
        private Object GetAttributeValue(GraphDBType myType, TypeAttribute myTypeAttribute, DBObjectStream myDBObject, EdgeList myLevelKey)
        {
            if (myTypeAttribute.TypeCharacteristics.IsBackwardEdge)
            {

                #region IsBackwardEdge

                EdgeKey edgeKey = myTypeAttribute.BackwardEdgeDefinition;
                var contBackwardExcept = myDBObject.ContainsBackwardEdge(edgeKey, _DBContext, _DBContext.DBObjectCache, myType);

                if (contBackwardExcept.Failed())
                    throw new GraphDBException(contBackwardExcept.IErrors);

                if (contBackwardExcept.Value)
                {
                    var getBackwardExcept = myDBObject.GetBackwardEdges(edgeKey, _DBContext, _DBContext.DBObjectCache, myTypeAttribute.GetDBType(_DBContext.DBTypeManager));

                    if (getBackwardExcept.Failed())
                        throw new GraphDBException(getBackwardExcept.IErrors);

                    return getBackwardExcept.Value;
                }

                #endregion

            }
            else if (myDBObject.HasAttribute(myTypeAttribute.UUID, myType))
            {

                #region ELSE (!IsBackwardEdge)

                return myDBObject.GetAttribute(myTypeAttribute.UUID, myType, _DBContext);

                #endregion

            }

            return null;
        }
        
        #endregion

        #region GetResult

        /// <summary>
        /// This will return the result of all examined DBOs, including calculated aggregates and groupings.
        /// The Value of the returned GraphResult is of type IEnumerable&lt;Vertex&gt;
        /// </summary>
        /// <param name="typeNode">The current typeNode</param>
        /// <param name="myVertices">The precalculated return objects</param>
        /// <returns>A GraphResult including the ResultType with the occured errors.</returns>
        public IEnumerable<Vertex> GetResult(String myReference, GraphDBType myReferencedDBType, IEnumerable<Vertex> myVertices, Boolean isWhereExpressionDependent = false)
        {

            foreach (var _Vertex in myVertices)
            {
                yield return _Vertex;
            }

        }

        public IEnumerable<Vertex> GetTypeIndependendResult()
        {

            //_DBOs = new IEnumerable<Vertex>();
            var Attributes = new Dictionary<string, object>();

            #region Go through all _SelectionElementsTypeIndependend

            foreach (var selection in _SelectionElementsTypeIndependend)
            {
                if (selection is SelectionElementFunction)
                {
                    var func = ((SelectionElementFunction)selection);
                    Exceptional<FuncParameter> funcResult = null;
                    var alias = func.Alias;

                    while (func != null)
                    {
                        funcResult = func.Function.Execute(null, null, null, _DBContext);
                        if (funcResult.Success())
                        {

                            if (funcResult.Value.Value == null)
                            {
                                break; // no result for this object because of not set attribute value
                            }
                            else
                            {
                                func = func.FollowingFunction;
                                if (func != null)
                                {
                                    func.Function.Function.CallingObject = funcResult.Value.Value;
                                }
                            }
                            
                        }
                        else
                        {
                            //return new Exceptional<IEnumerable<Vertex>>(res.Errors);
                            throw new GraphDBException(funcResult.IErrors);
                        }
                    }

                    if (funcResult.Value.Value == null)
                    {
                        continue; // no result for this object because of not set attribute value
                    }

                    Attributes.Add(alias, ((FuncParameter)funcResult.Value).Value.GetReadoutValue());
                }
            }

            #endregion

            if (!Attributes.IsNullOrEmpty())
            {
                yield return new Vertex(Attributes);
            }

        }

        #endregion

        #region GetSelectedAttributesList

        public Dictionary<String, String> GetSelectedAttributesList()
        {

            var retVal = new Dictionary<String, String>();

            #region AttributeList

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
                        else if (selElem.Selection != TypesOfSelect.None)
                        {
                            var attributes = _DBContext.DBTypeManager.GetTypeByUUID(sel.Key.Edges[0].TypeUUID).GetAllAttributes(_DBContext);

                            switch (selElem.Selection)
                            {
                                case TypesOfSelect.Minus:
                                    foreach (var attr in attributes)
                                    {
                                        if (!retVal.ContainsKey(attr.Name) && (attr.IsBackwardEdge || attr.GetDBType(_DBContext.DBTypeManager).IsUserDefined))
                                        {
                                            retVal.Add(attr.Name, attr.Name);
                                        }
                                    }

                                    break;

                                case TypesOfSelect.Rhomb:
                                    foreach (var attr in attributes)
                                    {
                                        if (!retVal.ContainsKey(attr.Name) && (!attr.GetDBType(_DBContext.DBTypeManager).IsUserDefined) && (attr.KindOfType != KindsOfType.SpecialAttribute))
                                        {
                                            retVal.Add(attr.Name, attr.Name);
                                        }
                                    }
                                    break;

                                case TypesOfSelect.Ad:
                                    foreach (var attr in attributes)
                                    {
                                        if (!retVal.ContainsKey(attr.Name))
                                        {
                                            if (attr.GetDBType(_DBContext.DBTypeManager).UUID == selElem.TypeID)
                                            {
                                                retVal.Add(attr.Name, attr.Name);
                                            }
                                        }
                                    }
                                    break;

                                case TypesOfSelect.Gt:
                                    foreach (var attr in attributes)
                                    {
                                        if (!retVal.ContainsKey(attr.Name) && (attr.GetDBType(_DBContext.DBTypeManager).IsUserDefined) && (!attr.IsBackwardEdge) && (attr.KindOfType != KindsOfType.SpecialAttribute))
                                        {
                                            retVal.Add(attr.Name, attr.Name);
                                        }
                                    }
                                    break;

                                case TypesOfSelect.Lt:
                                    foreach (var attr in attributes)
                                    {
                                        if (!retVal.ContainsKey(attr.Name) && (attr.GetDBType(_DBContext.DBTypeManager).IsUserDefined) && (attr.IsBackwardEdge) && (attr.KindOfType != KindsOfType.SpecialAttribute))
                                        {
                                            retVal.Add(attr.Name, attr.Name);
                                        }
                                    }
                                    break;

                                case TypesOfSelect.Asterisk:
                                    foreach (var attr in attributes)
                                    {
                                        if (!retVal.ContainsKey(attr.Name))
                                            retVal.Add(attr.Name, attr.Name);
                                    }
                                    break;
                            }
                        }

                    }
                }
            }

            #endregion

            #region Aggregates

            if (!_Aggregates.IsNullOrEmpty())
            {
                foreach (var selElem in _Aggregates.Values)
                {
                    foreach (var aggrList in selElem.Values)
                    {
                        foreach (var aggr in aggrList)
                        {
                            retVal.Add(aggr.AggregateDefinition.ChainPartAggregateDefinition.SourceParsedString, aggr.Alias);
                        }
                    }
                }
            }

            #endregion

            return retVal;
        }

        #endregion

    }

}
