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
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index;
using sones.Plugins.Index.Versioned;
using sones.Plugins.SonesGQL.Aggregates;
using sones.Plugins.SonesGQL.DBExport;
using sones.Plugins.SonesGQL.DBImport;
using sones.Plugins.SonesGQL.Functions;
using sones.Plugins.SonesGQL.Statements;

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

                .Register<IGraphDBImport>(IGraphDBImportVersionCompatibility.MinVersion, IGraphDBImportVersionCompatibility.MaxVersion)

            #endregion

            #region exporter

                .Register<IGraphDBExport>(IGraphDBExportVersionCompatibility.MinVersion, IGraphDBExportVersionCompatibility.MaxVersion)

            #endregion

            #region indices

                .Register<ISonesIndex>(ISonesIndexVersionCompatibility.MinVersion, ISonesIndexVersionCompatibility.MaxVersion)
                .Register<ISonesVersionedIndex>(ISonesIndexVersionCompatibility.MinVersion, ISonesIndexVersionCompatibility.MaxVersion)

            #endregion

            #region statements

                .Register<IGQLStatementPlugin>(IGQLStatementPluginVersionCompatibility.MinVersion, IGQLStatementPluginVersionCompatibility.MaxVersion);

            #endregion

            _pluginManager.Discover();

            #endregion

            #region Get all plugins and fill the lookup dictionaries

            var componentName = this.GetType().Assembly.GetName().Name;

            FillLookup<ISonesIndex>(componentName, _ => _.IndexName);
            FillLookup<ISonesVersionedIndex>(componentName, _ => _.IndexName);
            FillLookup<IGQLAggregate>(componentName, _ => _.AggregateName);
            FillLookup<IGQLFunction>(componentName, _ => _.FunctionName);
            FillLookup<IGraphDBImport>(componentName, _ => _.ImportFormat);
            FillLookup<IGraphDBExport>(componentName, _ => _.ExporterName);
            FillLookup<IGQLStatementPlugin>(componentName, _ => _.StatementName);

            #endregion
        }

        #endregion
    }
}
