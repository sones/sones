/* <id name="GraphDB – Tuple node" />
 * <copyright file="TupleNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of tuple statement.</summary>
 */

#region Usings

using System;
using System.Linq;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Operators;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an tuple statement.
    /// </summary>
    public class TupleNode : AStructureNode, IAstNodeInit
    {

        public TupleDefinition TupleDefinition { get; private set; }

        #region Constructor

        public TupleNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            TupleDefinition = new Managers.Structures.TupleDefinition(GetKindOfTuple(myCompilerContext, myParseTreeNode));

            ParseTreeNodeList childNodes;
            if (myParseTreeNode.ChildNodes[0].AstNode == null && myParseTreeNode.ChildNodes[0].HasChildNodes()) // this is a not resolved node and has childNodes
            {
                childNodes = myParseTreeNode.ChildNodes[0].ChildNodes;
            }

            else // single expression tuple - SetRefNode seems MarkTransient the tuple
            {
                childNodes = myParseTreeNode.ChildNodes;
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
                    var val = new ValueDefinition(GraphDBTypeMapper.ConvertGraph2CSharp(typeOfExpression.Name), aExpressionNode.Token.Value);
                    TupleDefinition.AddElement(new TupleElement(GraphDBTypeMapper.ConvertGraph2CSharp(typeOfExpression.Name), val));
                    continue;
                }

                #endregion

                var aTypeOfOperatorResult = GraphDBTypeMapper.ConvertGraph2CSharp(typeOfExpression.Name);

                if (GraphDBTypeMapper.ConvertGraph2CSharp(typeOfExpression.Name) == TypesOfOperatorResult.NotABasicType)
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

                                        aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, ((PartialSelectStmtNode)aExpressionNode.AstNode).SelectDefinition);
                                        TupleDefinition.AddElement(aElement);

                                        //if (qresult.ResultType != ResultType.Successful)
                                        //{
                                        //    new Exception("Error in inner select stm in tupelSet" + qresult.ToString());
                                        //}

                                        //foreach (SelectionResultSet aSelResult in qresult.Results)
                                        //{
                                        //    //Hack:
                                        //    int lowestSelectedLevel = (from aSelectionForReference in aSelResult.SelectedAttributes select aSelectionForReference.Key).Min();

                                        //    String attrName = aSelResult.SelectedAttributes[lowestSelectedLevel].First().Value;
                                        //    TypeAttribute curAttr = aSelResult.Type.GetTypeAttributeByName(attrName);

                                        //    var dbTypeOfAttribute = curAttr.GetDBType(DBcontext.DBTypeManager);

                                        //    aTypeOfOperatorResult = GraphDBTypeMapper.ConvertGraph2CSharp(dbTypeOfAttribute.Name);

                                        //    foreach (DBObjectReadout dbo in aSelResult.Objects)
                                        //    {
                                        //        if (!(dbo.Attributes.ContainsKey(attrName)))
                                        //            continue;

                                        //        if (curAttr != null)
                                        //        {
                                        //            var val = new ValueDefinition(aTypeOfOperatorResult, dbo.Attributes[attrName]);
                                        //            TupleDefinition.AddElement(new TupleElement(aTypeOfOperatorResult, val));
                                        //        }
                                        //        else 
                                        //        {
                                        //            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                        //        }
                                        //    }
                                        //}

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
