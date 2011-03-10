using System;
using System.Reflection;
using sones.Library.ErrorHandling;

namespace sones.Library.VersionedPluginManager
{
    /// <summary>
    /// This exception occurs if a version of a plugin does not match the expected version.
    /// </summary>
    public sealed class IncompatiblePluginVersionException : ASonesException
    {
        #region data

        /// <summary>
        /// The current plugin version.
        /// </summary>
        public readonly Version CurrentVersion;

        /// <summary>
        /// The maximum accepted version.
        /// </summary>
        public readonly Version MaxVersion;

        /// <summary>
        /// The minimum accepted version.
        /// </summary>
        public readonly Version MinVersion;

        /// <summary>
        /// The plugin assembly which implements the incompatible version.
        /// </summary>
        public readonly Assembly PluginAssembly;

        #endregion

        #region constructor

        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion,
                                                  Version myMinVersion)
        {
            PluginAssembly = myPluginAssembly;
            CurrentVersion = myCurrentVersion;
            MinVersion = myMinVersion;
        }

        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion,
                                                  Version myMinVersion, Version myMaxVersion)
        {
            PluginAssembly = myPluginAssembly;
            CurrentVersion = myCurrentVersion;
            MinVersion = myMinVersion;
            MaxVersion = myMaxVersion;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.IncompatiblePluginVersion; }
        }

        #endregion
    }
}