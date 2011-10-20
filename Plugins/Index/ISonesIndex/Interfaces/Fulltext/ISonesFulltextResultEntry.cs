using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Fulltext
{
    /// <summary>
    /// Interface represents the result for a single document / vertex.
    /// </summary>
    public interface ISonesFulltextResultEntry
    {
        /// <summary>
        /// Vertex / Document identifier
        /// </summary>
        /// <value>
        /// The ID of the Vertex / Document
        /// </value>
        Int64 VertexID { get; }

        /// <summary>
        /// Highlighted results for that document
        /// </summary>
        /// <value>
        /// Highlighted result ordered by
        /// the indexed propertyID
        /// </value>/
        IDictionary<Int64, string> Highlights { get; }

        /// <summary>
        /// Can be used for additional information about the query result.
        /// </summary>
        /// <value>
        /// The additional parameters.
        /// </value>
        IDictionary<string, object> AdditionalParameters { get; }
    }
}
