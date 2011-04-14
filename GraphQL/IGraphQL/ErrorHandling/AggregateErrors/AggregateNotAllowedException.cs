using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// An aggregate is not allowed in a context
    /// </summary>
    public class AggregateNotAllowedException : ASonesQLAggregateException
    {
        public String Aggregate { get; private set; }

        /// <summary>
        /// Creates a new AggregateNotAllowedException exception
        /// </summary>
        /// <param name="myAggregateNode">The aggregate node</param>
        public AggregateNotAllowedException(String myAggregateNode)
        {
            Aggregate = myAggregateNode;
            _msg = String.Format("The aggregate \"{0}\" is not allowed in this context!", Aggregate);
        }
 
    }
}
