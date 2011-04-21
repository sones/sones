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
        private readonly IDictionary<String, Object>        _propertyList;

        /// <summary>
        /// The list of target vertex of the edge.
        /// </summary>
        private readonly IVertexView                        _targetVertex;

        #endregion

        #region Constructor

        /// <summary>
        /// The single edge view constructor.
        /// </summary>
        /// <param name="myProperties">The properties of the edge.</param>
        /// <param name="myTargetVertex">The target vertex of the edge.</param>
        public SingleEdgeView(IDictionary<String, Object> myProperties, IVertexView myTargetVertex)
        {
            _propertyList       = myProperties;
            _targetVertex       = myTargetVertex;
        }

        #endregion

        #region ISingleEdgeView
        
        public IVertexView GetTargetVertex()
        {
            return _targetVertex;
        }

        public IEnumerable<IVertexView> GetTargetVertices()
        {
            throw new NotImplementedException();
        }

        public T GetProperty<T>(string myPropertyName)
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
