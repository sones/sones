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

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// A certain unstructured vertex property does not exist
    /// </summary>
    public sealed class CouldNotFindUnStructuredVertexPropertyException : AGraphFSException
    {
        #region data

        /// <summary>
        /// The id of the desired property
        /// </summary>
        public readonly String PropertyName;

        /// <summary>
        /// The id of the type of the vertex
        /// </summary>
        public readonly Int64 TypeID;

        /// <summary>
        /// The id of the desired vertex
        /// </summary>
        public readonly Int64 VertexID;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new CouldNotFindUnStructuredVertexPropertyException exception
        /// </summary>
        /// <param name="myTypeID">The vertex type id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myPropertyName">The desired property of the vertex</param>
        public CouldNotFindUnStructuredVertexPropertyException(Int64 myTypeID, Int64 myVertexID, String myPropertyName)
        {
            TypeID = myTypeID;
            VertexID = myVertexID;
            PropertyName = myPropertyName;
        }

        #endregion
                
    }
}