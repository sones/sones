using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidInRangeInterval : GraphDBOperatorError
    {
        public Int32 Expected { get; private set; }
        public Int32 Current { get; private set; }

        public Error_InvalidInRangeInterval(Int32 expected, Int32 current)
        {
            Expected = expected;
            Current = current;
        }

        public override string ToString()
        {
            return String.Format("Expected: \"{0}\" Current: \"{1}\"", Expected, Current);
        }
    }
}
