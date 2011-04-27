using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.Result;
using sones.GraphQL.GQL.Manager.Select;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class BinaryExpressionDefinition : AExpressionDefinition
    {

        #region Properties

        public TypesOfBinaryExpression TypeOfBinaryExpression { get; private set; }
        public AOperationDefinition ResultValue { get; private set; }
        public ABinaryOperator Operator { get; private set; }
        private Boolean _isValidated = false;

        private String _OperatorSymbol;

        public Boolean IsValidated
        {
            get
            {
                return _isValidated;
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

        public String OperatorSymbol
        {
            get { return _OperatorSymbol; }
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
        public void Validate(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params IVertexType[] types)
        {

            if (IsValidated)
            {
                return;
            }

            Operator = GetOperatorBySymbol(_OperatorSymbol);

            #region Left - Valid TypesOfBinaryExpression are RightComplex (left is atom) or LeftComplex (left is complex)

            if (_Left is ValueDefinition)
            {
                TypeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
            }
            else if (_Left is IDChainDefinition)
            {
                (_Left as IDChainDefinition).Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, true, types);
                TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
            }
            else if (_Left is TupleDefinition)
            {

                #region TupleDefinition

                if (IsEncapsulatedBinaryExpression(_Left as TupleDefinition))
                {
                    _Left = TryGetBinexpression((_Left as TupleDefinition).First().Value, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                    TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                }
                else
                {
                    _Left = AssignCorrectTuple((_Left as TupleDefinition), Operator, myGraphDB, mySecurityToken, myTransactionToken);
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
                _Left = (_Left as UnaryExpressionDefinition).GetBinaryExpression(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
            }
            else
            {
                #region try binexpr

                _Left = TryGetBinexpression(_Left, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;

                #endregion
            }


            #endregion

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
                (_Right as IDChainDefinition).Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, false, types);
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
                _Right = (_Right as UnaryExpressionDefinition).GetBinaryExpression(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
            }
            else if (_Right is TupleDefinition)
            {

                #region TupleDefinition

                if (IsEncapsulatedBinaryExpression(_Right as TupleDefinition))
                {
                    _Right = TryGetBinexpression((_Right as TupleDefinition).First().Value, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                    if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                    {
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                    }
                }
                else
                {
                    _Right = AssignCorrectTuple((_Right as TupleDefinition), Operator, myGraphDB, mySecurityToken, myTransactionToken);
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

                _Right = TryGetBinexpression(_Right, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
                {
                    TypeOfBinaryExpression = TypesOfBinaryExpression.Complex;
                }

                #endregion
            }

            #endregion

            #region try to get values from complex expr

            AOperationDefinition leftTemp;
            AOperationDefinition rightTemp;

            switch (TypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.Atom:

                    break;

                case TypesOfBinaryExpression.LeftComplex:

                    #region leftComplex

                    leftTemp = TryGetOperationValue(_Left);

                    if (leftTemp != null)
                    {
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _Left = leftTemp;
                    }

                    #endregion

                    break;

                case TypesOfBinaryExpression.RightComplex:

                    #region rightComplex

                    rightTemp = TryGetOperationValue(_Right);

                    if (rightTemp != null)
                    {
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _Right = rightTemp;
                    }

                    #endregion

                    break;

                case TypesOfBinaryExpression.Complex:

                    #region complex

                    leftTemp = TryGetOperationValue(_Left);
                    rightTemp = TryGetOperationValue(_Right);

                    if ((leftTemp != null) && (rightTemp != null))
                    {
                        TypeOfBinaryExpression = TypesOfBinaryExpression.Atom;
                        _Left = leftTemp;
                        _Right = rightTemp;
                    }
                    else
                    {
                        if (leftTemp != null)
                        {
                            TypeOfBinaryExpression = TypesOfBinaryExpression.RightComplex;
                            _Left = leftTemp;
                        }
                        else if (rightTemp != null)
                        {
                            TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                            _Right = rightTemp;
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
                    ResultValue = Operator.SimpleOperation(((AOperationDefinition)_Left), ((AOperationDefinition)_Right), TypeOfBinaryExpression);
                }

                #endregion
            }

            #endregion

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

        private AExpressionDefinition TryGetBinexpression(AExpressionDefinition expression, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {

            if (expression is BinaryExpressionDefinition)
            {
                (expression as BinaryExpressionDefinition).Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                return expression as BinaryExpressionDefinition;
            }
            else
            {
                //for negative values like -U.Age
                if (expression is UnaryExpressionDefinition)
                {
                    var binExpr = (expression as UnaryExpressionDefinition).GetBinaryExpression(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                    return binExpr;
                }
                else
                {
                    if (expression is TupleDefinition)
                    {
                        return TryGetBinexpression(((TupleDefinition)expression).First().Value, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                    }
                }
            }

            throw new NotImplementedQLException("");

        }

        /// <summary>
        /// This will check all tupe value. If it contains only one value it will be converted to a ValueDefinition. If it contains a SelectDefinition it will be executed and the result added to the tuple.
        /// </summary>
        private AOperationDefinition AssignCorrectTuple(TupleDefinition myTupleDefinition, ABinaryOperator myOperator, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var retVal = new TupleDefinition(myTupleDefinition.KindOfTuple);
            var validTuple = myOperator.GetValidTupleReloaded(myTupleDefinition, myGraphDB, mySecurityToken, myTransactionToken);

            if (validTuple is TupleDefinition)
            {
                foreach (var tupleVal in (validTuple as TupleDefinition))
                {
                    if (tupleVal.Value is ValueDefinition)
                    {
                        retVal.AddElement(tupleVal);
                    }
                    else if (tupleVal.Value is SelectDefinition)
                    {

                        #region partial select

                        var selectManager = new SelectManager();
                        var qresult = selectManager.ExecuteSelect(myGraphDB, mySecurityToken, myTransactionToken, (tupleVal.Value as SelectDefinition));
                        if (qresult.Error !=  null)
                        {
                            throw qresult.Error;
                        }

                        IAttributeDefinition curAttr = ((tupleVal.Value as SelectDefinition).SelectedElements.First().Item1 as IDChainDefinition).LastAttribute;

                        foreach (var _Vertex in qresult.Vertices)
                        {
                            if (!(_Vertex.HasProperty(curAttr.Name)))
                                continue;

                            if (curAttr != null)
                            {
                                var val = new ValueDefinition(_Vertex.GetProperty<Object>(curAttr.Name));
                                retVal.AddElement(new TupleElement(val));
                            }
                            else
                            {
                                throw new NotImplementedQLException("");
                            }
                        }


                        #endregion
                    }
                    else
                    {
                        throw new NotImplementedQLException("");
                    }
                }
            }
            else
            {
                return validTuple;
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myAExpressionDefinition"></param>
        /// <returns></returns>
        private AOperationDefinition TryGetOperationValue(AExpressionDefinition myAExpressionDefinition)
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
                return expression.ResultValue;
            }

            if (expression.ResultValue != null)
            {
                return expression.ResultValue;
            }

            return null;
        }

        internal object SimpleExecution(IVertex aDBObject)
        {
            SubstituteAttributeNames(this, aDBObject);

            return SimpleExecutionInternal().Value;
        }

        private ValueDefinition GetAtomValue(IDChainDefinition iDNode, IVertex aDBObject)
        {
            return new ValueDefinition(aDBObject.GetProperty(iDNode.LastAttribute.AttributeID));
        }

        /// <summary>
        /// This method
        /// </summary>
        /// <param name="aBinExpr"></param>
        /// <param name="aDBObject"></param>
        /// <param name="dbContext"></param>
        protected void SubstituteAttributeNames(BinaryExpressionDefinition aBinExpr, IVertex aDBObject)
        {
            if (aBinExpr.Left is IDChainDefinition)
            {
                aBinExpr.Left = GetAtomValue((IDChainDefinition)aBinExpr.Left, aDBObject);

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
                    SubstituteAttributeNames((BinaryExpressionDefinition)aBinExpr.Left, aDBObject);
                }
            }

            if (aBinExpr.Right is IDChainDefinition)
            {
                aBinExpr.Right = GetAtomValue((IDChainDefinition)aBinExpr.Right, aDBObject);

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
                    SubstituteAttributeNames((BinaryExpressionDefinition)aBinExpr.Right, aDBObject);
                }
            }
        }

        private ValueDefinition SimpleExecutionInternal()
        {
            switch (TypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.Atom:

                    return (Operator.SimpleOperation(((ValueDefinition)_Left), ((ValueDefinition)_Right), TypeOfBinaryExpression) as ValueDefinition);

                case TypesOfBinaryExpression.LeftComplex:

                    return Operator.SimpleOperation(
                        ((BinaryExpressionDefinition)_Left).SimpleExecutionInternal(),
                        ((ValueDefinition)_Right), TypeOfBinaryExpression)
                        as ValueDefinition;

                case TypesOfBinaryExpression.RightComplex:

                    return Operator.SimpleOperation(
                        ((ValueDefinition)_Left),
                        ((BinaryExpressionDefinition)_Right).SimpleExecutionInternal(), TypeOfBinaryExpression)
                        as ValueDefinition;

                case TypesOfBinaryExpression.Complex:
                case TypesOfBinaryExpression.Unknown:
                default:
                    return Operator.SimpleOperation(
                       ((BinaryExpressionDefinition)_Left).SimpleExecutionInternal(),
                       ((BinaryExpressionDefinition)_Right).SimpleExecutionInternal(), TypeOfBinaryExpression)
                       as ValueDefinition;
            }
        }


        /// <summary>
        /// This method evaluates binary expressions.
        /// </summary>
        /// <param name="currentTypeDefinition">KeyValuePair of Reference and corresponding GraphType.</param>
        /// <param name="referenceList">List of References.</param>
        /// <param name="dbContext">The TypeManager of the GraphDB.</param>
        /// <param name="queryCache">The current query cache.</param>
        /// <param name="resultGraph">A template of the result graph</param>
        /// <returns>A GraphResult container.</returns>
        public IExpressionGraph Calculon(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IExpressionGraph resultGraph, bool aggregateAllowed = true)
        {
            //a leaf expression is a expression without any recursive BinaryExpression
            if (IsLeafExpression())
            {
                #region process leaf expression

                return this.Operator.TypeOperation(
                    this.Left, this.Right,
                    myGraphDB, mySecurityToken, myTransactionToken,
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
                            return ((BinaryExpressionDefinition)this.Left).Calculon(myGraphDB, mySecurityToken, myTransactionToken, resultGraph.GetNewInstance(myGraphDB, mySecurityToken, myTransactionToken), aggregateAllowed);
                        }
                        else
                        {
                            throw new InvalidBinaryExpressionException(this);
                        }

                        #endregion

                    case TypesOfBinaryExpression.RightComplex:

                        #region right complex

                        if (this.Right is BinaryExpressionDefinition)
                        {
                            return ((BinaryExpressionDefinition)this.Right).Calculon(myGraphDB, mySecurityToken, myTransactionToken, resultGraph.GetNewInstance(myGraphDB, mySecurityToken, myTransactionToken), aggregateAllowed);
                        }
                        else
                        {
                            throw new InvalidBinaryExpressionException(this);
                        }

                        #endregion

                    case TypesOfBinaryExpression.Complex:

                        #region complex

                        if (!((this.Left is BinaryExpressionDefinition) && (this.Right is BinaryExpressionDefinition)))
                        {
                            throw new InvalidBinaryExpressionException(this);
                        }

                        if (!(this.Operator is ABinaryLogicalOperator))
                        {
                            throw new NotImplementedQLException("");
                        }


                        var left = ((BinaryExpressionDefinition)this.Left).Calculon(myGraphDB, mySecurityToken, myTransactionToken, resultGraph.GetNewInstance(myGraphDB, mySecurityToken, myTransactionToken), aggregateAllowed);
                        var right = ((BinaryExpressionDefinition)this.Right).Calculon(myGraphDB, mySecurityToken, myTransactionToken, resultGraph.GetNewInstance(myGraphDB, mySecurityToken, myTransactionToken), aggregateAllowed);

                        return (this.Operator as ABinaryLogicalOperator).TypeOperation(
                            left,
                            right,
                            myGraphDB, mySecurityToken, myTransactionToken,
                            this.TypeOfBinaryExpression, TypesOfAssociativity.Neutral, resultGraph.GetNewInstance(myGraphDB, mySecurityToken, myTransactionToken), aggregateAllowed);

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

        public bool IsSatisfyHaving(IVertexView myDBObjectReadoutGroup)
        {

            if (TypeOfBinaryExpression == TypesOfBinaryExpression.LeftComplex)
            {
                String attributeName = null;
                ValueDefinition leftValue = null;

                if (!EvaluateHaving(myDBObjectReadoutGroup, _Left, out attributeName, out leftValue))
                {
                    return false;
                }

                var resultValue = Operator.SimpleOperation(leftValue, ((ValueDefinition)_Right), TypeOfBinaryExpression);


                return (Boolean)((resultValue as ValueDefinition).Value);

            }

            else if (TypeOfBinaryExpression == TypesOfBinaryExpression.RightComplex)
            {
                String attributeName = null;
                ValueDefinition rightValue = null;

                if (!EvaluateHaving(myDBObjectReadoutGroup, _Right, out attributeName, out rightValue))
                {
                    return false;
                }

                var resultValue = Operator.SimpleOperation(((ValueDefinition)_Left), rightValue, TypeOfBinaryExpression);


                return (Boolean)((resultValue as ValueDefinition).Value);

            }

            throw new NotImplementedQLException("");

        }

        private Boolean EvaluateHaving(IVertexView myDBObjectReadoutGroup, AExpressionDefinition complexValue, out String attributeName, out ValueDefinition simpleValue)
        {

            //GraphDBType graphDBType = null;
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
                        {
                            throw new NotImplementedQLException("");
                        }

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
                        throw new NotImplementedQLException("");

                    attributeName = func.ChainPartAggregateDefinition.SourceParsedString;
                    //graphDBType = func.ContainingIDNodes.First().LastAttribute.GetDBType(dbContext.DBTypeManager);
                }
                else
                {
                    throw new NotImplementedQLException("");
                }
            }

            if (myDBObjectReadoutGroup.HasProperty(attributeName))
            {
                simpleValue = new ValueDefinition(myDBObjectReadoutGroup.GetProperty<Object>(attributeName));
                return true;
            }

            return false;

        }

        #endregion

        #endregion

        #endregion
    }
}
