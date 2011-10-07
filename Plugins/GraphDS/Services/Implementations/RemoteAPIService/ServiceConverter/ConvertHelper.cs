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
    }
}
