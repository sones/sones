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

namespace sones.Library.Commons.Transaction
{
    /// <summary>
    /// The state of a transaction
    /// </summary>
    public enum TransactionState
    {
		/// <summary>
		/// The transaction is active.
        /// </summary>
        Active,

        /// <summary>
		/// The transaction has been comitted.
        /// </summary>
        Committed,

		/// <summary>
		/// The transaction is in the process of comitting.
		/// </summary>
		Committing,

		/// <summary>
        /// The transaction has been aborted.
        /// </summary>
        Aborted,

		/// <summary>
		/// The transaction is in the process of aborting.
		/// </summary>
		Aborting,
    }
}