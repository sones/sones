using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    /// <summary>
    /// This class contains a singleton representation of the system defined vertex type Index
    /// </summary>
    internal sealed class IndexVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IEnumerable<IVertexType> _Childs = new IVertexType[0];

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            AttributeDefinitions.Name,
            AttributeDefinitions.ID,
            AttributeDefinitions.IsUserDefined,
            AttributeDefinitions.Comment,
            AttributeDefinitions.IndexedPropertiesOnIndex,
            AttributeDefinitions.DefiningVertexTypeOnIndex,
            AttributeDefinitions.TypeOnIndex,
            AttributeDefinitions.IsSingleOnIndex,
            AttributeDefinitions.IsRangeOnIndex,
            AttributeDefinitions.IsVersionedOnIndex
        };

        private static readonly IVertexType _Parent = BaseVertexTypeFactory.GetInstance(BaseVertexType.Vertex);

        #endregion

        #region c'tor

        internal IndexVertexType() : base(_Attributes, _Parent.GetAttributeDefinitions(true)) { }

        #endregion

        #region IVertexType Members

        string IVertexType.Name
        {
            get { return "Index"; }
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
            return base.GetAttributeDefinitions();
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetPropertyDefinitions();
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            return base.GetIncomingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasVisibleIncomingEdges(bool myIncludeAncestorDefinitions)
        {
            return base.HasIncomingDefinitions();
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetIncomingEdgeDefinitions();
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return base.GetOutgoingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeAncestorDefinitions)
        {
            return base.HasOutgoingDefinitions();
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetOutgoingEdgeDefinitions();
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
