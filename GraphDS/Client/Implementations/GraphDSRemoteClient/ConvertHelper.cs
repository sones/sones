using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.GraphElements;
using sones.GraphDS.GraphDSRemoteClient.TypeManagement;
using sones.GraphDB.Expression;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.GraphDSRemoteClient
{
    internal static class ConvertHelper
    {
        internal static object ToDsObject(object mySvcObject, IServiceToken myServiceToken)
        {
            if (mySvcObject is ServicePropertyMultiplicity)
                return ToPropertyMultiplicity((ServicePropertyMultiplicity)mySvcObject);
            else if (mySvcObject is ServiceEdgeMultiplicity)
                return ToEdgeMultiplicity((ServiceEdgeMultiplicity)mySvcObject);
            else if (mySvcObject is ServiceIndexDefinition)
                return new RemoteIndexDefinition((ServiceIndexDefinition)mySvcObject, myServiceToken);
            else
                return null;
        }

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

        internal static ServiceEdgeInstance ToServiceEdgeInstance(IEdge myEdge)
        {
            ServiceEdgeInstance svcEdge;
            if (myEdge is ISingleEdge)
            {
                svcEdge = new ServiceSingleEdgeInstance((ISingleEdge)myEdge);
            }
            else
            {
                svcEdge = new ServiceHyperEdgeInstance((IHyperEdge)myEdge);
            }
            return svcEdge;
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

        #region EdgeMultiplicity
        /// <summary>
        /// Converts EdgeMultiplicity into serializable ServiceEdgeMultiplicity, default: SingleEdge.
        /// </summary>
        /// <param name="myMultiplicity"></param>
        /// <returns></returns>
        internal static ServiceEdgeMultiplicity ToServiceEdgeMultiplicity(EdgeMultiplicity myMultiplicity)
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
        /// Converts serializable ServiceEdgeMultiplicity into EdgeMultiplicity, default: SingleEdge.
        /// </summary>
        /// <param name="myMultiplicity"></param>
        /// <returns></returns>
        internal static EdgeMultiplicity ToEdgeMultiplicity(ServiceEdgeMultiplicity myMultiplicity)
        {
            EdgeMultiplicity multiplicity;
            switch (myMultiplicity)
            {
                case ServiceEdgeMultiplicity.MultiEdge:
                    multiplicity = EdgeMultiplicity.MultiEdge;
                    break;
                case ServiceEdgeMultiplicity.HyperEdge:
                    multiplicity = EdgeMultiplicity.HyperEdge;
                    break;
                default:
                    multiplicity = EdgeMultiplicity.SingleEdge;
                    break;
            }
            return multiplicity;
        }
        #endregion

        #region PropertyMultiplicity
        /// <summary>
        /// Converts PropertyMultiplicity into serializable ServicePropertyMultiplicity, default: Single.
        /// </summary>
        /// <param name="myMultiplicity"></param>
        /// <returns></returns>
        internal static ServicePropertyMultiplicity ToServicePropertyMultiplicity(PropertyMultiplicity myMultiplicity)
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

        /// <summary>
        /// Converts serializable ServicePropertyMultiplicity into PropertyMultiplicity, default: Single.
        /// </summary>
        /// <param name="myMultiplicity"></param>
        /// <returns></returns>
        internal static PropertyMultiplicity ToPropertyMultiplicity(ServicePropertyMultiplicity myMultiplicity)
        {
            PropertyMultiplicity multiplicity;
            switch (myMultiplicity)
            {
                case ServicePropertyMultiplicity.Set:
                    multiplicity = PropertyMultiplicity.Set;
                    break;
                case ServicePropertyMultiplicity.List:
                    multiplicity = PropertyMultiplicity.List;
                    break;
                default:
                    multiplicity = PropertyMultiplicity.Single;
                    break;
            }
            return multiplicity;
        }
        #endregion
    }
}
