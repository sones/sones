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

using sones.Library.ErrorHandling;
using System;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// The interface for all GraphFS exceptions
    /// </summary>
    public abstract class AGraphFSException : ASonesException
    {
		/// <summary>
		/// Initializes a new instance of the AGraphFSException.
		/// </summary>
		protected AGraphFSException() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the AGraphFSException using a specified inner exception.
		/// </summary>
		/// 
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
		protected AGraphFSException(Exception innerException) : this(null, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the AGraphFSException using a specified message and inner exception.
		/// </summary>
		/// 
		/// <param name="message">The message of the Exception. If this value is NULL, the message will be empty.</param>
		/// <param name="innerException">The inner exception. This vlaue can be NULL.</param>
		protected AGraphFSException(string message, Exception innerException) : base(innerException)
		{
			_msg = message ?? "";
		}
    }
}