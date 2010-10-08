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
        where TValue : IEstimable
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
