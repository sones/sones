using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// An invalid reference for a function parameter
    /// </summary>
    public sealed class FunctionParameterInvalidReferenceException : AGraphQLFunctionException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new FunctionParameterInvalidReferenceException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public FunctionParameterInvalidReferenceException(String myInfo)
        {
            Info = myInfo;
            _msg = String.Format("An invalid reference for a function parameter: {0}! ", Info);
        }
         
    }
}
