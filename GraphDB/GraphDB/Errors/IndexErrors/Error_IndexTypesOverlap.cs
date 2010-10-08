using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_IndexTypesOverlap : GraphDBIndexError
    {
        public Error_IndexTypesOverlap()
        {        
        }

        public override string ToString()
        {
            return "An index over more than one type is not allowed.";
        }
    }
}
