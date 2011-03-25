using System;
using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.Aggregates.ErrorHandling
{
    public sealed class AggregateOrFunctionDoesNotExistException : ASonesQLAggregateException
    {
        public String AggregateOrFunctionName { get; private set; }

        /// <summary>
        /// Creates a new AggregateOrFunctionDoesNotExistException exception
        /// </summary>
        /// <param name="myAggregateOrFunctionName"></param>
        public AggregateOrFunctionDoesNotExistException(String myAggregateOrFunctionName)
        {
            AggregateOrFunctionName = myAggregateOrFunctionName;
        }

        public override string ToString()
        {
            return String.Format("The aggregate or function \"{0}\" does not exist!", AggregateOrFunctionName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.AggregateOrFunctionDoesNotExist; }
        }   
    }
}
