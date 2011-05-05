using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The updates for structured properties
    /// </summary>
    public sealed class StructuredPropertiesUpdate
    {
        #region data

        /// <summary>
        /// The to be updated structured properties
        /// </summary>
        public readonly IDictionary<Int64, IComparable> Updated;
        
        /// <summary>
        /// The properties that should be deleted
        /// </summary>
        public readonly IEnumerable<Int64> Deleted;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new update for structured properties
        /// </summary>
        /// <param name="myUpdated">The to be updated structured properties</param>
        /// <param name="myDeleted">The properties that should be deleted</param>
        public StructuredPropertiesUpdate(IDictionary<Int64, IComparable> myUpdated = null, IEnumerable<Int64> myDeleted = null)
        {
            Updated = myUpdated;
            Deleted = myDeleted;
        }

        #endregion
    }
}
