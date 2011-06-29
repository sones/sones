using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    public class IllegalOptionException: AGraphDBException
    {
        public String WrongParameter { get; private set; }
        public Type NeededType { get; private set; }

        public IllegalOptionException(string myOption, Type myNeededType, Exception myInnerException = null): base(myInnerException)
        {
            WrongParameter = myOption;
            NeededType = myNeededType;

            _msg = String.Format("The option {0} needs a value of type {1}. ", myOption, NeededType.Name);
        }
    }
}
