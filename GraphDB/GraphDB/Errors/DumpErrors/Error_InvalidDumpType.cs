using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidDumpType : GraphDBDumpError
    {
        public String DumpType { get; private set; }

        public Error_InvalidDumpType(String dumpType)
        {
            DumpType = dumpType;
        }

        public override string ToString()
        {
            return DumpType;
        }
    }
}
