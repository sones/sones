using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_IndexDoesNotExist : GraphDBIndexError
    {
        public String IndexName { get; private set; }
        public String IndexEdition { get; private set; }

        public Error_IndexDoesNotExist(String myIndexName, String myIndexEdition)
        {
            IndexName = myIndexName;
            IndexEdition = myIndexEdition;
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(IndexName) && !String.IsNullOrEmpty(IndexEdition))
                return String.Format("The index \"{0}\" with edition \"{1}\" does not exist!", IndexName, IndexEdition);
            if (!String.IsNullOrEmpty(IndexName))
                return String.Format("The index \"{0}\" does not exist!", IndexName);
            else
                return String.Format("The indexedition \"{0}\" does not exist!", IndexEdition);
        }
    }
}
