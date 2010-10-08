using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Errors
{
    public class Error_DatabaseNotFound : GraphDBError
    {
        public ObjectLocation Database { get; private set; }

        public Error_DatabaseNotFound(ObjectLocation myDatabase)
        {
            Database = myDatabase;
        }

        public override string ToString()
        {
            return String.Format("The database \"{0}\" could not be found!", Database);
        }
    }
}
