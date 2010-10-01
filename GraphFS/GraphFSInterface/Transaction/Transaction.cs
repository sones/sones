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


//#region Usings

//using System;
//using System.Text;
//using sones.Lib.DataStructures.UUID;
//using sones.GraphFS.Session;
//using sones.Lib.DataStructures.Timestamp;

//#endregion

//namespace sones.GraphFS.Transactions
//{

//    /// <summary>
//    /// This is the basic Transaction class containing some basic information about a transaction.
//    /// Derive this class for specific transactions
//    /// </summary>

//    public class Transaction : IDisposable
//    {

//        public UUID TransactionUUID
//        {
//            get { return _TransactionUUID; }
//        }

//        private UUID _TransactionUUID;


//        public Boolean IsAutocommit
//        {
//            get { return _IsAutocommit; }
//            set { _IsAutocommit = value; }
//        }

//        private Boolean _IsAutocommit;




//        private DateTime _Created;

//        protected SessionToken _SessionTokenReference;

//        public Transaction(SessionToken mySessionTokenReference)
//        {
//            _TransactionUUID = new UUID();
//            _Created = TimestampNonce.Now;
//            _SessionTokenReference = mySessionTokenReference;
//        }


//        #region IDisposable Members

//        public virtual void Dispose() { }

//        #endregion

//    }

//}
