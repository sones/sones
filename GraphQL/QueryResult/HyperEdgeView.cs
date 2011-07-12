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

namespace sones.GraphQL.Result
{
    public class HyperEdgeView : IHyperEdgeView
    {
        #region Data
        /// <summary>
        /// The list of properties of the edge.
        /// </summary>
        private readonly IDictionary<String, Object> _propertyList;

        /// <summary>
        /// The vertices, which belongs to the hyperedge.
        /// </summary>
        private readonly IEnumerable<ISingleEdgeView> _edges;
        #endregion

        #region Constructor
        /// <summary>
        /// The edge view constructor.
        /// </summary>
        /// <param name="myProperties">The properties of the edge.</param>
        /// <param name="myEdges">The vertices, which belongs to the hyperedge.</param>
        public HyperEdgeView(IDictionary<String, Object> myProperties, IEnumerable<ISingleEdgeView> myEdges)
        {
            _propertyList = myProperties;
            _edges = myEdges;
        }
        #endregion

        #region IHyperEdgeView
        /// <summary>
        /// Get all Edges
        /// </summary>
        /// <returns>an IEnumerable of ISingleEdgeView (or empty)</returns>
        public IEnumerable<ISingleEdgeView> GetAllEdges()
        {
            if (_edges == null)
                return new List<ISingleEdgeView>();
            else
                return _edges;
        }

        /// <summary>
        /// Get all Target Vertices
        /// </summary>
        /// <returns>an IEnumerable of IVertexView (or empty)</returns>
        public IEnumerable<IVertexView> GetTargetVertices()
        {
            if (_edges == null)
                return new List<IVertexView>();
            else
                return GetAllEdges().Select(aEdge => aEdge.GetTargetVertex());
        }

        public T GetProperty<T>(string myPropertyName)
        {
            if (_propertyList == null)
                return default(T);
            else
            {
                Object outValue;
                if (_propertyList.TryGetValue(myPropertyName, out outValue))
                {
                    return (T)outValue;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public bool HasProperty(string myPropertyName)
        {
            if (_propertyList == null)
                return false;
            else
                return _propertyList.ContainsKey(myPropertyName);
        }

        public int GetCountOfProperties()
        {
            if (_propertyList == null)
                return 0;
            else
                return _propertyList.Count;
        }

        public IEnumerable<Tuple<string, object>> GetAllProperties()
        {
            if (_propertyList == null)
                return new List<Tuple<string, object>>();
            else
                return _propertyList.Select(item => new Tuple<String, Object>(item.Key, item.Value));
        }

        public string GetPropertyAsString(string myPropertyName)
        {
            if (_propertyList == null)
                return String.Empty;
            else
            {
                Object outValue;
                if (_propertyList.TryGetValue(myPropertyName, out outValue))
                {
                    return outValue.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        #endregion
    }
}
