using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidReferenceAssignmentOfUndefAttr : GraphDBAttributeAssignmentError
    {
        public Error_InvalidReferenceAssignmentOfUndefAttr()
        { }

        public override string ToString()
        {
            return "An reference assignment is for undefined attributes not allowed.";   
        }
    }
}
