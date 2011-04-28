using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// This creates an graph element view.
    /// </summary>
    public class GraphElementView : IGraphElementView
    {
        #region Data
        /// <summary>
        /// The list of graph element view.
        /// </summary>
        private readonly IDictionary<String, Object> _propertyList;
        #endregion

        #region Constructor
        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="myPropertyList">The list of properties.</param>
        public GraphElementView(IDictionary<String, Object> myPropertyList)
        {
            _propertyList = myPropertyList;
        }
        #endregion

        public T GetProperty<T>(string myPropertyName)
        {
            if (_propertyList == null)
                return default(T);
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
    }
}
