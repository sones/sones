using System;
using System.Collections.Generic;
using sones.GraphFS.Definitions;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The in memory representation of graph element properties
    /// </summary>
    public struct InMemoryGraphElementInformation
    {
        #region data

        /// <summary>
        /// The comment of the vertex
        /// </summary>
        public readonly string Comment;

        /// <summary>
        /// The creation date of the vertex
        /// </summary>
        public readonly DateTime CreationDate;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        public readonly DateTime ModificationDate;

        /// <summary>
        /// The structured properties of the vertex
        /// </summary>
        public readonly Dictionary<Int64, Object> StructuredProperties;

        /// <summary>
        /// The vertex type id of the vertex
        /// </summary>
        public readonly Int64 TypeID;

        /// <summary>
        /// The unstructured properties of the vertex
        /// </summary>
        public readonly Dictionary<String, Object> UnstructuredProperties;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new in memory representation of the graph element properties
        /// </summary>
        /// <param name="myGraphElementinformation">The property definition for this graph element</param>
        public InMemoryGraphElementInformation(
            GraphElementInformation myGraphElementinformation)
        {
            TypeID = myGraphElementinformation.TypeID;
            Comment = myGraphElementinformation.Comment;
            CreationDate = myGraphElementinformation.CreationDate;
            ModificationDate = myGraphElementinformation.ModificationDate;
            StructuredProperties = myGraphElementinformation.StructuredProperties;
            UnstructuredProperties = myGraphElementinformation.UnstructuredProperties;
        }

        #endregion

        #region helper

        #region GetAllProperties_protected

        /// <summary>
        /// Returns all properties of this graph element
        /// </summary>
        /// <param name="myFilterFunc">An optional filter function</param>
        /// <returns>An enumerable of propertyID/propertyValue</returns>
        internal IEnumerable<Tuple<long, object>> GetAllPropertiesProtected(Func<long, object, bool> myFilterFunc)
        {
            if (StructuredProperties != null)
            {
                foreach (var aProperty in StructuredProperties)
                {
                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aProperty.Key, aProperty.Value))
                        {
                            yield return new Tuple<long, object>(aProperty.Key, aProperty.Value);
                        }
                    }
                    else
                    {
                        yield return new Tuple<long, object>(aProperty.Key, aProperty.Value);
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
        /// <param name="myFilterFunc">An optional filter function</param>
        /// <returns>An enumerable of propertyName/PropertyValue</returns>
        internal IEnumerable<Tuple<string, object>> GetAllUnstructuredPropertiesProtected(
            Func<string, object, bool> myFilterFunc)
        {
            if (UnstructuredProperties != null)
            {
                foreach (var aUnstructuredProperty in UnstructuredProperties)
                {
                    if (myFilterFunc != null)
                    {
                        if (myFilterFunc(aUnstructuredProperty.Key, aUnstructuredProperty.Value))
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