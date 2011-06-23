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
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using System.Collections.Generic;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using System.Diagnostics;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class LinkNode : AStatement, IAstNodeInit
    {
        #region Data

        private String                                      _SourceType;
        private TupleDefinition                             _Sources;
        private HashSet<AAttributeAssignOrUpdateOrRemove>   _Targets;
        private BinaryExpressionDefinition                  _Condition;
        private String                                      _query;

        #endregion

        #region Constructors

        public LinkNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode myParseTreeNode)
        {
            var _GraphQLGrammar = (SonesGQLGrammar)context.Language.Grammar;

            _Targets = new HashSet<AAttributeAssignOrUpdateOrRemove>();


            #region FROM Sources            

            _SourceType = ((AstNode)myParseTreeNode.ChildNodes[1].AstNode).AsString;
            var typeNode = _SourceType;

            _Sources = (myParseTreeNode.ChildNodes[2].AstNode as TupleNode).TupleDefinition;

            if (_Sources.Count() > 1)
            {
                var firstElement = (BinaryExpressionDefinition)_Sources.First().Value;
                _Sources.Remove(_Sources.First());
                _Condition = GetConditionNode("OR", firstElement, _Sources);
            }

            else
            {
                _Condition = (BinaryExpressionDefinition)_Sources.First().Value;
            }

            #endregion

            #region Find statement "VIA Edge"

            IDNode _EdgeAttr = null;

            // Semantic Web Yoda-style...
            if (myParseTreeNode.ChildNodes[3].Token.KeyTerm == _GraphQLGrammar.S_VIA)
            {
                _EdgeAttr = (myParseTreeNode.ChildNodes[4].AstNode as IDNode);
            }

            else // Human language style...
            {
                if (myParseTreeNode.ChildNodes[5].Token.KeyTerm == _GraphQLGrammar.S_VIA)
                {
                    _EdgeAttr = (myParseTreeNode.ChildNodes[6].AstNode as IDNode);
                }
            }

            #endregion

            #region Find statement "TO Targets"

            TupleDefinition tupleDef = null;

            // Semantic Web Yoda-style...
            if (myParseTreeNode.ChildNodes[5].Token.KeyTerm == _GraphQLGrammar.S_TO)
            {
                typeNode = ((AstNode)(myParseTreeNode.ChildNodes[6].ChildNodes[0].AstNode)).AsString;
                tupleDef = (myParseTreeNode.ChildNodes[6].ChildNodes[1].AstNode as TupleNode).TupleDefinition;
            }

            else // Human language style...
            {
                if (myParseTreeNode.ChildNodes[3].Token.KeyTerm == _GraphQLGrammar.S_TO)
                {
                    typeNode = myParseTreeNode.ChildNodes[4].ChildNodes[0].AstNode.ToString();
                    tupleDef = (myParseTreeNode.ChildNodes[4].ChildNodes[1].AstNode as TupleNode).TupleDefinition;
                }
            }

            #endregion

            #region Processing...
                        
            var tupleDefTargetType = new TupleDefinition(tupleDef.KindOfTuple);

            foreach (var item in tupleDef.TupleElements)
            {                
                var attrName = typeNode + SonesGQLConstants.EdgeTraversalDelimiterSymbol + ((IDChainDefinition)((BinaryExpressionDefinition)item.Value).Left).ContentString;
                var leftNode = new IDChainDefinition(attrName, new List<TypeReferenceDefinition>() { new TypeReferenceDefinition(typeNode, typeNode) });
                leftNode.AddPart(new ChainPartTypeOrAttributeDefinition(((IDChainDefinition)((BinaryExpressionDefinition)item.Value).Left).ContentString));
                var rightNode = ((BinaryExpressionDefinition)item.Value).Right;

                var binExpression = new BinaryExpressionDefinition(((BinaryExpressionDefinition)item.Value).OperatorSymbol, leftNode, rightNode);

                tupleDefTargetType.AddElement(new TupleElement(binExpression));

            }

            _Targets.Add(new AttributeAssignOrUpdateList(new CollectionDefinition(CollectionType.Set, tupleDefTargetType), _EdgeAttr.IDChainDefinition, false));

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get
            {
                return "Link";
            }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get
            {
                return TypesOfStatements.ReadWrite;
            }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            _query = myQuery;

            //prepare
            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_SourceType),
                (stats, vtype) => vtype);

            //validate
            _Condition.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

            //calculate
            var expressionGraph = _Condition.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

            //extract

            var myToBeUpdatedVertices = expressionGraph.Select(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true).ToList();

            if (myToBeUpdatedVertices.Count > 0)
            {

                //update
                ProcessUpdate(myToBeUpdatedVertices, myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

            }

            sw.Stop();

            return GenerateResult(sw.Elapsed.TotalMilliseconds);
        }

        #endregion

        #region Private Helpers

        private QueryResult GenerateResult(double myElapsedTotalMilliseconds)
        {
            return new QueryResult(_query, SonesGQLConstants.GQL, Convert.ToUInt64(myElapsedTotalMilliseconds), ResultType.Successful, new List<IVertexView>());
        }

        private void ProcessUpdate(IEnumerable<IVertex> myVertexIDs, IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            UpdateNode update = new UpdateNode();
            update.Init(_SourceType, _Targets, myVertexIDs);

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
