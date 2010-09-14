using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidSelectValueAssignment : GraphDBSelectError
    {
        public String Info { get; private set; }

        public Error_InvalidSelectValueAssignment(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return "You can not assign a value to [" + Info + "]";
        }
    }
}
