using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// This class creates an single edge view.
    /// </summary>
    public class SingleEdgeView : ISingleEdgeView
    {
        #region Data

        /// <summary>
        /// The list of properties of the edge.
        /// </summary>
        private readonly IDictionary<String, Object> _propertyList;

        /// <summary>
        /// The list of target vertex of the edge.
        /// </summary>
        private readonly IVertexView _targetVertex;

        #endregion

        #region Constructor

        /// <summary>
        /// The single edge view constructor.
        /// </summary>
        /// <param name="myProperties">The properties of the edge.</param>
        /// <param name="myTargetVertex">The target vertex of the edge.</param>
        public SingleEdgeView(IDictionary<String, Object> myProperties, IVertexView myTargetVertex)
        {
            _propertyList = myProperties;
            _targetVertex = myTargetVertex;
        }

        #endregion

        #region ISingleEdgeView

        public IVertexView GetTargetVertex()
        {
            return _targetVertex;
        }

        public IEnumerable<IVertexView> GetTargetVertices()
        {
            if (_targetVertex == null)
                return new List<IVertexView>();
            else
                return new List<IVertexView>() { _targetVertex };
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
