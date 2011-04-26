using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The update for graph elements
    /// </summary>
    public abstract class AGraphElementUpdateDefinition
    {
        /// <summary>
        /// A comment for the graphelement
        /// </summary>
        public readonly String CommentUpdate;

        /// <summary>
        /// The structured properties
        /// </summary>
        public readonly StructuredPropertiesUpdate UpdatedStructuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        public readonly UnstructuredPropertiesUpdate UpdatedUnstructuredProperties;

        #region constructor

        /// <summary>
        /// Creates a new graph element update definition
        /// </summary>
        /// <param name="myCommentUpdate">A comment for the graphelement</param>
        /// <param name="myUpdatedStructuredProperties">The structured properties</param>
        /// <param name="myUpdatedUnstructuredProperties">The unstructured properties</param>
        protected AGraphElementUpdateDefinition(
            String myCommentUpdate = null, 
            StructuredPropertiesUpdate myUpdatedStructuredProperties = null, 
            UnstructuredPropertiesUpdate myUpdatedUnstructuredProperties = null)
        {
            CommentUpdate = myCommentUpdate;
            UpdatedStructuredProperties = myUpdatedStructuredProperties;
            UpdatedUnstructuredProperties = myUpdatedUnstructuredProperties;
        }

        #endregion
    }

}
