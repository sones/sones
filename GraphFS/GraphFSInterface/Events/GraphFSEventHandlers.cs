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
 * GraphFS - GraphFSEventHandlers
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Transactions;

#endregion

namespace sones.GraphFS.Events
{

    public class GraphFSEventHandlers
    {

        // AFSObject handling
        public delegate void OnLoadEventHandler                       (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);
        public delegate void OnLoadedEventHandler                     (ObjectLocator  myObjectLocator,  AFSObject myAFSObject);
        public delegate void OnLoadedAsyncEventHandler                (ObjectLocator  myObjectLocator,  AFSObject myAFSObject);

        public delegate void OnSaveEventHandler                       (ObjectLocation myObjectLocation, AFSObject myAFSObject);
        public delegate void OnSavedEventHandler                      (ObjectLocator  myObjectLocator,  AFSObject myAFSObject, ObjectRevisionID myOldRevisionID);
        public delegate void OnSavedAsyncEventHandler                 (ObjectLocator  myObjectLocator,  AFSObject myAFSObject, ObjectRevisionID myOldRevisionID);

        public delegate void OnRemoveEventHandler                     (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);
        public delegate void OnRemovedEventHandler                    (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);
        public delegate void OnRemovedAsyncEventHandler               (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);

        // Transaction handling
        public delegate void OnTransactionStartEventHandler           (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);
        public delegate void OnTransactionStartedEventHandler         (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);
        public delegate void OnTransactionStartedAsyncEventHandler    (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);

        public delegate void OnTransactionCommitEventHandler          (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);
        public delegate void OnTransactionCommittedEventHandler       (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);
        public delegate void OnTransactionCommittedAsyncEventHandler  (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);

        public delegate void OnTransactionRollbackEventHandler        (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);
        public delegate void OnTransactionRollbackedEventHandler      (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);
        public delegate void OnTransactionRollbackedAsyncEventHandler (Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimestamp);

    }

}
