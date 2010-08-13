using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_FileNotFound : GraphDBError
    {
        public String FileName { get; private set; }

        public Error_FileNotFound(String myFileName)
        {
            FileName = myFileName;
        }

        public override string ToString()
        {
            return String.Format("The file \"{0}\" could not be found!", FileName);
        }
    }
}
