using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{

    public class Error_CouldNotAlterIndexOnType : GraphDBIndexError
    {

        public String IndexType { private set; get; }

        public Error_CouldNotAlterIndexOnType(String myIndexType)
        {
            IndexType = myIndexType;
        }

        public override string ToString()
        {
            return String.Format("Could not alter index on type \"{0}\".", IndexType);
        }

    }

}
