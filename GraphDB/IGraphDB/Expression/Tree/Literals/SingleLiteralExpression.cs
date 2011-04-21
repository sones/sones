using System;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.Expression.Tree.Literals;

namespace sones.GraphDB.Expression
{
    /// <summary>
    /// A constant expression
    /// </summary>
    public sealed class SingleLiteralExpression : IExpression, ILiteralExpression
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
        public SingleLiteralExpression(IComparable myConstant)
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

        #region ILiteralExpression Members

        public IComparable Value
        {
            get { return Constant; }
        }

        #endregion
    }
}
