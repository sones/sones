using System;

namespace sones.Library.Transaction
{
    /// <summary>
    /// A class that contains informations concerning the current Transaction
    /// </summary>
    public sealed class TransactionToken
    {
        #region Data

        /// <summary>
        /// The ID of the current transaction token
        /// </summary>
        public readonly TransactionID ID;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new transaction token
        /// </summary>
        /// <param name="myTransactionID">The ID of the token</param>
        public TransactionToken(Int64 myTransactionID)
        {
            ID = new TransactionID(myTransactionID);
        }

        #endregion

    }
}