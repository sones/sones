using System;
using System.Reflection;
using sones.Library.VersionedPluginManager;

namespace sones.Library.VersionedPluginManager
{

    #region PluginIncompatibleVersionEvent

    public delegate void PluginIncompatibleVersionEvent(
        PluginManager myPluginManager, PluginIncompatibleVersionEventArgs myPluginIncompatibleVersionEventArgs);

    public class PluginIncompatibleVersionEventArgs : EventArgs
    {
        #region Properties

        public Version PluginVersion { get; private set; }
        public Version MinVersion { get; private set; }
        public Version MaxVersion { get; private set; }
        public Type PluginType { get; private set; }
        public Assembly PluginAssembly { get; private set; }

        #endregion

        public PluginIncompatibleVersionEventArgs(Assembly myPluginAssembly, Version myPluginVersion,
                                                  Version myMinVersion, Version myMaxVersion, Type myPluginType)
        {
            PluginVersion = myPluginVersion;
            MinVersion = myMinVersion;
            MaxVersion = myMaxVersion;
            PluginType = myPluginType;
            PluginAssembly = myPluginAssembly;
        }
    }

    #endregion
}