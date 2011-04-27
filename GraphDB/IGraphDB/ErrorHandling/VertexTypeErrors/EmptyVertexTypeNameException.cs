using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a vertex type with an empty name should was used.
    /// </summary>
    public sealed class EmptyVertexTypeNameException: AGraphDBVertexTypeException
    {
        /// <summary>
        /// Creates an instance of EmptyVertexTypeNameException.
        /// </summary>
        public EmptyVertexTypeNameException()
        {
            _msg = "This operation needs a valid vertex type name.";
        }
    }
}
