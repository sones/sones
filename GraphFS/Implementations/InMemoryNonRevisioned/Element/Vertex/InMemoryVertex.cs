using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS.Element.Edge;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using sones.GraphFS.Definitions;
using System.Collections.Concurrent;

namespace sones.GraphFS.Element.Vertex
{
    /// <summary>
    /// The in memory representation of an ivertex
    /// </summary>
    public sealed class InMemoryVertex : IVertex
    {
        #region data

        public readonly Boolean IsBulkVertex = true;

        /// <summary>
        /// The binary properties of this vertex
        /// </summary>
        private readonly Dictionary<Int64, Stream> _binaryProperties;

        /// <summary>
        /// The edition of the vertex
        /// </summary>
        private readonly string _edition;

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        private readonly GraphElementInformation _graphElementInformation;

        /// <summary>
        /// The incoming edges of the vertex
        /// (VertexTypeID of the vertex type that points to this vertex, PropertyID of the edge that points to this vertex, SingleEdges)
        /// </summary>
        public Dictionary<IncomingEdgeKey, HashSet<SingleEdge>> IncomingEdges;

        /// <summary>
        /// The outgoing edges of the vertex
        /// </summary>
        private readonly Dictionary<Int64, IEdge> _outgoingEdges;

        /// <summary>
        /// The id of the vertex
        /// </summary>
        private readonly Int64 _vertexID;

        /// <summary>
        /// The revision id of the vertex
        /// </summary>
        private readonly VertexRevisionID _vertexRevisionID;
       
        #endregion

        #region constructor

        /// <summary>
        /// Creates a new in memory vertex
        /// </summary>
        /// <param name="myVertexID">The id of this vertex</param>
        /// <param name="myVertexRevisionID">The revision id of this vertex</param>
        /// <param name="myEdition">The edition of this vertex</param>
        /// <param name="myBinaryProperties">The binary properties of this vertex</param>
        /// <param name="myOutgoingEdges">The outgoing edges of this vertex</param>
        /// <param name="myGraphElementInformation">The graph element information of this vertex</param>
        public InMemoryVertex(
            Int64 myVertexID,
            VertexRevisionID myVertexRevisionID,
            String myEdition,
            Dictionary<long, Stream> myBinaryProperties,
            Dictionary<long, IEdge> myOutgoingEdges,
            GraphElementInformation myGraphElementInformation)
        {
            _vertexID = myVertexID;
            _vertexRevisionID = myVertexRevisionID;
            _edition = myEdition;
            _binaryProperties = myBinaryProperties;
            _outgoingEdges = myOutgoingEdges;
            _graphElementInformation = myGraphElementInformation;
            IncomingEdges = new Dictionary<IncomingEdgeKey, HashSet<SingleEdge>>();

            IsBulkVertex = false;
        }

        /// <summary>
        /// Creates a new bulk vertex
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The vertex type id</param>
        private InMemoryVertex(
            Int64 myVertexID,
            Int64 myVertexTypeID)
        {
            _vertexID = myVertexID;
            _graphElementInformation = new GraphElementInformation() { TypeID = myVertexTypeID };
            IsBulkVertex = true;
            IncomingEdges = new Dictionary<IncomingEdgeKey, HashSet<SingleEdge>>();
        }

        #endregion

        #region IVertex Members

        public bool HasIncomingEdge(long myVertexTypeID, long myEdgePropertyID)
        {
            return IncomingEdges != null &&
                   IncomingEdges.ContainsKey(new IncomingEdgeKey(myVertexTypeID, myEdgePropertyID));
        }

        public IEnumerable<Tuple<long, long, IEnumerable<ISingleEdge>>> GetAllIncomingEdges(
            Func<long, long, IEnumerable<ISingleEdge>, bool> myFilterFunc = null)
        {
            if (IncomingEdges != null)
            {
                foreach (var aIncomingEdge in IncomingEdges)
                {
                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aIncomingEdge.Key.VertexTypeID, aIncomingEdge.Key.EdgePropertyID, aIncomingEdge.Value))
                        {
                            yield return new Tuple<long, long, IEnumerable<ISingleEdge>>(aIncomingEdge.Key.VertexTypeID, aIncomingEdge.Key.EdgePropertyID, aIncomingEdge.Value);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, long, IEnumerable<ISingleEdge>>(aIncomingEdge.Key.VertexTypeID, aIncomingEdge.Key.EdgePropertyID, aIncomingEdge.Value);
                    }
                }

            }

            yield break;
        }

        public IEnumerable<ISingleEdge> GetIncomingHyperEdge(long myVertexTypeID, long myEdgePropertyID)
        {
            return HasIncomingEdge(myVertexTypeID, myEdgePropertyID)
                       ? IncomingEdges[new IncomingEdgeKey(myVertexTypeID, myEdgePropertyID)]
                       : null;
        }

        public bool HasOutgoingEdge(long myEdgePropertyID)
        {
            return _outgoingEdges != null &&
                   _outgoingEdges.ContainsKey(myEdgePropertyID);
        }

        public IEnumerable<Tuple<long, IEdge>> GetAllOutgoingEdges(Func<long, IEdge, bool> myFilterFunc = null)
        {
            if (_outgoingEdges != null)
            {
                foreach (var aEdge in _outgoingEdges)
                {
                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aEdge.Key, aEdge.Value))
                        {
                            yield return new Tuple<long, IEdge>(aEdge.Key, aEdge.Value);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, IEdge>(aEdge.Key, aEdge.Value);
                    }
                }
            }

            yield break;
        }

        public IEnumerable<Tuple<long, IHyperEdge>> GetAllOutgoingHyperEdges(
            Func<long, IHyperEdge, bool> myFilterFunc = null)
        {
            return GetAllOutgoingEdgesPrivate(myFilterFunc);
        }

        public IEnumerable<Tuple<long, ISingleEdge>> GetAllOutgoingSingleEdges(
            Func<long, ISingleEdge, bool> myFilterFunc = null)
        {
            return GetAllOutgoingEdgesPrivate(myFilterFunc);
        }

        public IEdge GetOutgoingEdge(long myEdgePropertyID)
        {
            return HasOutgoingEdge(myEdgePropertyID) ? _outgoingEdges[myEdgePropertyID] : null;
        }

        public IHyperEdge GetOutgoingHyperEdge(long myEdgePropertyID)
        {
            var edge = GetOutgoingEdge(myEdgePropertyID);

            return edge as IHyperEdge;
        }

        public ISingleEdge GetOutgoingSingleEdge(long myEdgePropertyID)
        {
            var edge = GetOutgoingEdge(myEdgePropertyID);

            return edge as ISingleEdge;
        }

        public Stream GetBinaryProperty(long myPropertyID)
        {
            return _binaryProperties != null && _binaryProperties.ContainsKey(myPropertyID) ? _binaryProperties[myPropertyID] : null;
        }

        public IEnumerable<Tuple<long, Stream>> GetAllBinaryProperties(Func<long, Stream, bool> myFilterFunc = null)
        {
            if (_binaryProperties != null)
            {
                foreach (var aBinary in _binaryProperties)
                {
                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aBinary.Key, aBinary.Value))
                        {
                            yield return new Tuple<long, Stream>(aBinary.Key, aBinary.Value);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, Stream>(aBinary.Key, aBinary.Value);
                    }
                }
            }

            yield break;
        }

        public T GetProperty<T>(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return (T) _graphElementInformation.StructuredProperties[myPropertyID];
            }
            
            throw new CouldNotFindStructuredVertexPropertyException(_graphElementInformation.TypeID,
                                                                    _vertexID, myPropertyID);
        }

        public bool HasProperty(long myPropertyID)
        {
            return _graphElementInformation.StructuredProperties != null &&
                   _graphElementInformation.StructuredProperties.ContainsKey(myPropertyID);
        }

        public int GetCountOfProperties()
        {
            return _graphElementInformation.StructuredProperties == null ? 0 : _graphElementInformation.StructuredProperties.Count;
        }

        public IEnumerable<Tuple<long, object>> GetAllProperties(Func<long, object, bool> myFilterFunc = null)
        {
            return _graphElementInformation.GetAllPropertiesProtected(myFilterFunc);
        }

        public string GetPropertyAsString(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _graphElementInformation.StructuredProperties[myPropertyID].ToString();
            }
            
            throw new CouldNotFindStructuredVertexPropertyException(_graphElementInformation.TypeID,
                                                                    _vertexID, myPropertyID);
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T) _graphElementInformation.UnstructuredProperties[myPropertyName];
            }
            
            throw new CouldNotFindUnStructuredVertexPropertyException(_graphElementInformation.TypeID,
                                                                      _vertexID, myPropertyName);
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _graphElementInformation.UnstructuredProperties != null &&
                   _graphElementInformation.UnstructuredProperties.ContainsKey(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties()
        {
            return _graphElementInformation.UnstructuredProperties == null ? 0 : _graphElementInformation.UnstructuredProperties.Count;
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(
            Func<string, object, bool> myFilterFunc = null)
        {
            return _graphElementInformation.GetAllUnstructuredPropertiesProtected(myFilterFunc);
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return _graphElementInformation.UnstructuredProperties[myPropertyName].ToString();
            }
            
            throw new CouldNotFindUnStructuredVertexPropertyException(_graphElementInformation.TypeID,
                                                                      _vertexID, myPropertyName);
        }

        public string Comment
        {
            get { return _graphElementInformation.Comment; }
        }

        public DateTime CreationDate
        {
            get { return _graphElementInformation.CreationDate; }
        }

        public DateTime ModificationDate
        {
            get { return _graphElementInformation.ModificationDate; }
        }

        public long TypeID
        {
            get { return _graphElementInformation.TypeID; }
        }

        public long VertexID
        {
            get { return _vertexID; }
        }

        public VertexRevisionID VertexRevisionID
        {
            get { return _vertexRevisionID; }
        }

        public string EditionName
        {
            get { return _edition; }
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

        #region private helper

        #region GetAllOutgoingEdges_private

        /// <summary>
        /// Returns all outgoing edges corresponding to their type and an optional filter function
        /// </summary>
        /// <typeparam name="T">The type of the result</typeparam>
        /// <param name="myFilterFunc">The optional filter function</param>
        /// <returns>All matching outgoing edges</returns>
        private IEnumerable<Tuple<long, T>> GetAllOutgoingEdgesPrivate<T>(Func<long, T, bool> myFilterFunc)
            where T : class
        {
            if (_outgoingEdges != null)
            {
                foreach (var aEdge in _outgoingEdges)
                {
                    var interestingEdge = aEdge.Value as T;

                    if (interestingEdge == null) continue;

                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aEdge.Key, interestingEdge))
                        {
                            yield return new Tuple<long, T>(aEdge.Key, interestingEdge);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, T>(aEdge.Key, interestingEdge);
                    }
                }
            }

            yield break;
        }

        #endregion

        #endregion

        #region static methods

        /// <summary>
        /// Creates a new InMemoryVertex from an IVertex
        /// </summary>
        /// <param name="aVertex">The vertex template</param>
        /// <returns>A new InMemoryVertex</returns>
        internal static InMemoryVertex CopyFromIVertex(IVertex aVertex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new bulk vertex
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The vertex type id</param>
        /// <returns>A new bulk InMemoryVertex</returns>
        internal static InMemoryVertex CreateNewBulkVertex(
            Int64 myVertexID,
            Int64 myVertexTypeID)
        {
            return new InMemoryVertex(myVertexID, myVertexTypeID);
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            var p = obj as InMemoryVertex;

            return p != null && Equals(p);
        }

        public Boolean Equals(InMemoryVertex p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return _vertexID == p._vertexID
                   && (_graphElementInformation.TypeID == p._graphElementInformation.TypeID);
        }

        public static Boolean operator ==(InMemoryVertex a, InMemoryVertex b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(InMemoryVertex a, InMemoryVertex b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return _vertexID.GetHashCode() ^ _graphElementInformation.TypeID.GetHashCode();
        }

        #endregion
    }
}