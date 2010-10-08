using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;

namespace sones.GraphDB.Warnings
{
    public class Warning_NothingInserted : GraphDBWarning
    {
        public override string ToString()
        {
            return "Nothing has been inserted.";
        }
    }
}
