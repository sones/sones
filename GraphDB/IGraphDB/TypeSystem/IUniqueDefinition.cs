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
    }
}
