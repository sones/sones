using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The vertex attribute already exists in the type
    /// </summary>
    public sealed class VertexAttributeAlreadyExistsException : AGraphQLVertexAttributeException
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
        }

        public override string ToString()
        {
            if (TypeName != null)
                return String.Format("The attribute \"{0}\" already exist in type \"{1}\"!", AttributeName, TypeName);
            else
                return String.Format("The attribute \"{0}\" already exist!", AttributeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.VertexAttributeAlreadyExists; }
        } 

    }
}
