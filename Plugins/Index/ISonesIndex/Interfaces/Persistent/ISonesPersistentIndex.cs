using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Persistent
{
    /// <summary>
    /// This interfaces defines a default Persistent Index used
    /// by the sones GraphDB.
    /// 
    /// It supports disposing and shutdown of an index.
    /// </summary>
    public interface ISonesPersistentIndex : ISonesIndex
    {
        /// <summary>
        /// Frees ressources used by the index.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Closes all open streams and disposes ressources.
        /// </summary>
        void Shutdown();
    }
}
