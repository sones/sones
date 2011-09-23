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

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown graphdb exception
    /// </summary>
    public sealed class UnknownDBException : AGraphDBException
    {
		#region constructor

		/// <summary>
		/// Initializes a new instance of the UnknownDBException.
		/// </summary>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public UnknownDBException(Exception innerException = null) : this("An unknown error has occurred.", innerException)
        {}

		/// <summary>
		/// Initializes a new instance of the UnknownDBException and specifying a custom exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public UnknownDBException(string message, Exception innerException = null) : base(innerException)
        {
            _msg = message;
        }

        #endregion
    }
}