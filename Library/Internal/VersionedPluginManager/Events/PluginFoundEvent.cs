using System;

namespace sones.Library.VersionedPluginManager
{

    #region PluginFoundEvent

    public delegate void PluginFoundEvent(PluginManager myPluginManager, PluginFoundEventArgs myPluginFoundEventArgs);

    public class PluginFoundEventArgs : EventArgs
    {
        public PluginFoundEventArgs(Type myPluginType, object myPluginInstance)
        {
            PluginType = myPluginType;
            PluginInstance = myPluginInstance;
        }

        public Type PluginType { get; private set; }
        public object PluginInstance { get; private set; }
    }

    #endregion
}