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
        /// The Name of the type which should be updated
        /// </summary>
        private String _TypeName;

        /// <summary>
        /// The executed query
        /// </summary>
        private String Query;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Type

            _TypeName = (parseNode.ChildNodes[1].AstNode as ATypeNode).ReferenceAndType.TypeName;

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
            //prepare
            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_TypeName),
                (stats, vtype) => vtype);

            //validate
            _WhereExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

            //calculate
            var expressionGraph = _WhereExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

            //extract

            var myToBeUpdatedVertices = expressionGraph.SelectVertexIDs(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true);

            var result = new RequestUpdate(new RequestGetVertices(vertexType.ID, myToBeUpdatedVertices, false));

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
            else
            {
                throw new NotImplementedQLException("");
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

                #endregion
            }
            else
            {
                #region expression

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

                result.AddEdge(edgeDefinition);

                return;
            }

            #endregion
        }

        private void ProcessAttributeAssignOrUpdateList(IVertexType vertexType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, AttributeAssignOrUpdateList attributeAssignOrUpdateList, ref RequestUpdate result)
        {
            switch (attributeAssignOrUpdateList.CollectionDefinition.CollectionType)
            {
                case CollectionType.Set:

                    #region set

                    EdgePredefinition edgeDefinition = new EdgePredefinition(attributeAssignOrUpdateList.AttributeIDChain.ContentString);
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

                    result.AddEdge(edgeDefinition);

                    #endregion)

                    return;
                case CollectionType.List:

                    #region list

                    //has to be list of comparables
                    ListCollectionWrapper listWrapper = new ListCollectionWrapper();

                    Type myRequestedType;
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
                        listWrapper.AddElement((IComparable)Convert.ChangeType(((ValueDefinition)aTupleElement.Value).Value, myRequestedType));
                    }

                    result.AddUnknownProperty(attributeAssignOrUpdateList.AttributeIDChain.ContentString, listWrapper);

                    #endregion)

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

                    result.AddEdge(anotheredgeDefinition);

                    #endregion

                    return;
                default:
                    return;
            }
        }

        private void ProcessAttributeAssignOrUpdateValue(AttributeAssignOrUpdateValue attributeAssignOrUpdateValue, ref RequestUpdate result)
        {
            result.AddUnknownProperty(attributeAssignOrUpdateValue.AttributeIDChain.ContentString , attributeAssignOrUpdateValue.Value);
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
