using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace sones.GraphDB.Errors
{
    public class Error_ExpressionGraphInternal : GraphDBError
    {
        public String Info { get; private set; }

        public Error_ExpressionGraphInternal(StackTrace myStacktrace, String myInfo)
        {
            Info = myInfo;
            StackTrace = myStacktrace;
        }

        public override string ToString()
        {
            return String.Format("An internal ExpressionGraph error occurred: \"{0}\"\nStacktrace:\n{1}", Info, StackTrace);
        }
    }
}
