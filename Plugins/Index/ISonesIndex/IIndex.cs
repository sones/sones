using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index
{
    #region IIndexVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IIndex plugin versions. 
    /// Defines the min and max version for all IIndex implementations which will be activated used this IIndex.
    /// </summary>
    public static class IIndexVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("1.0.0.1");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("1.0.0.1");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interfaces for all indices
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// Is this a persistent index?
        /// </summary>
        Boolean IsPersistent { get; }

        /// <summary>
        /// Returns the name of the index
        /// </summary>
        String Name { get; }
    }
}
