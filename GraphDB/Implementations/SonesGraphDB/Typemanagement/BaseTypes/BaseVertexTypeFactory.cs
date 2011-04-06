using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal static class BaseVertexTypeFactory
    {
        private static Dictionary<BaseVertexType, IVertexType> _Instances = new Dictionary<BaseVertexType, IVertexType>()
        {
            {BaseVertexType.Attribute, new AttributeVertexType()},
            {BaseVertexType.BaseType, new BaseTypeVertexType()},
            {BaseVertexType.Edge, new EdgeTypeVertexType()},
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
