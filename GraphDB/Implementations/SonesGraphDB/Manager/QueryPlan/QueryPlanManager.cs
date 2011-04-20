using System;
using sones.GraphDB.Expression;
using sones.GraphDB.Expression.QueryPlan;
using sones.GraphDB.Manager.TypeManagement;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Manager.Index;

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
        private readonly IVertexTypeManager _vertexTypeManager;

        /// <summary>
        /// A vertex store
        /// </summary>
        private readonly IVertexStore _vertexStore;

        /// <summary>
        /// An index manager is needed to create certain query-plan structures
        /// </summary>
        private readonly IIndexManager _indexManager;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new query plan manager
        /// </summary>
        /// <param name="myVertexTypeManager">A vertex type manager is needed to create certain query-plan structures</param>
        /// <param name="myVertexStore">A vertex store</param>
        public QueryPlanManager(IVertexTypeManager myVertexTypeManager, IVertexStore myVertexStore, IIndexManager myIndexManager)
        {
            _vertexTypeManager = myVertexTypeManager;
            _vertexStore = myVertexStore;
            _indexManager = myIndexManager;
        }

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
        public IQueryPlan CreateQueryPlan(IExpression myExpression, Boolean myIsLongRunning, TransactionToken myTransaction, SecurityToken mySecurity)
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
               
                case TypeOfExpression.Constant:

                    result = GenerateFromConstantExpression((ConstantExpression)myExpression);    

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
        private bool IsValidBinaryExpression(BinaryExpression binaryExpression)
        {
            switch (binaryExpression.Operator)
            {
                #region comparative

                case BinaryOperator.Equals:
                case BinaryOperator.GreaterOrEqualsThan:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.LessOrEqualsThan:
                case BinaryOperator.LessThan:
                case BinaryOperator.NotEquals:

                    if (binaryExpression.Left.TypeOfExpression == TypeOfExpression.Property)
                    {
                        return binaryExpression.Right.TypeOfExpression == TypeOfExpression.Constant;
                    }
                    else
                    {
                        return binaryExpression.Left.TypeOfExpression == TypeOfExpression.Constant && binaryExpression.Right.TypeOfExpression == TypeOfExpression.Property;
                    }

                case BinaryOperator.InRange:
                    if (binaryExpression.Left.TypeOfExpression == TypeOfExpression.Property)
                    {
                        return binaryExpression.Right.TypeOfExpression == TypeOfExpression.Constant && binaryExpression.Right is RangeConstantExpression;
                    }
                    else
                    {
                        return (binaryExpression.Left.TypeOfExpression == TypeOfExpression.Constant && binaryExpression.Left is RangeConstantExpression) && binaryExpression.Right.TypeOfExpression == TypeOfExpression.Property;
                    }

                #endregion

                #region logic

                case BinaryOperator.AND:
                case BinaryOperator.OR:

                    return IsValidExpression(binaryExpression.Left) && IsValidExpression(binaryExpression.Right);

                #endregion

                default:
                    break;
            }

            return false;
        }

        #endregion


        /// <summary>
        /// Generates a property query plan
        /// </summary>
        /// <param name="myPropertyExpression">The property expression that is going to be transfered</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current security token</param>
        /// <returns>A property query plan</returns>
        private IQueryPlan GenerateFromPropertyExpression(PropertyExpression myPropertyExpression, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var type = _vertexTypeManager.GetVertexType(myPropertyExpression.NameOfVertexType, myTransaction, mySecurity);

            return new QueryPlanProperty(type, type.GetPropertyDefinition(myPropertyExpression.NameOfProperty),
                                         myPropertyExpression.Edition, myPropertyExpression.Timespan);
        }

        /// <summary>
        /// Generates a constant
        /// </summary>
        /// <param name="constantExpression">The constant expression that is going to be transfered</param>
        /// <returns></returns>
        private static IQueryPlan GenerateFromConstantExpression(ConstantExpression constantExpression)
        {
            return new QueryPlanConstant(constantExpression.Constant);
        }

        /// <summary>
        /// Generates a query plan from an unary expression
        /// </summary>
        /// <param name="unaryExpression">The unary expression</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current security token</param>
        /// <returns>A query plan</returns>
        private static IQueryPlan GenerateFromUnaryExpression(UnaryExpression unaryExpression, TransactionToken myTransaction, SecurityToken mySecurity)
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
        private IQueryPlan GenerateFromBinaryExpression(BinaryExpression binaryExpression, Boolean myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            switch (binaryExpression.Operator)
            {
                #region Comparative

                case BinaryOperator.Equals:
                    return GenerateEqualsPlan(binaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                case BinaryOperator.GreaterOrEqualsThan:
                    return GenerateGreaterOrEqualsThanPlan(binaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                    

                case BinaryOperator.GreaterThan:
                    return GenerateGreaterThanPlan(binaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                    

                case BinaryOperator.InRange:
                    break;
                case BinaryOperator.LessOrEqualsThan:
                    return GenerateLessOrEqualsThanPlan(binaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                                        

                case BinaryOperator.LessThan:
                    return GenerateLessThanPlan(binaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);                    

                case BinaryOperator.NotEquals:
                    return GenerateNotEqualsPlan(binaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                #endregion

                #region Logic

                case BinaryOperator.AND:
                    return GenerateANDPlan(binaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                case BinaryOperator.OR:
                    return GenerateORPlan(binaryExpression, myIsLongRunning, myTransactionToken, mySecurityToken);

                #endregion

                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a plan for an AND operation
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into an AND query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>An AND query plan</returns>
        private IQueryPlan GenerateANDPlan(BinaryExpression binaryExpression, bool myIsLongRunning, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var left = CreateQueryPlan(binaryExpression.Left, myIsLongRunning, myTransaction, mySecurity);
            var right = CreateQueryPlan(binaryExpression.Right, myIsLongRunning, myTransaction, mySecurity);

            return new QueryPlanANDSequentiell(left, right, myIsLongRunning);
        }

        /// <summary>
        /// Generates a plan for an OR operation
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into an OR query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>An OR query plan</returns>
        private IQueryPlan GenerateORPlan(BinaryExpression binaryExpression, bool myIsLongRunning, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var left = CreateQueryPlan(binaryExpression.Left, myIsLongRunning, myTransaction, mySecurity);
            var right = CreateQueryPlan(binaryExpression.Right, myIsLongRunning, myTransaction, mySecurity);

            return new QueryPlanORSequentiell(left, right, myIsLongRunning);
        }

        /// <summary>
        /// Generats an equals query plan
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into an equals query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>An equals query plan</returns>
        private IQueryPlan GenerateEqualsPlan(BinaryExpression binaryExpression, Boolean myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            //it is not possible to have something complex (User.Age = Car.HorsePower) here --> filtered by validate of IExpression

            #region simple

            //sth like User/Age = 10
            QueryPlanProperty property;
            QueryPlanConstant constant;

            FindPropertyAndConstant(binaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index on this property?
            if (_indexManager.HasIndex(property.VertexType, property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanEqualsWithIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning, _indexManager);
            }
            else
            {
                return new QueryPlanEqualsWithoutIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning);
            }

            #endregion

        }

        /// <summary>
        /// Generats a not equals query plan
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into a not equals query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A not equals query plan</returns>
        private IQueryPlan GenerateNotEqualsPlan(BinaryExpression binaryExpression, bool myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            QueryPlanConstant constant;

            FindPropertyAndConstant(binaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index on this property?
            if (_indexManager.HasIndex(property.VertexType, property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanNotEqualsWithIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning, _indexManager);
            }
            else
            {
                return new QueryPlanNotEqualsWithoutIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning);
            }
        }

        /// <summary>
        /// Generates an less than query plan
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into a less than query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A less than query plan</returns>
        private IQueryPlan GenerateLessThanPlan(BinaryExpression binaryExpression, bool myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            QueryPlanConstant constant;

            FindPropertyAndConstant(binaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index on this property?
            if (_indexManager.HasIndex(property.VertexType, property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanLessThanWithIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning, _indexManager);
            }
            else
            {
                return new QueryPlanLessThanWithoutIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning);
            }
        }

        /// <summary>
        /// Generates a less or equals than query plan
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into a less or equal than query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A less or equal than query plan</returns>
        private IQueryPlan GenerateLessOrEqualsThanPlan(BinaryExpression binaryExpression, bool myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            QueryPlanConstant constant;

            FindPropertyAndConstant(binaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index on this property?
            if (_indexManager.HasIndex(property.VertexType, property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanLessOrEqualsThanWithIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning, _indexManager);
            }
            else
            {
                return new QueryPlanLessOrEqualsThanWithoutIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning);
            }
        }

        /// <summary>
        /// Generates an greater than query plan
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into a greater than query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A greater than query plan</returns>
        private IQueryPlan GenerateGreaterThanPlan(BinaryExpression binaryExpression, bool myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            QueryPlanProperty property;
            QueryPlanConstant constant;

            FindPropertyAndConstant(binaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index on this property?
            if (_indexManager.HasIndex(property.VertexType, property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanGreaterThanWithIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning, _indexManager);
            }
            else
            {
                return new QueryPlanGreaterThanWithoutIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning);
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
        private void FindPropertyAndConstant(BinaryExpression myBinaryExpression, TransactionToken myTransactionToken, SecurityToken mySecurityToken, out QueryPlanProperty myProperty, out QueryPlanConstant myConstant)
        {
            if (myBinaryExpression.Left is PropertyExpression)
            {
                myProperty = GenerateQueryPlanProperty((PropertyExpression)myBinaryExpression.Left, myTransactionToken, mySecurityToken);

                myConstant = new QueryPlanConstant(((ConstantExpression)myBinaryExpression.Right).Constant);
            }
            else
            {
                myProperty = GenerateQueryPlanProperty((PropertyExpression)myBinaryExpression.Right, myTransactionToken, mySecurityToken);

                myConstant = new QueryPlanConstant(((ConstantExpression)myBinaryExpression.Left).Constant);
            }
        }

        /// <summary>
        /// Generates an greater or equals than query plan
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into a greater or equals than query plan</param>
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>An greater or equals query plan</returns>
        private IQueryPlan GenerateGreaterOrEqualsThanPlan(BinaryExpression binaryExpression, bool myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            //sth like User/Age = 10
            QueryPlanProperty property;
            QueryPlanConstant constant;

            FindPropertyAndConstant(binaryExpression, myTransactionToken, mySecurityToken, out property, out constant);

            //is there an index on this property?
            if (_indexManager.HasIndex(property.VertexType, property.Property, mySecurityToken, myTransactionToken))
            {
                return new QueryPlanGreaterOrEqualsWithIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning, _indexManager);
            }
            else
            {
                return new QueryPlanGreaterOrEqualsWithoutIndex(mySecurityToken, myTransactionToken, property, constant, _vertexStore, myIsLongRunning);
            }

        }

        /// <summary>
        /// Generators a property plan
        /// </summary>
        /// <param name="propertyExpression">The property expression that is going to be transfered</param>
        /// <returns>A Property query plan</returns>
        private QueryPlanProperty GenerateQueryPlanProperty(PropertyExpression propertyExpression, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertexType = _vertexTypeManager.GetVertexType(propertyExpression.NameOfVertexType, myTransaction, mySecurity);
            var property = vertexType.GetPropertyDefinition(propertyExpression.NameOfProperty);

            return new QueryPlanProperty(vertexType, property, propertyExpression.Edition, propertyExpression.Timespan);
        }

        #endregion
    }
}
