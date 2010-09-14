using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_DBObjectDoesNotExistInIndex : GraphDBObjectError
    {
        public String Info { get; private set; }

        public Error_DBObjectDoesNotExistInIndex(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return String.Format("The dbObject could not be found: {0}", Info);
        }
    }
}
