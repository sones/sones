using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphFS.Definitions;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.PropertyHyperGraph;

namespace sones.GraphFS.Element.Edge
{
    /// <summary>
    /// A hyper edge is a 1-N relation within the property hypergraph
    /// </summary>
    public sealed class HyperEdge : IHyperEdge
    {
        #region data

        /// <summary>
        /// The single edges that are contained within this hyper edge
        /// </summary>
        private readonly HashSet<SingleEdge> _containedSingleEdges;

        /// <summary>
        /// A function to resolve the source vertex or the target vertices
        /// </summary>
        private readonly Func<long, long, IVertex> _getVertexFunc;

        /// <summary>
        /// Properties
        /// </summary>
        private readonly InMemoryGraphElementInformation _inMemoryGraphElementInformation;

        /// <summary>
        /// The location of the source vertex
        /// </summary>
        private readonly VertexLocation _sourceLocation;

        /// <summary>
        /// All the target locations
        /// </summary>
        private readonly HashSet<VertexLocation> _targetLocations;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new hyper edge
        /// </summary>
        /// <param name="myEdgeDefinition">The definition of the hyper edge</param>
        /// <param name="myGetVertex">The function to resolve either the source vertex or the target vertices</param>
        public HyperEdge(
            HyperEdgeAddDefinition myEdgeDefinition,
            Func<long, long, IVertex> myGetVertex)
        {
            _inMemoryGraphElementInformation =
                new InMemoryGraphElementInformation(myEdgeDefinition.GraphElementInformation);

            _getVertexFunc = myGetVertex;

            _sourceLocation = new VertexLocation(myEdgeDefinition.SourceVertex.VertexTypeID,
                                                 myEdgeDefinition.SourceVertex.VertexID);

            _targetLocations = new HashSet<VertexLocation>();
            _containedSingleEdges = new HashSet<SingleEdge>();
            foreach (var aSingleEdge in myEdgeDefinition.ContainedSingleEdges)
            {
                _containedSingleEdges.Add(new SingleEdge(aSingleEdge, myGetVertex));
                _targetLocations.Add(new VertexLocation(aSingleEdge.TargetVertexInformation.VertexTypeID,
                                                        aSingleEdge.TargetVertexInformation.VertexID));
            }
        }

        #endregion

        #region IHyperEdge Members

        public IEnumerable<ISingleEdge> GetEdges(Func<ISingleEdge, bool> myFilterFunction = null)
        {
            foreach (var aSingleEdge in
                _containedSingleEdges.Where(aSingleEdge => (myFilterFunction != null) && (myFilterFunction(aSingleEdge))))
            {
                yield return aSingleEdge;
            }

            yield break;
        }

        public TResult InvokeHyperEdgeFunc<TResult>(Func<IEnumerable<ISingleEdge>, TResult> myHyperEdgeFunction)
        {
            return myHyperEdgeFunction(_containedSingleEdges);
        }

        public IVertex GetSourceVertex()
        {
            return _getVertexFunc(_sourceLocation.VertexID, _sourceLocation.VertexTypeID);
        }

        public IEnumerable<IVertex> GetTargetVertices(Func<IVertex, bool> myFilterFunc = null)
        {
            foreach (var targetVertex in
                _targetLocations.Select(aTargetVertexLocation => _getVertexFunc(aTargetVertexLocation.VertexID, aTargetVertexLocation.VertexTypeID)))
            {
                if (myFilterFunc != null)
                {
                    if (myFilterFunc(targetVertex))
                    {
                        yield return targetVertex;
                    }
                }
                else
                {
                    yield return targetVertex;
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
                throw new CouldNotFindStructuredEdgePropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                      myPropertyID);
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
                throw new CouldNotFindStructuredEdgePropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                      myPropertyID);
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
                throw new CouldNotFindUnStructuredEdgePropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                        myPropertyName);
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
                throw new CouldNotFindUnStructuredEdgePropertyException(_inMemoryGraphElementInformation.TypeID,
                                                                        myPropertyName);
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

        public IEdgeStatistics Statistics
        {
            get { return null; }
        }

        #endregion
    }
}