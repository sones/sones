/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* <id name="sones GraphDB – AlterCmd node" />
 * <copyright file="AlterCmd.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an AlterCmd node.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Managers.AlterType;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Exceptions;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{


    /// <summary>
    /// This node is requested in case of an AlterCmd node.
    /// </summary>
    public class AlterCommandNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public AAlterTypeCommand AlterTypeCommand { get; set; }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {
             
                switch (parseNode.ChildNodes[0].Token.Text.ToLower())
                {
                    case "drop":

                        #region drop

                        if (parseNode.ChildNodes[1].AstNode is Exceptional<IndexDropOnAlterType>)
                        {
                            var dropNodeExcept = (Exceptional<IndexDropOnAlterType>)parseNode.ChildNodes[1].AstNode;

                            if (!dropNodeExcept.Success)
                            {
                                throw new GraphDBException(dropNodeExcept.Errors);
                            }

                            AlterTypeCommand = new AlterType_DropIndices(dropNodeExcept.Value.DropIndexList);

                            break;
                        }
                        
                        if (parseNode.ChildNodes.Count == 2 && parseNode.ChildNodes[1].Token.Text.ToLower() == GraphQL.TERMINAL_UNIQUE.ToLower())
                        {
                            AlterTypeCommand = new AlterType_DropUnique();
                            break;
                        }

                        if (parseNode.ChildNodes.Count == 2 && parseNode.ChildNodes[1].Token.Text.ToUpper() == GraphQL.TERMINAL_MANDATORY.ToUpper())
                        {
                            AlterTypeCommand = new AlterType_DropMandatory();
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

                        if (parseNode.ChildNodes[1].AstNode is Exceptional<IndexOnCreateTypeNode>)
                        {
                            #region data

                            var _IndexInformation = new List<Exceptional<IndexDefinition>>();

                            #endregion

                            #region add indices

                            var idxExceptional = (Exceptional<IndexOnCreateTypeNode>)parseNode.ChildNodes[1].AstNode;

                            if (!idxExceptional.Success)
                            {
                                throw new GraphDBException(idxExceptional.Errors);
                            }

                            _IndexInformation.AddRange(idxExceptional.Value.ListOfIndexDefinitions);

                            AlterTypeCommand = new AlterType_AddIndices(_IndexInformation);

                            #endregion
                        }
                        else
                        {
                            #region data

                            var listOfToBeAddedAttributes = new List<AttributeDefinition>();
                            var _BackwardEdgeInformation  = new List<BackwardEdgeDefinition>();

                            #endregion

                            #region add attributes

                            foreach (ParseTreeNode aNode in parseNode.ChildNodes[2].ChildNodes)
                            {
                                if (aNode.AstNode is AttributeDefinitionNode)
                                {
                                    listOfToBeAddedAttributes.Add(((AttributeDefinitionNode)aNode.AstNode).AttributeDefinition);
                                }
                                else if (aNode.AstNode is BackwardEdgeNode)
                                {
                                    _BackwardEdgeInformation.Add((aNode.AstNode as BackwardEdgeNode).BackwardEdgeDefinition);
                                }
                                else
                                {
                                    throw new NotImplementedException(aNode.AstNode.GetType().ToString());
                                }
                            }

                            AlterTypeCommand = new AlterType_AddAttributes(listOfToBeAddedAttributes, _BackwardEdgeInformation);

                            #endregion
                        }                       

                        #endregion

                        break;

                    case "rename":

                        #region rename

                        if (parseNode.ChildNodes.Count > 3)
                        {
                            if (parseNode.ChildNodes[1].Token.Text.ToUpper() == "BACKWARDEDGE")
                            {
                                AlterTypeCommand = new AlterType_RenameBackwardedge() { OldName = parseNode.ChildNodes[2].Token.ValueString, NewName = parseNode.ChildNodes[4].Token.ValueString };
                            }
                            else
                            {
                                AlterTypeCommand = new AlterType_RenameAttribute() { OldName = parseNode.ChildNodes[2].Token.ValueString, NewName = parseNode.ChildNodes[4].Token.ValueString };
                            }
                        }
                        else if(parseNode.ChildNodes.Count <= 3)
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

                        parseNode.ChildNodes[2].ChildNodes.ForEach(node => listOfDefinedAttributes.Add(((AttributeDefinitionNode)node.AstNode).AttributeDefinition));

                        AlterTypeCommand = new AlterType_DefineAttributes(listOfDefinedAttributes);

                        break;
                }
            }
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }
}
