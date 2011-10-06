using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using sones.GraphDB.TypeSystem;
using GraphDSRemoteClient.GraphElements;
using GraphDSRemoteClient.TypeManagement;
using sones.GraphDB.Expression;

namespace GraphDSRemoteClient
{
    internal static class ConvertHelper
    {
        internal static IBaseType ToBaseType(ServiceBaseType myBaseType, IServiceToken myServiceToken)
        {
            IBaseType result = null;
            if (myBaseType is ServiceVertexType)
            {
                result = new RemoteVertexType((ServiceVertexType)myBaseType, myServiceToken);
            }
            else if (myBaseType is ServiceEdgeType)
            {
                result = new RemoteEdgeType((ServiceEdgeType)myBaseType, myServiceToken);
            }
            return result;
        }

        internal static IAttributeDefinition ToAttributeDefinition(ServiceAttributeDefinition mySvcAttributeDefinition, IServiceToken myServiceToken)
        {
            IAttributeDefinition AttributeDefinition = null;
            switch(mySvcAttributeDefinition.Kind)
            {
                case ServiceAttributeType.Property:
                    AttributeDefinition = new RemotePropertyDefinition((ServicePropertyDefinition)mySvcAttributeDefinition, myServiceToken);
                    break;
                case ServiceAttributeType.BinaryProperty:
                    throw new NotImplementedException();
                case ServiceAttributeType.IncomingEdge:
                    AttributeDefinition = new RemoteIncomingEdgeDefinition((ServiceIncomingEdgeDefinition)mySvcAttributeDefinition, myServiceToken);
                    break;
                case ServiceAttributeType.OutgoingEdge:
                    AttributeDefinition = new RemoteOutgoingEdgeDefinition((ServiceOutgoingEdgeDefinition)mySvcAttributeDefinition, myServiceToken);
                    break;
            }
            return AttributeDefinition;
        }

        internal static ServiceBaseExpression ToServiceExpression(IExpression myExpression)
        {
            ServiceBaseExpression expression = null;
            if (myExpression is BinaryExpression)
            {
                expression = new ServiceBinaryExpression((BinaryExpression)myExpression);
            }
            else if (myExpression is PropertyExpression)
            {
                expression = new ServicePropertyExpression((PropertyExpression)myExpression);
            }
            else if (myExpression is SingleLiteralExpression)
            {
                expression = new ServiceSingleLiteralExpression((SingleLiteralExpression)myExpression);
            }
            else if (myExpression is CollectionLiteralExpression)
            {
                expression = new ServiceCollectionLiteralExpression((CollectionLiteralExpression)myExpression);
            }
            else if (myExpression is RangeLiteralExpression)
            {
                expression = new ServiceRangeLiteralExpression((RangeLiteralExpression)myExpression);
            }
            else if (myExpression is UnaryExpression)
            {
                expression = new ServiceUnaryExpression((UnaryExpression)myExpression);
            }
            return expression;
        }
    }
}
