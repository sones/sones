using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ImporterDoesNotExist : GraphDBError
    {
        public String ImportFormat { get; private set; }

        public Error_ImporterDoesNotExist(String myImportFormat)
        {
            ImportFormat = myImportFormat;
        }

        public override string ToString()
        {
            return ImportFormat;
        }
    }
}
