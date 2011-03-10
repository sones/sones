using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace sones.VersionedPluginManager.ErrorHandling.Events
{

    #region PluginIncompatibleVersionEvent

    public delegate void PluginIncompatibleVersionEvent(PluginManager myPluginManager, PluginIncompatibleVersionEventArgs myPluginIncompatibleVersionEventArgs);
    public class PluginIncompatibleVersionEventArgs : EventArgs
    {

        #region Properties

        public Version PluginVersion { get; private set; }
        public Version MinVersion { get; private set; }
        public Version MaxVersion { get; private set; }
        public Type PluginType { get; private set; }
        public Assembly PluginAssembly { get; private set; }

        #endregion

        public PluginIncompatibleVersionEventArgs(Assembly myPluginAssembly, Version myPluginVersion, Version myMinVersion, Version myMaxVersion, Type myPluginType)
        {
            this.PluginVersion = myPluginVersion;
            this.MinVersion = myMinVersion;
            this.MaxVersion = myMaxVersion;
            this.PluginType = myPluginType;
            this.PluginAssembly = myPluginAssembly;
        }

    }

    #endregion
    
}
