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
    public sealed class AlterVertexTypeNode : AStatement, IAstNodeInit
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
                    var alterCommand = (AlterVertexTypeCommandNode) alterCmds.AstNode;

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
            get { return "AlterVertexType"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, Int64 myTransactionToken)
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

        private RequestAlterVertexType CreateNewRequest(IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, Int64 myTransactionToken)
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
                case TypesOfAlterCmd.DropIndex:

                    ProcessDropIndex(myAlterCommand, ref result);

                    break;
                case TypesOfAlterCmd.AddIndex:

                    ProcessAddIndex(myAlterCommand, ref result);

                    break;
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
                case TypesOfAlterCmd.DefineAttribute:

                    ProcessDefineAttribute(myAlterCommand, ref result);                    

                    break;
                case TypesOfAlterCmd.UndefineAttribute:

                    ProcessUndefineAttribute(myAlterCommand, ref result);                    

                    break;
                default:
                    break;
            }
        }

        private void ProcessDropIndex(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_DropIndices)myAlterCommand;

            foreach (var aIndex in command.IdxDropList)
            {
                result.RemoveIndex(aIndex.Key, aIndex.Value);
            }
        }

        private void ProcessAddIndex(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_AddIndices)myAlterCommand;

            foreach (var aIndexDefinition in command.IdxDefinitionList)
            {
                result.AddIndex(GenerateIndex(aIndexDefinition));
            }
        }

        /// <summary>
        /// Generates a index predefinition
        /// </summary>
        /// <param name="aIndex">The index definition by the gql</param>
        /// <returns>An IndexPredefinition</returns>
        private IndexPredefinition GenerateIndex(IndexDefinition aIndex)
        {
            IndexPredefinition result;

            if (String.IsNullOrEmpty(aIndex.IndexName))
            {
                result = new IndexPredefinition(_TypeName);
            }
            else
            {
                result = new IndexPredefinition(aIndex.IndexName, _TypeName);
            }

            if (!String.IsNullOrEmpty(aIndex.IndexType))
            {
                result.SetIndexType(aIndex.IndexType);
            }

            result.SetEdition(aIndex.Edition);

            //options for the index
            if (aIndex.Options != null)
            {
                foreach (var aKV in aIndex.Options)
                {
                    result.AddOption(aKV.Key, aKV.Value);
                }
            }

            foreach (var aIndexProperty in aIndex.IndexAttributeDefinitions)
            {
                result.AddProperty(aIndexProperty.IndexAttribute.ContentString);
            }

            return result;
        }

        private void ProcessChangeComment(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_ChangeComment)myAlterCommand;

            result.SetComment(command.NewComment);
        }

        private void ProcessDropMandatory(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_DropMandatory)myAlterCommand;

            result.RemoveMandatory(command.DroppedMandatory);
        }

        private void ProcessMandatory(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_SetMandatory)myAlterCommand;

            foreach (var aMandatory in command.MandatoryAttributes)
            {
                result.AddMandatory(new MandatoryPredefinition(aMandatory));
            }
        }

        private void ProcessDropUnique(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_DropUnique)myAlterCommand;

            result.RemoveUnique(command.DroppedUnique);
        }

        private void ProcessUnique(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_SetUnique)myAlterCommand;

            foreach (var aUnique in command.UniqueAttributes)
            {
                result.AddUnique(new UniquePredefinition(aUnique));
            }

        }

        private void ProcessRenameIncomingEdge(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_RenameIncomingEdge)myAlterCommand;

            result.RenameAttribute(command.OldName, command.NewName);
        }

        private void ProcessRenameVertexType(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_RenameType)myAlterCommand;

            result.RenameType(command.NewName);
        }

        private void ProcessRenameAttribute(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_RenameAttribute)myAlterCommand;

            result.RenameAttribute(command.OldName, command.NewName);
        }

        private void ProcessDropAttribute(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_DropAttributes)myAlterCommand;

            foreach (var aAttribute in command.ListOfAttributes)
            {
                result.RemoveUnknownAttribute(aAttribute);
            }
        }

        private void ProcessAddAttribute(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterVertexType_AddAttributes)myAlterCommand;

            if (command.ListOfAttributes != null && command.ListOfAttributes.Count > 0)
            {
                foreach (var aAttribute in command.ListOfAttributes)
                {
                    result.AddUnknownAttribute(GenerateUnknownAttribute(aAttribute));
                }
            }
            else
            {
                if (command.BackwardEdgeInformation != null && command.BackwardEdgeInformation.Count > 0)
                {
                    foreach (var aIncomingEdge in command.BackwardEdgeInformation)
                    {
                        result.AddIncomingEdge(GenerateAIncomingEdge(aIncomingEdge));
                    }
                }
            }
            
        }

        private void ProcessDefineAttribute(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_DefineAttributes)myAlterCommand;

            if (command.ListOfAttributes != null && command.ListOfAttributes.Count > 0)
            {
                foreach (var aAttribute in command.ListOfAttributes)
                {
                    result.DefineAttribute(GenerateUnknownAttribute(aAttribute));
                }
            }
        }

        private void ProcessUndefineAttribute(AAlterTypeCommand myAlterCommand, ref RequestAlterVertexType result)
        {
            var command = (AlterType_DropAttributes)myAlterCommand;

            foreach (var aAttribute in command.ListOfAttributes)
            {
                //result.RemoveUnknownAttribute(aAttribute);
            }
        }

        /// <summary>
        /// Generates an incoming edge attribute
        /// </summary>
        /// <param name="aIncomingEdge">The incoming edge definition by the gql</param>
        /// <returns>An incoming edge predefinition</returns>
        private IncomingEdgePredefinition GenerateAIncomingEdge(IncomingEdgeDefinition aIncomingEdge)
        {
            IncomingEdgePredefinition result = new IncomingEdgePredefinition(aIncomingEdge.AttributeName,
                                                                                aIncomingEdge.TypeName, 
                                                                                aIncomingEdge.TypeAttributeName);

            return result;
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
            UnknownAttributePredefinition result = new UnknownAttributePredefinition(myAttributeDefinition.AttributeName, myAttributeDefinition.AttributeType.Name);

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
