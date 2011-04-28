using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS.Element.Edge;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using System.Collections;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.VertexStore.Definitions.Update;

namespace sones.GraphFS.Element.Vertex
{
    /// <summary>
    /// The in memory representation of an ivertex
    /// </summary>
    public sealed class InMemoryVertex : AGraphElement, IVertex
    {
        #region data

        public readonly Boolean IsBulkVertex = true;

        /// <summary>
        /// The binary properties of this vertex
        /// </summary>
        private readonly IDictionary<Int64, Stream> _binaryProperties;

        /// <summary>
        /// The edition of the vertex
        /// </summary>
        private readonly string _edition;

        /// <summary>
        /// The incoming edges of the vertex
        /// (VertexTypeID of the vertex type that points to this vertex, PropertyID of the edge that points to this vertex, Incoming vertices)
        /// </summary>
        public IDictionary<Int64, Dictionary<Int64, IncomingEdgeCollection>> IncomingEdges;

        /// <summary>
        /// The outgoing edges of the vertex
        /// </summary>
        private readonly IDictionary<Int64, IEdge> _outgoingEdges;

        /// <summary>
        /// The id of the vertex
        /// </summary>
        private readonly Int64 _vertexID;

        /// <summary>
        /// The vertex type id
        /// </summary>
        private readonly Int64 _vertexTypeID;

        /// <summary>
        /// The revision id of the vertex
        /// </summary>
        private readonly Int64 _vertexRevisionID;
       
        #endregion

        #region constructor

        /// <summary>
        /// Creates a new in memory vertex
        /// </summary>
        /// <param name="myVertexID">The id of this vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myVertexRevisionID">The revision id of this vertex</param>
        /// <param name="myEdition">The edition of this vertex</param>
        /// <param name="myBinaryProperties">The binary properties of this vertex</param>
        /// <param name="myOutgoingEdges">The outgoing edges of this vertex</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public InMemoryVertex(
            Int64 myVertexID,
            Int64 myVertexTypeID,
            Int64 myVertexRevisionID,
            String myEdition,
            IDictionary<long, Stream> myBinaryProperties,
            IDictionary<long, IEdge> myOutgoingEdges,
            String myComment,
            long myCreationDate,
            long myModificationDate,
            IDictionary<Int64, IComparable> myStructuredProperties,
            IDictionary<String, Object> myUnstructuredProperties)
            :base(myComment, myCreationDate, myModificationDate, myStructuredProperties, myUnstructuredProperties)
        {
            _vertexID = myVertexID;
            _vertexTypeID = myVertexTypeID;
            _vertexRevisionID = myVertexRevisionID;
            _edition = myEdition;
            _binaryProperties = myBinaryProperties;
            _outgoingEdges = myOutgoingEdges;
            
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
            _vertexTypeID = myVertexTypeID;
            
            IsBulkVertex = true;
        }

        #endregion

        #region IVertex Members

        public bool HasIncomingVertices(long myVertexTypeID, long myEdgePropertyID)
        {
            return IncomingEdges != null &&
                   IncomingEdges.ContainsKey(myVertexTypeID) &&
                   IncomingEdges[myVertexTypeID].ContainsKey(myEdgePropertyID);
        }

        public IEnumerable<Tuple<long, long, IEnumerable<IVertex>>> GetAllIncomingVertices(
            PropertyHyperGraphFilter.IncomingVerticesFilter myFilter = null)
        {
            if (IncomingEdges != null)
            {
                foreach (var aType in IncomingEdges)
                {
                    foreach (var aEdge in aType.Value)
                    {
                        if (myFilter != null)
                        {
                            if (myFilter(aType.Key, aEdge.Key, aEdge.Value))
                            {
                                yield return new Tuple<long, long, IEnumerable<IVertex>>(aType.Key, aEdge.Key, aEdge.Value);
                            }
                        }
                        else
                        {
                            yield return new Tuple<long, long, IEnumerable<IVertex>>(aType.Key, aEdge.Key, aEdge.Value);
                        }
                    }
                }
            }

            yield break;
        }

        public IEnumerable<IVertex> GetIncomingVertices(Int64 myVertexTypeID, Int64 myEdgePropertyID)
        {
            return HasIncomingVertices(myVertexTypeID, myEdgePropertyID)
                       ? IncomingEdges[myVertexTypeID][myEdgePropertyID]
                       : new IncomingEdgeCollection(1);
        }

        public bool HasOutgoingEdge(long myEdgePropertyID)
        {
            return _outgoingEdges != null &&
                   _outgoingEdges.ContainsKey(myEdgePropertyID);
        }

        public IEnumerable<Tuple<long, IEdge>> GetAllOutgoingEdges(PropertyHyperGraphFilter.OutgoingEdgeFilter myFilter = null)
        {
            if (_outgoingEdges != null)
            {
                foreach (var aEdge in _outgoingEdges)
                {
                    if (myFilter != null)
                    {
                        if (myFilter(aEdge.Key, aEdge.Value))
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
            PropertyHyperGraphFilter.OutgoingHyperEdgeFilter myFilter = null)
        {
            if (_outgoingEdges != null)
            {
                foreach (var aEdge in _outgoingEdges)
                {
                    var interestingEdge = aEdge.Value as IHyperEdge;

                    if (interestingEdge == null) continue;

                    if (myFilter != null)
                    {
                        if (myFilter(aEdge.Key, interestingEdge))
                        {
                            yield return new Tuple<long, IHyperEdge>(aEdge.Key, interestingEdge);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, IHyperEdge>(aEdge.Key, interestingEdge);
                    }
                }
            }

            yield break;
        }

        public IEnumerable<Tuple<long, ISingleEdge>> GetAllOutgoingSingleEdges(
            PropertyHyperGraphFilter.OutgoingSingleEdgeFilter myFilter = null)
        {
            if (_outgoingEdges != null)
            {
                foreach (var aEdge in _outgoingEdges)
                {
                    var interestingEdge = aEdge.Value as ISingleEdge;

                    if (interestingEdge == null) continue;

                    if (myFilter != null)
                    {
                        if (myFilter(aEdge.Key, interestingEdge))
                        {
                            yield return new Tuple<long, ISingleEdge>(aEdge.Key, interestingEdge);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, ISingleEdge>(aEdge.Key, interestingEdge);
                    }
                }
            }

            yield break;
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

        public IEnumerable<Tuple<long, Stream>> GetAllBinaryProperties(PropertyHyperGraphFilter.BinaryPropertyFilter myFilter = null)
        {
            if (_binaryProperties != null)
            {
                foreach (var aBinary in _binaryProperties)
                {
                    if (myFilter != null)
                    {
                        if (myFilter(aBinary.Key, aBinary.Value))
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
                return (T) _structuredProperties[myPropertyID];
            }
            
            throw new CouldNotFindStructuredVertexPropertyException(_vertexTypeID,
                                                                    _vertexID, myPropertyID);
        }

        public IComparable GetProperty(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _structuredProperties[myPropertyID];
            }

            throw new CouldNotFindStructuredVertexPropertyException(_vertexTypeID,
                                                                    _vertexID, myPropertyID);
        }

        public bool HasProperty(long myPropertyID)
        {
            return _structuredProperties != null &&
                   _structuredProperties.ContainsKey(myPropertyID);
        }

        public int GetCountOfProperties()
        {
            return _structuredProperties == null ? 0 : _structuredProperties.Count;
        }

        public IEnumerable<Tuple<long, IComparable>> GetAllProperties(PropertyHyperGraphFilter.GraphElementStructuredPropertyFilter myFilter = null)
        {
            return GetAllPropertiesProtected(myFilter);
        }

        public string GetPropertyAsString(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _structuredProperties[myPropertyID].ToString();
            }
            
            throw new CouldNotFindStructuredVertexPropertyException(_vertexTypeID,
                                                                    _vertexID, myPropertyID);
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T) _unstructuredProperties[myPropertyName];
            }
            
            throw new CouldNotFindUnStructuredVertexPropertyException(_vertexTypeID,
                                                                      _vertexID, myPropertyName);
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _unstructuredProperties != null &&
                   _unstructuredProperties.ContainsKey(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties()
        {
            return _unstructuredProperties == null ? 0 : _unstructuredProperties.Count;
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(
            PropertyHyperGraphFilter.GraphElementUnStructuredPropertyFilter myFilter = null)
        {
            return GetAllUnstructuredPropertiesProtected(myFilter);
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return _unstructuredProperties[myPropertyName].ToString();
            }
            
            throw new CouldNotFindUnStructuredVertexPropertyException(_vertexTypeID,
                                                                      _vertexID, myPropertyName);
        }

        public string Comment
        {
            get { return _comment; }
        }

        public long CreationDate
        {
            get { return _creationDate; }
        }

        public long ModificationDate
        {
            get { return _modificationDate; }
        }

        public long VertexTypeID
        {
            get { return _vertexTypeID; }
        }

        public long VertexID
        {
            get { return _vertexID; }
        }

        public Int64 VertexRevisionID
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

        #region update methods

        public void UpdateComment(String myComment)
        {
            lock (_comment)
            {
                _comment = myComment;
            }
        }

        public void UpdateBinaryProperties(Dictionary<Int64, StreamAddDefinition> myBinaryUpdatedProperties, IEnumerable<Int64> myDeletedBinaryProperties)
        {
            lock (_binaryProperties)
            {
                foreach (var item in myDeletedBinaryProperties)
                {
                    _binaryProperties.Remove(item);
                }

                foreach (var item in myBinaryUpdatedProperties)
                {
                    if (_binaryProperties.ContainsKey(item.Value.PropertyID))
                    {
                        _binaryProperties[item.Value.PropertyID] = item.Value.Stream;
                    }
                }
            }
        }

        public void UpdateSingleEdges(SingleEdgeUpdate mySingleEdgeUpdate)
        {
            lock (_outgoingEdges)
            {
                foreach (var item in mySingleEdgeUpdate.Deleted)
                {
                    _outgoingEdges.Remove(item);
                }

                foreach (var item in mySingleEdgeUpdate.Updated)
                {
                    //_outgoingEdges[item.Key].Comment
                }
            }
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
                   && (_vertexTypeID == p._vertexTypeID);
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
            return _vertexID.GetHashCode() ^ _vertexTypeID.GetHashCode();
        }

        #endregion
    }
}