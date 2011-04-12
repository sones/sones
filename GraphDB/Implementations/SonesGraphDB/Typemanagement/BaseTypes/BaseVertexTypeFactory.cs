using System.Collections.Generic;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal static class BaseVertexTypeFactory
    {
        private static readonly Dictionary<BaseVertexType, IVertexType> _Instances = new Dictionary<BaseVertexType, IVertexType>()
        {
            {BaseVertexType.Attribute, new AttributeVertexType()},
            {BaseVertexType.BaseType, new BaseTypeVertexType()},
            {BaseVertexType.IncomingEdge, new IncomingEdgeVertexType()},
            {BaseVertexType.OutgoingEdge, new OutgoingEdgeVertexType()},
            {BaseVertexType.EdgeType, new EdgeTypeVertexType()},
            {BaseVertexType.Index, new IndexVertexType()},
            {BaseVertexType.Property, new PropertyVertexType()},
            {BaseVertexType.VertexType, new VertexTypeVertexType()}
        };
        
        public static IVertexType GetInstance(BaseVertexType myBaseVertexType)
        {
            IVertexType result;
            _Instances.TryGetValue(myBaseVertexType, out result);
            return result;
        }

    }
}
