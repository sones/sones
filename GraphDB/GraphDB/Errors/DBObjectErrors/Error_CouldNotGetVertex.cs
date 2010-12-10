using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Errors
{
    public class Error_CouldNotGetVertex : GraphDBObjectError
    {
        public String VertexTypeName { get; private set; }
        public ObjectUUID VertexUUID { get; private set; }

        public Error_CouldNotGetVertex(String myVertexTypeName, ObjectUUID myVertexUUID)
        {
            VertexTypeName = myVertexTypeName;
            VertexUUID = myVertexUUID;
        }

        public override string ToString()
        {
            return String.Format("Could not get the vertex {0} of vertextype {1}.", VertexUUID, VertexTypeName);
        }
    }
}
