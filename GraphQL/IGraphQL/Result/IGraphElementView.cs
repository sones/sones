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

namespace sones.GraphQL.Result
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
    }
}
