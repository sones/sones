using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Errors
{
    public class Error_CouldNotGetIndexReference : GraphDBIndexError
    {
        public IEnumerable<IError> Errors { get; private set; }
        public String IndexName { get; private set; }
        public String IndexEdition { get; private set; }

        public Error_CouldNotGetIndexReference(IEnumerable<IError> myErrors, String myIndexName, String myIndexEdition)
        {
            Errors = myErrors;
            IndexName = myIndexName;
            IndexEdition = myIndexEdition;
        }

        public override string ToString()
        {
            return String.Format("The index reference of {0} with edition {1} could not be loaded!", IndexName, IndexEdition);
        }
    }
}
