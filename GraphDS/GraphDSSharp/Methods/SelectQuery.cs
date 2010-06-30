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

/* sones GraphDS API - SelectQuery
 * Achim Friedland, 2009-2010
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using sones.GraphDS.API.CSharp.Reflection;

#endregion

namespace sones.GraphDS.API.CSharp
{

    public class SelectQuery : AGraphDSQuery
    {

        #region Data

        private List<String> _FromStrings = new List<String>();
        private List<String> _SelectStrings = new List<String>();
        private List<String> _WhereStrings = new List<String>();

        #endregion

        #region Constructors

        #region SelectQuery(mySelection)

        public SelectQuery(String mySelection)
        {
            _SelectStrings.Add(mySelection);
        }

        #endregion

        #region SelectQuery(myIGraphDBSession, myTypeName)

        public SelectQuery(AGraphDSSharp myIGraphDBSession, String myTypeName)
            : this(myTypeName)
        {
            _DBWrapper = myIGraphDBSession;
        }

        #endregion

        #endregion


        #region From(myDBObjectType, myAlias)

        public SelectQuery From(DBObject myDBObjectType, String myAlias)
        {
            _FromStrings.Add(myDBObjectType.UUID + " " + myAlias);
            return this;
        }

        #endregion

        #region From(myTypeName, myAlias)

        public SelectQuery From(String myTypeName, String myAlias)
        {
            _FromStrings.Add(myTypeName + " " + myAlias);
            return this;
        }

        #endregion

        #region From(myDBObjectType, myAlias)

        public SelectQuery From<T>(String myAlias)
            where T : DBObject
        {
            _FromStrings.Add(typeof(T).Name + " " + myAlias);
            return this;
        }

        #endregion

        #region Select(mySelection)

        public SelectQuery Select(String mySelection)
        {
            _SelectStrings.Add(mySelection);
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

