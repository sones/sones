using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidAttributeValue : GraphDBAttributeError
    {
        public String AttributeName { get; private set; }
        public Object AttributeValue { get; private set; }

        public Error_InvalidAttributeValue(String myAttributeName, Object myAttributeValue)
        {
            AttributeName = myAttributeName;
            AttributeValue = myAttributeValue;
        }

        public override string ToString()
        {
            return String.Format("The attribute \"{0}\" has an invalid value: \"{1}\"", AttributeName, AttributeValue);
        }
    }
}
