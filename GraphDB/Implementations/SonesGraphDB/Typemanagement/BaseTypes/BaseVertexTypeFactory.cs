using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.Library.VertexStore.Definitions;
using System;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal static class BaseVertexTypeFactory
    {
        public static VertexAddDefinition GetInstance(BaseVertexType myBaseVertexType)
        {
            //var result = new VertexAddDefinition();
            //result.CreationDate = DateTime.UtcNow.Ticks;
            //result.ModificationDate = result.CreationDate;
            //result.VertexID = (long)myBaseVertexType;
            //result.VertexTypeID = (long)BaseVertexType.VertexType;
            
            //result.StructuredProperties = new Dictionary<long, object>();
            //result.StructuredProperties.Add(AttributeDefinitions.CreationOnVertex.AttributeID, )



            //result.OutgoingSingleEdges = new List<SingleEdgeAddDefinition>();

            throw new NotImplementedException();
        }

    }
}
