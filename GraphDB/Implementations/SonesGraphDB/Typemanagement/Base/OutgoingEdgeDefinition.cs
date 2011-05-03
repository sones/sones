using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement
{
    internal class OutgoingEdgeDefinition: IOutgoingEdgeDefinition
    {
        #region IOutgoingEdgeDefinition

        public IEdgeType EdgeType { get; internal set; }

        public IEdgeType InnerEdgeType { get; internal set; }

        public EdgeMultiplicity Multiplicity { get; internal set; }

        public IVertexType SourceVertexType { get; internal set; }

        public IVertexType TargetVertexType { get; internal set; }

        #endregion

        #region IAttributeDefinition Members

        public string Name { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.OutgoingEdge; } }

        public IBaseType RelatedType { get; internal set; }

        #endregion
    }
}
