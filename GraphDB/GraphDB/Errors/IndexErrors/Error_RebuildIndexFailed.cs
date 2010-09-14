using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ObjectManagement;

namespace sones.GraphDB.Errors
{
    public class Error_RebuildIndexFailed : GraphDBIndexError
    {
        public String IndexName     { get; private set; }
        public String IndexEdition  { get; private set; }
        public String Errors    { get; private set; }

        public Error_RebuildIndexFailed(String myIndexName, String myIndexEdition, String myErrors)
        {
            IndexName = myIndexName;
            IndexEdition = myIndexEdition;
            Errors = myErrors;
        }

        public override string ToString()
        {
            return String.Format("Rebuilt of index \"{0}\" (Edition: \"{1}\") failed. Errors:" + Environment.NewLine + "{2}", IndexName, IndexEdition, Errors);
        }
    }
}
