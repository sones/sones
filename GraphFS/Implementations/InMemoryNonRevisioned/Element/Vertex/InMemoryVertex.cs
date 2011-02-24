using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS.ErrorHandling;
using sones.PropertyHyperGraph;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The in memory representation of an ivertex
    /// </summary>
    public sealed class InMemoryVertex : IVertex
    {
        #region data

        /// <summary>
        /// The id of the vertex
        /// </summary>
        private readonly UInt64 _vertexID;

        /// <summary>
        /// The revision id of the vertex
        /// </summary>
        private readonly VertexRevisionID _vertexRevisionID;

        /// <summary>
        /// The vertex type id of the vertex
        /// </summary>
        private readonly UInt64 _typeID;

        /// <summary>
        /// The comment of the vertex
        /// </summary>
        private readonly string _comment;

        /// <summary>
        /// The creation date of the vertex
        /// </summary>
        private readonly DateTime _creationDate;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        private readonly DateTime _modificationDate;

        /// <summary>
        /// The edition of the vertex
        /// </summary>
        private readonly string _edition;

        /// <summary>
        /// The structured properties of the vertex
        /// </summary>
        private readonly Dictionary<UInt64, Object> _structuredProperties;

        /// <summary>
        /// The unstructured properties of the vertex
        /// </summary>
        private readonly Dictionary<String, Object> _unstructuredProperties;

        /// <summary>
        /// The outgoing edges of the vertex
        /// </summary>
        private readonly Dictionary<UInt64, IEdge> _outgoingEdges;

        /// <summary>
        /// The incoming edges of the vertex
        /// (VertexTypeID of the vertex type that points to this vertex, PropertyID of the edge that points to this vertex, HyperEdge)
        /// </summary>
        private Dictionary<UInt64, Dictionary<UInt64, HyperEdge>> _incomingEdges;

        /// <summary>
        /// The binary properties of this vertex
        /// </summary>
        private readonly Dictionary<UInt64, Stream> _binaryProperties;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new in memory vertex
        /// </summary>
        /// <param name="myVertexID">The id of this vertex</param>
        /// <param name="myVertexRevisionID">The revision id of this vertex</param>
        /// <param name="myVertexDefinition">The definition of this vertex</param>
        public InMemoryVertex(
            UInt64 myVertexID,
            VertexRevisionID myVertexRevisionID,
            VertexInsertDefinition myVertexDefinition)
        {
            _vertexID = myVertexID;
            _vertexRevisionID = myVertexRevisionID;

            _typeID = myVertexDefinition.TypeID;
            _comment = myVertexDefinition.Comment;
            _creationDate = myVertexDefinition.CreationDate;
            _modificationDate = myVertexDefinition.ModificationDate;
            _edition = myVertexDefinition.Edition;

            _structuredProperties = myVertexDefinition.StructuredProperties;
            _unstructuredProperties = myVertexDefinition.UnstructuredProperties;
            _binaryProperties = myVertexDefinition.BinaryProperties;

            _outgoingEdges = ConvertToIEdge(myVertexDefinition.OutgoingEdges);
            _incomingEdges = new Dictionary<ulong, Dictionary<ulong, HyperEdge>>();
        }

        #endregion

        #region IVertex Members

        public bool HasIncomingEdge(ulong myVertexTypeID, ulong myEdgePropertyID)
        {
            return _incomingEdges.ContainsKey(myVertexTypeID) && _incomingEdges[myVertexTypeID].ContainsKey(myEdgePropertyID);
        }

        public IEnumerable<Tuple<ulong, ulong, IHyperEdge>> GetAllIncomingEdges(Func<ulong, ulong, IHyperEdge, bool> myFilterFunc = null)
        {
            foreach (var aType in _incomingEdges)
            {
                foreach (var aEdge in aType.Value)
                {
                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aType.Key, aEdge.Key, aEdge.Value))
                        {
                            yield return new Tuple<ulong, ulong, IHyperEdge>(aType.Key, aEdge.Key, aEdge.Value);                            
                        }
                    }
                    else
                    {
                        yield return new Tuple<ulong, ulong, IHyperEdge>(aType.Key, aEdge.Key, aEdge.Value);
                    }
                }
            }

            yield break;
        }

        public IHyperEdge GetIncomingHyperEdge(ulong myVertexTypeID, ulong myEdgePropertyID)
        {
            if (HasIncomingEdge(myVertexTypeID, myEdgePropertyID))
            {
                return _incomingEdges[myVertexTypeID][myEdgePropertyID];
            }
            else
            {
                return null;
            }
        }

        public bool HasOutgoingEdge(ulong myEdgePropertyID)
        {
            return _outgoingEdges.ContainsKey(myEdgePropertyID);
        }

        public IEnumerable<Tuple<ulong, IEdge>> GetAllOutgoingEdges(Func<ulong, IEdge, bool> myFilterFunc = null)
        {
            foreach (var aEdge in _outgoingEdges)
            {
                if (myFilterFunc != null)
                {
                    if (myFilterFunc(aEdge.Key, aEdge.Value))
                    {
                        yield return new Tuple<ulong, IEdge>(aEdge.Key, aEdge.Value);                        
                    }
                }
                else
                {
                    yield return new Tuple<ulong, IEdge>(aEdge.Key, aEdge.Value);
                }
            }

            yield break;
        }

        public IEnumerable<Tuple<ulong, IHyperEdge>> GetAllOutgoingHyperEdges(Func<ulong, IHyperEdge, bool> myFilterFunc = null)
        {
            return GetAllOutgoingEdges_private<IHyperEdge>(myFilterFunc);
        }

        public IEnumerable<Tuple<ulong, ISingleEdge>> GetAllOutgoingSingleEdges(Func<ulong, ISingleEdge, bool> myFilterFunc = null)
        {
            return GetAllOutgoingEdges_private<ISingleEdge>(myFilterFunc);
        }

        public IEdge GetOutgoingEdge(ulong myEdgePropertyID)
        {
            if (HasOutgoingEdge(myEdgePropertyID))
            {
                return _outgoingEdges[myEdgePropertyID];
            }

            return null;
        }

        public IHyperEdge GetOutgoingHyperEdge(ulong myEdgePropertyID)
        {
            var edge = GetOutgoingEdge(myEdgePropertyID);

            return edge as IHyperEdge;
        }

        public ISingleEdge GetOutgoingSingleEdge(ulong myEdgePropertyID)
        {
            var edge = GetOutgoingEdge(myEdgePropertyID);

            return edge as ISingleEdge;
        }

        public Stream GetBinaryProperty(ulong myPropertyID)
        {
            if (_binaryProperties.ContainsKey(myPropertyID))
            {
                return _binaryProperties[myPropertyID];
            }

            return null;
        }

        public IEnumerable<Tuple<ulong, Stream>> GetAllBinaryProperties(Func<ulong, Stream, bool> myFilterFunc = null)
        {
            foreach (var aBinary in _binaryProperties)
            {
                if (myFilterFunc != null)
                {
                    if (myFilterFunc(aBinary.Key, aBinary.Value))
                    {
                        yield return new Tuple<ulong, Stream>(aBinary.Key, aBinary.Value);
                    }
                }
                else
                {
                    yield return new Tuple<ulong, Stream>(aBinary.Key, aBinary.Value);
                }

            }

            yield break;
        }

        #endregion

        #region IGraphElement Members

        public T GetProperty<T>(ulong myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return (T)_structuredProperties[myPropertyID];
            }
            else
            {
                throw new CouldNotFindStructuredVertexPropertyException(_typeID, _vertexID, myPropertyID);
            }
        }

        public bool HasProperty(ulong myPropertyID)
        {
            return _structuredProperties.ContainsKey(myPropertyID);
        }

        public ulong GetCountOfProperties()
        {
            return Convert.ToUInt64(_structuredProperties.Count);
        }

        public IEnumerable<Tuple<ulong, object>> GetAllProperties(Func<ulong, object, bool> myFilterFunc = null)
        {
            foreach (var aProperty in _structuredProperties)
            {
                if (myFilterFunc != null)
                {
                    if (myFilterFunc(aProperty.Key, aProperty.Value))
                    {
                        yield return new Tuple<ulong, object>(aProperty.Key, aProperty.Value);
                    }
                }
                else
                {
                    yield return new Tuple<ulong, object>(aProperty.Key, aProperty.Value);
                }
            }

            yield break;
        }

        public string GetPropertyAsString(ulong myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _structuredProperties[myPropertyID].ToString();
            }
            else
            {
                throw new CouldNotFindStructuredVertexPropertyException(_typeID, _vertexID, myPropertyID);
            }
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T)_unstructuredProperties[myPropertyName];
            }
            else
            {
                throw new CouldNotFindUnStructuredVertexPropertyException(_typeID, _vertexID, myPropertyName);
            }
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _unstructuredProperties.ContainsKey(myPropertyName);
        }

        public ulong GetCountOfUnstructuredProperties()
        {
            return Convert.ToUInt64(_unstructuredProperties.Count);
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(Func<string, object, bool> myFilterFunc = null)
        {
            foreach (var aUnstructuredProperty in _unstructuredProperties)
            {
                if (myFilterFunc != null)
                {
                    if (myFilterFunc(aUnstructuredProperty.Key, aUnstructuredProperty.Value))
                    {
                        yield return new Tuple<String, object>(aUnstructuredProperty.Key, aUnstructuredProperty.Value);
                    }
                }
                else
                {
                    yield return new Tuple<String, object>(aUnstructuredProperty.Key, aUnstructuredProperty.Value);
                }
            }

            yield break;
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return _unstructuredProperties[myPropertyName].ToString();
            }
            else
            {
                throw new CouldNotFindUnStructuredVertexPropertyException(_typeID, _vertexID, myPropertyName);
            }
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

        #region ConvertToIEdge

        /// <summary>
        /// Converts edge definitions into iEdges
        /// </summary>
        /// <param name="myEdgeDefinitions">The edge definitions</param>
        /// <returns>A dictionary that conains the iEdges</returns>
        private Dictionary<ulong, IEdge> ConvertToIEdge(Dictionary<ulong, EdgeAddDefinition> myEdgeDefinitions)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetAllOutgoingEdges_private

        /// <summary>
        /// Returns all outgoing edges corresponding to their type and an optional filter function
        /// </summary>
        /// <typeparam name="T">The type of the result</typeparam>
        /// <param name="myFilterFunc">The optional filter function</param>
        /// <returns>All matching outgoing edges</returns>
        private IEnumerable<Tuple<ulong, T>> GetAllOutgoingEdges_private<T>(Func<ulong, T, bool> myFilterFunc) where T : class
        {
            T interestingEdge;

            foreach (var aEdge in _outgoingEdges)
            {
                interestingEdge = aEdge.Value as T;

                if (interestingEdge != null)
                {
                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aEdge.Key, interestingEdge))
                        {
                            yield return new Tuple<ulong, T>(aEdge.Key, interestingEdge);
                        }
                    }
                    else
                    {
                        yield return new Tuple<ulong, T>(aEdge.Key, interestingEdge);
                    }
                }
            }

            yield break;
        }

        #endregion

        #endregion
    }
}
