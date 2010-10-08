using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidDumpFormat : GraphDBDumpError
    {
        public String DumpFormat { get; private set; }

        public Error_InvalidDumpFormat(String dumpFormat)
        {
            DumpFormat = dumpFormat;
        }

        public override string ToString()
        {
            return DumpFormat;
        }
    }
}
