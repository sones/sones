using System;
using sones.Library.PropertyHyperGraph;

namespace sones.Library.Commons.VertexStore
{
    /// <summary>
    /// Static filter class
    /// </summary>
    public static class VertexStoreFilter
    {
        /// <summary>
        /// A delegate to filter editions
        /// </summary>
        /// <param name="myEdition">The to be filtered edition</param>
        /// <returns>True or false</returns>
        public delegate bool EditionFilter(String myEdition);
        
        /// <summary>
        /// A delegate to filter revisions
        /// </summary>
        /// <param name="myRevisionID">The to be filtered revisions</param>
        /// <returns>True or false</returns>
        public delegate bool RevisionFilter(Int64 myRevisionID);
    }
}
