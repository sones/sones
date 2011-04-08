using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// An aggregate or function does not exist
    /// </summary>
    public sealed class AggregateOrFunctionDoesNotExistException : AGraphDBException
    {
        public String AggregateOrFunctionName { get; private set; }

        /// <summary>
        /// Creates a new AggregateOrFunctionDoesNotExistException exception
        /// </summary>
        /// <param name="myAggregateOrFunctionName"></param>
        public AggregateOrFunctionDoesNotExistException(String myAggregateOrFunctionName)
        {
            AggregateOrFunctionName = myAggregateOrFunctionName;

            _errorCode = ErrorCodes.AggregateOrFunctionDoesNotExist;
            _msg = String.Format("{0} : The aggregate or function \"{1}\" does not exist!", _errorCode, AggregateOrFunctionName);            
        }        

        public override ushort ErrorCode
        {
            get { return _errorCode; }
        }                 
    }
}
