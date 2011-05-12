/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Returns the pluginnames for a special type.
        /// </summary>
        /// <typeparam name="T">The plugin type.</typeparam>
        /// <returns>An ienumerable with the plugin names.</returns>
        public IEnumerable<String> GetPluginsForType<T>()
        {
            var type = typeof(T);
            var result = new List<String>();

            lock (_plugins)
            {
                foreach (var item in _plugins)
                {
                    if (item.Key == type)
                    {
                        result.AddRange(item.Value.Keys.AsEnumerable());
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Is there a certain plugin?
        /// </summary>
        /// <typeparam name="T">The type of the plugin</typeparam>
        /// <param name="myPluginName">The name of the plugin</param>
        /// <returns>True or false</returns>
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
        /// <param name="UniqueID">An ID that is unique for the combination <paramref name="myPluginName"/> and <typeparamref name="T"/>.</param>
        /// <returns>A T</returns>
        public T GetAndInitializePlugin<T>(String myPluginName, Dictionary<String, Object> myParameter = null, long? UniqueID = null)
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
                var uniqueString = String.Join("-", 
                    typeof(T).Name, 
                    myPluginName, 
                    (UniqueID == null) ? Guid.NewGuid().ToString() : UniqueID.ToString());

                return (T)interestingLookup[myPluginName.ToUpper()].InitializePlugin(uniqueString, myParameter);
            }
        }

        #endregion

        #region protected helper

        /// <summary>
        /// Fill a certain lookup dictionary
        /// </summary>
        /// <typeparam name="T1">The type of the plugin</typeparam>
        protected void FillLookup<T1>(String myRequestingComponent, Func<T1, String> myConverter = null)
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

                    interestingLookup.Add((myConverter != null ? myConverter(aPlugin) : pluginName).ToUpper(), ((IPluginable)aPlugin));

                    #endregion

                }
            }
        }

        #endregion
    }
}
