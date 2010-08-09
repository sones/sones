using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib;

namespace sones.GraphDB.Errors
{
    public class Error_EdgeParameterCountMismatch : GraphDBEdgeError
    {
        public String Edge { get; private set; }
        public Int32 CurrentNumOfParams { get; private set; }
        public Int32 ExpectedNumOfParams { get; private set; }

        public Error_EdgeParameterCountMismatch(String edge, Int32 currentNumOfParams, Int32 expectedNumOfParams)
        {
            Edge = edge;
            CurrentNumOfParams = currentNumOfParams;
            ExpectedNumOfParams = expectedNumOfParams;
        }

        public override string ToString()
        {
            return String.Format("The edge [{0}] expects [{1}] params but found [{2}].", Edge, ExpectedNumOfParams, CurrentNumOfParams);
        }
    }
    
}
