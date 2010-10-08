using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_SelectTimeOut : GraphDBSelectError
    {
        public Int64 TimeOut { get; private set; }

        public Error_SelectTimeOut(Int64 myTimeout)
        {
            TimeOut = myTimeout;
        }

        public override string ToString()
        {
            return String.Format("Aborting query because the timeout of {0}ms has been reached.", TimeOut);
        }
    }
}
