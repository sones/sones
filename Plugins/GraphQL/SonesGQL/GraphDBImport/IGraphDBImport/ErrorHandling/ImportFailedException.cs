using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.SonesGQL.DBImport.ErrorHandling
{
    public sealed class ImportFailedException : ASonesQLGraphDBImportException
    {
        public Exception Exception { get; private set; }
        public String Query        { get; private set; }
        public Int64 Line          { get; private set; }

        public ImportFailedException(Exception myException)
        {
            Exception = myException;
        }

        public ImportFailedException(String myQuery, Int64 myLine)
        {
            Query = myQuery;
            Line = myLine;
        }

        public override string ToString()
        {
            if (Exception != null)
            {
                return Exception.ToString();
            }
            else
            {
                return String.Format("Line: [{0}] Query: " + Query, Line);
            }
        }
    }
}
