using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_UnknownDBError : GraphDBError
    {

        public Exception ThrownException { get; private set; }

        public Error_UnknownDBError(Exception myException)
        {
            ThrownException = myException;
        }

        public Error_UnknownDBError(String myErrorMessage)
        {
            ThrownException = new Exception(myErrorMessage);
        }

        public override string ToString()
        {
            if (ThrownException.InnerException != null)
                return String.Format("An unknown GraphDB error occured: {0} Exception: {1} Stacktrace: {2}", Environment.NewLine, ThrownException.InnerException.Message + Environment.NewLine, ThrownException.InnerException.StackTrace);
            else
                return String.Format("An unknown GraphDB error occured: {0} Exception: {1} Stacktrace: {2}", Environment.NewLine, ThrownException.Message + Environment.NewLine, ThrownException.StackTrace);
        }

    }
}
