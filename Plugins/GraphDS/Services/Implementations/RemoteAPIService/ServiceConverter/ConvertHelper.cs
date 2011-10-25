using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceConverter
{
    public static class ConvertHelper
    {
        public static object ToServiceObject(object myObject)
        {
            if (myObject is PropertyMultiplicity)
                return ToServicePropertyMultiplicity((PropertyMultiplicity)myObject);
            else if (myObject is EdgeMultiplicity)
                return ToServiceEdgeMultiplicity((EdgeMultiplicity)myObject);
            else if (myObject is IIndexDefinition)
                return new ServiceIndexDefinition((IIndexDefinition)myObject);
            else
                return null;
        }

        public static ServiceAttributeDefinition ToServiceAttributeDefinition(IAttributeDefinition myAttributeDefinition)
        {
            ServiceAttributeDefinition svcAttributeDef = null;
            switch (myAttributeDefinition.Kind)
            {
                case AttributeType.Property:
                    svcAttributeDef = new ServicePropertyDefinition((IPropertyDefinition)myAttributeDefinition);
                    break;
                case AttributeType.OutgoingEdge:
                    svcAttributeDef = new ServiceOutgoingEdgeDefinition((IOutgoingEdgeDefinition)myAttributeDefinition);
                    break;
                case AttributeType.IncomingEdge:
                    svcAttributeDef = new ServiceIncomingEdgeDefinition((IIncomingEdgeDefinition)myAttributeDefinition);
                    break;
                case AttributeType.BinaryProperty:
                    svcAttributeDef = new ServiceBinaryPropertyDefinition((IBinaryPropertyDefinition)myAttributeDefinition);
                    break;
            }
            return svcAttributeDef;
        }

        public static ServiceBaseType ToServiceBaseType(IBaseType myBaseType)
        {
            ServiceBaseType svcBaseType = null;
            if (myBaseType is IVertexType)
            {
                svcBaseType = new ServiceVertexType((IVertexType)myBaseType);
            }
            else if (myBaseType is IEdgeType)
            {
                svcBaseType = new ServiceEdgeType((IEdgeType)myBaseType);
            }
            return svcBaseType;
        }

        /// <summary>
        /// Converts EdgeMultiplicity into serializable ServiceEdgeMultiplicity, default: SingleEdge.
        /// </summary>
        /// <param name="myMultiplicity"></param>
        /// <returns></returns>
        public static ServiceEdgeMultiplicity ToServiceEdgeMultiplicity(EdgeMultiplicity myMultiplicity)
        {
            ServiceEdgeMultiplicity multiplicity;
            switch (myMultiplicity)
            {
                case EdgeMultiplicity.MultiEdge:
                    multiplicity = ServiceEdgeMultiplicity.MultiEdge;
                    break;
                case EdgeMultiplicity.HyperEdge:
                    multiplicity = ServiceEdgeMultiplicity.HyperEdge;
                    break;
                default:
                    multiplicity = ServiceEdgeMultiplicity.SingleEdge;
                    break;
            }
            return multiplicity;
        }

        /// <summary>
        /// Converts PropertyMultiplicity into serializable ServicePropertyMultiplicity, default: Single.
        /// </summary>
        /// <param name="myMultiplicity"></param>
        /// <returns></returns>
        public static ServicePropertyMultiplicity ToServicePropertyMultiplicity(PropertyMultiplicity myMultiplicity)
        {
            ServicePropertyMultiplicity multiplicity;
            switch (myMultiplicity)
            {
                case PropertyMultiplicity.Set:
                    multiplicity = ServicePropertyMultiplicity.Set;
                    break;
                case PropertyMultiplicity.List:
                    multiplicity = ServicePropertyMultiplicity.List;
                    break;
                default:
                    multiplicity = ServicePropertyMultiplicity.Single;
                    break;
            }
            return multiplicity;
        }
    }
}
