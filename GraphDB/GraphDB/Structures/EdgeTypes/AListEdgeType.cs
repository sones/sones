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

/* <id name="sones GraphDB – AListEdgeType" />
 * <copyright file="AListEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The abstract for all list edges - either ListBase or ListReference.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// The abstract for all list edges - either ListBase or SetReferences or SetBase.
    /// </summary>    
    public abstract class AListEdgeType : AEdgeType, IEnumerable
    {
        /// <summary>
        /// Count all values
        /// </summary>
        /// <returns>The number of values</returns>
        public abstract UInt64 Count();

        /// <summary>
        /// Returns all values
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable GetAll();

        /// <summary>
        /// Returns the top <paramref name="myNumOfEntries"/> values
        /// </summary>
        /// <param name="myNumOfEntries"></param>
        /// <returns></returns>
        public abstract IEnumerable GetTop(UInt64 myNumOfEntries);

        /// <summary>
        /// Union with a AEdgeType of the same type.
        /// </summary>
        /// <param name="myAListEdgeType"></param>
        public abstract void UnionWith(AEdgeType myAListEdgeType);

        /// <summary>
        /// Removes all elements which exist more than once
        /// </summary>
        public abstract void Distinction();

        /// <summary>
        /// Clear all elements
        /// </summary>
        public abstract void Clear();

        #region IEnumerable Members

        public abstract IEnumerator GetEnumerator();

        #endregion

    }
}
