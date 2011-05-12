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

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for all hyper edges
    /// </summary>
    public interface IHyperEdge : IEdge
    {
        /// <summary>
        /// Gets all contained edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges</param>
        /// <returns>An IEnumerable of edges</returns>
        IEnumerable<ISingleEdge> GetAllEdges(PropertyHyperGraphFilter.SingleEdgeFilter myFilter = null);

        /// <summary>
        /// Invokes a function on a hyper edge
        /// </summary>
        /// <typeparam name="TResult">The type of the result aka the function output</typeparam>
        /// <param name="myHyperEdgeFunction">A function that is executed on a hyper edge</param>
        /// <returns>A TResult</returns>
        TResult InvokeHyperEdgeFunc<TResult>(Func<IEnumerable<ISingleEdge>, TResult> myHyperEdgeFunction);
    }
}