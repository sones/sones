using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class AttributeVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IEnumerable<IVertexType> _Childs = new IVertexType[] 
        { 
            BaseVertexTypeFactory.GetInstance(BaseVertexType.IncomingEdge),
            BaseVertexTypeFactory.GetInstance(BaseVertexType.OutgoingEdge),
            BaseVertexTypeFactory.GetInstance(BaseVertexType.Property)
        };

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            AttributeDefinitions.Name,
            AttributeDefinitions.ID,
            AttributeDefinitions.IsUserDefined,
            AttributeDefinitions.Comment,
            AttributeDefinitions.TypeOnAttribute,
            AttributeDefinitions.DefiningTypeOnAttribute
        };

        private static readonly IVertexType _Parent = BaseVertexTypeFactory.GetInstance(BaseVertexType.VertexType);

        #endregion

        #region c'tor

        internal AttributeVertexType() : base(_Attributes, _Parent.GetAttributeDefinitions(true)) { }

        #endregion

        #region IVertexType Members

        long IVertexType.ID
        {
            get { return (long)BaseVertexType.Attribute; }
        }

        string IVertexType.Name
        {
            get { return "Attribute"; }
        }

        IBehaviour IVertexType.Behaviour
        {
            get { return null; }
        }

        string IVertexType.Comment
        {//TODO add the comment as a resource (language dependent)
            get { throw new NotImplementedException(); }
        }

        bool IVertexType.IsAbstract
        {
            get { return true; }
        }

        bool IVertexType.IsSealed
        {
            get { return false; }
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
            get { return true; }
        }

        IEnumerable<IVertexType> IVertexType.GetChildVertexTypes
        {
            get { return _Childs; }
        }


        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeParents)
        {
            return base.GetAttributeDefinitions(myIncludeParents);
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeParents)
        {
            return base.GetPropertyDefinitions(myIncludeParents);
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeParents)
        {
            return base.GetIncomingEdgeDefinitions(myIncludeParents);
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeParents)
        {
            return base.GetOutgoingEdgeDefinitions(myIncludeParents);
        }

        bool IVertexType.HasVisibleIncomingEdges(bool myIncludeParents)
        {
            return base.HasIncomingDefinitions(myIncludeParents);
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeParents)
        {
            return base.HasOutgoingDefinitions(myIncludeParents);
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return base.GetOutgoingEdgeDefinition(myEdgeName);
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            return base.GetIncomingEdgeDefinition(myEdgeName);
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
