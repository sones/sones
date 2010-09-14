using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_AggregateDoesNotMatchGroupLevel : GraphDBAggregateError
    {
        public String Info { get; private set; }

        public Error_AggregateDoesNotMatchGroupLevel(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return Info;
        }
    }
}
