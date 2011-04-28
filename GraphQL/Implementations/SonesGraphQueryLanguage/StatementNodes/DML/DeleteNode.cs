using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using System.Collections.Generic;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;

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

            if (parseNode.ChildNodes[3].ChildNodes != null && parseNode.ChildNodes[3].ChildNodes.Count != 0)
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
            //var qresult = myGraphDB.Delete(_TypeReferenceDefinitions, _IDChainDefinitions, _WhereExpression);
            //return qresult;

            return null;
        }

        #endregion
    }
}
