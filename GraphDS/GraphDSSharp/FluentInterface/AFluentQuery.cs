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
 * sones GraphDS API - AFluentQuery
 * (c) Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.NewAPI;

using sones.GraphDB.Result;

#endregion

namespace sones.GraphDS.API.CSharp.Fluent
{

    public abstract class AFluentQuery
    {

        #region Data

        internal  readonly AGraphDSSharp _DBWrapper      = null;
        protected readonly StringBuilder _CommandString  = null;

        #endregion

        #region Properties

        public String Name { get; protected set; }

        #endregion

        #region Constructor(s)

        public AFluentQuery(AGraphDSSharp myAGraphDSSharp)
        {
            _DBWrapper      = myAGraphDSSharp;
            _CommandString  = new StringBuilder();
        }

        #endregion


        public abstract String GetGQLQuery();

        #region AddToCommandString(myIEnumerable, myPrefix, myAddSeparator = false)

        protected void AddToCommandString(IEnumerable<String> myIEnumerable, String myPrefix, Boolean myAddSeparator = false)
        {

            var _Array = myIEnumerable.ToArray();            

            if (_Array.Any())
            {

                _CommandString.Append(" ").Append(myPrefix).Append(" (");

                var Anzahl = _Array.Count();

                for (var i = 0; i < Anzahl - 1; i++)
                    _CommandString.Append(_Array[i] + ", ");

                _CommandString.Append(_Array[Anzahl - 1]).Append(")");
                                
                if(myAddSeparator)
                    _CommandString.Append(",");
            
            }

        }

        #endregion


        #region Execute()

        public QueryResult Execute()
        {

            if (_DBWrapper != null)
                return _DBWrapper.Query(GetGQLQuery());

            throw new ArgumentNullException("_DBWrapper is null!");

        }

        #endregion

        #region Execute(myAction)

        public QueryResult Execute(Action<QueryResult> myAction)
        {

            if (_DBWrapper != null)
            {

                var _QueryResult = _DBWrapper.Query(GetGQLQuery());

                if (myAction != null)
                    myAction(_QueryResult);

                return _QueryResult;

            }

            throw new ArgumentNullException("_DBWrapper is null!");

        }

        #endregion

        #region Execute(mySuccessAction, myFailureAction)

        public QueryResult Execute(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction)
        {

            if (_DBWrapper != null)
            {

                var _QueryResult = _DBWrapper.Query(GetGQLQuery());

                if (_QueryResult.Failed)
                {
                    if (myFailureAction != null)
                        myFailureAction(_QueryResult);
                }

                else
                {
                    if (mySuccessAction != null)
                        mySuccessAction(_QueryResult);
                }

                return _QueryResult;

            }

            throw new ArgumentNullException("_DBWrapper is null!");

        }

        #endregion

        #region Execute(mySuccessAction, myPartialSuccessAction, myFailureAction)

        public QueryResult Execute(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction)
        {

            if (_DBWrapper != null)
            {

                var _QueryResult = _DBWrapper.Query(GetGQLQuery());

                if (_QueryResult.Success)
                {
                    if (mySuccessAction != null)
                        mySuccessAction(_QueryResult);
                }

                else if (_QueryResult.PartialSuccess)
                {
                    if (myPartialSuccessAction != null)
                        myPartialSuccessAction(_QueryResult);
                }

                else
                {
                    if (myFailureAction != null)
                        myFailureAction(_QueryResult);
                }

                return _QueryResult;

            }

            throw new ArgumentNullException("_DBWrapper is null!");

        }

        #endregion


        #region ExecuteToObject(myDepth)

        public SelectToObjectGraph ExecuteToObject(UInt16? myDepth = null)
        {

            if (_DBWrapper == null)
                throw new ArgumentNullException("_DBWrapper is null!");

            if (myDepth == null)
                return new SelectToObjectGraph(_DBWrapper.Query(GetGQLQuery()));

            else
                return new SelectToObjectGraph(_DBWrapper.Query(GetGQLQuery() + " DEPTH " + myDepth));

        }

        #endregion

        #region ExecuteToObject<T>(myDepth)

        public IEnumerable<T> ExecuteToObject<T>(UInt16? myDepth = null)
            where T : DBObject, new()
        {

            if (_DBWrapper == null)
                throw new ArgumentNullException("_DBWrapper is null!");

            if (myDepth == null)
                return new SelectToObjectGraph(_DBWrapper.Query(GetGQLQuery())).ToVertexType<T>();

            else
                return new SelectToObjectGraph(_DBWrapper.Query(GetGQLQuery() + " DEPTH " + myDepth)).ToVertexType<T>();

        }

        #endregion

    }

}
