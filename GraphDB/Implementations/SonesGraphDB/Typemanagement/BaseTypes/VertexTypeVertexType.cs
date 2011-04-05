using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class VertexTypeVertexType: TypeBase, IVertexType
    {        
        #region Data

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            //TODO
        };

        #endregion

        internal static readonly IVertexType Instance = new VertexTypeVertexType();

        private VertexTypeVertexType() : base(_Attributes) { }

        #region IVertexType Members

        string IVertexType.Name
        {
            get { throw new NotImplementedException(); }
        }

        IBehaviour IVertexType.Behaviour
        {
            get { throw new NotImplementedException(); }
        }

        string IVertexType.Comment
        {
            get { throw new NotImplementedException(); }
        }

        bool IVertexType.IsAbstract
        {
            get { throw new NotImplementedException(); }
        }

        bool IVertexType.IsSealed
        {
            get { throw new NotImplementedException(); }
        }

        bool IVertexType.HasParentVertexType
        {
            get { throw new NotImplementedException(); }
        }

        IVertexType IVertexType.GetParentVertexType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IVertexType.HasChildVertexTypes
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<IVertexType> IVertexType.GetChildVertexTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        bool IVertexType.HasVisibleIncomingEdges(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IUniqueDefinition> IVertexType.GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IIndexDefinition> IVertexType.GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
