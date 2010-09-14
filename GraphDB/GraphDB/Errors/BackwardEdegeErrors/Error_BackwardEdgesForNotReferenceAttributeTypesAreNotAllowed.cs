using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_BackwardEdgesForNotReferenceAttributeTypesAreNotAllowed : GraphDBBackwardEdgeError
    {
        public String AttributeName { get; private set; }

        public Error_BackwardEdgesForNotReferenceAttributeTypesAreNotAllowed(String myAttributeName)
        {
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            return String.Format("You can't define backwardedges for non reference attribute \"{0}\"!", AttributeName);
        }
    }
}
