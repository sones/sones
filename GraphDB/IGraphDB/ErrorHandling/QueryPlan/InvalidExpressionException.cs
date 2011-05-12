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
using sones.GraphDB.Expression;

namespace sones.GraphDB.ErrorHandling.QueryPlan
{
    /// <summary>
    /// An invalid query plan execution exception occured
    /// </summary>
    public sealed class InvalidQueryPlanExecutionException : AGraphDBException
    {
        #region data

        /// <summary>
        /// A description concerning the exception
        /// </summary>
        public readonly String Info;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new invalid query plan execution exception
        /// </summary>
        /// <param name="myInfo">A description concerning the exception</param>
        public InvalidQueryPlanExecutionException(String myInfo)
        {
            Info = myInfo;
            _msg = Info;
        }

        #endregion
    }
}
