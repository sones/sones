using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The updates for binary properties
    /// </summary>
    public sealed class BinaryPropertiesUpdate
    {
        #region data

        /// <summary>
        /// The to be updated binary properties
        /// </summary>
        public IDictionary<Int64, StreamAddDefinition> Updated;
        
        /// <summary>
        /// The to be deleted unstructured properties
        /// </summary>
        public IEnumerable<Int64> Deleted;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new update for binary properties
        /// </summary>
        /// <param name="myUpdated">The to be updated binary properties</param>
        /// <param name="myDeleted">The to be deleted unstructured properties</param>
        public BinaryPropertiesUpdate(IDictionary<Int64, StreamAddDefinition> myUpdated = null, IEnumerable<Int64> myDeleted = null)
        {
            Updated = myUpdated;
            Deleted = myDeleted;
        }

        #endregion
    }
}
