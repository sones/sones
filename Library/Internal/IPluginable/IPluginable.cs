using System;
using System.Collections.Generic;

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
        /// <param name="UniqueString">A string, that is unique for all plugins in the system.</param>
        /// <returns>A new instance of the plugin</returns>
        IPluginable InitializePlugin(String UniqueString, Dictionary<String, Object> myParameters = null);
    }
}
