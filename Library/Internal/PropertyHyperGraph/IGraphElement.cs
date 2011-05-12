/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
        /// Returns the property of a graph element.
        /// </summary>
        /// <param name="myPropertyID">The ID of the interesing property</param>
        /// <returns>A Property</returns>
        IComparable GetProperty(Int64 myPropertyID);

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
        IEnumerable<Tuple<Int64, IComparable>> GetAllProperties(PropertyHyperGraphFilter.GraphElementStructuredPropertyFilter myFilter = null);

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
            PropertyHyperGraphFilter.GraphElementUnStructuredPropertyFilter myFilter = null);

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
    }
}