using System;
using System.Collections.Generic;

namespace sones.GraphFS.Definitions
{
    public struct GraphElementInformation
    {
        #region data

        /// <summary>
        /// A comment for the vertex
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
        /// The structured properties
        /// </summary>
        public readonly Dictionary<Int64, Object> StructuredProperties;

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        public Int64 TypeID;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        public readonly Dictionary<String, Object> UnstructuredProperties;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new graph element information
        /// </summary>
        /// <param name="myTypeID">The type id of this graph element</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public GraphElementInformation(
            Int64 myTypeID,
            String myComment,
            DateTime myCreationDate,
            DateTime myModificationDate,
            Dictionary<Int64, Object> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties)
        {
            TypeID = myTypeID;
            Comment = myComment;
            CreationDate = myCreationDate;
            ModificationDate = myModificationDate;
            StructuredProperties = myStructuredProperties;
            UnstructuredProperties = myUnstructuredProperties;
        }

        #endregion

        #region helper

        #region GetAllProperties_protected

        /// <summary>
        /// Returns all properties of this graph element
        /// </summary>
        /// <param name="myFilterFunc">An optional filter function</param>
        /// <returns>An enumerable of propertyID/propertyValue</returns>
        public IEnumerable<Tuple<long, object>> GetAllPropertiesProtected(Func<long, object, bool> myFilterFunc)
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
        public IEnumerable<Tuple<string, object>> GetAllUnstructuredPropertiesProtected(
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