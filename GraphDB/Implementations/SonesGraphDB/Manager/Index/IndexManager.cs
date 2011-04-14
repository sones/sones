using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.Plugin;
using System.Collections.Generic;
using sones.Library.VersionedPluginManager;
using sones.Library.Settings;
using System.Linq;
using System;

namespace sones.GraphDB.Manager.Index
{
    /// <summary>
    /// This class represents an index manager.
    /// </summary>
    /// The responsibilities of the index manager are creating, removing und retrieving of indices.
    /// Each database has one index manager.
    public class IndexManager : IIndexManager
    {
        #region data

        /// <summary>
        /// The plugin manager that is used to generate new instances of indices
        /// </summary>
        private readonly GraphDBPluginManager _pluginManager;

        /// <summary>
        /// The potential parameters for plugin-indices
        /// </summary>
        private readonly Dictionary<string, PluginDefinition> _indexPluginParameter;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new index manager
        /// </summary>
        /// <param name="myPluginManager">The sones graphDB plugin manager</param>
        /// <param name="myPluginDefinitions">The parameters for plugin-indices</param>
        public IndexManager(GraphDBPluginManager myPluginManager, HashSet<PluginDefinition> myPluginDefinitions = null)
        {
            _pluginManager = myPluginManager;

            if (myPluginDefinitions != null)
            {
                _indexPluginParameter = myPluginDefinitions.ToDictionary(key => key.NameOfPlugin, value => value);
            }
            else
            {
                _indexPluginParameter = new Dictionary<string, PluginDefinition>();
            }
        }

        #endregion


        #region IIndexManager Members

        public void CreateIndex(IIndexDefinition myIndexDefinition)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
