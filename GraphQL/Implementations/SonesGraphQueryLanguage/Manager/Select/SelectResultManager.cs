/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.Extensions;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.Library.LanguageExtensions;
using sones.GraphQL.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphQL.GQL.Manager.Select
{
    /// <summary>
    /// This will create the result of any kind of select - working on an IExpressionGraph.
    /// </summary>
    public sealed class SelectResultManager
    {
        #region data

        List<SelectionElement> _SelectionElementsTypeIndependend;

        /// <summary>
        /// the selection element of _Selections , the aggregate selection element
        /// </summary>
        //List<SelectionElementAggregate> _Aggregates;
        Dictionary<String, Dictionary<EdgeList, List<SelectionElementAggregate>>> _Aggregates;
        Dictionary<String, List<SelectionElement>> _Groupings;

        BinaryExpressionDefinition _HavingExpression;

        private readonly IGraphDB _graphdb;
        private readonly GQLPluginManager _pluginManager;

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

        #endregion

        #region constrcutor

        public SelectResultManager(IGraphDB myGraphDB, GQLPluginManager myPluginManager)
        {
            _Aggregates = new Dictionary<string, Dictionary<EdgeList, List<SelectionElementAggregate>>>();
            _Groupings = new Dictionary<string, List<SelectionElement>>();
            _Selections = new Dictionary<string, Dictionary<EdgeList, List<SelectionElement>>>();
            _SelectionElementsTypeIndependend = new List<SelectionElement>();


            _graphdb = myGraphDB;
            _pluginManager = myPluginManager;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Single IDNode selection attribute
        /// </summary>
        public void AddElementToSelection(string myAlias, 
                                            String myReference, 
                                            IDChainDefinition myIDChainDefinition, 
                                            Boolean myIsGroupedOrAggregated, 
                                            SelectValueAssignment mySelectValueAssignment = null)
        {
            SelectionElement lastElem = null;

            var curLevel = new EdgeList();
            EdgeList preLevel = null;

            if (myReference != null && _Selections.ContainsKey(myReference) &&  
                _Selections[myReference].Any(kv => kv.Value.Any(se => se.RelatedIDChainDefinition == myIDChainDefinition && 
                se.Alias == myAlias)))
            {
                throw new DuplicateAttributeSelectionException(myAlias);
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
                            preLevel = new EdgeList(myIDChainDefinition.LastType.ID);
                        }
                        else
                        {
                            var element = _Selections[myReference].Last();

                            preLevel = curLevel.GetPredecessorLevel();
                            //preLevel = curLevel;
                        }
                        selElem.Alias = typeOrAttr.TypeOrAttributeName;
                        selElem.Element = new UnstructuredProperty(typeOrAttr.TypeOrAttributeName);

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
                        && _Selections[myReference][preLevel].Exists(item => item.Alias == selElem.Alias &&
                                                                    selElem.EdgeList.Level == item.EdgeList.Level &&
                                                                    item.RelatedIDChainDefinition.Depth == selElem.RelatedIDChainDefinition.Depth &&
                                                                    item.Element != null) && !myIsGroupedOrAggregated)
                    {
                        throw new DuplicateAttributeSelectionException(selElem.Alias);
                    }

                    // Do not add again if:
                    // - it is a defined attribute and there is an asterisk selection at this level
                    // - it is not the last part AND
                    // - there is already an item of this part with the same alias and the same Depth
                    if (nodeEdgeKey.Next == null || 
                        _Selections[myReference][preLevel].Count == 0 || 
                        _Selections[myReference][preLevel].Any(item => IsNewSelectionElement(item, selElem)))
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
                                throw new DuplicateAttributeSelectionException(funcElem.Alias);
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


                            if (!_Selections[myReference][preLevel].Contains(funcElem)) // In case this Element with func is already in the selection list do nothing.
                            {

                                _Selections[myReference][preLevel].Add(funcElem);

                            }

                            #endregion

                        }  //functions are not equal && it is the end of a function sequence
                        else if (!_Selections[myReference][preLevel].Contains(funcElem) && !(nodeEdgeKey.Next is ChainPartFuncDefinition))
                        {

                            #region In this case we have a similar function but NOT THE SAME. Since we don't know what to do, return error.

                            throw new InvalidVertexAttributeSelectionException(myIDChainDefinition.ContentString);

                            #endregion

                        }

                        //always assign funcElem to lastElem, otherwise the part of add following functions doesn't work
                        lastElem = funcElem;
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

                if (lastElem.Element.Kind != AttributeType.Property && lastElem.Element.Kind != AttributeType.BinaryProperty)
                {
                    throw new InvalidSelectValueAssignmentException(lastElem.Element.Name);
                }

                if (!(mySelectValueAssignment.TermDefinition is ValueDefinition))
                {
                    throw new NotImplementedQLException("");
                }

                #endregion

                #region Validate datatype if the attribute is a defined attribute

                //if (!(lastElem.Element is UnstructuredProperty))
                //{

                //    if (!lastElem.Element.GetADBBaseObjectType(_DBContext.DBTypeManager).IsValidValue((mySelectValueAssignment.TermDefinition as ValueDefinition).Value.Value))
                //    {
                //        return new Exceptional(new Error_SelectValueAssignmentDataTypeDoesNotMatch(lastElem.Element.GetADBBaseObjectType(_DBContext.DBTypeManager).ObjectName, (mySelectValueAssignment.TermDefinition as ValueDefinition).Value.ObjectName));
                //    }
                //    var typedValue = new ValueDefinition(lastElem.Element.GetADBBaseObjectType(_DBContext.DBTypeManager).Clone((mySelectValueAssignment.TermDefinition as ValueDefinition).Value.Value));
                //    mySelectValueAssignment.TermDefinition = typedValue;
                //}

                #endregion

                lastElem.SelectValueAssignment = mySelectValueAssignment;
            }


            #endregion

        }

        /// <summary>
        /// Adds an aggregate to the selection. It will check whether it is an index aggregate or not.
        /// Aggregates on attributes with level > 1 will return an error.
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myAlias"></param>
        /// <param name="myAStructureNode"></param>
        /// <param name="myGraphType"></param>
        public void AddAggregateElementToSelection(SecurityToken mySecurityToken, 
                                                    TransactionToken myTransactionToken, 
                                                    string myAlias, 
                                                    string myReference, 
                                                    SelectionElementAggregate mySelectionPartAggregate)
        {

            if (mySelectionPartAggregate.EdgeList.Level > 1)
            {
                throw new AggregateIsNotValidOnThisAttributeException(mySelectionPartAggregate.ToString());
            }

            #region Check for index aggregate

            foreach (var edge in mySelectionPartAggregate.EdgeList.Edges)
            {
                var vertexType = _graphdb.GetVertexType<IVertexType>(
                        mySecurityToken,
                        myTransactionToken,
                        new RequestGetVertexType(edge.VertexTypeID),
                        (stats, vType) => vType);

                #region COUNT(*)

                if (!edge.IsAttributeSet)
                {
                    //mySelectionPartAggregate.Element = _DBContext.DBTypeManager.GetUUIDTypeAttribute();
                    mySelectionPartAggregate.Element = vertexType.GetAttributeDefinition("VertexID");
                }

                #endregion

                else
                {
                    // if the GetAttributeIndex did not return null we will pass this as the aggregate operation value
                    mySelectionPartAggregate.Element = vertexType.GetAttributeDefinition(edge.AttributeID);
                }
            }

            #endregion

            if (!_Aggregates.ContainsKey(myReference))
            {
                _Aggregates.Add(myReference, new Dictionary<EdgeList, List<SelectionElementAggregate>>());
            }

            var level = mySelectionPartAggregate.EdgeList.GetPredecessorLevel();
            if (!_Aggregates[myReference].ContainsKey(level))
            {
                _Aggregates[myReference].Add(level, new List<SelectionElementAggregate>());
            }

            _Aggregates[myReference][level].Add(mySelectionPartAggregate);
        }

        /// <summary>
        /// Adds a group element to the selection and validat it
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myIDChainDefinition"></param>
        public void AddGroupElementToSelection(string myReference, IDChainDefinition myIDChainDefinition)
        {

            if (myIDChainDefinition.Edges.Count > 1)
            {
                throw new InvalidGroupByLevelException(myIDChainDefinition.Edges.Count.ToString(), myIDChainDefinition.ContentString);
            }

            if (_Selections.ContainsKey(myReference))
            {
                foreach (var aReference in _Selections[myReference])
                {
                    foreach (var aSelection in aReference.Value)
                    {
                        if (aSelection.Element.Equals(myIDChainDefinition.LastAttribute))
                        {
                            if (!_Groupings.ContainsKey(myReference))
                            {
                                _Groupings.Add(myReference, new List<SelectionElement>());
                            }
                            _Groupings[myReference].Add(new SelectionElement(myReference, new EdgeList(myIDChainDefinition.Edges), false, myIDChainDefinition, myIDChainDefinition.LastAttribute));

                            return;
                        }
                    }
                }
            }

            // if the grouped attr is not selected
            throw new GroupedAttributeIsNotSelectedException(myIDChainDefinition.LastAttribute.Name);

        }

        /// <summary>
        /// Adds a having to the selection
        /// </summary>
        /// <param name="myHavingExpression"></param>
        public void AddHavingToSelection(BinaryExpressionDefinition myHavingExpression)
        {
            _HavingExpression = myHavingExpression;
        }


        /// <summary>
        /// Adds the typeNode as an asterisk *, rhomb # or minus - or ad
        /// </summary>
        public void AddSelectionType(string myReference, IVertexType myType, TypesOfSelect mySelType, long? myTypeID = null)
        {
            var selElem = new SelectionElement(mySelType, myTypeID);

            if (!_Selections.ContainsKey(myReference))
                _Selections.Add(myReference, new Dictionary<EdgeList, List<SelectionElement>>());

            var level = new EdgeList(new EdgeKey(myType.ID));

            if (!_Selections[myReference].ContainsKey(level))
                _Selections[myReference].Add(level, new List<SelectionElement>());

            if (!_Selections[myReference][level].Exists(item => item.Selection == mySelType))
            {
                _Selections[myReference][level].Add(selElem);
            }
        }

        #region Validate Grouping, Selections and Aggregates

        /// <summary>
        /// Validates the groupings and aggregates
        /// </summary>
        /// <param name="myReference"></param>
        /// <param name="myDBType"></param>
        /// <returns></returns>
        public void ValidateGroupingAndAggregate(String myReference, IVertexType myDBType)
        {

            #region No Aggregates nor Groupings

            if (_Aggregates.IsNullOrEmpty() && _Groupings.IsNullOrEmpty())
            {
                return;
            }

            #endregion

            #region No groupings <-> no selections

            if (_Groupings.IsNullOrEmpty() && _Selections.IsNullOrEmpty())
            {
                return;
            }

            #endregion

            if (!_Groupings.ContainsKey(myReference) && !_Selections.ContainsKey(myReference))
            {

                #region The reference is not existent in selections nor groupings

                return;

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
                            throw new NoGroupingArgumentException(selection.RelatedIDChainDefinition.ContentString);
                        }
                    }
                }

                #endregion

            }
            else
            {
                #region Only one of the selections or the groupings contains the reference -> Error

                throw new NoGroupingArgumentException("");

                #endregion
            }

            #region Validate that aggregates and groups are of the same level

            if (_Selections.ContainsKey(myReference) && _Aggregates.ContainsKey(myReference))
            {
                foreach (var selectionEdge in _Selections[myReference])
                {
                    if (!_Aggregates[myReference].Any(a => a.Key.Level == selectionEdge.Key.Level))
                    {
                        throw new AggregateDoesNotMatchGroupLevelException(_Aggregates[myReference].Where(a => a.Key.Level != selectionEdge.Key.Level).First().Value.First().Alias);
                    }
                }
            }

            #endregion
        }

        #endregion

        #region GetResult

        /// <summary>
        /// This will return the result of all examined DBOs, including calculated aggregates and groupings.
        /// The Value of the returned GraphResult is of type IEnumerable&lt;Vertex&gt;
        /// </summary>
        //public IEnumerable<IVertexView> GetResult(IEnumerable<IVertexView> myVertices)
        //{

        //    foreach (var _Vertex in myVertices)
        //    {
        //        yield return _Vertex;
        //    }

        //}

        #endregion

        /// <summary>
        /// Examine a TypeNode to a specific <paramref name="myResolutionDepth"/> by using the underlying graph or the type guid index
        /// </summary>
        /// <param name="myResolutionDepth">The depth to which the reference attributes should be resolved</param>
        /// <param name="myTypeNode">The type node which should be examined on selections</param>
        /// <param name="myUsingGraph">True if there is a valid where expression of this typeNode</param>
        /// <returns>True if succeeded, false if there was nothing to select for this type</returns>
        public Boolean Examine(Int64 myResolutionDepth,
                                String myReference,
                                IVertexType myReferencedDBType,
                                Boolean myUsingGraph,
                                ref IEnumerable<IVertexView> myVertices,
                                SecurityToken mySecurityToken,
                                TransactionToken myTransactionToken)
        {

            if ((!_Selections.ContainsKey(myReference) || !_Selections[myReference].ContainsKey(new EdgeList(myReferencedDBType.ID))) && _Aggregates.IsNullOrEmpty())
            {
                return false;
            }

            var levelKey = new EdgeList(myReferencedDBType.ID);
            //var levelKeys = new EdgeList(myReferencedDBType.GetAncestorVertexTypesAndSelf().Select(x => new EdgeKey(x.ID)));

            myVertices = ExamineVertex(myResolutionDepth, myReference, myReferencedDBType, levelKey, myUsingGraph, mySecurityToken, myTransactionToken);

            return true;
        }

        #endregion

        #region private methods

        /// <summary>
        /// The element <paramref name="myNewElement"/> is new if
        /// - it is not a special type attribute and there is an asterisk selection at this level
        /// - it is not the last part AND there is already an item of this part with the same alias and the same Depth
        /// </summary>
        /// <param name="myExistingElement"></param>
        /// <param name="myNewElement"></param>
        /// <returns></returns>
        private Boolean IsNewSelectionElement(SelectionElement myExistingElement, 
                                                SelectionElement myNewElement)
        {

            #region The existing is an asterisk and the new one is NOT a SpecialTypeAttribute

            if (myExistingElement.Selection == TypesOfSelect.Asterisk && 
                !myNewElement.RelatedIDChainDefinition.IsSpecialTypeAttribute())
            {
                return false;
            }

            #endregion

            #region The same level is already selected

            var item = myExistingElement;
            var selElem = myNewElement;

            if (item.Alias == selElem.Alias// &&
                // same depth or Undefined and one lower depth Friends.Undefined is Depth 1 and Friends.Age is Depth 2 but at the same selection level
                //	(item.RelatedIDChainDefinition.Depth == selElem.RelatedIDChainDefinition.Depth || 
                //      (selElem.RelatedIDChainDefinition.IsUndefinedAttribute && item.RelatedIDChainDefinition.Depth == selElem.RelatedIDChainDefinition.Depth + 1))
                )
            {
                return false;
            }

            #endregion

            return true;

        }

        /// <summary>
        /// This is the main function. It will check all selections on this type and will create the readouts
        /// </summary>
        private IEnumerable<IVertexView> ExamineVertex(long myResolutionDepth, 
                                                        String myReference, 
                                                        IVertexType myRelatedVertexType, 
                                                        EdgeList myLevelKey, 
                                                        bool myUsingGraph, 
                                                        SecurityToken mySecurityToken, 
                                                        TransactionToken myTransactionToken)
        {

            #region Get all selections and aggregates for this reference, type and level

            var _Selections = getAttributeSelections(myReference, myLevelKey);
            var aggregates = getAttributeAggregates(myReference, myLevelKey);

            #endregion


            #region Otherwise load all dbos until this level and return them

            #region Get dbos enumerable of the first level - either from ExpressionGraph or via index

            IEnumerable<IVertex> dbos;
            if (myUsingGraph)
            {
                dbos = _ExpressionGraph.Select(new LevelKey(myLevelKey.Edges, 
                                                            _graphdb, 
                                                            mySecurityToken, 
                                                            myTransactionToken), null, true);
            }
            else // using GUID index
            {
                dbos = _graphdb.GetVertices<IEnumerable<IVertex>>(
                    mySecurityToken,
                    myTransactionToken,
                    new RequestGetVertices(myRelatedVertexType.ID),
                    (stats, vertices) => vertices);
            }

            #endregion

            if (aggregates.IsNotNullOrEmpty())
            {
                foreach (var val in ExamineDBO_Aggregates(myTransactionToken, 
                                                            mySecurityToken, 
                                                            dbos, 
                                                            aggregates, 
                                                            _Selections, 
                                                            myUsingGraph, 
                                                            myResolutionDepth))
                {
                    if (val != null)
                    {
                        yield return val;
                    }
                }
            }

            else if (_Groupings.IsNotNullOrEmpty())
            {
                foreach (var val in ExamineDBO_Groupings(dbos, _Selections))
                {
                    if (val != null)
                    {
                        yield return val;
                    }
                }
            }

            else if (!_Selections.IsNullOrEmpty())
            {

                #region Usually attribute selections

                var vertexType = _graphdb.GetVertexType<IVertexType>(
                    mySecurityToken,
                    myTransactionToken,
                    new RequestGetVertexType(myLevelKey.LastEdge.VertexTypeID),
                    (stats, type) => type);

                foreach (var aDBObject in dbos)
                {
                    #region Create a readoutObject for this DBO and yield it: on failure throw an exception

                    Tuple<IDictionary<String, Object>, IDictionary<String, IEdgeView>> Attributes = 
                        GetAllSelectedAttributesFromVertex(mySecurityToken, 
                                                            myTransactionToken, 
                                                            aDBObject, 
                                                            vertexType, 
                                                            myResolutionDepth, 
                                                            myLevelKey, 
                                                            myReference, 
                                                            myUsingGraph);

                    if (Attributes != null && (Attributes.Item1.Count > 0 || Attributes.Item2.Count > 0))
                    {
                        yield return new VertexView(Attributes.Item1, Attributes.Item2);
                    }

                    #endregion

                }

                #endregion

            }

            #endregion


        }

        public IEnumerable<IVertexView> GetTypeIndependendResult(SecurityToken mySecurityToken, 
                                                                    TransactionToken myTransactionToken)
        {

            //_DBOs = new IEnumerable<Vertex>();
            var Attributes = new Dictionary<string, object>();

            #region Go through all _SelectionElementsTypeIndependend

            foreach (var selection in _SelectionElementsTypeIndependend)
            {
                if (selection is SelectionElementFunction)
                {
                    var func = ((SelectionElementFunction)selection);
                    FuncParameter funcResult = null;
                    var alias = func.Alias;
                    object CallingObject = null;
                    while (func != null)
                    {
                        var parameter = func.Function.Execute(null, null, null, _pluginManager, _graphdb, mySecurityToken, myTransactionToken);
                        funcResult = func.Function.Function.ExecFunc(null, CallingObject, null, _graphdb, mySecurityToken, myTransactionToken, parameter.ToArray());
                        if (funcResult.Value == null)
                        {
                            break; // no result for this object because of not set attribute value
                        }
                        else
                        {
                            func = func.FollowingFunction;
                            if (func != null)
                            {
                                CallingObject = funcResult.Value;
                            }
                        }

                    }

                    if (funcResult.Value == null)
                    {
                        continue; // no result for this object because of not set attribute value
                    }

                    Attributes.Add(alias, funcResult.Value);
                }
            }

            #endregion

            if (!Attributes.IsNullOrEmpty())
            {
                yield return new VertexView(Attributes, null);
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
        public Tuple<IDictionary<String, Object>, IDictionary<String, IEdgeView>> 
                GetAllSelectedAttributesFromVertex(SecurityToken mySecurityToken, 
                                                    TransactionToken myTransactionToken, 
                                                    IVertex myDBObject, 
                                                    IVertexType myDBType, 
                                                    Int64 myDepth, 
                                                    EdgeList myLevelKey, 
                                                    String myReference, 
                                                    Boolean myUsingGraph)
        {
            IDictionary<String, Object> properties = new Dictionary<string, object>();
            IDictionary<String, IEdgeView> edges = new Dictionary<string, IEdgeView>();

            var Attributes = new Tuple<IDictionary<string, object>, IDictionary<string, IEdgeView>>(properties, edges);
            Int64 Depth;

            var minDepth = 0;

            //save the already created LevelKey of the refernce type
            EdgeList levelKey = myLevelKey;

            IEnumerable<SelectionElement> attributeSelections = null;

            attributeSelections = getAttributeSelections(myReference, myLevelKey);

            if (attributeSelections.IsNullOrEmpty())
            {

                #region Get all attributes from the DBO if nothing special was selected
                if ((myDepth == -1) || (myDepth >= myLevelKey.Level))
                {
                    AddAttributesByDBO(mySecurityToken, 
                                        myTransactionToken, 
                                        ref Attributes, 
                                        myDBType, 
                                        myDBObject, 
                                        myDepth, 
                                        myLevelKey, 
                                        myReference, 
                                        myUsingGraph, 
                                        TypesOfSelect.Asterisk);
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

                    if (attrSel.Selection == TypesOfSelect.Asterisk)
                    {

                        #region Asterisk (*), Rhomb (#), Minus (-), Ad (@) selection

                        Depth = GetDepth(myDepth, 0, myDBType);

                        AddAttributesByDBO(mySecurityToken, 
                                            myTransactionToken, 
                                            ref Attributes, 
                                            myDBType, 
                                            myDBObject, 
                                            Depth, 
                                            myLevelKey, 
                                            myReference, 
                                            myUsingGraph, 
                                            attrSel.Selection, 
                                            attrSel.TypeID);

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
                        alias = (attrSel.Alias == null) ? attrSel.Element.Name : attrSel.Alias;
                    }

                    #endregion

                    if (Attributes.Item1.ContainsKey(alias) || Attributes.Item2.ContainsKey(alias))
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

                        if (selectionElementFunction.SelectValueAssignment != null && 
                            selectionElementFunction.SelectValueAssignment.ValueAssignmentType == SelectValueAssignment.ValueAssignmentTypes.Always)
                        {
                            Attributes.Item1.Add(alias, (selectionElementFunction.SelectValueAssignment.TermDefinition as ValueDefinition).Value);
                            continue;
                        }

                        #region Get the CallingObject

                        Object callingObject = null;

                        if (myUsingGraph)
                        {
                            //a special attribute has no RelatedGraphDBTypeUUID. So use myDBType to create an EdgeKey
                            //EdgeKey key = (selectionElementFunction.Element is ASpecialTypeAttribute) ? new EdgeKey(myDBType.UUID, selectionElementFunction.Element.UUID) : new EdgeKey(selectionElementFunction.Element);
                            EdgeKey key = new EdgeKey(selectionElementFunction.Element.RelatedType.ID, selectionElementFunction.Element.ID);

                            //the levelKey is created with the type inside the select -- f.e. FROM User SELECT Friends.TOP(2) -- it is 'User'
                            //we have to check if the levelKey contains an edge of the VertexType which is equal to the related type of the selection element
                            //this is neccessary -- f.e. FROM User SELECT Friends.TOP(2) -- if the edge 'FRIENDS' is not defined on type User
                            //in this case we have to check if there is a parent type of the reference type which is equal to the related type of the selection element
                            if (levelKey.Edges.All(x => x.VertexTypeID != selectionElementFunction.Element.RelatedType.ID))
                            {
                                if (myDBType.GetAncestorVertexTypesAndSelf().Select(x => x.ID).Contains(selectionElementFunction.Element.RelatedType.ID))
                                {
                                    var typeID = myDBType.GetAncestorVertexTypesAndSelf().Select(x => x.ID).Where(x => x == selectionElementFunction.Element.RelatedType.ID).First();
                                    levelKey = new EdgeList(typeID);
                                }
                            }

                            myUsingGraph = _ExpressionGraph.IsGraphRelevant(new LevelKey((levelKey + key).Edges, _graphdb, mySecurityToken, myTransactionToken), myDBObject);
                        }

                        if (myUsingGraph && !(selectionElementFunction.Element.Kind != AttributeType.Property))
                        {
                            switch (selectionElementFunction.Element.Kind)
                            {
                                case AttributeType.IncomingEdge:
                                    #region incoming edge

                                    var incomingEdgeDefinition = (IIncomingEdgeDefinition)selectionElementFunction.Element;

                                    if (myDBObject.HasIncomingVertices(incomingEdgeDefinition.RelatedEdgeDefinition.RelatedType.ID, incomingEdgeDefinition.RelatedEdgeDefinition.ID))
                                    {
                                        callingObject = _ExpressionGraph.Select(
                                            new LevelKey(
                                                (myLevelKey + new EdgeKey(selectionElementFunction.Element.RelatedType.ID, selectionElementFunction.Element.ID)).Edges,
                                                _graphdb, mySecurityToken, myTransactionToken), myDBObject, true);
                                    }
                                    else
                                    {
                                        callingObject = null;
                                    }

                                    #endregion
                                    break;

                                case AttributeType.OutgoingEdge:
                                    #region outgoing edge

                                    if (myDBObject.HasOutgoingEdge(selectionElementFunction.Element.ID))
                                    {

                                        callingObject = _ExpressionGraph.Select(
                                            new LevelKey(
                                                (myLevelKey + new EdgeKey(selectionElementFunction.Element.RelatedType.ID, selectionElementFunction.Element.ID)).Edges,
                                                _graphdb, mySecurityToken, myTransactionToken), myDBObject, true);

                                    }
                                    else
                                    {
                                        callingObject = null;
                                    }

                                    #endregion

                                    break;

                                case AttributeType.Property:

                                    callingObject = GetCallingObject(myDBObject, selectionElementFunction.Element);
                                    break;

                                case AttributeType.BinaryProperty:
                                default:
                                    continue;
                            }
                        }
                        else
                        {
                            callingObject = GetCallingObject(myDBObject, selectionElementFunction.Element);
                        }

                        #endregion

                        #region Execute the function

                        var res = ExecuteFunction(selectionElementFunction, 
                                                    myDBObject, 
                                                    callingObject, 
                                                    myDepth, 
                                                    myReference, 
                                                    myDBType, 
                                                    myLevelKey, 
                                                    myUsingGraph, 
                                                    mySecurityToken, 
                                                    myTransactionToken);

                        if (res == null)
                        {
                            continue;
                        }

                        #endregion

                        #region Add function return value to attributes

                        if (res.Value is IEnumerable<IVertex>)
                        {

                            #region Reference edge

                            //minDepth = attrSel.EdgeList.Level + 1; // depth should be at least the depth of the selected element
                            minDepth = (attrSel.RelatedIDChainDefinition != null) ? attrSel.RelatedIDChainDefinition.Edges.Count - 1 : 0;
                            myUsingGraph = false;

                            Depth = GetDepth(myDepth, minDepth, myDBType);

                            var attr = (attrSel as SelectionElementFunction).Element;

                            //if (Depth > myLevelKey.Level || getAttributeSelections(myReference, myLevelKey + new EdgeKey(attr.RelatedType.ID, attr.ID)).IsNotNullOrEmpty())
                            if (Depth > levelKey.Level || getAttributeSelections(myReference, levelKey + new EdgeKey(attr.RelatedType.ID, attr.ID)).IsNotNullOrEmpty())
                            {

                                myUsingGraph = false;

                                #region Resolve DBReferences

                                Attributes.Item2.Add(alias,
                                                    ResolveAttributeValue((IOutgoingEdgeDefinition)attr, 
                                                                            (IEnumerable<IVertex>)res.Value, 
                                                                            Depth, 
                                                                            myLevelKey, 
                                                                            myDBObject, 
                                                                            myReference, 
                                                                            myUsingGraph, 
                                                                            mySecurityToken, 
                                                                            myTransactionToken));

                                #endregion

                            }
                            else
                            {
                                Attributes.Item2.Add(alias, GetNotResolvedReferenceEdgeAttributeValue((IEnumerable<IVertex>)res.Value));
                            }

                            #endregion

                        }
                        else if (res.Value is IVertexView)
                        {
                            //the result is one or multiple 'paths'
                            if ((res.Value as IVertexView).HasEdge("path"))
                            {
                                var pathHyperEdge = (res.Value as IVertexView).GetAllHyperEdges().First();

                                if (!String.IsNullOrWhiteSpace(attrSel.Alias))
                                {
                                    Attributes.Item2.Add(attrSel.Alias, pathHyperEdge.Item2);
                                }
                                else
                                    Attributes.Item2.Add(pathHyperEdge.Item1, pathHyperEdge.Item2);
                            }
                        }
                        else
                        {

                            Attributes.Item1.Add(alias, res.Value);

                        }

                        #endregion

                        #endregion

                    }
                    else if (attrSel.Element is UnstructuredProperty)
                    {

                        #region undefined attribute selection

                        var undef_alias = attrSel.Alias;

                        if (!Attributes.Item1.ContainsKey(undef_alias))
                        {
                            Object attrValue = null;

                            if (myDBObject.HasUnstructuredProperty(undef_alias))
                            {
                                Attributes.Item1.Add(undef_alias, attrValue);
                            }
                        }

                        #endregion

                    }
                    else
                    {

                        #region Attribute selection

                        minDepth = (attrSel.RelatedIDChainDefinition != null) ? attrSel.RelatedIDChainDefinition.Edges.Count - 1 : 0;

                        //var alias = (attrSel.Alias == null) ? (attrSel.Element as TypeAttribute).Name : attrSel.Alias;

                        Depth = GetDepth(myDepth, minDepth, myDBType, attrSel.Element);

                        Object attrValue = null;

                        if (GetAttributeValueAndResolve(mySecurityToken, myTransactionToken, myDBType, attrSel, myDBObject, Depth, myLevelKey, myReference, myUsingGraph, out attrValue))
                        {
                            if (attrValue is IEdgeView)
                            {
                                Attributes.Item2.Add(alias, (IEdgeView)attrValue);
                            }
                            else
                            {
                                Attributes.Item1.Add(alias, attrValue);
                            }
                        }

                        #endregion

                    }

                    #endregion

                }

            }

            return Attributes;
        }

        private object GetCallingObject(IVertex myDBObject, IAttributeDefinition iAttributeDefinition)
        {
            switch (iAttributeDefinition.Kind)
            {
                case AttributeType.Property:

                    return ((IPropertyDefinition)iAttributeDefinition).GetValue(myDBObject);

                case AttributeType.IncomingEdge:

                    var incomingEdgeAttribue = (IIncomingEdgeDefinition)iAttributeDefinition;

                    if (myDBObject.HasIncomingVertices(incomingEdgeAttribue.RelatedEdgeDefinition.RelatedType.ID, incomingEdgeAttribue.RelatedEdgeDefinition.ID))
                    {
                        return myDBObject.GetIncomingVertices(incomingEdgeAttribue.RelatedEdgeDefinition.RelatedType.ID, incomingEdgeAttribue.RelatedEdgeDefinition.ID);
                    }
                    return null;

                case AttributeType.OutgoingEdge:

                    return myDBObject.HasOutgoingEdge(iAttributeDefinition.ID) ? myDBObject.GetOutgoingEdge(iAttributeDefinition.ID) : null;

                case AttributeType.BinaryProperty:
                default:

                    return null;
            }
        }

        private IEdgeView GetNotResolvedReferenceEdgeAttributeValue(IEnumerable<IVertex> iEnumerable)
        {
            return new HyperEdgeView(
                null,
                iEnumerable.Select(
                aVertex =>
                {
                    return new SingleEdgeView(null, new VertexView(new Dictionary<String, object> { { "VertexTypeID", aVertex.VertexTypeID }, { "VertexID", aVertex.VertexID } }, null));
                }));
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
        private Boolean GetAttributeValueAndResolve(SecurityToken mySecurityToken,
                                                    TransactionToken myTransactionToken,
                                                    IVertexType myType,
                                                    SelectionElement mySelectionelement,
                                                    IVertex myDBObject,
                                                    Int64 myDepth,
                                                    EdgeList myLevelKey,
                                                    String reference,
                                                    Boolean myUsingGraph,
                                                    out Object attributeValue,
                                                    String myUndefAttrName = null)
        {

            var typeAttribute = mySelectionelement.Element;

            switch (typeAttribute.Kind)
            {
                case AttributeType.Property:
                    #region property

                    var property = (IPropertyDefinition)typeAttribute;

                    attributeValue = property.GetValue(myDBObject);

                    return attributeValue != null;

                    #endregion

                case AttributeType.IncomingEdge:

                    #region IsBackwardEdge

                    var incomingEdgeAttribute = (IIncomingEdgeDefinition)typeAttribute;

                    if (myDBObject.HasIncomingVertices(incomingEdgeAttribute.RelatedEdgeDefinition.RelatedType.ID, incomingEdgeAttribute.RelatedEdgeDefinition.ID))
                    {
                        var dbos = myDBObject.GetIncomingVertices(incomingEdgeAttribute.RelatedEdgeDefinition.RelatedType.ID, incomingEdgeAttribute.RelatedEdgeDefinition.ID);

                        if (dbos != null)
                        {
                            if (myDepth > 0)
                            {
                                attributeValue = ResolveIncomingEdgeValue(incomingEdgeAttribute, 
                                                                            dbos, 
                                                                            myDepth, 
                                                                            myLevelKey, 
                                                                            myDBObject, 
                                                                            reference, 
                                                                            myUsingGraph, 
                                                                            mySecurityToken, 
                                                                            myTransactionToken);
                            }
                            else
                            {
                                attributeValue = GetNotResolvedReferenceEdgeAttributeValue(dbos);
                            }

                            return true;

                        }
                    }

                    #endregion

                    break;
                case AttributeType.OutgoingEdge:
                    #region outgoing edges

                    if (myDBObject.HasOutgoingEdge(typeAttribute.ID))
                    {
                        var edge = myDBObject.GetOutgoingEdge(typeAttribute.ID);

                        if (edge != null)
                        {
                            if (myDepth > 0)
                            {
                                attributeValue = ResolveAttributeValue(
                                                    (IOutgoingEdgeDefinition)typeAttribute, 
                                                    edge, 
                                                    myDepth, 
                                                    myLevelKey, 
                                                    myDBObject, 
                                                    reference, 
                                                    myUsingGraph, 
                                                    mySecurityToken, 
                                                    myTransactionToken);
                            }
                            else
                            {
                                attributeValue = GetNotResolvedReferenceEdgeAttributeValue(edge.GetTargetVertices());
                            }

                            return true;
                        }
                    }

                    #endregion

                    break;
                case AttributeType.BinaryProperty:
                default:
                    break;
            }

            attributeValue = null;
            return false;

        }


        /// <summary>
        /// Executes the function and return the result - for concatenated functions this will be done recursively
        /// </summary>
        private FuncParameter ExecuteFunction(SelectionElementFunction mySelectionElementFunction, 
                                                IVertex myDBObject, 
                                                object myCallingObject, 
                                                Int64 myDepth, 
                                                String myReference, 
                                                IVertexType myReferencedDBType, 
                                                EdgeList myLevelKey, 
                                                Boolean myUsingGraph, 
                                                SecurityToken mySecurityToken, 
                                                TransactionToken myTransactionToken)
        {

            #region Function

            if (myCallingObject == null) // DBObject does not have the attribute
            {

                if (mySelectionElementFunction.SelectValueAssignment != null)
                {
                    return new FuncParameter((mySelectionElementFunction.SelectValueAssignment.TermDefinition as ValueDefinition).Value);
                }

                return null;
            }

            #region Get the FunctionNode and validate the Element

            var func = mySelectionElementFunction.Function;

            if (mySelectionElementFunction.Element == null)
            {
                return null;
            }

            #endregion

            #region Execute the function

            var parameters = func.Execute(myReferencedDBType, myDBObject, myReference, _pluginManager, _graphdb, mySecurityToken, myTransactionToken);
            var result = func.Function.ExecFunc(mySelectionElementFunction.Element, myCallingObject, myDBObject, _graphdb, mySecurityToken, myTransactionToken, parameters.ToArray());

            if (result.Value == null)
            {
                return null; // no result for this object because of not set attribute value
            }

            if (mySelectionElementFunction.FollowingFunction != null)
            {
                return ExecuteFunction(mySelectionElementFunction.FollowingFunction, myDBObject, result.Value, myDepth, myReference, myReferencedDBType, myLevelKey, myUsingGraph, mySecurityToken, myTransactionToken);
            }
            else
            {
                return result;
            }

            #endregion

            #endregion
        }


        /// <summary>
        /// This will add all attributes of <paramref name="myDBObject"/> to the 
        /// <paramref name="myAttributes"/> reference. Reference attributes will be resolved to the <paramref name="myDepth"/>
        /// </summary>
        private void AddAttributesByDBO(
            SecurityToken mySecurityToken,
            TransactionToken myTransactionToken,
            ref Tuple<IDictionary<String, Object>, IDictionary<String, IEdgeView>> myAttributes,
            IVertexType myType,
            IVertex myDBObject,
            Int64 myDepth,
            EdgeList myEdgeList,
            String myReference,
            Boolean myUsingGraph,
            TypesOfSelect mySelType,
            Int64? myTypeID = null)
        {

            #region Get all attributes which are stored at the DBO

            #region properties

            foreach (var aProperty in myType.GetPropertyDefinitions(true))
            {
                var tempResult = aProperty.GetValue(myDBObject);

                if (tempResult != null)
                {
                    myAttributes.Item1.Add(aProperty.Name, tempResult);
                }
            }

            #endregion

            #region unstructured data

            foreach (var aUnstructuredProperty in myDBObject.GetAllUnstructuredProperties())
            {
                myAttributes.Item1.Add(aUnstructuredProperty.Item1, aUnstructuredProperty.Item2);
            }

            #endregion

            #region outgoing edges

            foreach (var outgoingEdgeDefinition in myType.GetOutgoingEdgeDefinitions(true))
            {
                if (myDBObject.HasOutgoingEdge(outgoingEdgeDefinition.ID))
                {
                    // Since we can define special depth (via setting) for attributes we need to check them now
                    myDepth = GetDepth(-1, myDepth, myType, outgoingEdgeDefinition);
                    if ((myDepth > 0))
                    {
                        myAttributes.Item2.Add(
                            outgoingEdgeDefinition.Name,
                            ResolveAttributeValue(
                                outgoingEdgeDefinition,
                                myDBObject.GetOutgoingEdge(outgoingEdgeDefinition.ID),
                                myDepth,
                                myEdgeList,
                                myDBObject,
                                myReference,
                                myUsingGraph,
                                mySecurityToken,
                                myTransactionToken));
                    }
                    else
                    {
                        myAttributes.Item2.Add(outgoingEdgeDefinition.Name, 
                                                GetNotResolvedReferenceEdgeAttributeValue(myDBObject
                                                                                            .GetOutgoingEdge(outgoingEdgeDefinition.ID)
                                                                                            .GetTargetVertices()));
                    }
                }
            }

            #endregion

            #region incoming

            foreach (var aIncomingEdgeDefinition in myType.GetIncomingEdgeDefinitions(true))
            {
                if (myDBObject.HasIncomingVertices(aIncomingEdgeDefinition
                                                    .RelatedEdgeDefinition
                                                    .RelatedType.ID, 
                                                   aIncomingEdgeDefinition
                                                    .RelatedEdgeDefinition.ID))
                {
                    if (myDepth > 0)
                    {
                        myAttributes.Item2.Add(
                            aIncomingEdgeDefinition.Name,
                            ResolveIncomingEdgeValue(
                                aIncomingEdgeDefinition,
                                myDBObject.GetIncomingVertices(aIncomingEdgeDefinition
                                                                .RelatedEdgeDefinition
                                                                .RelatedType
                                                                .ID, 
                                                               aIncomingEdgeDefinition
                                                                .RelatedEdgeDefinition.ID),
                                myDepth,
                                myEdgeList,
                                myDBObject,
                                myReference,
                                myUsingGraph,
                                mySecurityToken,
                                myTransactionToken));
                    }
                    else
                    {
                        myAttributes.Item2.Add(aIncomingEdgeDefinition.Name, 
                                                GetNotResolvedReferenceEdgeAttributeValue(myDBObject.GetIncomingVertices(aIncomingEdgeDefinition
                                                                                                                            .RelatedEdgeDefinition
                                                                                                                            .RelatedType.ID, 
                                                                                                                          aIncomingEdgeDefinition
                                                                                                                            .RelatedEdgeDefinition.ID)));
                    }
                }
            }

            #endregion

            #endregion
        }

        /// <summary>
        /// Resolves an attribute 
        /// </summary>
        private IEdgeView ResolveIncomingEdgeValue(IIncomingEdgeDefinition attrDefinition, 
                                                    IEnumerable<IVertex> myIncomingVertices, 
                                                    Int64 myDepth, 
                                                    EdgeList myEdgeList, 
                                                    IVertex mySourceDBObject, 
                                                    String reference, 
                                                    Boolean myUsingGraph, 
                                                    SecurityToken mySecurityToken, 
                                                    TransactionToken myTransactionToken)
        {
            #region Get levelKey and UsingGraph

            if (myEdgeList.Level == 0)
            {
                myEdgeList = new EdgeList(new EdgeKey(attrDefinition.RelatedType.ID, attrDefinition.ID));
            }
            else
            {
                myEdgeList += new EdgeKey(attrDefinition.RelatedType.ID, attrDefinition.ID);
            }

            // at some deeper level we could get into graph independend results. From this time, we can use the GUID index rather than asking the graph all the time
            if (myUsingGraph)
            {
                myUsingGraph = _ExpressionGraph.IsGraphRelevant(new LevelKey(myEdgeList.Edges, _graphdb, mySecurityToken, myTransactionToken), mySourceDBObject);
            }

            #endregion

            #region SetReference attribute -> return new Edge

            IEnumerable<IVertexView> resultList = null;

            if (myUsingGraph)
            {
                var dbos = _ExpressionGraph.Select(new LevelKey(myEdgeList.Edges, 
                                                                _graphdb, 
                                                                mySecurityToken, 
                                                                myTransactionToken), 
                                                                mySourceDBObject, 
                                                                true);

                resultList = GetVertices(mySecurityToken, 
                                            myTransactionToken, 
                                            attrDefinition.RelatedEdgeDefinition.SourceVertexType, 
                                            dbos, 
                                            myDepth, 
                                            myEdgeList, 
                                            reference, 
                                            myUsingGraph);
            }
            else
            {
                resultList = GetVertices(mySecurityToken, 
                                            myTransactionToken, 
                                            attrDefinition.RelatedEdgeDefinition.SourceVertexType, 
                                            myIncomingVertices, 
                                            myDepth, 
                                            myEdgeList, 
                                            reference, 
                                            myUsingGraph);
            }

            return new HyperEdgeView(null, resultList.Select(aTargetVertex => new SingleEdgeView(null, aTargetVertex)));
            #endregion
        }

        /// <summary>
        /// Resolve a AListReferenceEdgeType to a DBObjectReadouts. 
        /// This will resolve each edge target using the 'GetAllAttributesFromDBO' method
        /// </summary>

        private IEnumerable<IVertexView> GetVertices(SecurityToken mySecurityToken, 
                                                        TransactionToken myTransactionToken, 
                                                        IVertexType myTypeOfAttribute, 
                                                        IEnumerable<IVertex> myObjectUUIDs, 
                                                        Int64 myDepth, 
                                                        EdgeList myLevelKey, 
                                                        String myReference, 
                                                        Boolean myUsingGraph)
        {
            //TODO:obey weighted edges here


            foreach (var aVertex in myObjectUUIDs)
            {
                yield return LoadAndResolveVertex(mySecurityToken, 
                                                    myTransactionToken, 
                                                    aVertex, 
                                                    myTypeOfAttribute, 
                                                    myDepth, 
                                                    myLevelKey, 
                                                    myReference, 
                                                    myUsingGraph);
            }

            yield break;

        }

        /// <summary>
        /// Resolves an attribute 
        /// </summary>
        private IEdgeView ResolveAttributeValue(IOutgoingEdgeDefinition attrDefinition, 
                                                IEdge myEdge, 
                                                Int64 myDepth, 
                                                EdgeList myEdgeList, 
                                                IVertex mySourceDBObject, 
                                                String reference, 
                                                Boolean myUsingGraph, 
                                                SecurityToken mySecurityToken, 
                                                TransactionToken myTransactionToken)
        {
            #region Get levelKey and UsingGraph

            if (myEdgeList.Level == 0)
            {
                myEdgeList = new EdgeList(new EdgeKey(attrDefinition.RelatedType.ID, 
                                                        attrDefinition.ID));
            }
            else
            {
                myEdgeList += new EdgeKey(attrDefinition.RelatedType.ID, 
                                            attrDefinition.ID);
            }

            // at some deeper level we could get into graph independend results. 
            //From this time, we can use the GUID index rather than asking the graph all the time
            if (myUsingGraph)
            {
                myUsingGraph = _ExpressionGraph.IsGraphRelevant(new LevelKey(myEdgeList.Edges, 
                                                                                _graphdb, 
                                                                                mySecurityToken, 
                                                                                myTransactionToken), 
                                                                mySourceDBObject);
            }

            #endregion

            if (attrDefinition.Multiplicity != EdgeMultiplicity.SingleEdge)
            {

                #region SetReference attribute -> return new Edge

                IEnumerable<SingleEdgeView> resultList = null;

                if (myUsingGraph)
                {
                    var dbos = _ExpressionGraph.Select(new LevelKey(myEdgeList.Edges, 
                                                                    _graphdb, 
                                                                    mySecurityToken, 
                                                                    myTransactionToken), 
                                                        mySourceDBObject, 
                                                        true).ToList();

                    //Todo: find a better way to get the edge properties
                    resultList = GenerateSingleEdgeViews(mySecurityToken, 
                                                            myTransactionToken,
                                                            ((IHyperEdge)myEdge)
                                                                .GetAllEdges((aSingleEdge) => 
                                                                              dbos.Contains(aSingleEdge.GetTargetVertex())),
                                                            attrDefinition.TargetVertexType, 
                                                            myDepth, 
                                                            myEdgeList, 
                                                            reference, 
                                                            myUsingGraph, 
                                                            attrDefinition.InnerEdgeType);

                }
                else
                {

                    resultList = GenerateSingleEdgeViews(mySecurityToken, myTransactionToken,
                        ((IHyperEdge)myEdge).GetAllEdges(),
                        attrDefinition.TargetVertexType, 
                        myDepth, 
                        myEdgeList, 
                        reference, 
                        myUsingGraph, 
                        attrDefinition.InnerEdgeType);
                }

                return new HyperEdgeView(null, resultList);
                #endregion

            }
            else
            {

                #region Single reference

                return GenerateSingleEdgeViews(mySecurityToken, myTransactionToken,
                    new List<ISingleEdge> { (ISingleEdge)myEdge },
                    attrDefinition.TargetVertexType, 
                    myDepth, 
                    myEdgeList, 
                    reference, 
                    myUsingGraph, 
                    attrDefinition.InnerEdgeType).FirstOrDefault();

                #endregion

            }
        }

        private IEnumerable<SingleEdgeView> GenerateSingleEdgeViews(SecurityToken mySecurityToken, 
                                                                    TransactionToken myTransactionToken, 
                                                                    IEnumerable<ISingleEdge> mySingleEdges, 
                                                                    IVertexType myVertexType, 
                                                                    long myDepth, 
                                                                    EdgeList myEdgeList, 
                                                                    string reference, 
                                                                    bool myUsingGraph, 
                                                                    IEdgeType myInnerEdgeType)
        {
            foreach (var aSingleEdge in mySingleEdges)
            {
                yield return GenerateASingleEdgeView(mySecurityToken, 
                                                        myTransactionToken, 
                                                        aSingleEdge, 
                                                        myVertexType, 
                                                        myDepth, 
                                                        myEdgeList, 
                                                        reference, 
                                                        myUsingGraph, 
                                                        myInnerEdgeType);
            }

            yield break;
        }

        private SingleEdgeView GenerateASingleEdgeView(SecurityToken mySecurityToken, 
                                                        TransactionToken myTransactionToken, 
                                                        ISingleEdge aSingleEdge, 
                                                        IVertexType myVertexType, 
                                                        long myDepth, 
                                                        EdgeList myEdgeList, 
                                                        string reference, 
                                                        bool myUsingGraph, 
                                                        IEdgeType myInnerEdgeType)
        {
            return new SingleEdgeView(GetEdgeProperties(aSingleEdge, myInnerEdgeType), 
                                        LoadAndResolveVertex(mySecurityToken, 
                                                                myTransactionToken, 
                                                                aSingleEdge.GetTargetVertex(), 
                                                                myVertexType, 
                                                                myDepth, 
                                                                myEdgeList, 
                                                                reference, 
                                                                myUsingGraph));
        }

        private IDictionary<string, object> GetEdgeProperties(ISingleEdge aSingleEdge, IEdgeType myInnerEdgeType)
        {
            Dictionary<String, Object> result = new Dictionary<string, object>();

            #region properties

            if (myInnerEdgeType != null)
            {
                foreach (var aProperty in myInnerEdgeType.GetPropertyDefinitions(true))
                {
                    if (aSingleEdge.HasProperty(aProperty.ID))
                    {
                        result.Add(aProperty.Name, aProperty.GetValue(aSingleEdge));
                    }
                    else
                    {
                        var tempResult = aProperty.GetValue(aSingleEdge);

                        if (tempResult != null)
                        {
                            result.Add(aProperty.Name, tempResult);
                        }
                    }
                }
            }

            #endregion

            #region unstructured data

            foreach (var aUnstructuredProperty in aSingleEdge.GetAllUnstructuredProperties())
            {
                result.Add(aUnstructuredProperty.Item1, aUnstructuredProperty.Item2);
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Resolves an attribute 
        /// </summary>
        private IEdgeView ResolveAttributeValue(IOutgoingEdgeDefinition attrDefinition, 
                                                IEnumerable<IVertex> attributeValue, 
                                                Int64 myDepth, 
                                                EdgeList myEdgeList, 
                                                IVertex mySourceDBObject, 
                                                String reference, 
                                                Boolean myUsingGraph, 
                                                SecurityToken mySecurityToken, 
                                                TransactionToken myTransactionToken)
        {
            #region Get levelKey and UsingGraph

            if (myEdgeList.Level == 0)
            {
                myEdgeList = new EdgeList(new EdgeKey(attrDefinition.RelatedType.ID, attrDefinition.ID));
            }
            else
            {
                myEdgeList += new EdgeKey(attrDefinition.RelatedType.ID, attrDefinition.ID);
            }

            // at some deeper level we could get into graph independend results. 
            // From this time, we can use the GUID index rather than asking the graph all the time
            if (myUsingGraph)
            {
                myUsingGraph = _ExpressionGraph.IsGraphRelevant(new LevelKey(myEdgeList.Edges, 
                                                                                _graphdb, 
                                                                                mySecurityToken, 
                                                                                myTransactionToken), 
                                                                mySourceDBObject);
            }

            #endregion

            if (attrDefinition.Multiplicity != EdgeMultiplicity.SingleEdge)
            {

                #region SetReference attribute -> return new Edge

                IEnumerable<IVertexView> resultList = null;

                if (myUsingGraph)
                {
                    var dbos = _ExpressionGraph.Select(new LevelKey(myEdgeList.Edges, 
                                                                    _graphdb, 
                                                                    mySecurityToken, 
                                                                    myTransactionToken), 
                                                       mySourceDBObject, 
                                                       true);

                    resultList = GetVertices(mySecurityToken, 
                                                myTransactionToken, 
                                                attrDefinition.TargetVertexType, 
                                                dbos, 
                                                myDepth, 
                                                myEdgeList, 
                                                reference, 
                                                myUsingGraph);
                }
                else
                {
                    resultList = GetVertices(mySecurityToken, 
                                                myTransactionToken, 
                                                attrDefinition.
                                                TargetVertexType, 
                                                attributeValue, 
                                                myDepth, 
                                                myEdgeList, 
                                                reference, 
                                                myUsingGraph);
                }

                return new HyperEdgeView(null, resultList.Select(aTargetVertex => new SingleEdgeView(null, aTargetVertex)));
                #endregion

            }
            else
            {

                #region Single reference

                return new SingleEdgeView(null, LoadAndResolveVertex(mySecurityToken, 
                                                                        myTransactionToken, 
                                                                        attributeValue.FirstOrDefault(), 
                                                                        attrDefinition.SourceVertexType, 
                                                                        myDepth, 
                                                                        myEdgeList, 
                                                                        reference, 
                                                                        myUsingGraph));

                #endregion

            }
        }


        /// <summary>
        /// This will load the vertex (check for load errors) and get all selected attributes of this vertex
        /// </summary>
        private VertexView LoadAndResolveVertex(SecurityToken mySecurityToken, 
                                                TransactionToken myTransactionToken, 
                                                IVertex myObjectUUID, 
                                                IVertexType myTypeOfAttribute, 
                                                Int64 myDepth, 
                                                EdgeList myLevelKey, 
                                                String myReference, 
                                                Boolean myUsingGraph)
        {
            var tuple = GetAllSelectedAttributesFromVertex(mySecurityToken, 
                                                            myTransactionToken, 
                                                            myObjectUUID, 
                                                            myTypeOfAttribute, 
                                                            myDepth, 
                                                            myLevelKey, 
                                                            myReference, 
                                                            myUsingGraph);

            return new VertexView(tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// Returns the depth based on the parameters and settings
        /// </summary>
        private long GetDepth(long myDepth, long minDepth, IVertexType myType, IAttributeDefinition myAttribute = null)
        {
            Int64 Depth = 0;

            #region Get the depth for this type or from select

            /// This results of all selected attributes and their LevelKey. If U.Friends.Friends.Name is selected, than the MinDepth is 3

            if (myAttribute != null)
            {

                if (myDepth > -1)
                    Depth = Math.Max(minDepth, myDepth);
                else
                    Depth = Math.Max(minDepth, 0L);

            }
            else
            {

                if (myDepth > -1)
                    Depth = Math.Max(minDepth, myDepth);
                else
                    Depth = Math.Max(minDepth, 0L);

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
        private List<SelectionElement> getAttributeSelections(String myReference, EdgeList myLevelKey)
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
        private List<SelectionElementAggregate> getAttributeAggregates(String myReference, EdgeList myLevelKey)
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

        /// <summary>
        /// Go through each DBO and aggregate them
        /// </summary>
        /// <param name="myAggregates"></param>
        /// <param name="mySelections"></param>
        /// <param name="myDBOs"></param>
        /// <param name="myReferencedDBType"></param>
        /// <returns></returns>
        private IEnumerable<IVertexView> ExamineDBO_Aggregates(TransactionToken myTransactionToken, 
                                                                SecurityToken mySecurityToken, 
                                                                IEnumerable<IVertex> myDBOs, 
                                                                List<SelectionElementAggregate> myAggregates, 
                                                                List<SelectionElement> mySelections, 
                                                                Boolean myUsingGraph, 
                                                                Int64 myDepth)
        {

            #region Aggregate

            if (mySelections.CountIsGreater(0))
            {

                #region Grouping - each selection is grouped (checked prior)

                var aggregatedGroupings = new Dictionary<GroupingKey, SelectionElementAggregate>();

                #region Create groupings using the ILookup

                var groupedDBOs = myDBOs.ToLookup((dbo) =>
                {
                    #region Create GroupingKey based on the group values and attributes

                    Dictionary<GroupingValuesKey, IComparable> groupingVals = new Dictionary<GroupingValuesKey, IComparable>();
                    foreach (var selection in mySelections)
                    {
                        var attrValue = (selection.Element as IPropertyDefinition).GetValue(dbo);

                        groupingVals.Add(new GroupingValuesKey(selection.Element, selection.Alias), attrValue);
                    }
                    GroupingKey groupingKey = new GroupingKey(groupingVals);

                    #endregion

                    return groupingKey;

                }, (dbo) =>
                {

                    return dbo;
                });

                #endregion

                foreach (var group in groupedDBOs)
                {

                    #region Create group readouts

                    var aggregatedAttributes = new Dictionary<String, Object>();

                    foreach (var aggr in myAggregates)
                    {
                        var aggrResult =
                            aggr.Aggregate.Aggregate(
                                (group).Select(
                                    aVertex => (aggr.Element as IPropertyDefinition).GetValue(aVertex)), (IPropertyDefinition)aggr.Element);

                        if (aggrResult.Value != null)
                        {
                            //aggregatedAttributes.Add(aggr.Alias, aggrResult.Value.Value.GetReadoutValue());
                            aggregatedAttributes.Add(aggr.Alias, ResolveAggregateResult(aggrResult, myDepth));
                        }

                    }

                    foreach (var groupingKeyVal in group.Key.Values)
                    {
                        aggregatedAttributes.Add(groupingKeyVal.Key.AttributeAlias, groupingKeyVal.Value);
                    }

                    var dbObjectReadout = new VertexView(aggregatedAttributes, null);

                    #endregion

                    #region Evaluate having if exist and yield return

                    if (_HavingExpression != null)
                    {
                        var res = _HavingExpression.IsSatisfyHaving(dbObjectReadout);
                        if (res)
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
                VertexView _Vertex;

                #region OR Attribute aggregates

                foreach (var aggr in myAggregates)
                {
                    FuncParameter aggrResult = null;

                    if (aggr.Aggregate.AggregateName == "count" && aggr.RelatedIDChainDefinition.SelectType == TypesOfSelect.Asterisk)
                    {
                        aggrResult = new FuncParameter(_graphdb.GetVertexCount<UInt64>(mySecurityToken, 
                                                                                        myTransactionToken, 
                                                                                        new RequestGetVertexCount(aggr.EdgeList.LastEdge.VertexTypeID), 
                                                                                        (stats, count) => count));
                    }
                    else
                    {
                        aggrResult =
                            aggr.Aggregate.Aggregate(
                            myDBOs.Where(aVertex => (aggr.Element as IPropertyDefinition).HasValue(aVertex)).Select(
                                dbo => (aggr.Element as IPropertyDefinition).GetValue(dbo)), (IPropertyDefinition)aggr.Element);

                    }


                    //aggregatedAttributes.Add(aggr.Alias, aggrResult.Value.GetReadoutValue());
                    if (aggrResult.Value != null)
                    {
                        aggregatedAttributes.Add(aggr.Alias, ResolveAggregateResult(aggrResult, myDepth));
                    }

                }

                _Vertex = new VertexView(aggregatedAttributes, null);

                #endregion

                #region Check having expression and yield return value

                if (_HavingExpression != null)
                {
                    var res = _HavingExpression.IsSatisfyHaving(_Vertex);
                    if (res)
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
        /// This will check the exceptional for errors. 
        /// Depending on the SettingInvalidReferenceHandling an expcetion will be thrown or false will be return on any load error.
        /// </summary>
        /// <returns></returns>
        private Boolean CheckLoadedDBObjectStream(IVertex dbStream, IVertexType myDBType, IAttributeDefinition myTypeAttribute = null)
        {
            return true;
        }

        private Object ResolveAggregateResult(FuncParameter aggrResult, Int64 myDepth)
        {
            #region Add aggregate return value to attributes

            if (aggrResult.Value is IHyperEdge)
            {
                throw new NotImplementedQLException("TODO");

                //#region Reference edge

                ////myUsingGraph = false;

                ////Depth = GetDepth(myDepth, minDepth, myDBType);
                //var Depth = myDepth;

                //if (myDepth > 0)
                //{

                //    var myLevelKey = new EdgeList(new EdgeKey(aggrResult.TypeAttribute));

                //    #region Resolve DBReferences

                //    return ResolveAttributeValue(aggrResult.TypeAttribute, aggrResult.Value, Depth, myLevelKey, null, null, false);

                //    #endregion

                //}
                //else
                //{
                //    GraphDBType type;
                //    if (aggrResult.TypeAttribute.IsBackwardEdge)
                //    {
                //        type = _DBContext.DBTypeManager.GetTypeByUUID(aggrResult.TypeAttribute.BackwardEdgeDefinition.TypeUUID);
                //    }
                //    else
                //    {
                //        type = aggrResult.TypeAttribute.GetDBType(_DBContext.DBTypeManager);
                //    }

                //    return GetNotResolvedReferenceEdgeAttributeValue(aggrResult.Value as IReferenceEdge, type, _DBContext);

                //}

                //#endregion

            }
            else
            {

                return aggrResult.Value;

            }

            #endregion
        }

        /// <summary>
        ///  Group all DBOs and return the readouts
        /// </summary>
        /// <param name="myDBObjectStreams"></param>
        /// <param name="mySelections"></param>
        /// <param name="myReferencedDBType"></param>
        /// <returns></returns>
        private IEnumerable<IVertexView> ExamineDBO_Groupings(IEnumerable<IVertex> myDBObjectStreams, List<SelectionElement> mySelections)
        {

            #region Create groupings using the ILookup

            var _GroupedVertices = myDBObjectStreams.ToLookup((dbo) =>
            {

                #region Create GroupingKey based on the group values and attributes

                var groupingVals = new Dictionary<GroupingValuesKey, IComparable>();

                foreach (var selection in mySelections)
                {

                    var attrValue = (selection.Element as IPropertyDefinition).GetValue(dbo);


                    groupingVals.Add(new GroupingValuesKey(selection.Element, selection.Alias), attrValue);

                }

                GroupingKey groupingKey = new GroupingKey(groupingVals);

                #endregion

                return groupingKey;

            }, (dbo) =>
            {
                return dbo;
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
                    groupedAttributes.Add(groupingKeyVal.Key.AttributeAlias, groupingKeyVal.Value);
                }

                var _VertexGroup = new VertexView(groupedAttributes, null);

                #region Check having

                if (_HavingExpression != null)
                {

                    var res = _HavingExpression.IsSatisfyHaving(_VertexGroup);

                    if (res)
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
    }
}