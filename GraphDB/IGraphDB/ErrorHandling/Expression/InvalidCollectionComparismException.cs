using System;
using sones.GraphDB.Expression;

namespace sones.GraphDB.ErrorHandling.Expression
{
    /// <summary>
    /// An invalid collection comparism occured
    /// </summary>
    public sealed class InvalidCollectionComparismException : AGraphDBException
    {
        #region Constructor

        /// <summary>
        /// Creates a new invalid expression exception
        /// </summary>
        /// <param name="myInvalidExpression">The expression that has been declared as invalid</param>
        public InvalidCollectionComparismException(String myInfo)
        {
            _msg = myInfo;
        }

        #endregion
    }
}
