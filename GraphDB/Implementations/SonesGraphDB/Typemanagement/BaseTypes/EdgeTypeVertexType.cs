using System.Collections.Generic;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class EdgeTypeVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IEnumerable<IVertexType> _Childs = new IVertexType[0];

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            AttributeDefinitions.ParentOnEdgeType,
            AttributeDefinitions.ChildrenOnEdgeType
        };

        private static readonly IVertexType _Parent = BaseVertexTypeFactory.GetInstance(BaseVertexType.BaseType);

        #endregion

        #region c'tor

        internal EdgeTypeVertexType(): base(_Attributes, _Parent.GetAttributeDefinitions(true)) {}
        
        #endregion

        #region IVertexType

        #region Vertex type properties

        long IVertexType.ID
        {
            get { return (long)BaseVertexType.EdgeType; }
        }

        string IVertexType.Name
        {
            get { return "EdgeType"; }
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
            get { return true; }
        }

        #endregion

        #region Inheritance

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

        #endregion

        #region Attributes

        bool IVertexType.HasAttribute(string myAttributeName)
        {
            throw new System.NotImplementedException();
        }

        IAttributeDefinition IVertexType.GetAttributeDefinition(string myAttributeName)
        {
            return base.GetAttributeDefinition(myAttributeName);
        }

        bool IVertexType.HasAttributes(bool myIncludeAncestorDefinitions)
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeParents)
        {
            return base.GetAttributeDefinitions(myIncludeParents);
        }

        #endregion

        #region Properties

        bool IVertexType.HasProperty(string myAttributeName)
        {
            throw new System.NotImplementedException();
        }

        IPropertyDefinition IVertexType.GetPropertyDefinition(string myPropertyName)
        {
            return base.GetPropertyDefinition(myPropertyName);
        }

        bool IVertexType.HasProperties(bool myIncludeAncestorDefinitions)
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeParents)
        {
            return base.GetPropertyDefinitions(myIncludeParents);
        }

        #endregion

        #region Incoming Edges

        bool IVertexType.HasIncomingEdge(string myEdgeName)
        {
            throw new System.NotImplementedException();
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            return base.GetIncomingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasIncomingEdges(bool myIncludeParents)
        {
            return base.HasIncomingDefinitions(myIncludeParents);
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeParents)
        {
            return base.GetIncomingEdgeDefinitions(myIncludeParents);
        }

        #endregion

        #region Outgoing Edges

        bool IVertexType.HasOutgoingEdge(string myEdgeName)
        {
            throw new System.NotImplementedException();
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return base.GetOutgoingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeParents)
        {
            return base.HasOutgoingDefinitions(myIncludeParents);
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeParents)
        {
            return base.GetOutgoingEdgeDefinitions(myIncludeParents);
        }

        #endregion

        #region Unique

        IEnumerable<IUniqueDefinition> IVertexType.GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Indices

        IEnumerable<IIndexDefinition> IVertexType.GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
           throw new System.NotImplementedException();
        }

        #endregion

        #endregion
    }
}
