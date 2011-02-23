using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.PropertyHyperGraph;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The in memory representation of an ivertex
    /// </summary>
    public sealed class InMemoryVertex : IVertex
    {
        #region IVertex Members

        public bool HasIncomingHyperEdge(ulong myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<ulong, IHyperEdge>> GetAllIncomingHyperEdges(Func<ulong, IHyperEdge, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IHyperEdge GetIncomingHyperEdge(ulong myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdge(ulong myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<ulong, IEdge>> GetAllOutgoingEdges(Func<ulong, IEdge, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<ulong, IHyperEdge>> GetAllOutgoingHyperEdges(Func<ulong, IHyperEdge, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<ulong, ISingleEdge>> GetAllOutgoingSingleEdges(Func<ulong, ISingleEdge, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IEdge GetOutgoingEdge(ulong myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public IHyperEdge GetOutgoingHyperEdge(ulong myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public ISingleEdge GetOutgoingSingleEdge(ulong myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetBinaryProperty(ulong myPropertyID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<ulong, System.IO.Stream>> GetAllBinaryProperties(Func<ulong, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphElement Members

        public T GetProperty<T>(ulong myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(ulong myPropertyID)
        {
            throw new NotImplementedException();
        }

        public ulong GetCountOfProperties()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<ulong, object>> GetAllProperties(Func<ulong, object, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyAsString(ulong myPropertyID)
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

        public ulong GetCountOfUnstructuredProperties()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, object>> GetAllUnstructuredProperties(Func<string, object, bool> myFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public string Comment
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime CreationDate
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime ModificationDate
        {
            get { throw new NotImplementedException(); }
        }

        public ulong TypeID
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IVertexProperties Members

        public ulong VertexID
        {
            get { throw new NotImplementedException(); }
        }

        public VertexRevisionID VertexRevisionID
        {
            get { throw new NotImplementedException(); }
        }

        public string EditionName
        {
            get { throw new NotImplementedException(); }
        }

        public ulong InDegree
        {
            get { throw new NotImplementedException(); }
        }

        public ulong OutDegree
        {
            get { throw new NotImplementedException(); }
        }

        public ulong Degree
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

        #endregion
    }
}
