using System;

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// The sub type of all 
    /// </summary>
    public interface IAttributeDefinition
    {
        /// <summary>
        /// The name, that is unique for all attributes on a vertex type.
        /// </summary>
        String Name { get; }
        
        /// <summary>
        /// A identifier, that is unique for all attributes on a vertex type.
        /// </summary>
        Int64 AttributeID { get; }

        /// <summary>
        /// The type of the attribute        
        /// </summary>
        /// <remarks>
        /// If Kind is <c>Property</c> you can cast this IAttributeDefinition to IPropertyDefinition.
        /// If Kind is <c>IncomingEdge</c> you can cast this IAttributeDefinition to IIncomingEdgeDefintiton.
        /// If Kind is <c>OutgoingEdge</c> you can cast this IAttributeDefinition to IOutgoingEdgeDefinition.
        /// </remarks>
        AttributeType Kind { get; }
    }
}
