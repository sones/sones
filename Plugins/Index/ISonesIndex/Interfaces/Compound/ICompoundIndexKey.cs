using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Compound
{
    /// <summary>
    /// Used to map a key to a propertyID.
    /// </summary>
    public interface ICompoundIndexKey
    {
        /// <summary>
        /// The propertyID
        /// </summary>
        Int64 PropertyID { get; }

        /// <summary>
        /// The value of the key
        /// </summary>
        IComparable Key { get; }
    }
}
