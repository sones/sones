using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_IndexAlreadyExist : GraphDBIndexError
    {
        public String Index { get; private set; }

        public Error_IndexAlreadyExist(String myIndex)
        {
            Index = myIndex;
        }

        public override string ToString()
        {
            return String.Format("The index \"{0}\" already exists!", Index);
        }
    }
}
