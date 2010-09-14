using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ListAttributeNotAllowed : GraphDBTypeError
    {
        public String TypeName { get; private set; }
        public Error_ListAttributeNotAllowed(String myTypeName)
        {
            TypeName = myTypeName;
        }

        public override string ToString()
        {
            return String.Format("The user defined type \\{0}\\ should not be used with LIST<> attributes, please use SET<> instead.", TypeName);
        }
    }
}
