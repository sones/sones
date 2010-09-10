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

/* <id name=”Libraries Datastructures – BStarTree” />
 * <copyright file=”IBStarTree.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
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

namespace sones.Lib.DataStructures.BPlusTree.Versioned
{
    public interface IVersionedBPlusTree<TKey, TValue> : IVersionedIndexInterface<TKey, TValue>
        where TKey : IComparable
    {
        #region properties

        UInt64 HistorySize
        {
            get;
            set;
        }

        Int32 Order
        {
            get;
        }

        #endregion

        #region KeyValuePair<TKey, IndexValueHistoryList<TValue>> Handling

        /*
         * HACK:
         * These methods are used for simple (de-)serialization
         * 
         * When there are methods to insert versions into existing versions 
         * these methods can be replaced. Currently it's not possible to insert
         * KeyValuePairs with a given version.
         * 
         * (see IndexValueHistoryList)
         * 
         */
        void Add(TKey myKey, IndexValueHistoryList<TValue> myIndexValueHistoryList);

        IEnumerator<KeyValuePair<TKey, IndexValueHistoryList<TValue>>> GetKVPEnumerator();

        #endregion
    }
}
