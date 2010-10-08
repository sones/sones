using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidAssignOfSet : GraphDBAttributeAssignmentError
    {
        public String AttributeName { get; private set; }
        
        public Error_InvalidAssignOfSet(String myAttributeName)
        {
            AttributeName = myAttributeName;    
        }

        public override string ToString()
        {
            return String.Format("Assignment of the reference type {0} with a list is not allowed. Use SETOF or REF (REFERENCE) instead.", AttributeName);
        }
    }
}
