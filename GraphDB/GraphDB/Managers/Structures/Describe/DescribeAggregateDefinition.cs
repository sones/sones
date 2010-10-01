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

/*
 * DescribeAggregateDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.Aggregates;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    public class DescribeAggregateDefinition : ADescribeDefinition
    {

        #region Data

        private String _AggregateName;

        #endregion

        #region Ctor

        public DescribeAggregateDefinition(String myAggregateName = null)
        {
            _AggregateName = myAggregateName;
        }

        #endregion

        #region ADescribeDefinition

        public override Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext)
        {

            if (!String.IsNullOrEmpty(_AggregateName))
            {

                #region Specific aggregate

                var aggregate = myDBContext.DBPluginManager.GetAggregate(_AggregateName);
                if (aggregate != null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new List<Vertex>(){GenerateOutput(aggregate, _AggregateName)});
                }
                else
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_AggregateOrFunctionDoesNotExist(_AggregateName));
                }

                #endregion

            }

            else
            {

                #region All aggregates

                var resultingVertices = new List<Vertex>();

                foreach (var aggregate in myDBContext.DBPluginManager.GetAllAggregates())
                {
                    resultingVertices.Add(GenerateOutput(aggregate.Value, aggregate.Key));
                }

                return new Exceptional<IEnumerable<Vertex>>(resultingVertices);

                #endregion

            }

        }

        #endregion

        #region GenerateOutput

        /// <summary>
        /// generate an output for an aggregate
        /// </summary>
        /// <param name="myAggregate">the aggregate</param>
        /// <param name="myAggrName">aggregate name</param>
        /// <returns>list of readouts with the information</returns>
        private Vertex GenerateOutput(ABaseAggregate myAggregate, String myAggrName)
        {

            var _Aggregate = new Dictionary<String, Object>();

            _Aggregate.Add("Aggregate", myAggrName);
            _Aggregate.Add("Type",      myAggrName);

            return new Vertex(_Aggregate);

        }
        
        #endregion

    }
}
