/*
 * BinaryExpressionDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Operators;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using System.Threading.Tasks;
using sones.Lib;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class BinaryExpressionDefinition : AExpressionDefinition
    {

        #region Properties

        public TypesOfBinaryExpression TypeOfBinaryExpression { get; private set; }
        public Exceptional<AOperationDefinition> ResultValue { get; private set; }
        public ABinaryOperator Operator { get; private set; }
        public Exceptional ValidateResult { get; private set; }

        private String _OperatorSymbol;

        public Boolean IsValidated
        {
            get
            {
                return ValidateResult != null;
            }
        }

        private AExpressionDefinition _Left;
        public AExpressionDefinition Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        private AExpressionDefinition _Right;
        public AExpressionDefinition Right
        {
            get { return _Right; }
            set { _Right = value; }
        }

        #endregion

        #region Ctor

        public BinaryExpressionDefinition(String myOperatorSymbol, AExpressionDefinition myLeft, AExpressionDefinition myRight)
        {

            _OperatorSymbol = myOperatorSymbol;
            _Left = myLeft;
            _Right = myRight;

        }

        #endregion

        #region Validate

        /// <summary>
        /// Validates the expression (Set the TypeOfBinaryExpression, do some simplifications and validates all IDChains
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="types">This is optional, usually you wont pass any types</param>
        /// <returns></returns>
        public Exceptional Validate(DBContext myDBContext, params GraphDBType[] types)
        {

            if (IsValidated)
            {
                return ValidateResult;
            }
            ValidateResult = new Exceptional();

            Operator = myDBContext.DBPluginManager.GetBinaryOperator(_OperatorSymbol);

            #region Left - Valid TypesOfBinaryExpression are RightComplex (left is atom) or LeftComplex (left is complex)

            if (_Left is ValueDefinition)
            {
                TypeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
            }
            else if (_Left is IDChainDefinition)
            {
                ValidateResult.Push((_Left as IDChainDefinition).Validate(myDBContext, false, types));
                TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
            }
            else if (_Left is TupleDefinition)
            {

                #region TupleDefinition

                if (IsEncapsulatedBinaryExpression(_Left as TupleDefinition))
                {
                    var exceptionalResult = TryGetBinexpression((_Left as TupleDefinition).First().Value, myDBContext);
                    if (exceptionalResult.Failed)
                    {
                        return ValidateResult.Push(exceptionalResult);
                    }
                    _Left = exceptionalResult.Value;
                    TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                }
                else
                {
                    _Left = AssignCorrectTuple((_Left as TupleDefinition), Operator, myDBContext);
                    TypeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                }

                #endregion

            }
            else if (_Left is AggregateDefinition)
            {
                TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
            }
            else if (_Left is UnaryExpressionDefinition)
            {
                TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                var binExprResult = (_Left as UnaryExpressionDefinition).GetBinaryExpression(myDBContext);

                ValidateResult.Push(binExprResult);

                _Left = binExprResult.Value;
            }
            else
            {
                #region try binexpr

                var exceptionalResult = TryGetBinexpression(_Left, myDBContext);
                if (exceptionalResult.Failed)
                {
                    return ValidateResult.Push(exceptionalResult);
                }
                _Left = exceptionalResult.Value;
                TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;

                #endregion
            }


            #endregion

            if (ValidateResult.Failed)
            {
                return ValidateResult;
            }

            #region Right

            if (_Right is ValueDefinition)
            {
                if (TypeOfBinaryExpression == TypesOfBinaryExpression.RightComplex)
                {
                    TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                }
            }
            else if (_Right is IDChainDefinition)
            {
                ValidateResult.Push((_Right as IDChainDefinition).Validate(myDBContext, false, types));
                if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    TypeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                }
            }
            else if (_Right is UnaryExpressionDefinition)
            {
                if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    TypeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                }
                var binExprResult = (_Right as UnaryExpressionDefinition).GetBinaryExpression(myDBContext);

                ValidateResult.Push(binExprResult);

                _Right = binExprResult.Value;
            }
            else if (_Right is TupleDefinition)
            {

                #region TupleDefinition

                if (IsEncapsulatedBinaryExpression(_Right as TupleDefinition))
                {
                    var exceptionalResult = TryGetBinexpression((_Right as TupleDefinition).First().Value, myDBContext);
                    if (exceptionalResult.Failed)
                    {
                        return ValidateResult.Push(exceptionalResult);
                    }
                    _Right = exceptionalResult.Value;
                    if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                    {
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                    }
                }
                else
                {
                    _Right = AssignCorrectTuple((_Right as TupleDefinition), Operator, myDBContext);
                    if (TypeOfBinaryExpression == TypesOfBinaryExpression.RightComplex)
                    {
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                    }
                }

                #endregion

            }
            else if (_Right is AggregateDefinition)
            {
                if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    TypeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                }
            }
            else
            {
                #region try binexpr

                var exceptionalResult = TryGetBinexpression(_Right, myDBContext);
                if (exceptionalResult.Failed)
                {
                    return ValidateResult.Push(exceptionalResult);
                }
                _Right = exceptionalResult.Value;
                if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    TypeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                }

                #endregion
            }

            #endregion

            if (ValidateResult.Failed)
            {
                return ValidateResult;
            }

            #region try to get values from complex expr

            Exceptional<AOperationDefinition> leftTemp;
            Exceptional<AOperationDefinition> rightTemp;

            switch (TypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.Atom:

                    break;

                case TypesOfBinaryExpression.LeftComplex:

                    #region leftComplex

                    leftTemp = TryGetOperationValue(_Left);

                    if (leftTemp != null)
                    {
                        if (leftTemp.Failed)
                        {
                            return ValidateResult.Push(leftTemp);
                        }
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _Left = leftTemp.Value;
                    }

                    #endregion

                    break;

                case TypesOfBinaryExpression.RightComplex:

                    #region rightComplex

                    rightTemp = TryGetOperationValue(_Right);

                    if (rightTemp != null)
                    {
                        if (rightTemp.Failed)
                        {
                            return ValidateResult.Push(rightTemp);
                        }
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _Right = rightTemp.Value;
                    }

                    #endregion

                    break;

                case TypesOfBinaryExpression.Complex:

                    #region complex

                    leftTemp = TryGetOperationValue(_Left);
                    rightTemp = TryGetOperationValue(_Right);

                    #region Check for errors

                    if (leftTemp != null && leftTemp.Failed)
                    {
                        return ValidateResult.Push(leftTemp);
                    }
                    if (rightTemp != null && rightTemp.Failed)
                    {
                        return ValidateResult.Push(rightTemp);
                    }

                    #endregion

                    if ((leftTemp != null) && (rightTemp != null))
                    {
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _Left = leftTemp.Value;
                        _Right = rightTemp.Value;
                    }
                    else
                    {
                        if (leftTemp != null)
                        {
                            TypeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                            _Left = leftTemp.Value;
                        }
                        else if (rightTemp != null)
                        {
                            TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                            _Right = rightTemp.Value;
                        }
                    }

                    #endregion

                    break;
            }

            #endregion

            #region process values

            if (TypeOfBinaryExpression == TypesOfBinaryExpression.Atom)
            {
                #region atomic values

                if ((_Left is AOperationDefinition) && (_Right is AOperationDefinition))
                {
                    var opResult = Operator.SimpleOperation(((AOperationDefinition)_Left), ((AOperationDefinition)_Right), TypeOfBinaryExpression);
                    if (opResult.Failed)
                    {
                        return ValidateResult.Push(opResult);
                    }
                    ResultValue = new Exceptional<AOperationDefinition>(opResult.Value);
                }

                #endregion
            }
            else
            {
                #region some kind of complex values

                if (IsCombinableAble(_Left, _Right))
                {
                    #region summarize expressions
                    //sth like (U.Age + 1) + 1 --> U.Age + 2

                    SimpleCombination(ref _Left, ref _Right);

                    #endregion
                }
                else
                {
                    #region tweak expressions

                    while (IsTweakAble(_Left, _Right))
                    {
                        SimpleExpressionTweak(ref _Left, ref _Right, myDBContext);
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            return ValidateResult;

        }

        #endregion

        #region Helper

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myTupleDefinition"></param>
        /// <returns></returns>
        private bool IsEncapsulatedBinaryExpression(TupleDefinition myTupleDefinition)
        {
            if (myTupleDefinition.Count() == 1)
            {
                if ((myTupleDefinition.First().Value is BinaryExpressionDefinition) || (myTupleDefinition.First().Value is UnaryExpressionDefinition))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Exceptional<AExpressionDefinition> TryGetBinexpression(AExpressionDefinition expression, DBContext myDBContext)
        {

            if (expression is BinaryExpressionDefinition)
            {
                var validateResult = (expression as BinaryExpressionDefinition).Validate(myDBContext);
                return new Exceptional<AExpressionDefinition>((expression as BinaryExpressionDefinition), validateResult);
            }
            else
            {
                //for negative values like -U.Age
                if (expression is UnaryExpressionDefinition)
                {
                    var binExpr = (expression as UnaryExpressionDefinition).GetBinaryExpression(myDBContext);
                    return new Exceptional<AExpressionDefinition>(binExpr.Value, binExpr);
                }
                else
                {
                    if (expression is TupleDefinition)
                    {
                        return TryGetBinexpression(((TupleDefinition)expression).First().Value, myDBContext);
                    }
                }
            }

            return new Exceptional<AExpressionDefinition>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myTupleDefinition"></param>
        /// <param name="myOperator"></param>
        /// <param name="myDBContext"></param>
        /// <returns></returns>
        private AOperationDefinition AssignCorrectTuple(TupleDefinition myTupleDefinition, ABinaryOperator myOperator, DBContext myDBContext)
        {
            return myOperator.GetValidTupleReloaded(myTupleDefinition, myDBContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myAExpressionDefinition"></param>
        /// <returns></returns>
        private Exceptional<AOperationDefinition> TryGetOperationValue(AExpressionDefinition myAExpressionDefinition)
        {
            #region data

            var expression = myAExpressionDefinition as BinaryExpressionDefinition;

            #endregion

            if (expression == null)
            {
                return null;
            }

            if (expression.TypeOfBinaryExpression == TypesOfBinaryExpression.Atom)
            {

                if (expression.ResultValue.Failed)
                {
                    return expression.ResultValue;
                }

            }

            if (expression.ResultValue != null)
            {
                return expression.ResultValue;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_left"></param>
        /// <param name="_right"></param>
        /// <param name="typeManager"></param>
        private void SimpleExpressionTweak(ref AExpressionDefinition _left, ref AExpressionDefinition _right, DBContext typeManager)
        {
            if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                #region left is complex

                #region data

                var leftNode = (BinaryExpressionDefinition)_left;

                #endregion

                #region get contrary operator for complex part

                ABinaryOperator contraryOperator = typeManager.DBPluginManager.GetBinaryOperator(leftNode.Operator.ContraryOperationSymbol);

                #endregion

                #region get values

                ValueDefinition simpleValue = null;
                AExpressionDefinition _leftComplex = null;

                if (leftNode.TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    simpleValue = (leftNode._Right as ValueDefinition);
                    simpleValue.ChangeType(leftNode._Left);
                    _leftComplex = leftNode._Left;
                }
                else
                {
                    simpleValue = (leftNode._Left as ValueDefinition);
                    simpleValue.ChangeType(leftNode._Right);
                    _leftComplex = leftNode._Right;
                }

                #endregion

                #region get new right value for current node

                var _rightTempResult = contraryOperator.SimpleOperation((AOperationDefinition)_right, simpleValue, TypeOfBinaryExpression);

                _right = _rightTempResult.Value;

                #endregion

                #region get the new left value

                _left = _leftComplex;

                #endregion

                #region update operator in case of an inequality

                if (Operator.Label == BinaryOperator.Inequal)
                {
                    if (leftNode.Operator.Label == BinaryOperator.Division)
                    {
                        if (Convert.ToDouble(simpleValue).CompareTo(0) < 0)
                        {
                            Operator = typeManager.DBPluginManager.GetBinaryOperator(Operator.ContraryOperationSymbol);
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

                var rightNode = (BinaryExpressionDefinition)_right;

                #endregion

                #region get contrary operator for complex part

                ABinaryOperator contraryOperator = typeManager.DBPluginManager.GetBinaryOperator(rightNode.Operator.ContraryOperationSymbol);

                #endregion

                #region get values

                AOperationDefinition _rightAtom = null;
                AExpressionDefinition _rightComplex = null;

                if (rightNode.TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    _rightAtom = (AOperationDefinition)rightNode._Right;
                    _rightComplex = rightNode._Left;
                }
                else
                {
                    _rightAtom = (AOperationDefinition)rightNode._Left;
                    _rightComplex = rightNode._Right;
                }

                #endregion

                #region get new left value for current node

                var _leftTempResult = contraryOperator.SimpleOperation(((AOperationDefinition)_left), _rightAtom, TypeOfBinaryExpression).Value;
                _left = _leftTempResult;

                #endregion

                #region get the new right value

                _right = _rightComplex;

                #endregion

                #region update operator in case of an inequality

                if ((Operator.Label == BinaryOperator.Inequal) || (Operator.Label == BinaryOperator.GreaterEquals) || (Operator.Label == BinaryOperator.GreaterThan) || (Operator.Label == BinaryOperator.LessEquals) || (Operator.Label == BinaryOperator.LessThan))
                {
                    if (rightNode.Operator.Label == BinaryOperator.Division)
                    {
                        if (Convert.ToDouble(_rightAtom).CompareTo(0) < 0)
                        {
                            Operator = typeManager.DBPluginManager.GetBinaryOperator(Operator.ContraryOperationSymbol);
                        }
                    }
                }

                #endregion

                #endregion
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myBinaryOperatorType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_left"></param>
        /// <param name="_right"></param>
        /// <returns></returns>
        private bool IsTweakAble(AExpressionDefinition _left, AExpressionDefinition _right)
        {
            if (TypeOfBinaryExpression == TypesOfBinaryExpression.Complex)
            {
                return false;
            }

            if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                #region left complex

                if (_left is BinaryExpressionDefinition)
                {
                    #region data

                    var leftNode = (BinaryExpressionDefinition)_left;

                    #endregion

                    if (leftNode.TypeOfBinaryExpression != TypesOfBinaryExpression.Complex)
                    {
                        if ((leftNode.Operator.Label == BinaryOperator.Addition) || (leftNode.Operator.Label == BinaryOperator.Subtraction) || (leftNode.Operator.Label == BinaryOperator.Multiplication) || (leftNode.Operator.Label == BinaryOperator.Division))
                        {
                            if (isTweakAbleOperator(Operator.Label))
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

                if (_right is BinaryExpressionDefinition)
                {
                    #region data

                    var rightNode = (BinaryExpressionDefinition)_right;

                    #endregion

                    if (rightNode.TypeOfBinaryExpression != TypesOfBinaryExpression.Complex)
                    {
                        if ((rightNode.Operator.Label == BinaryOperator.Addition) || (rightNode.Operator.Label == BinaryOperator.Subtraction) || (rightNode.Operator.Label == BinaryOperator.Multiplication) || (rightNode.Operator.Label == BinaryOperator.Division))
                        {
                            if ((Operator.Label == BinaryOperator.Equal) || (Operator.Label == BinaryOperator.Inequal))
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


        /// <summary>
        /// This method checks if a binary expression is combinable.
        /// </summary>
        /// <param name="_left">Left part of the binary expression.</param>
        /// <param name="_right">Right part of the binary expression.</param>
        /// <returns></returns>
        private bool IsCombinableAble(AExpressionDefinition _left, AExpressionDefinition _right)
        {
            if (TypeOfBinaryExpression == TypesOfBinaryExpression.Complex)
            {
                return false;
            }

            if ((Operator.Label == BinaryOperator.Addition) || (Operator.Label == BinaryOperator.Subtraction) || (Operator.Label == BinaryOperator.Multiplication) || (Operator.Label == BinaryOperator.Division))
            {

                if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    #region left complex

                    if (_left is BinaryExpressionDefinition)
                    {
                        #region data

                        var leftNode = (BinaryExpressionDefinition)_left;

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

                    if (_right is BinaryExpressionDefinition)
                    {
                        #region data

                        var rightNode = (BinaryExpressionDefinition)_right;

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
        /// This method combines simple binary expressions like (U.Age + 1) + 1 --> U.Age + 2
        /// </summary>
        /// <param name="_left">Left part of the binary expression.</param>
        /// <param name="_right">Right part of the binary expression.</param>
        private void SimpleCombination(ref AExpressionDefinition _left, ref AExpressionDefinition _right)
        {
            if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                #region left is complex

                #region data

                var leftNode = _left as BinaryExpressionDefinition;
                Boolean isValidOperation = false;

                #endregion

                foreach (String aSymbol in leftNode.Operator.Symbol)
                {
                    if (Operator.ContraryOperationSymbol.Equals(aSymbol) || Operator.Symbol.Contains(aSymbol))
                    {
                        isValidOperation = true;
                        break;
                    }
                }

                if (isValidOperation)
                {

                    #region get values

                    ValueDefinition simpleValue = null;
                    AExpressionDefinition _leftComplex = null;

                    if (leftNode.TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                    {
                        simpleValue = leftNode._Right as ValueDefinition;
                        simpleValue.ChangeType(leftNode._Left);
                        _leftComplex = leftNode._Left;
                    }
                    else
                    {
                        simpleValue = leftNode._Left as ValueDefinition;
                        simpleValue.ChangeType(leftNode._Right);
                        _leftComplex = leftNode._Right;
                    }

                    #endregion

                    #region get the new left value

                    _left = _leftComplex;

                    #endregion

                    #region get new right value for current node

                    var _rightTempResult = Operator.SimpleOperation(simpleValue, ((ValueDefinition)_right), TypeOfBinaryExpression).Value;
                    _right = _rightTempResult;

                    #endregion
                }
                #endregion
            }
            else
            {
                #region right is complex

                #region data

                var rightNode = (BinaryExpressionDefinition)_right;
                Boolean isValidOperation = false;

                #endregion

                foreach (String aSymbol in rightNode.Operator.Symbol)
                {
                    if (Operator.ContraryOperationSymbol.Equals(aSymbol) || Operator.Symbol.Contains(aSymbol))
                    {
                        isValidOperation = true;
                        break;
                    }
                }

                if (isValidOperation)
                {
                    #region get values

                    ValueDefinition _rightAtom = null;
                    AExpressionDefinition _rightComplex = null;

                    if (rightNode.TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                    {
                        _rightAtom = ((ValueDefinition)rightNode.Right);
                        _rightComplex = rightNode.Left;
                    }
                    else
                    {
                        _rightAtom = ((ValueDefinition)rightNode.Left);
                        _rightComplex = rightNode.Right;
                    }

                    #endregion

                    #region get the new right value

                    _right = _rightComplex;

                    #endregion

                    #region get new left value for current node

                    var _leftTempResult = Operator.SimpleOperation(((ValueDefinition)_left), _rightAtom, TypeOfBinaryExpression).Value;
                    _left = _leftTempResult;

                    #endregion
                }

                #endregion
            }
        }

        internal AObject SimpleExecution(ObjectManagement.DBObjectStream aDBObject, DBContext dbContext)
        {
            SubstituteAttributeNames(this, aDBObject, dbContext);

            return SimpleExecutionInternal().Value;
        }

        private ValueDefinition GetAtomValue(IDChainDefinition iDNode, DBObjectStream aDBObject, DBContext dbContext)
        {
            return new ValueDefinition(GraphDBTypeMapper.ConvertPandora2CSharp(iDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).Name), aDBObject.GetAttribute(iDNode.LastAttribute.UUID, iDNode.LastType, dbContext));
        }

        /// <summary>
        /// This method
        /// </summary>
        /// <param name="aBinExpr"></param>
        /// <param name="aDBObject"></param>
        /// <param name="dbContext"></param>
        protected void SubstituteAttributeNames(BinaryExpressionDefinition aBinExpr, DBObjectStream aDBObject, DBContext dbContext)
        {
            if (aBinExpr.Left is IDChainDefinition)
            {
                aBinExpr.Left = GetAtomValue((IDChainDefinition)aBinExpr.Left, aDBObject, dbContext);

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
                if (aBinExpr.Left is BinaryExpressionDefinition)
                {
                    SubstituteAttributeNames((BinaryExpressionDefinition)aBinExpr.Left, aDBObject, dbContext);
                }
            }

            if (aBinExpr.Right is IDChainDefinition)
            {
                aBinExpr.Right = GetAtomValue((IDChainDefinition)aBinExpr.Right, aDBObject, dbContext);

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
                if (aBinExpr.Right is BinaryExpressionDefinition)
                {
                    SubstituteAttributeNames((BinaryExpressionDefinition)aBinExpr.Right, aDBObject, dbContext);
                }
            }
        }

        private ValueDefinition SimpleExecutionInternal()
        {
            switch (TypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.Atom:

                    return (Operator.SimpleOperation(((ValueDefinition)_Left), ((ValueDefinition)_Right), TypeOfBinaryExpression).Value as ValueDefinition);

                case TypesOfBinaryExpression.LeftComplex:

                    return Operator.SimpleOperation(
                        ((BinaryExpressionDefinition)_Left).SimpleExecutionInternal(),
                        ((ValueDefinition)_Right), TypeOfBinaryExpression)
                        .Value as ValueDefinition;

                case TypesOfBinaryExpression.RightComplex:

                    return Operator.SimpleOperation(
                        ((ValueDefinition)_Left),
                        ((BinaryExpressionDefinition)_Right).SimpleExecutionInternal(), TypeOfBinaryExpression)
                        .Value as ValueDefinition;

                case TypesOfBinaryExpression.Complex:
                case TypesOfBinaryExpression.Unknown:
                default:
                    return Operator.SimpleOperation(
                       ((BinaryExpressionDefinition)_Left).SimpleExecutionInternal(),
                       ((BinaryExpressionDefinition)_Right).SimpleExecutionInternal(), TypeOfBinaryExpression)
                       .Value as ValueDefinition;
            }
        }


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

                        if (this.Left is BinaryExpressionDefinition)
                        {
                            return ((BinaryExpressionDefinition)this.Left).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                        }
                        else
                        {
                            return new Exceptional<IExpressionGraph>(new Error_InvalidBinaryExpression(this));
                        }

                        #endregion

                    case TypesOfBinaryExpression.RightComplex:

                        #region right complex

                        if (this.Right is BinaryExpressionDefinition)
                        {
                            return ((BinaryExpressionDefinition)this.Right).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                        }
                        else
                        {
                            return new Exceptional<IExpressionGraph>(new Error_InvalidBinaryExpression(this));
                        }

                        #endregion

                    case TypesOfBinaryExpression.Complex:

                        #region complex

                        var runMT = DBConstants.RunMT;

                        if (!((this.Left is BinaryExpressionDefinition) && (this.Right is BinaryExpressionDefinition)))
                        {
                            return new Exceptional<IExpressionGraph>(new Error_InvalidBinaryExpression(this));
                        }

                        if (!(this.Operator is ABinaryLogicalOperator))
                        {
                            return new Exceptional<IExpressionGraph>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }

                        if (runMT)
                        {
                            var leftTask = Task<Exceptional<IExpressionGraph>>.Factory.StartNew(() =>
                            {
                                return ((BinaryExpressionDefinition)this.Left).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                            });

                            var rightTask = Task<Exceptional<IExpressionGraph>>.Factory.StartNew(() =>
                            {
                                return ((BinaryExpressionDefinition)this.Right).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                            });

                            if (!leftTask.Result.Success)
                            {
                                return new Exceptional<IExpressionGraph>(leftTask.Result);
                            }

                            if (!rightTask.Result.Success)
                            {
                                return new Exceptional<IExpressionGraph>(rightTask.Result);
                            }

                            return (this.Operator as ABinaryLogicalOperator).TypeOperation(
                                leftTask.Result.Value,
                                rightTask.Result.Value,
                                dbContext,
                                this.TypeOfBinaryExpression, TypesOfAssociativity.Neutral, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                            //*/

                        }
                        else
                        {
                            var left = ((BinaryExpressionDefinition)this.Left).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);
                            var right = ((BinaryExpressionDefinition)this.Right).Calculon(dbContext, resultGraph.GetNewInstance(dbContext), aggregateAllowed);

                            if (!left.Success)
                            {
                                return new Exceptional<IExpressionGraph>(left);
                            }

                            if (!right.Success)
                            {
                                return new Exceptional<IExpressionGraph>(right);
                            }

                            ///* Synchronious call
                            return (this.Operator as ABinaryLogicalOperator).TypeOperation(
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
        /// This method gets the associativity of a binary expression.
        /// </summary>
        /// <param name="leftIDNode">Left IDNode.</param>
        /// <param name="rightIDNode">Right IDNode.</param>
        /// <returns>The associativity</returns>
        private TypesOfAssociativity GetAssociativityReloaded(IDChainDefinition leftIDNode, IDChainDefinition rightIDNode)
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
        /// Extracts an IDNode out of an object.
        /// </summary>
        /// <param name="p">A potential IDNode</param>
        /// <returns>An IDNode.</returns>
        private IDChainDefinition ExtractIDNode(object p)
        {
            if (p != null)
            {
                if (p is IDChainDefinition)
                {
                    return (IDChainDefinition)p;
                }
                else
                {
                    if (p is ChainPartFuncDefinition)
                    {
                        return ExtractIDNode(((ChainPartFuncDefinition)p).Parameters.FirstOrDefault());
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

        /// <summary>
        /// Returns if the current BinaryExpressionNode is a leaf-expression without any sub-expressions.
        /// </summary>
        /// <returns>True if it is a leaf expression, otherwise false.</returns>
        public bool IsLeafExpression()
        {
            if ((this.Left is BinaryExpressionDefinition) || (this.Right is BinaryExpressionDefinition))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region Having

        public Exceptional<bool> IsSatisfyHaving(DBObjectReadoutGroup myDBObjectReadoutGroup, DBContext dbContext)
        {

            if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                String attributeName = null;
                ValueDefinition leftValue = null;

                if (!EvaluateHaving(myDBObjectReadoutGroup, _Left, out attributeName, out leftValue, dbContext).Value)
                    return new Exceptional<bool>(false);

                var resultValue = Operator.SimpleOperation(leftValue, ((ValueDefinition)_Right), TypeOfBinaryExpression);
                if (resultValue.Failed)
                {
                    return new Exceptional<bool>(resultValue);
                }
                return new Exceptional<bool>(((resultValue.Value as ValueDefinition).Value as DBBoolean).GetValue());

            }

            else if (TypeOfBinaryExpression == TypesOfBinaryExpression.RightComplex)
            {
                String attributeName = null;
                ValueDefinition rightValue = null;

                if (!EvaluateHaving(myDBObjectReadoutGroup, _Right, out attributeName, out rightValue, dbContext).Value)
                    return new Exceptional<bool>(false);

                var resultValue = Operator.SimpleOperation(((ValueDefinition)_Left), rightValue, TypeOfBinaryExpression);
                if (resultValue.Failed)
                {
                    return new Exceptional<bool>(resultValue);
                } 
                return new Exceptional<bool>(((resultValue.Value as ValueDefinition).Value as DBBoolean).GetValue());

            }

            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

        }

        private Exceptional<Boolean> EvaluateHaving(DBObjectReadoutGroup myDBObjectReadoutGroup, AExpressionDefinition complexValue, out String attributeName, out ValueDefinition simpleValue, DBContext dbContext)
        {

            GraphDBType graphDBType = null;
            attributeName = null;
            simpleValue = null;

            if (complexValue is IDChainDefinition)
            {
                if (((IDChainDefinition)complexValue).LastAttribute == null)
                {
                    if (((IDChainDefinition)complexValue).Last() is ChainPartFuncDefinition)
                    {
                        var func = (((IDChainDefinition)complexValue).Last() as ChainPartFuncDefinition);
                        if (func.Parameters.Count != 1)
                            return new Exceptional<Boolean>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                        attributeName = func.SourceParsedString;
                        //graphDBType = func.Parameters.First().LastAttribute.GetDBType(dbContext.DBTypeManager);
                    }
                    else if (((IDChainDefinition)complexValue).IsUndefinedAttribute)
                    {
                        attributeName = ((IDChainDefinition)complexValue).UndefinedAttribute;

                        //return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                    else
                    {
                        //return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                }
                else
                {
                    attributeName = ((IDChainDefinition)complexValue).LastAttribute.Name;
                    //graphDBType = ((IDChainDefinition)complexValue).LastAttribute.GetDBType(dbContext.DBTypeManager);
                }
            }
            else
            {
                if (complexValue is AggregateDefinition)
                {
                    var func = (complexValue as AggregateDefinition);
                    if (func.ChainPartAggregateDefinition.Parameters.Count != 1)
                        return new Exceptional<Boolean>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                    attributeName = func.ChainPartAggregateDefinition.SourceParsedString;
                    //graphDBType = func.ContainingIDNodes.First().LastAttribute.GetDBType(dbContext.DBTypeManager);
                }
                else
                {
                    return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
            }

            if (myDBObjectReadoutGroup.Attributes.ContainsKey(attributeName))
            {
                ADBBaseObject objectValue = GraphDBTypeMapper.GetBaseObjectFromCSharpType(myDBObjectReadoutGroup.Attributes[attributeName]);
                simpleValue = new ValueDefinition(objectValue);
                return new Exceptional<bool>(true);
            }

            return new Exceptional<bool>(false);

        }

        #endregion

        #endregion
    }
}
