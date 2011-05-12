/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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

        private readonly object _lockobject = new object();

        /// <summary>
        /// The edge type id
        /// </summary>
        private Int64 _edgeTypeID;

        /// <summary>
        /// The source vertex
        /// </summary>
        public InMemoryVertex SourceVertex;

        /// <summary>
        /// The target vertex
        /// </summary>
        public InMemoryVertex TargetVertex;

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
            IDictionary<Int64, IComparable> myStructuredProperties,
            IDictionary<String, Object> myUnstructuredProperties)
            : base(myComment, myCreationDate, myModificationDate, myStructuredProperties, myUnstructuredProperties)
        {
            _edgeTypeID = myEdgeTypeID;

            SourceVertex = mySourceVertex;

            TargetVertex = myTargetVertex;
        }

        #endregion

        #region ISingleEdge Members

        public IVertex GetTargetVertex()
        {
            return TargetVertex;
        }

        public IVertex GetSourceVertex()
        {
            return SourceVertex;
        }

        public IEnumerable<IVertex> GetTargetVertices(PropertyHyperGraphFilter.TargetVertexFilter myFilter = null)
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

        #region Public Update Methods
        

        public void UpdateComment(String myComment)
        {
            lock (_lockobject)
            {
                _comment = myComment;
            }
        }

        public void UpdateEdgeType(Int64 myEdgeType)
        {
            _edgeTypeID = myEdgeType;
        }

        public void UpdateStructuredProperties(IDictionary<long, IComparable> myUpdatedProperties, IEnumerable<long> myDeletedProperties)
        {
            lock (_lockobject)
            {
                if (myDeletedProperties != null)
                {
                    foreach (var item in myDeletedProperties)
                    {
                        _structuredProperties.Remove(item);
                    }
                }

                if (myUpdatedProperties != null)
                {
                    foreach (var item in _structuredProperties)
                    {
                        if (_structuredProperties.ContainsKey(item.Key))
                        {
                            _structuredProperties[item.Key] = item.Value;
                        }
                        else
                        {
                            _structuredProperties.Add(item.Key, item.Value);
                        }
                    }
                }
            }
        }

        public void UpdateUnStructuredProperties(IDictionary<String, Object> myUpdatedProperties, IEnumerable<String> myDeletedProperties)
        {
            lock (_lockobject)
            {
                if (myDeletedProperties != null)
                {
                    foreach (var item in myDeletedProperties)
                    {
                        _unstructuredProperties.Remove(item);
                    }
                }

                if (myUpdatedProperties != null)
                {
                    foreach (var item in myUpdatedProperties)
                    {
                        if (_unstructuredProperties.ContainsKey(item.Key))
                        {
                            _unstructuredProperties[item.Key] = item.Value;
                        }
                        else
                        {
                            _unstructuredProperties.Add(item.Key, item.Value);
                        }
                    }
                }
            }
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

            return Equals(SourceVertex, p.SourceVertex)
                && Equals(TargetVertex, p.TargetVertex) 
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
            return SourceVertex.GetHashCode() ^ TargetVertex.GetHashCode();
        }

        #endregion
    }
}