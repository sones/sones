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
    /// The number of parameters of the function does not match the definition
    /// </summary>
    public sealed class FunctionParameterCountMismatchException : ASonesQLFunctionException
    {
        #region data        

        public Int32 ExpectedParameterCount { get; private set; }
        public Int32 CurrentParameterCount { get; private set; }
        public String Function { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new FunctionParameterCountMismatchException exception
        /// </summary>
        /// <param name="myFunction">The current function</param>
        /// <param name="myExpectedParameterCount">The expected count of parameters</param>
        /// <param name="myCurrentParameterCount">The current count of parameters</param>
        public FunctionParameterCountMismatchException(String myFunction, Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Function = myFunction;

            _msg = String.Format("The number of parameters [{0}] of the function [{1}]does not match the definition [{2}]", CurrentParameterCount, Function, ExpectedParameterCount);
        }

        /// <summary>
        /// Creates a new FunctionParameterCountMismatchException exception
        /// </summary>
        /// <param name="myExpectedParameterCount">The expected count of parameters</param>
        /// <param name="myCurrentParameterCount">The current count of parameters</param>
        public FunctionParameterCountMismatchException(Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            _msg = String.Format("The number of parameters [{0}] of the function does not match the definition [{1}]", CurrentParameterCount, ExpectedParameterCount);

        }

        #endregion
       
    }
}
