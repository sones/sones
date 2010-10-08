using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_IndexAttributeDoesNotExist : GraphDBIndexError
    {
        public String IndexAttributeName { get; private set; }

        public Error_IndexAttributeDoesNotExist(String myIndexAttributeName)
        {
            IndexAttributeName = myIndexAttributeName;
        }

        public override string ToString()
        {
            return String.Format("The attribute \"{0}\" for the index does not exist!", IndexAttributeName);
        }
    }
}
