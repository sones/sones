using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidAttrDefaultValueAssignment : GraphDBAttributeAssignmentError
    {
        public String AttributeName { get; private set; }
        public String AttributeType   { get; private set; }
        public String ExpectedType  { get; private set; }

        public Error_InvalidAttrDefaultValueAssignment(String myAttributeName, String myAttrType, String myExpectedType)
        {
            AttributeName = myAttributeName;
            AttributeType = myAttrType;
            ExpectedType = myExpectedType;
        }

        public Error_InvalidAttrDefaultValueAssignment(String myAttrType, String myExpectedType)
        {
            AttributeName = String.Empty;
            AttributeType = myAttrType;
            ExpectedType = myExpectedType;
        }

        public override string ToString()
        {
            String retVal = String.Empty;

            if (AttributeName.Length > 0)
            {
                retVal = String.Format("An assignment for the attribute \"{0}\" from type \"{1}\" with an value of the type \"{2}\" is not valid.", AttributeName, AttributeType, ExpectedType);
            }
            else
            {
                retVal = String.Format("Invalid type assignment for default value. Current type is \"{0}\". The type \"{1}\" is expected.", AttributeType, ExpectedType);
            }

            return retVal;
        }
    }
}
