using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_EdgeTypeDoesNotExist : GraphDBEdgeError
    {
        public String EdgeType { get; private set; }

        public Error_EdgeTypeDoesNotExist(String myEdgeType)
        {
            EdgeType = myEdgeType;
        }

        public override string ToString()
        {
            return String.Format("The edgetype \"{0}\" does not exist!", EdgeType);
        }
    }
}
