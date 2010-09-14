using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Errors
{
    public class Error_IndexRebuildError : GraphDBIndexError
    {
        public GraphDBType Type { get; private set; }
        public ObjectLocation IndexLocation { get; private set; }
        
        public Error_IndexRebuildError(GraphDBType myType, ObjectLocation myObjectLocation)
        {
            Type = myType;
            IndexLocation = myObjectLocation;
        }

        public override string ToString()
        {
            return String.Format("Could not rebuild index of type \"{0}\" at ObjectLocation \"{1}\".", Type.Name, IndexLocation.Path);
        }
    }
}
