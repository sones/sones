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
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Interfaces;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using System.Collections.Generic;
using ISonesGQLFunction.Structure;

namespace sones.Plugins.SonesGQL.Aggregates
{

    #region IQLAggregateVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IQLAggregate plugin versions. 
    /// Defines the min and max version for all IQLAggregate implementations which will be activated used this IQLAggregate.
    /// </summary>
    public static class IGQLAggregateVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all GQL aggregates
    /// </summary>
    public interface IGQLAggregate
    {
        /// <summary>
        /// Abstract aggregate function
        /// </summary>
        /// <returns>The result of the aggregation</returns>
        FuncParameter Aggregate(IEnumerable<IComparable> myValues,
                                IPropertyDefinition myPropertyDefinition);

        /// <summary>
        /// The name of the aggregate
        /// </summary>
        String AggregateName { get; }

        /// <summary>
        /// The ouput of a describe.
        /// </summary>
        /// <returns></returns>
        String GetDescribeOutput();
    }
}