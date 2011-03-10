using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS.Definitions;
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
        /// (VertexTypeID of the vertex type that points to this vertex, PropertyID of the edge that points to this vertex, HyperEdge)
        /// </summary>
        private readonly Dictionary<Int64, Dictionary<long, HyperEdge>> _incomingEdges;

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
        }

        #endregion

        #region IVertex Members

        public bool HasIncomingEdge(long myVertexTypeID, long myEdgePropertyID)
        {
            return _incomingEdges.ContainsKey(myVertexTypeID) &&
                   _incomingEdges[myVertexTypeID].ContainsKey(myEdgePropertyID);
        }

        public IEnumerable<Tuple<long, long, IHyperEdge>> GetAllIncomingEdges(
            Func<long, long, IHyperEdge, bool> myFilterFunc = null)
        {
            foreach (var aType in _incomingEdges)
            {
                foreach (var aEdge in aType.Value)
                {
                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aType.Key, aEdge.Key, aEdge.Value))
                        {
                            yield return new Tuple<long, long, IHyperEdge>(aType.Key, aEdge.Key, aEdge.Value);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, long, IHyperEdge>(aType.Key, aEdge.Key, aEdge.Value);
                    }
                }
            }

            yield break;
        }

        public IHyperEdge GetIncomingHyperEdge(long myVertexTypeID, long myEdgePropertyID)
        {
            return HasIncomingEdge(myVertexTypeID, myEdgePropertyID)
                       ? _incomingEdges[myVertexTypeID][myEdgePropertyID]
                       : null;
        }

        public bool HasOutgoingEdge(long myEdgePropertyID)
        {
            return _outgoingEdges.ContainsKey(myEdgePropertyID);
        }

        public IEnumerable<Tuple<long, IEdge>> GetAllOutgoingEdges(Func<long, IEdge, bool> myFilterFunc = null)
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

            yield break;
        }

        public IEnumerable<Tuple<long, IHyperEdge>> GetAllOutgoingHyperEdges(
            Func<long, IHyperEdge, bool> myFilterFunc = null)
        {
            return GetAllOutgoingEdges_private(myFilterFunc);
        }

        public IEnumerable<Tuple<long, ISingleEdge>> GetAllOutgoingSingleEdges(
            Func<long, ISingleEdge, bool> myFilterFunc = null)
        {
            return GetAllOutgoingEdges_private(myFilterFunc);
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
            return _binaryProperties.ContainsKey(myPropertyID) ? _binaryProperties[myPropertyID] : null;
        }

        public IEnumerable<Tuple<long, Stream>> GetAllBinaryProperties(Func<long, Stream, bool> myFilterFunc = null)
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

            yield break;
        }

        public T GetProperty<T>(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return (T) _inMemoryGraphElementInformation.StructuredProperties[myPropertyID];
            }
            else
            {
                throw new CouldNotFindStructuredVertexPropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                        _vertexID, myPropertyID);
            }
        }

        public bool HasProperty(long myPropertyID)
        {
            return _inMemoryGraphElementInformation.StructuredProperties.ContainsKey(myPropertyID);
        }

        public int GetCountOfProperties()
        {
            return _inMemoryGraphElementInformation.StructuredProperties.Count;
        }

        public IEnumerable<Tuple<long, object>> GetAllProperties(Func<long, object, bool> myFilterFunc = null)
        {
            return _inMemoryGraphElementInformation.GetAllProperties_protected(myFilterFunc);
        }

        public string GetPropertyAsString(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _inMemoryGraphElementInformation.StructuredProperties[myPropertyID].ToString();
            }
            else
            {
                throw new CouldNotFindStructuredVertexPropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                        _vertexID, myPropertyID);
            }
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T) _inMemoryGraphElementInformation.UnstructuredProperties[myPropertyName];
            }
            else
            {
                throw new CouldNotFindUnStructuredVertexPropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                          _vertexID, myPropertyName);
            }
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _inMemoryGraphElementInformation.UnstructuredProperties.ContainsKey(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties()
        {
            return _inMemoryGraphElementInformation.UnstructuredProperties.Count;
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(
            Func<string, object, bool> myFilterFunc = null)
        {
            return _inMemoryGraphElementInformation.GetAllUnstructuredProperties_protected(myFilterFunc);
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return _inMemoryGraphElementInformation.UnstructuredProperties[myPropertyName].ToString();
            }
            else
            {
                throw new CouldNotFindUnStructuredVertexPropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                          _vertexID, myPropertyName);
            }
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
        private IEnumerable<Tuple<long, T>> GetAllOutgoingEdges_private<T>(Func<long, T, bool> myFilterFunc)
            where T : class
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

            yield break;
        }

        #endregion

        #endregion
    }
}