using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index
{
    /// <summary>
    /// 
    /// By default, the underlying type of an enum is int. 
    /// We don't need this space here, so we take byte.
    /// </summary>
    public enum IndexAddStrategy : byte
    {
        /// <summary>
        /// Replace value of existing keys
        /// </summary>
        REPLACE,
        /// <summary>
        /// Merge values of existing keys
        /// </summary>
        MERGE,
        /// <summary>
        /// Index key has to be unique, set fails
        /// </summary>
        UNIQUE
    }
}
