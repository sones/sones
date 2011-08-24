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
using System.Linq;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Interfaces;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The aggregate Max
    /// </summary>
    public sealed class MaxAggregate : IGQLAggregate, IPluginable
    {
        #region constructor

        /// <summary>
        /// Creates a new max aggregate
        /// </summary>
        public MaxAggregate()
        {

        }

        #endregion

        #region describe
        /// <summary>
        /// Returns the description of the aggregate.
        /// </summary>
        public string GetDescribeOutput()
        {
            return "This aggregate will calculate the max value of the given operands. This aggregate is type dependent and will only operate on comparable operands.";
        }
        #endregion

        #region IGQLAggregate Members

        /// <summary>
        /// Calculates the maximum
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            IComparable max = null;

            foreach (var value in myValues)
            {
                if (max == null)
                {
                    max = value;
                }
                else if (max.CompareTo(value) < 0)
                {
                    max = value;
                }
            }

            return new FuncParameter(max, myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.max"; }
        }

        public string PluginShortName
        {
            get { return "max"; }
        }

        public string PluginDescription
        {
            get { return "The Aggregate Max."; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new MaxAggregate();
        }

        public void Dispose()
        { }

        #endregion

        #region IGQLAggregate Members


        public string AggregateName
        {
            get { return "max"; }
        }

        #endregion
    }
}
