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


/* <id name="sones GraphDB – AttrUpdateOrAssignListNode Node" />
 * <copyright file="AttrUpdateOrAssignListNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an AttrUpdateOrAssignListNode Node.</summary>
 */

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.Frameworks.Irony.Scripting.Runtime;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures.EdgeTypes;

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// 
    /// </summary>
    public class AttrUpdateOrAssignListNode : AStructureNode, IAstNodeInit
    {
        #region data

        private HashSet<AttributeUpdateOrAssign> _listOfUpdates = new HashSet<AttributeUpdateOrAssign>();
        private HashSet<AttributeUpdateOrAssign> _UndefinedAttributes = new HashSet<AttributeUpdateOrAssign>();

        #endregion

        #region constructor

        public AttrUpdateOrAssignListNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            foreach (ParseTreeNode aChild in parseNode.ChildNodes)
            {
                if (aChild.AstNode is AttributeAssignNode)
                {
                    #region attribute assign

                    AttributeAssignNode aAttributeAssignNode = (AttributeAssignNode)aChild.AstNode;
                    if(aAttributeAssignNode.AttributeIDNode == null)
                    {
                        if (aChild.HasChildNodes())
                        {
                            if (aChild.ChildNodes[0].HasChildNodes())
                            {
                                ParseTreeNode UndefNode = aChild;

                                var UndefName = UndefNode.ChildNodes[0].ChildNodes[0].Token.ValueString;

                                if (UndefNode.ChildNodes[2].AstNode is SetRefNode)
                                {
                                    throw new GraphDBException(new Error_InvalidReferenceAssignmentOfUndefAttr());
                                }

                                if (UndefNode.ChildNodes[2].AstNode is CollectionOfDBObjectsNode)
                                {
                                    CollectionOfDBObjectsNode colNode = (CollectionOfDBObjectsNode)UndefNode.ChildNodes[2].AstNode;
                                    EdgeTypeListOfBaseObjects valueList = new EdgeTypeListOfBaseObjects();

                                    try
                                    {
                                        foreach (var tuple in colNode.TupleNodeElement.Tuple)
                                        {
                                            if (tuple.TypeOfValue == TypesOfOperatorResult.Unknown)
                                                valueList.Add(GraphDBTypeMapper.GetBaseObjectFromCSharpType(tuple.Value));
                                        }

                                        if (colNode.CollectionType == CollectionType.Set)
                                            valueList.UnionWith(valueList);

                                        var keyValPair = new KeyValuePair<String, AObject>(UndefName, valueList);
                                        _listOfUpdates.Add(new AttributeUpdateOrAssign(TypesOfUpdate.AssignAttribute, keyValPair) { IsUndefinedAttribute = true });

                                    }
                                    catch (GraphDBException e)
                                    {
                                        throw e;
                                    }
                                }
                                else
                                {
                                    var typeOfExpression = UndefNode.ChildNodes[2].Term.GetType();
                                    var UndefValue = GraphDBTypeMapper.GetPandoraObjectFromType(GraphDBTypeMapper.ConvertPandora2CSharp(typeOfExpression.Name), UndefNode.ChildNodes[2].Token.Value);
                                    var keyValPair = new KeyValuePair<String, AObject>(UndefName, UndefValue);
                                    _listOfUpdates.Add(new AttributeUpdateOrAssign(TypesOfUpdate.AssignAttribute, keyValPair) { IsUndefinedAttribute = true });
                                }
                            }
                        }
                    }
                    else
                        _listOfUpdates.Add(new AttributeUpdateOrAssign(TypesOfUpdate.AssignAttribute, aChild.AstNode));

                    #endregion
                }
                else
                {
                    if ((aChild.AstNode is AddToListAttrUpdateNode) || (aChild.AstNode is RemoveFromListAttrUpdateNode))
                    {   
                        #region list update

                        if (aChild.AstNode is AddToListAttrUpdateNode)
                        {
                            if (((AddToListAttrUpdateNode)aChild.AstNode).Attribute == null)
                            {
                                #region undefined attributes
                                
                                _listOfUpdates.Add(new AttributeUpdateOrAssign(TypesOfUpdate.UpdateListAttribute, aChild.AstNode) { IsUndefinedAttribute = true });

                                #endregion
                            }
                            else
                            {
                                _listOfUpdates.Add(new AttributeUpdateOrAssign(TypesOfUpdate.UpdateListAttribute, aChild.AstNode));
                            }
                        }
                        #endregion
                                                
                        if (aChild.AstNode is RemoveFromListAttrUpdateNode)
                        {
                            #region list remove

                            if (((RemoveFromListAttrUpdateNode)aChild.AstNode).Attribute == null)
                            {
                                #region undefined attributes

                                _listOfUpdates.Add(new AttributeUpdateOrAssign(TypesOfUpdate.UpdateListAttribute, aChild.AstNode) { IsUndefinedAttribute = true });

                                #endregion
                            }
                            else
                            {
                                _listOfUpdates.Add(new AttributeUpdateOrAssign(TypesOfUpdate.UpdateListAttribute, aChild.AstNode));
                            }

                            #endregion                        
                        }
                    }
                    else
                    {
                        if (aChild.AstNode is AttrRemoveNode)
                        {
                            #region remove attribute

                            _listOfUpdates.Add(new AttributeUpdateOrAssign(TypesOfUpdate.RemoveAttribute, aChild.AstNode));

                            #endregion
                        }
                        else
                        {
                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Invalid task node \"" + aChild.AstNode.GetType().Name + "\" in update statement"));
                        }
                    }
                }
            }
        }

        #region accessors

        public HashSet<AttributeUpdateOrAssign> ListOfUpdate { get { return _listOfUpdates; } }

        #endregion


        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }//class
}//namespace
