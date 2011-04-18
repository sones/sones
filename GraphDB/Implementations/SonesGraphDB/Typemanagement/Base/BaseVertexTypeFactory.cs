using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.Library.VertexStore.Definitions;
using System;
using System.Resources;

namespace sones.GraphDB.TypeManagement.Base
{
    internal static class BaseVertexTypeFactory
    {
        private const String Comment = ".Comment";
        private static readonly ResourceManager ResMgr = new ResourceManager("SonesGraphDB", System.Reflection.Assembly.GetExecutingAssembly());

        public static VertexAddDefinition GetInstance(BaseTypes myBaseVertexType)
        {
            #region Data

            var result = new VertexAddDefinition();
            var props = new Dictionary<long, IComparable>();
            var edges = new List<SingleEdgeAddDefinition>();

            var creationDate = DateTime.UtcNow.Ticks;
            var modificationDate = creationDate;
            var vertexID = (long)myBaseVertexType;
            var vertexTypeID = (long)BaseTypes.VertexType;
            var comment = ResMgr.GetString(myBaseVertexType.ToString() + Comment);

            #endregion

            #region calculate depending properties

            var isAbstract = myBaseVertexType == BaseTypes.Attribute;
            var isSealed = myBaseVertexType == BaseTypes.EdgeType 
                || myBaseVertexType == BaseTypes.IncomingEdge 
                || myBaseVertexType == BaseTypes.Index 
                || myBaseVertexType == BaseTypes.OutgoingEdge 
                || myBaseVertexType == BaseTypes.Property 
                || myBaseVertexType == BaseTypes.VertexType;

            BaseTypes? parent = BaseTypes.Vertex;

            switch (myBaseVertexType)
            {
                case BaseTypes.Attribute:
                    break;
                case BaseTypes.BaseType:
                    break;
                case BaseTypes.EdgeType:
                    parent = BaseTypes.BaseType;
                    break;
                case BaseTypes.IncomingEdge:
                    parent = BaseTypes.Attribute;
                    break;
                case BaseTypes.Index:
                    break;
                case BaseTypes.OutgoingEdge:
                    parent = BaseTypes.Attribute;
                    break;
                case BaseTypes.Property:
                    parent = BaseTypes.Attribute;
                    break;
                case BaseTypes.Vertex:
                    parent = null;
                    break;
                case BaseTypes.VertexType:
                    parent = BaseTypes.BaseType;
                    break;
                case BaseTypes.Edge:
                    parent = null;
                    break;
            }

            if (parent != null)
            {
                edges.Add(
                    new SingleEdgeAddDefinition(
                        AttributeDefinitions.ParentOnVertexType.ID,
                        (long)BaseTypes.Edge,
                        new VertexInformation(vertexID, vertexTypeID),
                        new VertexInformation((long)parent, vertexTypeID),
                        null, creationDate, modificationDate, null, null));
            }
            

            #endregion

            #region set vertex properties

            props.Add(AttributeDefinitions.Name.ID, myBaseVertexType.ToString());
            props.Add(AttributeDefinitions.ID.ID, (long)myBaseVertexType);
            props.Add(AttributeDefinitions.IsUserDefined.ID, false);
            props.Add(AttributeDefinitions.IsAbstractOnBaseType.ID, isAbstract);
            props.Add(AttributeDefinitions.IsSealedOnBaseType.ID, isSealed);
            
            #endregion

            return new VertexAddDefinition(vertexID, vertexTypeID, null, null, edges, null, comment, creationDate, modificationDate, props, null);
        }

    }
}
