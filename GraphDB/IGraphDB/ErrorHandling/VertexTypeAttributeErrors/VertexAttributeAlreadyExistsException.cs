using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The vertex attribute already exists in the type
    /// </summary>
    public sealed class VertexAttributeAlreadyExistsException : AGraphDBVertexAttributeException
    {
        public String AttributeName { get; private set; }
        public String TypeName { get; private set; }

        /// <summary>
        /// Creates a new VertexAttributeAlreadyExistsException exception
        /// </summary>
        /// <param name="myAttributeName">The attribute name</param>
        public VertexAttributeAlreadyExistsException(String myAttributeName)
        {
            AttributeName = myAttributeName;
            _msg = String.Format("The attribute \"{0}\" already exist!", AttributeName);
        }

        /// <summary>
        /// Creates a new VertexAttributeAlreadyExistsException exception
        /// </summary>
        /// <param name="myTypeName">The name of the type</param>
        /// <param name="myAttributeName">The attribute name</param>
        public VertexAttributeAlreadyExistsException(String myTypeName, String myAttributeName)
        {
            TypeName = myTypeName;
            AttributeName = myAttributeName;
            _msg = String.Format("The attribute \"{0}\" already exist in type \"{1}\"!", AttributeName, TypeName);
        }

    }
}
