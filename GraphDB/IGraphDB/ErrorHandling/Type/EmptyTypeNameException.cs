using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    public class EmptyTypeNameException: AGraphDBTypeException
    {
        public EmptyTypeNameException()
        {
            _msg = "There is no name for the type specified!";
        }

        public EmptyTypeNameException(String myInfo)
        { 
            _msg = "There is no name for the type specified! " + myInfo;
        }
    }
}
