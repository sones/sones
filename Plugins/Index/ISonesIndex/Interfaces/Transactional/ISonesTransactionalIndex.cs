using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Transactional
{
    /// <summary>
    /// Interface defines a transactional index.
    /// </summary>
    public interface ISonesTransactionalIndex : ISonesIndex
    {
        /// <summary>
        /// Commit a transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback a transaction.
        /// </summary>
        void Rollback();
    }
}
