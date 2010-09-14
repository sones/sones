using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidInRangeOperation : GraphDBOperatorError
    {
        public String Info { get; private set; }

        public Error_InvalidInRangeOperation(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return Info;
        }
    }
}
