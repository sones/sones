using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Errors
{
    public class Error_NotImplementedAggregateTarget : GraphDBAggregateError
    {
        public Type AggregateTarget { get; private set; }

        public Error_NotImplementedAggregateTarget(Type myAggregateTarget)
        {
            AggregateTarget = myAggregateTarget;
        }

        public override string ToString()
        {
            return String.Format("Currently the type {0} is not implemented for aggregates.", AggregateTarget.Name);
        }
    }
}
