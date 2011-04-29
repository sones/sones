using System;
using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The abstract graph element
    /// </summary>
    public abstract class AGraphElement
    {
        #region data

        /// <summary>
        /// A comment for the vertex
        /// </summary>
        protected string _comment;

        /// <summary>
        /// The creation date of the vertex
        /// </summary>
        protected long _creationDate;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        protected long _modificationDate;

        /// <summary>
        /// The structured properties
        /// </summary>
        protected IDictionary<Int64, IComparable> _structuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        protected IDictionary<String, Object> _unstructuredProperties;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new abstract graph element
        /// </summary>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        protected AGraphElement(
            String myComment,
            long myCreationDate,
            long myModificationDate,
            IDictionary<Int64, IComparable> myStructuredProperties,
            IDictionary<String, Object> myUnstructuredProperties)
        {
            _comment = myComment;
            _creationDate = myCreationDate;
            _modificationDate = myModificationDate;
            _structuredProperties = myStructuredProperties;
            _unstructuredProperties = myUnstructuredProperties;
        }

        protected AGraphElement()
        {
        }

        #endregion

        #region helper

        #region GetAllProperties_protected

        /// <summary>
        /// Returns all properties of this graph element
        /// </summary>
        /// <param name="myFilter">An optional filter function</param>
        /// <returns>An enumerable of propertyID/propertyValue</returns>
        protected IEnumerable<Tuple<long, IComparable>> GetAllPropertiesProtected(PropertyHyperGraphFilter.GraphElementStructuredPropertyFilter myFilter = null)
        {
            if (_structuredProperties != null)
            {
                foreach (var aProperty in _structuredProperties)
                {
                    if (myFilter != null)
                    {
                        if (myFilter(aProperty.Key, aProperty.Value))
                        {
                            yield return new Tuple<long, IComparable>(aProperty.Key, aProperty.Value);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, IComparable>(aProperty.Key, aProperty.Value);
                    }
                }
            }
            yield break;
        }

        #endregion

        #region GetAllUnstructuredProperties_protected

        /// <summary>
        /// Returns all unstructured properties of this graph element
        /// </summary>
        /// <param name="myFilter">An optional filter function</param>
        /// <returns>An enumerable of propertyName/PropertyValue</returns>
        protected IEnumerable<Tuple<string, object>> GetAllUnstructuredPropertiesProtected(
            PropertyHyperGraphFilter.GraphElementUnStructuredPropertyFilter myFilter = null)
        {
            if (_unstructuredProperties != null)
            {
                foreach (var aUnstructuredProperty in _unstructuredProperties)
                {
                    if (myFilter != null)
                    {
                        if (myFilter(aUnstructuredProperty.Key, aUnstructuredProperty.Value))
                        {
                            yield return
                                new Tuple<String, object>(aUnstructuredProperty.Key, aUnstructuredProperty.Value);
                        }
                    }
                    else
                    {
                        yield return new Tuple<String, object>(aUnstructuredProperty.Key, aUnstructuredProperty.Value);
                    }
                }
            }
            yield break;
        }

        #endregion

        #endregion
    }
}
