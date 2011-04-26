using System;

namespace sones.Library.Commons.Transaction
{
    /// <summary>
    /// A class that contains informations concerning the current Transaction
    /// </summary>
    [Serializable]
    public sealed class TransactionToken
    {
        #region Data

        private TransactionID _iID;
        /// <summary>
        /// The ID of the current transaction token
        /// </summary>
        public TransactionID ID
        {
            set {}
            get {
                return _iID;
                }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new transaction token
        /// </summary>
        /// <param name="myTransactionID">The ID of the token</param>
        public TransactionToken(Int64 myTransactionID)
        {
            _iID = new TransactionID(myTransactionID);
        }

        #endregion

    }
}