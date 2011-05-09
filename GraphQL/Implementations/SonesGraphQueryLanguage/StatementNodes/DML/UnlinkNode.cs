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
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphDB.TypeSystem;
using System.Diagnostics;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class UnlinkNode : AStatement, IAstNodeInit
    {
        #region data

        private TypeReferenceDefinition                     _SourceType;
        private TupleDefinition                             _Targets;
        private HashSet<AAttributeAssignOrUpdateOrRemove>   _Sources;
        private BinaryExpressionDefinition                  _Condition;
        private String                                      _query;

        #endregion

        #region constructors

        public UnlinkNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _Sources = new HashSet<AAttributeAssignOrUpdateOrRemove>();

            #region VIA edge

            IDNode _EdgeAttr = null;

            if (parseNode.ChildNodes[4].AstNode is IDNode) //Semantic Web Yoda style
            {
                _EdgeAttr = (parseNode.ChildNodes[4].AstNode as IDNode);
            }
            else //Human language style
            {
                _EdgeAttr = (parseNode.ChildNodes[6].AstNode as IDNode);
            }

            #endregion

            #region sources

            var typeNode = (parseNode.ChildNodes[1].AstNode as ATypeNode);

            var tupleDef = (parseNode.ChildNodes[2].AstNode as TupleNode).TupleDefinition;

            var tupleDefSourceType = new TupleDefinition(tupleDef.KindOfTuple);

            foreach (var item in tupleDef.TupleElements)
            {
                var attrName = typeNode.ReferenceAndType.TypeName + SonesGQLConstants.EdgeTraversalDelimiterSymbol + ((IDChainDefinition)((BinaryExpressionDefinition)item.Value).Left).ContentString;
                var leftNode = new IDChainDefinition(attrName, new List<TypeReferenceDefinition>() { typeNode.ReferenceAndType });
                leftNode.AddPart(new ChainPartTypeOrAttributeDefinition(((IDChainDefinition)((BinaryExpressionDefinition)item.Value).Left).ContentString));
                var rightNode = ((BinaryExpressionDefinition)item.Value).Right;

                var binExpression = new BinaryExpressionDefinition(((BinaryExpressionDefinition)item.Value).OperatorSymbol, leftNode, rightNode);

                tupleDefSourceType.AddElement(new TupleElement(binExpression));
            }

            _Sources.Add(new AttributeRemoveList(_EdgeAttr.IDChainDefinition, _EdgeAttr.IDChainDefinition.TypeName, tupleDefSourceType));

            #endregion

            #region sources FROM

            if (parseNode.ChildNodes[6].ChildNodes[0].AstNode is ATypeNode)  //Semantic Web Yoda style
            {
                typeNode = (parseNode.ChildNodes[6].ChildNodes[0].AstNode as ATypeNode);
                _Targets = (parseNode.ChildNodes[6].ChildNodes[1].AstNode as TupleNode).TupleDefinition;
            }
            else  //Human language style
            {
                typeNode = (parseNode.ChildNodes[4].ChildNodes[0].AstNode as ATypeNode);
                _Targets = (parseNode.ChildNodes[4].ChildNodes[1].AstNode as TupleNode).TupleDefinition;
            }

            _SourceType = typeNode.ReferenceAndType;

            if (_Targets.Count() > 1)
            {
                var firstElement = (BinaryExpressionDefinition)_Targets.First().Value;
                _Targets.Remove(_Targets.First());

                _Condition = GetConditionNode("OR", firstElement, _Targets);
            }
            else
            {
                _Condition = (BinaryExpressionDefinition)_Targets.First().Value;
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Unlink"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            _query = myQuery;

            //prepare
            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_SourceType.TypeName),
                (stats, vtype) => vtype);

            //validate
            _Condition.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

            //calculate
            var expressionGraph = _Condition.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

            //extract

            var myToBeUpdatedVertices = expressionGraph.SelectVertexIDs(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true).ToList();

            if (myToBeUpdatedVertices.Count > 0)
            {

                //update
                ProcessUpdate(myToBeUpdatedVertices, myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

            }

            sw.Stop();

            return GenerateResult(sw.Elapsed.TotalMilliseconds);
        }

        #endregion

        #region private helpers

        private QueryResult GenerateResult(double myElapsedTotalMilliseconds)
        {
            return new QueryResult(_query, SonesGQLConstants.GQL, Convert.ToUInt64(myElapsedTotalMilliseconds), ResultType.Successful, new List<IVertexView>());
        }

        private void ProcessUpdate(IEnumerable<long> myVertexIDs, IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            UpdateNode update = new UpdateNode();
            update.Init(_SourceType.TypeName, _Sources, myVertexIDs);

            update.Execute(myGraphDB, null, myPluginManager, _query, mySecurityToken, myTransactionToken);
        }

        private BinaryExpressionDefinition GetConditionNode(String myOperator, BinaryExpressionDefinition myPrevNode, TupleDefinition myNodeList)
        {
            var binElem = myNodeList.FirstOrDefault();

            if (binElem == null)
            {
                return myPrevNode;
            }

            myNodeList.Remove(binElem);

            var binNode = new BinaryExpressionDefinition(myOperator, myPrevNode, binElem.Value);
            return GetConditionNode(myOperator, binNode, myNodeList);
        }

        #endregion
    }
}
