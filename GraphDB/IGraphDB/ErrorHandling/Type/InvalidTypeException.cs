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

        public InvalidTypeException(string myExpectedType, string myUnexpectedType)
        {
            // TODO: Complete member initialization
            this.p = myExpectedType;
            this.p_2 = myUnexpectedType;
        }
    }
}
