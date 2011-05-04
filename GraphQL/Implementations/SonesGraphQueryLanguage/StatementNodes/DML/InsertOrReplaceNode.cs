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
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class InsertOrReplaceNode : AStatement, IAstNodeInit
    {
        #region data
		
        private BinaryExpressionDefinition _WhereExpression;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private String _Type;

	    #endregion

        #region Constructor

        public InsertOrReplaceNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                //get type
                if (parseNode.ChildNodes[1] != null && parseNode.ChildNodes[1].AstNode != null)
                {
                    _Type = ((ATypeNode)(parseNode.ChildNodes[1].AstNode)).ReferenceAndType.TypeName;
                }
                else
                {
                    throw new NotImplementedQLException("");
                }

                if (parseNode.ChildNodes[3] != null && HasChildNodes(parseNode.ChildNodes[3]))
                {

                    _AttributeAssignList = (parseNode.ChildNodes[3].AstNode as AttributeAssignListNode).AttributeAssigns;

                }

                if (parseNode.ChildNodes[4] != null && ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinaryExpressionDefinition != null)
                {
                    _WhereExpression = ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinaryExpressionDefinition;

                }
            }   
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "InsertOrReplace"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            //var qresult = graphDBSession.InsertOrReplace(_Type, _AttributeAssignList, _WhereExpression);
            //qresult.PushIExceptional(ParsingResult);
            //return qresult;

            return null;
        }

        #endregion
    }
}
