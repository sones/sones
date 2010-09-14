using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidIndexReference : GraphDBIndexError
    {
        public String IndexName { get; private set; }
        public String IndexEdition { get; private set; }

        public Error_InvalidIndexReference(String myIndexName, String myIndexEdition)
        {
            IndexName = myIndexName;
            IndexEdition = myIndexEdition;
        }

        public override string ToString()
        {
            return String.Format("The index reference of {0} with edition {1} is invalid!", IndexName, IndexEdition);
        }
    }
}
