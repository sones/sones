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


/* <id name=”Libraries Datastructures – BStarTree” />
 * <copyright file=”IBStarTree.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Martin Junghanns</developer>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.BPlusTree
{
    public interface IBPlusTree<TKey, TValue> : IIndexInterface<TKey, TValue>
        where TKey : IComparable
    {
        #region trygetvalues

        #endregion

        #region range query

        /// <summary>
        /// Returns all values from keys greater or equal fromKey and lower or equal toKey.
        /// If toKey is smaller then fromKey, they are swapped and the values between them 
        /// will be returned.
        /// 
        /// The Method DOES NOT handle duplicate values assigned to different keys!
        /// </summary>
        /// <param name="myFromKey"></param>
        /// <param name="myToKey"></param>
        /// <returns>An IEnumerable</returns>
        IEnumerable<TValue> GetValuesInRange(TKey myFromKey, TKey myToKey);

        #endregion        
    }
}
