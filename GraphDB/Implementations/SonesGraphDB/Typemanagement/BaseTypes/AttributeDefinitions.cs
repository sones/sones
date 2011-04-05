using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    /// <summary>
    /// This static class contains all attribute definitions.
    /// </summary>
    /// We use a summary file, that contains attribute definitions, because some attributes are used in multiple vertex types
    /// and we want to be sure, that the AttributeIDs are similar for VertexTypes
    /// 
    internal static class AttributeDefinitions
    {
        internal static readonly IPropertyDefinition ID = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue,
            BaseType = typeof(UInt64),
            IsMandatory = true,
            Kind = AttributeType.Property,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "ID"
        };

        internal static readonly IPropertyDefinition Name = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + 1,
            BaseType = typeof(String),
            IsMandatory = true,
            Kind = AttributeType.Property,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "Name"
        };

        internal static readonly IPropertyDefinition Comment = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + 2,
            BaseType = typeof(String),
            IsMandatory = false,
            Kind = AttributeType.Property,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "Comment"
        };

        internal static readonly IPropertyDefinition IsAbstract = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + 3,
            BaseType = typeof(Boolean),
            IsMandatory = true,
            Kind = AttributeType.Property,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsAbstract"
        };

        internal static readonly IPropertyDefinition IsSealed = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + 4,
            BaseType = typeof(Boolean),
            IsMandatory = true,
            Kind = AttributeType.Property,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsSealed"
        };

        internal static readonly IIncomingEdgeDefinition Attributes = new IncomingEdgeDefinition() 
        {
            AttributeID = Int64.MinValue + 5, 
            Kind = AttributeType.IncomingEdge, 
            Name = "Attributes", 
            RelatedEdgeDefinition = DefiningType
            
        };

        internal static readonly IOutgoingEdgeDefinition DefiningType = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + 6,
            EdgeType = NormalEdgeType.Instance,
            Kind = AttributeType.OutgoingEdge,
            Name = "DefiningType",
            SourceVertexType = AttributeVertexType.Instance,
            TargetVertexType = BaseTypeVertexType.Instance
        };

        internal static readonly IOutgoingEdgeDefinition TypeOnAttribute = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + 7,
            EdgeType = NormalEdgeType.Instance,
            Kind = TypeSystem.AttributeType.OutgoingEdge,
            Name = "Type",
            SourceVertexType = AttributeVertexType.Instance,
            TargetVertexType = BaseTypeVertexType.Instance
        };
    }

}
