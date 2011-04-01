using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public readonly Object Constant;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new constant expression
        /// </summary>
        /// <param name="myConstant">The constant expression</param>
        public ConstantExpression(Object myConstant)
        {
            Constant = myConstant;
        }

        #endregion

    }
}
