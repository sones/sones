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
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphDB.Request;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphDB.Expression.Tree.Literals;
using sones.Library.LanguageExtensions;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class UpdateNode : AStatement, IAstNodeInit
    {
        #region Data

        /// <summary>
        /// The attributes to update / add / remove
        /// </summary>
        private HashSet<AAttributeAssignOrUpdateOrRemove> _listOfUpdates;

        /// <summary>
        /// Where Expression
        /// </summary>
        private BinaryExpressionDefinition _WhereExpression;
        /// <summary>
        /// substitute for where expression
        /// </summary>
        private IEnumerable<long> _vertexIDs = null;

        /// <summary>
        /// The Name of the type which should be updated
        /// </summary>
        private String _TypeName;

        /// <summary>
        /// The executed query
        /// </summary>
        private String Query;

        #endregion

        /// <summary>
        /// Init method that is called by the InsertOrUpdate/Link node
        /// </summary>
        /// <param name="myTypeName"></param>
        /// <param name="myAttributeAssignList"></param>
        public void Init(String myTypeName, IEnumerable<AAttributeAssignOrUpdateOrRemove> myAttributeAssignList, IEnumerable<long> myVertexIDs)
        {
            _listOfUpdates = new HashSet<AAttributeAssignOrUpdateOrRemove>(myAttributeAssignList);
            _vertexIDs = myVertexIDs;
            _TypeName = myTypeName;
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Type

            _TypeName = ((AstNode)parseNode.ChildNodes[1].AstNode).AsString;

            #endregion

            #region get myAttributes

            if (HasChildNodes(parseNode.ChildNodes[3]))
            {
                var AttrUpdateOrAssign = (AttributeUpdateOrAssignListNode)parseNode.ChildNodes[3].AstNode;
                _listOfUpdates = AttrUpdateOrAssign.ListOfUpdate;
            }

            #endregion

            #region whereClauseOpt

            if (parseNode.ChildNodes[4].ChildNodes != null && parseNode.ChildNodes[4].ChildNodes.Count != 0)
            {
                var tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinaryExpressionDefinition;
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Update"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        /// <summary>
        /// Executes the statement and returns a QueryResult.
        /// </summary>
        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            Query = myQuery;

            return myGraphDB.Update(mySecurityToken, myTransactionToken, GenerateUpdateRequest(myGraphDB, myPluginManager, mySecurityToken, myTransactionToken), GenerateOutput);
        }

        #endregion

        #region helper

        private static IEnumerable<IVertex> ProcessBinaryExpression(BinaryExpressionDefinition binExpression, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexType vertexType)
        {
            //validate
            binExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

            //calculate
            var expressionGraph = binExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

            //extract
            return
                expressionGraph.Select(
                    new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true);
        }

        private RequestUpdate GenerateUpdateRequest(IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            IEnumerable<long> toBeupdatedVertices = null;
            //prepare
            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_TypeName),
                (stats, vtype) => vtype);

            if (_vertexIDs == null)
            {
                if (_WhereExpression != null)
                {
                    //validate
                    _WhereExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

                    //calculate
                    var expressionGraph = _WhereExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

                    //extract

                    toBeupdatedVertices = expressionGraph.SelectVertexIDs(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true);
                }
                else
                {
                    toBeupdatedVertices = myGraphDB.GetVertices<IEnumerable<long>>(
                        mySecurityToken,
                        myTransactionToken,
                        new RequestGetVertices(vertexType.ID),
                        (stats, vertices) => vertices.Select(_ => _.VertexID));
                }
                
            }
            else
            {
                toBeupdatedVertices = _vertexIDs;
            }


            var result = new RequestUpdate(new RequestGetVertices(vertexType.ID, toBeupdatedVertices, false));

            ProcessListOfUpdates(vertexType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, ref result);

            return result;
        }

        private void ProcessListOfUpdates(IVertexType vertexType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, ref RequestUpdate result)
        {
            foreach (var aUpdate in _listOfUpdates)
            {
                ProcessUpdate(vertexType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, aUpdate, ref result);
            }
        }

        private void ProcessUpdate(IVertexType vertexType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, AAttributeAssignOrUpdateOrRemove aUpdate, ref RequestUpdate result)
        {
            if (aUpdate is AttributeAssignOrUpdateValue)
            {
                ProcessAttributeAssignOrUpdateValue((AttributeAssignOrUpdateValue)aUpdate, ref result);
            }
            else if (aUpdate is AttributeAssignOrUpdateList)
            {
                ProcessAttributeAssignOrUpdateList(vertexType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, (AttributeAssignOrUpdateList)aUpdate, ref result);
            }
            else if (aUpdate is AttributeAssignOrUpdateSetRef)
            {
                ProcessAttributeAssignOrUpdateSetRef(vertexType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, (AttributeAssignOrUpdateSetRef)aUpdate, ref result);
            }
            else if (aUpdate is AttributeRemove)
            {
                foreach (var aToBeRemovedAttribute in ((AttributeRemove)aUpdate).ToBeRemovedAttributes)
                {
                    result.RemoveAttribute(aToBeRemovedAttribute);
                }
            }
            else if (aUpdate is AttributeRemoveList)
            {
                ProcessAttributeRemoveList(vertexType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, (AttributeRemoveList)aUpdate, ref result);
            }
            else
            {
                throw new NotImplementedQLException("");
            }
        }

        private void ProcessAttributeRemoveList(IVertexType vertexType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, AttributeRemoveList attributeRemoveList, ref RequestUpdate result)
        {
            if (attributeRemoveList.TupleDefinition is VertexTypeVertexIDCollectionNode)
            {
                #region setofUUIDs

                var edgedef = new EdgePredefinition(attributeRemoveList.AttributeName);

                List<EdgePredefinition> toBeRemovedEdges = new List<EdgePredefinition>();

                foreach (var aTupleElement in ((VertexTypeVertexIDCollectionNode)attributeRemoveList.TupleDefinition).Elements)
                {
                    foreach (var aVertexIDTuple in aTupleElement.VertexIDs)
                    {
                        var innerEdge = new EdgePredefinition();

                        innerEdge.AddVertexID(aTupleElement.ReferencedVertexTypeName, aVertexIDTuple.Item1);

                        edgedef.AddEdge(innerEdge);
                    }
                }

                result.RemoveElementsFromCollection(attributeRemoveList.AttributeName, edgedef);

                #endregion
            }
            else if (attributeRemoveList.TupleDefinition is TupleDefinition)
            {
                if (((TupleDefinition)attributeRemoveList.TupleDefinition).All(_ => _.Value is ValueDefinition))
                {
                    #region base-set

                    //has to be list of comparables
                    ListCollectionWrapper listWrapper = new ListCollectionWrapper();
                    Type myRequestedType;
                    if (vertexType.HasProperty(attributeRemoveList.AttributeIDChain.ContentString))
                    {
                        myRequestedType = ((IPropertyDefinition)vertexType.GetAttributeDefinition(attributeRemoveList.AttributeIDChain.ContentString)).BaseType;
                    }
                    else
                    {
                        myRequestedType = typeof(String);
                    }

                    foreach (var aTupleElement in (TupleDefinition)attributeRemoveList.TupleDefinition)
                    {
                        listWrapper.Add(((ValueDefinition)aTupleElement.Value).Value.ConvertToIComparable(myRequestedType));
                    }

                    result.RemoveElementsFromCollection(attributeRemoveList.AttributeIDChain.ContentString, listWrapper);

                    #endregion
                }
                else
                {
                    #region binaryExpression

                    foreach (var aTupleElement in ((TupleDefinition)attributeRemoveList.TupleDefinition))
                    {
                        if (aTupleElement.Value is BinaryExpressionDefinition)
                        {
                            #region BinaryExpressionDefinition

                            if (!vertexType.HasAttribute(attributeRemoveList.AttributeName))
                            {
                                throw new InvalidVertexAttributeException(String.Format("The vertex type {0} has no attribute named {1}.", vertexType.Name, attributeRemoveList.AttributeName));
                            }
                            IAttributeDefinition attribute = vertexType.GetAttributeDefinition(attributeRemoveList.AttributeName);
                            List<EdgePredefinition> toBeRemovedEdges = new List<EdgePredefinition>();

                            var targetVertexType = ((IOutgoingEdgeDefinition)attribute).TargetVertexType;

                            var vertexIDs = ProcessBinaryExpression(
                                (BinaryExpressionDefinition)aTupleElement.Value,
                                myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, targetVertexType).ToList();

                            if (vertexIDs.Count > 1)
                            {
                                throw new ReferenceAssignmentExpectedException(String.Format("It is not possible to create a single edge pointing to {0} vertices", vertexIDs.Count));
                            }

                            EdgePredefinition edge = new EdgePredefinition(attributeRemoveList.AttributeName);

                            foreach (var aVertex in vertexIDs)
                            {
                                edge.AddEdge(new EdgePredefinition().AddVertexID(aVertex.VertexTypeID, aVertex.VertexID));
                            }

                            result.RemoveElementsFromCollection(attributeRemoveList.AttributeName, edge);

                            #endregion
                        }
                        else
                        {
                            throw new NotImplementedQLException("");
                        }
                    }

                    #endregion
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ProcessAttributeAssignOrUpdateSetRef(IVertexType vertexType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, AttributeAssignOrUpdateSetRef attributeAssignOrUpdateSetRef, ref RequestUpdate result)
        {
            #region SetRefNode

            var edgeDefinition = new EdgePredefinition(attributeAssignOrUpdateSetRef.AttributeIDChain.ContentString);

            if (attributeAssignOrUpdateSetRef.SetRefDefinition.IsREFUUID)
            {
                #region direct vertex ids

                foreach (var aTupleElement in attributeAssignOrUpdateSetRef.SetRefDefinition.TupleDefinition)
                {
                    if (aTupleElement.Value is ValueDefinition)
                    {
                        #region ValueDefinition

                        foreach (var aProperty in aTupleElement.Parameters)
                        {
                            edgeDefinition.AddUnknownProperty(aProperty.Key, aProperty.Value);
                        }

                        edgeDefinition.AddVertexID(
                            attributeAssignOrUpdateSetRef.SetRefDefinition.ReferencedVertexType, 
                            Convert.ToInt64(((ValueDefinition)aTupleElement.Value).Value));

                        #endregion
                    }
                    else
                    {
                        throw new NotImplementedQLException("TODO");
                    }
                }

                result.UpdateEdge(edgeDefinition);

                #endregion
            }
            else
            {
                #region expression

                if (!vertexType.HasAttribute(attributeAssignOrUpdateSetRef.AttributeIDChain.ContentString))
                {
                    throw new InvalidVertexAttributeException(String.Format("The vertex type {0} has no attribute named {1}.", vertexType.Name, attributeAssignOrUpdateSetRef.AttributeIDChain.ContentString));
                }
                IAttributeDefinition attribute = vertexType.GetAttributeDefinition(attributeAssignOrUpdateSetRef.AttributeIDChain.ContentString);

                foreach (var aTupleElement in attributeAssignOrUpdateSetRef.SetRefDefinition.TupleDefinition)
                {
                    if (aTupleElement.Value is BinaryExpressionDefinition)
                    {
                        #region BinaryExpressionDefinition

                        var targetVertexType = ((IOutgoingEdgeDefinition)attribute).TargetVertexType;

                        var vertexIDs = ProcessBinaryExpression(
                            (BinaryExpressionDefinition)aTupleElement.Value,
                            myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, targetVertexType).ToList();

                        if (vertexIDs.Count > 1)
                        {
                            throw new ReferenceAssignmentExpectedException(String.Format("It is not possible to create a single edge pointing to {0} vertices", vertexIDs.Count));
                        }

                        var inneredge = new EdgePredefinition();

                        foreach (var aStructuredProperty in aTupleElement.Parameters)
                        {
                            edgeDefinition.AddUnknownProperty(aStructuredProperty.Key, aStructuredProperty.Value);
                        }

                        edgeDefinition.AddVertexID(vertexIDs.FirstOrDefault().VertexTypeID, vertexIDs.FirstOrDefault().VertexID);

                        #endregion
                    }
                    else
                    {
                        throw new NotImplementedQLException("");
                    }
                }

                #endregion

                result.UpdateEdge(edgeDefinition);

                return;
            }

            #endregion
        }

        private void ProcessAttributeAssignOrUpdateList(IVertexType vertexType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, AttributeAssignOrUpdateList attributeAssignOrUpdateList, ref RequestUpdate result)
        {
            Type myRequestedType;

            switch (attributeAssignOrUpdateList.CollectionDefinition.CollectionType)
            {
                case CollectionType.Set:

                    #region set

                    if (((TupleDefinition)attributeAssignOrUpdateList.CollectionDefinition.TupleDefinition).All(_ => _.Value is ValueDefinition))
                    {
                        #region base-set

                        //has to be list of comparables
                        SetCollectionWrapper setWrapper = new SetCollectionWrapper();

                        if (vertexType.HasProperty(attributeAssignOrUpdateList.AttributeIDChain.ContentString))
                        {
                            myRequestedType = ((IPropertyDefinition)vertexType.GetAttributeDefinition(attributeAssignOrUpdateList.AttributeIDChain.ContentString)).BaseType;
                        }
                        else
                        {
                            myRequestedType = typeof(String);
                        }

                        foreach (var aTupleElement in (TupleDefinition)attributeAssignOrUpdateList.CollectionDefinition.TupleDefinition)
                        {
                            setWrapper.Add(((ValueDefinition)aTupleElement.Value).Value.ConvertToIComparable(myRequestedType));
                        }

                        result.AddElementsToCollection(attributeAssignOrUpdateList.AttributeIDChain.ContentString, setWrapper);

                        #endregion
                    }
                    else
                    {
                        #region edge-set

                        EdgePredefinition edgeDefinition = new EdgePredefinition(attributeAssignOrUpdateList.AttributeIDChain.ContentString);

                        if (!vertexType.HasAttribute(attributeAssignOrUpdateList.AttributeIDChain.ContentString))
                        {
                            throw new InvalidVertexAttributeException(String.Format("The vertex type {0} has no attribute named {1}.", vertexType.Name, attributeAssignOrUpdateList.AttributeIDChain.ContentString));
                        }

                        IAttributeDefinition attribute =  vertexType.GetAttributeDefinition(attributeAssignOrUpdateList.AttributeIDChain.ContentString);
                        foreach (var aTupleElement in (TupleDefinition)attributeAssignOrUpdateList.CollectionDefinition.TupleDefinition)
                        {

                            if (aTupleElement.Value is BinaryExpressionDefinition)
                            {
                                #region BinaryExpressionDefinition

                                var targetVertexType = ((IOutgoingEdgeDefinition)attribute).TargetVertexType;

                                foreach (var aVertex in ProcessBinaryExpression(
                                    (BinaryExpressionDefinition)aTupleElement.Value,
                                    myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, targetVertexType))
                                {
                                    var inneredge = new EdgePredefinition().AddVertexID(aVertex.VertexTypeID, aVertex.VertexID);

                                    foreach (var aStructuredProperty in aTupleElement.Parameters)
                                    {
                                        inneredge.AddUnknownProperty(aStructuredProperty.Key, aStructuredProperty.Value);
                                    }

                                    edgeDefinition.AddEdge(inneredge);
                                }

                                #endregion
                            }
                            else
                            {
                                throw new NotImplementedQLException("TODO");
                            }
                        }

                        if (attributeAssignOrUpdateList.Assign)
                        {
                            result.UpdateEdge(edgeDefinition);
                        }
                        else
                        {
                            result.AddElementsToCollection(attributeAssignOrUpdateList.AttributeIDChain.ContentString, edgeDefinition);
                        }

                        #endregion
                    }
                    #endregion

                    return;
                case CollectionType.List:

                    #region list

                    //has to be list of comparables
                    ListCollectionWrapper listWrapper = new ListCollectionWrapper();

                    if (vertexType.HasProperty(attributeAssignOrUpdateList.AttributeIDChain.ContentString))
                    {
                        myRequestedType = ((IPropertyDefinition)vertexType.GetAttributeDefinition(attributeAssignOrUpdateList.AttributeIDChain.ContentString)).BaseType;
                    }
                    else
	                {
                        myRequestedType = typeof(String);
	                }

                    foreach (var aTupleElement in (TupleDefinition)attributeAssignOrUpdateList.CollectionDefinition.TupleDefinition)
                    {
                        listWrapper.Add(((ValueDefinition)aTupleElement.Value).Value.ConvertToIComparable(myRequestedType));
                    }

                    result.AddElementsToCollection(attributeAssignOrUpdateList.AttributeIDChain.ContentString, listWrapper);

                    #endregion

                    return;
                case CollectionType.SetOfUUIDs:

                    #region SetOfUUIDs

                    EdgePredefinition anotheredgeDefinition = new EdgePredefinition(attributeAssignOrUpdateList.AttributeIDChain.ContentString);

                    foreach (var aTupleElement in ((VertexTypeVertexIDCollectionNode)attributeAssignOrUpdateList.CollectionDefinition.TupleDefinition).Elements)
                    {
                        foreach (var aVertexIDTuple in aTupleElement.VertexIDs)
                        {
                            var innerEdge = new EdgePredefinition();

                            foreach (var aStructuredProperty in aVertexIDTuple.Item2)
                            {
                                innerEdge.AddUnknownProperty(aStructuredProperty.Key, aStructuredProperty.Value);
                            }

                            innerEdge.AddVertexID(aTupleElement.ReferencedVertexTypeName, aVertexIDTuple.Item1);

                            anotheredgeDefinition.AddEdge(innerEdge);
                        }
                    }

                    result.AddElementsToCollection(attributeAssignOrUpdateList.AttributeIDChain.ContentString, anotheredgeDefinition);

                    #endregion

                    return;
                default:
                    return;
            }
        }

        private void ProcessAttributeAssignOrUpdateValue(AttributeAssignOrUpdateValue attributeAssignOrUpdateValue, ref RequestUpdate result)
        {
            result.UpdateUnknownProperty(attributeAssignOrUpdateValue.AttributeIDChain.ContentString , attributeAssignOrUpdateValue.Value);
        }

        private QueryResult GenerateOutput(IRequestStatistics myStats, IEnumerable<IVertex> myVertices)
        {
            var dict = new Dictionary<String, object>();

            foreach (var vertex in myVertices)
            {
                dict.Add("Updated ", vertex.VertexID);
            }

            return new QueryResult(Query, 
                                    "GQL", 
                                    Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), 
                                    ResultType.Successful,
                                    new List<IVertexView> { new VertexView(dict, new Dictionary<String, IEdgeView>()) });
        }

        #endregion
    }
}
