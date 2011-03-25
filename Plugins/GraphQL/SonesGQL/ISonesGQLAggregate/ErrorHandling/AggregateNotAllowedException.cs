using System;
using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.Aggregates.ErrorHandling
{
    /// <summary>
    /// An aggregate is not allowed in a context
    /// </summary>
    public sealed class AggregateNotAllowedException : ASonesQLAggregateException
    {
        public ChainPartFuncDefinition Aggregate { get; private set; }

        /// <summary>
        /// Creates a new AggregateNotAllowedException exception
        /// </summary>
        /// <param name="myAggregateNode"></param>
        public AggregateNotAllowedException(ChainPartFuncDefinition myAggregateNode)
        {
            Aggregate = myAggregateNode;
        }

        public override string ToString()
        {
            return String.Format("The aggregate \"{0}\" is not allowed in this context!", Aggregate);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.AggregateNotAllowed; }
        }  
    }
}
