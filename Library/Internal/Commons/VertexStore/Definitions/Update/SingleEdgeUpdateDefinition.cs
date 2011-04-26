using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The update definition for single edges
    /// </summary>
    public sealed class SingleEdgeUpdateDefinition : AGraphElementUpdateDefinition
    {
        #region data
        
        /// <summary>
        /// Defines the single edge that should be updated
        /// </summary>
        public readonly SingelEdgeVector UpdatedVector;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new single edge update definition
        /// </summary>
        /// <param name="myCommentUpdate">A comment for the graphelemen</param>
        /// <param name="myUpdatedStructuredProperties">The structured properties</param>
        /// <param name="myUpdatedUnstructuredProperties">The unstructured properties</param>
        /// <param name="myUpdatedVector">Defines the single edge that should be updated</param>
        public SingleEdgeUpdateDefinition(
            String myCommentUpdate = null,
            StructuredPropertiesUpdate myUpdatedStructuredProperties = null,
            UnstructuredPropertiesUpdate myUpdatedUnstructuredProperties = null,
            SingelEdgeVector myUpdatedVector = null)
            : base(myCommentUpdate, myUpdatedStructuredProperties, myUpdatedUnstructuredProperties)
        {
            UpdatedVector = myUpdatedVector;
        }

        #endregion
    }
}
