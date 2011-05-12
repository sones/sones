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

using System.Collections.Generic;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for all edge species
    /// </summary>
    public interface IEdge : IGraphElement, IEdgeProperties
    {
        #region Source

        /// <summary>
        /// Get the source vertex of the edge
        /// </summary>
        /// <returns>The source vertex</returns>
        IVertex GetSourceVertex();

        #endregion

        #region Targets

        /// <summary>
        /// Get all target vertices
        /// </summary>
        /// <param name="myFilter">A function to filter vertices</param>
        /// <returns>A IEnumerable of vertices</returns>
        IEnumerable<IVertex> GetTargetVertices(PropertyHyperGraphFilter.TargetVertexFilter myFilter = null);

        #endregion
    }
}