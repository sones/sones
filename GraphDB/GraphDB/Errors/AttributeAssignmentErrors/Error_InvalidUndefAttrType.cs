using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors.AttributeAssignmentErrors
{
    public class Error_InvalidUndefAttrType : GraphDBAttributeAssignmentError
    {        
        public String Attribute     { get; private set; }
        public String AttributeType { get; private set; }
        
        public Error_InvalidUndefAttrType(String myUndefAttribute, String myAttributeType)
        {            
            Attribute = myUndefAttribute;
            AttributeType = myAttributeType;
        }

        public override string ToString()
        {
            return String.Format("Could not assign the value of the undefined attribute \" {0} \" to an defined attribute \" {1} \" with type \" {2} \".", Attribute, Attribute, AttributeType);
        } 
    }
}
