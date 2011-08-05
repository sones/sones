using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    public class InvalidBaseTypeException: AGraphDBTypeException
    {
        public InvalidBaseTypeException(String myType)
        {
            _msg = String.Format("The base type [{0}] must be a user defined type.", myType);
        }
    }
}
