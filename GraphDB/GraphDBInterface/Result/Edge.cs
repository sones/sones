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
using System.Collections.Generic;
using System.Linq;

namespace sones.GraphDBInterface.Result
{
    public class Edge : IEnumerable<DBObjectReadout>
    {

        private IEnumerable<DBObjectReadout> _Objects;

        #region EdgeTypeName

        private String _EdgeTypeName;
        public String EdgeTypeName
        {
            get { return _EdgeTypeName; }
            set { _EdgeTypeName = value; }
        }

        #endregion

        /// <summary>
        /// Creates a new edge containing the <paramref name="objects"/>
        /// </summary>
        /// <param name="objects">The objects</param>
        /// <param name="edgeTypeName">The type of the edge. For User.Friends this would be User. </param>
        public Edge(IEnumerable<DBObjectReadout> objects, String edgeTypeName)
        {
            _Objects = objects;
            _EdgeTypeName = edgeTypeName;
        }

        public Edge(DBObjectReadout dBObjectReadout, String edgeTypeName)
        {
            _Objects = new List<DBObjectReadout>(){ dBObjectReadout };
            _EdgeTypeName = edgeTypeName;
        }

        #region IEnumerable<DBObjectReadout> Members

        public IEnumerator<DBObjectReadout> GetEnumerator()
        {
            return _Objects.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region implicit operator

        public static implicit operator DBObjectReadout(Edge edge)
        {
            return edge.FirstOrDefault();
        }


        public static implicit operator List<DBObjectReadout>(Edge edge)
        {
            if (edge._Objects is List<DBObjectReadout>)
                return (edge._Objects as List<DBObjectReadout>);
            else
                return edge.ToList();
        }

        #endregion

        public DBObjectReadout this[Int32 elementAt]
        {
            get
            {
                return _Objects.ElementAtOrDefault(elementAt);
            }
        }

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is Edge)
            {
                return Equals((Edge)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(Edge p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this._EdgeTypeName == p._EdgeTypeName) && CompareVertices(this._Objects, p._Objects);
        }

        private bool CompareVertices(IEnumerable<DBObjectReadout> iEnumerable_1, IEnumerable<DBObjectReadout> iEnumerable_2)
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

        public static Boolean operator ==(Edge a, Edge b)
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

        public static Boolean operator !=(Edge a, Edge b)
        {
            return !(a == b);
        }

        #endregion

    }
}
