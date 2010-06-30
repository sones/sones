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
 * IGraphDBSession
 * (c) sones GmbH, 2008 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Transactions;
using sones.GraphFS.Transactions;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.Notifications;


#endregion

namespace sones.GraphDB
{

    public interface IGraphDBSession
    {

        #region Notifications...

        NotificationDispatcher GetNotificationDispatcher();
        NotificationSettings GetNotificationSettings();

        void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher);

        void SetNotificationSettings(NotificationSettings myNotificationSettings);

        #endregion

        String DatabaseRootPath { get; }
        Boolean ReadOnly { get; set; }

        QueryResult             Query(String myQuery);
        QueryResult             ActionQuery(Action<QueryResult> myCheckQuery, String myQuery);
        Exceptional<Object>     MapAndReduce(String myDBTypeName, Func<DBObjectMR, Object> myMap, Func<Object, Object> myReduce);
        IEnumerable<T2>         FilterMapReduce<T1, T2>(String myDBTypeName, Func<DBObjectMR, Boolean> myFilter, Func<DBObjectMR, T1> myMap, Func<IEnumerable<T1>, IEnumerable<T2>> myReduce);

        #region Database Management...

        void Shutdown();
        
        UUID GetDatabaseUniqueID();

        #warning: "No one should get Access to the TypeManager! Or the _PandoraDB.TypeManager needs to be changed to a method with SessionToken parameter!"
        /// <summary>
        /// Do not use me except in tests!!!!
        /// </summary>
        /// <returns></returns>
        DBContext GetDBContext();

        #endregion

        #region Transactions...

        DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null);
        DBTransaction CommitTransaction(bool async = false);
        DBTransaction RollbackTransaction(bool async = false);
        DBTransaction GetLatestTransaction();

        #endregion


    }

}
