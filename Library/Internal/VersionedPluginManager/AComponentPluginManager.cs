using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager.ErrorHandling;

namespace sones.Library.VersionedPluginManager
{
    /// <summary>
    /// The abstract class for all component related plugin manager
    /// </summary>
    public abstract class AComponentPluginManager
    {
        #region data

        /// <summary>
        /// The reflection machine
        /// </summary>
        protected readonly PluginManager _pluginManager;

        /// <summary>
        /// The datastructure that contains all the plugins for a certain component
        /// TypeOfPlugin(IGraphFS, ITransactionmanager,...) { NameOfPlugin, ThePluginItself}
        /// </summary>
        protected readonly Dictionary<Type, Dictionary<String, IPluginable>> _plugins;

        #endregion

        #region constructor

        /// <summary>
        /// A protected constructor to initialize the used data structures
        /// </summary>
        protected AComponentPluginManager()
        {
            _pluginManager = new PluginManager();
            _plugins = new Dictionary<Type, Dictionary<string, IPluginable>>();
        }

        #endregion

        #region public methods

        public bool HasPlugin<T>(String myPluginName)
        {
            var type = typeof(T);
            Dictionary<string, IPluginable> interestingLookup;

            lock (_plugins)
            {
                if (!_plugins.ContainsKey(type))
                {
                    return false;
                }

                interestingLookup = _plugins[type];
            }

            lock (interestingLookup)
            {
                if (!interestingLookup.ContainsKey(myPluginName.ToUpper()))
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Get and initalize a certain plugin and...
        /// During initialization a new instance of the plugin is created
        /// </summary>
        /// <typeparam name="T">The interface of the Plugin</typeparam>
        /// <param name="myPluginName">The name of the plugin</param>
        /// <param name="myParameter">The parameters that are necessary to initialize an IPluginable</param>
        /// <param name="myApplicationSetting">The application settings that are necessary to initialize an IPluginable</param>
        /// <returns>A T</returns>
        public T GetAndInitializePlugin<T>(String myPluginName, Dictionary<String, Object> myParameter, GraphApplicationSettings myApplicationSetting)
        {
            var type = typeof(T);
            Dictionary<string, IPluginable> interestingLookup;

            lock (_plugins)
            {
                if (!_plugins.ContainsKey(type))
                {
                    throw new UnknownPluginException(myPluginName, type);
                }

                interestingLookup = _plugins[type];
            }

            lock (interestingLookup)
            {
                if (!interestingLookup.ContainsKey(myPluginName.ToUpper()))
                {
                    throw new UnknownPluginException(myPluginName, type);
                }

                return (T)interestingLookup[myPluginName.ToUpper()].InitializePlugin(myParameter, myApplicationSetting);
            }
        }

        #endregion

        #region protected helper

        /// <summary>
        /// Fill a certain lookup dictionary
        /// </summary>
        /// <typeparam name="T1">The type of the plugin</typeparam>
        protected void FillLookup<T1>(String myRequestingComponent)
        {
            var type = typeof(T1);
            Dictionary<string, IPluginable> interestingLookup;

            lock (_plugins)
            {
                if (!_plugins.ContainsKey(type))
                {
                    _plugins.Add(type, new Dictionary<string, IPluginable>());
                }

                interestingLookup = _plugins[type];
            }

            lock (interestingLookup)
            {
                foreach (var aPlugin in _pluginManager.GetPlugins<T1>())
                {
                    var pluginName = (aPlugin as IPluginable).PluginName.ToUpper();

                    //verify that there are no duplicates
                    if (interestingLookup.ContainsKey(pluginName))
                    {
                        throw new DuplicatePluginException(pluginName, typeof(T1), myRequestingComponent);
                    }

                    #region Add function if the name does not exist

                    interestingLookup.Add(pluginName, ((IPluginable)aPlugin));

                    #endregion

                }
            }
        }

        #endregion
    }
}
