using System;
using sones.GraphDB.Expression;

namespace sones.GraphDB.ErrorHandling.Expression
{
    /// <summary>
    /// An invalid expression occured
    /// </summary>
    public sealed class InvalidExpressionException : AGraphDBException
    {
        #region data

        /// <summary>
        /// The expression that has been declared as invalid
        /// </summary>
        public readonly IExpression InvalidExpression;

        String VertexTypeName;
        long VertexID;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new invalid expression exception
        /// </summary>
        /// <param name="myInvalidExpression">The expression that has been declared as invalid</param>
        public InvalidExpressionException(IExpression myInvalidExpression)
        {
            InvalidExpression = myInvalidExpression;
        }

        public InvalidExpressionException(String myVertexTypeName, long myVertexID)
        {
            VertexTypeName = myVertexTypeName;
            VertexID = myVertexID;
            InvalidExpression = null;
        }

        #endregion

        public override string ToString()
        {
            if(InvalidExpression != null)
                return String.Format("The expression {0} is invalid.", InvalidExpression);
            else
                return String.Format("The expression or VertexTypeName and VertexID must be set");
        }
    }
}
