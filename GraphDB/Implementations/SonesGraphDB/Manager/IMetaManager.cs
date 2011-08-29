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
using System.Text;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Manager.Vertex;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Manager.QueryPlan;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager.BaseGraph;
using sones.Library.VersionedPluginManager;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A meta manager that aggregates all GraphDB manager
    /// </summary>
    public interface IMetaManager
    {
        /// <summary>
        /// The interface to the query plan manager
        /// </summary>
        IQueryPlanManager QueryPlanManager { get; }

        /// <summary>
        /// The interface to the indices
        /// </summary>
        IIndexManager IndexManager { get; }

        /// <summary>
        /// The interface to the vertex types
        /// </summary>
        IManagerOf<ITypeHandler<IVertexType>> VertexTypeManager { get; }

        /// <summary>
        /// The interface to the edge types
        /// </summary>
        IManagerOf<ITypeHandler<IEdgeType>> EdgeTypeManager { get; }

        /// <summary>
        /// <summary>
        /// The managed interface to the vertices
        /// </summary>
        IManagerOf<IVertexHandler> VertexManager { get; }

        /// <summary>
        /// The raw interface to the interfaces
        /// </summary>
        IVertexStore VertexStore { get; }

        /// <summary>
        /// The base graph storage manager.
        /// </summary>
        BaseGraphStorageManager BaseGraphStorageManager { get; }

        /// The plugin manager.
        /// </summary>
        AComponentPluginManager PluginManager { get; }

        /// <summary>
        /// The base type manager.
        /// </summary>
        BaseTypeManager BaseTypeManager { get; }

        /// <summary>
        /// The security token for graph db intern usage.
        /// </summary>
        SecurityToken SystemSecurityToken { get; }
    }
}
