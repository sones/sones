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
using sones.Library.LanguageExtensions;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The return type of the function is invalid
    /// </summary>
    public sealed class InvalidFunctionReturnTypeException : AGraphQLFunctionException
    {
        public String FunctionName { get; private set; }
        public Type TypeOfFunctionReturn { get; private set; }
        public Type[] ValidTypes { get; private set; }

        /// <summary>
        /// Creates a new InvalidFunctionReturnTypeException exception
        /// </summary>
        /// <param name="myFunctionName">The name of the function</param>
        /// <param name="myTypeOfFunctionReturn">The type of the return value of the function</param>
        /// <param name="myValidTypes">A List of valid types</param>
        public InvalidFunctionReturnTypeException(String myFunctionName, Type myTypeOfFunctionReturn, params Type[] myValidTypes)
        {
            FunctionName = myFunctionName;
            TypeOfFunctionReturn = myTypeOfFunctionReturn;
            ValidTypes = myValidTypes;

            if (ValidTypes.IsNullOrEmpty())
            {
                _msg = String.Format("The return type [{0}] of function [{1}] is not valid.", TypeOfFunctionReturn, FunctionName);
            }
            else
            {
                _msg = String.Format("The return type [{0}] of function [{1}] is not valid. Please choose one of: {2}", TypeOfFunctionReturn, FunctionName, ValidTypes.ToAggregatedString(t => t.Name));
            }
        }
        
    }
}
