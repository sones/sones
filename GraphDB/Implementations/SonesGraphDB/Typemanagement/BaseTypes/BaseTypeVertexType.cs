using sones.GraphDB.TypeSystem;
using System.Collections.Generic;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class BaseTypeVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IEnumerable<IVertexType> _Childs = new IVertexType[] 
        { 
            BaseVertexTypeFactory.GetInstance(BaseVertexType.VertexType),
            BaseVertexTypeFactory.GetInstance(BaseVertexType.EdgeType)
        };

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            AttributeDefinitions.ID, 
            AttributeDefinitions.Name,
            AttributeDefinitions.Comment, 
            AttributeDefinitions.IsAbstractOnBaseType, 
            AttributeDefinitions.IsSealedOnBaseType, 
            AttributeDefinitions.AttributesOnBaseType
        };

        private static readonly IVertexType _Parent = BaseVertexTypeFactory.GetInstance(BaseVertexType.Vertex);

        #endregion

        #region c'tor

        internal BaseTypeVertexType() : base(_Attributes, _Parent.GetAttributeDefinitions(true)) {}
        
        #endregion

        #region IVertexType Members

        long IVertexType.ID
        {
            get { return (long)BaseVertexType.BaseType; }
        }

        string IVertexType.Name
        {
            get { return "BaseType"; }
        }

        IBehaviour IVertexType.Behaviour
        {
            get { return null; }
        }

        string IVertexType.Comment
        {//TODO add the comment as a resource (language dependent)
            get { throw new System.NotImplementedException(); }
        }

        bool IVertexType.IsAbstract
        {
            get { return false; }
        }

        bool IVertexType.IsSealed
        {
            get { return false; }
        }

        bool IVertexType.HasParentVertexType
        {
            get { return true; }
        }

        bool IVertexType.HasChildVertexTypes
        {
            get { return true; }
        }

        bool IVertexType.HasVisibleIncomingEdges(bool myIncludeParents)
        {
            return base.HasIncomingDefinitions(myIncludeParents);
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeParents)
        {
            return base.HasOutgoingDefinitions(myIncludeParents);
        }

        IVertexType IVertexType.GetParentVertexType
        {
            get { return _Parent; }
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
            throw new System.NotImplementedException();
        }

        IEnumerable<IIndexDefinition> IVertexType.GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new System.NotImplementedException();
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
