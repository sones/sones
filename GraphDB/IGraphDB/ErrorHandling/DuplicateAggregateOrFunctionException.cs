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

            if (IsFunction)
            {
                _msg = String.Format("{0} : The function name \"{1}\" is duplicate! The name has to be unique!", FunctionName);
            }

            else
            {
                _msg = String.Format("{0} : The aggregate name \"{1}\" is duplicate! The name has to be unique!", FunctionName);
            }
        }

        #endregion
                
                
    }
}
