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
using sones.GraphDB;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDS
{
    #region IGraphDSVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDS plugin versions. 
    /// Defines the min and max version for all IGraphDS implementations which will be activated used this IGraphDS.
    /// </summary>
    internal static class IGraphDSVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all graphDS
    /// </summary>
    public interface IGraphDS : IGraphDB
    {
        /// <summary>
        /// Returns a query result by passing a query string
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myQueryString">The query string that should be executed</param>
        /// <param name="myQueryLanguageName">The identifier of the language that should be used for parsing the query</param>
        /// <returns>A query result</returns>
        QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                          String myQueryString,
                          String myQueryLanguageName);
    }
}