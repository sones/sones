using System;
using System.Collections.Generic;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphFS.Element.Edge
{
    /// <summary>
    /// The single edge defines a 1-1 relation within the property hypergraph
    /// </summary>
    public sealed class SingleEdge : AGraphElement, ISingleEdge
    {
        #region Properties

        /// <summary>
        /// The edge type id
        /// </summary>
        private readonly Int64 _edgeTypeID;

        /// <summary>
        /// The source vertex
        /// </summary>
        private readonly InMemoryVertex _sourceVertex;

        /// <summary>
        /// The target vertex
        /// </summary>
        private readonly InMemoryVertex _targetVertex;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new single edge
        /// </summary>
        /// <param name="myEdgeTypeID">The edge property id</param>
        /// <param name="mySourceVertex">The source vertex</param>
        /// <param name="myTargetVertex">The target vertex</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public SingleEdge(
            Int64 myEdgeTypeID,
            InMemoryVertex mySourceVertex,
            InMemoryVertex myTargetVertex,
            String myComment,
            long myCreationDate,
            long myModificationDate,
            Dictionary<Int64, Object> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties)
            : base(myComment, myCreationDate, myModificationDate, myStructuredProperties, myUnstructuredProperties)
        {
            _edgeTypeID = myEdgeTypeID;

            _sourceVertex = mySourceVertex;

            _targetVertex = myTargetVertex;
        }

        #endregion

        #region ISingleEdge Members

        public IVertex GetTargetVertex()
        {
            return _targetVertex;
        }

        public IVertex GetSourceVertex()
        {
            return _sourceVertex;
        }

        public IEnumerable<IVertex> GetTargetVertices(Filter.TargetVertexFilter myFilter = null)
        {
            var targetVertex = GetTargetVertex();

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

            yield break;
        }

        public T GetProperty<T>(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return (T)_structuredProperties[myPropertyID];
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

        public IEnumerable<Tuple<long, object>> GetAllProperties(Filter.GraphElementStructuredPropertyFilter myFilter = null)
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
                return (T)_unstructuredProperties[myPropertyName];
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
            Filter.GraphElementUnStructuredPropertyFilter myFilter = null)
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

        #region Equals Overrides

        public override Boolean Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            var p = obj as SingleEdge;

            return (Object)p != null && Equals(p);
        }

        public Boolean Equals(SingleEdge p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return Equals(_sourceVertex, p._sourceVertex)
                && Equals(_targetVertex, p._targetVertex) 
                && (_edgeTypeID == p._edgeTypeID);
        }

        public static Boolean operator ==(SingleEdge a, SingleEdge b)
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

        public static Boolean operator !=(SingleEdge a, SingleEdge b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return _sourceVertex.GetHashCode() ^ _targetVertex.GetHashCode();
        }

        #endregion
    }
}