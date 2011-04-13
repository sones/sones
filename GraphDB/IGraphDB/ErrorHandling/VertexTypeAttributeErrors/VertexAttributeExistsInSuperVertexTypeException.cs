using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The vertex attribute already exists in supertype
    /// </summary>
    public sealed class VertexAttributeExistsInSuperVertexTypeException : AGraphDBVertexAttributeException
    {
        #region data        

        public String VertexAttributeName { get; private set; }
        public String VertexSupertypeName { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Creates a new VertexAttributeExistsInSuperVertexTypeException exception
        /// </summary>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
        public VertexAttributeExistsInSuperVertexTypeException(String myVertexAttributeName)
        {
            VertexAttributeName = myVertexAttributeName;
        }

        /// <summary>
        /// Create a new VertexAttributeExistsInSuperVertexTypeException exception
        /// </summary>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
        /// <param name="myVertexSupertypeName">The name of the vertex supertype</param>
        public VertexAttributeExistsInSuperVertexTypeException(String myVertexAttributeName, String myVertexSupertypeName)
        {
            VertexAttributeName = myVertexAttributeName;
            VertexSupertypeName = myVertexSupertypeName;

            _msg = String.Format("The vertex attribute \"{0}\" already exists in supertype \"{1}\"!", VertexAttributeName, VertexSupertypeName);
        }

        #endregion

    }
}
