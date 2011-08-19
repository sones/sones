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
    public sealed class AlterEdgeTypeCommandNode : AStructureNode, IAstNodeInit
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
                    case "add":

                        #region add

                        #region data

                        var listOfToBeAddedAttributes = new List<AttributeDefinition>();

                        #endregion

                        #region add attributes

                        foreach (ParseTreeNode aNode in parseNode.ChildNodes[2].ChildNodes)
                        {
                            if (aNode.AstNode is EdgeTypeAttributeDefinitionNode)
                                listOfToBeAddedAttributes.Add(((EdgeTypeAttributeDefinitionNode)aNode.AstNode).AttributeDefinition);
                            else
                                throw new NotImplementedException(aNode.AstNode.GetType().ToString());
                        }

                        AlterTypeCommand = new AlterEdgeType_AddAttributes(listOfToBeAddedAttributes);

                        #endregion

                        #endregion

                        break;

                    case "drop":

                        #region drop

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

                    case "rename":

                        #region rename

                        if (parseNode.ChildNodes.Count > 3)
                            AlterTypeCommand = new AlterType_RenameAttribute() 
                                                    { OldName = parseNode.ChildNodes[2].Token.ValueString, 
                                                      NewName = parseNode.ChildNodes[4].Token.ValueString };
                        else if (parseNode.ChildNodes.Count <= 3)
                            AlterTypeCommand = new AlterType_RenameType() 
                                                    { NewName = parseNode.ChildNodes[2].Token.ValueString };

                        #endregion

                        break;

                    case "comment":

                        #region comment

                        AlterTypeCommand = new AlterType_ChangeComment() 
                                                { NewComment = parseNode.ChildNodes[2].Token.ValueString };

                        #endregion

                        break;

                    case "undefine":

                        #region undefine

                        #region data

                        var listOfUndefAttributes = new List<String>();

                        #endregion

                        #region undefine attributes

                        parseNode.ChildNodes[2].ChildNodes.ForEach(node => listOfUndefAttributes.Add(node.Token.ValueString));

                        AlterTypeCommand = new AlterType_UndefineAttributes(listOfUndefAttributes);

                        #endregion

                        #endregion

                        break;

                    case "define":

                        #region define

                        #region data

                        var listOfDefinedAttributes = new List<AttributeDefinition>();

                        #endregion

                        parseNode
                            .ChildNodes[2]
                            .ChildNodes
                            .ForEach(node => listOfDefinedAttributes.Add(((VertexTypeAttributeDefinitionNode)node.AstNode).AttributeDefinition));

                        AlterTypeCommand = new AlterType_DefineAttributes(listOfDefinedAttributes);
                        
                        #endregion

                        break;
                }
            }
        }

        #endregion
    }
}
