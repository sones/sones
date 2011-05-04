using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.AlterType;
using sones.GraphQL.Structure.Nodes.DDL;
using sones.GraphDB.Request.GetVertexType;

namespace sones.GraphQL.StatementNodes.DDL
{
    /// <summary>
    /// This node is requested in case of an Alter Type statement.
    /// </summary>
    public sealed class AlterTypeNode : AStatement, IAstNodeInit
    {
        #region Data

        private String _TypeName = String.Empty; //the name of the type that should be altered
        private List<AAlterTypeCommand> _AlterTypeCommand;
        private String _query;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode myParseTreeNode)
        {

            _AlterTypeCommand = new List<AAlterTypeCommand>();

            _TypeName = myParseTreeNode.ChildNodes[2].Token.ValueString;

            #region Get the AlterTypeCommand

            foreach (var alterCmds in myParseTreeNode.ChildNodes[3].ChildNodes)
            {
                if (alterCmds.AstNode != null)
                {
                    var alterCommand = (AlterCommandNode) alterCmds.AstNode;

                    if (alterCommand.AlterTypeCommand != null)
                    {
                        _AlterTypeCommand.Add(alterCommand.AlterTypeCommand);
                    }
                }
            }

            if (HasChildNodes(myParseTreeNode.ChildNodes[4]))
            {
                _AlterTypeCommand.Add(new AlterType_SetUnique()
                                          {
                                              UniqueAttributes =
                                                  ((UniqueAttributesOptNode) myParseTreeNode.ChildNodes[4].AstNode).
                                                  UniqueAttributes
                                          });
            }

            if (HasChildNodes(myParseTreeNode.ChildNodes[5]))
            {
                _AlterTypeCommand.Add(new AlterType_SetMandatory()
                                          {
                                              MandatoryAttributes =
                                                  ((MandatoryOptNode) myParseTreeNode.ChildNodes[5].AstNode).
                                                  MandatoryAttribs
                                          });
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "AlterType"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            //_query = myQuery;

            //return myGraphDB.Alt AllVertexTypes<QueryResult>(
            //    mySecurityToken,
            //    myTransactionToken,
            //    CreateNewRequest(myGraphDB, myPluginManager, mySecurityToken, myTransactionToken),
            //    CreateOutput);

            return null;
        }

        #endregion

        //#region private helper

        //private RequestGetAllVertexTypes CreateNewRequest(IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

    }
}
