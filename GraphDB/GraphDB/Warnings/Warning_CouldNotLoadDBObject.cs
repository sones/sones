using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Warnings
{
    public class Warning_CouldNotLoadDBObject : GraphDBWarning
    {
        public IEnumerable<IError> Errors { get; private set; }
        public System.Diagnostics.StackTrace Stacktrace { get; private set; }        

        public Warning_CouldNotLoadDBObject(IEnumerable<IError> myErrors, System.Diagnostics.StackTrace myStackTrace)
        {
            Errors = myErrors;
            Stacktrace = myStackTrace;
        }

        public override string ToString()
        {
            String ErrorString = "";
            foreach (var aError in Errors)
            {
                ErrorString += aError.ToString() + Environment.NewLine;
            }

            return String.Format("Error while loading the a DBObject. " + Environment.NewLine + "Errors:" + Environment.NewLine + "{0}", ErrorString);
        }
    }
}
