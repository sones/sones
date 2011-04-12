using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown, if a bunch of vertex types are added, but the list contains duplicated vertex names.
    /// </summary>
    public sealed class DuplicatedVertexTypeNameException: AGraphDBException
    {
        /// <summary>
        /// The name of the vertex type, that is tried to be added multiple times.
        /// </summary>
        public string DuplicatedName { get; private set; }

        /// <summary>
        /// Creates a new instance of DuplicatedVertexTypeNameException.
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type, that is tried to be added multiple times.</param>
        public DuplicatedVertexTypeNameException(String myVertexTypeName)
        {
            DuplicatedName = myVertexTypeName;
            _msg = string.Format("The vertex type {0} was declared multiple times.");
        }

    }
}
