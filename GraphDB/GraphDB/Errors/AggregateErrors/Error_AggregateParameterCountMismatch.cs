/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates;

namespace sones.GraphDB.Errors
{
    public class Error_AggregateParameterCountMismatch : GraphDBAggregateError
    {

        public Int32 ExpectedParameterCount { get; private set; }
        public Int32 CurrentParameterCount { get; private set; }
        //public ABaseAggregate Aggregate { get; private set; }
        public String Aggregate { get; private set; }

        public Error_AggregateParameterCountMismatch(String myAggregate, Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Aggregate = myAggregate;
        }

        public Error_AggregateParameterCountMismatch(Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Aggregate = null;
        }
        
        public override string ToString()
        {
            if (Aggregate != null)
            {
                return String.Format("The number of parameters [{0}] of the function [{1}]does not match the definition [{2}]", CurrentParameterCount, Aggregate, ExpectedParameterCount);
            }
            else
            {
                return String.Format("The number of parameters [{0}] of the function does not match the definition [{1}]", CurrentParameterCount, ExpectedParameterCount);
            }
        }
    }
}
