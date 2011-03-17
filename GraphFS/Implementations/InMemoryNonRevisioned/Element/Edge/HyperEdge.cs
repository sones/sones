using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphFS.Definitions;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphFS.Element.Edge
{
    /// <summary>
    /// A hyper edge is a 1-N relation within the property hypergraph
    /// </summary>
    public sealed class HyperEdge : IHyperEdge
    {
        #region data

        /// <summary>
        /// The edge type id
        /// </summary>
        private readonly Int64 _edgeTypeID;

        /// <summary>
        /// The single edges that are contained within this hyper edge
        /// </summary>
        private readonly List<SingleEdge> _containedSingleEdges;

        /// <summary>
        /// Properties
        /// </summary>
        private readonly GraphElementInformation _graphElementInformation;

        /// <summary>
        /// The source vertex
        /// </summary>
        private readonly InMemoryVertex _sourceVertex;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new hyper edge
        /// </summary>
        /// <param name="myContainedSingleEdges">The single edges that are contained within the hyper edge</param>
        /// <param name="myEdgeTypeID">The type id of the edge</param>
        /// <param name="myGraphElementInformation">The graph element information of the hyper edge</param>
        /// <param name="mySourceVertex">The source vertex</param>
        public HyperEdge(
            List<SingleEdge> myContainedSingleEdges,
            Int64 myEdgeTypeID,
            GraphElementInformation myGraphElementInformation,
            InMemoryVertex mySourceVertex)
        {
            _edgeTypeID = myEdgeTypeID;

            _graphElementInformation = myGraphElementInformation;

            _sourceVertex = mySourceVertex;
            
            _containedSingleEdges = myContainedSingleEdges;
        }

        #endregion

        #region IHyperEdge Members

        public IEnumerable<ISingleEdge> GetEdges(Filter.SingleEdgeFilter myFilter = null)
        {
            if (myFilter != null)
            {
                foreach (var aSingleEdge in _containedSingleEdges)
                {
                    if (myFilter(aSingleEdge))
                    {
                        yield return aSingleEdge;
                    }
                }
            }
            else
            {
                foreach (var aSingleEdge in _containedSingleEdges)
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

        public IVertex GetSourceVertex()
        {
            return _sourceVertex;
        }

        public IEnumerable<IVertex> GetTargetVertices(Filter.TargetVertexFilter myFilter = null)
        {
            foreach (var targetVertex in
                _containedSingleEdges.Select(aSingleEdge =>
                                             aSingleEdge.GetTargetVertex()))
            {
                if (myFilter != null)
                {
                    if (myFilter(targetVertex))
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
                return (T) _graphElementInformation.StructuredProperties[myPropertyID];
            }
            
            throw new CouldNotFindStructuredEdgePropertyException(_edgeTypeID,
                                                                  myPropertyID);
        }

        public bool HasProperty(long myPropertyID)
        {
            return _graphElementInformation.StructuredProperties.ContainsKey(myPropertyID);
        }

        public int GetCountOfProperties()
        {
            return _graphElementInformation.StructuredProperties.Count;
        }

        public IEnumerable<Tuple<long, object>> GetAllProperties(Filter.GraphElementStructuredPropertyFilter myFilter = null)
        {
            return _graphElementInformation.GetAllPropertiesProtected(myFilter);
        }

        public string GetPropertyAsString(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _graphElementInformation.StructuredProperties[myPropertyID].ToString();
            }
            
            throw new CouldNotFindStructuredEdgePropertyException(_edgeTypeID,
                                                                  myPropertyID);
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T) _graphElementInformation.UnstructuredProperties[myPropertyName];
            }
            
            throw new CouldNotFindUnStructuredEdgePropertyException(_edgeTypeID,
                                                                    myPropertyName);
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _graphElementInformation.UnstructuredProperties.ContainsKey(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties()
        {
            return _graphElementInformation.UnstructuredProperties.Count;
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(
            Filter.GraphElementUnStructuredPropertyFilter myFilter = null)
        {
            return _graphElementInformation.GetAllUnstructuredPropertiesProtected(myFilter);
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return _graphElementInformation.UnstructuredProperties[myPropertyName].ToString();
            }
            
            throw new CouldNotFindUnStructuredEdgePropertyException(_edgeTypeID,
                                                                    myPropertyName);
        }

        public string Comment
        {
            get { return _graphElementInformation.Comment; }
        }

        public long CreationDate
        {
            get { return _graphElementInformation.CreationDate; }
        }

        public long ModificationDate
        {
            get { return _graphElementInformation.ModificationDate; }
        }

        public long EdgeTypeID
        {
            get { return _edgeTypeID; }
        }

        public IEdgeStatistics Statistics
        {
            get { return null; }
        }

        #endregion
    }
}