using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_AggregateOrFunctionDoesNotExist : GraphDBAggregateError
    {
        public String AggregateOrFunctionName { get; private set; }

        public Error_AggregateOrFunctionDoesNotExist(String myAggregateOrFunctionName)
        {
            AggregateOrFunctionName = myAggregateOrFunctionName;
        }

        public override string ToString()
        {
            return String.Format("The aggregate or function \"{0}\" does not exist!", AggregateOrFunctionName);
        }
    }
}
