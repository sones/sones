using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Fulltext
{
    /// <summary>
    /// Represents a single result of a full text query.
    /// </summary>
    public interface ISonesFulltextResult
    {
        /// <summary>
        /// Gets the max score of all resultentries.
        /// </summary>
        /// <value>
        /// The max score.
        /// </value>
        Double? MaxScore { get; }

        /// <summary>
        /// The resulting documents / vertices
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        ICloseableEnumerable<ISonesFulltextResultEntry> Entries { get; }

        /*
         * TODO: place for additional functionality
         * like facets, grouping, 
         */

        /// <summary>
        /// Can be used for additional information.
        /// </summary>
        /// <value>
        /// The additional parameters.
        /// </value>
        IDictionary<string, object> AdditionalParameters { get; }
    }
}
