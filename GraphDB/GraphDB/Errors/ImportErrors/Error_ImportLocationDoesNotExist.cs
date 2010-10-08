using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_ImportLocationDoesNotExist : GraphDBError
    {
        public String ImportLocation { get; private set; }

        public Error_ImportLocationDoesNotExist(String myImportLocation)
        {
            ImportLocation = myImportLocation;
        }

        public override string ToString()
        {
            return "ImportLocation [" + ImportLocation + "] does not exist.";
        }
    }
}
