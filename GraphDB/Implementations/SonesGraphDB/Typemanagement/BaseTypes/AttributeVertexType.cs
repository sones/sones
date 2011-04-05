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

        private static readonly IVertexType[] _Childs = new IVertexType[] 
        { 
            EdgeVertexType.Instance,
            PropertyVertexType.Instance
        };

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            AttributeDefinitions.TypeOnAttribute,
            AttributeDefinitions.Name,
            AttributeDefinitions.ID,
            AttributeDefinitions.DefiningType
        };

        #endregion

        internal static readonly IVertexType Instance = new AttributeVertexType();

        private AttributeVertexType() : base(_Attributes) { }

        #region IVertexType Members

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
            get
            {
                return VertexVertexType.Instance;
            }
        }

        bool IVertexType.HasChildVertexTypes
        {
            get { return true; }
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

        bool IVertexType.HasVisibleIncomingEdges(bool myIncludeParents)
        {
            return false;
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeParents)
        {
            return base.GetIncomingEdgeDefinitions(myIncludeParents);
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return GetOutgoingEdgeDefinition(myEdgeName);
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeParents)
        {
            return true;
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeParents)
        {
            return GetOutgoingEdgeDefinitions(myIncludeParents);
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
