using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidIndexAttribute : GraphDBIndexError
    {
        public String Attribute { get; private set; }

        public Error_InvalidIndexAttribute(String myAttribute)
        {
            Attribute = myAttribute;
        }

        public override string ToString()
        {
            return String.Format("The attribute \"{0}\" can not be indexed!", Attribute);
        }
    }
}
