using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_NoGroupingArgument : GraphDBSelectError
    {

        public String Selection { get; private set; }

        public Error_NoGroupingArgument(String mySelection)
        {
            Selection = mySelection;
        }

        public override string ToString()
        {
            return "A selection with aggregates must be grouped. Missing for selection " + Selection;
        }
    }
}
