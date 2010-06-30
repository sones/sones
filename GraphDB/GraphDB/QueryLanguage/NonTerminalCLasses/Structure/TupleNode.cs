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


#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of an tuple statement.
    /// </summary>
    public class TupleNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private List<TupleElement> _Tuple = null;
        public KindOfTuple KindOfTuple { get; private set; }

        #endregion

        #region constructor

        public TupleNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _Tuple = new List<TupleElement>();

            DBContext DBcontext = context.IContext as DBContext;

            GetKindOfTuple(context, parseNode);

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
                    typeOfExpression = ((ExpressionOfAListNode)aExpressionNode.AstNode).ParseTreeNode.Term.GetType();
                }
                else if (aExpressionNode.AstNode is ExpressionNode)
                {
                    typeOfExpression = ((ExpressionNode)aExpressionNode.AstNode).ParseTreeNode.Term.GetType();
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
                    _Tuple.AddRange(((TupleNode)aExpressionNode.AstNode).Tuple);
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
                    _Tuple.Add(new TupleElement(PandoraTypeMapper.ConvertPandora2CSharp(typeOfExpression.Name), aExpressionNode.Token.Value));
                    continue;
                }

                #endregion

                var aTypeOfOperatorResult = PandoraTypeMapper.ConvertPandora2CSharp(typeOfExpression.Name);

                if (PandoraTypeMapper.ConvertPandora2CSharp(typeOfExpression.Name) == TypesOfOperatorResult.NotABasicType)
                {
                    #region NotABasicType

                    if (aExpressionNode.AstNode is ExpressionNode)
                    {
                        aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, ((ExpressionNode)aExpressionNode.AstNode).ParseTreeNode.AstNode);
                        _Tuple.Add(aElement);
                    }
                    else
                    {
                        if (aExpressionNode.AstNode is ExpressionOfAListNode)
                        {
                            aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, ((ExpressionOfAListNode)aExpressionNode.AstNode).ParseTreeNode.AstNode);

                            if (((ExpressionOfAListNode)aExpressionNode.AstNode).ParametersNode != null)
                            {
                                aElement.Parameters = ((ExpressionOfAListNode)aExpressionNode.AstNode).ParametersNode.ParameterValues;
                            }
                            _Tuple.Add(aElement);
                        }
                        else
                        {
                            if (aExpressionNode.AstNode is BinaryExpressionNode)
                            {
                                aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, aExpressionNode.AstNode);
                                _Tuple.Add(aElement);
                            }
                            else
                            {
                                if (aExpressionNode.AstNode is UnaryExpressionNode)
                                {
                                    aElement = new TupleElement(TypesOfOperatorResult.NotABasicType, aExpressionNode.AstNode);
                                    _Tuple.Add(aElement);
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

                                            aTypeOfOperatorResult = PandoraTypeMapper.ConvertPandora2CSharp(dbTypeOfAttribute.Name);

                                            foreach (DBObjectReadout dbo in aSelResult.Objects)
                                            {
                                                if (!(dbo.Attributes.ContainsKey(attrName)))
                                                    continue;

                                                if (curAttr != null)
                                                {
                                                    _Tuple.Add(new TupleElement(aTypeOfOperatorResult, dbo.Attributes[attrName]));
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
                        aElement = new TupleElement(aTypeOfOperatorResult, ((ExpressionNode)aExpressionNode.AstNode).ParseTreeNode.Token.Value);
                    else if (aExpressionNode.AstNode is ExpressionOfAListNode)
                        aElement = new TupleElement(aTypeOfOperatorResult, ((ExpressionOfAListNode)aExpressionNode.AstNode).ParseTreeNode.Token.Value);
                    else
                        throw new NotImplementedException();
                    _Tuple.Add(aElement);

                    #endregion
                }
            }


            #endregion
        }

        private void GetKindOfTuple(CompilerContext context, ParseTreeNode parseNode)
        {
            var grammar = GetGraphQLGrammar(context);

            var leftSymbol = ExtractBracket(parseNode.FirstChild);
            var rightSymbol = ExtractBracket(parseNode.LastChild);

            if (leftSymbol == grammar.S_TUPLE_BRACKET_LEFT && rightSymbol == grammar.S_TUPLE_BRACKET_RIGHT)
            {
                KindOfTuple = KindOfTuple.Inclusive;
            }
            else if (leftSymbol == grammar.S_TUPLE_BRACKET_LEFT_EXCLUSIVE && rightSymbol == grammar.S_TUPLE_BRACKET_RIGHT)
            {
                KindOfTuple = KindOfTuple.LeftExclusive;
            }
            else if (leftSymbol == grammar.S_TUPLE_BRACKET_LEFT && rightSymbol == grammar.S_TUPLE_BRACKET_RIGHT_EXCLUSIVE)
            {
                KindOfTuple = KindOfTuple.RightExclusive;
            }
            else if (leftSymbol == grammar.S_TUPLE_BRACKET_LEFT_EXCLUSIVE && rightSymbol == grammar.S_TUPLE_BRACKET_RIGHT_EXCLUSIVE)
            {
                KindOfTuple = KindOfTuple.Exclusive;
            }
            else
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
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

        public List<TupleElement> Tuple { get { return _Tuple; } }

        public Exceptional<TupleValue> GetAsTupleValue(DBContext dbContext, TypeAttribute attr)
        {
            var graphDBType = attr.GetDBType(dbContext.DBTypeManager);
            var tupleBaseType = PandoraTypeMapper.GetPandoraObjectFromTypeName(graphDBType.Name);
            if (tupleBaseType == null)
            {
                return new Exceptional<TupleValue>(new Error_InvalidTuple("invalid type " + graphDBType.Name));
            }

            var edge = new EdgeTypeListOfBaseObjects();

            foreach (TupleElement aTupleElement in _Tuple)
            {
                if (tupleBaseType.IsValidValue(aTupleElement.Value))
                {
                    edge.Add(tupleBaseType.Clone(aTupleElement.Value), aTupleElement.Parameters.ToArray());
                }
                else
                {
                    if (aTupleElement.Value is BinaryExpressionNode)
                    {
                        if (!((BinaryExpressionNode)aTupleElement.Value).ResultValue.Success)
                        {
                            return new Exceptional<TupleValue>(((BinaryExpressionNode)aTupleElement.Value).ResultValue);
                        }

                        var binExprVal = ((BinaryExpressionNode)aTupleElement.Value).ResultValue.Value;
                        if (binExprVal is AtomValue && tupleBaseType.IsValidValue(((AtomValue)binExprVal).Value))
                        {
                            edge.Add(tupleBaseType.Clone(((AtomValue)binExprVal).Value), aTupleElement.Parameters.ToArray());
                        }
                    }
                    else //if (!(aTupleElement.Value is BinaryExpressionNode))
                    {
                        return new Exceptional<TupleValue>(new Error_DataTypeDoesNotMatch(attr.GetDBType(dbContext.DBTypeManager).Name, aTupleElement.Value.GetType().Name));
                    }
                    

                }
            }

            return new Exceptional<TupleValue>(new TupleValue(PandoraTypeMapper.ConvertPandora2CSharp(graphDBType.Name), edge, graphDBType, KindOfTuple));
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

        internal Exceptional<ASetReferenceEdgeType> GetAsUUIDEdge(DBContext dbContext, GraphDBType graphDBType)
        {
            var edge = new EdgeTypeSetOfReferences();

            foreach (TupleElement aTupleElement in _Tuple)
            {

                if (aTupleElement.Value is BinaryExpressionNode)
                {
                    return new Exceptional<ASetReferenceEdgeType>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
                else
                {
                    //var result = SpecialTypeAttribute_UUID.ConvertToUUID(aTupleElement.Value.ToString(), graphDBType, dbContext.SessionSettings, dbContext.DBTypeManager);
                    //if (result.Failed)
                    //{
                    //    return new Exceptional<ASetReferenceEdgeType>(result);
                    //}
                    //edge.Add(result.Value);

                    edge.Add(ObjectUUID.FromString(aTupleElement.Value.ToString()));

                }

            }

            return new Exceptional<ASetReferenceEdgeType>(edge);
        }

        internal Exceptional<ASingleReferenceEdgeType> GetAsUUIDSingleEdge(DBContext dbContext, GraphDBType graphDBType)
        {
            var edge = new EdgeTypeSingleReference();
            if (_Tuple.Count > 1)
            {
                return new Exceptional<ASingleReferenceEdgeType>(new Error_TooManyElementsForEdge(edge, (UInt64)_Tuple.Count));
            }

            var aTupleElement = _Tuple.First();

            if (aTupleElement.Value is BinaryExpressionNode)
            {
                return new Exceptional<ASingleReferenceEdgeType>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
            else
            {
                edge.Set(ObjectUUID.FromString(aTupleElement.Value.ToString()));
            }

            return new Exceptional<ASingleReferenceEdgeType>(edge);
        }
    }
}
