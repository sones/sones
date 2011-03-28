using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// An aggregate or function is duplicate
    /// </summary>
    public sealed class DuplicateAggregateOrFunctionException : AGraphDBException
    {
        #region data
        
        public String FunctionName { get; private set; }
        public Boolean IsFunction { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new DuplicateAggregateOrFunctionException exception
        /// </summary>
        /// <param name="myFunctionName">The function name</param>
        /// <param name="myIsFunction">Truth value of is a function</param>
        public DuplicateAggregateOrFunctionException(String myFunctionName, Boolean myIsFunction = true)
        {
            FunctionName = myFunctionName;
            IsFunction   = myIsFunction;
        }

        #endregion

        public override string ToString()
        {
            if (IsFunction)
            {
                return String.Format("The function name \"{0}\" is duplicate! The name has to be unique!", FunctionName);
            }
            else
            {
                return String.Format("The aggregate name \"{0}\" is duplicate! The name has to be unique!", FunctionName);
            }
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.DuplicateAggregateOrFunction; }
        }
    }
}
