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

/* <id name="GraphDB – Selection list element result" />
 * <copyright file="SelectionListElementResult.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sones.Lib;

#endregion

namespace sones.GraphDBInterface.Result
{

    /// <summary>
    /// The instances of this class represent a single selection in the selectionList.
    /// </summary>
    public class SelectionResultSet : IEnumerable<DBObjectReadout>
    {

        #region Data

        public UInt64 NumberOfAffectedObjects
        {
            get
            {
                return _Objects.ULongCount();
            }
        }

        #region Objects

        private IEnumerable<DBObjectReadout> _Objects = null;

        public IEnumerable<DBObjectReadout> Objects
        {
            get
            {
                return _Objects;
            }
        }

        #endregion

        #region this[Int32]

        public DBObjectReadout this[Int32 elementAt]
        {
            get
            {
                return _Objects.ElementAt(elementAt);
            }
        }

        #endregion

        #endregion

        #region Constructor

        public SelectionResultSet()
        {
            _Objects = new List<DBObjectReadout>();
        }

        public SelectionResultSet(DBObjectReadout myListOfDBObjectReadout)
            : this(new List<DBObjectReadout>(){ myListOfDBObjectReadout })
        { }

        public SelectionResultSet(IEnumerable<DBObjectReadout> myListOfDBObjectReadout)
        {
            _Objects            = myListOfDBObjectReadout ?? new List<DBObjectReadout>();
        }

        #endregion


        #region IEnumerable<DBObjectReadout> Members

        public IEnumerator<DBObjectReadout> GetEnumerator()
        {
            return _Objects.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is SelectionResultSet)
            {
                return Equals((SelectionResultSet)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(SelectionResultSet p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return CompareDBObjectReadouts(this._Objects, p._Objects);
        }

        private bool CompareDBObjectReadouts(IEnumerable<DBObjectReadout> iEnumerable_1, IEnumerable<DBObjectReadout> iEnumerable_2)
        {
            if (iEnumerable_1.Count() != iEnumerable_2.Count())
            {
                return false;
            }

            if (iEnumerable_1.Count() == 0)
            {
                return true;
            }

            foreach (var aOuterResult in iEnumerable_1)
            {
                var equals = iEnumerable_2.Contains(aOuterResult);

                if (!equals)
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean operator ==(SelectionResultSet a, SelectionResultSet b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(SelectionResultSet a, SelectionResultSet b)
        {
            return !(a == b);
        }

        #endregion

    }

}
