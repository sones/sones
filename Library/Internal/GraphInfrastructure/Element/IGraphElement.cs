using System;
using System.Collections.Generic;

namespace sones.GraphInfrastructure.Element
{
    /// <summary>
    /// The interface for graph elements like vertices or edges
    /// </summary>
    public interface IGraphElement
    {
        #region Properties

        /// <summary>
        /// Returns the property of a graph element.
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyID">The ID of the interesing property</param>
        /// <returns>A Property</returns>
        T GetProperty<T>(PropertyID myPropertyID);

        /// <summary>
        /// Checks whether the graph element is in possession of a certain property
        /// </summary>
        /// <param name="myPropertyID">The ID of the property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        bool HasProperty(PropertyID myPropertyID);

        /// <summary>
        /// Returns the count of the vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        UInt64 GetCountOfProperties();

        /// <summary>
        /// Returns all properties
        /// </summary>
        /// <param name="myFilterFunc">A function to filter properties</param>
        /// <returns>An IEnumerable of Property/Value</returns>
        IEnumerable<KeyValuePair<PropertyID, Object>> GetAllProperties(Func<PropertyID, Object, bool> myFilterFunc = null);

        #endregion

        #region Unstructured data/properties

        /// <summary>
        /// Gets unstructured data of the graph element
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyName">The name of the interesting unstructured property</param>
        /// <returns>The value of an unstructured property</returns>
        T GetUnstructuredProperty<T>(string myPropertyName);

        /// <summary>
        /// Checks whether the graph element is in possession of a certain unstructered property
        /// </summary>
        /// <param name="myPropertyName">The name of the unstructured property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        bool HasUnstructuredProperty(String myPropertyName);

        /// <summary>
        /// Returns the count of the unstructured vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        UInt64 GetCountOfUnstructuredProperties();

        /// <summary>
        /// Returns all unstructured properties
        /// </summary>
        /// <param name="myFilterFunc">A function to filter properties</param>
        /// <returns>An IEnumerable of NameOfProperty/Value</returns>
        IEnumerable<KeyValuePair<String, Object>> GetAllUnstructuredProperties(Func<String, Object, bool> myFilterFunc = null);

        #endregion

        #region Comment

        /// <summary>
        /// Gets the comment of this graph element
        /// </summary>
        /// <returns>A String</returns>
        String GetComment();

        #endregion

        #region Type

        /// <summary>
        /// Gets the type of this graph element
        /// </summary>
        /// <returns></returns>
        String GetType();

        #endregion
    }
}
