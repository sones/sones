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

using sones.GraphDB;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ObjectManagement;

using sones.GraphFS.Session;
using sones.Lib;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Transactions;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Transactions;

namespace GraphDBAPI
{
    /// <summary>
    /// A NOT thread-save bulk insert class.
    /// </summary>
    public class BulkInsert : IDisposable
    {

        private IGraphDBSession _GraphDBSession;
        private IGraphFSSession _GraphFSSession;
        private DBTransaction _DBTransaction;
        private GraphDBType _Type;
        
        private DBObjectStream _LastDBO = null;
        private BackwardEdgeStream _LastBackwardEdge = null;

        /// <summary>
        /// Total of currently inserted DBObjects
        /// </summary>
        public UInt64 Count
        {
            get { return _Count; }
        }
        private UInt64 _Count = 0;

        /// <summary>
        /// Total of currently inserted edges
        /// </summary>
        public UInt64 EdgesCount
        {
            get { return _EdgesCount; }
        }
        private UInt64 _EdgesCount = 0;

        /// <summary>
        /// Total of currently inserted backwardEdges
        /// </summary>
        public UInt64 BackwardEdgesCount
        {
            get { return _BackwardEdgesCount; }
        }
        private UInt64 _BackwardEdgesCount = 0;

        /// <summary>
        /// Create a new BulkInsert.
        /// </summary>
        /// <param name="myGraphDBSession">The DB Reference</param>
        /// <param name="myGraphFSSession">The FS reference</param>
        /// <param name="myType">The type of the ne DBObjects</param>
        public BulkInsert(IGraphDBSession myGraphDBSession, IGraphFSSession myGraphFSSession, GraphDBType myType)
        {
            _GraphDBSession = myGraphDBSession;
            _GraphFSSession = myGraphFSSession;

            _Type = myType;

            (_GraphFSSession.SessionToken.SessionInfo as FSSessionInfo).FSSettings.ReflushAllocationMap = false;

            _DBTransaction = _GraphDBSession.BeginTransaction(myLongRunning: true, myIsolationLevel: IsolationLevel.Serializable);
        }

        /// <summary>
        /// Create a new BulkInsert.
        /// </summary>
        /// <param name="myGraphDBSession">The DB Reference</param>
        /// <param name="myGraphFSSession">The FS reference</param>
        /// <param name="myType">The type of the ne DBObjects</param>
        public BulkInsert(IGraphDBSession myGraphDBSession, IGraphFSSession myGraphFSSession, String myType)
        {
            _GraphDBSession = myGraphDBSession;
            _GraphFSSession = myGraphFSSession;

            (_GraphFSSession.SessionToken.SessionInfo as FSSessionInfo).FSSettings.ReflushAllocationMap = false;

            _DBTransaction = _GraphDBSession.BeginTransaction(myLongRunning: true, myIsolationLevel: IsolationLevel.Serializable);
            _Type = ((DBContext)_DBTransaction.GetDBContext()).DBTypeManager.GetTypeByName(myType);
        }

        /// <summary>
        /// Inserts a complete DBObject with all attributes and edges without verifying references.
        /// Be aware that an already existing DBObject will be overwritten! 
        /// Do not forget to call Flush() after doing all changes! Otherwise no objects will be stored.
        /// </summary>
        /// <param name="myObjectUUID"></param>
        /// <returns></returns>
        public BulkInsertDBO Insert(ObjectUUID myObjectUUID)
        {
            var dbContext = (DBContext)_DBTransaction.GetDBContext();

            if (myObjectUUID == null)
            {
                myObjectUUID = ObjectUUID.NewUUID;
            }

            var DBObjectStream = new DBObjectStream(myObjectUUID, 
                                                    _Type, 
                                                    new Dictionary<AttributeUUID, IObject>(),
                                                    new ObjectLocation(_Type.ObjectLocation, DBConstants.DBObjectsLocation, dbContext.DBObjectManager.GetDBObjectStreamShard(_Type, myObjectUUID), myObjectUUID.ToString()));

            return new BulkInsertDBO(dbContext, _Type, DBObjectStream, this);

        }

        /*
        public BulkInsertDBO Insert(ObjectUUID myObjectUUID)
        {
            var DBObjectStream = new DBObjectStream(_Type, new Dictionary<AttributeUUID, AObject>());
            UnitTestHelper.SetPrivateField("_ObjectUUID", DBObjectStream, myObjectUUID);
            DBObjectStream.ObjectLocation = new ObjectLocation(DBObjectStream.ObjectLocation.Path, DBObjectStream.ObjectUUID.ToHexString());

            return new BulkInsertDBO(_GraphDBSession.GetTypeManager(), _Type, DBObjectStream, this);
        }
         
        public BulkInsertBE InsertBackwardedge(ObjectUUID myObjectUUID)
        {
            var BackwardEdgeStream = new BackwardEdgeStream(new ObjectLocation(_Type.ObjectLocation, DBConstants.DBObjectsLocation, myObjectUUID.ToHexString()));

            return new BulkInsertBE(_GraphDBSession.GetTypeManager(), _Type, BackwardEdgeStream, this);
        }
*/
        
        /// <summary>
        /// Internal method which will be invoked from BulkInsertDBO.Flush() for each new DBObject.
        /// </summary>
        /// <param name="myDBObjectStream"></param>
        /// <param name="myBackwardEdgeStream"></param>
        internal void Insert(DBObjectStream myDBObjectStream, BackwardEdgeStream myBackwardEdgeStream, ulong EdgesCount, ulong BackwardEdgesCount)
        {
            if (myDBObjectStream != null && _LastDBO != null)
            {
                _GraphFSSession.StoreFSObject(_LastDBO, true).FailedAction(e =>
                {
                    throw new GraphDBException(e.IErrors);
                });
            }
            if (myDBObjectStream != null)
            {
                _LastDBO = myDBObjectStream;
            }

            #region The first item need to be write in the usual way

            if (_Count <= 1)
            {
                (_GraphFSSession.SessionToken.SessionInfo as FSSessionInfo).FSSettings.UseRevisionsForParentDirectories = true;
            }
            else
            {
                (_GraphFSSession.SessionToken.SessionInfo as FSSessionInfo).FSSettings.UseRevisionsForParentDirectories = false;
            }

            #endregion

            if (myBackwardEdgeStream != null && _LastBackwardEdge != null)
            {
                _GraphFSSession.StoreFSObject(_LastBackwardEdge, true).FailedAction(e =>
                {
                    throw new GraphDBException(e.IErrors);
                });
            }
            if (myBackwardEdgeStream != null)
            {
                _LastBackwardEdge = myBackwardEdgeStream;
            }

            _Count++;
            _EdgesCount += EdgesCount;
            _BackwardEdgesCount += BackwardEdgesCount;
        }

        /// <summary>
        /// This will store the last DBOject and finish the insert process by flushing the parent and allocationMap.
        /// </summary>
        public Exceptional Flush()
        {
            (_GraphFSSession.SessionToken.SessionInfo as FSSessionInfo).FSSettings.UseRevisionsForParentDirectories = true;
            (_GraphFSSession.SessionToken.SessionInfo as FSSessionInfo).FSSettings.ReflushAllocationMap = true;

            if (_LastDBO != null)
                _GraphFSSession.StoreFSObject(_LastDBO, true);
            if (_LastBackwardEdge != null)
                _GraphFSSession.StoreFSObject(_LastBackwardEdge, true);
        
            _LastDBO = null;
            _LastBackwardEdge = null;
            return _DBTransaction.Commit();
        }

        #region IDisposable Members

        /// <summary>
        /// This will close the BulkInsert and flushes all DBObjects.
        /// </summary>
        public void Dispose()
        {
            Flush();
            _LastDBO = null;
            _LastBackwardEdge = null;
            _GraphDBSession = null;
            _GraphFSSession = null;
            _Type = null;
        }

        #endregion
    }
}
