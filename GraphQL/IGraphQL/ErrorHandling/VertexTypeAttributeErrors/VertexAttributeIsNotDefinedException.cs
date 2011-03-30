using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The attribute is not defined on this type
    /// </summary>
    public sealed class VertexAttributeIsNotDefinedException : AGraphQLVertexAttributeException
    {
        public String AttributeName { get; private set; }
        public String TypeName { get; private set; }

        /// <summary>
        /// Creates a new VertexAttributeIsNotDefinedException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
        public VertexAttributeIsNotDefinedException(String myAttributeName)
        {
            AttributeName = myAttributeName;

        }

        /// <summary>
        /// Creates a new VertexAttributeIsNotDefinedException exception
        /// </summary>
        /// <param name="myTypeName">The name of the type</param>
        /// <param name="myAttributeName">The name of the attribute</param>
        public VertexAttributeIsNotDefinedException(String myTypeName, String myAttributeName)
        {
            TypeName = myTypeName;
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            if (TypeName != null)
                return String.Format("The attribute \"{0}\" is not defined on type \"{1}\"!", AttributeName, TypeName);
            else
                return String.Format("The attribute \"{0}\" is not defined!", AttributeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.VertexAttributeIsNotDefined; }
        } 
    }
}
