using System;
using System.Collections.Generic;
using sones.GraphFS.Definitions;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphFS.Element.Edge
{
    /// <summary>
    /// The single edge defines a 1-1 relation within the property hypergraph
    /// </summary>
    public sealed class SingleEdge : ISingleEdge
    {
        #region Properties

        /// <summary>
        /// Properties
        /// </summary>
        private readonly GraphElementInformation _graphElementInformation;

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
        /// <param name="mySourceVertex">The source vertex</param>
        /// <param name="myTargetVertex">The target vertex</param>
        /// <param name="myGraphElementInformation">The graph element information</param>
        public SingleEdge(
            InMemoryVertex mySourceVertex,
            InMemoryVertex myTargetVertex,
            GraphElementInformation myGraphElementInformation)
        {
            _sourceVertex = mySourceVertex;

            _targetVertex = myTargetVertex;

            _graphElementInformation = myGraphElementInformation;
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

        public IEnumerable<IVertex> GetTargetVertices(Func<IVertex, bool> myFilterFunc = null)
        {
            var targetVertex = GetTargetVertex();

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

            yield break;
        }

        public T GetProperty<T>(long myPropertyID)
        {
            if (HasProperty(myPropertyID))
            {
                return (T)_graphElementInformation.StructuredProperties[myPropertyID];
            }
            
            throw new CouldNotFindStructuredEdgePropertyException(_graphElementInformation.TypeID,
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
            
            throw new CouldNotFindStructuredEdgePropertyException(_graphElementInformation.TypeID,
                                                                  myPropertyID);
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            if (HasUnstructuredProperty(myPropertyName))
            {
                return (T)_graphElementInformation.UnstructuredProperties[myPropertyName];
            }

            throw new CouldNotFindUnStructuredEdgePropertyException(_graphElementInformation.TypeID,
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

            throw new CouldNotFindUnStructuredEdgePropertyException(_graphElementInformation.TypeID,
                                                                    myPropertyName);
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
                && (_graphElementInformation.TypeID == p._graphElementInformation.TypeID);
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