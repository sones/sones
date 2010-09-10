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


using System;

namespace sones.GraphDB.Structures.EdgeTypes
{

    public interface IListOrSetEdgeType
    {
        /// <summary>
        /// Count all values
        /// </summary>
        /// <returns>The number of values</returns>
        UInt64 Count();
        
        /// <summary>
        /// Returns the top <paramref name="myNumOfEntries"/> values
        /// </summary>
        /// <param name="myNumOfEntries"></param>
        /// <returns></returns>
        IListOrSetEdgeType GetTopAsEdge(UInt64 myNumOfEntries);

        /// <summary>
        /// Union with a AEdgeType of the same type.
        /// </summary>
        /// <param name="myAListEdgeType"></param>
        void UnionWith(IListOrSetEdgeType myAListEdgeType);

        /// <summary>
        /// Removes all elements which exist more than once
        /// </summary>
        void Distinction();

        /// <summary>
        /// Clear all elements
        /// </summary>
        void Clear();

    }

}
