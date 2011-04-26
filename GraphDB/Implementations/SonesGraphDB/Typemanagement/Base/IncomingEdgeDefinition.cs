using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement
{
    internal class IncomingEdgeDefinition: IIncomingEdgeDefinition
    {
        #region IIncomingEdgeDefinition Members

        public IOutgoingEdgeDefinition RelatedEdgeDefinition { get; internal set; }

        #endregion

        #region IAttributeDefinition Members

        public string Name { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.IncomingEdge; } }


        public IBaseType RelatedType { get; internal set;}

        #endregion
    }
}
