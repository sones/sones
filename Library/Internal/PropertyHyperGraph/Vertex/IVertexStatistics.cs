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

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for vertex statistics
    /// </summary>
    public interface IVertexStatistics : IGraphElementStatistics
    {
        #region Degree

        /// <summary>
        /// For a vertex, the number of incoming edges is called the indegree 
        /// </summary>
        UInt64 InDegree { get; }

        /// <summary>
        /// For a vertex, the number outgoing edges is called the outdegree 
        /// </summary>
        UInt64 OutDegree { get; }

        /// <summary>
        /// For a vertex the number of incoming plus the number of outgoing edges is called the degree
        /// </summary>
        UInt64 Degree { get; }

        #endregion
    }
}