using System;

namespace sones.GraphDB.Expression
{
    /// <summary>
    /// A constant expression
    /// </summary>
    public sealed class ConstantExpression : IExpression
    {
        #region data

        /// <summary>
        /// A constant expression... sth like 13 or "Alice"
        /// </summary>
        public readonly IComparable Constant;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new constant expression
        /// </summary>
        /// <param name="myConstant">The constant expression</param>
        public ConstantExpression(IComparable myConstant)
        {
            Constant = myConstant;
        }

        #endregion

        #region IExpression Members

        public TypeOfExpression TypeOfExpression
        {
            get { return TypeOfExpression.Constant; }
        }

        #endregion
    }
}
