using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The attribute is not defined on this type
    /// </summary>
    public sealed class VertexAttributeIsNotDefinedException : AGraphDBVertexAttributeException
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
            _msg = String.Format("The attribute \"{0}\" is not defined!", AttributeName);
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
            _msg = String.Format("The attribute \"{0}\" is not defined on type \"{1}\"!", AttributeName, TypeName);
        }
    }
}
