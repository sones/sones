using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Errors
{
    public class Error_NotImplementedExpressionNode : GraphDBError
    {
        public Type NodeType { get; private set; }
        public Error_NotImplementedExpressionNode(Type myNodeType)
        {
            NodeType = myNodeType;
        }

        public override string ToString()
        {
            return String.Format("Currently the type {0} has not been implemented for expressions.", NodeType.Name);
        }
    }
}
