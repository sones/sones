using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_TruncateNotAllowedOnInheritedType : GraphDBError
    {
        public String TypeName { get; private set; }

        public Error_TruncateNotAllowedOnInheritedType(String myTypeName)
        {
            TypeName = myTypeName;
        }

        public override string ToString()
        {
            return String.Format("Truncate on the inherited type '{0}' is not allowed!", TypeName);
        }
    }
}
