using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_BackwardEdgeAlreadyExist : GraphDBBackwardEdgeError
    {
        public GraphDBType Type { get; private set; }
        public String EdgeAttributeName { get; private set; }
        public String EdgeTypeName { get; private set; }

        public Error_BackwardEdgeAlreadyExist(GraphDBType myType, String myEdgeTypeName, String myEdgeAttributeName)
        {
            Type = myType;
            EdgeAttributeName = myEdgeAttributeName;
            EdgeTypeName = myEdgeTypeName;
        }

        public override string ToString()
        {
            return String.Format("The type \"{0}\" already has a backward edge definition to \"{1}\".\"{2}\"", Type.Name, EdgeTypeName, EdgeAttributeName);
        }
    }
}
