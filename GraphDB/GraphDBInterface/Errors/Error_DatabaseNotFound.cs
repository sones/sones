using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_DatabaseNotFound : GraphDBError
    {
        public String Database { get; private set; }

        public Error_DatabaseNotFound(String myDatabase)
        {
            Database = myDatabase;
        }

        public override string ToString()
        {
            return String.Format("The database \"{0}\" could not be found!", Database);
        }
    }
}
