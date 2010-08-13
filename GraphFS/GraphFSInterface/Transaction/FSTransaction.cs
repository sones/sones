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


///* IPandoraFS - FSTransaction
// * (c) Stefan Licht, 2009
// *  
// * This class stores filesystem specific transactions. 
// * Each BeginTransaction will create a new instance.
// *  - each change on a location (ObjectLocator) or ObjectStream will add a new TransactionDetail which stores some information about the change like
// *    changed ObjectLocator and INode, location and the TransactionUUID
// * 
// * 
// * Lead programmer:
// *      Stefan Licht
// * 
// * */

//using System;
//using System.Linq;
//using System.Collections.Generic;

//using sones.GraphFS.Transactions;
//using sones.Lib.Session;
//using sones.GraphFS.DataStructures;
//using sones.GraphFS.Exceptions;

//namespace sones.GraphFS.Session
//{


//    /// <summary>
//    /// Stores filesystem specific transactions
//    /// </summary>
//    public class FSTransaction : Transaction
//    {

//        /// <summary>
//        /// Subscribe to this event to get informed if the transaction was closed unexpected.
//        /// The delegate must clean up all ressources of the disposed transaction (PreAllcoated tickets etc)
//        /// </summary>
//        public event TransactionDisposedHandler OnDispose;

//        private TransactionState _TransactionState;

//        public TransactionState TransactionState
//        {
//            get { return _TransactionState; }
//            set { _TransactionState = value; }
//        }

//        private Dictionary<String, TransactionDetail> _TransactionDetails;

//        public Dictionary<String, TransactionDetail> TransactionDetails
//        {
//            get { return _TransactionDetails; }
//        }

//        public FSTransaction(SessionToken mySessionTokenReference)
//            : base(mySessionTokenReference)
//        {
//            _TransactionDetails = new Dictionary<string, TransactionDetail>();
//            _TransactionState = TransactionState.Started;
//        }

//        /// <summary>
//        /// Add the information needed for this particular change on a object. This act like a placeholder if just a ObjectStream needs a transaction revision
//        /// </summary>
//        /// <param name="myObjectLocation">The location where something happend</param>
//        public void AddDetail(String myObjectLocation)
//        {
//            /*
//            if (_TransactionDetails.ContainsKey(myObjectLocation))
//                throw new GraphFSException_LocationAlreadyHoldTransaction(myObjectLocation);

//            _TransactionDetails.Add(myObjectLocation, new TransactionDetail());
//             */
//            AddDetail(myObjectLocation, null, null);
//        }

//        /// <summary>
//        /// Add the information needed for this particular change on a object
//        /// </summary>
//        /// <param name="myObjectLocation">The location where something happend</param>
//        /// <param name="myObjectLocator">The new ObjectLocator</param>
//        /// <param name="myINode">The new INode</param>
//        public void AddDetail(String myObjectLocation, ObjectLocator myObjectLocator, INode myINode)
//        {
//            if ((myObjectLocator != null && myObjectLocator.TransactionUUID != null && myObjectLocator.TransactionUUID != this.TransactionUUID)
//                || (myINode != null && myINode.TransactionUUID != null && myINode.TransactionUUID != this.TransactionUUID))
//            {
//                throw new GraphFSException_LocationAlreadyHoldTransaction(myObjectLocation);
//            }

//            if (_TransactionDetails.ContainsKey(myObjectLocation))
//            {
//                if (myObjectLocator != null)
//                    _TransactionDetails[myObjectLocation].ObjectLocator = myObjectLocator;
//                if (myINode != null)
//                    _TransactionDetails[myObjectLocation].INode = myINode;
//            }
//            else
//            {
//                _TransactionDetails.Add(myObjectLocation, new TransactionDetail() { ObjectLocator = myObjectLocator, INode = myINode });
//            }

//            if (myObjectLocator != null)
//                myObjectLocator.TransactionUUID = this.TransactionUUID;

//            if (myINode != null)
//                myINode.TransactionUUID = this.TransactionUUID;
//        }

//        /// <summary>
//        /// Get the transaction details for this location
//        /// </summary>
//        /// <param name="myObjectLocation">The ObjectLocation</param>
//        /// <returns>The transaction details</returns>
//        public TransactionDetail GetDetail(String myObjectLocation)
//        {
//            if (!_TransactionDetails.ContainsKey(myObjectLocation))
//                throw new GraphFSException_NoTransactionFound(myObjectLocation);

//            return _TransactionDetails[myObjectLocation];
//        }

//        /// <summary>
//        /// Check for pending transaction on this ObjectLocator
//        /// </summary>
//        /// <param name="myObjectLocation">The ObjectLocation</param>
//        /// <returns>True, if this location contains some pending transactions</returns>
//        public Boolean HasObjectLocatorTransaction(String myObjectLocation)
//        {
//            if (_TransactionDetails.ContainsKey(myObjectLocation))
//                if (_TransactionDetails[myObjectLocation].ObjectLocator != null)
//                    return true;

//            return false;
//        }

//        /// <summary>
//        /// Check for pending transaction on this location. Without checking the ObjectLocator
//        /// </summary>
//        /// <param name="myObjectLocation">The ObjectLocation</param>
//        /// <returns>True, if this location contains some pending transactions</returns>
//        public Boolean HasDetail(String myObjectLocation)
//        {
//            return _TransactionDetails.ContainsKey(myObjectLocation);
//        }

//        /// <summary>
//        /// Mark this transaction as committed. Will not invoke the OnDispose event to clean up the ressources
//        /// </summary>
//        public void Commit()
//        {
//            _TransactionState = TransactionState.Committed;
//        }

//        /// <summary>
//        /// Mark this transaction as rolledback. Will invoke the event OnDispose to clean up ressources.
//        /// </summary>
//        public void Rollback()
//        {
//            _TransactionState = TransactionState.Rolledback;
//        }

//        /// <summary>
//        /// Remove (rollback) the complete transaction with all containing details.
//        /// </summary>
//        public override void Dispose()
//        {

//            base.Dispose();

//            if (_TransactionState != TransactionState.Committed)
//            {

//                if (OnDispose != null)
//                    OnDispose(this, new TransactionDisposedEventArgs(this, _SessionTokenReference));
//                else
//                    throw new Exception("Dispose event not registered. Can't clean up PreAllocated ressources!");

//            }

//            _TransactionDetails = null;
//            //_SessionTokenReference.RemoveTransaction(this);

//        }

//        public override string ToString()
//        {
//            String retval = String.Concat("TransactionDetails (", TransactionDetails.Count, ") ");
//            foreach (String key in _TransactionDetails.Keys)
//                retval += " [ " + key + " ] ";

//            return retval;
//        }

//    }
//}
