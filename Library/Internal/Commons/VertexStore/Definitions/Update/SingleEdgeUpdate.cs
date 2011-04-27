using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The class that defines updates on single edges
    /// </summary>
    public sealed class SingleEdgeUpdate
    {
        #region data

        /// <summary>
        /// The single edges that should be updated
        /// </summary>
        public Dictionary<Int64, SingleEdgeUpdateDefinition> Updated;
        
        /// <summary>
        /// The single edge property-ids that should be deleted
        /// </summary>
        public IEnumerable<Int64> Deleted;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new single edge update
        /// </summary>
        /// <param name="myAdded">The single edges that should be added</param>
        /// <param name="myUpdated">The single edges that should be updated</param>
        /// <param name="myDeleted">The single edge property-ids that should be deleted</param>
        public SingleEdgeUpdate(
            Dictionary<Int64, SingleEdgeAddDefinition> myAdded = null,
            Dictionary<Int64, SingleEdgeUpdateDefinition> myUpdated = null,
            IEnumerable<Int64> myDeleted = null)
        {
            Updated = myUpdated;
            Added = myAdded;
            Deleted = myDeleted;
        }

        #endregion
    }
}
