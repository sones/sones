using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidAttribute : GraphDBAttributeError
    {
        public String Info { get; private set; }

        public Error_InvalidAttribute(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return String.Format("The attribute is not valid: {0}", Info);
        }
    }
}
