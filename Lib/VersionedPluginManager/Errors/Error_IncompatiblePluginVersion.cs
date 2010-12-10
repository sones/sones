using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;

namespace sones.Lib.VersionedPluginManager.Errors
{
    /// <summary>
    /// This error occurs if a version of a plugin does not match the expected version.
    /// </summary>
    public class Error_IncompatiblePluginVersion : GeneralError
    {
        private   System.Reflection.Assembly PluginAssembly;
        private   Version CurrentVersion;
        private   Version MinVersion;
        private   Version MaxVersion;

        /// <summary>
        /// The ctor
        /// </summary>
        /// <param name="myPluginAssembly">The pluginassembly which implements the incompatible version.</param>
        /// <param name="myCurrentVersion">The current plugin version.</param>
        /// <param name="myMinVersion">The minimum accepted version.</param>
        /// <param name="myMaxVersion">The maximum accepted version.</param>
        public Error_IncompatiblePluginVersion(System.Reflection.Assembly myPluginAssembly, Version myCurrentVersion, Version myMinVersion, Version myMaxVersion)
        {
            this.PluginAssembly = myPluginAssembly;
            this.CurrentVersion   = myCurrentVersion;
            this.MinVersion       = myMinVersion;
            this.MaxVersion       = myMaxVersion;
        }

        public override string ToString()
        {
            if (MaxVersion != null)
            {
                return String.Format("The plugin '{0}' at '{1}' is of a not supported version {2}. The minimum version is '{3}' and the maximum version is '{4}'!",
                    PluginAssembly, PluginAssembly.Location, CurrentVersion, MinVersion, MaxVersion);
            }
            else
            {
                return String.Format("The plugin '{0}' at '{1}' is of a not supported version {2}. The minimum version is '{3}'!",
                    PluginAssembly, PluginAssembly.Location, CurrentVersion, MinVersion);
            }
        }

    }
}
