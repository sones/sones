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

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents an uniqueness definition for attributes on an vertex type definition.
    /// </summary>
    public interface IUniqueDefinition
    {
        /// <summary>
        /// The attributes that are unique together.
        /// </summary>
        /// <returns>A set of attribute definitions that together must be unique. Never <c>NULL</c>.</returns>
        IEnumerable<IPropertyDefinition> UniquePropertyDefinitions { get; }

        /// <summary>
        /// The vertex type that defines the uniqueness.
        /// </summary>
        IVertexType DefiningVertexType { get; }

        /// <summary>
        /// The index definition, that forces the unique constraint.
        /// </summary>
        IIndexDefinition CorrespondingIndex { get; }
    }
}
