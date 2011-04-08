using System;
using System.Linq;
using sones.Library.ErrorHandling;
using sones.Library.LanguageExtensions;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The return type of the function is invalid
    /// </summary>
    public sealed class InvalidFunctionReturnTypeException : AGraphQLFunctionException
    {
        public String FunctionName { get; private set; }
        public Type TypeOfFunctionReturn { get; private set; }
        public Type[] ValidTypes { get; private set; }

        /// <summary>
        /// Creates a new InvalidFunctionReturnTypeException exception
        /// </summary>
        /// <param name="myFunctionName">The name of the function</param>
        /// <param name="myTypeOfFunctionReturn">The type of the return value of the function</param>
        /// <param name="myValidTypes">A List of valid types</param>
        public InvalidFunctionReturnTypeException(String myFunctionName, Type myTypeOfFunctionReturn, params Type[] myValidTypes)
        {
            FunctionName = myFunctionName;
            TypeOfFunctionReturn = myTypeOfFunctionReturn;
            ValidTypes = myValidTypes;
        }

        public override string ToString()
        {
            if (ValidTypes.IsNullOrEmpty())
            {
                return String.Format("The return type [{0}] of function [{1}] is not valid.", TypeOfFunctionReturn, FunctionName);
            }
            else
            {
                return String.Format("The return type [{0}] of function [{1}] is not valid. Please choose one of: {2}", TypeOfFunctionReturn, FunctionName, ValidTypes.ToAggregatedString(t => t.Name));
            }
        }

        
    }
}
