using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    public class UselessVertexTypeException : AGraphDBVertexTypeException
    {
        private VertexTypePredefinition VertexType;

        public UselessVertexTypeException(Request.VertexTypePredefinition predef)
        {
            this.VertexType = predef;
            _msg = string.Format("Vertex type [{0}] is marked sealed and abstract. This makes this type useless.", predef.VertexTypeName);
        }
    }
}
