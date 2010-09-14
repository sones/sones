using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidIDNode : GraphDBError
    {
        public String Info { get; private set; }

        public Error_InvalidIDNode(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return String.Format("The idNode is not valid: \"{0}\"", Info);
        }
    }
}
