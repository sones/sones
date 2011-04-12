using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Settings;

namespace sones.Library.VersionedPluginManager
{
    /// <summary>
    /// The interface for all pluginable components
    /// </summary>
    public interface IPluginable
    {
        /// <summary>
        /// The unique name of the plugin
        /// </summary>
        String PluginName { get; }

        /// <summary>
        /// The parameters that are settable for this plugin
        /// </summary>
        Dictionary<String, Type> SetableParameters { get; }

        /// <summary>
        /// A method to initialize a plugin
        /// </summary>
        /// <param name="myParameters">The parameters for the plugin</param>
        /// <param name="myApplicationSetting">The application settings</param>
        void InitializePlugin(Dictionary<String, Object> myParameters, GraphApplicationSettings myApplicationSetting);
    }
}
