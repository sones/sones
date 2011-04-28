using System;
using System.Collections.Generic;

namespace sones.Library.VersionedPluginManager
{
    /// <summary>
    /// This class is used to specify which plugin should be used
    /// </summary>
    public sealed class PluginDefinition
    {
        #region data

        /// <summary>
        /// The name of the plugin
        /// </summary>
        public readonly String NameOfPlugin;

        /// <summary>
        /// The parameters for plugins
        /// </summary>
        public readonly Dictionary<String, Object> PluginParameter;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new plugin definition
        /// </summary>
        /// <param name="myPluginName">The name of the plugin</param>
        /// <param name="myParameter">The parameter for this plugin</param>
        public PluginDefinition(String myPluginName, Dictionary<String, Object> myParameter = null)
        {
            NameOfPlugin = myPluginName;

            if (myParameter != null)
            {
                PluginParameter = myParameter;
            }
            else
            {
                PluginParameter = new Dictionary<string, object>();
            }
        }

        #endregion

        #region fluid methods

        /// <summary>
        /// Fluid method to add a parameter
        /// </summary>
        /// <param name="myParameterName">The name of the parameter</param>
        /// <param name="myParameterValue">The value of the parameter</param>
        /// <returns>The plugindefinition itself</returns>
        public PluginDefinition AddParameter(String myParameterName, Object myParameterValue)
        {
            PluginParameter[myParameterName] = myParameterValue;

            return this;
        }

        #endregion

    }
}
