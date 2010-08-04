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

/* <id name="sones GraphDB – Tuple node" />
 * <copyright file="TupleNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of tuple statement.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement.PandoraTypes;
using System.Linq;
using sones.GraphDB.Managers.Structures;


#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of an tuple statement.
    /// </summary>
    public class TupleNode : AStructureNode, IAstNodeInit
    {

        public TupleDefinition TupleDefinition { get; private set; }

        #region constructor

        public TupleNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            DBContext DBcontext = context.IContext as DBContext;

            TupleDefinition = new Managers.Structures.TupleDefinition(GetKindOfTuple(context, parseNode));

            ParseTreeNodeList childNodes;
            if (parseNode.ChildNodes[0].AstNode == null && parseNode.ChildNodes[0].HasChildNodes()) // this is a not resolved node and has childNodes
            {
                childNodes = parseNode.ChildNodes[0].ChildNodes;
            }
            else // single expression tuple - SetRefNode seems MarkTransient the tuple
            {
                childNodes = parseNode.ChildNodes;
            }

            #region get tuple elements

            foreach (ParseTreeNode aExpressionNode in childNodes)
            {

                #region Data

                Type typeOfExpression = null;
                TupleElement aElement = null;

                #endregion

                #region Check the AstNode type and set the typeOfExpression. In case of a tuple just add them.

                if (aExpressionNode.Term != null)
                {
                    if ((aExpressionNode.Term.Name == DBConstants.BracketLeft) || (aExpressionNode.Term.Name == DBConstants.BracketRight))
                    {
                        continue;
                    }
                }

                if (aExpressionNode.AstNode is ExpressionOfAListNode)
                {
                    typeOfExpression = ((ExpressionOfAListNode)aExpressionNode.AstNode).GetType();
                }
                else if (aExpressionNode.AstNode is ExpressionNode)
                {
                    typeOfExpression = ((ExpressionNode)aExpressionNode.AstNode).GetType();
                }
                else if (aExpressionNode.AstNode is BinaryExpressionNode)
                {
                    typeOfExpression = aExpressionNode.Term.GetType();
                }
                else if (aExpressionNode.AstNode is UnaryExpressionNode)
                {
                    typeOfExpression = aExpressionNode.Term.GetType();
                }
                else if (aExpressionNode.AstNode is TupleNode)
                {
                    TupleDefinition = ((TupleNode)aExpressionNode.AstNode).TupleDefinition;
                    continue;
                }
                else if (aExpressionNode.AstNode is PartialSelectStmtNode)
                {
                    typeOfExpression = aExpressionNode.Term.GetType();
                }
                else if (aExpressionNode.AstNode is IDNode)
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
                else
                {
                    typeOfExpression = aExpressionNode.Term.GetType();
                    var val = new ValueDefinition(GraphDBTypeMapper.ConvertPandora2CSharp(typeOfExpression.Name), aExpressionNode.Token.Value);
                    TupleDefinition.AddElement(new TupleElement(GraphDBTypeMapper.ConvertPandora2CSharp(typeOfExpression.Name), val));
                    continue;
                }

                #endregion

                var aTypeOfOperatorResult = GraphDBTypeMapper.ConvertPandora2CSharp(typeOfExpression.Name);

                if (GraphDBTypeMapper.ConvertPandora2CSharp(typeOfExpression.Name) == TypesOfOperatorResult.NotABasicType)
                {
                    #region NotABasicType

                    if (aExpressionNode.AstNode is ExpressionNode)
                    {
                        aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, ((ExpressionNode)aExpressionNode.AstNode).ExpressionDefinition);
                        TupleDefinition.AddElement(aElement);
                    }
                    else
                    {
                        if (aExpressionNode.AstNode is ExpressionOfAListNode)
                        {
                            aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, (((ExpressionOfAListNode)aExpressionNode.AstNode).ExpressionDefinition));

                            if (((ExpressionOfAListNode)aExpressionNode.AstNode).Parameters != null)
                            {
                                aElement.Parameters = ((ExpressionOfAListNode)aExpressionNode.AstNode).Parameters;
                            }
                            TupleDefinition.AddElement(aElement);
                        }
                        else
                        {
                            if (aExpressionNode.AstNode is BinaryExpressionNode)
                            {
                                aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, (aExpressionNode.AstNode as BinaryExpressionNode).BinaryExpressionDefinition);
                                TupleDefinition.AddElement(aElement);
                            }
                            else
                            {
                                if (aExpressionNode.AstNode is UnaryExpressionNode)
                                {
                                    aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, (aExpressionNode.AstNode as UnaryExpressionNode).UnaryExpressionDefinition);
                                    TupleDefinition.AddElement(aElement);
                                }
                                else
                                {
                                    if (aExpressionNode.AstNode is PartialSelectStmtNode)
                                    {
                                        #region partial select

                                        QueryResult qresult = ((PartialSelectStmtNode)aExpressionNode.AstNode).QueryResult;

                                        if (qresult.ResultType != ResultType.Successful)
                                        {
                                            new Exception("Error in inner select stm in tupelSet" + qresult.ToString());
                                        }

                                        foreach (SelectionResultSet aSelResult in qresult.Results)
                                        {
                                            //Hack:
                                            int lowestSelectedLevel = (from aSelectionForReference in aSelResult.SelectedAttributes select aSelectionForReference.Key).Min();

                                            String attrName = aSelResult.SelectedAttributes[lowestSelectedLevel].First().Value;
                                            TypeAttribute curAttr = aSelResult.Type.GetTypeAttributeByName(attrName);

                                            var dbTypeOfAttribute = curAttr.GetDBType(DBcontext.DBTypeManager);

                                            aTypeOfOperatorResult = GraphDBTypeMapper.ConvertPandora2CSharp(dbTypeOfAttribute.Name);

                                            foreach (DBObjectReadout dbo in aSelResult.Objects)
                                            {
                                                if (!(dbo.Attributes.ContainsKey(attrName)))
                                                    continue;

                                                if (curAttr != null)
                                                {
                                                    var val = new ValueDefinition(aTypeOfOperatorResult, dbo.Attributes[attrName]);
                                                    TupleDefinition.AddElement(new TupleElement(aTypeOfOperatorResult, val));
                                                }
                                                else 
                                                {
                                                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                    }
                                }
                            }
                        }
                    }
                    
                    #endregion
                }
                else 
                {
                    #region Basis type

                    if (aExpressionNode.AstNode is ExpressionNode)
                    {
                        aElement = new TupleElement(aTypeOfOperatorResult, ((ExpressionNode)aExpressionNode.AstNode).ExpressionDefinition);
                    }
                    else if (aExpressionNode.AstNode is ExpressionOfAListNode)
                    {
                        aElement = new TupleElement(aTypeOfOperatorResult, ((ExpressionOfAListNode)aExpressionNode.AstNode).ExpressionDefinition);

                        if (((ExpressionOfAListNode)aExpressionNode.AstNode).Parameters != null)
                        {
                            aElement.Parameters = ((ExpressionOfAListNode)aExpressionNode.AstNode).Parameters;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    TupleDefinition.AddElement(aElement);

                    #endregion
                }
            }


            #endregion
        }

        private KindOfTuple GetKindOfTuple(CompilerContext context, ParseTreeNode parseNode)
        {
            var grammar = GetGraphQLGrammar(context);

            var leftSymbol = ExtractBracket(parseNode.FirstChild);
            var rightSymbol = ExtractBracket(parseNode.LastChild);

            KindOfTuple kindOfTuple;

            if (leftSymbol == grammar.S_TUPLE_BRACKET_LEFT && rightSymbol == grammar.S_TUPLE_BRACKET_RIGHT)
            {
                kindOfTuple = KindOfTuple.Inclusive;
            }
            else if (leftSymbol == grammar.S_TUPLE_BRACKET_LEFT_EXCLUSIVE && rightSymbol == grammar.S_TUPLE_BRACKET_RIGHT)
            {
                kindOfTuple = KindOfTuple.LeftExclusive;
            }
            else if (leftSymbol == grammar.S_TUPLE_BRACKET_LEFT && rightSymbol == grammar.S_TUPLE_BRACKET_RIGHT_EXCLUSIVE)
            {
                kindOfTuple = KindOfTuple.RightExclusive;
            }
            else if (leftSymbol == grammar.S_TUPLE_BRACKET_LEFT_EXCLUSIVE && rightSymbol == grammar.S_TUPLE_BRACKET_RIGHT_EXCLUSIVE)
            {
                kindOfTuple = KindOfTuple.Exclusive;
            }
            else
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            return kindOfTuple;
        }

        private SymbolTerminal ExtractBracket(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode.FirstChild != null)
            {
                return parseTreeNode.FirstChild.Token.AsSymbol;
            }
            else
            {
                return parseTreeNode.Token.AsSymbol;
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
