/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using sones.GraphDB.Expression;
using sones.GraphDB.Expression.QueryPlan;
using sones.GraphDB.Manager.TypeManagement;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Expression.Tree.Literals;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.TypeSystem;
using sones.Constants;

namespace sones.GraphDB.Manager.QueryPlan
{
    /// <summary>
    /// The query plan manager creates a query plan from an expression
    /// </summary>
    public sealed class QueryPlanManager : IQueryPlanManager
    {
        #region data

        /// <summary>
        /// A vertex type manager is needed to create certain query-plan structures
        /// </summary>
        private IManagerOf<ITypeHandler<IVertexType>> _vertexTypeManager;
        /// <summary>
        /// A vertex type manager is needed to create certain query-plan structures
        /// </summary>
        private IManagerOf<ITypeHandler<IEdgeType>> _edgeTypeManager;

        /// <summary>
        /// A vertex store
        /// </summary>
        private IVertexStore _vertexStore;

        /// <summary>
        /// An index manager is needed to create certain query-plan structures
        /// </summary>
        private IIndexManager _indexManager;

        #endregion

        #region IQueryPlanManager Members

        /// <summary>
        /// Creates a query plan using a logic expression
        /// </summary>
        /// <param name="myExpression">The logic expression</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current security token</param>
        /// <returns>A query plan</returns>
        public IQueryPlan CreateQueryPlan(IExpression myExpression, 
                                            Boolean myIsLongRunning, 
                                            Int64 myTransaction, 
                                            SecurityToken mySecurity)
        {
            IQueryPlan result;

            switch (myExpression.TypeOfExpression)
            {
                case TypeOfExpression.Binary:

                    result = GenerateFromBinaryExpression((BinaryExpression) myExpression, myIsLongRunning, myTransaction, mySecurity);

                    break;
                
                case TypeOfExpression.Unary:
                    
                    result = GenerateFromUnaryExpression((UnaryExpression)myExpression, myTransaction, mySecurity);    

                    break;
               
                case TypeOfExpression.Property:

                    result = GenerateFromPropertyExpression((PropertyExpression)myExpression, myTransaction, mySecurity);

                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public bool IsValidExpression(IExpression myExpression)
        {
            switch (myExpression.TypeOfExpression)
            {
                case TypeOfExpression.Binary:

                    return IsValidBinaryExpression((BinaryExpression)myExpression);

                case TypeOfExpression.Unary:

                    return IsValidUnaryExpression((UnaryExpression)myExpression);

                case TypeOfExpression.Constant:
                case TypeOfExpression.Property:
                default:
                    return false;
            }
        }

        #endregion

        #region private helper

        #region IsValidUnaryExpression

        /// <summary>
        /// Is the unary expression valid
        /// </summary>
        /// <param name="unaryExpression">The to be validated expression</param>
        /// <returns>True or false</returns>
        private bool IsValidUnaryExpression(UnaryExpression unaryExpression)
        {
            return IsValidExpression(unaryExpression.Expression);
        }

        #endregion

        #region IsValidBinaryExpression

        /// <summary>
        /// Is this binary expression valid
        /// </summary>
        /// <param name="binaryExpression">The to be validated binary expression</param>
        /// <returns>True or false</returns>
        private bool IsValidBinaryExpression(BinaryExpression myBinaryExpression)
        {
            switch (myBinaryExpression.Operator)
            {
                #region comparative

                case BinaryOperator.Equals:
                case BinaryOperator.Like:
                case BinaryOperator.GreaterOrEqualsThan:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.LessOrEqualsThan:
                case BinaryOperator.LessThan:
                case BinaryOperator.NotEquals:

                    if (myBinaryExpression.Left.TypeOfExpression == TypeOfExpression.Property)
                    {
                        return myBinaryExpression.Right.TypeOfExpression == TypeOfExpression.Constant &&
                                IsValidExpressionIndex(myBinaryExpression);
                    }
                    else
                    {
                        return myBinaryExpression.Left.TypeOfExpression == TypeOfExpression.Constant &&
                                myBinaryExpression.Right.TypeOfExpression == TypeOfExpression.Property &&
                                IsValidExpressionIndex(myBinaryExpression);
                    }

                case BinaryOperator.InRange:

                    if (myBinaryExpression.Left.TypeOfExpression == TypeOfExpression.Property)
                    {
                        return myBinaryExpression.Right.TypeOfExpression == TypeOfExpression.Constant &&
                                myBinaryExpression.Right is RangeLiteralExpression &&
                                IsValidExpressionIndex(myBinaryExpression);
                    }
                    else
                    {
                        return (myBinaryExpression.Left.TypeOfExpression == TypeOfExpression.Constant &&
                                myBinaryExpression.Left is RangeLiteralExpression) &&
                                myBinaryExpression.Right.TypeOfExpression == TypeOfExpression.Property &&
                                IsValidExpressionIndex(myBinaryExpression);
                    }

                #endregion

                #region logic

                case BinaryOperator.AND:
                case BinaryOperator.OR:

                    return IsValidExpression(myBinaryExpression.Left) &&
                            IsValidExpression(myBinaryExpression.Right) &&
                            myBinaryExpression.ExpressionIndex == null;

                #endregion

                default:
                    break;
            }

            return false;
        }

        private bool IsValidExpressionIndex(BinaryExpression myBinaryExpression)
        {
            return myBinaryExpression.ExpressionIndex != null ? 
                    _indexManager.HasIndex(myBinaryExpression.ExpressionIndex) : true;
        }

        #endregion


        /// <summary>
        /// Generates a property query plan
        /// </summary>
        /// <param name="myPropertyExpression">The property expression that is going to be transfered</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current security token</param>
        /// <returns>A property query plan</returns>
        private IQueryPlan GenerateFromPropertyExpression(PropertyExpression myPropertyExpression, 
                                                            Int64 myTransaction, 
                                                            SecurityToken mySecurity)
        {
            var type = _vertexTypeManager.ExecuteManager.GetType(myPropertyExpression.NameOfVertexType, myTransaction, mySecurity);

            return new QueryPlanProperty(type, type.GetPropertyDefinition(myPropertyExpression.NameOfProperty),
                                         myPropertyExpression.Edition, myPropertyExpression.Timespan);
        }

        /// <summary>
        /// Generates a query plan from an unary expression
        /// </summary>
        /// <param name="unaryExpression">The unary expression</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current security token</param>
        /// <returns>A query plan</returns>
        private static IQueryPlan GenerateFromUnaryExpression(UnaryExpression unaryExpression, 
                                                                Int64 myTransaction, 
                                                                SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a query plan from a binary expression
        /// </summary>
        /// <param name="binaryExpression">The binary expression</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A query plan</returns>
        private IQueryPlan GenerateFromBinaryExpression(BinaryExpression myBinaryExpression, 
                                                        Boolean myIsLongRunning, 
                                                        Int64 myTransactionToken, 
                                                        SecurityToken mySecurityToken)
        {
            switch (myBinaryExpression.Operator)
            {
                #region Comparative

                case BinaryOperator.Equals:
                    return GenerateEqualsPlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                case BinaryOperator.GreaterOrEqualsThan:
                    return GenerateGreaterOrEqualsThanPlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                    

                case BinaryOperator.GreaterThan:
                    return GenerateGreaterThanPlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                    

                case BinaryOperator.InRange:
                    return GenerateInRangePlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                    

                case BinaryOperator.LessOrEqualsThan:
                    return GenerateLessOrEqualsThanPlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                                        

                case BinaryOperator.LessThan:
                    return GenerateLessThanPlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                    

                case BinaryOperator.NotEquals:
                    return GenerateNotEqualsPlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                case BinaryOperator.Like:
                    return GenerateLikePlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                #endregion

                #region Logic

                case BinaryOperator.AND:
                    return GenerateANDPlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                case BinaryOperator.OR:
                    return GenerateORPlan(myBinaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                #endregion

                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a plan for an AND operation
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into an AND query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>An AND query plan</returns>
        private IQueryPlan GenerateANDPlan(BinaryExpression myBinaryExpression, 
                                            bool myIsLongRunning, 
                                            Int64 myTransaction,    
                                            SecurityToken mySecurity)
        {
            var left = CreateQueryPlan(myBinaryExpression.Left, myIsLongRunning, myTransaction, mySecurity);
            var right = CreateQueryPlan(myBinaryExpression.Right, myIsLongRunning, myTransaction, mySecurity);

            return new QueryPlanANDSequentiell(left, right, myIsLongRunning);
        }

        /// <summary>
        /// Generates a plan for an OR operation
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into an OR query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>An OR query plan</returns>
        private IQueryPlan GenerateORPlan(BinaryExpression myBinaryExpression, 
                                            bool myIsLongRunning, 
                                            Int64 myTransaction, 
                                            SecurityToken mySecurity)
        {
            var left = CreateQueryPlan(myBinaryExpression.Left, myIsLongRunning, myTransaction, mySecurity);
            var right = CreateQueryPlan(myBinaryExpression.Right, myIsLongRunning, myTransaction, mySecurity);

            return new QueryPlanORSequentiell(left, right, myIsLongRunning);
        }

        /// <summary>
        /// Generats an equals query plan
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into an equals query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>An equals query plan</returns>
        private IQueryPlan GenerateEqualsPlan(BinaryExpression myBinaryExpression, 
                                                Boolean myIsLongRunning, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken)
        {
            //it is not possible to have something complex (User.Age = Car.HorsePower) here --> filtered by validate of IExpression

            #region simple

            //sth like User/Age = 10
            QueryPlanProperty property;
            ILiteralExpression constant;

            FindPropertyAndConstant(myBinaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index specified
            if (myBinaryExpression.ExpressionIndex != null && _indexManager != null)
            {
                return new QueryPlanEqualsWithIndex(mySecurityToken, 
                                                    myTransactionToken, 
                                                    property, 
                                                    constant, 
                                                    _vertexStore, 
                                                    myIsLongRunning, 
                                                    _indexManager,
                                                    myBinaryExpression.ExpressionIndex);
            }
            //is there an index on this property?
            else if (_indexManager != null && _indexManager.HasIndex(property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanEqualsWithIndex(mySecurityToken, 
                                                    myTransactionToken, 
                                                    property, 
                                                    constant, 
                                                    _vertexStore, 
                                                    myIsLongRunning, 
                                                    _indexManager);
            }
            else
            {
                return new QueryPlanEqualsWithoutIndex(mySecurityToken, 
                                                        myTransactionToken, 
                                                        property, 
                                                        constant, 
                                                        _vertexStore, 
                                                        myIsLongRunning);
            }

            #endregion

        }

        private IQueryPlan GenerateInRangePlan(BinaryExpression myBinaryExpression, 
                                                bool myIsLongRunning, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            RangeLiteralExpression constant;

            if (myBinaryExpression.Left is PropertyExpression)
            {
                property = GenerateQueryPlanProperty((PropertyExpression)myBinaryExpression.Left, 
                                                        myTransactionToken, mySecurityToken);

                constant = (RangeLiteralExpression)myBinaryExpression.Right;
            }
            else
            {
                property = GenerateQueryPlanProperty((PropertyExpression)myBinaryExpression.Right, 
                                                        myTransactionToken, mySecurityToken);

                constant = (RangeLiteralExpression)myBinaryExpression.Left;
            }

            //is there an index specified
            if (myBinaryExpression.ExpressionIndex != null && _indexManager != null)
            {
                return new QueryPlanInRangeWithIndex(mySecurityToken,
                                                        myTransactionToken,
                                                        property,
                                                        constant,
                                                        _vertexStore,
                                                        myIsLongRunning,
                                                        _indexManager,
                                                        myBinaryExpression.ExpressionIndex);
            }
            //is there an index on this property?
            else if (_indexManager != null && 
                        _indexManager.HasIndex(property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanInRangeWithIndex(mySecurityToken, 
                                                        myTransactionToken, 
                                                        property, 
                                                        constant, 
                                                        _vertexStore, 
                                                        myIsLongRunning, 
                                                        _indexManager);
            }
            else
            {
                return new QueryPlanInRangeWithoutIndex(mySecurityToken, 
                                                        myTransactionToken, 
                                                        property, 
                                                        constant, 
                                                        _vertexStore, 
                                                        myIsLongRunning);
            }
        }

        /// <summary>
        /// Generats a like query plan
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into a like query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A like query plan</returns>
        private IQueryPlan GenerateLikePlan(BinaryExpression myBinaryExpression, 
                                            bool myIsLongRunning, 
                                            Int64 myTransactionToken, 
                                            SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            ILiteralExpression constant;

            FindPropertyAndConstant(myBinaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //check property
            if (property.Property.BaseType != typeof(String))
            {
                throw new InvalidLikeOperationException(
                            String.Format("The property {0} is not of type String.", 
                                            property.Property.Name));
            }

            //check constant
            if (!(constant.Value is String))
            {
                throw new InvalidLikeOperationException(
                            String.Format("There has to be a String (current: {0}) constant to create a regular expression for a Like operation.", 
                                            constant.Value.GetType().Name));
            }

            //is there an index specified
            if (myBinaryExpression.ExpressionIndex != null && _indexManager != null)
            {
                return new QueryPlanLikeWithIndex(mySecurityToken,
                                                        myTransactionToken,
                                                        property,
                                                        constant,
                                                        _vertexStore,
                                                        myIsLongRunning,
                                                        _indexManager,
                                                        myBinaryExpression.ExpressionIndex);
            }
            //is there an index on this property?
            else if (_indexManager != null && _indexManager.HasIndex(property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanLikeWithIndex(mySecurityToken, 
                                                    myTransactionToken, 
                                                    property, 
                                                    constant, 
                                                    _vertexStore, 
                                                    myIsLongRunning, 
                                                    _indexManager);
            }
            else
            {
                return new QueryPlanLikeWithoutIndex(mySecurityToken, 
                                                        myTransactionToken, 
                                                        property, 
                                                        constant, 
                                                        _vertexStore, 
                                                        myIsLongRunning);
            }
        }

        /// <summary>
        /// Generats a not equals query plan
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into a not equals query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A not equals query plan</returns>
        private IQueryPlan GenerateNotEqualsPlan(BinaryExpression myBinaryExpression, 
                                                    bool myIsLongRunning, 
                                                    Int64 myTransactionToken, 
                                                    SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            ILiteralExpression constant;

            FindPropertyAndConstant(myBinaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index specified
            if (myBinaryExpression.ExpressionIndex != null && _indexManager != null)
            {
                return new QueryPlanNotEqualsWithIndex(mySecurityToken,
                                                        myTransactionToken,
                                                        property,
                                                        constant,
                                                        _vertexStore,
                                                        myIsLongRunning,
                                                        _indexManager,
                                                        myBinaryExpression.ExpressionIndex);
            }
            //is there an index on this property?
            else if (_indexManager != null && _indexManager.HasIndex(property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanNotEqualsWithIndex(mySecurityToken, 
                                                        myTransactionToken, 
                                                        property, 
                                                        constant, 
                                                        _vertexStore, 
                                                        myIsLongRunning, 
                                                        _indexManager);
            }
            else
            {
                return new QueryPlanNotEqualsWithoutIndex(mySecurityToken, 
                                                            myTransactionToken, 
                                                            property, 
                                                            constant, 
                                                            _vertexStore, 
                                                            myIsLongRunning);
            }
        }

        /// <summary>
        /// Generates an less than query plan
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into a less than query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A less than query plan</returns>
        private IQueryPlan GenerateLessThanPlan(BinaryExpression myBinaryExpression, 
                                                bool myIsLongRunning, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            ILiteralExpression constant;

            FindPropertyAndConstant(myBinaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index specified
            if (myBinaryExpression.ExpressionIndex != null && _indexManager != null)
            {
                return new QueryPlanLessThanWithIndex(mySecurityToken,
                                                        myTransactionToken,
                                                        property,
                                                        constant,
                                                        _vertexStore,
                                                        myIsLongRunning,
                                                        _indexManager,
                                                        myBinaryExpression.ExpressionIndex);
            }
            //is there an index on this property?
            else if (_indexManager != null && _indexManager.HasIndex(property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanLessThanWithIndex(mySecurityToken, 
                                                        myTransactionToken, 
                                                        property, 
                                                        constant, 
                                                        _vertexStore, 
                                                        myIsLongRunning, 
                                                        _indexManager);
            }
            else
            {
                return new QueryPlanLessThanWithoutIndex(mySecurityToken, 
                                                            myTransactionToken, 
                                                            property, 
                                                            constant, 
                                                            _vertexStore, 
                                                            myIsLongRunning);
            }
        }

        /// <summary>
        /// Generates a less or equals than query plan
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into a less or equal than query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A less or equal than query plan</returns>
        private IQueryPlan GenerateLessOrEqualsThanPlan(BinaryExpression myBinaryExpression, 
                                                        bool myIsLongRunning, 
                                                        Int64 myTransactionToken, 
                                                        SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            ILiteralExpression constant;

            FindPropertyAndConstant(myBinaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index specified
            if (myBinaryExpression.ExpressionIndex != null && _indexManager != null)
            {
                return new QueryPlanLessOrEqualsThanWithIndex(mySecurityToken,
                                                                myTransactionToken,
                                                                property,
                                                                constant,
                                                                _vertexStore,
                                                                myIsLongRunning,
                                                                _indexManager,
                                                                myBinaryExpression.ExpressionIndex);
            }
            //is there an index on this property?
            else if (_indexManager != null && _indexManager.HasIndex(property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanLessOrEqualsThanWithIndex(mySecurityToken, 
                                                                myTransactionToken, 
                                                                property, 
                                                                constant, 
                                                                _vertexStore, 
                                                                myIsLongRunning, 
                                                                _indexManager);
            }
            else
            {
                return new QueryPlanLessOrEqualsThanWithoutIndex(mySecurityToken, 
                                                                    myTransactionToken, 
                                                                    property, 
                                                                    constant, 
                                                                    _vertexStore, 
                                                                    myIsLongRunning);
            }
        }

        /// <summary>
        /// Generates an greater than query plan
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into a greater than query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A greater than query plan</returns>
        private IQueryPlan GenerateGreaterThanPlan(BinaryExpression myBinaryExpression, 
                                                    bool myIsLongRunning, 
                                                    Int64 myTransactionToken, 
                                                    SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            ILiteralExpression constant;

            FindPropertyAndConstant(myBinaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index specified
            if (myBinaryExpression.ExpressionIndex != null && _indexManager != null)
            {
                return new QueryPlanGreaterThanWithIndex(mySecurityToken,
                                                            myTransactionToken,
                                                            property,
                                                            constant,
                                                            _vertexStore,
                                                            myIsLongRunning,
                                                            _indexManager,
                                                            myBinaryExpression.ExpressionIndex);
            }
            //is there an index on this property?
            else if (_indexManager != null && _indexManager.HasIndex(property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanGreaterThanWithIndex(mySecurityToken, 
                                                            myTransactionToken, 
                                                            property, 
                                                            constant, 
                                                            _vertexStore, 
                                                            myIsLongRunning, 
                                                            _indexManager);
            }
            else
            {
                return new QueryPlanGreaterThanWithoutIndex(mySecurityToken, 
                                                            myTransactionToken, 
                                                            property, 
                                                            constant, 
                                                            _vertexStore, 
                                                            myIsLongRunning);
            }
        }

        /// <summary>
        /// This method extracts the property and the constant out of a binary expression
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myProperty">The property out parameter</param>
        /// <param name="myConstant">The constant out parameter</param>
        private void FindPropertyAndConstant(BinaryExpression myBinaryExpression, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken, 
                                                out QueryPlanProperty myProperty, 
                                                out ILiteralExpression myConstant)
        {
            if (myBinaryExpression.Left is PropertyExpression)
            {
                myProperty = GenerateQueryPlanProperty((PropertyExpression)myBinaryExpression.Left, 
                                                        myTransactionToken, 
                                                        mySecurityToken);

                myConstant = (ILiteralExpression)myBinaryExpression.Right;
            }
            else
            {
                myProperty = GenerateQueryPlanProperty((PropertyExpression)myBinaryExpression.Right, 
                                                        myTransactionToken, 
                                                        mySecurityToken);

                myConstant = (ILiteralExpression)myBinaryExpression.Left;
            }
        }

        /// <summary>
        /// Generates an greater or equals than query plan
        /// </summary>
        /// <param name="myBinaryExpression">The binary expression that has to be transfered into a greater or equals than query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>An greater or equals query plan</returns>
        private IQueryPlan GenerateGreaterOrEqualsThanPlan(BinaryExpression myBinaryExpression, 
                                                            bool myIsLongRunning, 
                                                            Int64 myTransactionToken, 
                                                            SecurityToken mySecurityToken)
        {
            //sth like User/Age = 10
            QueryPlanProperty property;
            ILiteralExpression constant;

            FindPropertyAndConstant(myBinaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index specified
            if (myBinaryExpression.ExpressionIndex != null && _indexManager != null)
            {
                return new QueryPlanGreaterOrEqualsWithIndex(mySecurityToken,
                                                                myTransactionToken,
                                                                property,
                                                                constant,
                                                                _vertexStore,
                                                                myIsLongRunning,
                                                                _indexManager,
                                                                myBinaryExpression.ExpressionIndex);
            }
            //is there an index on this property?
            else if (_indexManager != null && _indexManager.HasIndex(property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanGreaterOrEqualsWithIndex(mySecurityToken, 
                                                                myTransactionToken, 
                                                                property, 
                                                                constant, 
                                                                _vertexStore, 
                                                                myIsLongRunning, 
                                                                _indexManager);
            }
            else
            {
                return new QueryPlanGreaterOrEqualsWithoutIndex(mySecurityToken, 
                                                                myTransactionToken, 
                                                                property, 
                                                                constant, 
                                                                _vertexStore, 
                                                                myIsLongRunning);
            }

        }

        /// <summary>
        /// Generators a property plan
        /// </summary>
        /// <param name="propertyExpression">The property expression that is going to be transfered</param>
        /// <returns>A Property query plan</returns>
        private QueryPlanProperty GenerateQueryPlanProperty(PropertyExpression propertyExpression, 
                                                            Int64 myTransaction, 
                                                            SecurityToken mySecurity)
        {
            IVertexType type = null;

            type = _vertexTypeManager.ExecuteManager.GetType(propertyExpression.NameOfVertexType, myTransaction, mySecurity);

            var property = type.GetPropertyDefinition(propertyExpression.NameOfProperty);

            return new QueryPlanProperty(type, property, propertyExpression.Edition, propertyExpression.Timespan);
        }

        #endregion

        #region IManager Members

        void IManager.Initialize(IMetaManager myMetaManager)
        {
            _vertexTypeManager = myMetaManager.VertexTypeManager;
            _edgeTypeManager = myMetaManager.EdgeTypeManager;
            _vertexStore = myMetaManager.VertexStore;
            _indexManager = myMetaManager.IndexManager;
        }

        void IManager.Load(Int64 myTransaction, SecurityToken mySecurity) 
        { }

        #endregion
    }
}
