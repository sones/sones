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

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents an IncomingEdge type.
    /// </summary>
    public interface IEdgeType: IBaseType
    {

        #region Inheritance

        /// <summary>
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IEdgeType ParentEdgeType { get; }

        /// <summary>
        /// Get all child edge types.
        /// </summary>
        /// <param name="myRecursive">Include all dexcendant.</param>
        /// <param name="myIncludeSelf">If true, this edge type will be included to the result list.</param>
        /// <returns>An enumerable of child vertex types, never <c>NULL</c>.</returns>
        IEnumerable<IEdgeType> GetChildEdgeTypes(bool myRecursive = true, bool myIncludeSelf = false);

        #endregion

    }
}
