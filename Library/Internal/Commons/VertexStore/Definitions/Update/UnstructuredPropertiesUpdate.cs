using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The updates for unstructured properties
    /// </summary>
    public sealed class UnstructuredPropertiesUpdate
    {
        #region data

        /// <summary>
        /// The to be updated unstructured properties
        /// </summary>
        public IDictionary<String, Object> Updated;
        
        /// <summary>
        /// The unstructured properties that should be deleted
        /// </summary>
        public IEnumerable<String> Deleted;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new update for structured properties
        /// </summary>
        /// <param name="myUpdated">The to be updated unstructured properties</param>
        /// <param name="myDeleted">The unstructured properties that should be deleted</param>
        public UnstructuredPropertiesUpdate(IDictionary<String, Object> myUpdated = null, IEnumerable<String> myDeleted = null)
        {
            Updated = myUpdated;
            Deleted = myDeleted;
        }

        #endregion
    }
}
