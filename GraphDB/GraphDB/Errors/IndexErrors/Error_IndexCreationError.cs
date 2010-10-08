using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_IndexCreationError : GraphDBIndexError
    {

        public String IndexName { get; private set; }
        public String IndexEdition { get; private set; }
        public String Info { get; private set; }
        public Exception Exception { get; private set; }
        public GraphDBType Type { get; private set; }

        public Error_IndexCreationError(String myIndexName, String myIndexEdition, String myInfo)
        {
            IndexName = myIndexName;
            IndexEdition = myIndexEdition;
            Info = myInfo;
            Type = null;
            Exception = null;
        }

        public Error_IndexCreationError(GraphDBType myType, Exception myException)
        {
            Type = myType;
            Exception = myException;
        }

        public override string ToString()
        {
            if (Type != null)
            {
                if (Exception != null && Exception.Message != null)
                {
                    return String.Format("Could not create index on type \"{0}\". Description:" + Environment.NewLine + "{1}", Type.Name, Exception.Message);
                }
                else
                {
                    return String.Format("Could not create index on type \"{0}\".", Type.Name);
                }
            }
            else
            {
                return String.Format("Could not create index \"{0}\" (Edition: \"{1}\"). Error:" + Environment.NewLine + "{2}", IndexName, IndexEdition, Info);
            }
        }
    }
}
