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

        public Error_ImportFailed(Exception myException)
        {
            Exception = myException;
        }

        public override string ToString()
        {
            
            return Exception.ToString(true);
        }
    }
}
