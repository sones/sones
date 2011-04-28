using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.Index
{
    #region ISonesIndexVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible ISonesIndex plugin versions. 
    /// Defines the min and max version for all ISonesIndex implementations which will be activated used this IIndex.
    /// </summary>
    public static class ISonesIndexVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion

    public interface ISonesIndex
    {
        /// <summary>
        /// The name of the index
        /// </summary>
        String IndexName { get; }
    }
}
