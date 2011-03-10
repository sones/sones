using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using sones.ErrorHandling;

namespace sones.VersionedPluginManager.ErrorHandling
{
    /// <summary>
    /// This exception occurs if a version of a plugin does not match the expected version.
    /// </summary>
    public sealed class IncompatiblePluginVersionException : ASonesException
    {
        #region data

        /// <summary>
        /// The plugin assembly which implements the incompatible version.
        /// </summary>
        public readonly Assembly PluginAssembly;

        /// <summary>
        /// The current plugin version.
        /// </summary>
        public readonly Version CurrentVersion;
                
        /// <summary>
        /// The minimum accepted version.
        /// </summary>
        public readonly Version MinVersion;

        /// <summary>
        /// The maximum accepted version.
        /// </summary>
        public readonly Version MaxVersion;

        #endregion

        #region constructor

        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion, Version myMinVersion)            
        {
            this.PluginAssembly = myPluginAssembly;
            this.CurrentVersion = myCurrentVersion;
            this.MinVersion = myMinVersion;
        }

        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion, Version myMinVersion, Version myMaxVersion)            
        {
            this.PluginAssembly = myPluginAssembly;
            this.CurrentVersion = myCurrentVersion;
            this.MinVersion = myMinVersion;
            this.MaxVersion = myMaxVersion;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.IncompatiblePluginVersion; }
        }

        #endregion
    }
}
