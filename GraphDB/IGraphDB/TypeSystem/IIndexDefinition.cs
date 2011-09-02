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
    /// An interface that represents a definition of an index.
    /// </summary>
    public interface IIndexDefinition
    {
        /// <summary>
        /// The user defined index name. Maybe <c>NULL</c>.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// A identifier, that is unique for all indices.
        /// </summary>
        Int64 ID { get; }

        /// <summary>
        /// The name of the index type. It can be used to get the index implementation from the index manager.
        /// Never <c>NULL</c>.
        /// </summary>
        String IndexTypeName { get; }

        /// <summary>
        /// The edition name of the index
        /// </summary>
        String Edition { get; }

        /// <summary>
        /// Determines if this index has been created by a user
        /// </summary>
        Boolean IsUserdefined { get; }

        /// <summary>
        /// The attributes that are indexed. Never <c>NULL</c>. 
        /// </summary>
        IList<IPropertyDefinition> IndexedProperties { get; }

        /// <summary>
        /// The defining vertex type.
        /// </summary>
        IVertexType VertexType { get; }

        /// <summary>
        /// The index definition, that causes that index
        /// </summary>
        IIndexDefinition SourceIndex { get; }

        bool IsRange { get; }

        bool IsVersioned { get; }
    }
}
