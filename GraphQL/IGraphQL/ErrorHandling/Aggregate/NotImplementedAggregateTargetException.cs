using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A type is not implemented for aggregates
    /// </summary>
    public sealed class NotImplementedAggregateTargetException : ASonesQLAggregateException
    {
        public Type AggregateTarget { get; private set; }

        /// <summary>
        /// Creates a new NotImplementedAggregateTargetException exception
        /// </summary>
        /// <param name="myAggregateTarget"></param>
        public NotImplementedAggregateTargetException(Type myAggregateTarget)
        {
            AggregateTarget = myAggregateTarget;
        }

        public override string ToString()
        {
            return String.Format("Currently the type {0} is not implemented for aggregates.", AggregateTarget.Name);
        }

    }
}
