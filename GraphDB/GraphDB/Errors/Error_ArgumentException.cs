using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ArgumentException : GraphDBError
    {
        public String Argument { get; private set; }

        public Error_ArgumentException(String myArgument)
        {
            Argument = myArgument;
        }

        public override string ToString()
        {
            return Argument;
        }

    }
}
