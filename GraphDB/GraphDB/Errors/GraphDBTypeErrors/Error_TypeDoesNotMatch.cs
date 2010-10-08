using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_TypeDoesNotMatch : GraphDBTypeError
    {
        public String ExpectedType { get; private set; }
        public String CurrentType { get; private set; }

        public Error_TypeDoesNotMatch(String myExpectedType, String myCurrentType)
        {
            ExpectedType = myExpectedType;
            CurrentType = myCurrentType;
        }

        public override string ToString()
        {
            return String.Format("The Type {0} does not match the expected Type {1}.", CurrentType, ExpectedType);
        }
    }
}
