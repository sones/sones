using System;

namespace sones.Library.Transaction
{
    public sealed class TransactionID : IComparable, IComparable<TransactionID>, IEquatable<TransactionID>
    {
        #region Data

        public readonly UInt64 ID;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a TransactionID
        /// </summary>
        public TransactionID(UInt64 myID)
        {
            ID = myID;
        }

        #endregion

        #region Operator overloading

        #region Operator == (myTransactionID1, myTransactionID2)

        public static Boolean operator ==(TransactionID myTransactionID1, TransactionID myTransactionID2)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(myTransactionID1, myTransactionID2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myTransactionID1 == null) || ((Object) myTransactionID2 == null))
                return false;

            return myTransactionID1.Equals(myTransactionID2);
        }

        #endregion

        #region Operator != (myTransactionID1, myTransactionID2)

        public static Boolean operator !=(TransactionID myTransactionID1, TransactionID myTransactionID2)
        {
            return !(myTransactionID1 == myTransactionID2);
        }

        #endregion

        #region Operator <  (myTransactionID1, myTransactionID2)

        public static Boolean operator <(TransactionID myTransactionID1, TransactionID myTransactionID2)
        {
            // Check if myTransactionID1 is null
            if (myTransactionID1 == null)
                throw new ArgumentNullException("myTransactionID1");

            // Check if myTransactionID2 is null
            if (myTransactionID2 == null)
                throw new ArgumentNullException("myTransactionID2");

            return myTransactionID1.CompareTo(myTransactionID2) < 0;
        }

        #endregion

        #region Operator >  (myTransactionID1, myTransactionID2)

        public static Boolean operator >(TransactionID myTransactionID1, TransactionID myTransactionID2)
        {
            // Check if myTransactionID1 is null
            if (myTransactionID1 == null)
                throw new ArgumentNullException("myTransactionID1");

            // Check if myTransactionID2 is null
            if (myTransactionID2 == null)
                throw new ArgumentNullException("myTransactionID2");

            return myTransactionID1.CompareTo(myTransactionID2) > 0;
        }

        #endregion

        #region Operator <= (myTransactionID1, myTransactionID2)

        public static Boolean operator <=(TransactionID myTransactionID1, TransactionID myTransactionID2)
        {
            return !(myTransactionID1 > myTransactionID2);
        }

        #endregion

        #region Operator >= (myTransactionID1, myTransactionID2)

        public static Boolean operator >=(TransactionID myTransactionID1, TransactionID myTransactionID2)
        {
            return !(myTransactionID1 < myTransactionID2);
        }

        #endregion

        #endregion

        #region IComparable Members

        public Int32 CompareTo(Object myObject)
        {
            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject");

            // Check if myObject can be casted to an TransactionUUID object
            var myTransactionUUID = myObject as TransactionID;
            if (myTransactionUUID == null)
                throw new ArgumentException("myTransactionUUID");

            return CompareTo(myTransactionUUID);
        }

        #endregion

        #region IComparable<TransactionID> Members

        public Int32 CompareTo(TransactionID myTransactionUUID)
        {
            // Check if myTransactionUUID is null
            if (myTransactionUUID == null)
                throw new ArgumentNullException("myTransactionUUID");

            return ID.CompareTo(myTransactionUUID.ID);
        }

        #endregion

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {
            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject");

            // Check if myObject can be cast to TransactionUUID
            var myTransactionUUID = myObject as TransactionID;
            if (myTransactionUUID == null)
                throw new ArgumentException("myTransactionUUID");

            return Equals(myTransactionUUID);
        }

        #endregion

        #region Equals(myTransactionUUID)

        public Boolean Equals(TransactionID myTransactionUUID)
        {
            // Check if myTransactionUUID is null
            if (myTransactionUUID == null)
                throw new ArgumentNullException("myTransactionUUID");

            return ID.Equals(myTransactionUUID.ID);
        }

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return ID.GetHashCode();
        }

        #endregion

        #region ToString()

        public override string ToString()
        {
            return ID.ToString();
        }

        #endregion
    }
}