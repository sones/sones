using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request.Helper.Operator.Logic;
using sones.GraphDB.Request.Helper.Operator;

namespace sones.GraphDB.Request.Helper.Expression
{
    /// <summary>
    /// An unary expression has a unary operator and a single expression
    /// </summary>
    public sealed class UnaryExpression : IExpression
    {
        #region data

        /// <summary>
        /// The unary operator
        /// </summary>
        public readonly IUnaryOperator Operator;

        /// <summary>
        /// The single expression
        /// </summary>
        public readonly IExpression Expression;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new unary expression
        /// </summary>
        /// <param name="myUnaryOperator">The unary operator</param>
        /// <param name="myUnaryExpression">The expression</param>
        public UnaryExpression(IUnaryOperator myUnaryOperator, IExpression myUnaryExpression)
        {
            Operator = myUnaryOperator;
            Expression = myUnaryExpression;
        }

        #endregion
    }
}
