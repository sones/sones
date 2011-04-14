using System;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown, if a vertex type is added, but it contains duplicated attribute names.
    /// </summary>
    public sealed class DuplicatedAttributeNameException: AGraphDBVertexAttributeException
    {
        /// <summary>
        /// The name of the attribute, that is tried to be added multiple times.
        /// </summary>
        public string DuplicatedName { get; private set; }

        /// <summary>
        /// The vertex type predefinition that contains a duplicated attribute name.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// Creates a new instance of DuplicatedVertexTypeNameException.
        /// </summary>
        /// <param name="VertexTypeName">The name of the vertex type, that is tried to be added multiple times.</param>
        public DuplicatedAttributeNameException(VertexTypePredefinition myVertexTypePredefinition, String myVertexTypeName)
        {
            Predefinition = myVertexTypePredefinition;
            DuplicatedName = myVertexTypeName;
            _msg = string.Format("The attribute {0} was declared multiple times on vertex type {1}.", DuplicatedName, Predefinition.VertexTypeName);
        }

    }
}
