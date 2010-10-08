using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ExporterDoesNotExist : GraphDBError
    {
        public String ExportFormat { get; private set; }

        public Error_ExporterDoesNotExist(String myExportFormat)
        {
            ExportFormat = myExportFormat;
        }

        public override string ToString()
        {
            return ExportFormat;
        }
    }
}
