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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib;

namespace sones.GraphDB.QueryLanguage.Result
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
        public Edge(IEnumerable<DBObjectReadout> objects, String edgeTypeName = "")
        {
            _Objects = objects;
            _EdgeTypeName = edgeTypeName;
        }

        public Edge(DBObjectReadout dBObjectReadout, String edgeTypeName = "")
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

    }
}
