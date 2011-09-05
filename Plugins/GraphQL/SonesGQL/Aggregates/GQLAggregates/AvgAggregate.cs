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
using System.Collections.Generic;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.Library.Arithmetics;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The aggregate Avg
    /// </summary>
    public sealed class AvgAggregate : IGQLAggregate, IPluginable
    {
        #region constructor

        /// <summary>
        /// creates a new avg aggregate
        /// </summary>
        public AvgAggregate()
        {
 
        }

        #endregion

        #region describe
        /// <summary>
        /// Returns the description of the aggregate.
        /// </summary>
        public string GetDescribeOutput()
        {
            return "This aggregate will calculate the average of the given operands. This aggregate is type dependent and will only operate on numbers.";
        } 
        #endregion

        #region IGQLAggregate Members

        /// <summary>
        /// Calculates the average
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            var divType = myPropertyDefinition.BaseType;
            IComparable sum = null;
            uint total = 0;

            foreach (var value in myValues)
            {
                if (sum == null)
                {
                    sum = ArithmeticOperations.Add(divType, 0, value);
                }
                else
                {
                    sum = ArithmeticOperations.Add(divType, sum, value);
                }

                total++;
            }

            var aggregateResult = ArithmeticOperations.Div(typeof(Double), sum, total);

            return new FuncParameter(aggregateResult, myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.avg"; }
        }

        public string PluginShortName
        {
            get { return "avg"; }
        }
        
        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new AvgAggregate();
        }

        public void Dispose()
        { }

        #endregion

        #region IGQLAggregate Members

        public string AggregateName
        {
            get { return "avg"; }
        }

        #endregion
    }
}
