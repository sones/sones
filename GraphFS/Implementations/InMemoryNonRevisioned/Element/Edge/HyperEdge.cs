using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphFS.Element.Edge
{
    /// <summary>
    /// A hyper edge is a 1-N relation within the property hypergraph
    /// </summary>
    public sealed class HyperEdge : AGraphElement, IHyperEdge
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
        /// <param name="mySourceVertex">The source vertex</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public HyperEdge(
            List<SingleEdge> myContainedSingleEdges,
            Int64 myEdgeTypeID,
            InMemoryVertex mySourceVertex,
            String myComment,
            long myCreationDate,
            long myModificationDate,
            Dictionary<Int64, IComparable> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties)
            : base(myComment, myCreationDate, myModificationDate, myStructuredProperties, myUnstructuredProperties)
        {
            _edgeTypeID = myEdgeTypeID;

            _sourceVertex = mySourceVertex;
            
            _containedSingleEdges = myContainedSingleEdges;
        }

        #endregion

        #region IHyperEdge Members

        public IEnumerable<ISingleEdge> GetEdges(PropertyHyperGraphFilter.SingleEdgeFilter myFilter = null)
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

        public IEnumerable<IVertex> GetTargetVertices(PropertyHyperGraphFilter.TargetVertexFilter myFilter = null)
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
                return (T) _structuredProperties[myPropertyID];
            }
            
            throw new CouldNotFindStructuredEdgePropertyException(_edgeTypeID,
                                                                  myPropertyID);
        }

        public IComparable GetProperty(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return _structuredProperties[myPropertyID];
            }

            throw new CouldNotFindStructuredEdgePropertyException(_edgeTypeID,
                                                                  myPropertyID);
        }

        public bool HasProperty(long myPropertyID)
        {
            return _structuredProperties.ContainsKey(myPropertyID);
        }

        public int GetCountOfProperties()
        {
            return _structuredProperties.Count;
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
            
            throw new CouldNotFindStructuredEdgePropertyException(_edgeTypeID,
                                                                  myPropertyID);
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T) _unstructuredProperties[myPropertyName];
            }
            
            throw new CouldNotFindUnStructuredEdgePropertyException(_edgeTypeID,
                                                                    myPropertyName);
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _unstructuredProperties.ContainsKey(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties()
        {
            return _unstructuredProperties.Count;
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
            
            throw new CouldNotFindUnStructuredEdgePropertyException(_edgeTypeID,
                                                                    myPropertyName);
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