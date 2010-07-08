using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib;
using Newtonsoft.Json.Linq;

namespace sones.GraphDB.Errors
{
    public class Error_ImportFailed : GraphDBError
    {
        public Exception Exception { get; private set; }
        public String Query        { get; private set; }
        public Int64 Line          { get; private set; }

        public Error_ImportFailed(Exception myException)
        {
            Exception = myException;
        }

        public Error_ImportFailed(String myQuery, Int64 myLine)
        {
            Query = myQuery;
            Line = myLine;
        }

        public override string ToString()
        {
            if (Exception != null)
            {
                return Exception.ToString(true);
            }
            else
            {
                return String.Format("Line: [{0}] Query: " + Query, Line);
            }
        }
    }
}
