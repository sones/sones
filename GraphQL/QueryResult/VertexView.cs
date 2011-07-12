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
using System.IO;


namespace sones.GraphQL.Result
{
    /// <summary>
    /// This class creates an vertex view.
    /// </summary>
    public class VertexView : IVertexView
    {
        #region Data

        /// <summary>
        /// The list with the result properties.
        /// </summary>
        private readonly IDictionary<String, Object> _propertyList;

        /// <summary>
        /// The list with result edges.
        /// </summary>
        private readonly IDictionary<String, IEdgeView> _edgeList;

        #endregion

        #region Constructor

        /// <summary>
        /// The vertex view constructor.
        /// </summary>
        /// <param name="myPropertyList">The property list.</param>
        /// <param name="myEdges">The edge list.</param>
        public VertexView(IDictionary<String, Object> myPropertyList, IDictionary<String, IEdgeView> myEdges)
        {
            _propertyList = myPropertyList;
            _edgeList = myEdges;
        }

        #endregion

        #region IVertexView

        public bool HasEdge(string myEdgePropertyName)
        {
            if (_edgeList == null)
                return false;
            else
                return _edgeList.ContainsKey(myEdgePropertyName);
        }

        public IEnumerable<Tuple<string, IEdgeView>> GetAllEdges()
        {
            if (_edgeList == null)
                return new List<Tuple<string, IEdgeView>>();
            else
                return _edgeList.Select(item => new Tuple<String, IEdgeView>(item.Key, item.Value));
        }

        public IEnumerable<Tuple<string, IHyperEdgeView>> GetAllHyperEdges()
        {
            if (_edgeList == null)
                return new List<Tuple<string, IHyperEdgeView>>();
            else
                return _edgeList.Where(item => item.Value is IHyperEdgeView).Select(item => new Tuple<String, IHyperEdgeView>(item.Key, (IHyperEdgeView)item.Value));
        }

        public IEnumerable<Tuple<string, ISingleEdgeView>> GetAllSingleEdges()
        {
            if (_edgeList == null)
                return new List<Tuple<string, ISingleEdgeView>>();
            else
                return _edgeList.Where(item => item.Value is ISingleEdgeView).Select(item => new Tuple<String, ISingleEdgeView>(item.Key, (ISingleEdgeView)item.Value));
        }

        public IEdgeView GetEdge(string myEdgePropertyName)
        {
            if (_edgeList == null)
            {
                return null;
            }
            else
            {
                IEdgeView outValue;
                if (_edgeList.TryGetValue(myEdgePropertyName, out outValue))
                {
                    return outValue;
                }
                else
                {
                    return null;
                }
            }
        }

        public IHyperEdgeView GetHyperEdge(string myEdgePropertyName)
        {
            if (_edgeList == null)
            {
                return null;
            }
            else
                return
                    (IHyperEdgeView)_edgeList.Where(
                        item => item.Key == myEdgePropertyName && item.Value is IHyperEdgeView).
                                         FirstOrDefault().Value;
        }

        public ISingleEdgeView GetSingleEdge(string myEdgePropertyName)
        {
            if (_edgeList == null)
                return null;
            else
                return (ISingleEdgeView)_edgeList.Where(item => item.Key == myEdgePropertyName && item.Value is ISingleEdgeView).FirstOrDefault().Value;
        }

        public Stream GetBinaryProperty(string myPropertyName)
        {
            return GetProperty<Stream>(myPropertyName);
        }

        public IEnumerable<Tuple<string, Stream>> GetAllBinaryProperties()
        {
            if (_propertyList == null)
                return new List<Tuple<string, Stream>>();
            else
                return _propertyList.Where(item => item.Value is Stream).Select(item => new Tuple<String, Stream>(item.Key, (Stream)item.Value));
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
            }
            return default(T);
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
                return _propertyList.Select(item => new Tuple<string, object>(item.Key, item.Value));
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

        public IEnumerable<IVertexView> GetAllNeighbours(string myEdgePropertyName)
        {
            if (_edgeList == null)
                return new List<IVertexView>();
            else
            {
                IEdgeView outValue;
                if (_edgeList.TryGetValue(myEdgePropertyName, out outValue))
                {
                    return outValue.GetTargetVertices();
                }
                else
                {
                    return new List<IVertexView>();
                }
            }
        }
        #endregion

    }
}
