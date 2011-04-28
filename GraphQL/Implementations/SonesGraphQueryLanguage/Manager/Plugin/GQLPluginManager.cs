using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Interfaces;
using sones.Plugins.Index;
using sones.Plugins.SonesGQL;
using sones.Plugins.SonesGQL.Functions;
using sones.Plugins.SonesGQL.Aggregates;
using sones.Plugins.SonesGQL.DBImport;

namespace sones.GraphQL.GQL.Manager.Plugin
{
    /// <summary>
    /// A plugin manager for the sones GQL component
    /// </summary>
    public sealed class GQLPluginManager : AComponentPluginManager
    {
        #region constructor

        /// <summary>
        /// Create a new sones GQL plugin manager
        /// </summary>
        public GQLPluginManager()
        {
            #region Register & Discover

            // Change the version if there are ANY changes which will prevent loading the plugin.
            // As long as there are still some plugins which does not have their own assembly you need to change the compatibility of ALL plugins of the GraphDB and GraphFSInterface assembly.
            // So, if any plugin in the GraphDB changes you need to change the AssemblyVersion of the GraphDB AND modify the compatibility version of the other plugins.
            _pluginManager
            #region functions

                .Register<IGQLFunction>(IGQLFunctionVersionCompatibility.MinVersion, IGQLFunctionVersionCompatibility.MaxVersion)

            #endregion

            #region aggregates

                .Register<IGQLAggregate>(IGQLAggregateVersionCompatibility.MinVersion, IGQLAggregateVersionCompatibility.MaxVersion)

            #endregion

            #region importer

                .Register<IGraphDBImport>(IGraphDBImportVersionCompatibility.MaxVersion, IGraphDBImportVersionCompatibility.MaxVersion)

            #endregion

            #region indices

                .Register<ISingleValueIndex<IComparable, Int64>>(ISonesIndexVersionCompatibility.MinVersion, ISonesIndexVersionCompatibility.MaxVersion)
                .Register<IVersionedIndex<IComparable, Int64, Int64>>(ISonesIndexVersionCompatibility.MinVersion, ISonesIndexVersionCompatibility.MaxVersion)
                .Register<IMultipleValueIndex<IComparable, Int64>>(ISonesIndexVersionCompatibility.MinVersion, ISonesIndexVersionCompatibility.MaxVersion);

            #endregion

            _pluginManager.Discover();

            #endregion

            #region Get all plugins and fill the lookup dictionaries

            var componentName = this.GetType().Assembly.GetName().Name;

            FillLookup<ISingleValueIndex<IComparable, Int64>>(componentName);
            FillLookup<IVersionedIndex<IComparable, Int64, Int64>>(componentName);
            FillLookup<IMultipleValueIndex<IComparable, Int64>>(componentName);
            FillLookup<IGQLAggregate>(componentName);
            FillLookup<IGQLFunction>(componentName);
            FillLookup<IGraphDBImport>(componentName);

            #endregion
        }

        #endregion
    }
}
