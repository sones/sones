using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The desired operator does not exists 
    /// </summary>
    public sealed class OperatorDoesNotExistException : AGraphQLOperatorException
    {
        public String Operator { get; private set; }

        /// <summary>
        /// Creates a new OperatorDoesNotExistException exception
        /// </summary>
        /// <param name="myOperator"></param>
        public OperatorDoesNotExistException(String myOperator)
        {
            Operator = myOperator;
            _msg = String.Format("The operator {0} does not exist.", Operator);
        }
        
    }
}
