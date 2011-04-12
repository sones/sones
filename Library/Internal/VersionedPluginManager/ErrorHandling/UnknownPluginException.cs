using System;
using sones.Library.ErrorHandling;
using sones.Library.VersionedPluginManager.ErrorHandling;

namespace sones.Library.VersionedPluginManager.ErrorHandling
{
    /// <summary>
    /// There's a unknown plugin
    /// </summary>
    public sealed class UnknownPluginException : APluginManagerException
    {
        #region data

        /// <summary>
        /// The name of the plugin
        /// </summary>
        public String PluginName { get; private set; }

        /// <summary>
        /// The type of the plugin
        /// </summary>
        public Type PluginType { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new unknown plugin exception
        /// </summary>
        /// <param name="myUnknownPluginName">The name of the unknown plugin</param>
        /// <param name="myPluginType">The type of the unknown plugin</param>
        public UnknownPluginException(String myUnknownPluginName, Type myPluginType)
        {
            PluginName = myUnknownPluginName;
            PluginType = myPluginType;
            
            _msg = String.Format("{0} : The {1} plugin name is unknown!", PluginName, PluginType.Name);
        }

        #endregion
                
                
    }
}
