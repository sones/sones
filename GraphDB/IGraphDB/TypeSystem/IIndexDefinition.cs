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
        IEnumerable<IAttributeDefinition> IndexedProperties { get; }
    }
}
