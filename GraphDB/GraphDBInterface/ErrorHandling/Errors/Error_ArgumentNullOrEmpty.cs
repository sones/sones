using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ArgumentNullOrEmpty : GraphDBError
    {
        public String Argument { get; private set; }

        public Error_ArgumentNullOrEmpty(String myArgument)
        {
            Argument = myArgument;
        }

        public override string ToString()
        {
            return String.Format("The argument \"{0}\" is null or empty!", Argument);
        }

    }
}
