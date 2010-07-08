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

/* 
 * sones GraphDS API - AGraphDSQuery
 * Achim Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;

using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace sones.GraphDS.API.CSharp
{

    public abstract class AGraphDSQuery
    {

        #region Data

        internal  AGraphDSSharp _DBWrapper      = null;
        protected StringBuilder _CommandString  = null;

        #endregion

        public abstract String GetGQLQuery();


        protected void AddToCommandString(IEnumerable<String> myIEnumerable, String myPrefix)
        {

            var _Array = myIEnumerable.ToArray();

            if (_Array.Count() > 0)
            {

                _CommandString.Append(" ").Append(myPrefix).Append(" (");

                var Anzahl = _Array.Count();

                for (var i = 0; i < Anzahl - 1; i++)
                    _CommandString.Append(_Array[i] + ", ");

                _CommandString.Append(_Array[Anzahl - 1]).Append(")");

            }

        }


        #region Execute()

        public QueryResult Execute()
        {

            if (_DBWrapper != null)
                return _DBWrapper.Query(this.GetGQLQuery());

            throw new ArgumentNullException("_DBWrapper is null!");

        }

        #endregion

        #region Execute(myAction)

        public QueryResult Execute(Action<QueryResult> myAction)
        {

            if (_DBWrapper != null)
            {

                var _QueryResult = _DBWrapper.Query(this.GetGQLQuery());

                if (myAction != null)
                    myAction(_QueryResult);

                return _QueryResult;

            }

            throw new ArgumentNullException("_DBWrapper is null!");

        }

        #endregion

        #region Execute()

        public SelectToObjectGraph ExecuteToObject(UInt16 myDepth)
        {

            if (_DBWrapper != null)
                //return new SelectToObject(_DBWrapper.QueryXml(this.GetGQLQuery() + " DEPTH " + myDepth));
                return new SelectToObjectGraph(_DBWrapper.Query(this.GetGQLQuery() + " DEPTH " + myDepth));

            throw new ArgumentNullException("_DBWrapper is null!");

        }

        #endregion

    }

}
