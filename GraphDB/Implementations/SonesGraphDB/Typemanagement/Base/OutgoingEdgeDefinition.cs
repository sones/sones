using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement
{
    internal class OutgoingEdgeDefinition: IOutgoingEdgeDefinition
    {
        public IEdgeType EdgeType { get; internal set; }

        public IVertexType SourceVertexType { get; internal set; }

        public IVertexType TargetVertexType { get; internal set; }

        public string Name { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.OutgoingEdge; } }

        public EdgeMultiplicity Multiplicity { get; internal set; }
    }
}
