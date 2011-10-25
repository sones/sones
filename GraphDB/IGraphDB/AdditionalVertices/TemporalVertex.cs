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
using System.Text;
using sones.Library.PropertyHyperGraph;
using sones.Library.DataStructures;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.AdditionalVertices
{
    /// <summary>
    /// This class is an extension of the sones IVertex class.
    /// 
    /// This implementation will give the ability to store some additional temporal attributes
    /// on a vertex. This means all these additional attributes won't be saved permanently.
    /// 
    /// To initialize a TemporalVertex, a vertex of the IGraphFS is needed.
    /// </summary>
    public sealed class TemporalVertex: IVertex
    {
        /// <summary>
        /// The temporal properties, which store additional information.
        /// </summary>
        private readonly Dictionary<string, object> _TemporalProperties;

        /// <summary>
        /// The vertex which contains the vertex attributes (f.e. edges. properties aso.).
        /// </summary>
        private IVertex _Vertex;

        #region constructor

        /// <summary>
        /// Constructor, which initializes a new TemporalVertex.
        /// </summary>
        /// <param name="myVertex">
        /// The vertex which contains the vertex attributes (f.e. edges. properties aso.).
        /// </param>
        public TemporalVertex(IVertex myVertex)
        {
            if (myVertex == null)
                throw new InvalidAttributeTypeException("myVertex", "null", "IVertex");

            _Vertex = myVertex;
            _TemporalProperties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructor, which initializes a new TemporalProperties object.
        /// </summary>
        /// <param name="myVertex">
        /// The vertex which contains the vertex attributes (f.e. edges. properties aso.).
        /// </param>
        /// <param name="myVertex">
        /// The temporal attributes which are added to this vertex.
        /// </param>
        public TemporalVertex(IVertex myVertex, Dictionary<string, object> myTemporalProperties)
        {
            if (myVertex == null)
                throw new InvalidAttributeTypeException(
                            "myVertex", "null", "IVertex");

            if (myTemporalProperties == null)
                throw new InvalidAttributeTypeException(
                            "myTemporalProperties", "null", "Dictionary<string, object>");

            _Vertex = myVertex;
            _TemporalProperties = myTemporalProperties;
        }
        
        #endregion

        #region fields

        /// <summary>
        /// The temporal properties of the vertex.
        /// </summary>
        public Dictionary<string, object> TemporalProperties
        {
            get { return _TemporalProperties; }
        }

        /// <summary>
        /// The vertex which is stored inside this hull.
        /// </summary>
        public IVertex Vertex
        {
            get { return _Vertex; }
        }

        #endregion

        #region TempData

        /// <summary>
        /// Adds a temporal property.
        /// </summary>
        /// <param name="myKey">
        /// The key which is associated with the value.
        /// </param>
        /// <param name="myValue">
        /// The value which is associated with the given key.
        /// </param>
        public void AddTemporalProperty(string myKey, object myValue)
        {
            _TemporalProperties.Add(myKey, myValue);
        }

        /// <summary>
        /// Gets the value, which is associated with the given key.
        /// </summary>
        /// <param name="myKey">
        /// The key of the interesting value.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        public object GetTemporalProperty(string myKey)
        {
            return _TemporalProperties[myKey];
        }

        /// <summary>
        /// Gets the casted value, which is associated with the given key.
        /// </summary>
        /// <typeparam name="T">
        /// The type to which the value gets casted.
        /// </typeparam>
        /// <param name="myKey">
        /// The key of the interesting value.
        /// </param>
        /// <returns>
        /// The value, casted to <paramref name="T"/>
        /// </returns>
        public T GetTemporalProperty<T>(string myKey)
        {
            return (T)_TemporalProperties[myKey];
        }

        /// <summary>
        /// This method will return all temporal properties.
        /// </summary>
        /// <returns>
        /// All temporal properties as IEnumerable of KeyValuePair.
        /// </returns>
        public IEnumerable<KeyValuePair<string, object>> GetAllTemporalProperties()
        {
            return _TemporalProperties.AsEnumerable();
        }

        #endregion

        #region IVertex member
        // This vertex is just a hull of another vertex,
        // which contains additional information

        public bool HasIncomingVertices(long myVertexTypeID, long myEdgePropertyID)
        {
            return _Vertex.HasIncomingVertices(myVertexTypeID, myEdgePropertyID);
        }

        public IEnumerable<Tuple<long, long, IEnumerable<IVertex>>> GetAllIncomingVertices(
            PropertyHyperGraphFilter.IncomingVerticesFilter myFilter)
        {
            return _Vertex.GetAllIncomingVertices(myFilter);
        }

        public IEnumerable<IVertex> GetIncomingVertices(long myVertexTypeID, long myEdgePropertyID)
        {
            return _Vertex.GetIncomingVertices(myVertexTypeID, myEdgePropertyID);
        }

        public bool HasOutgoingEdge(long myEdgePropertyID)
        {
            return _Vertex.HasOutgoingEdge(myEdgePropertyID);
        }

        public IEnumerable<Tuple<long, IEdge>> GetAllOutgoingEdges(
            PropertyHyperGraphFilter.OutgoingEdgeFilter myFilter)
        {
            return _Vertex.GetAllOutgoingEdges(myFilter);
        }

        public IEnumerable<Tuple<long, IHyperEdge>> GetAllOutgoingHyperEdges(
            PropertyHyperGraphFilter.OutgoingHyperEdgeFilter myFilter)
        {
            return _Vertex.GetAllOutgoingHyperEdges(myFilter);
        }

        public IEnumerable<Tuple<long, ISingleEdge>> GetAllOutgoingSingleEdges(
            PropertyHyperGraphFilter.OutgoingSingleEdgeFilter myFilter)
        {
            return _Vertex.GetAllOutgoingSingleEdges(myFilter);
        }

        public IEdge GetOutgoingEdge(long myEdgePropertyID)
        {
            return _Vertex.GetOutgoingEdge(myEdgePropertyID);
        }

        public IHyperEdge GetOutgoingHyperEdge(long myEdgePropertyID)
        {
            return _Vertex.GetOutgoingHyperEdge(myEdgePropertyID);
        }

        public ISingleEdge GetOutgoingSingleEdge(long myEdgePropertyID)
        {
            return _Vertex.GetOutgoingSingleEdge(myEdgePropertyID);
        }

        public System.IO.Stream GetBinaryProperty(long myPropertyID)
        {
            return _Vertex.GetBinaryProperty(myPropertyID);
        }

        public IEnumerable<Tuple<long, System.IO.Stream>> GetAllBinaryProperties(
            PropertyHyperGraphFilter.BinaryPropertyFilter myFilter)
        {
            return _Vertex.GetAllBinaryProperties(myFilter);
        }

        public T GetProperty<T>(long myPropertyID)
        {
            return _Vertex.GetProperty<T>(myPropertyID);
        }

        public IComparable GetProperty(long myPropertyID)
        {
            return _Vertex.GetProperty(myPropertyID);
        }

        public bool HasProperty(long myPropertyID)
        {
            return _Vertex.HasProperty(myPropertyID);
        }

        public int GetCountOfProperties()
        {
            return _Vertex.GetCountOfProperties();
        }

        public IEnumerable<Tuple<long, IComparable>> GetAllProperties(
            PropertyHyperGraphFilter.GraphElementStructuredPropertyFilter myFilter)
        {
            return _Vertex.GetAllProperties(myFilter);
        }

        public string GetPropertyAsString(long myPropertyID)
        {
            return _Vertex.GetPropertyAsString(myPropertyID);
        }

        public T GetUnstructuredProperty<T>(string myPropertyName)
        {
            return _Vertex.GetUnstructuredProperty<T>(myPropertyName);
        }

        public bool HasUnstructuredProperty(string myPropertyName)
        {
            return _Vertex.HasUnstructuredProperty(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties()
        {
            return _Vertex.GetCountOfUnstructuredProperties();
        }

        public IEnumerable<Tuple<string, object>> GetAllUnstructuredProperties(
            PropertyHyperGraphFilter.GraphElementUnStructuredPropertyFilter myFilter)
        {
            return _Vertex.GetAllUnstructuredProperties(myFilter);
        }

        public string GetUnstructuredPropertyAsString(string myPropertyName)
        {
            return _Vertex.GetUnstructuredPropertyAsString(myPropertyName);
        }

        public string Comment
        {
            get { return _Vertex.Comment; }
        }

        public long CreationDate
        {
            get { return _Vertex.CreationDate; }
        }

        public long ModificationDate
        {
            get { return _Vertex.ModificationDate; }
        }

        public long VertexTypeID
        {
            get { return _Vertex.VertexTypeID; }
        }

        public long VertexID
        {
            get { return _Vertex.VertexID; }
        }

        public long VertexRevisionID
        {
            get { return _Vertex.VertexRevisionID; }
        }

        public string EditionName
        {
            get { return _Vertex.EditionName; }
        }

        public IVertexStatistics Statistics
        {
            get { return _Vertex.Statistics; }
        }

        public IGraphPartitionInformation PartitionInformation
        {
            get { return _Vertex.PartitionInformation; }
        }

        #endregion
    }
}
