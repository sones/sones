/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper
{
    /// <summary>
    /// this class generate an output for the describe aggregate(s)
    /// </summary>
    public class DescrAggrsOutput
    {

        #region constructor

        public DescrAggrsOutput()
        { }

        #endregion

        #region Output

        /// <summary>
        /// generate an output for an aggregate
        /// </summary>
        /// <param name="myAggregate">the aggregate</param>
        /// <param name="myAggrName">aggregate name</param>
        /// <returns>list of readouts with the information</returns>
        public IEnumerable<DBObjectReadout> GenerateOutput(ABaseAggregate myAggregate, String myAggrName)
        {

            var _Aggregate = new Dictionary<String, Object>();

            _Aggregate.Add("Aggregate",   myAggrName);
            _Aggregate.Add("Type",        myAggrName);
            _Aggregate.Add("ResultType",  myAggregate.TypeOfResult);

            return new List<DBObjectReadout>() { new DBObjectReadout(_Aggregate) };

        }

        #endregion


    }
}
