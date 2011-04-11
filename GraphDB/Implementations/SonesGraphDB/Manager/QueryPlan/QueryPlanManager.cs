using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;
using sones.GraphDB.Expression.QueryPlan;
using sones.GraphDB.Manager.TypeManagement;

namespace sones.GraphDB.Manager.QueryPlan
{
    /// <summary>
    /// The query plan manager creates a query plan from an expression
    /// </summary>
    public sealed class QueryPlanManager : IQueryPlanManager
    {
        #region data

        /// <summary>
        /// A type manager is needed to create certain query-plan structures
        /// </summary>
        private readonly IVertexTypeManager _vertexTypeManager;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new query plan manager
        /// </summary>
        /// <param name="myTypeManager"></param>
        public QueryPlanManager(IVertexTypeManager myTypeManager)
        {
            _vertexTypeManager = myTypeManager;
        }

        #endregion

        #region IQueryPlanManager Members

        public IQueryPlan CreateQueryPlan(IExpression myExpression)
        {
            IQueryPlan result;

            switch (myExpression.TypeOfExpression)
            {
                case TypeOfExpression.Binary:

                    result = GenerateFromBinaryExpression((BinaryExpression) myExpression);

                    break;
                
                case TypeOfExpression.Unary:
                    
                    result = GenerateFromUnaryExpression((UnaryExpression)myExpression);    

                    break;
               
                case TypeOfExpression.Constant:

                    result = GenerateFromConstantExpression((ConstantExpression)myExpression);    

                    break;
                
                case TypeOfExpression.Property:

                    result = GenerateFromPropertyExpression((PropertyExpression)myExpression);

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
        /// <returns>A property query plan</returns>
        private IQueryPlan GenerateFromPropertyExpression(PropertyExpression myPropertyExpression)
        {
            var type = _vertexTypeManager.GetVertexType(myPropertyExpression.NameOfVertexType);

            return new QueryPlanProperty(type, type.GetPropertyDefinition(myPropertyExpression.NameOfProperty));
        }

        /// <summary>
        /// Generates a constant
        /// </summary>
        /// <param name="constantExpression">The constant expression that is going to be transfered</param>
        /// <returns></returns>
        private IQueryPlan GenerateFromConstantExpression(ConstantExpression constantExpression)
        {
            return new QueryPlanConstant(constantExpression.Constant);
        }

        /// <summary>
        /// Generates a query plan from an unary expression
        /// </summary>
        /// <param name="unaryExpression">The unary expression</param>
        /// <returns>A query plan</returns>
        private IQueryPlan GenerateFromUnaryExpression(UnaryExpression unaryExpression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a query plan from a binary expression
        /// </summary>
        /// <param name="binaryExpression">The binary expression</param>
        /// <returns>A query plan</returns>
        private IQueryPlan GenerateFromBinaryExpression(BinaryExpression binaryExpression)
        {
            switch (binaryExpression.Operator)
            {
                #region Comparative

                case BinaryOperator.Equals:
                    return GenerateEqualsPlan(binaryExpression);

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
                    break;
                case BinaryOperator.OR:
                    break;

                #endregion

                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generats an equals query plan
        /// </summary>
        /// <param name="binaryExpression">The binary expression that has to be transfered into an equals query plan</param>
        /// <returns>An equals query plan</returns>
        private IQueryPlan GenerateEqualsPlan(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left is PropertyExpression && binaryExpression.Right is PropertyExpression)
            {
                #region complex

                //complex sth like User/Age = Car/PS

                return new QueryPlanEqualsWithoutIndexComplex(GenerateQueryPlanProperty((PropertyExpression)binaryExpression.Left), GenerateQueryPlanProperty((PropertyExpression)binaryExpression.Right));

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
                    property = GenerateQueryPlanProperty((PropertyExpression)binaryExpression.Left);

                    constant = new QueryPlanConstant(((ConstantExpression)binaryExpression.Right).Constant);
                }
                else
                {
                    property = GenerateQueryPlanProperty((PropertyExpression)binaryExpression.Right);

                    constant = new QueryPlanConstant(((ConstantExpression)binaryExpression.Left).Constant);
                }

                return new QueryPlanEqualsWithoutIndex(property, constant);

                #endregion
            }
        }

        /// <summary>
        /// Generators a property plan
        /// </summary>
        /// <param name="propertyExpression">The property expression that is going to be transfered</param>
        /// <returns>A Property query plan</returns>
        private QueryPlanProperty GenerateQueryPlanProperty(PropertyExpression propertyExpression)
        {
            var vertexType = _vertexTypeManager.GetVertexType(propertyExpression.NameOfVertexType);
            var property = vertexType.GetPropertyDefinition(propertyExpression.NameOfProperty);

            return new QueryPlanProperty(vertexType, property);
        }

        #endregion
    }
}
