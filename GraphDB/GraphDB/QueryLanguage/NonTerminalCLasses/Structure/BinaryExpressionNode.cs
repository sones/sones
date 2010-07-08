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


/* <id name="sones GraphDB – BinaryExpressionNode" />
 * <copyright file="BinaryExpressionNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary>This node is requested in case of expression statement.</summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Operator;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Session;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryExpressionNode : AStructureNode
    {
        #region properties

        private TypesOfBinaryExpression _typeOfBinaryExpression;
        private Exceptional<IOperationValue> _resultValue = null;
        private Object _left = null;
        private Object _right = null;
        private ABinaryOperator _operator = null;
        private String OriginalString = String.Empty;
        private List<IDNode> _ContainingIDNodes;
        
        #endregion

        #region constructor

        public BinaryExpressionNode()
        {

        }

        #endregion

        #region public methods

        /// <summary>
        /// Handles the Binary Expression node
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parseNode"></param>
        /// <param name="typeManager"></param>
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;

            #region Data

            Object leftTemp = null;
            Object rightTemp = null;
            _operator = dbContext.DBPluginManager.GetBinaryOperator(parseNode.ChildNodes[1].Term.Name);

            #endregion

            #region set type of binary expression

            if ((parseNode.ChildNodes[0].Token != null) && (parseNode.ChildNodes[2].Token != null))
            {
                #region two token

                if (parseNode.ChildNodes[0].Token.Category.Equals(TokenCategory.Literal) && parseNode.ChildNodes[2].Token.Category.Equals(TokenCategory.Literal))
                {
                    if ((parseNode.ChildNodes[0].Term.GetType().Equals(typeof(NumberLiteral)) && parseNode.ChildNodes[2].Term.GetType().Equals(typeof(StringLiteral)))
                     || (parseNode.ChildNodes[2].Term.GetType().Equals(typeof(NumberLiteral)) && parseNode.ChildNodes[0].Term.GetType().Equals(typeof(StringLiteral))))
                    {
                        throw new GraphDBException(new Error_DataTypeDoesNotMatch("NumberLiteral", "StringLiteral"));
                    }

                    #region atom

                    _left = _operator.GetAtomValue(parseNode.ChildNodes[0].Token);
                    _right = _operator.GetAtomValue(parseNode.ChildNodes[2].Token);

                    #endregion

                    _typeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                }
                else
                {
                    throw new NotImplementedException();
                }

                #endregion
            }
            else
            {
                if (parseNode.ChildNodes[0].Term is NonTerminal)
                {
                    #region left is NonTerminal

                    #region left

                    if (parseNode.ChildNodes[0].AstNode is IDNode)
                    {
                        #region IDNode

                        var tempIDNode = parseNode.ChildNodes[0].AstNode as IDNode;

                        if (tempIDNode.IsValidated == false)
                        {
                            if (tempIDNode.LastError != null)
                            {
                                throw new GraphDBException(((IDNode)parseNode.ChildNodes[0].AstNode).LastError);
                            }
                        }

                        _left = tempIDNode;
                        _typeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;


                        #endregion
                    }
                    else
                    {
                        if (parseNode.ChildNodes[0].AstNode is TupleNode)
                        {
                            #region TupleNode

                            var tempTupleNode = parseNode.ChildNodes[0].AstNode as TupleNode;

                            if (IsEncapsulatedBinaryExpression(tempTupleNode))
                            {
                                //_left = tempTupleNode.Tuple[0].Value;
                                _left = TryGetBinexpressionNode(tempTupleNode.Tuple[0].Value, dbContext);
                                _typeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                            }
                            else
                            {
                                _left = AssignCorrectTuple(tempTupleNode, _operator, dbContext);
                                _typeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                            }

                            #endregion
                        }
                        else
                        {
                            if (parseNode.ChildNodes[0].AstNode is FuncCallNode)
                            {
                                #region FuncCall

                                _left = parseNode.ChildNodes[0].AstNode;
                                _typeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;

                                #endregion
                            }
                            else
                            {
                                #region try binexpr

                                _left = TryGetBinexpressionNode(parseNode.ChildNodes[0], dbContext);
                                _typeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;

                                #endregion
                            }
                        }
                    }

                    #endregion

                    #region right

                    if (parseNode.ChildNodes[2].Term is NonTerminal)
                    {
                        #region right is a Nonterminal too

                        if (parseNode.ChildNodes[2].AstNode is IDNode)
                        {
                            #region IDNode

                            var tempIDNode = parseNode.ChildNodes[2].AstNode as IDNode;

                            if ((tempIDNode).IsValidated == false)
                            {
                                if (tempIDNode.LastError != null)
                                    throw new GraphDBException(((IDNode)parseNode.ChildNodes[2].AstNode).LastError);
                            }

                            _right = tempIDNode;
                            if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                            {
                                _typeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                            }
                            else
                            {
                                _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                            }


                            #endregion
                        }
                        else
                        {
                            if (parseNode.ChildNodes[2].AstNode is TupleNode)
                            {
                                #region TupleNode

                                var tempTupleNode = parseNode.ChildNodes[2].AstNode as TupleNode;

                                if (IsEncapsulatedBinaryExpression(tempTupleNode))
                                {
                                    //_right = tempTupleNode.Tuple[0].Value;
                                    _right = TryGetBinexpressionNode(parseNode.ChildNodes[2], dbContext);
                                    if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                                    {
                                        _typeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                                    }
                                    else
                                    {
                                        _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                                    }
                                }
                                else
                                {
                                    _right = AssignCorrectTuple(tempTupleNode, _operator, dbContext);
                                }

                                #endregion

                            }
                            else
                            {
                                if (parseNode.ChildNodes[2].AstNode is FuncCallNode)
                                {
                                    #region FuncCall

                                    _right = parseNode.ChildNodes[2].AstNode;
                                    if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                                    {
                                        _typeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                                    }
                                    else
                                    {
                                        _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region try binexpr

                                    _right = TryGetBinexpressionNode(parseNode.ChildNodes[2], dbContext);
                                    if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                                    {
                                        _typeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                                    }
                                    else
                                    {
                                        _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region right is atom

                        _right = _operator.GetAtomValue(parseNode.ChildNodes[2].Token);

                        #endregion
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    #region left is no nonTerminal

                    #region left

                    _left = _operator.GetAtomValue(parseNode.ChildNodes[0].Token);
                    _typeOfBinaryExpression = TypesOfBinaryExpression.Atom;

                    #endregion

                    #region right

                    if (parseNode.ChildNodes[2].Term is NonTerminal)
                    {
                        #region right is a Nonterminal too

                        if (parseNode.ChildNodes[2].AstNode is IDNode)
                        {
                            #region IDNode

                            var tempIDNode = parseNode.ChildNodes[2].AstNode as IDNode;

                            if ((tempIDNode).IsValidated == false)
                            {
                                if (tempIDNode.LastError != null)
                                    throw new GraphDBException(((IDNode)parseNode.ChildNodes[2].AstNode).LastError);
                            }

                            _right = tempIDNode;
                            _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;


                            #endregion
                        }
                        else
                        {
                            if (parseNode.ChildNodes[2].AstNode is TupleNode)
                            {
                                #region TupleNode

                                var tempTupleNode = parseNode.ChildNodes[2].AstNode as TupleNode;

                                if (IsEncapsulatedBinaryExpression(tempTupleNode))
                                {
                                    //_right = TryGetBinexpressionNode( tempTupleNode.Tuple[0].Value;
                                    _right = TryGetBinexpressionNode(parseNode.ChildNodes[2], dbContext);
                                    _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                                }
                                else
                                {
                                    _right = AssignCorrectTuple(tempTupleNode, _operator, dbContext);
                                    _typeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                                }

                                #endregion
                            }
                            else
                            {
                                if (parseNode.ChildNodes[2].AstNode is FuncCallNode)
                                {
                                    #region FuncCall

                                    _right = parseNode.ChildNodes[2].AstNode;
                                    _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;

                                    #endregion
                                }
                                else
                                {
                                    #region try binexpr

                                    _right = TryGetBinexpressionNode(parseNode.ChildNodes[2], dbContext);
                                    _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region right is atom

                        _right = _operator.GetAtomValue(parseNode.ChildNodes[2].Token);

                        #endregion
                    }

                    #endregion

                    #endregion
                }
            }

            #endregion

            #region try to get values from complex expr

            switch (_typeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.Atom:

                    break;

                case TypesOfBinaryExpression.LeftComplex:

                    #region leftComplex

                    leftTemp = TryGetOperationValue(_left);

                    if (leftTemp != null)
                    {
                        _typeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _left = leftTemp;
                    }
                    else
                    {
                        if (_left == null)
                        {
                            _left = parseNode.ChildNodes[0].AstNode;
                        }
                    }

                    #endregion

                    break;

                case TypesOfBinaryExpression.RightComplex:

                    #region rightComplex

                    rightTemp = TryGetOperationValue(_right);

                    if (rightTemp != null)
                    {
                        _typeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _right = rightTemp;
                    }
                    else
                    {
                        if (_right == null)
                        {
                            _right = parseNode.ChildNodes[2].AstNode;
                        }
                    }

                    #endregion

                    break;

                case TypesOfBinaryExpression.Complex:

                    #region complex

                    leftTemp = TryGetOperationValue(_left);
                    rightTemp = TryGetOperationValue(_right);

                    if ((leftTemp != null) && (rightTemp != null))
                    {
                        _typeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _left = leftTemp;
                        _right = rightTemp;
                    }
                    else
                    {
                        if (leftTemp != null)
                        {
                            _typeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                            _left = leftTemp;

                            if (_right == null)
                            {
                                _right = parseNode.ChildNodes[2].AstNode;
                            }
                        }
                        else
                        {
                            if (rightTemp == null)
                            {
                                if (_left == null)
                                {
                                    _left = parseNode.ChildNodes[0].AstNode;
                                }

                                if (_right == null)
                                {
                                    _right = parseNode.ChildNodes[2].AstNode;
                                }

                            }
                            else
                            {
                                _typeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                                _right = rightTemp;

                                if (_left == null)
                                {
                                    _left = parseNode.ChildNodes[0].AstNode;
                                }
                            }
                        }
                    }

                    #endregion

                    break;
            }

            #endregion

            #region process values

            if (_typeOfBinaryExpression.Equals(TypesOfBinaryExpression.Atom))
            {
                #region atomic values
                if ((_left is IOperationValue) && (_right is IOperationValue))
                {
                    _resultValue = _operator.SimpleOperation((IOperationValue)_left, (IOperationValue)_right, _typeOfBinaryExpression);
                }

                #endregion
            }
            else
            {
                #region some kind of complex values

                if (IsCombinableAble(_left, _right))
                {
                    #region summarize expressions
                    //sth like (U.Age + 1) + 1 --> U.Age + 2

                    SimpleCombination(ref _left, ref _right);

                    #endregion
                }
                else
                {
                    #region tweak expressions

                    while (IsTweakAble(_left, _right))
                    {
                        SimpleExpressionTweak(ref _left, ref _right, dbContext);
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            OriginalString += parseNode.ChildNodes[0].ToString() + " ";
            OriginalString += parseNode.ChildNodes[1].Term.Name + " ";
            OriginalString += parseNode.ChildNodes[2].ToString();
            
        }

        private object AssignCorrectTuple(TupleNode tupleNode, ABinaryOperator myOperator, DBContext aContext)
        {
            return myOperator.GetValidTupleReloaded(tupleNode, aContext);
        }

        private bool IsEncapsulatedBinaryExpression(TupleNode tempTupleNode)
        {
            if (tempTupleNode.Tuple.Count == 1)
            {
                if ((tempTupleNode.Tuple[0].Value is BinaryExpressionNode) || (tempTupleNode.Tuple[0].Value is UnaryExpressionNode))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        ///// <summary>
        ///// The method that executes the calculation of a binary expression
        ///// </summary>
        ///// <param name="BinExpr">The BinaryExpression that will be executed.</param>
        ///// <param name="currentTypeDefinition">KeyValuePair of Reference and corresponding PandoraType.</param>
        ///// <param name="referenceList">List of References.</param>
        ///// <param name="typeManager">The TypeManager of the PandoraDB.</param>
        ///// <returns>A PandoraResulat that contains the evaluation result for each reference and level.</returns>
        //private Exceptional<IExpressionGraph> DoItReloaded(BinaryExpressionNode BinExpr, TypeManager typeManager, IDBObjectCache dbObjectCache, IExpressionGraph resultGraph, SessionInfos mySessionToken)
        //{
        //    return BinExpr.Calculon(typeManager, dbObjectCache, resultGraph, mySessionToken);
        //}
        
        /// <summary>
        /// This method evaluates binary expressions.
        /// </summary>
        /// <param name="currentTypeDefinition">KeyValuePair of Reference and corresponding PandoraType.</param>
        /// <param name="referenceList">List of References.</param>
        /// <param name="dbContext">The TypeManager of the PandoraDB.</param>
        /// <param name="queryCache">The current query cache.</param>
        /// <param name="resultGraph">A template of the result graph</param>
        /// <returns>A PandoraResult container.</returns>
        public Exceptional<IExpressionGraph> Calculon(DBContext dbContext, IExpressionGraph resultGraph, bool aggregateAllowed = true)
        {
            //a leaf expression is a expression without any recursive BinaryExpression
            if (IsLeafExpression())
            {
                #region process leaf expression

                return this.Operator.TypeOperation(
                    this.Left, this.Right, dbContext, 
                    this.TypeOfBinaryExpression,
                    GetAssociativityReloaded(ExtractIDNode(this.Left), ExtractIDNode(this.Right)),
                   resultGraph,
                   aggregateAllowed);

                #endregion
            }
            else
            {
                #region process sub expr

                switch (this.TypeOfBinaryExpression)
                {
                    case TypesOfBinaryExpression.LeftComplex:

                        #region left complex

                        if (this.Left is BinaryExpressionNode)
                        {
                            return ((BinaryExpressionNode)this.Left).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                        }
                        else
                        {
                            return new Exceptional<IExpressionGraph>(new Error_InvalidBinaryExpression(this));                            
                        }

                        #endregion

                    case TypesOfBinaryExpression.RightComplex:

                        #region right complex

                        if (this.Right is BinaryExpressionNode)
                        {
                            return ((BinaryExpressionNode)this.Right).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                        }
                        else
                        {
                            return new Exceptional<IExpressionGraph>(new Error_InvalidBinaryExpression(this));
                        }

                        #endregion

                    case TypesOfBinaryExpression.Complex:

                        #region complex

                        var runMT = DBConstants.RunMT;

                        if (!((this.Left is BinaryExpressionNode) && (this.Right is BinaryExpressionNode)))
                        {
                            return new Exceptional<IExpressionGraph>(new Error_InvalidBinaryExpression(this));
                        }

                        if (runMT)
                        {
                            var leftTask = Task<Exceptional<IExpressionGraph>>.Factory.StartNew(() =>
                                {
                                    return ((BinaryExpressionNode)this.Left).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                                });

                            var rightTask = Task<Exceptional<IExpressionGraph>>.Factory.StartNew(() =>
                                {
                                    return ((BinaryExpressionNode)this.Right).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                                });

                            if (!leftTask.Result.Success)
                            {
                                return new Exceptional<IExpressionGraph>(leftTask.Result);
                            }

                            if (!rightTask.Result.Success)
                            {
                                return new Exceptional<IExpressionGraph>(rightTask.Result);
                            }

                            return this.Operator.TypeOperation(
                                leftTask.Result.Value,
                                rightTask.Result.Value,
                                dbContext,
                                this.TypeOfBinaryExpression, TypesOfAssociativity.Neutral, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                            //*/

                        }
                        else
                        {
                            var left = ((BinaryExpressionNode)this.Left).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                            var right = ((BinaryExpressionNode)this.Right).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);

                             if (!left.Success)
                            {
                                return new Exceptional<IExpressionGraph>(left);
                            }

                            if (!right.Success)
                            {
                                return new Exceptional<IExpressionGraph>(right);
                            }

                            ///* Synchronious call
                            return this.Operator.TypeOperation(
                                left.Value,
                                right.Value,
                                dbContext,
                                this.TypeOfBinaryExpression, TypesOfAssociativity.Neutral, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                            //*/
                        }

                        #endregion

                    default:

                        throw new ArgumentException();
                }

                #endregion
            }
        }

        /// <summary>
        /// Extracts an IDNode out of an object.
        /// </summary>
        /// <param name="p">A potential IDNode</param>
        /// <returns>An IDNode.</returns>
        private IDNode ExtractIDNode(object p)
        {
            if (p != null)
            {
                if (p is IDNode)
                {
                    return (IDNode)p;
                }
                else
                {
                    if (p is FuncCallNode)
                    {
                        return ExtractIDNode(((FuncCallNode)p).Expressions.FirstOrDefault());
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return OriginalString;
        }

        public Exceptional<bool> IsSatisfyHaving(DBObjectReadoutGroup myDBObjectReadoutGroup, DBContext dbContext)
        {

            if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                String attributeName = null;
                AtomValue leftValue = null;

                if (!EvaluateHaving(myDBObjectReadoutGroup, _left, out attributeName, out leftValue, dbContext).Value)
                    return new Exceptional<bool>(false);

                var resultValue = (AtomValue)_operator.SimpleOperation(leftValue, (AtomValue)_right, _typeOfBinaryExpression).Value;
                return new Exceptional<bool>((resultValue.Value as DBBoolean).GetValue());

            }

            else if (_typeOfBinaryExpression == TypesOfBinaryExpression.RightComplex)
            {
                String attributeName = null;
                AtomValue rightValue = null;

                if (!EvaluateHaving(myDBObjectReadoutGroup, _right, out attributeName, out rightValue, dbContext).Value)
                    return new Exceptional<bool>(false);

                var resultValue = (AtomValue)_operator.SimpleOperation((AtomValue)_left, rightValue, _typeOfBinaryExpression).Value;
                return new Exceptional<bool>((resultValue.Value as DBBoolean).GetValue());
                
            }

            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

        }

        private Exceptional<Boolean> EvaluateHaving(DBObjectReadoutGroup myDBObjectReadoutGroup, Object complexValue, out String attributeName, out AtomValue simpleValue, DBContext dbContext)
        {

            GraphDBType graphDBType = null;
            attributeName = null;
            simpleValue = null;

            if (complexValue is IDNode)
            {
                if (((IDNode)complexValue).LastAttribute == null)
                {
                    if (((IDNode)complexValue).IDNodeParts.Last() is IDNodeFunc)
                    {
                        var func = (((IDNode)complexValue).IDNodeParts.Last() as IDNodeFunc).FuncCallNode;
                        if (func.Expressions.Count != 1)
                            return new Exceptional<Boolean>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                        attributeName = func.Alias;
                        graphDBType = func.ContainingIDNodes.First().LastAttribute.GetDBType(dbContext.DBTypeManager);
                    }
                    else
                    {
                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                }
                else
                {
                    attributeName = ((IDNode)complexValue).LastAttribute.Name;
                    graphDBType = ((IDNode)complexValue).LastAttribute.GetDBType(dbContext.DBTypeManager);
                }
            }
            else
            {
                if (complexValue is AggregateNode)
                {
                    var func = complexValue as FuncCallNode;
                    if (func.Expressions.Count != 1)
                        return new Exceptional<Boolean>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                    attributeName = func.Alias;
                    graphDBType = func.ContainingIDNodes.First().LastAttribute.GetDBType(dbContext.DBTypeManager);
                }
                else
                {
                    return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
            }

            if (myDBObjectReadoutGroup.Attributes.ContainsKey(attributeName))
            {
                ADBBaseObject objectValue = GraphDBTypeMapper.GetPandoraObjectFromTypeName(graphDBType.Name, myDBObjectReadoutGroup.Attributes[attributeName]);
                simpleValue = new AtomValue(objectValue);
                return new Exceptional<bool>(true);
            }

            return new Exceptional<bool>(false);

        }

        public List<IDNode> ContainingIDNodes
        {
            get
            {
                if (_ContainingIDNodes == null)
                {
                    _ContainingIDNodes = new List<IDNode>();
                    calculateIDNodes(ref _ContainingIDNodes, _left, _right);
                }

                return _ContainingIDNodes;
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Returns if the current BinaryExpressionNode is a leaf-expression without any sub-expressions.
        /// </summary>
        /// <returns>True if it is a leaf expression, otherwise false.</returns>
        public bool IsLeafExpression()
        {
            if ((this.Left is BinaryExpressionNode) || (this.Right is BinaryExpressionNode))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private Boolean isTweakAbleOperator(BinaryOperator myBinaryOperatorType)
        {
            return (myBinaryOperatorType == BinaryOperator.Equal 
                || myBinaryOperatorType == BinaryOperator.Inequal
                || myBinaryOperatorType == BinaryOperator.NotEqual
                || myBinaryOperatorType == BinaryOperator.LessThan
                || myBinaryOperatorType == BinaryOperator.LessEquals
                || myBinaryOperatorType == BinaryOperator.GreaterThan
                || myBinaryOperatorType == BinaryOperator.GreaterEquals
                );
        }

        private bool IsTweakAble(object _left, object _right)
        {
            if (_typeOfBinaryExpression == TypesOfBinaryExpression.Complex)
            {
                return false;
            }

            if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                #region left complex

                if (_left is BinaryExpressionNode)
                {
                    #region data

                    BinaryExpressionNode leftNode = (BinaryExpressionNode)_left;

                    #endregion

                    if (leftNode.TypeOfBinaryExpression != TypesOfBinaryExpression.Complex)
                    {
                        if ((leftNode.Operator.Label == BinaryOperator.Addition) || (leftNode.Operator.Label == BinaryOperator.Subtraction) || (leftNode.Operator.Label == BinaryOperator.Multiplication) || (leftNode.Operator.Label == BinaryOperator.Division))
                        {
                            if (isTweakAbleOperator(_operator.Label))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region right complex

                if (_right is BinaryExpressionNode)
                {
                    #region data

                    BinaryExpressionNode rightNode = (BinaryExpressionNode)_right;

                    #endregion

                    if (rightNode.TypeOfBinaryExpression != TypesOfBinaryExpression.Complex)
                    {
                        if ((rightNode.Operator.Label == BinaryOperator.Addition) || (rightNode.Operator.Label == BinaryOperator.Subtraction) || (rightNode.Operator.Label == BinaryOperator.Multiplication) || (rightNode.Operator.Label == BinaryOperator.Division))
                        {
                            if ((_operator.Label == BinaryOperator.Equal) || (_operator.Label == BinaryOperator.Inequal))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                #endregion
            }
        }

        private void SimpleExpressionTweak(ref object _left, ref object _right, DBContext typeManager)
        {
            if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                #region left is complex

                #region data

                BinaryExpressionNode leftNode = (BinaryExpressionNode)_left;

                #endregion

                #region get contrary operator for complex part

                ABinaryOperator contraryOperator = typeManager.DBPluginManager.GetBinaryOperator(leftNode.Operator.ContraryOperationSymbol);

                #endregion

                #region get values

                IOperationValue _leftAtom = null;
                object _leftComplex = null;

                if (leftNode.TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    _leftAtom = (leftNode.Right as IOperationValue);
                    _leftComplex = leftNode.Left;
                }
                else
                {
                    _leftAtom = (leftNode.Left as IOperationValue);
                    _leftComplex = leftNode.Right;
                }

                #endregion

                #region get new right value for current node

                IOperationValue _rightTempResult = (IOperationValue)contraryOperator.SimpleOperation((IOperationValue)_right, _leftAtom, _typeOfBinaryExpression).Value;
                _right = _rightTempResult;

                #endregion

                #region get the new left value

                _left = _leftComplex;

                #endregion

                #region update operator in case of an inequality

                if (_operator.Label == BinaryOperator.Inequal)
                {
                    if (leftNode.Operator.Label == BinaryOperator.Division)
                    {
                        if (Convert.ToDouble(_leftAtom).CompareTo(0) < 0)
                        {
                            _operator = typeManager.DBPluginManager.GetBinaryOperator(_operator.ContraryOperationSymbol);
                        }
                    }
                }

                #endregion

                #endregion
            }
            else
            {
                #region right is complex

                #region data

                BinaryExpressionNode rightNode = (BinaryExpressionNode)_right;

                #endregion

                #region get contrary operator for complex part

                ABinaryOperator contraryOperator = typeManager.DBPluginManager.GetBinaryOperator(rightNode.Operator.ContraryOperationSymbol);

                #endregion

                #region get values

                IOperationValue _rightAtom = null;
                object _rightComplex = null;

                if (rightNode.TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    _rightAtom = (IOperationValue)rightNode.Right;
                    _rightComplex = rightNode.Left;
                }
                else
                {
                    _rightAtom = (IOperationValue)rightNode.Left;
                    _rightComplex = rightNode.Right;
                }

                #endregion

                #region get new left value for current node

                IOperationValue _leftTempResult = (IOperationValue)contraryOperator.SimpleOperation((IOperationValue)_left, _rightAtom, _typeOfBinaryExpression).Value;
                _left = _leftTempResult;

                #endregion

                #region get the new right value

                _right = _rightComplex;

                #endregion

                #region update operator in case of an inequality

                if ((_operator.Label == BinaryOperator.Inequal) || (_operator.Label == BinaryOperator.GreaterEquals) || (_operator.Label == BinaryOperator.GreaterThan) || (_operator.Label == BinaryOperator.LessEquals) || (_operator.Label == BinaryOperator.LessThan))
                {
                    if (rightNode.Operator.Label == BinaryOperator.Division)
                    {
                        if (Convert.ToDouble(_rightAtom).CompareTo(0) < 0)
                        {
                            _operator = typeManager.DBPluginManager.GetBinaryOperator(_operator.ContraryOperationSymbol);
                        }
                    }
                }

                #endregion

                #endregion
            }
        }

        private object TryGetBinexpressionNode(object expression, DBContext dbContext)
        {
            #region data

            Object result = null;

            #endregion

            if (expression is BinaryExpressionNode)
            {
                result = expression;
            }
            else
            {
                //for negative values like -U.Age
                if (expression is UnaryExpressionNode)
                {
                    return _operator.GetBinaryExpressionNode((UnaryExpressionNode)expression, dbContext);
                }
                else
                {
                    if (expression is TupleNode)
                    {
                        return TryGetBinexpressionNode(((TupleNode)expression).Tuple[0].Value, dbContext);
                    }
                }
            }

            return result;
        }

        private object TryGetBinexpressionNode(ParseTreeNode aTreeNode, DBContext dbContext)
        {
            return TryGetBinexpressionNode(aTreeNode.AstNode, dbContext);
        }

        private IOperationValue TryGetOperationValue(object left)
        {
            #region data

            BinaryExpressionNode tempNode = left as BinaryExpressionNode;

            #endregion

            if (tempNode == null)
            {
                return null;
            }

            if (tempNode.TypeOfBinaryExpression.Equals(TypesOfBinaryExpression.Atom))
            {

                if (tempNode.ResultValue.Failed)
                {
                    //think about error handling here
                    throw new NotImplementedException();
                }

            }

            if (tempNode.ResultValue != null)
                return tempNode.ResultValue.Value;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myListOfIDNodes"></param>
        /// <param name="myLeftExpr"></param>
        /// <param name="myRightExpr"></param>
        private void calculateIDNodes(ref List<IDNode> myListOfIDNodes, Object myLeftExpr, Object myRightExpr)
        {
            if (myLeftExpr is BinaryExpressionNode)
            {
                calculateIDNodes(ref myListOfIDNodes, ((BinaryExpressionNode)myLeftExpr).Left, ((BinaryExpressionNode)myLeftExpr).Right);
            }
            else if (myLeftExpr is IDNode)
            {
                _ContainingIDNodes.Add((IDNode)myLeftExpr);
            }

            if (myRightExpr is BinaryExpressionNode)
            {
                calculateIDNodes(ref myListOfIDNodes, ((BinaryExpressionNode)myRightExpr).Left, ((BinaryExpressionNode)myRightExpr).Right);
            }
            else if (myRightExpr is IDNode)
            {
                _ContainingIDNodes.Add((IDNode)myRightExpr);
            }
        }

        /// <summary>
        /// This method combines simple binary expressions like (U.Age + 1) + 1 --> U.Age + 2
        /// </summary>
        /// <param name="_left">Left part of the binary expression.</param>
        /// <param name="_right">Right part of the binary expression.</param>
        private void SimpleCombination(ref object _left, ref object _right)
        {
            if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                #region left is complex

                #region data

                BinaryExpressionNode leftNode = (BinaryExpressionNode)_left;
                Boolean isValidOperation = false;

                #endregion

                foreach (String aSymbol in leftNode.Operator.Symbol)
                {
                    if (_operator.ContraryOperationSymbol.Equals(aSymbol) || _operator.Symbol.Contains(aSymbol))
                    {
                        isValidOperation = true;
                        break;
                    }
                }

                if (isValidOperation)
                {

                    #region get values

                    AtomValue _leftAtom = null;
                    object _leftComplex = null;

                    if (leftNode.TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                    {
                        _leftAtom = (AtomValue)leftNode.Right;
                        _leftComplex = leftNode.Left;
                    }
                    else
                    {
                        _leftAtom = (AtomValue)leftNode.Left;
                        _leftComplex = leftNode.Right;
                    }

                    #endregion

                    #region get the new left value

                    _left = _leftComplex;

                    #endregion

                    #region get new right value for current node

                    AtomValue _rightTempResult = (AtomValue)_operator.SimpleOperation(_leftAtom, (AtomValue)_right, _typeOfBinaryExpression).Value;
                    _right = _rightTempResult;

                    #endregion
                }
                #endregion
            }
            else
            {
                #region right is complex

                #region data

                BinaryExpressionNode rightNode = (BinaryExpressionNode)_right;
                Boolean isValidOperation = false;

                #endregion

                foreach (String aSymbol in rightNode.Operator.Symbol)
                {
                    if (_operator.ContraryOperationSymbol.Equals(aSymbol) || _operator.Symbol.Contains(aSymbol))
                    {
                        isValidOperation = true;
                        break;
                    }
                }

                if (isValidOperation)
                {
                    #region get values

                    AtomValue _rightAtom = null;
                    object _rightComplex = null;

                    if (rightNode.TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                    {
                        _rightAtom = (AtomValue)rightNode.Right;
                        _rightComplex = rightNode.Left;
                    }
                    else
                    {
                        _rightAtom = (AtomValue)rightNode.Left;
                        _rightComplex = rightNode.Right;
                    }

                    #endregion

                    #region get the new right value

                    _right = _rightComplex;

                    #endregion

                    #region get new left value for current node

                    AtomValue _leftTempResult = (AtomValue)_operator.SimpleOperation((AtomValue)_left, _rightAtom, _typeOfBinaryExpression).Value;
                    _left = _leftTempResult;

                    #endregion
                }

                #endregion
            }
        }

        /// <summary>
        /// This method checks if a binary expression is combinable.
        /// </summary>
        /// <param name="_left">Left part of the binary expression.</param>
        /// <param name="_right">Right part of the binary expression.</param>
        /// <returns></returns>
        private bool IsCombinableAble(object _left, object _right)
        {
            if (_typeOfBinaryExpression == TypesOfBinaryExpression.Complex)
            {
                return false;
            }

            if ((_operator.Label == BinaryOperator.Addition) || (_operator.Label == BinaryOperator.Subtraction) || (_operator.Label == BinaryOperator.Multiplication) || (_operator.Label == BinaryOperator.Division))
            {

                if (_typeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    #region left complex

                    if (_left is BinaryExpressionNode)
                    {
                        #region data

                        BinaryExpressionNode leftNode = (BinaryExpressionNode)_left;

                        #endregion

                        if (leftNode.TypeOfBinaryExpression != TypesOfBinaryExpression.Complex)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    #endregion
                }
                else
                {
                    #region right complex

                    if (_right is BinaryExpressionNode)
                    {
                        #region data

                        BinaryExpressionNode rightNode = (BinaryExpressionNode)_right;

                        #endregion

                        if (rightNode.TypeOfBinaryExpression != TypesOfBinaryExpression.Complex)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    #endregion
                }
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// This method gets the associativity of a binary expression.
        /// </summary>
        /// <param name="leftIDNode">Left IDNode.</param>
        /// <param name="rightIDNode">Right IDNode.</param>
        /// <returns>The associativity</returns>
        private TypesOfAssociativity GetAssociativityReloaded(IDNode leftIDNode, IDNode rightIDNode)
        {
            if (leftIDNode != null && rightIDNode != null)
            {
                if (leftIDNode.Reference == rightIDNode.Reference)
                {
                    return TypesOfAssociativity.Neutral;
                }
                else
                {
                    return TypesOfAssociativity.Unknown;
                }
            }
            else
            {
                if (leftIDNode != null)
                {
                    return TypesOfAssociativity.Left;
                }
                else
                {
                    if (rightIDNode != null)
                    {
                        return TypesOfAssociativity.Right;
                    }
                    else
                    {
                        return TypesOfAssociativity.Neutral;
                    }
                }
            }
        }

        /// <summary>
        /// This method gets the associativity of a binary expression.
        /// </summary>
        /// <param name="reference">The Reference string</param>
        /// <param name="leftIDNode">Left IDNode.</param>
        /// <param name="rightIDNode">Right IDNode.</param>
        /// <returns>The associativity</returns>
        private Exceptional<TypesOfAssociativity> GetAssociativity(string reference, IDNode leftIDNode, IDNode rightIDNode)
        {
            #region data

            String leftReference = null;
            String rightReference = null;

            #endregion

            if (leftIDNode != null)
            {
                if (!leftIDNode.IsValidated)
                {
                    return new Exceptional<TypesOfAssociativity>(new Error_InvalidIDNode(leftIDNode.ToString()));
                }

                leftReference = leftIDNode.Reference.Item1;
            }

            if (rightIDNode != null)
            {
                if (!rightIDNode.IsValidated)
                {
                    return new Exceptional<TypesOfAssociativity>(new Error_InvalidIDNode(rightIDNode.ToString()));
                }

                rightReference = rightIDNode.Reference.Item1;
            }

            if (leftReference != null)
            {
                #region special case

                if (rightReference == null)
                {
                    return new Exceptional<TypesOfAssociativity>(TypesOfAssociativity.Left);
                }

                #endregion


                if (leftReference.Equals(reference))
                {
                    return new Exceptional<TypesOfAssociativity>(TypesOfAssociativity.Left);
                }
                else
                {
                    if (rightReference != null)
                    {
                        if (rightReference.Equals(reference))
                        {
                            return new Exceptional<TypesOfAssociativity>(TypesOfAssociativity.Right);
                        }
                        else
                        {
                            return new Exceptional<TypesOfAssociativity>(TypesOfAssociativity.Neutral);
                        }
                    }
                    else
                    {
                        return new Exceptional<TypesOfAssociativity>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                }
            }
            else
            {
                if (rightReference != null)
                {
                    #region special case

                    if (leftReference == null)
                    {
                        return new Exceptional<TypesOfAssociativity>(TypesOfAssociativity.Right);
                    }

                    #endregion


                    if (rightReference.Equals(reference))
                    {
                        return new Exceptional<TypesOfAssociativity>(TypesOfAssociativity.Right);
                    }
                    else
                    {
                        return new Exceptional<TypesOfAssociativity>(TypesOfAssociativity.Neutral);
                    }
                }
                else
                {
                    return new Exceptional<TypesOfAssociativity>(TypesOfAssociativity.Neutral);
                }
            }
        }

        #endregion

        #region Accessessors

        public TypesOfBinaryExpression TypeOfBinaryExpression   { get { return _typeOfBinaryExpression; } set { _typeOfBinaryExpression = value; } }
        public Exceptional<IOperationValue> ResultValue         { get { return _resultValue; } }
        public Object                   Left                    { get { return _left; } set { _left = value;}}
        public Object                   Right                   { get { return _right; } set { _right = value;} }
        public ABinaryOperator          Operator                { get { return _operator; } set { _operator = value; } }

        #endregion


        public AObject SimpleExecution(DBObjectStream aDBObject, DBContext typeManager, SessionSettings mySessionToken)
        {
            SubstituteAttributeNames(this, aDBObject, typeManager, mySessionToken);

            return SimpleExecutionInternal().Value;
        }

        private AtomValue SimpleExecutionInternal()
        {
            switch (_typeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.Atom:

                    return (_operator.SimpleOperation((AtomValue)_left, (AtomValue)_right, _typeOfBinaryExpression).Value as AtomValue);

                case TypesOfBinaryExpression.LeftComplex:

                    return _operator.SimpleOperation(
                        ((BinaryExpressionNode)_left).SimpleExecutionInternal(),
                        (AtomValue)_right, _typeOfBinaryExpression)
                        .Value as AtomValue;

                case TypesOfBinaryExpression.RightComplex:

                    return _operator.SimpleOperation(
                        (AtomValue)_left,
                        ((BinaryExpressionNode)_right).SimpleExecutionInternal(), _typeOfBinaryExpression)
                        .Value as AtomValue;

                case TypesOfBinaryExpression.Complex:
                case TypesOfBinaryExpression.Unknown:
                default:
                    return _operator.SimpleOperation(
                       ((BinaryExpressionNode)_left).SimpleExecutionInternal(),
                       ((BinaryExpressionNode)_right).SimpleExecutionInternal(), _typeOfBinaryExpression)
                       .Value as AtomValue;
            }
        }

        /// <summary>
        /// This method
        /// </summary>
        /// <param name="aBinExpr"></param>
        /// <param name="aDBObject"></param>
        /// <param name="dbContext"></param>
        protected void SubstituteAttributeNames(BinaryExpressionNode aBinExpr, DBObjectStream aDBObject, DBContext dbContext, SessionSettings mySessionToken)
        {
            if (aBinExpr.Left is IDNode)
            {
                aBinExpr.Left = GetAtomValue((IDNode)aBinExpr.Left, aDBObject, dbContext);

                switch (aBinExpr.TypeOfBinaryExpression)
                {
                    case TypesOfBinaryExpression.LeftComplex:
                        aBinExpr.TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        break;
                    case TypesOfBinaryExpression.Complex:
                        aBinExpr.TypeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                        break;
                    
                    case TypesOfBinaryExpression.Atom:
                    case TypesOfBinaryExpression.RightComplex:
                    case TypesOfBinaryExpression.Unknown:
                    default:
                        break;
                }
            }
            else
            {
                if (aBinExpr.Left is BinaryExpressionNode)
                {
                    SubstituteAttributeNames((BinaryExpressionNode)aBinExpr.Left, aDBObject, dbContext, mySessionToken);
                }
            }

            if (aBinExpr.Right is IDNode)
            {
                aBinExpr.Right = GetAtomValue((IDNode)aBinExpr.Right, aDBObject, dbContext);

                switch (aBinExpr.TypeOfBinaryExpression)
                {
                    case TypesOfBinaryExpression.RightComplex:
                        aBinExpr.TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        break;
                    case TypesOfBinaryExpression.Complex:
                        aBinExpr.TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                        break;

                    case TypesOfBinaryExpression.Atom:
                    case TypesOfBinaryExpression.LeftComplex:
                    case TypesOfBinaryExpression.Unknown:
                    default:
                        break;
                }
            }
            else
            {
                if (aBinExpr.Right is BinaryExpressionNode)
                {
                    SubstituteAttributeNames((BinaryExpressionNode)aBinExpr.Right, aDBObject, dbContext, mySessionToken);
                }
            }
        }

        private object GetAtomValue(IDNode iDNode, DBObjectStream aDBObject, DBContext dbContext)
        {
            return new AtomValue(GraphDBTypeMapper.ConvertPandora2CSharp(iDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).Name), aDBObject.GetAttribute(iDNode.LastAttribute.UUID, iDNode.LastType, dbContext));
        }

        /// <summary>
        /// Validate all idnodes of this binaryExpressionNode
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="typeList"></param>
        /// <returns></returns>
        internal Exceptional Validate(DBContext dbContext, params GraphDBType[] typeList)
        {

            var retVal = new Exceptional();

            #region Validate where expression

            foreach (var idNode in ContainingIDNodes)
            {
                if (!idNode.IsValidated)
                {
                    var errors = new List<IError>();

                    #region Try to validate now

                    foreach (var type in typeList)
                    {
                        var result = idNode.ValidateMe(type, dbContext);
                        if (result.Success)
                        {
                            break;
                        }
                        else
                        {
                            retVal.AddErrorsAndWarnings(result);
                        }
                    }

                    #endregion

                    #region If idnode is still not validate we have an invalid where expression and must break

                    if (!idNode.IsValidated)
                    {
                        return retVal;
                    }

                    #endregion

                }
            }

            #endregion

            return retVal;

        }

    }//class
}//namespace
