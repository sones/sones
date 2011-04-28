using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.DDL;

namespace sones.GraphQL.StatementNodes.DDL
{
    /// <summary>
    /// This node is requested in case of an Create Types statement.
    /// </summary>
    public sealed class CreateTypesNode : AStatement, IAstNodeInit
    {
        #region Data

        private List<GraphDBTypeDefinition> _TypeDefinitions = new List<GraphDBTypeDefinition>();

        #endregion

        #region constructor

        public CreateTypesNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode myParseTreeNode)
        {
            if (myParseTreeNode.ChildNodes.Count > 3)
            {

                #region Single type

                BulkTypeNode aTempNode = (BulkTypeNode)myParseTreeNode.ChildNodes[3].AstNode;

                Boolean isAbstract = false;

                if (HasChildNodes(myParseTreeNode.ChildNodes[1]))
                {
                    isAbstract = true;
                }

                _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, isAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));

                #endregion

            }

            else
            {

                #region Multi types

                foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[2].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode != null)
                    {
                        BulkTypeListMemberNode aTempNode = (BulkTypeListMemberNode)_ParseTreeNode.AstNode;
                        _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, aTempNode.IsAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));
                    }
                }

                #endregion

            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "CreateTypes"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            //var qresult = graphDBSession.CreateTypes(_TypeDefinitions);
            //qresult.PushIExceptional(ParsingResult);
            
            //return qresult;

            throw new NotImplementedException();
        }

        #endregion

    }
}
