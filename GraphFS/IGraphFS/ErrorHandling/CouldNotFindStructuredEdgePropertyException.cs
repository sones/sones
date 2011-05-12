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
    /// A certain structured edge property does not exist
    /// </summary>
    public sealed class CouldNotFindStructuredEdgePropertyException : AGraphFSException
    {
        #region data

        /// <summary>
        /// The id of the desired property
        /// </summary>
        public readonly Int64 PropertyID;

        /// <summary>
        /// The id of the type of the edge
        /// </summary>
        public readonly Int64 TypeID;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new CouldNotFindStructuredEdgePropertyException exception
        /// </summary>
        /// <param name="myTypeID">The edge type id</param>
        /// <param name="myPropertyID">The desired property of the edge</param>
        public CouldNotFindStructuredEdgePropertyException(Int64 myTypeID, Int64 myPropertyID)
        {
            TypeID = myTypeID;
            PropertyID = myPropertyID;
        }

        #endregion
                
    }
}