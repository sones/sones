using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The function has a invalid working base
    /// </summary>
    public sealed class InvalidFunctionBaseException : AGraphQLFunctionException
    {
        public String TypeAttribute { get; private set; }
        public String FunctionName { get; private set; }

        /// <summary>
        /// Creates a new InvalidFunctionBaseException exception
        /// </summary>
        /// <param name="myTypeAttribute"></param>
        /// <param name="myFunctionName"></param>
        public InvalidFunctionBaseException(String myTypeAttribute, String myFunctionName)
        {
            TypeAttribute = myTypeAttribute;
            FunctionName = myFunctionName;

            if (TypeAttribute != null)
            {
                _msg = String.Format("The function {0} is invalid on attribute {1}.", FunctionName, TypeAttribute);
            }
            else
            {
                _msg = String.Format("The function {0} has a invalid working base.", FunctionName);
            }
        }
        
    }
}
