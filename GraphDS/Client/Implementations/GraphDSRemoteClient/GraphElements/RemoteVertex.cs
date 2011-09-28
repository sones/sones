using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using sones.Library.PropertyHyperGraph;

namespace GraphDSRemoteClient.GraphElements
{
    public class RemoteVertex : ARemoteGraphElement, IVertex
    {
        private VertexInstanceService _VertexInstanceService;

        internal RemoteVertex(ServiceVertexInstance myVertex)
        {

        }

        public bool HasIncomingVertices(long myVertexTypeID, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<long, long, IEnumerable<IVertex>>> GetAllIncomingVertices(PropertyHyperGraphFilter.IncomingVerticesFilter myFilter = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetIncomingVertices(long myVertexTypeID, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdge(long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<long, IEdge>> GetAllOutgoingEdges(PropertyHyperGraphFilter.OutgoingEdgeFilter myFilter = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<long, IHyperEdge>> GetAllOutgoingHyperEdges(PropertyHyperGraphFilter.OutgoingHyperEdgeFilter myFilter = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<long, ISingleEdge>> GetAllOutgoingSingleEdges(PropertyHyperGraphFilter.OutgoingSingleEdgeFilter myFilter = null)
        {
            throw new NotImplementedException();
        }

        public IEdge GetOutgoingEdge(long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public IHyperEdge GetOutgoingHyperEdge(long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public ISingleEdge GetOutgoingSingleEdge(long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetBinaryProperty(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<long, System.IO.Stream>> GetAllBinaryProperties(PropertyHyperGraphFilter.BinaryPropertyFilter myFilter = null)
        {
            throw new NotImplementedException();
        }

        public T GetProperty<T>(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public IComparable GetProperty(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public int GetCountOfProperties()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<long, IComparable>> GetAllProperties(PropertyHyperGraphFilter.GraphElementStructuredPropertyFilter myFilter = null)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyAsString(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public int GetCountOfUnstructuredProperties()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(PropertyHyperGraphFilter.GraphElementUnStructuredPropertyFilter myFilter = null)
        {
            throw new NotImplementedException();
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public long VertexTypeID
        {
            get { throw new NotImplementedException(); }
        }

        public long VertexID
        {
            get { throw new NotImplementedException(); }
        }

        public long VertexRevisionID
        {
            get { throw new NotImplementedException(); }
        }

        public string EditionName
        {
            get { throw new NotImplementedException(); }
        }

        public IVertexStatistics Statistics
        {
            get { throw new NotImplementedException(); }
        }

        public IGraphPartitionInformation PartitionInformation
        {
            get { throw new NotImplementedException(); }
        }
    }
}
