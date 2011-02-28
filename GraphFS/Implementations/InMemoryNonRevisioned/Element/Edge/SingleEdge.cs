using System;
using System.Collections.Generic;
using sones.PropertyHyperGraph;
using sones.GraphFS.ErrorHandling;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The single edge defines a 1-1 relation within the property hypergraph
    /// </summary>
    public sealed class SingleEdge : ISingleEdge
    {
        #region Properties

        /// <summary>
        /// The location of the source vertex
        /// </summary>
        private readonly VertexLocation _sourceLocation;

        /// <summary>
        /// The location of the target vertex
        /// </summary>
        private readonly VertexLocation _targetLocation;

        /// <summary>
        /// THe function to resolve the source or target vertex
        /// </summary>
        private readonly Func<ulong, ulong, IVertex> _getVertexFunc;

        /// <summary>
        /// Properties
        /// </summary>
        private readonly InMemoryGraphElementInformation _inMemoryGraphElementInformation;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new single edge
        /// </summary>
        /// <param name="myEdgeDefinition">The definition of this edge</param>
        /// <param name="myGetVertex">The function to resolve either the source or the target vertex</param>
        public SingleEdge(
            SingleEdgeAddDefinition myEdgeDefinition,
            Func<ulong, ulong, IVertex> myGetVertex)
        {
            _getVertexFunc = myGetVertex;

            _sourceLocation = new VertexLocation(myEdgeDefinition.SourceVertexInformation.VertexTypeID, myEdgeDefinition.SourceVertexInformation.VertexID);

            _targetLocation = new VertexLocation(myEdgeDefinition.TargetVertexInformation.VertexTypeID, myEdgeDefinition.TargetVertexInformation.VertexID);

            _inMemoryGraphElementInformation = new InMemoryGraphElementInformation(myEdgeDefinition.GraphElementInformation);
        }

        #endregion

        #region ISingleEdge Members

        public IVertex GetTargetVertex()
        {
            return _getVertexFunc(_targetLocation.VertexID, _targetLocation.VertexTypeID);
        }

        #endregion

        #region IEdge Members

        public IVertex GetSourceVertex()
        {
            return _getVertexFunc(_sourceLocation.VertexID, _sourceLocation.VertexTypeID);
        }

        public IEnumerable<IVertex> GetTargetVertices(Func<IVertex, bool> myFilterFunc = null)
        {
            var targetVertex = GetTargetVertex();

            if (myFilterFunc(targetVertex))
            {
                yield return targetVertex;
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

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            SingleEdge p = obj as SingleEdge;
            if ((System.Object)p == null)
            {
                return false;
            }

            return Equals(p);

        }

        public Boolean Equals(SingleEdge p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this._sourceLocation == p._sourceLocation)
                && (this._targetLocation == p._targetLocation)
                && (this._inMemoryGraphElementInformation.TypeID == p._inMemoryGraphElementInformation.TypeID);
        }

        public static Boolean operator ==(SingleEdge a, SingleEdge b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
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
            return _sourceLocation.GetHashCode() ^ _targetLocation.GetHashCode();
        }

        #endregion
    }
}