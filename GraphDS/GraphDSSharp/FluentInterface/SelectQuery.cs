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

/*
 * sones GraphDS API - SelectQuery
 * (c) Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDS.API.CSharp.Fluent
{

    public class SelectQuery : AFluentQuery
    {

        #region Data

        private List<String> _FromStrings;
        private List<String> _SelectStrings;
        private List<String> _WhereStrings;
        private UInt16?      _Depth;

        #endregion

        #region Constructor(s)

        #region SelectQuery(myAGraphDSSharp)

        public SelectQuery(AGraphDSSharp myAGraphDSSharp)
            : base(myAGraphDSSharp)
        {
            _FromStrings    = new List<String>();
            _SelectStrings  = new List<String>();
            _WhereStrings   = new List<String>();
            _Depth          = null;
        }

        #endregion

        #endregion


        #region From(myCreateVertexQuery)

        public SelectQuery From(CreateVertexQuery myCreateVertexQuery)
        {
            return From(myCreateVertexQuery, "");
        }

        #endregion

        #region From(myCreateVertexQuery, myAlias)

        public SelectQuery From(CreateVertexQuery myCreateVertexQuery, String myAlias)
        {
            Name = myCreateVertexQuery.Name;
            _FromStrings.Add(Name + " " + myAlias);
            return this;
        }

        #endregion

        #region From(myDBVertex)

        public SelectQuery From(DBVertex myDBVertex)
        {
            return From(myDBVertex, "");
        }

        #endregion

        #region From(myDBVertex, myAlias)

        public SelectQuery From(DBVertex myDBVertex, String myAlias)
        {
            Name = myDBVertex.GetType().Name;
            _FromStrings.Add(Name + " " + myAlias);
            return this;
        }

        #endregion

        #region From(myDBVertexTypeName)

        public SelectQuery From(String myDBVertexTypeName)
        {
            return From(myDBVertexTypeName, "");
        }

        #endregion

        #region From(myDBVertexTypeName, myAlias)

        public SelectQuery From(String myDBVertexTypeName, String myAlias)
        {
            Name = myDBVertexTypeName;
            _FromStrings.Add(Name + " " + myAlias);
            return this;
        }

        #endregion

        #region From<T>(myAlias)

        public SelectQuery From<T>(String myAlias)
            where T : DBVertex
        {
            Name = typeof(T).Name;
            _FromStrings.Add(Name + " " + myAlias);
            return this;
        }

        #endregion

        

        #region Select(mySelections)

        public SelectQuery Select(List<String> mySelections)
        {
            _SelectStrings.AddRange(mySelections);
            return this;
        }

        #endregion

        #region Select(params mySelections)

        public SelectQuery Select(params String[] mySelections)
        {
            _SelectStrings.AddRange(mySelections);
            return this;
        }

        #endregion

        #region SelectAll()

        public SelectQuery SelectAll()
        {
            _SelectStrings.Add("*");
            return this;
        }

        #endregion

        #region SelectAllEdges()

        public SelectQuery SelectAllEdges()
        {
            _SelectStrings.Add("-");
            return this;
        }

        #endregion

        #region SelectAllProperties()

        public SelectQuery SelectAllProperties()
        {
            _SelectStrings.Add("#");
            return this;
        }

        #endregion



        #region Where(myWhereClause)

        public SelectQuery Where(String myWhereClause)
        {
            _WhereStrings.Add(myWhereClause);
            return this;
        }

        #endregion

        #region Depth(myDepth)

        public SelectQuery Depth(UInt16 myDepth)
        {
            _Depth = myDepth;
            return this;
        }

        #endregion


        #region GetGQLQuery()

        public override String GetGQLQuery()
        {

            var _Command = "";

            #region FROM ...

            _Command += "FROM ";

            if (_FromStrings.Count > 0)
            {

                var Anzahl = _FromStrings.Count;

                for (var i = 0; i < Anzahl - 1; i++)
                    _Command = _Command + _FromStrings[i] + ", ";

                _Command += _FromStrings[Anzahl - 1] + " ";

            }

            #endregion

            #region SELECT...

            if (_SelectStrings.Count > 0)
            {

                _Command += "SELECT ";

                var Anzahl = _SelectStrings.Count;

                for (var i = 0; i < Anzahl - 1; i++)
                    _Command = _Command + _SelectStrings[i] + ", ";

                _Command += _SelectStrings[Anzahl - 1] + " ";

            }

            #endregion

            #region WHERE...

            if (_WhereStrings.Count > 0)
            {

                _Command += "WHERE ";

                var Anzahl = _WhereStrings.Count;

                for (var i = 0; i < Anzahl - 1; i++)
                    _Command = _Command + _WhereStrings[i] + ", ";

                _Command += _WhereStrings[Anzahl - 1];

            }

            #endregion

            #region DEPTH...

            if (_Depth != null)
                _Command += "DEPTH " + _Depth;

            #endregion

            return _Command;

        }

        #endregion


        #region ToString()

        public override string ToString()
        {
            return GetGQLQuery();
        }

        #endregion

    }

}

