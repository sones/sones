using System;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.Expression.Tree.Literals;

namespace sones.GraphDB.Expression
{
    /// <summary>
    /// A constant expression for range-definition
    /// </summary>
    public sealed class RangeLiteralExpression : IExpression
    {
        #region data

        /// <summary>
        /// A constant expression... sth like 13 or "Alice"
        /// The lower limit for the range
        /// </summary>
        public readonly IComparable Lower;

        /// <summary>
        /// A constant expression... sth like 13 or "Alice"
        /// The upper limit for the range
        /// </summary>
        public readonly IComparable Upper;

        /// <summary>
        /// Include the upper and lower boarder?
        /// </summary>
        public readonly Boolean IncludeBorders;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new constant range expression
        /// </summary>
        /// <param name="myLower">The lower limit for the range</param>
        /// <param name="myUpper">The upper limit for the range</param>
        /// <param name="myIncludeBorder">Include the upper and lower boarder?</param>
        public RangeLiteralExpression(IComparable myLower, IComparable myUpper, Boolean myIncludeBorder = true)
        {
            Lower = myLower;
            Upper = myUpper;
            IncludeBorders = myIncludeBorder;
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
