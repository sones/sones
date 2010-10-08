
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
