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
using sones.GraphDB.Expression.Tree.Literals;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class InsertNode : AStatement, IAstNodeInit
    {
        private String _TypeName;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private String _queryString;

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get type for name

            _TypeName = GetTypeReferenceDefinitions(context).First().TypeName;

            #endregion

            #region get myAttributes

            if (HasChildNodes(parseNode.ChildNodes[3]))
            {
                _AttributeAssignList = ((parseNode.ChildNodes[3].ChildNodes[1].AstNode as AttrAssignListNode).AttributeAssigns);
            }

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

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            _queryString = myQuery;

            QueryResult result;

            try
            {
                result = myGraphDB.Insert<QueryResult>(
                        mySecurityToken,
                        myTransactionToken,
                        CreateRequest(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken),
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
            return new QueryResult(_queryString, SonesGQLConstants.GQL,
                                   Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), ResultType.Successful,
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
        private RequestInsertVertex CreateRequest(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            #region data

            var result = new RequestInsertVertex(_TypeName);

            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_TypeName),
                (stats, vtype) => vtype);

            #endregion

            foreach (var aAttributeDefinition in _AttributeAssignList)
            {
                ProcessAAttributeDefinition(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType, aAttributeDefinition, ref result);
            }

            return result;
        }

        private static void ProcessAAttributeDefinition(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexType vertexType, AAttributeAssignOrUpdate aAttributeDefinition, ref RequestInsertVertex result)
        {
            if (vertexType.HasAttribute(aAttributeDefinition.AttributeIDChain.ContentString))
            {
                ProcessStructuredProperty(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType, aAttributeDefinition, ref result);
            }
            else
            {
                ProcessUnstructuredAttribute(vertexType, aAttributeDefinition, ref result);                                
            }
        }

        private static void ProcessStructuredProperty(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexType vertexType, AAttributeAssignOrUpdate aAttributeDefinition, ref RequestInsertVertex result)
        {
            #region data

            IAttributeDefinition attribute =
                vertexType.GetAttributeDefinition(aAttributeDefinition.AttributeIDChain.ContentString);

            #endregion

            #region AttributeAssignOrUpdateValue

            if (aAttributeDefinition is AttributeAssignOrUpdateValue)
            {
                var value = aAttributeDefinition as AttributeAssignOrUpdateValue;
                
                result.AddStructuredProperty(value.AttributeIDChain.ContentString, (IComparable)Convert.ChangeType(value.Value, ((IPropertyDefinition)attribute).BaseType));

                return;
            }

            #endregion

            #region AttributeAssignOrUpdateValue

            if (aAttributeDefinition is AttributeAssignOrUpdateList)
            {
                var value = aAttributeDefinition as AttributeAssignOrUpdateList;

                switch (value.CollectionDefinition.CollectionType)
                {
                    case CollectionType.Set:

                        #region set

                        EdgePredefinition edgeDefinition = new EdgePredefinition(value.AttributeIDChain.ContentString);

                        foreach (var aTupleElement in value.CollectionDefinition.TupleDefinition)
                        {
                            
                            if (aTupleElement.Value is BinaryExpressionDefinition)
                            {
                                #region BinaryExpressionDefinition

                                foreach (var aVertexID in ProcessBinaryExpression(
                                    (BinaryExpressionDefinition)aTupleElement.Value,
                                    myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType))
                                {
                                    var inneredge = new EdgePredefinition().AddVertexID(aVertexID);
                                    
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
                        var property = (IPropertyDefinition) attribute;
                        ListCollectionWrapper listWrapper = new ListCollectionWrapper();

                        foreach (var aTupleElement in value.CollectionDefinition.TupleDefinition)
                        {
                            listWrapper.AddElement((IComparable)Convert.ChangeType(((ValueDefinition)aTupleElement.Value).Value, property.BaseType));
                        }

                        result.AddStructuredProperty(aAttributeDefinition.AttributeIDChain.ContentString, listWrapper);

                        #endregion)

                        return;
                    case CollectionType.SetOfUUIDs:

                        EdgePredefinition anotheredgeDefinition = new EdgePredefinition(value.AttributeIDChain.ContentString);

                        foreach (var aTupleElement in value.CollectionDefinition.TupleDefinition)
                        {
                            if (aTupleElement.Value is ValueDefinition)
                            {
                                var innerEdge = new EdgePredefinition();

                                foreach (var aStructuredProperty in aTupleElement.Parameters)
                                {
                                    innerEdge.AddUnknownProperty(aStructuredProperty.Key, aStructuredProperty.Value);
                                }

                                innerEdge.AddVertexID(Convert.ToInt64(((ValueDefinition)aTupleElement.Value).Value));

                                anotheredgeDefinition.AddEdge(innerEdge);
                            }
                            else
                            {
                                throw new NotImplementedQLException("TODO");
                            }
                        }

                        result.AddEdge(anotheredgeDefinition);

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

                            edgeDefinition.AddVertexID(Convert.ToInt64(((ValueDefinition) aTupleElement.Value).Value));

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

                    foreach (var aTupleElement in value.SetRefDefinition.TupleDefinition)
                    {
                        if (aTupleElement.Value is BinaryExpressionDefinition)
                        {
                            #region BinaryExpressionDefinition

                            var vertexIDs = ProcessBinaryExpression(
                                (BinaryExpressionDefinition)aTupleElement.Value,
                                myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType).ToList();

                            if (vertexIDs.Count > 1)
                            {
                                throw new ReferenceAssignmentExpectedException(String.Format("It is not possible to create a single edge pointing to {0} vertices", vertexIDs.Count));
                            }

                            var inneredge = new EdgePredefinition();

                            foreach (var aStructuredProperty in aTupleElement.Parameters)
                            {
                                edgeDefinition.AddUnknownProperty(aStructuredProperty.Key, aStructuredProperty.Value);
                            }

                            edgeDefinition.AddVertexID(vertexIDs.FirstOrDefault());

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

        private static void ProcessUnstructuredAttribute(IVertexType vertexType, AAttributeAssignOrUpdate aAttributeDefinition, ref RequestInsertVertex result)
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

        private static IEnumerable<long> ProcessBinaryExpression(BinaryExpressionDefinition binExpression, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexType vertexType)
        {
            binExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

            var expressionGraph = binExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

            return
                expressionGraph.SelectVertexIDs(
                    new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true);
        }

        #endregion

    }
}
