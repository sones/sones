using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_UniqueConstrainViolation : GraphDBError
    {
        public String TypeName { get; private set; }
        public String IndexName { get; private set; }

        public Error_UniqueConstrainViolation(String myTypeName, String myIndexName)
        {
            TypeName = myTypeName;
            IndexName = myIndexName;
        }

        public override string ToString()
        {
            return String.Format("Unique constraint violation on index {0} of type {1}", IndexName, TypeName);
        }
    }
}
