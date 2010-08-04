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


/* <id name="sones GraphDB – Attribute Assign astnode" />
 * <copyright file="AttributeAssignNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of attribute assign statement.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using Lib;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.QueryLanguage.Operators;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of attribute assign statement.
    /// </summary>
    public class AttributeAssignNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public AAttributeAssignOrUpdate AttributeValue { get; private set; }

        private IDChainDefinition _AttributeIDNode = null;
        //private ParseTreeNode _Node;

        #endregion

        #region constructor

        public AttributeAssignNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region get myAttributeName

            _AttributeIDNode = ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition;

            var attributeType = GraphDBTypeMapper.ConvertPandora2CSharp(parseNode.ChildNodes[2].Term.GetType().Name);

            #endregion

            var _Node = parseNode.ChildNodes[2];

            if (_Node.Token != null)
            {
                AttributeValue = new AttributeAssignOrUpdateValue(_AttributeIDNode, attributeType, _Node.Token.Value);
            }
            else if (_Node.AstNode is BinaryExpressionNode)
            {
                #region binary expression

                AttributeValue = new AttributeAssignOrUpdateExpression(_AttributeIDNode, (_Node.AstNode as BinaryExpressionNode).BinaryExpressionDefinition);

                #endregion
            }
            else if (_Node.AstNode is TupleNode)
            {
                #region Tuple

                TupleNode tempTupleNode = (TupleNode)_Node.AstNode;

                if (tempTupleNode.TupleDefinition.Count() == 1)
                {
                    if (tempTupleNode.TupleDefinition.First().Value is BinaryExpressionDefinition)
                    {
                        AttributeValue = new AttributeAssignOrUpdateExpression(_AttributeIDNode, tempTupleNode.TupleDefinition.First().Value as BinaryExpressionDefinition);
                    }
                    else
                    {
                        throw new GraphDBException(new Error_InvalidTuple("Could not extract BinaryExpressionNode from TupleNode."));
                    }
                }
                else
                {
                    throw new GraphDBException(new Error_InvalidTuple("It is not possible to have more than one binary expression in one tuple. Please check brackets."));
                }

                #endregion
            }
            else if (_Node.AstNode is IDNode)
            {
                throw new GraphDBException(new Error_InvalidAttributeValue(_AttributeIDNode.ToString(), (_Node.AstNode as IDNode).ToString()));
            }
            else if (_Node.AstNode is SetRefNode)
            {
                #region setref

                AttributeValue = new AttributeAssignOrUpdateSetRef(_AttributeIDNode, (_Node.AstNode as SetRefNode).SetRefDefinition);

                #endregion
            }
            else if ((_Node.AstNode is CollectionOfDBObjectsNode))
            {
                #region collection like list

                AttributeValue = new AttributeAssignOrUpdateList((_Node.AstNode as CollectionOfDBObjectsNode).CollectionDefinition, _AttributeIDNode, true);

                #endregion
            }
            else
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

        }

        private String GenerateHelperMessage()
        {
            return string.Format(Environment.NewLine + "Try to use REF/REFERENCE (edge) or SETOF/LISTOF (hyperedge).");
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }
}
