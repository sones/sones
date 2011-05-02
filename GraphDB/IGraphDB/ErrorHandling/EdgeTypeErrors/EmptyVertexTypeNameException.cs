using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if an edge type with an empty name was used.
    /// </summary>
    public sealed class EmptyEdgeTypeNameException: AGraphDBEdgeTypeException
    {
        /// <summary>
        /// Creates an instance of EmptyEdgeTypeNameException.
        /// </summary>
        public EmptyEdgeTypeNameException()
        {
            _msg = "This operation needs a valid edge type name.";
        }
    }
}
