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
    /// The data type of the SelectValueAssignment does not match the type
    /// </summary>
    public class SelectValueAssignmentDataTypeDoesNotMatchException : AGraphQLSelectException
    {
        public String ExpectedDataType { get; private set; }
        public String DataType { get; private set; }

        /// <summary>
        /// Creates a new SelectValueAssignmentDataTypeDoesNotMatchException exception
        /// </summary>
        /// <param name="myExpectedDataType">The expected data type</param>
        /// <param name="myDataType">The current data type</param>
        public SelectValueAssignmentDataTypeDoesNotMatchException(String myExpectedDataType, String myDataType)
        {
            ExpectedDataType = myExpectedDataType;
            DataType = myDataType;
            _msg = String.Format("The data type of the SelectValueAssignment \"{0}\" does not match the type \"{1}\"!", DataType, ExpectedDataType);
        }
                
    }
}
