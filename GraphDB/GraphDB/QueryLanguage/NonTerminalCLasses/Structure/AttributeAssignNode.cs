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

/* <id name="PandoraDB – Attribute Assign astnode" />
 * <copyright file="AttributeAssignNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
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

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of attribute assign statement.
    /// </summary>
    public class AttributeAssignNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private IDNode _AttributeIDNode= null;
        private Object _AttributeValue = null;
        private TypesOfOperatorResult _AttributeType;

        #endregion

        #region constructor

        public AttributeAssignNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            
            //an undefined attribute doesn't need to validate
            if (!((IDNode)parseNode.ChildNodes[0].AstNode).IsValidated)
                return;
            
            #region get myAttributeName

            _AttributeIDNode = (IDNode)parseNode.ChildNodes[0].AstNode;

            #region checkIDNode

            if (_AttributeIDNode.Level > 1)
            {
                throw new GraphDBException(new Error_InvalidAttribute(_AttributeIDNode.ToString()));
            }

            #endregion

            _AttributeType = GraphDBTypeMapper.ConvertPandora2CSharp(parseNode.ChildNodes[2].Term.GetType().Name);

            #endregion

            #region get Attribute value

            //get DBContext

            var dbContext = context.IContext as DBContext;

            if (dbContext == null)
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            if ((!_AttributeIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined) && (_AttributeIDNode.LastAttribute.KindOfType != KindsOfType.SetOfReferences) && (_AttributeIDNode.LastAttribute.KindOfType != KindsOfType.ListOfNoneReferences) && (_AttributeIDNode.LastAttribute.KindOfType != KindsOfType.SetOfNoneReferences))
            {
                #region simple value
                //simple values are string (quoted or not) and numbers

                if (parseNode.ChildNodes[2].Token != null)
                {
                    _AttributeValue = parseNode.ChildNodes[2].Token.Value;
                }
                else
                {
                    if (parseNode.ChildNodes[2].AstNode is BinaryExpressionNode)
                    {
                        #region binary expression

                        _AttributeType = TypesOfOperatorResult.Expression;
                        _AttributeValue = parseNode.ChildNodes[2].AstNode;

                        #endregion
                    }
                    else
                    {
                        if (parseNode.ChildNodes[2].AstNode is TupleNode)
                        {
                            #region Tuple

                            TupleNode tempTupleNode = (TupleNode)parseNode.ChildNodes[2].AstNode;

                            if (tempTupleNode.Tuple.Count == 1)
                            {
                                if (tempTupleNode.Tuple[0].Value is BinaryExpressionNode)
                                {
                                    _AttributeType = TypesOfOperatorResult.Expression;
                                    _AttributeValue = tempTupleNode.Tuple[0].Value;
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
                        else if (parseNode.ChildNodes[2].AstNode is SetRefNode)
                        {
                            throw new GraphDBException(new Error_InvalidAttributeValue(_AttributeIDNode.LastAttribute.Name, "SetRefNode"));
                        }
                        else
                        {
                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently it is not supported to assign a \"" + parseNode.ChildNodes[2].AstNode.GetType().Name + "\" to an attribute."));
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region complex

                if (parseNode.ChildNodes[2].AstNode is SetRefNode)
                {
                    #region setref

                    _AttributeType = TypesOfOperatorResult.Reference;
                    _AttributeValue = parseNode.ChildNodes[2].AstNode;

                    #endregion
                }
                else
                {
                    if ((parseNode.ChildNodes[2].AstNode is CollectionOfDBObjectsNode))
                    {
                        #region collection like list
                      
                        _AttributeType = TypesOfOperatorResult.SetOfDBObjects;
                        _AttributeValue = parseNode.ChildNodes[2].AstNode;

                        #endregion
                    }
                    else
                    {
                        throw new GraphDBException(new Error_InvalidTuple(String.Format("{0} is not a valid value for attribute {1}.{2}",  parseNode.ChildNodes[2].ToString(), _AttributeIDNode.LastAttribute.Name, GenerateHelperMessage())));
                    }
                }

                #endregion
            }

            #endregion

        }

        private String GenerateHelperMessage()
        {
            return string.Format(Environment.NewLine + "Try to use REF/REFERENCE (edge) or SETOF/LISTOF (hyperedge).");
        }

        public IDNode AttributeIDNode { get { return _AttributeIDNode; } }
        public TypesOfOperatorResult AttributeType { get { return _AttributeType; } }
        public Object AttributeValue { get { return _AttributeValue; } }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }
}
