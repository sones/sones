using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The class that defines updates on hyperedges of a vertex
    /// </summary>
    public sealed class HyperEdgeUpdate
    {
        #region data

        /// <summary>
        /// The hyperedges that are going to be updated
        /// </summary>
        public IDictionary<Int64, HyperEdgeUpdateDefinition> Updated;
          
        /// <summary>
        /// The hyperedge property-ids that are going to be deleted
        /// </summary>
        public IEnumerable<Int64> Deleted;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new hyper edge update
        /// </summary>
        /// <param name="myAdded">The hyperedges that are going to be added</param>
        /// <param name="myUpdated">The hyperedges that are going to be updated</param>
        /// <param name="myDeleted">The hyperedge property-ids that are going to be deleted</param>
        public HyperEdgeUpdate(
            IDictionary<Int64, HyperEdgeUpdateDefinition> myUpdated = null,
            IEnumerable<Int64> myDeleted = null)
        {
            Updated = myUpdated;
            Deleted = myDeleted;
        }

        #endregion
    }
}
