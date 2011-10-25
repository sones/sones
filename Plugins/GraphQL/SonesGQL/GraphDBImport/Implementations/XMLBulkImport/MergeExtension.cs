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
using System.Text;

namespace sones.Plugins.SonesGQL.XMLBulkImport
{
    /// <summary>
    /// Contains a merge algorithm.
    /// </summary>
    /// <remarks>
    /// This class contains only one method that implements a mege algorithm.
    /// It was made as an extension class to have easier access from the test framework.
    /// </remarks>
    internal static class MergeExtension
    {
        /// <summary>
        /// Merges a bunch of sorted chunks.
        /// </summary>
        /// <remarks>
        /// This method merges a bunch of enumerations to one big enumeration.
        /// If all enumerations are sorted (according to the IncomingEdgeComparer), the result enumeration will be sorted too.
        /// </remarks>
        /// <param name="myChunks">A bunch of enumerations, which values are sorted.</param>
        /// <returns>All values contained in the inner enumerables, in order.</returns>
        public static IEnumerable<IncomingEdge> Merge(this IEnumerable<IEnumerable<IncomingEdge>> myChunks)
        {
            if (myChunks == null)
                throw new NullReferenceException();

            #region Preparations
		    
            //gets the enumerators and move them to the first element. If the enumeration is empty it is excluded.
            var enumerators = myChunks
                .Where(_ => _ != null)
                .Select(_ => _.GetEnumerator())
                .Where(_ => _.MoveNext())
                .ToList();

            //no data
            if (enumerators.Count == 0)
                yield break;

            //initial sort
            enumerators.Sort(IncomingEdgeComparer.Instance);

	        #endregion

            #region The merge algorithm

            while (enumerators.Count > 1)
            {
                var enumerator = enumerators.First();

                //return the element of the first enumerator
                yield return enumerator.Current;

                //remove the enumerator before MoveNext
                enumerators.RemoveAt(0);

                //move the enumerator
                if (enumerator.MoveNext())
                {
                    //if the enumerator contains more values, insert it at the right position to hold the order.

                    var pos = enumerators.BinarySearch(enumerator, IncomingEdgeComparer.Instance);
                    if (pos < 0)
                        pos = ~pos;

                    enumerators.Insert(pos, enumerator);
                }
            }

            #endregion

            #region Returning the content of the last enumerator

            var remaining = enumerators.First();
            do
            {
                yield return remaining.Current;

            } while (remaining.MoveNext());

            #endregion
        }
       
    }
}
