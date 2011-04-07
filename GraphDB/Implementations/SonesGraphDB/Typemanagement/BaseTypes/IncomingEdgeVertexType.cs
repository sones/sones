using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class IncomingEdgeVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IEnumerable<IVertexType> _Childs = new IVertexType[0];

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            AttributeDefinitions.RelatedEdgeOnIncomingEdge
        };

        private static readonly IVertexType _Parent = BaseVertexTypeFactory.GetInstance(BaseVertexType.Attribute);

        #endregion

        #region c'tor

        internal IncomingEdgeVertexType() : base(_Attributes, _Parent.GetAttributeDefinitions(true)) { }

        #endregion

        #region IVertexType Members

        long IVertexType.ID
        {
            get { return (long)BaseVertexType.IncomingEdge; }
        }

        string IVertexType.Name
        {
            get { return "IncomingEdge"; }
        }

        IBehaviour IVertexType.Behaviour
        {
            get { return null; }
        }

        string IVertexType.Comment
        {
            get { throw new NotImplementedException(); }
        }

        bool IVertexType.IsAbstract
        {
            get { return false; }
        }

        bool IVertexType.IsSealed
        {
            get { return true; }
        }

        bool IVertexType.HasParentVertexType
        {
            get { return true; }
        }

        IVertexType IVertexType.GetParentVertexType
        {
            get { return _Parent; }
        }

        bool IVertexType.HasChildVertexTypes
        {
            get { return false; }
        }

        IEnumerable<IVertexType> IVertexType.GetChildVertexTypes
        {
            get { return _Childs; }
        }

        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetAttributeDefinitions(myIncludeAncestorDefinitions);
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetPropertyDefinitions(myIncludeAncestorDefinitions);
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            return base.GetIncomingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasVisibleIncomingEdges(bool myIncludeAncestorDefinitions)
        {
            return base.HasIncomingDefinitions(myIncludeAncestorDefinitions);
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetIncomingEdgeDefinitions(myIncludeAncestorDefinitions);
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return base.GetOutgoingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeAncestorDefinitions)
        {
            return base.HasOutgoingDefinitions(myIncludeAncestorDefinitions);
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetOutgoingEdgeDefinitions(myIncludeAncestorDefinitions);
        }

        IEnumerable<IUniqueDefinition> IVertexType.GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IIndexDefinition> IVertexType.GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IAttributeDefinition IVertexType.GetAttributeDefinition(string myAttributeName)
        {
            return base.GetAttributeDefinition(myAttributeName);
        }

        IPropertyDefinition IVertexType.GetPropertyDefinition(string myPropertyName)
        {
            return base.GetPropertyDefinition(myPropertyName);
        }

        #endregion
    }
}
