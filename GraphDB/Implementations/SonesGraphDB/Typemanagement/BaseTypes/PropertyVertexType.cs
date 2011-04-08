using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class PropertyVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IEnumerable<IVertexType> _Childs = new IVertexType[0];

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            AttributeDefinitions.IsMandatoryOnProperty,
            AttributeDefinitions.InIndicesOnProperty
        };

        private static readonly IVertexType _Parent = BaseVertexTypeFactory.GetInstance(BaseVertexType.Attribute);

        #endregion

        #region c'tor

        internal PropertyVertexType() : base(_Attributes, _Parent.GetAttributeDefinitions(true)) {}

        #endregion

        #region IVertexType Members

        #region Vertex type properties

        long IVertexType.ID
        {
            get { return (long)BaseVertexType.Property; }
        }

        string IVertexType.Name
        {
            get { return "Property"; }
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
            throw new NotImplementedException();
        }

        IAttributeDefinition IVertexType.GetAttributeDefinition(string myAttributeName)
        {
            return base.GetAttributeDefinition(myAttributeName);
        }

        bool IVertexType.HasAttributes(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetAttributeDefinitions(myIncludeAncestorDefinitions);
        }

        #endregion

        #region Properties

        bool IVertexType.HasProperty(string myAttributeName)
        {
            throw new NotImplementedException();
        }

        IPropertyDefinition IVertexType.GetPropertyDefinition(string myPropertyName)
        {
            return base.GetPropertyDefinition(myPropertyName);
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetPropertyDefinitions(myIncludeAncestorDefinitions);
        }

        bool IVertexType.HasProperties(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Incoming Edges

        bool IVertexType.HasIncomingEdge(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            return base.GetIncomingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasIncomingEdges(bool myIncludeAncestorDefinitions)
        {
            return base.HasIncomingDefinitions(myIncludeAncestorDefinitions);
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return base.GetIncomingEdgeDefinitions(myIncludeAncestorDefinitions);
        }

        #endregion

        #region Outgoing Edges

        bool IVertexType.HasOutgoingEdge(string myEdgeName)
        {
            throw new NotImplementedException();
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

        #endregion

        #region Unique

        IEnumerable<IUniqueDefinition> IVertexType.GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Indices

        IEnumerable<IIndexDefinition> IVertexType.GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
