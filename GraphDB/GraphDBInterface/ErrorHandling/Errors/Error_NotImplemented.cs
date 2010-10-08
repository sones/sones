using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using System.Diagnostics;

namespace sones.GraphDB.Errors
{
    public class Error_NotImplemented : GraphDBError
    {
        
        public Exception    Exception   { get; private set; }

        public Error_NotImplemented(StackTrace myStackTrace)
        {
            StackTrace  = myStackTrace;
        }

        public Error_NotImplemented(StackTrace myStackTrace, Exception myException)
        {
            StackTrace  = myStackTrace;
            Exception   = myException;
        }

        public Error_NotImplemented(StackTrace myStackTrace, String myMessage)
        {
            StackTrace  = myStackTrace;
            Message = myMessage;
        }

        public override String ToString()
        {
            if (Exception != null)
            {
                return String.Format("{0}" + Environment.NewLine + "Stacktrace" + Environment.NewLine + "{1}" + Environment.NewLine + Environment.NewLine + Message, Exception, StackTrace);
            }
            else
            {
                if (Message != null)
                {
                    return String.Format("{0}\nStacktrace:\n{1}", Message, StackTrace);
                }
                else
                {
                    return String.Format("Stacktrace:\n{0}", StackTrace);

                }
            }
        }


    }

}
