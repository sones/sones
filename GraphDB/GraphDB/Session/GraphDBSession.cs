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
 * GraphDBSession
 * (c) sones GmbH, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Session;
using sones.GraphDB.Transactions;
using sones.GraphFS.Transactions;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.Notifications;
using sones.GraphDB.Query;


#endregion

namespace sones.GraphDB
{

    public class GraphDBSession : IGraphDBSession
    {

        #region Data

        private GraphDB2     _GraphDB;
        private SessionToken _SessionToken;
        private DBContext    _DBContext = null;
        private QueryManager _QueryManager = null;

        #endregion

        #region Constructor

        #region GraphDBSession(myGraphDB, myUsername)

        public GraphDBSession(GraphDB2 myGraphDB, String myUsername)
        {
            _GraphDB        = myGraphDB;
            _SessionToken   = new SessionToken(new DBSessionInfo(myUsername));

            //TODO: remove true for rebuild indices as soon as they are really persistent
            _DBContext = new DBContext(myGraphDB.GraphFSSession, _GraphDB.DatabaseRootPath, null, _GraphDB.DBSettings, false, new DBSessionSettings(_SessionToken.SessionSettings));

            _QueryManager = new QueryManager(_DBContext.DBPluginManager);
        }

        #endregion

        #endregion


        #region IGraphDBSession Members

        #region ReadOnly

        public Boolean ReadOnly
        {

            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }

        }

        #endregion

        #region Notifications

        public NotificationDispatcher GetNotificationDispatcher()
        {
            return _GraphDB.GetNotificationDispatcher(_SessionToken);
        }

        public NotificationSettings GetNotificationSettings()
        {
            return _GraphDB.GetNotificationSettings(_SessionToken);
        }

        public void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher)
        {
            _GraphDB.SetNotificationDispatcher(myNotificationDispatcher, _SessionToken);
        }

        public void SetNotificationSettings(NotificationSettings myNotificationSettings)
        {
            _GraphDB.SetNotificationSettings(myNotificationSettings, _SessionToken);
        }

        #endregion

        #region Query(QueryScript)

        public QueryResult Query(String QueryScript)
        {
            //TODO: for existing transactions use the context or create a new one

            return _GraphDB.Query(QueryScript, this, _QueryManager);
        }

        #endregion

        #region Query(myAction, QueryScript)

        public QueryResult ActionQuery(Action<QueryResult> myAction, String QueryScript)
        {

            var _QueryResult = _GraphDB.Query(QueryScript, this, _QueryManager);

            if (myAction != null)
                myAction(_QueryResult);

            return _QueryResult;

        }

        #endregion

        #region MapAndReduce(myDBTypeName, myMap, myReduce)

        public Exceptional<Object> MapAndReduce(String myName, Func<DBObjectMR, Object> myMapAction, Func<Object, Object> myReduceAction)
        {
            return _GraphDB.MapAndReduce(_DBContext, myName, myMapAction, myReduceAction);
        }

        #endregion

        #region FilterMapReduce(myDBTypeName, myFilter, myMap, myReduce)

        public IEnumerable<T2> FilterMapReduce<T1, T2>(String myDBTypeName, Func<DBObjectMR, Boolean> myFilter, Func<DBObjectMR, T1> myMap, Func<IEnumerable<T1>, IEnumerable<T2>> myReduce)
        {
            return _GraphDB.FilterMapReduce<T1, T2>(_DBContext, myDBTypeName, myFilter, myMap, myReduce);
        }

        #endregion

        #region Database Management

        public UUID GetDatabaseUniqueID()
        {
            return _GraphDB.GetDatabaseUUID();
        }
        
        public void Shutdown()
        {
            _GraphDB.Shutdown(_SessionToken);
        }

#warning: "No one should get Access to the TypeManager! Or the _PandoraDB.TypeManager needs to be changed to a method with SessionToken parameter!"
        /// <summary>
        /// Do not use me except in tests!!!!
        /// If you need the DBContext, create a new Transaction and use that dbContext!!
        /// </summary>
        /// <returns></returns>
        public DBContext GetDBContext()
        {
            return _DBContext;
        }

        #endregion

        #region Transactions
        
        public DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null)
        {
            return _GraphDB.BeginTransaction(_DBContext, _SessionToken, myDistributed, myLongRunning, myIsolationLevel, myName, timestamp);
        }

        public DBTransaction CommitTransaction(bool async = false)
        {
            return _GraphDB.CommitTransaction(_SessionToken, async);
        }

        public DBTransaction RollbackTransaction(bool async = false)
        {
            return _GraphDB.RollbackTransaction(_SessionToken, async);
        }

        public DBTransaction GetLatestTransaction()
        {
            return DBTransaction.GetLatestTransaction(_SessionToken.SessionInfo.SessionUUID);
        }

        #endregion

        #region DatabaseRootPath

        public String DatabaseRootPath
        {
            get { return _GraphDB.DatabaseRootPath; }
        }

        #endregion

        #endregion

    }

}
