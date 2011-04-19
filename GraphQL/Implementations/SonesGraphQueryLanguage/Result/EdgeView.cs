using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// This class creates an edge view.
    /// </summary>
    public class EdgeView : IEdgeView
    {
        #region Data

        /// <summary>
        /// The list of properties of the edge.
        /// </summary>
        private readonly IDictionary<String, Object>        _propertyList;

        /// <summary>
        /// The list of target vertices of the edge.
        /// </summary>
        private readonly IEnumerable<IVertexView>           _targetVertices;

        /// <summary>
        /// The source vertex of the edge.
        /// </summary>
        private readonly IVertexView                        _sourceVertex;

        #endregion

        #region Constructor

        /// <summary>
        /// The edge view constructor.
        /// </summary>
        /// <param name="myProperties">The properties of the edge.</param>
        /// <param name="myTargetVertices">The target vertices of the edge.</param>
        /// <param name="mySourceVertex">The source vertex of the edge.</param>
        public EdgeView(IDictionary<String, Object> myProperties, IEnumerable<IVertexView> myTargetVertices, IVertexView mySourceVertex)
        {
            _propertyList       = myProperties;
            _targetVertices     = myTargetVertices;
            _sourceVertex       = mySourceVertex;
        }

        #endregion

        #region IEdgeView

        public IVertexView GetSourceVertex()
        {
            return _sourceVertex;
        }

        public IEnumerable<IVertexView> GetTargetVertices()
        {
            return _targetVertices;
        }

        public T GetProperty<T>(string myPropertyName)
        {
            Object outValue;

            if (_propertyList.TryGetValue(myPropertyName, out outValue))
            {
                return (T) outValue;
            }
            else
            {
                return default(T);
            }
        }

        public bool HasProperty(string myPropertyName)
        {
            return _propertyList.ContainsKey(myPropertyName);
        }

        public int GetCountOfProperties()
        {
            return _propertyList.Count;
        }

        public IEnumerable<Tuple<string, object>> GetAllProperties()
        {
            return _propertyList.Select(item => new Tuple<String, Object>(item.Key, item.Value));
        }

        public string GetPropertyAsString(string myPropertyName)
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

        #endregion
    }
}
