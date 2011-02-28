using System;
using System.Linq;
using System.Collections.Generic;
using sones.PropertyHyperGraph;
using sones.GraphFS.ErrorHandling;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// A hyper edge is a 1-N relation within the property hypergraph
    /// </summary>
    public sealed class HyperEdge : IHyperEdge
    {
        #region data

        /// <summary>
        /// Properties
        /// </summary>
        private readonly InMemoryGraphElementInformation _inMemoryGraphElementInformation;

        /// <summary>
        /// The single edges that are contained within this hyper edge
        /// </summary>
        private readonly HashSet<SingleEdge> _containedSingleEdges;

        /// <summary>
        /// The location of the source vertex
        /// </summary>
        private readonly VertexLocation _sourceLocation;

        /// <summary>
        /// All the target locations
        /// </summary>
        private readonly HashSet<VertexLocation> _targetLocations;

        /// <summary>
        /// A function to resolve the source vertex or the target vertices
        /// </summary>
        private readonly Func<ulong, ulong, IVertex> _getVertexFunc;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new hyper edge
        /// </summary>
        /// <param name="myEdgeDefinition">The definition of the hyper edge</param>
        /// <param name="myGetVertex">The function to resolve either the source vertex or the target vertices</param>
        public HyperEdge(
            HyperEdgeAddDefinition myEdgeDefinition,
            Func<ulong, ulong, IVertex> myGetVertex)
        {
            _inMemoryGraphElementInformation = new InMemoryGraphElementInformation(myEdgeDefinition.GraphElementInformation);

            _getVertexFunc = myGetVertex;

            _sourceLocation = new VertexLocation(myEdgeDefinition.SourceVertex.VertexTypeID, myEdgeDefinition.SourceVertex.VertexID);

            _targetLocations = new HashSet<VertexLocation>();
            _containedSingleEdges = new HashSet<SingleEdge>();
            foreach (var aSingleEdge in myEdgeDefinition.ContainedSingleEdges)
            {
                _containedSingleEdges.Add(new SingleEdge(aSingleEdge, myGetVertex));
                _targetLocations.Add(new VertexLocation(aSingleEdge.TargetVertexInformation.VertexTypeID, aSingleEdge.TargetVertexInformation.VertexID));
            }
        }

        #endregion

        #region IHyperEdge Members

        public IEnumerable<ISingleEdge> GetEdges(Func<ISingleEdge, bool> myFilterFunction = null)
        {
            foreach (var aSingleEdge in _containedSingleEdges)
            {
                if ((myFilterFunction != null) && (myFilterFunction(aSingleEdge)))
                {
                    yield return aSingleEdge;
                }
            }

            yield break;
        }

        public TResult InvokeHyperEdgeFunc<TResult>(Func<IEnumerable<ISingleEdge>, TResult> myHyperEdgeFunction)
        {
            return myHyperEdgeFunction(_containedSingleEdges);
        }

        #endregion

        #region IEdge Members

        public IVertex GetSourceVertex()
        {
            return _getVertexFunc(_sourceLocation.VertexID, _sourceLocation.VertexTypeID);
        }

        public IEnumerable<IVertex> GetTargetVertices(Func<IVertex, bool> myFilterFunc = null)
        {
            IVertex targetVertex = null;

            foreach (var aTargetVertexLocation in _targetLocations)
            {
                targetVertex = _getVertexFunc(aTargetVertexLocation.VertexID, aTargetVertexLocation.VertexTypeID);

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

        #endregion

        #region IGraphElement Members

        public T GetProperty<T>(ulong myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return (T)_inMemoryGraphElementInformation.StructuredProperties[myPropertyID];
            }
            else
            {
                throw new CouldNotFindStructuredEdgePropertyException(_inMemoryGraphElementInformation.TypeID, myPropertyID);
            }
        }

        public bool HasProperty(ulong myPropertyID)
        {
            return _inMemoryGraphElementInformation.StructuredProperties.ContainsKey(myPropertyID);            
        }

        public ulong GetCountOfProperties()
        {
            return Convert.ToUInt64(_inMemoryGraphElementInformation.StructuredProperties.Count);            
        }

        public IEnumerable<Tuple<ulong, object>> GetAllProperties(Func<ulong, object, bool> myFilterFunc = null)
        {
            return _inMemoryGraphElementInformation.GetAllProperties_protected(myFilterFunc);            
        }

        public string GetPropertyAsString(ulong myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _inMemoryGraphElementInformation.StructuredProperties[myPropertyID].ToString();
            }
            else
            {
                throw new CouldNotFindStructuredEdgePropertyException(_inMemoryGraphElementInformation.TypeID, myPropertyID);
            }
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T)_inMemoryGraphElementInformation.UnstructuredProperties[myPropertyName];
            }
            else
            {
                throw new CouldNotFindUnStructuredEdgePropertyException(_inMemoryGraphElementInformation.TypeID, myPropertyName);
            }
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _inMemoryGraphElementInformation.UnstructuredProperties.ContainsKey(myPropertyName);            
        }

        public ulong GetCountOfUnstructuredProperties()
        {
            return Convert.ToUInt64(_inMemoryGraphElementInformation.UnstructuredProperties.Count);            
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(Func<string, object, bool> myFilterFunc = null)
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
                throw new CouldNotFindUnStructuredEdgePropertyException(_inMemoryGraphElementInformation.TypeID, myPropertyName);
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

        public ulong TypeID
        {
            get { return _inMemoryGraphElementInformation.TypeID; }
        }

        #endregion

        #region IEdgeProperties Members

        public IEdgeStatistics Statistics
        {
            get { return null; }
        }

        #endregion
    }
}