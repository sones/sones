using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling.Type
{
    public class InvalidTypeException: AGraphDBTypeException
    {
        private string p;
        private string p_2;

        public InvalidTypeException(string myUnexpectedType, string myExpectedType)
        {
            // TODO: Complete member initialization
            this.p = myUnexpectedType;
            this.p_2 = myExpectedType;
        }
    }
}
