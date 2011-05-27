using System;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Extensions
{
    public static class IAttributeDefinitionExtension
    {
        public static bool HasValue(this IAttributeDefinition myAttribute, IGraphElement myElement)
        {
            if (myAttribute == null)
                throw new NullReferenceException();

            if (myElement == null)
                throw new ArgumentNullException("myElement");

            switch (myAttribute.Kind)
            {
                case AttributeType.Property:
                    return IPropertyDefintionExtension.HasValue(myAttribute as IPropertyDefinition, myElement);
                case AttributeType.OutgoingEdge:
                    return IOutgoingEdgeDefinitionExtension.HasValue(myAttribute as IOutgoingEdgeDefinition, myElement as IVertex);
                case AttributeType.IncomingEdge:
                    return IIncomingEdgeDefinitionExtension.HasValue(myAttribute as IIncomingEdgeDefinition, myElement as IVertex);
                case AttributeType.BinaryProperty:
                    return IBinaryPropertyDefintionExtension.HasValue(myAttribute as IBinaryPropertyDefinition, myElement as IVertex);
                default:
                    throw new Exception("AttributeType enumeration was changed, but not this switch statement.");
            }
        }
    }
}
