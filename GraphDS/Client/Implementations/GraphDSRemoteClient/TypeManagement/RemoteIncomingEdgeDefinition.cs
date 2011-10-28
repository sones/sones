using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace sones.GraphDS.GraphDSRemoteClient.TypeManagement
{
    internal class RemoteIncomingEdgeDefinition : IIncomingEdgeDefinition
    {
        #region Constructor

        internal RemoteIncomingEdgeDefinition(ServiceIncomingEdgeDefinition myIncomingEdgeDefinition, IServiceToken myServiceToken)
        {
            this.RelatedEdgeDefinition = new RemoteOutgoingEdgeDefinition(myIncomingEdgeDefinition.RelatedEdgeDefinition, myServiceToken);
            this.Name = myIncomingEdgeDefinition.Name;
            this.ID = myIncomingEdgeDefinition.ID;
            this.IsUserDefined = myIncomingEdgeDefinition.IsUserDefined;
            this.RelatedType = ConvertHelper.ToBaseType(myIncomingEdgeDefinition.RelatedType, myServiceToken);
        }

        #endregion


        #region IIncomingEdgeDefinition Members

        public IOutgoingEdgeDefinition RelatedEdgeDefinition { get; internal set; }

        #endregion


        #region IAttributeDefinition Members

        public string Name { get; internal set; }

        public long ID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.IncomingEdge; } }

        public bool IsUserDefined { get; internal set; }

        public IBaseType RelatedType { get; internal set; }

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
