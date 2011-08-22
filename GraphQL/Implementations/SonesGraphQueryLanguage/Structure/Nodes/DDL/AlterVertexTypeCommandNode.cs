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
using sones.GraphQL.GQL.Structure.Helper.Definition.AlterType;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class AlterVertexTypeCommandNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public AAlterTypeCommand AlterTypeCommand { get; set; }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                switch (parseNode.ChildNodes[0].Token.Text.ToLower())
                {
                    case "drop":

                        #region drop

                        if (parseNode.ChildNodes[1].AstNode is IndexDropOnAlterType)
                        {
                            var dropNodeExcept = (IndexDropOnAlterType)parseNode.ChildNodes[1].AstNode;

                            AlterTypeCommand = new AlterType_DropIndices(dropNodeExcept.DropIndexList);

                            break;
                        }

                        if (parseNode.ChildNodes.Count == 4 && parseNode.ChildNodes[1].Token.Text.ToLower() == SonesGQLGrammar.TERMINAL_UNIQUE.ToLower())
                        {
                            AlterTypeCommand = new AlterType_DropUnique(parseNode.ChildNodes[3].Token.ValueString);
                            break;
                        }

                        if (parseNode.ChildNodes.Count == 4 && parseNode.ChildNodes[1].Token.Text.ToUpper() == SonesGQLGrammar.TERMINAL_MANDATORY.ToUpper())
                        {
                            AlterTypeCommand = new AlterType_DropMandatory(parseNode.ChildNodes[3].Token.ValueString);
                            break;
                        }

                        #region data

                        List<String> listOfToBeDroppedAttributes = new List<string>();

                        #endregion

                        foreach (ParseTreeNode aNode in parseNode.ChildNodes[2].ChildNodes)
                        {
                            listOfToBeDroppedAttributes.Add(aNode.Token.ValueString);
                        }

                        AlterTypeCommand = new AlterType_DropAttributes(listOfToBeDroppedAttributes);

                        #endregion

                        break;

                    case "add":

                        #region add

                        if (parseNode.ChildNodes[1].AstNode is IndexOnCreateTypeNode)
                        {
                            #region data

                            var _IndexInformation = new List<IndexDefinition>();

                            #endregion

                            #region add indices

                            var indexOnCreateTypeNode = (IndexOnCreateTypeNode)parseNode.ChildNodes[1].AstNode;

                            _IndexInformation.AddRange(indexOnCreateTypeNode.ListOfIndexDefinitions);

                            AlterTypeCommand = new AlterType_AddIndices(_IndexInformation);

                            #endregion
                        }
                        else
                        {
                            #region data

                            var listOfToBeAddedAttributes = new List<AttributeDefinition>();
                            var _BackwardEdgeInformation = new List<IncomingEdgeDefinition>();

                            #endregion

                            #region add attributes

                            foreach (ParseTreeNode aNode in parseNode.ChildNodes[2].ChildNodes)
                            {
                                if (aNode.AstNode is VertexTypeAttributeDefinitionNode)
                                {
                                    listOfToBeAddedAttributes.Add(((VertexTypeAttributeDefinitionNode)aNode.AstNode).AttributeDefinition);
                                }
                                else if (aNode.AstNode is IncomingEdgeNode)
                                {
                                    _BackwardEdgeInformation.Add((aNode.AstNode as IncomingEdgeNode).BackwardEdgeDefinition);
                                }
                                else
                                {
                                    throw new NotImplementedException(aNode.AstNode.GetType().ToString());
                                }
                            }

                            AlterTypeCommand = new AlterVertexType_AddAttributes(listOfToBeAddedAttributes, _BackwardEdgeInformation);

                            #endregion
                        }

                        #endregion

                        break;

                    case "rename":

                        #region rename

                        if (parseNode.ChildNodes.Count > 3)
                        {
                            if (parseNode.ChildNodes[1].Token.Text.ToUpper() == SonesGQLConstants.INCOMINGEDGE)
                            {
                                AlterTypeCommand = new AlterType_RenameIncomingEdge() { OldName = parseNode.ChildNodes[2].Token.ValueString, NewName = parseNode.ChildNodes[4].Token.ValueString };
                            }
                            else
                            {
                                AlterTypeCommand = new AlterType_RenameAttribute() { OldName = parseNode.ChildNodes[2].Token.ValueString, NewName = parseNode.ChildNodes[4].Token.ValueString };
                            }
                        }
                        else if (parseNode.ChildNodes.Count <= 3)
                        {
                            AlterTypeCommand = new AlterType_RenameType() { NewName = parseNode.ChildNodes[2].Token.ValueString };
                        }

                        #endregion

                        break;

                    case "comment":

                        #region comment

                        AlterTypeCommand = new AlterType_ChangeComment() { NewComment = parseNode.ChildNodes[2].Token.ValueString };

                        #endregion

                        break;

                    case "undefine":

                        #region data

                        var listOfUndefAttributes = new List<String>();

                        #endregion

                        #region undefine attributes

                        parseNode.ChildNodes[2].ChildNodes.ForEach(node => listOfUndefAttributes.Add(node.Token.ValueString));

                        AlterTypeCommand = new AlterType_UndefineAttributes(listOfUndefAttributes);

                        #endregion


                        break;

                    case "define":

                        #region data

                        var listOfDefinedAttributes = new List<AttributeDefinition>();

                        #endregion

                        parseNode.ChildNodes[2].ChildNodes.ForEach(node => listOfDefinedAttributes.Add(((VertexTypeAttributeDefinitionNode)node.AstNode).AttributeDefinition));

                        AlterTypeCommand = new AlterType_DefineAttributes(listOfDefinedAttributes);

                        break;
                }
            }
        }

        #endregion
    }
}
