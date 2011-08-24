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
using sones.GraphDB.TypeSystem;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The Aggregate Count
    /// </summary>
    public sealed class CountAggregate :IGQLAggregate, IPluginable
    {
        #region constructor

        /// <summary>
        /// Creates a new count aggregate
        /// </summary>
        public CountAggregate()
        {
 
        }

        #endregion

        #region describe
        /// <summary>
        /// Returns the description of the aggregate.
        /// </summary>
        public string GetDescribeOutput()
        {
            return "This aggregate will operate a count on a MultiEdge / HyperEdge. This aggregate is type dependent and will only operate on Multi- / HyperEdges.";
        }
        #endregion

        #region IGQLAggregate Members

        /// <summary>
        /// Calculates the count
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            return new FuncParameter(Convert.ToUInt64(myValues.Count()), myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.count"; }
        }

        public string PluginShortName
        {
            get { return "count"; }
        }

        public string PluginDescription
        {
            get { return "The Aggregate Count."; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }


        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new CountAggregate();
        }

        public void Dispose()
        { }

        #endregion

        #region IGQLAggregate Members


        public string AggregateName
        {
            get { return "count"; }
        }

        #endregion
    }
}
