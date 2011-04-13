using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VertexStore;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.TypeManagement;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.Library.Transaction;
using sones.Library.Security;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Manager.QueryPlan;

namespace sones.GraphDB.Manager.Vertex
{

    /// <summary>
    /// This manager is responsible for getting (chosen) vertices from the persistence layer
    /// </summary>
    public sealed class VertexManager : IVertexManager
    {
        #region data

        /// <summary>
        /// Needed for getting vertices from the persistence layer
        /// </summary>
        private readonly IVertexStore _vertexStore;

        /// <summary>
        /// Needed for index interaction
        /// </summary>
        private readonly IIndexManager _indexManager;

        /// <summary>
        /// Needed for VertexType interaction
        /// </summary>
        private readonly IVertexTypeManager _vertexTypeManager;

        /// <summary>
        /// Needed for transforming an expression into a query plan
        /// </summary>
        private readonly IQueryPlanManager _queryPlanManager;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new vertex manaager
        /// </summary>
        /// <param name="myVertexStore">Interface to the persistence layer</param>
        /// <param name="myVertexTypeManager">Interface to vertex types</param>
        /// <param name="myIndexManager">Interface to indices</param>
        public VertexManager(IVertexStore myVertexStore, IVertexTypeManager myVertexTypeManager, IIndexManager myIndexManager)
        {
            _vertexStore = myVertexStore;
            _indexManager = myIndexManager;
            _vertexTypeManager = myVertexTypeManager;

            _queryPlanManager = new QueryPlanManager(_vertexTypeManager);
        }

        #endregion

        #region IVertexManager Members

        public IEnumerable<IVertex> GetVertices(IExpression myExpression, bool myIsLongrunning, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void CanGetVertices(IExpression iExpression, bool p, TransactionToken TransactionToken, SecurityToken SecurityToken)
        {
            if (!IsValidExpression(iExpression))
                throw new InvalidExpressionException(iExpression);
        }

        #endregion

        #region private helper

        #region IsValidExpression

        /// <summary>
        /// Is the expression valid
        /// </summary>
        /// <param name="myExpression">The to be validated expression</param>
        /// <returns>True or false</returns>
        private bool IsValidExpression(IExpression myExpression)
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
                case BinaryOperator.In:
                case BinaryOperator.InRange:
                case BinaryOperator.LessOrEqualsThan:
                case BinaryOperator.LessThan:
                case BinaryOperator.NotEquals:
                case BinaryOperator.NotIn:

                    if (binaryExpression.Left is PropertyExpression)
                    {
                        return (binaryExpression.Right is PropertyExpression) || (binaryExpression.Right is ConstantExpression);
                    }
                    else
                    {
                        return binaryExpression.Left is ConstantExpression && binaryExpression.Right is PropertyExpression;
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

        #endregion
    }
}
