using System;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Extensions
{
    public static class IAttributeDefinitionExtension
    {
        public static bool HasValue(this IAttributeDefinition myAttribute, IVertex myVertex)
        {
            if (myAttribute == null)
                throw new NullReferenceException();

            switch (myAttribute.Kind)
            {
                case AttributeType.Property:
                    return IPropertyDefintionExtension.HasValue(myAttribute as IPropertyDefinition, myVertex);
                case AttributeType.OutgoingEdge:
                    return IOutgoingEdgeDefinitionExtension.HasValue(myAttribute as IOutgoingEdgeDefinition, myVertex);
                case AttributeType.IncomingEdge:
                    return IIncomingEdgeDefinitionExtension.HasValue(myAttribute as IIncomingEdgeDefinition, myVertex);
                case AttributeType.BinaryProperty:
                    return IBinaryPropertyDefintionExtension.HasValue(myAttribute as IBinaryPropertyDefinition, myVertex);
                default:
                    throw new Exception("AttributeType enumeration was changed, but not this switch statement.");
            }
        }
    }
}
