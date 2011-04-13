using System;

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
            _msg = String.Format("Currently the type {0} is not implemented for aggregates.", AggregateTarget.Name);
        }

    }
}
