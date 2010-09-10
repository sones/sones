/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="PandoraDB – AlterCmd node" />
 * <copyright file="AlterCmd.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an AlterCmd node.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.DataStructures;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of an AlterCmd node.
    /// </summary>
    class AlterCommandNode : AStructureNode, IAstNodeInit
    {
        #region Data

        TypesOfAlterCmd _TypeOfAlterCmd;
        Object _Value = null;

        /// <summary>
        /// The information about the BackwardEdge: &lt;Type, Attribute, Visible AttributeName&gt;
        /// </summary>
        public List<BackwardEdgeNode> BackwardEdgeInformation
        {
            get { return _BackwardEdgeInformation; }
        }
        private List<BackwardEdgeNode> _BackwardEdgeInformation;

        #endregion

        #region constructor

        public AlterCommandNode()
        {
            _BackwardEdgeInformation = new List<BackwardEdgeNode>();
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {
             
                switch (parseNode.ChildNodes[0].Token.ValueString.ToLower())
                {
                    case "drop":

                        #region drop

                        if (parseNode.ChildNodes.Count == 2 && parseNode.ChildNodes[1].Token.Text.ToLower() == GraphQL.TERMINAL_UNIQUE.ToLower())
                        {
                            _TypeOfAlterCmd = TypesOfAlterCmd.DropUnqiue;
                            break;
                        }

                        if (parseNode.ChildNodes.Count == 2 && parseNode.ChildNodes[1].Token.Text.ToUpper() == GraphQL.TERMINAL_MANDATORY.ToUpper())
                        {
                            _TypeOfAlterCmd = TypesOfAlterCmd.DropMandatory;
                            break;
                        }

                        #region data

                        List<String> listOfToBeDroppedAttributes = new List<string>();

                        #endregion

                        _TypeOfAlterCmd = TypesOfAlterCmd.Drop;

                        foreach (ParseTreeNode aNode in parseNode.ChildNodes[2].ChildNodes)
                        {
                            listOfToBeDroppedAttributes.Add(aNode.Token.ValueString);
                        }

                        _Value = (object)listOfToBeDroppedAttributes;

#endregion

                        break;

                    case "add":

                        #region add

                        #region data

                        List<AttributeDefinitionNode> listOfToBeAddedAttributes = new List<AttributeDefinitionNode>();

                        #endregion

                        _TypeOfAlterCmd = TypesOfAlterCmd.Add;

                        foreach (ParseTreeNode aNode in parseNode.ChildNodes[2].ChildNodes)
                        {
                            if (aNode.AstNode is AttributeDefinitionNode)
                            {
                                listOfToBeAddedAttributes.Add((AttributeDefinitionNode)aNode.AstNode);
                            }
                            else if (aNode.AstNode is BackwardEdgeNode)
                            {
                                _BackwardEdgeInformation.Add((BackwardEdgeNode)aNode.AstNode);
                            }
                            else
                            {
                                throw new NotImplementedException(aNode.AstNode.GetType().ToString());
                            }
                        }

                        _Value = (object)listOfToBeAddedAttributes;

#endregion

                        break;

                    case "rename":

                        #region rename

                        if (parseNode.ChildNodes.Count > 3)
                        {
                            if (parseNode.ChildNodes[1].Token.Text.ToUpper() == "BACKWARDEDGE")
                            {
                                _TypeOfAlterCmd = TypesOfAlterCmd.RenameBackwardedge;
                                _Value = new KeyValuePair<String, String>(parseNode.ChildNodes[2].Token.ValueString, parseNode.ChildNodes[4].Token.ValueString);
                            }
                            else
                            {
                                _TypeOfAlterCmd = TypesOfAlterCmd.RenameAttribute;
                                _Value = new KeyValuePair<String, String>(parseNode.ChildNodes[2].Token.ValueString, parseNode.ChildNodes[4].Token.ValueString);
                            }
                        }
                        else if(parseNode.ChildNodes.Count <= 3)
                        {
                            _TypeOfAlterCmd = TypesOfAlterCmd.RenameType;
                            _Value = (string)parseNode.ChildNodes[2].Token.ValueString;
                        }

                        #endregion
                        break;

                    case "comment":

                        #region comment

                        _TypeOfAlterCmd = TypesOfAlterCmd.ChangeComment;
                        _Value = parseNode.ChildNodes[2].Token.ValueString;

                        #endregion

                        break;
                }
            }
        }

        public TypesOfAlterCmd TypeOfAlterCmd { get { return _TypeOfAlterCmd; } }
        public Object Value { get { return _Value; } }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }
}
