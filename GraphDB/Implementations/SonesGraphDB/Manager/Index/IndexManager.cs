using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.Plugin;
using System.Collections.Generic;
using sones.Library.VersionedPluginManager;
using sones.Library.Settings;

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

        private readonly GraphDBPluginManager _pluginManager;

        #endregion

        #region constructor

        public IndexManager(GraphApplicationSettings myApplicationSettings, GraphDBPluginManager myPluginManager, HashSet<PluginDefinition> myPluginDefinitions = null)
        {
            
        }

        #endregion


        public void CreateIndex(IIndexDefinition myIndexDefinition) 
        {
            throw new System.NotImplementedException();
        }
    }
}
