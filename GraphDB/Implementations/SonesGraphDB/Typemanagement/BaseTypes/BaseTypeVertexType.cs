using sones.GraphDB.TypeSystem;
using System.Collections.Generic;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class BaseTypeVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IVertexType[] _Childs = new IVertexType[] 
        { 
            VertexTypeVertexType.Instance, 
            EdgeTypeVertexType.Instance 
        };

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            AttributeDefinitions.Name,
            AttributeDefinitions.ID, 
            AttributeDefinitions.Comment, 
            AttributeDefinitions.IsAbstract, 
            AttributeDefinitions.IsSealed, 
            AttributeDefinitions.Attributes 
        };

        #endregion

        internal static readonly IVertexType Instance = new BaseTypeVertexType();

        private BaseTypeVertexType() : base(_Attributes) {}

        #region IVertexType Members

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
            return true;
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeParents)
        {
            return false;
        }

        IVertexType IVertexType.GetParentVertexType
        {
            get
            {
                return VertexVertexType.Instance;
            }
        }

        IEnumerable<IVertexType> IVertexType.GetChildVertexTypes
        {
            get
            {
                return _Childs;
            }
        }

        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeParents)
        {
            return base.GetAttributeDefinitions(myIncludeParents);
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeParents)
        {
            return base.GetPropertyDefinitions(myIncludeParents);
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            return base.GetIncomingEdgeDefinition(myEdgeName);
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeParents)
        {
            return base.GetIncomingEdgeDefinitions(myIncludeParents);
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return base.GetOutgoingEdgeDefinition(myEdgeName);
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeParents)
        {
            return base.GetOutgoingEdgeDefinitions(myIncludeParents);
        }

        IEnumerable<IUniqueDefinition> IVertexType.GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<IIndexDefinition> IVertexType.GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}
