using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class EdgeVertexType: TypeBase, IVertexType
    {
        #region Data

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            //TODO
        };

        #endregion

        internal static readonly EdgeVertexType Instance = new EdgeVertexType();

        private EdgeVertexType() : base(_Attributes) { }

        #region IVertexType Members

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public IBehaviour Behaviour
        {
            get { throw new NotImplementedException(); }
        }

        public string Comment
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAbstract
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSealed
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasParentVertexType
        {
            get { throw new NotImplementedException(); }
        }

        public IVertexType GetParentVertexType
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasChildVertexTypes
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IVertexType> GetChildVertexTypes
        {
            get { throw new NotImplementedException(); }
        }

        public new IEnumerable<IAttributeDefinition> GetAttributeDefinitions(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        public new IEnumerable<IPropertyDefinition> GetPropertyDefinitions(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        public new IIncomingEdgeDefinition GetIncomingEdgeDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasVisibleIncomingEdges(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        public new IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgeDefinitions(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        public new IOutgoingEdgeDefinition GetOutgoingEdgeDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdges(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        public new IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(bool myIncludeParents)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IUniqueDefinition> GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIndexDefinition> GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
           throw new NotImplementedException();
        }

        #endregion
    }
}
