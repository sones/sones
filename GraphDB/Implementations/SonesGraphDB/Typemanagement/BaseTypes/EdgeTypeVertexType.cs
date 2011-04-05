using sones.GraphDB.TypeSystem;
using System.Collections.Generic;
using System.Linq;
namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class EdgeTypeVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
        };

        #endregion

        internal static readonly EdgeTypeVertexType Instance = new EdgeTypeVertexType();

        private EdgeTypeVertexType(): base(_Attributes) {}

        #region IVertexType

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

        bool IVertexType.HasParentVertexType
        {
            get { return true; }
        }

        IVertexType IVertexType.GetParentVertexType
        {
            get
            {
                return BaseTypeVertexType.Instance;
            }
        }

        bool IVertexType.HasChildVertexTypes
        {
            get { return false; }
        }

        IEnumerable<IVertexType> IVertexType.GetChildVertexTypes
        {
            get
            {
                return Enumerable.Empty<IVertexType>();
            }
        }

        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeParents)
        {
            return base.GetAttributeDefinitions();
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeParents)
        {
            return base.GetPropertyDefinitions();
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            return base.GetIncomingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasVisibleIncomingEdges(bool myIncludeParents)
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeParents)
        {
            throw new System.NotImplementedException();
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            throw new System.NotImplementedException();
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeParents)
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeParents)
        {
            throw new System.NotImplementedException();
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
