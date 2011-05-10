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

        public AttributeType Kind { get { return AttributeType.IncomingEdge; } }

        public bool IsUserDefined { get; internal set; }

        public IBaseType RelatedType { get; internal set;}

        #endregion

        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition myOther)
        {
            return myOther != null && myOther.ID == ID;
        }

        #endregion

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IAttributeDefinition);
        }


    }
}
