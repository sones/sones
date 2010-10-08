using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Errors
{
    public class Error_LoadObject : GraphDBObjectError
    {
        public ObjectLocation ObjectName { get; private set; }
        public Exception Exception { get; private set; }

        public Error_LoadObject(ObjectLocation myObjectName)
        {
            ObjectName = myObjectName;
        }

        public Error_LoadObject(ObjectLocation myObjectName, Exception myException)
        {
            ObjectName = myObjectName;
            Exception = myException;
        }

        public override string ToString()
        {
            if (Exception != null)
            {
                return String.Format("The object {0} could not be loaded.\nException:\n{1}", ObjectName, Exception);
            }
            else
            {
                return String.Format("The object {0} could not be loaded.", ObjectName);
            }
        }
    }
}
