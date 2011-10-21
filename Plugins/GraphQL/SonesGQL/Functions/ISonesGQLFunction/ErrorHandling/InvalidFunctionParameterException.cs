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

namespace sones.Plugins.SonesGQL.Function.ErrorHandling
{
    /// <summary>
    /// The parameter value for this function is invalid
    /// </summary>
    public sealed class InvalidFunctionParameterException : ASonesQLFunctionException
    {
        #region data        

        public String FunctionParameterName { get; private set; }
        public Object FunctionParameterValue { get; private set; }
        public Object Expected { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Creates a new InvalidFunctionParameterException exception
        /// </summary>
        /// <param name="myFunctionParameterName">The function parameter name</param>
        /// <param name="myExpected">The expected parameter value</param>
        /// <param name="myFunctionParameterValue">The function parameter value</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidFunctionParameterException(String myFunctionParameterName, Object myExpected, Object myFunctionParameterValue, Exception innerException = null)
			: base(innerException)
        {
            FunctionParameterName = myFunctionParameterName;
            FunctionParameterValue = myFunctionParameterValue;
            Expected = myExpected;
            _msg = String.Format("Invalid parameter value for \"{0}\"! Expected [{1}] \nCurrent [{2}]", FunctionParameterName, Expected, FunctionParameterValue);
        }

        #endregion

    }
}

