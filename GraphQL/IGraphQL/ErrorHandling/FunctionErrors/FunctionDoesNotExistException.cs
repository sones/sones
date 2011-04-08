using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A function does not exists
    /// </summary>
    public sealed class FunctionDoesNotExistException : AGraphQLFunctionException
    {
        public String FunctionName { get; private set; }

        /// <summary>
        /// Creates a new FunctionDoesNotExistException exception
        /// </summary>
        /// <param name="myFunctionName">The name of the function</param>
        public FunctionDoesNotExistException(String myFunctionName)
        {
            FunctionName = myFunctionName;
        }

        public override string ToString()
        {
            return String.Format("The function \"{0}\" does not exist!", FunctionName);
        }
        
    }
}
