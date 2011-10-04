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

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A type is not implemented for aggregates
    /// </summary>
    public sealed class NotImplementedAggregateTargetException : ASonesQLAggregateException
    {
        public Type AggregateTarget { get; private set; }

        /// <summary>
        /// Creates a new NotImplementedAggregateTargetException exception
        /// </summary>
        /// <param name="myAggregateTarget"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public NotImplementedAggregateTargetException(Type myAggregateTarget, Exception innerException = null) : base(innerException)
        {
            AggregateTarget = myAggregateTarget;
            _msg = String.Format("Currently the type {0} is not implemented for aggregates.", AggregateTarget.Name);
        }

    }
}
