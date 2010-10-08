using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ReferenceAssignment : GraphDBAttributeAssignmentError
    {
        public String Info { get; private set; }

        public Error_ReferenceAssignment(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return Info;
        }
    }
}
