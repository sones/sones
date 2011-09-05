using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Compound;

namespace sones.Plugins.Index.Fulltext
{
    /// <summary>
    /// This interfaces defines a default Fulltext Index structure used
    /// by the sones GraphDB.
    /// 
    /// It supports single- and multi-key indexing and uses more complex
    /// result types which contain index specific data
    /// 
    /// This interface is non-generic because in the sones GraphDB
    /// every indexable attribute is hidden behing a propertyID
    /// of type Int64.
    /// </summary>
    public interface ISonesFulltextIndex : ISonesCompoundIndex
    {
        /// <summary>
        /// Sends a specific query to the full text index and
        /// returns a result which contains resulting entries
        /// and some more information.
        /// </summary>
        /// <param name="myQuery">
        /// The query to select matching vertices / documents.
        /// </param>
        ISonesFulltextResult Query(string myQuery);
    }
}
