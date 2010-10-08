using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_BackwardEdgeDestinationIsInvalid : GraphDBBackwardEdgeError
    {
        public String AttributeName { get; private set; }
        public String TypeName { get; private set; }

        public Error_BackwardEdgeDestinationIsInvalid(String myTypeName, String myAttributeName)
        {
            AttributeName = myAttributeName;
            TypeName = myTypeName;
        }

        public override string ToString()
        {
            return String.Format("The backwardedge destination \"{0}\".\"{1}\" is invalid!", TypeName, AttributeName);
        }
    }
}
