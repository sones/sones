using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.Library.VertexStore.Definitions;
using System;
using System.Resources;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal static class BaseVertexTypeFactory
    {
        private const String Comment = ".Comment";
        private static readonly ResourceManager ResMgr = new ResourceManager("SonesGraphDB", System.Reflection.Assembly.GetExecutingAssembly());

        public static VertexAddDefinition GetInstance(BaseVertexType myBaseVertexType)
        {
            #region Data

            var result = new VertexAddDefinition();
            var props = new Dictionary<long, object>();
            var edges = new List<SingleEdgeAddDefinition>();

            var creationDate = DateTime.UtcNow.Ticks;
            var modificationDate = creationDate;
            var vertexID = (long)myBaseVertexType;
            var vertexTypeID = (long)BaseVertexType.VertexType;
            var comment = ResMgr.GetString(myBaseVertexType.ToString() + Comment);

            #endregion

            #region calculate depending properties

            var isAbstract = myBaseVertexType == BaseVertexType.Attribute;
            var isSealed = myBaseVertexType == BaseVertexType.EdgeType 
                || myBaseVertexType == BaseVertexType.IncomingEdge 
                || myBaseVertexType == BaseVertexType.Index 
                || myBaseVertexType == BaseVertexType.OutgoingEdge 
                || myBaseVertexType == BaseVertexType.Property 
                || myBaseVertexType == BaseVertexType.VertexType;

            BaseVertexType parent = BaseVertexType.Vertex;

            switch (myBaseVertexType)
            {
                case BaseVertexType.Attribute:
                    break;
                case BaseVertexType.BaseType:
                    break;
                case BaseVertexType.EdgeType:
                    parent = BaseVertexType.BaseType;
                    break;
                case BaseVertexType.IncomingEdge:
                    parent = BaseVertexType.Attribute;
                    break;
                case BaseVertexType.Index:
                    break;
                case BaseVertexType.OutgoingEdge:
                    parent = BaseVertexType.Attribute;
                    break;
                case BaseVertexType.Property:
                    parent = BaseVertexType.Attribute;
                    break;
                case BaseVertexType.Vertex:
                    break;
                case BaseVertexType.VertexType:
                    parent = BaseVertexType.BaseType;
                    break;
            }


            #endregion

            #region set vertex properties

            props.Add(AttributeDefinitions.Name.ID, myBaseVertexType.ToString());
            props.Add(AttributeDefinitions.ID.ID, (long)myBaseVertexType);
            props.Add(AttributeDefinitions.IsUserDefined.ID, false);
            props.Add(AttributeDefinitions.IsAbstractOnBaseType.ID, isAbstract);
            props.Add(AttributeDefinitions.IsSealedOnBaseType.ID, isSealed);
            props.Add(AttributeDefinitions.BehaviourOnBaseType.ID, String.Empty);
            
            #endregion



            

            throw new NotImplementedException();
        }

    }
}
