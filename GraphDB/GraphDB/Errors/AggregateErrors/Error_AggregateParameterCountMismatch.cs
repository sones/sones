using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Functions;
using sones.GraphDB.Aggregates;

namespace sones.GraphDB.Errors
{
    public class Error_AggregateParameterCountMismatch : GraphDBAggregateError
    {

        public Int32 ExpectedParameterCount { get; private set; }
        public Int32 CurrentParameterCount { get; private set; }
        //public ABaseAggregate Aggregate { get; private set; }
        public String Aggregate { get; private set; }

        public Error_AggregateParameterCountMismatch(String myAggregate, Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Aggregate = myAggregate;
        }

        public Error_AggregateParameterCountMismatch(Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Aggregate = null;
        }
        
        public override string ToString()
        {
            if (Aggregate != null)
            {
                return String.Format("The number of parameters [{0}] of the function [{1}]does not match the definition [{2}]", CurrentParameterCount, Aggregate, ExpectedParameterCount);
            }
            else
            {
                return String.Format("The number of parameters [{0}] of the function does not match the definition [{1}]", CurrentParameterCount, ExpectedParameterCount);
            }
        }
    }
}
