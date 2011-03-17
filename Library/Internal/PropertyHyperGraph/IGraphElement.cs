using System;
using System.Collections.Generic;

namespace sones.Library.PropertyHyperGraph
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
        T GetProperty<T>(Int64 myPropertyID);

        /// <summary>
        /// Checks whether the graph element is in possession of a certain property
        /// </summary>
        /// <param name="myPropertyID">The ID of the property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        bool HasProperty(Int64 myPropertyID);

        /// <summary>
        /// Returns the count of the vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        int GetCountOfProperties();

        /// <summary>
        /// Returns all properties
        /// </summary>
        /// <param name="myFilter">A function to filter properties</param>
        /// <returns>An IEnumerable of Property/Value</returns>
        IEnumerable<Tuple<Int64, Object>> GetAllProperties(Filter.GraphElementStructuredPropertyFilter myFilter = null);

        /// <summary>
        /// Returns a property as string
        /// </summary>
        /// <param name="myPropertyID">The id of the property</param>
        /// <returns>The string representation of the property</returns>
        String GetPropertyAsString(Int64 myPropertyID);

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
        int GetCountOfUnstructuredProperties();

        /// <summary>
        /// Returns all unstructured properties
        /// </summary>
        /// <param name="myFilter">A function to filter properties</param>
        /// <returns>An IEnumerable of NameOfProperty/Value</returns>
        IEnumerable<Tuple<String, Object>> GetAllUnstructuredProperties(
            Filter.GraphElementUnStructuredPropertyFilter myFilter = null);

        /// <summary>
        /// Returns an unstructured property as string
        /// </summary>
        /// <param name="myPropertyName">The name of the unstructured property</param>
        /// <returns>The string representation of the property</returns>
        String GetUnstructuredPropertyAsString(String myPropertyName);

        #endregion

        #region Comment

        /// <summary>
        /// Gets the comment of this graph element
        /// </summary>
        String Comment { get; }

        #endregion

        #region Creation date

        /// <summary>
        /// The date the graph element has been created
        /// </summary>
        long CreationDate { get; }

        #endregion

        #region Modification date

        /// <summary>
        /// The date the graph element has been modified the last time
        /// </summary>
        long ModificationDate { get; }

        #endregion

        #region TypeID

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        Int64 TypeID { get; }

        #endregion
    }
}