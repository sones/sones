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

        #endregion

        #region private helper

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
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current security token</param>
        /// <returns>A query plan</returns>
        private IQueryPlan GenerateFromBinaryExpression(BinaryExpression binaryExpression, Boolean myIsLongRunning, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            switch (binaryExpression.Operator)
            {
                #region Comparative

                case BinaryOperator.Equals:
                    return GenerateEqualsPlan(binaryExpression, myIsLongRunning, myTransaction, mySecurity);

                case BinaryOperator.GreaterOrEqualsThan:
                    break;
                case BinaryOperator.GreaterThan:
                    break;
                case BinaryOperator.In:
                    break;
                case BinaryOperator.InRange:
                    break;
                case BinaryOperator.LessOrEqualsThan:
                    break;
                case BinaryOperator.LessThan:
                    break;
                case BinaryOperator.NotEquals:
                    break;
                case BinaryOperator.NotIn:
                    break;

                #endregion

                #region Logic

                case BinaryOperator.AND:
                    return GenerateANDPlan(binaryExpression, myIsLongRunning, myTransaction, mySecurity);

                case BinaryOperator.OR:
                    return GenerateORPlan(binaryExpression, myIsLongRunning, myTransaction, mySecurity);

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
            if (binaryExpression.Left is PropertyExpression && binaryExpression.Right is PropertyExpression)
            {
                #region complex

                //complex sth like User/Age = Car/PS

                return new QueryPlanEqualsWithoutIndexComplex(
                    GenerateQueryPlanProperty((PropertyExpression)binaryExpression.Left, myTransactionToken, mySecurityToken), 
                    GenerateQueryPlanProperty((PropertyExpression)binaryExpression.Right, myTransactionToken, mySecurityToken));

                #endregion
            }
            else
            {
                #region simple

                //sth like User/Age = 10
                QueryPlanProperty property;
                QueryPlanConstant constant;

                if (binaryExpression.Left is PropertyExpression)
                {
                    property = GenerateQueryPlanProperty((PropertyExpression)binaryExpression.Left, myTransactionToken, mySecurityToken);

                    constant = new QueryPlanConstant(((ConstantExpression)binaryExpression.Right).Constant);
                }
                else
                {
                    property = GenerateQueryPlanProperty((PropertyExpression)binaryExpression.Right, myTransactionToken, mySecurityToken);

                    constant = new QueryPlanConstant(((ConstantExpression)binaryExpression.Left).Constant);
                }

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
