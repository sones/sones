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
        public delegate void OnRemovedEventHandler                    (ObjectLocator  myObjectLocator,  String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);
        public delegate void OnRemovedAsyncEventHandler               (ObjectLocator  myObjectLocator,  String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);

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
