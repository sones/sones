using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_IndexTypeDoesNotExist : GraphDBIndexError
    {
        public String IndexTypeName { get; private set; }

        public Error_IndexTypeDoesNotExist(String myIndexTypeName)
        {
            IndexTypeName = myIndexTypeName;
        }

        public override string ToString()
        {
            return String.Format("The index type \"{0}\" does not exist!", IndexTypeName);
        }
    }
}
