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
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphDB.Request.CreateVertexTypes;

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

            _TypeName = myParseTreeNode.ChildNodes[3].Token.ValueString;

            #region Get the AlterTypeCommand

            foreach (var alterCmds in myParseTreeNode.ChildNodes[4].ChildNodes)
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

            if (HasChildNodes(myParseTreeNode.ChildNodes[5]))
            {
                _AlterTypeCommand.Add(new AlterType_SetUnique()
                                          {
                                              UniqueAttributes =
                                                  ((UniqueAttributesOptNode) myParseTreeNode.ChildNodes[5].AstNode).
                                                  UniqueAttributes
                                          });
            }

            if (HasChildNodes(myParseTreeNode.ChildNodes[6]))
            {
                _AlterTypeCommand.Add(new AlterType_SetMandatory()
                                          {
                                              MandatoryAttributes =
                                                  ((MandatoryOptNode) myParseTreeNode.ChildNodes[6].AstNode).
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
            _query = myQuery;

            return myGraphDB.AlterVertexType<QueryResult>(
                mySecurityToken,
                myTransactionToken,
                CreateNewRequest(myGraphDB, myPluginManager, mySecurityToken, myTransactionToken),
                CreateOutput);
        }

        #endregion

        #region private helper

        private RequestAlterVertexType CreateNewRequest(IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            RequestAlterVertexType result = new RequestAlterVertexType(_TypeName);

            foreach (var aAlterCommand in _AlterTypeCommand)
            {
                ProcessAlterCommand(aAlterCommand, ref result);
            }

            return result;
        }

        private void ProcessAlterCommand(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            switch (myAlterCommand.AlterType)
            {
                case TypesOfAlterCmd.Add:

                    ProcessAdd(myAlterCommand, ref result);

                    break;
                case TypesOfAlterCmd.Drop:

                    ProcessDrop(myAlterCommand, ref result);

                    break;
                case TypesOfAlterCmd.RenameAttribute:

                    ProcessRenameAttribute(myAlterCommand, ref result);

                    break;
                case TypesOfAlterCmd.RenameVertexType:

                    ProcessRenameVertexType(myAlterCommand, ref result);

                    break;
                case TypesOfAlterCmd.RenameIncomingEdge:

                    ProcessRenameIncomingEdge(myAlterCommand, ref result);

                    break;
                case TypesOfAlterCmd.Unqiue:

                    ProcessUnique(myAlterCommand, ref result);

                    break;
                case TypesOfAlterCmd.DropUnqiue:

                    ProcessDropUnique(myAlterCommand, ref result);                    

                    break;
                case TypesOfAlterCmd.Mandatory:

                    ProcessMandatory(myAlterCommand, ref result);                    

                    break;
                case TypesOfAlterCmd.DropMandatory:

                    ProcessDropMandatory(myAlterCommand, ref result);                    

                    break;
                case TypesOfAlterCmd.ChangeComment:

                    ProcessChangeComment(myAlterCommand, ref result);                    

                    break;
                default:
                    break;
            }
        }

        private void ProcessChangeComment(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessDropMandatory(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessMandatory(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessDropUnique(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessUnique(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessRenameIncomingEdge(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessRenameVertexType(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessRenameAttribute(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessDrop(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            throw new NotImplementedException();
        }

        private void ProcessAdd(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_AddAttributes)myAlterCommand;

            foreach (var aAttribute in command.ListOfAttributes)
            {
                result.AddToBeAddedUnknownAttribute(GenerateUnknownAttribute(aAttribute));
            }
        }

        private QueryResult CreateOutput(IRequestStatistics myStats, IVertexType myALteredVertexType)
        {
            return new QueryResult(_query, SonesGQLConstants.GQL, Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), ResultType.Successful, CreateVertexViews(myALteredVertexType));
        }

        private IEnumerable<IVertexView> CreateVertexViews(IVertexType myALteredVertexType)
        {
            yield return new VertexView(new Dictionary<string,object>
                                                         {
                                                             {SonesGQLConstants.VertexType, myALteredVertexType.Name},
                                                             {"VertexTypeID", myALteredVertexType.ID}
                                                         }, null);

            yield break;
        }

        /// <summary>
        /// Generates a attribute definition
        /// </summary>
        /// <param name="aAttribute">The attribute that is going to be transfered</param>
        /// <returns>A attribute predefinition</returns>
        private UnknownAttributePredefinition GenerateUnknownAttribute(AttributeDefinition myAttributeDefinition)
        {
            UnknownAttributePredefinition result = new UnknownAttributePredefinition(myAttributeDefinition.AttributeName);

            result.SetAttributeType(myAttributeDefinition.AttributeType.Name);
            
            if (myAttributeDefinition.AttributeType.EdgeType != null)
            {
                result.SetInnerEdgeType(myAttributeDefinition.AttributeType.EdgeType);
            }

            if (myAttributeDefinition.DefaultValue != null)
            {
                result.SetDefaultValue(myAttributeDefinition.DefaultValue.ToString());
            }

            switch (myAttributeDefinition.AttributeType.Type)
            {
                case SonesGQLGrammar.TERMINAL_SET:

                    result.SetMultiplicityAsSet();

                    break;

                case SonesGQLGrammar.TERMINAL_LIST:

                    result.SetMultiplicityAsList();

                    break;
            }


            return result;
        }

        #endregion

    }
}
