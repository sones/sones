using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.VersionedPluginManager.ErrorHandling.Events
{

    #region PluginFoundEvent

    public delegate void PluginFoundEvent(PluginManager myPluginManager, PluginFoundEventArgs myPluginFoundEventArgs);
    public class PluginFoundEventArgs : EventArgs
    {
        public Type PluginType { get; private set; }
        public object PluginInstance { get; private set; }

        public PluginFoundEventArgs(Type myPluginType, object myPluginInstance)
        {
            this.PluginType = myPluginType;
            this.PluginInstance = myPluginInstance;
        }

    }

    #endregion

}
