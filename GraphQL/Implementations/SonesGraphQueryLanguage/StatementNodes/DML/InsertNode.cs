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
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.Structure.Nodes.DML;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.ErrorHandling;
using sones.Library.LanguageExtensions;
using sones.Library.CollectionWrapper;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class InsertNode : AStatement, IAstNodeInit
    {
        private String _TypeName;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private String _queryString;

        /// <summary>
        /// Init method that is called by the InsertOrUpdate/Replace nodes
        /// </summary>
        /// <param name="myTypeName"></param>
        /// <param name="myAttributeAssignList"></param>
        public void Init(String myTypeName, 
                            List<AAttributeAssignOrUpdate> myAttributeAssignList)
        {
            _AttributeAssignList = myAttributeAssignList;
            _TypeName = myTypeName;
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get type for name

            _TypeName = ((AstNode)parseNode.ChildNodes[2].AstNode).AsString;

            #endregion

            #region get myAttributes

            if (HasChildNodes(parseNode.ChildNodes[3]))
                _AttributeAssignList = ((parseNode
                                            .ChildNodes[3]
                                            .ChildNodes[1]
                                            .AstNode as AttributeAssignListNode).AttributeAssigns);

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Insert"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, 
                                            IGraphQL myGraphQL, 
                                            GQLPluginManager myPluginManager, 
                                            String myQuery, 
                                            SecurityToken mySecurityToken, 
                                            TransactionToken myTransactionToken)
        {
            _queryString = myQuery;

            QueryResult result;

            try
            {
                result = myGraphDB.Insert<QueryResult>(
                        mySecurityToken,
                        myTransactionToken,
                        CreateRequest(myPluginManager, 
                                        myGraphDB, 
                                        mySecurityToken, 
                                        myTransactionToken),
                        CreateQueryResult);
            }
            catch (ASonesException e)
            {
                result = new QueryResult(_queryString, SonesGQLConstants.GQL, 0, ResultType.Failed, null, e);
            }

            return result;
        }

        #endregion

        #region private helper

        /// <summary>
        /// Creates the query result
        /// </summary>
        /// <param name="myStats">The stats of the request</param>
        /// <param name="myCreatedVertex">The vertex that has been created</param>
        /// <returns>The created query result</returns>
        private QueryResult CreateQueryResult(IRequestStatistics myStats, IVertex myCreatedVertex)
        {
            return new QueryResult(_queryString, 
                                    SonesGQLConstants.GQL,
                                    Convert.ToUInt64(myStats.ExecutionTime.TotalMilliseconds), 
                                    ResultType.Successful,
                                    new List<IVertexView> {CreateAVertexView(myCreatedVertex)});
        }

        private IVertexView CreateAVertexView(IVertex myCreatedVertex)
        {
            return new VertexView(new Dictionary<string, object>
                                                         {
                                                             {"VertexID", myCreatedVertex.VertexID},
                                                             {"VertexTypeID", myCreatedVertex.VertexTypeID}
                                                         }, null);
        }

        /// <summary>
        /// Creates the request for the graphdb
        /// </summary>
        /// <returns>The created vertex</returns>
        private RequestInsertVertex CreateRequest(GQLPluginManager myPluginManager, 
                                                    IGraphDB myGraphDB, 
                                                    SecurityToken mySecurityToken, 
                                                    TransactionToken myTransactionToken)
        {
            #region data

            var result = new RequestInsertVertex(_TypeName);

            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_TypeName),
                (stats, vtype) => vtype);

            #endregion

            if (_AttributeAssignList != null)
            {
                foreach (var aAttributeDefinition in _AttributeAssignList)
                {
                    ProcessAAttributeDefinition(myPluginManager, 
                                                myGraphDB, 
                                                mySecurityToken, 
                                                myTransactionToken,
                                                vertexType, 
                                                aAttributeDefinition, 
                                                ref result);
                }
            }

            return result;
        }

        private static void ProcessAAttributeDefinition(GQLPluginManager myPluginManager, 
                                                        IGraphDB myGraphDB, 
                                                        SecurityToken mySecurityToken, 
                                                        TransactionToken myTransactionToken, 
                                                        IVertexType vertexType, 
                                                        AAttributeAssignOrUpdate aAttributeDefinition, 
                                                        ref RequestInsertVertex result)
        {
            if (vertexType.HasAttribute(aAttributeDefinition.AttributeIDChain.ContentString))
            {
                ProcessStructuredProperty(myPluginManager, 
                                            myGraphDB, 
                                            mySecurityToken, 
                                            myTransactionToken, 
                                            vertexType, 
                                            aAttributeDefinition, 
                                            ref result);
            }
            else
            {
                ProcessUnstructuredAttribute(vertexType, 
                                                aAttributeDefinition, 
                                                ref result);                                
            }
        }

        private static void ProcessStructuredProperty(GQLPluginManager myPluginManager, 
                                                        IGraphDB myGraphDB, 
                                                        SecurityToken mySecurityToken, 
                                                        TransactionToken myTransactionToken, 
                                                        IVertexType vertexType, 
                                                        AAttributeAssignOrUpdate aAttributeDefinition, 
                                                        ref RequestInsertVertex result)
        {
            #region AttributeAssignOrUpdateValue

            if (aAttributeDefinition is AttributeAssignOrUpdateValue)
            {
                var value = aAttributeDefinition as AttributeAssignOrUpdateValue;

                result.AddUnknownProperty(value.AttributeIDChain.ContentString, value.Value);

                return;
            }

            #endregion

            #region AttributeAssignOrUpdateList

            if (aAttributeDefinition is AttributeAssignOrUpdateList)
            {
                var value = aAttributeDefinition as AttributeAssignOrUpdateList;

                switch (value.CollectionDefinition.CollectionType)
                {
                    case CollectionType.Set:

                        #region set

                        if (!vertexType.HasAttribute(aAttributeDefinition.AttributeIDChain.ContentString))
                        {
                            throw new InvalidVertexAttributeException(String.Format("The vertex type {0} has no attribute named {1}.", 
                                                                        vertexType.Name, 
                                                                        aAttributeDefinition.AttributeIDChain.ContentString));
                        }

                        IAttributeDefinition attribute = vertexType.GetAttributeDefinition(aAttributeDefinition
                                                                                            .AttributeIDChain
                                                                                            .ContentString);

                        EdgePredefinition edgeDefinition = new EdgePredefinition(value.AttributeIDChain.ContentString);

                        foreach (var aTupleElement in (TupleDefinition)value.CollectionDefinition.TupleDefinition)
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

                        result.AddEdge(edgeDefinition);

                        #endregion)

                        return;
                    case CollectionType.List:

                        #region list

                        //has to be list of comparables
                        ListCollectionWrapper listWrapper = new ListCollectionWrapper();

                        Type myRequestedType;
                        if (vertexType.HasProperty(aAttributeDefinition.AttributeIDChain.ContentString))
                        {
                            myRequestedType = ((IPropertyDefinition)vertexType
                                                .GetAttributeDefinition(aAttributeDefinition.AttributeIDChain.ContentString)).BaseType;
                        }
                        else
                        {
                            myRequestedType = typeof(String);
                        }

                        foreach (var aTupleElement in (TupleDefinition)value.CollectionDefinition.TupleDefinition)
                        {
                            listWrapper.Add(((ValueDefinition)aTupleElement.Value).Value.ConvertToIComparable(myRequestedType));
                        }

                        result.AddStructuredProperty(aAttributeDefinition.AttributeIDChain.ContentString, listWrapper);

                        #endregion)

                        return;
                    case CollectionType.SetOfUUIDs:

                        #region SetOfUUIDs

                        EdgePredefinition anotheredgeDefinition = new EdgePredefinition(value.AttributeIDChain.ContentString);

                        foreach (var aTupleElement in ((VertexTypeVertexIDCollectionNode)value.CollectionDefinition.TupleDefinition).Elements)
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

                        result.AddEdge(anotheredgeDefinition);

                        #endregion

                        return;
                    default:
                        return;
                }

            }

            #endregion

            #region SetRefNode

            if (aAttributeDefinition is AttributeAssignOrUpdateSetRef)
            {
                var value = aAttributeDefinition as AttributeAssignOrUpdateSetRef;

                var edgeDefinition = new EdgePredefinition(value.AttributeIDChain.ContentString);

                if (value.SetRefDefinition.IsREFUUID)
                {
                    #region direct vertex ids

                    foreach (var aTupleElement in value.SetRefDefinition.TupleDefinition)
                    {
                        if (aTupleElement.Value is ValueDefinition)
                        {
                            #region ValueDefinition

                            foreach (var aProperty in aTupleElement.Parameters)
                            {
                                edgeDefinition.AddUnknownProperty(aProperty.Key, aProperty.Value);
                            }

                            edgeDefinition.AddVertexID(value.SetRefDefinition.ReferencedVertexType, 
                                                        Convert.ToInt64(((ValueDefinition) aTupleElement.Value).Value));

                            #endregion
                        }
                        else
                        {
                            throw new NotImplementedQLException("TODO");
                        }
                    }

                    #endregion
                }
                else
                {
                    #region expression

                    if (!vertexType.HasAttribute(aAttributeDefinition.AttributeIDChain.ContentString))
                    {
                        throw new InvalidVertexAttributeException(String.Format("The vertex type {0} has no attribute named {1}.", 
                                                                    vertexType.Name, 
                                                                    aAttributeDefinition.AttributeIDChain.ContentString));
                    }
                    IAttributeDefinition attribute = vertexType.GetAttributeDefinition(aAttributeDefinition.AttributeIDChain.ContentString);

                    foreach (var aTupleElement in value.SetRefDefinition.TupleDefinition)
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
                                throw new ReferenceAssignmentExpectedException(String.Format("It is not possible to create a single edge pointing to {0} vertices", 
                                                                                vertexIDs.Count));
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
                }

                result.AddEdge(edgeDefinition);

                return;
            }

            #endregion
        }

        private static void ProcessUnstructuredAttribute(IVertexType vertexType, 
                                                            AAttributeAssignOrUpdate aAttributeDefinition, 
                                                            ref RequestInsertVertex result)
        {
            #region AttributeAssignOrUpdateValue

            if (aAttributeDefinition is AttributeAssignOrUpdateValue)
            {
                var value = aAttributeDefinition as AttributeAssignOrUpdateValue;
                
                result.AddUnstructuredProperty(value.AttributeIDChain.ContentString, value.Value);

                return;
            }

            #endregion

            else
            {
                throw new NotImplementedQLException("TODO");
            }
        }

        private static IEnumerable<IVertex> ProcessBinaryExpression(BinaryExpressionDefinition binExpression, 
                                                                    GQLPluginManager myPluginManager, 
                                                                    IGraphDB myGraphDB, 
                                                                    SecurityToken mySecurityToken, 
                                                                    TransactionToken myTransactionToken, 
                                                                    IVertexType vertexType)
        {
            binExpression.Validate(myPluginManager, 
                                    myGraphDB, 
                                    mySecurityToken, 
                                    myTransactionToken, 
                                    vertexType);

            var expressionGraph = binExpression.Calculon(myPluginManager, 
                                                            myGraphDB, 
                                                            mySecurityToken, 
                                                            myTransactionToken, 
                                                            new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), 
                                                            false);

            return
                expressionGraph.Select(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true);
        }

        #endregion

    }
}
