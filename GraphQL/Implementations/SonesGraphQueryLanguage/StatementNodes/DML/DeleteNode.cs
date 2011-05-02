using System;
using System.Collections.Generic;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request.Delete;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class DeleteNode : AStatement, IAstNodeInit
    {
        #region Data

        private BinaryExpressionDefinition _WhereExpression;

        private List<IDChainDefinition> _IDChainDefinitions;

        private List<TypeReferenceDefinition> _TypeReferenceDefinitions;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _IDChainDefinitions = new List<IDChainDefinition>();

            _TypeReferenceDefinitions = (parseNode.ChildNodes[1].AstNode as TypeListNode).Types;

            if (HasChildNodes(parseNode.ChildNodes[3]))
            {
                IDNode tempIDNode;
                foreach (var _ParseTreeNode in parseNode.ChildNodes[3].ChildNodes[0].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode is IDNode)
                    {
                        tempIDNode = (IDNode)_ParseTreeNode.AstNode;
                        _IDChainDefinitions.Add(tempIDNode.IDChainDefinition);
                    }
                }
            }
            else
            {
                foreach (var type in _TypeReferenceDefinitions)
                {
                    var def = new IDChainDefinition();
                    def.AddPart(new ChainPartTypeOrAttributeDefinition(type.Reference));
                    _IDChainDefinitions.Add(def);
                }
            }

            #region whereClauseOpt

            if (parseNode.ChildNodes[4].ChildNodes != null && parseNode.ChildNodes[4].ChildNodes.Count != 0)
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinaryExpressionDefinition;

            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Delete"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            QueryResult qresult = null;

            try
            {
                var stat = myGraphDB.Delete(mySecurityToken, myTransactionToken, new RequestDelete(), (stats) => stats);

                qresult = new QueryResult(myQuery, "sones.gql", Convert.ToUInt64(stat.ExecutionTime.Milliseconds), ResultType.Successful);
            }
            catch (ASonesException e)
            {
                qresult.Error = e;
            }

            return qresult;
        }

        #endregion
    }
}
