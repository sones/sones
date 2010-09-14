using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_MultipleResults : GraphDBSelectError
    {
        public Error_MultipleResults()
        { }

        public override string ToString()
        {
            return "The selection returned more than one result. This is not allowed for the INSERTORUPDATE statement.";
        }
    }
}
