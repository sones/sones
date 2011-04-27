using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown when a vertex will be inserted with an abstract vertex type.
    /// </summary>
    public class AbstractConstraintViolationException: AGraphDBInsertException
    {
        /// <summary>
        /// Create a new instance of AbstractConstraintViolationException.
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type.</param>
        public AbstractConstraintViolationException(String myVertexTypeName)
        {
            VertexTypeName = myVertexTypeName;
            _msg = string.Format("Vertex type {0} ist abstract and can not contain vertices.", myVertexTypeName);
        }

        /// <summary>
        /// The name of the abstract vertex type.
        /// </summary>
        public string VertexTypeName { get; set; }
    }
}
