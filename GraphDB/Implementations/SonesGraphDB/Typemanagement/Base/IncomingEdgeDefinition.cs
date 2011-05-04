using sones.GraphDB.TypeSystem;
using System.Collections.Generic;

namespace sones.GraphDB.TypeManagement
{
    internal class IncomingEdgeDefinition: IIncomingEdgeDefinition
    {
        #region IIncomingEdgeDefinition Members

        public IOutgoingEdgeDefinition RelatedEdgeDefinition { get; internal set; }

        #endregion

        #region IAttributeDefinition Members

        public string Name { get; internal set; }

        public long ID { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.IncomingEdge; } }


        public IBaseType RelatedType { get; internal set;}

        #endregion

        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition myOther)
        {
            return myOther != null && myOther.AttributeID == AttributeID && EqualityComparer<IBaseType>.Default.Equals(RelatedType, myOther.RelatedType);
        }

        #endregion


    }
}
