using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS.Element.Edge;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphFS.Element.Vertex
{
    /// <summary>
    /// The in memory representation of an ivertex
    /// </summary>
    public sealed class InMemoryVertex : IVertex
    {
        #region data

        /// <summary>
        /// Determines whether a vertex has been created because of a bulk import
        /// </summary>
        public readonly Boolean IsBulkVertex;

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
        private readonly InMemoryGraphElementInformation _inMemoryGraphElementInformation;

        /// <summary>
        /// The incoming edges of the vertex
        /// (VertexTypeID of the vertex type that points to this vertex, PropertyID of the edge that points to this vertex, SingleEdges)
        /// </summary>
        private Dictionary<Int64, Dictionary<long, HashSet<SingleEdge>>> _incomingEdges;

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
            InMemoryGraphElementInformation myGraphElementInformation)
        {
            _vertexID = myVertexID;
            _vertexRevisionID = myVertexRevisionID;
            _edition = myEdition;
            _binaryProperties = myBinaryProperties;
            _outgoingEdges = myOutgoingEdges;
            _inMemoryGraphElementInformation = myGraphElementInformation;
            _incomingEdges = null;
            IsBulkVertex = false;
        }

        /// <summary>
        /// Creates a new bulk in memory vertex in favour of adding an incoming edge
        /// </summary>
        /// <param name="myIncomingVertexTypeID">The id of the vertex type that aims to this vertex</param>
        /// <param name="myIncomingEdgeID">The outgoing edge property id that points to this vertex</param>
        /// <param name="mySingleEdge">The incoming edge</param>
        private InMemoryVertex(long myIncomingVertexTypeID, long myIncomingEdgeID, SingleEdge mySingleEdge)
        {
            IsBulkVertex = true;

            AddIncomingEdge(mySingleEdge, myIncomingVertexTypeID, myIncomingEdgeID);
        }

        /// <summary>
        /// Creates a new in memory vertex
        /// </summary>
        /// <param name="myVertexID">The id of this vertex</param>
        /// <param name="myVertexRevisionID">The revision id of this vertex</param>
        /// <param name="myEdition">The edition of this vertex</param>
        /// <param name="myBinaryProperties">The binary properties of this vertex</param>
        /// <param name="myOutgoingEdges">The outgoing edges of this vertex</param>
        /// <param name="myGraphElementInformation">The graph element information of this vertex</param>
        /// <param name="myIncomingEdges">The incoming edges</param>
        private InMemoryVertex(
            Int64 myVertexID,
            VertexRevisionID myVertexRevisionID,
            String myEdition,
            Dictionary<long, Stream> myBinaryProperties,
            Dictionary<long, IEdge> myOutgoingEdges,
            InMemoryGraphElementInformation myGraphElementInformation,
            Dictionary<long, Dictionary<long, HashSet<SingleEdge>>> myIncomingEdges)
            :this(myVertexID, myVertexRevisionID, myEdition, myBinaryProperties, myOutgoingEdges, myGraphElementInformation)
        {
            _incomingEdges = myIncomingEdges;
        }

        #endregion

        #region IVertex Members

        public bool HasIncomingEdge(long myVertexTypeID, long myEdgePropertyID)
        {
            return _incomingEdges != null &&
                   _incomingEdges.ContainsKey(myVertexTypeID) &&
                   _incomingEdges[myVertexTypeID].ContainsKey(myEdgePropertyID);
        }

        public IEnumerable<Tuple<long, long, IEnumerable<ISingleEdge>>> GetAllIncomingEdges(
            Func<long, long, IEnumerable<ISingleEdge>, bool> myFilterFunc = null)
        {
            if (_incomingEdges != null)
            {
                foreach (var aType in _incomingEdges)
                {
                    foreach (var aEdge in aType.Value)
                    {
                        if (myFilterFunc != null)
                        {
                            if (myFilterFunc(aType.Key, aEdge.Key, aEdge.Value))
                            {
                                yield return new Tuple<long, long, IEnumerable<ISingleEdge>>(aType.Key, aEdge.Key, aEdge.Value);
                            }
                        }
                        else
                        {
                            yield return new Tuple<long, long, IEnumerable<ISingleEdge>>(aType.Key, aEdge.Key, aEdge.Value);
                        }
                    }
                }

            }

            yield break;
        }

        public IEnumerable<ISingleEdge> GetIncomingHyperEdge(long myVertexTypeID, long myEdgePropertyID)
        {
            return HasIncomingEdge(myVertexTypeID, myEdgePropertyID)
                       ? _incomingEdges[myVertexTypeID][myEdgePropertyID]
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
                return (T) _inMemoryGraphElementInformation.StructuredProperties[myPropertyID];
            }
            
            throw new CouldNotFindStructuredVertexPropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                    _vertexID, myPropertyID);
        }

        public bool HasProperty(long myPropertyID)
        {
            return _inMemoryGraphElementInformation.StructuredProperties != null &&
                   _inMemoryGraphElementInformation.StructuredProperties.ContainsKey(myPropertyID);
        }

        public int GetCountOfProperties()
        {
            return _inMemoryGraphElementInformation.StructuredProperties == null ? 0 : _inMemoryGraphElementInformation.StructuredProperties.Count;
        }

        public IEnumerable<Tuple<long, object>> GetAllProperties(Func<long, object, bool> myFilterFunc = null)
        {
            return _inMemoryGraphElementInformation.GetAllPropertiesProtected(myFilterFunc);
        }

        public string GetPropertyAsString(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _inMemoryGraphElementInformation.StructuredProperties[myPropertyID].ToString();
            }
            
            throw new CouldNotFindStructuredVertexPropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                    _vertexID, myPropertyID);
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T) _inMemoryGraphElementInformation.UnstructuredProperties[myPropertyName];
            }
            
            throw new CouldNotFindUnStructuredVertexPropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                      _vertexID, myPropertyName);
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _inMemoryGraphElementInformation.UnstructuredProperties != null &&
                   _inMemoryGraphElementInformation.UnstructuredProperties.ContainsKey(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties()
        {
            return _inMemoryGraphElementInformation.UnstructuredProperties == null ? 0 : _inMemoryGraphElementInformation.UnstructuredProperties.Count;
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(
            Func<string, object, bool> myFilterFunc = null)
        {
            return _inMemoryGraphElementInformation.GetAllUnstructuredPropertiesProtected(myFilterFunc);
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return _inMemoryGraphElementInformation.UnstructuredProperties[myPropertyName].ToString();
            }
            
            throw new CouldNotFindUnStructuredVertexPropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                      _vertexID, myPropertyName);
        }

        public string Comment
        {
            get { return _inMemoryGraphElementInformation.Comment; }
        }

        public DateTime CreationDate
        {
            get { return _inMemoryGraphElementInformation.CreationDate; }
        }

        public DateTime ModificationDate
        {
            get { return _inMemoryGraphElementInformation.ModificationDate; }
        }

        public long TypeID
        {
            get { return _inMemoryGraphElementInformation.TypeID; }
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

        /// <summary>
        /// Adds an incoming edge to the vertex
        /// </summary>
        /// <param name="myIncomingEdge">The edge that should be added</param>
        /// <param name="myIncomingVertexTypeID">The id of the vertex type that aims to this vertex</param>
        /// <param name="myIncomingEdgePropertyID">The outgoing edge property id that points to this vertex</param>
        private void AddIncomingEdge(SingleEdge myIncomingEdge, Int64 myIncomingVertexTypeID, Int64 myIncomingEdgePropertyID)
        {
            if (_incomingEdges == null)
            {
                _incomingEdges = new Dictionary<long, Dictionary<long, HashSet<SingleEdge>>>();
            }

            if (!_incomingEdges.ContainsKey(myIncomingVertexTypeID))
            {
                _incomingEdges.Add(myIncomingVertexTypeID, new Dictionary<long, HashSet<SingleEdge>>());
            }

            if (!_incomingEdges[myIncomingVertexTypeID].ContainsKey(myIncomingEdgePropertyID))
            {
                _incomingEdges[myIncomingVertexTypeID].Add(myIncomingEdgePropertyID, new HashSet<SingleEdge>() { myIncomingEdge });
            }
            else
            {
                _incomingEdges[myIncomingVertexTypeID][myIncomingEdgePropertyID].Add(myIncomingEdge);
            }
        }

        #endregion

        #region static methods

        internal static InMemoryVertex CopyAndAddIncomingEdge(InMemoryVertex myToBeCopiedVertex, long myIncomingVertexTypeID, long myIncomingEdgeID, SingleEdge mySingleEdge)
        {
            //copy the ones that are not interesting
            var incomingEdges = new Dictionary<long, Dictionary<long, HashSet<SingleEdge>>>(myToBeCopiedVertex._incomingEdges);

            if (incomingEdges.ContainsKey(myIncomingVertexTypeID) && incomingEdges[myIncomingVertexTypeID].ContainsKey(myIncomingEdgeID))
            {
                //there is sth to update
                HashSet<SingleEdge> toBeUpdated = new HashSet<SingleEdge>(incomingEdges[myIncomingVertexTypeID][myIncomingEdgeID]);

                toBeUpdated.Add(mySingleEdge);

                incomingEdges[myIncomingVertexTypeID][myIncomingEdgeID] = toBeUpdated;
            }
            else
            {
                //create a new one
                if (!incomingEdges.ContainsKey(myIncomingVertexTypeID))
                {
                    incomingEdges.Add(myIncomingVertexTypeID, new Dictionary<long, HashSet<SingleEdge>>());
                }

                incomingEdges[myIncomingVertexTypeID].Add(myIncomingEdgeID, new HashSet<SingleEdge>() { mySingleEdge });
            }

            return new InMemoryVertex(
                myToBeCopiedVertex._vertexID,
                myToBeCopiedVertex._vertexRevisionID,
                myToBeCopiedVertex._edition,
                myToBeCopiedVertex._binaryProperties,
                myToBeCopiedVertex._outgoingEdges,
                myToBeCopiedVertex._inMemoryGraphElementInformation,
                incomingEdges);
        }

        internal static InMemoryVertex CreateBulkVertexWithIncomingEdge(long myIncomingVertexTypeID, long myIncomingEdgeID, SingleEdge mySingleEdge)
        {
            return new InMemoryVertex(myIncomingVertexTypeID, myIncomingEdgeID, mySingleEdge);
        }

        internal static InMemoryVertex CopyFromBulkVertex(InMemoryVertex oldBulkVertex, InMemoryVertex myVertex)
        {
            return new InMemoryVertex(
                myVertex._vertexID,
                myVertex._vertexRevisionID,
                myVertex._edition,
                myVertex._binaryProperties,
                myVertex._outgoingEdges,
                myVertex._inMemoryGraphElementInformation,
                oldBulkVertex._incomingEdges);
        }

        internal static InMemoryVertex CopyFromIVertex(IVertex aVertex)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}