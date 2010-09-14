using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.Errors
{
    public class Error_AggregateNotAllowed : GraphDBAggregateError
    {
        public ChainPartFuncDefinition Aggregate { get; private set; }

        public Error_AggregateNotAllowed(ChainPartFuncDefinition myAggregateNode)
        {
            Aggregate = myAggregateNode;
        }

        public override string ToString()
        {
            return String.Format("The aggregate \"{0}\" is not allowed in this context!", Aggregate);
        }
    }
}
