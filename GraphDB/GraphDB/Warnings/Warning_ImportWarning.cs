using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.Lib;

namespace sones.GraphDB.Warnings
{
    public class Warning_ImportWarning : GraphDBWarning
    {
        public List<IWarning> Warnings { get; private set; }
        public String Query { get; private set; }
        public Int64 Line { get; private set; }

        public Warning_ImportWarning(String myQuery, Int64 myLine)
        {
            //Warnings.AddRange(myWarnings);
            Query = myQuery;
            Line = myLine;
        }

        public override string ToString()
        {
            return String.Concat("Warning(s) for Line ", Line, " Query [", Query, "] ");
        }

    }
}
