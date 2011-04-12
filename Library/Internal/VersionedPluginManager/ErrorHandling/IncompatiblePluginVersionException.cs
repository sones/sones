using System;
using System.Reflection;

namespace sones.Library.VersionedPluginManager.ErrorHandling
{
    /// <summary>
    /// This exception occurs if a version of a plugin does not match the expected version.
    /// </summary>
    public sealed class IncompatiblePluginVersionException : APluginManagerException
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

        /// <summary>
        /// Creates a new IncompatiblePluginVersionException exception
        /// </summary>
        /// <param name="myPluginAssembly">The current plugin assembly</param>
        /// <param name="myCurrentVersion">The current version of the plugin interface</param>
        /// <param name="myMinVersion">The minimum expected verion of the plugin interface</param>
        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion,
                                                  Version myMinVersion)
        {
            PluginAssembly = myPluginAssembly;
            CurrentVersion = myCurrentVersion;
            MinVersion = myMinVersion;
        }

        /// <summary>
        /// Creates a new IncompatiblePluginVersionException exception
        /// </summary>
        /// <param name="myPluginAssembly">The current plugin assembly</param>
        /// <param name="myCurrentVersion">The current version of the plugin interface</param>
        /// <param name="myMinVersion">The minimum expected verion of the plugin interface</param>
        /// <param name="myMaxVersion">The maximum expected verion of the plugin interface</param>
        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion,
                                                  Version myMinVersion, Version myMaxVersion)
        {
            PluginAssembly = myPluginAssembly;
            CurrentVersion = myCurrentVersion;
            MinVersion = myMinVersion;
            MaxVersion = myMaxVersion;
        }        

        #endregion
    }
}