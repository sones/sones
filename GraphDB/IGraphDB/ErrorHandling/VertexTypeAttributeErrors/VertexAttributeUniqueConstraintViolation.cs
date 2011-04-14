using System;

namespace sones.GraphDB.ErrorHandling
{
   /// <summary>
    /// The unique constraint of an attribute of an vertex type has been violated
   /// </summary>
    public sealed class VertexAttributeUniqueConstraintViolation : AGraphDBVertexAttributeException
    {
        public String VertexAttributeName { get; private set; }
        public String VertexTypeName { get; private set; }

       /// <summary>
        /// Creates a new VertexAttributeUniqueConstraintViolation exception
       /// </summary>
       /// <param name="myVertexTypeName">The name of given the vertex type</param>
       /// <param name="myVertexAttributeName">The name of the given vertex attribute</param>
        public VertexAttributeUniqueConstraintViolation(String myVertexTypeName, String myVertexAttributeName)
       {
           VertexAttributeName = myVertexAttributeName;
           VertexTypeName = myVertexTypeName;
           _msg = String.Format("The unique constraint of the attribute {0} of the vertex type {1} has been violated.", VertexAttributeName, VertexTypeName);
       }

    }
}
