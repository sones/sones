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

        private readonly object _lockobject = new object();

        /// <summary>
        /// The edge type id
        /// </summary>
        private Int64 _edgeTypeID;

        /// <summary>
        /// The single edges that are contained within this hyper edge
        /// </summary>
        public HashSet<SingleEdge> ContainedSingleEdges;

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
            HashSet<SingleEdge> myContainedSingleEdges,
            Int64 myEdgeTypeID,
            InMemoryVertex mySourceVertex,
            String myComment,
            long myCreationDate,
            long myModificationDate,
            IDictionary<Int64, IComparable> myStructuredProperties,
            IDictionary<String, Object> myUnstructuredProperties)
            : base(myComment, myCreationDate, myModificationDate, myStructuredProperties, myUnstructuredProperties)
        {
            _edgeTypeID = myEdgeTypeID;

            _sourceVertex = mySourceVertex;
            
            ContainedSingleEdges = myContainedSingleEdges;
        }

        #endregion

        #region public update methods

        public void UpdateEdgeType(Int64 myEdgeType)
        {
            lock (_lockobject)
            {
                _edgeTypeID = myEdgeType;
            }
        }

        public void UpdateComment(String myComment)
        {
            lock (_lockobject)
            {
                if (myComment != null)
                {
                    _comment = myComment;
                }
            }
        }

        public void UpdateStructuredProperties(IDictionary<long, IComparable> myUpdatedProperties, IEnumerable<long> myDeletedProperties)
        {
            lock (_lockobject)
            {
                if (myDeletedProperties != null)
                {
                    if (_structuredProperties != null)
                    {
                        foreach (var item in myDeletedProperties)
                        {                            
                            _structuredProperties.Remove(item);
                        }
                    }
                }

                if (myUpdatedProperties != null)
                {
                    if (_structuredProperties != null)
                    {
                        foreach (var item in myUpdatedProperties)
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
                    else
                    {
                        _structuredProperties = new Dictionary<Int64, IComparable>();

                        foreach (var item in myUpdatedProperties)
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
                    if (_unstructuredProperties != null)
                    {
                        foreach (var item in myDeletedProperties)
                        {
                            _unstructuredProperties.Remove(item);
                        }
                    }
                }

                if (myUpdatedProperties != null)
                {
                    if (_unstructuredProperties != null)
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
                    else
                    {
                        _unstructuredProperties = new Dictionary<string, Object>();

                        foreach (var item in myUpdatedProperties)
                        {
                            _unstructuredProperties.Add(item.Key, item.Value);
                        }
                    }
                }
            }
        }
        

        #endregion

        #region IHyperEdge Members

        public IEnumerable<ISingleEdge> GetAllEdges(PropertyHyperGraphFilter.SingleEdgeFilter myFilter = null)
        {
            if (myFilter != null)
            {
                foreach (var aSingleEdge in ContainedSingleEdges)
                {
                    if (myFilter(aSingleEdge))
                    {
                        yield return aSingleEdge;
                    }
                }
            }
            else
            {
                foreach (var aSingleEdge in ContainedSingleEdges)
                {
                    yield return aSingleEdge;
                }
            }

            yield break;
        }

        public TResult InvokeHyperEdgeFunc<TResult>(Func<IEnumerable<ISingleEdge>, TResult> myHyperEdgeFunction)
        {
            return myHyperEdgeFunction(ContainedSingleEdges);
        }

        public IVertex GetSourceVertex()
        {
            return _sourceVertex;
        }

        public IEnumerable<IVertex> GetTargetVertices(PropertyHyperGraphFilter.TargetVertexFilter myFilter = null)
        {
            foreach (var targetVertex in
                ContainedSingleEdges.Select(aSingleEdge =>
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
            if (_structuredProperties != null)
            {
                return _structuredProperties.ContainsKey(myPropertyID);
            }
            else
            {
                return false;
            }            
        }

        public int GetCountOfProperties()
        {
            if (_structuredProperties != null)
            {
                return _structuredProperties.Count;
            }
            else
            {
                return 0;
            }            
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
            if (_unstructuredProperties != null)
            {
                return _unstructuredProperties.ContainsKey(myPropertyName);
            }
            else
            {
                return false;    
            }
        }

        public int GetCountOfUnstructuredProperties()
        {
            if (_unstructuredProperties != null)
            {
                return _unstructuredProperties.Count;
            }
            else
            {
                return 0;
            }
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