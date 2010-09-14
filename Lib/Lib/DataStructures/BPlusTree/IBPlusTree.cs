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

namespace sones.Lib.DataStructures.BPlusTree
{
    public interface IBPlusTree<TKey, TValue> : IIndexInterface<TKey, TValue>
        where TKey : IComparable
    {
        #region range query

        /// <summary>
        /// Returns all values from keys greater or equal fromKey and lower or equal toKey.
        /// If toKey is smaller then fromKey, they are swapped and the values between them 
        /// will be returned.
        /// 
        /// The Method DOES NOT handle duplicate values assigned to different keys!
        /// 
        /// Info: range methods now provided by IIndexInterface
        /// </summary>
        /// <param name="myFromKey"></param>
        /// <param name="myToKey"></param>
        /// <returns>An IEnumerable</returns>
        IEnumerable<TValue> GetValuesInRange(TKey myFromKey, TKey myToKey);

        #endregion        

        #region best match

        /// <summary>
        /// Method returns the value for the best match (nearest neighbour) of the key. 
        /// 
        /// Exact match returns the corresponding set of values.
        /// LowerThan means that if the key is not in the tree, the key which would be
        /// its left neighbour is returned. 
        /// !LowerThen means that if the key is not in the tree, the key which would be
        /// its right neighbour is returned.
        /// 
        /// If myLowerThan is true and there is no lower key, then null will be returned
        /// If myLowerThan is false and there is no greater key, then null will be returned
        /// </summary>
        /// <param name="myKey">the key to find a match</param>
        /// <param name="myLowerThan">the values associated to the best matching key</param>
        /// <returns>Set of values or null if no best key is found</returns>
        HashSet<TValue> GetBestMatch(TKey myKey, bool myLowerThan = true);

        #endregion
    }
}
