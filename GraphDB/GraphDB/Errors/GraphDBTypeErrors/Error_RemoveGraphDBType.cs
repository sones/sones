using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_RemoveGraphDBType : GraphDBTypeError
    {
        public GraphDBType GraphDBType { get; private set; }

        public Error_RemoveGraphDBType(GraphDBType graphDBType)
        {
            GraphDBType = graphDBType;
        }

        public override string ToString()
        {
            return String.Format("The type " + GraphDBType.Name + " could not be removed from GraphFS.");
        }
    }
}
