using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using sones.GraphDS.GraphDSRemoteClient.GraphElements;

namespace sones.GraphDS.GraphDSRemoteClient.TypeManagement
{
    internal class RemoteOutgoingEdgeDefinition : IOutgoingEdgeDefinition
    {
        #region Constructor

        internal RemoteOutgoingEdgeDefinition(ServiceOutgoingEdgeDefinition myOutgoingEdgeDefinition, IServiceToken myServiceToken)
        {
            this.EdgeType = new RemoteEdgeType(myOutgoingEdgeDefinition.EdgeType, myServiceToken);
            this.InnerEdgeType = (myOutgoingEdgeDefinition.InnerEdgeType == null) ?
                null : new RemoteEdgeType(myOutgoingEdgeDefinition.InnerEdgeType, myServiceToken);
            this.Multiplicity = ConvertHelper.ToEdgeMultiplicity(myOutgoingEdgeDefinition.Multiplicity);
            this.SourceVertexType = new RemoteVertexType(myOutgoingEdgeDefinition.SourceVertexType, myServiceToken);
            this.TargetVertexType = new RemoteVertexType(myOutgoingEdgeDefinition.TargetVertexType, myServiceToken);
            this.Name = myOutgoingEdgeDefinition.Name;
            this.ID = myOutgoingEdgeDefinition.ID;
            this.RelatedType = ConvertHelper.ToBaseType(myOutgoingEdgeDefinition.RelatedType, myServiceToken);
            this.IsUserDefined = myOutgoingEdgeDefinition.IsUserDefined;
        }

        #endregion


        #region IOutgoingEdgeDefinition

        public IEdgeType EdgeType { get; internal set; }

        public IEdgeType InnerEdgeType { get; internal set; }

        public EdgeMultiplicity Multiplicity { get; internal set; }

        public IVertexType SourceVertexType { get; internal set; }

        public IVertexType TargetVertexType { get; internal set; }

        #endregion

        #region IAttributeDefinition Members

        public string Name { get; internal set; }

        public long ID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.OutgoingEdge; } }

        public IBaseType RelatedType { get; internal set; }

        public bool IsUserDefined { get; internal set; }

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
