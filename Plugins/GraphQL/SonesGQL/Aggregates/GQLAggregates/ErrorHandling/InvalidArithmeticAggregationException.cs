using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.SonesGQL.Aggregates.ErrorHandling
{
    public sealed class InvalidArithmeticAggregationException : ASonesQLAggregateException
    {
        public readonly Type AggregatedType;
        public readonly string Operation;

        public InvalidArithmeticAggregationException(Type myAggregatedType, string myOperation)
        {
            // TODO: Complete member initialization
            AggregatedType = myAggregatedType;
            Operation = myOperation;

            _msg = String.Format("It is not allowed to calculate a {0} aggregate on properties of type {1}.", Operation, AggregatedType);
        }
    }
}
