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
    /// The interface for vertices
    /// </summary>
    public interface IVertexProperties
    {
        #region ID / Edition / Revision

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        Int64 VertexTypeID { get; }

        /// <summary>
        /// The id of the vertex
        /// </summary>
        Int64 VertexID { get; }

        /// <summary>
        /// Returns the revision id of this vertex
        /// </summary>
        Int64 VertexRevisionID { get; }

        /// <summary>
        /// Returns the name of the edition of this vertex
        /// </summary>
        String EditionName { get; }

        #endregion

        #region Statistics

        /// <summary>
        /// Statistics concerning the vertex
        /// </summary>
        IVertexStatistics Statistics { get; }

        #endregion

        #region PartitionInformation

        /// <summary>
        /// Informations concerning the current partition
        /// </summary>
        IGraphPartitionInformation PartitionInformation { get; }

        #endregion
    }
}