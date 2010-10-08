using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidType : GraphDBTypeError
    {
        public GraphDBType InvalidType { get; private set; }
        public String Info { get; private set; }

        public Error_InvalidType(GraphDBType myInvalidType, String myInfo)
        {
            Info = myInfo;
            InvalidType = myInvalidType;
        }

        public override string ToString()
        {
            return String.Format("The type {0} is not valid. {1}.", InvalidType.Name, Info);
        }
    }
}
