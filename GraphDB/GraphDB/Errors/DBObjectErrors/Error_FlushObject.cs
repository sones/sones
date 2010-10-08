using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Errors
{
    public class Error_FlushObject : GraphDBObjectError
    {
        public ObjectLocation ObjectName { get; private set; }
        public Exception Exception { get; private set; }

        public Error_FlushObject(ObjectLocation myObjectName, Exception myException)
        {
            ObjectName = myObjectName;
            Exception = myException;
        }

        public override string ToString()
        {
            if (Exception != null)
                return String.Format("The object \"{0}\" could not be flushed!{1}Message:{2}{3}StackTrace:{4}", ObjectName, "\n\n", Exception.Message, "\n\n", Exception.StackTrace);
            else
                return String.Format("The object \"{0}\" could not be flushed!");
        }
    }
}
