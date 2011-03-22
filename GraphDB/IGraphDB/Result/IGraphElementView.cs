using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Result
{
    /// <summary>
    /// The interface for all graph element views
    /// </summary>
    public interface IGraphElementView
    {
        #region Properties

        /// <summary>
        /// Get a certain property
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns>The property</returns>
        T GetProperty<T>(String myPropertyName);

        /// <summary>
        /// Is there a certain property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns>True or false</returns>
        bool HasProperty(String myPropertyName);

        /// <summary>
        /// Returns the count of properties
        /// </summary>
        /// <returns>The count of properties</returns>
        int GetCountOfProperties();

        /// <summary>
        /// Returns all properties of a graph element
        /// </summary>
        /// <returns>All properties in Key/Value manner</returns>
        IEnumerable<Tuple<String, Object>> GetAllProperties();

        /// <summary>
        /// Gets a certain property as string
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns></returns>
        String GetPropertyAsString(String myPropertyName);

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
        DateTime CreationDate { get; }

        #endregion

        #region Modification date

        /// <summary>
        /// The date the graph element has been modified the last time
        /// </summary>
        DateTime ModificationDate { get; }

        #endregion
    }
}
