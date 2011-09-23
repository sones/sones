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
    /// The number of parameters of the function does not match the definition
    /// </summary>
    public sealed class AggregateParameterCountMismatchException : ASonesQLAggregateException
    {

        public Int32 ExpectedParameterCount { get; private set; }
        public Int32 CurrentParameterCount { get; private set; }        
        public String Aggregate { get; private set; }

        /// <summary>
        /// Creates a new AggregateParameterCountMismatchException exception
        /// </summary>
        /// <param name="myAggregate">The used aggregate</param>
        /// <param name="myExpectedParameterCount">The expected count of parameters</param>
        /// <param name="myCurrentParameterCount">The current count of parameters</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public AggregateParameterCountMismatchException(String myAggregate, Int32 myExpectedParameterCount, Int32 myCurrentParameterCount, Exception innerException = null)
			: base(innerException)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Aggregate = myAggregate;

            if (Aggregate != null)
            {
                _msg = String.Format("The number of parameters [{0}] of the function [{1}]does not match the definition [{2}]", CurrentParameterCount, Aggregate, ExpectedParameterCount);
            }
        }

        /// <summary>
        /// Creates a new AggregateParameterCountMismatchException exception
        /// </summary>        
        /// <param name="myExpectedParameterCount">The expected count of parameters</param>
        /// <param name="myCurrentParameterCount">The current count of parameters</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public AggregateParameterCountMismatchException(Int32 myExpectedParameterCount, Int32 myCurrentParameterCount, Exception innerException = null)
			: base(innerException)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Aggregate = null;

            _msg = String.Format("The number of parameters [{0}] of the function does not match the definition [{1}]", CurrentParameterCount, ExpectedParameterCount);
        }
          
    }
}
