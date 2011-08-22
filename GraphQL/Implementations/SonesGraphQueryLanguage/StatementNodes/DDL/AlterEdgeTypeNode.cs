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
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.StatementNodes.DDL
{
    /// <summary>
    /// This node is requested in case of an Alter Type statement.
    /// </summary>
    public sealed class AlterEdgeTypeNode : AStatement, IAstNodeInit
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
                    var alterCommand = (AlterEdgeTypeCommandNode)alterCmds.AstNode;

                    if (alterCommand.AlterTypeCommand != null)
                    {
                        _AlterTypeCommand.Add(alterCommand.AlterTypeCommand);
                    }
                }
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "AlterEdgeType"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, 
                                            IGraphQL myGraphQL, 
                                            GQLPluginManager myPluginManager, 
                                            String myQuery, 
                                            SecurityToken mySecurityToken, 
                                            TransactionToken myTransactionToken)
        {
            _query = myQuery;

            return myGraphDB.AlterEdgeType<QueryResult>(
                mySecurityToken,
                myTransactionToken,
                CreateNewRequest(myGraphDB, 
                                    myPluginManager, 
                                    mySecurityToken, 
                                    myTransactionToken),
                CreateOutput);
        }

        #endregion

        #region private helper

        private RequestAlterEdgeType CreateNewRequest(IGraphDB myGraphDB, 
                                                        GQLPluginManager myPluginManager, 
                                                        SecurityToken mySecurityToken, 
                                                        TransactionToken myTransactionToken)
        {
            RequestAlterEdgeType result = new RequestAlterEdgeType(_TypeName);

            foreach (var aAlterCommand in _AlterTypeCommand)
            {
                ProcessAlterCommand(aAlterCommand, ref result);
            }

            return result;
        }

        private void ProcessAlterCommand(AAlterTypeCommand myAlterCommand, ref RequestAlterEdgeType result)
        {
            switch (myAlterCommand.AlterType)
            {
                case TypesOfAlterCmd.AddAttribute:
                    ProcessAddAttribute(myAlterCommand, ref result);
                    break;

                case TypesOfAlterCmd.DropAttribute:
                    ProcessDropAttribute(myAlterCommand, ref result);
                    break;

                case TypesOfAlterCmd.RenameAttribute:
                    ProcessRenameAttribute(myAlterCommand, ref result);
                    break;

                case TypesOfAlterCmd.RenameType:
                    ProcessRenameType(myAlterCommand, ref result);
                    break;

                case TypesOfAlterCmd.ChangeComment:
                    ProcessChangeComment(myAlterCommand, ref result);
                    break;

                default:
                    break;
            }
        }

        private void ProcessChangeComment(AAlterTypeCommand myAlterCommand, 
                                            ref RequestAlterEdgeType result)
        {
            var command = (AlterType_ChangeComment)myAlterCommand;

            result.SetComment(command.NewComment);
        }

        private void ProcessRenameType(AAlterTypeCommand myAlterCommand,
                                            ref RequestAlterEdgeType result)
        {
            var command = (AlterType_RenameType)myAlterCommand;

            result.RenameType(command.NewName);
        }

        private void ProcessRenameAttribute(AAlterTypeCommand myAlterCommand,
                                            ref RequestAlterEdgeType result)
        {
            var command = (AlterType_RenameAttribute)myAlterCommand;

            result.RenameAttribute(command.OldName, command.NewName);
        }

        private void ProcessDropAttribute(AAlterTypeCommand myAlterCommand,
                                            ref RequestAlterEdgeType result)
        {
            var command = (AlterType_DropAttributes)myAlterCommand;

            foreach (var aAttribute in command.ListOfAttributes)
            {
                result.RemoveUnknownAttribute(aAttribute);
            }
        }

        private void ProcessAddAttribute(AAlterTypeCommand myAlterCommand,
                                            ref RequestAlterEdgeType result)
        {
            var command = (AlterEdgeType_AddAttributes)myAlterCommand;

            if (command.ListOfAttributes != null && command.ListOfAttributes.Count > 0)
                foreach (var aAttribute in command.ListOfAttributes)
                {
                    result.AddUnknownAttribute(GenerateUnknownAttribute(aAttribute));
                }
        }

        private QueryResult CreateOutput(IRequestStatistics myStats, 
                                            IEdgeType myALteredType)
        {
            return new QueryResult(_query, 
                                    SonesGQLConstants.GQL, 
                                    Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), 
                                    ResultType.Successful, 
                                    CreateVertexViews(myALteredType));
        }

        private IEnumerable<IVertexView> CreateVertexViews(IEdgeType myALteredType)
        {
            yield return new VertexView(new Dictionary<string, object>
                                                         {
                                                             {SonesGQLConstants.EdgeType, myALteredType.Name},
                                                             {"EdgeTypeID", myALteredType.ID}
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
            UnknownAttributePredefinition result = 
                new UnknownAttributePredefinition(myAttributeDefinition.AttributeName, 
                                                    myAttributeDefinition.AttributeType.Name);

            if (myAttributeDefinition.DefaultValue != null)
                result.SetDefaultValue(myAttributeDefinition.DefaultValue.ToString());

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
