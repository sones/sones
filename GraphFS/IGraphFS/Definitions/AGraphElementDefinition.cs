using System;
using System.Collections.Generic;

namespace sones.GraphFS.Definitions
{

    /// <summary>
    /// Definition of a graph element
    /// </summary>
    public abstract class AGraphElementDefinition
    {
       #region data

        /// <summary>
        /// A comment for the vertex
        /// </summary>
        public readonly string Comment;

        /// <summary>
        /// The creation date of the vertex
        /// </summary>
        public readonly long CreationDate;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        public readonly long ModificationDate;

        /// <summary>
        /// The structured properties
        /// </summary>
        public readonly Dictionary<Int64, Object> StructuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        public readonly Dictionary<String, Object> UnstructuredProperties;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new graph element definition
        /// </summary>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        protected AGraphElementDefinition(
            String myComment,
            long myCreationDate,
            long myModificationDate,
            Dictionary<Int64, Object> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties)
        {
            Comment = myComment;
            CreationDate = myCreationDate;
            ModificationDate = myModificationDate;
            StructuredProperties = myStructuredProperties;
            UnstructuredProperties = myUnstructuredProperties;
        }

        #endregion
    }
}