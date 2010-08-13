using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ReadOnlyViolation : GraphDBError
    {
        public String StatementName { get; private set; }

        public Error_ReadOnlyViolation(String myStatementName)
        {
            StatementName = myStatementName;
        }

        public override string ToString()
        {
            return String.Format("The database is set to be readonly at the moment. You tried to execute a \"{0}\" statement which is readwrite!", StatementName);
        }
    }
}
