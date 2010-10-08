using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ReferenceAssignment_EmptyValue : GraphDBAttributeAssignmentError
    {
        public String AttributeName { get; private set; }

        public Error_ReferenceAssignment_EmptyValue(String myAttributeName)
        {
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            return String.Format("The single reference attribute {0} does not contain any value.", AttributeName);
        }
    }
}
