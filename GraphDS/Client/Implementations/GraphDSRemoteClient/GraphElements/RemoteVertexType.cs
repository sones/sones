using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace GraphDSRemoteClient.GraphElements
{
    public class RemoteVertexType : ARemoteBaseType, IVertexType
    {
        private VertexTypeService _VertexTypeService;

        internal RemoteVertexType(ServiceVertexType myVertexType, VertexTypeService myVertexTypeService) : base(myVertexType)
        {
            _VertexTypeService = myVertexTypeService;
        }

        public bool IsAbstract
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IVertexType> GetDescendantVertexTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertexType> GetDescendantVertexTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertexType> GetAncestorVertexTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertexType> GetAncestorVertexTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertexType> GetKinsmenVertexTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertexType> GetKinsmenVertexTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertexType> ChildrenVertexTypes
        {
            get { throw new NotImplementedException(); }
        }

        public IVertexType ParentVertexType
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasBinaryProperty(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public IBinaryPropertyDefinition GetBinaryPropertyDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasBinaryProperties(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBinaryPropertyDefinition> GetBinaryProperties(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasIncomingEdge(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public IIncomingEdgeDefinition GetIncomingEdgeDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasIncomingEdges(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdge(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public IOutgoingEdgeDefinition GetOutgoingEdgeDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdges(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IUniqueDefinition> GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIndexDefinition> GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IBaseType other)
        {
            throw new NotImplementedException();
        }
    }
}
