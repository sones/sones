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
        #region data

        private readonly UInt64 _vertexID;

        private readonly UInt64 _typeID;

        private readonly String _comment;

        private readonly DateTime _creationDate;
        
        private readonly DateTime _modificationDate;

        private readonly VertexRevisionID _vertexRevisionID;

        private readonly String _editionName;

        private UInt64 _inDegree;
        
        private UInt64 _outDegree;

        #endregion

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
            get { return _comment; }
        }

        public DateTime CreationDate
        {
            get { return _creationDate; }
        }

        public DateTime ModificationDate
        {
            get { return _modificationDate; }
        }

        public ulong TypeID
        {
            get { return _typeID; }
        }

        #endregion

        #region IVertexProperties Members

        public ulong VertexID
        {
            get { return _vertexID; }
        }

        public VertexRevisionID VertexRevisionID
        {
            get { return _vertexRevisionID; }
        }

        public string EditionName
        {
            get { return _editionName; }
        }

        public ulong InDegree
        {
            get { return _inDegree; }
        }

        public ulong OutDegree
        {
            get { return _outDegree; }
        }

        public ulong Degree
        {
            get { return InDegree + OutDegree; }
        }

        public IVertexStatistics Statistics
        {
            get { return null; }
        }

        public IGraphPartitionInformation PartitionInformation
        {
            get { return null; }
        }

        #endregion
    }
}
