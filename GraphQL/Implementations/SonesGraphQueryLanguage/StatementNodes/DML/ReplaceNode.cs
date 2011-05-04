using System;
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
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class ReplaceNode : AStatement, IAstNodeInit
    {
        #region Data
        
        private BinaryExpressionDefinition _whereExpression;
        private String _TypeName;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                _TypeName = ((ATypeNode)(parseNode.ChildNodes[1].AstNode)).ReferenceAndType.TypeName;

                if (parseNode.ChildNodes[3] != null && HasChildNodes(parseNode.ChildNodes[3]))
                {
                    _AttributeAssignList = (parseNode.ChildNodes[3].AstNode as AttributeAssignListNode).AttributeAssigns;
                }

                _whereExpression = ((BinaryExpressionNode)parseNode.ChildNodes[5].AstNode).BinaryExpressionDefinition;

                System.Diagnostics.Debug.Assert(_whereExpression != null);
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Replace"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            //var qresult = graphDBSession.Replace(_TypeName, _AttributeAssignList, _whereExpression);
            //qresult.PushIExceptional(ParsingResult);
            //return qresult;

            return null;
        }

        #endregion
    }
}
