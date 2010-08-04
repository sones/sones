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

/* <id name="sones GraphDB – ColumnItem node" />
 * <copyright file="ColumnItemNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of ColumnItemNode.</summary>
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
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of an ColumnItem node
    /// </summary>
    public class SelectionListElementNode : AStructureNode, IAstNodeInit
    {

        public Boolean IsAsterisk { get; private set; }
        
        #region Data

        TypesOfColumnSource _TypeOfColumnSource;
        AExpressionDefinition _ColumnSourceValue = null;
        String _AliasId = null;

        #endregion

        #region Accessors

        public TypesOfColumnSource TypeOfColumnSource { get { return _TypeOfColumnSource; } }
        public AExpressionDefinition ColumnSourceValue { get { return _ColumnSourceValue; } }
        public String AliasId { get { return _AliasId; } }

        #endregion

        #region constructor

        public SelectionListElementNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if ((parseNode.ChildNodes[0].Token != null) && (parseNode.ChildNodes[0].Token.Text == DBConstants.ASTERISKSYMBOL))
            {
                IsAsterisk = true;
            }
            else
            {
                IsAsterisk = false;
                object astNode = parseNode.ChildNodes[0].AstNode;

                if (astNode == null)
                {
                    // why the hack are functions without a caller in the child/child?
                    if (parseNode.ChildNodes[0].HasChildNodes() && parseNode.ChildNodes[0].ChildNodes[0].AstNode != null)
                    {
                        astNode = parseNode.ChildNodes[0].ChildNodes[0].AstNode;
                    }
                    else
                    {
                        if (parseNode.ChildNodes[0].HasChildNodes())
                        {
                            throw new ArgumentException("This is not a valid IDNode: " + (parseNode.ChildNodes[0].ChildNodes.Aggregate("", (result, elem) => { result += elem.Token.Text + "."; return result; })));
                        }
                        else
                        {
                            throw new ArgumentException("Found an invalid IDNode");
                        }
                    }
                }


                if (astNode is IDNode)
                {
                    IDNode aIDNode = astNode as IDNode;

                    #region ID

                    _TypeOfColumnSource = TypesOfColumnSource.ID;

                    #region get value

                    _ColumnSourceValue = ((IDNode)aIDNode).IDChainDefinition;

                    #endregion

                    #region Alias handling

                    if (parseNode.ChildNodes.Count > 1)
                    {
                        _AliasId = parseNode.ChildNodes[2].Token.ValueString;
                    }

                    #endregion

                    #endregion
                }
                else if (astNode is AggregateNode)
                {
                    #region aggregate

                    _TypeOfColumnSource = TypesOfColumnSource.Aggregate;

                    AggregateNode aAggregateNode = (AggregateNode)astNode;

                    _ColumnSourceValue = aAggregateNode.AggregateDefinition;

                    #endregion

                    #region Alias handling

                    if (parseNode.ChildNodes.Count.CompareTo(1) > 0)
                    {
                        _AliasId = parseNode.ChildNodes[2].Token.ValueString;
                    }
                    else
                    {
                        _AliasId = aAggregateNode.AggregateDefinition.ChainPartAggregateDefinition.SourceParsedString;
                    }

                    #endregion
                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), parseNode.ChildNodes[0].AstNode.GetType().Name));
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
