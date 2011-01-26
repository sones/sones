using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Transaction
{
    public sealed class TransactionID : IComparable, IComparable<TransactionID>, IEquatable<TransactionID>
    {

        #region Data

        public readonly String ID;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Generates a TransactionID based on the content of myString.
        /// </summary>
        public TransactionID(String myString = null)
        {
            if (myString == null)
	{
                ID = Guid.NewGuid().ToString();
	}
            else
	{
            ID = myString;

	}
        }

        #endregion

        #region Operator overloading

        #region Operator == (myTransactionID1, myTransactionID2)

        public static Boolean operator ==(TransactionID myTransactionID1, TransactionID myTransactionID2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myTransactionID1, myTransactionID2))
                return true;

            // If one is null, but not both, return false.
            if (((Object)myTransactionID1 == null) || ((Object)myTransactionID2 == null))
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
            if ((Object)myTransactionID1 == null)
                throw new ArgumentNullException("Parameter myTransactionID1 must not be null!");

            // Check if myTransactionID2 is null
            if ((Object)myTransactionID2 == null)
                throw new ArgumentNullException("Parameter myTransactionID2 must not be null!");


            // Check the length of the TransactionUUIDs
            if (myTransactionID1.ID.Length < myTransactionID2.ID.Length)
                return true;

            if (myTransactionID1.ID.Length > myTransactionID2.ID.Length)
                return false;

            return myTransactionID1.CompareTo(myTransactionID2) < 0;

        }

        #endregion

        #region Operator >  (myTransactionID1, myTransactionID2)

        public static Boolean operator >(TransactionID myTransactionID1, TransactionID myTransactionID2)
        {

            // Check if myTransactionID1 is null
            if ((Object)myTransactionID1 == null)
                throw new ArgumentNullException("Parameter myTransactionID1 must not be null!");

            // Check if myTransactionID2 is null
            if ((Object)myTransactionID2 == null)
                throw new ArgumentNullException("Parameter myTransactionID2 must not be null!");


            // Check the length of the TransactionUUIDs
            if (myTransactionID1.ID.Length > myTransactionID2.ID.Length)
                return true;

            if (myTransactionID1.ID.Length < myTransactionID2.ID.Length)
                return false;

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
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an TransactionUUID object
            var myTransactionUUID = myObject as TransactionID;
            if ((Object)myTransactionUUID == null)
                throw new ArgumentException("myObject is not of type TransactionID!");

            return CompareTo(myTransactionUUID);

        }

        #endregion

        #region IComparable<TransactionUUID> Members

        public Int32 CompareTo(TransactionID myTransactionUUID)
        {

            // Check if myTransactionUUID is null
            if (myTransactionUUID == null)
                throw new ArgumentNullException("myTransactionID must not be null!");

            return ID.CompareTo(myTransactionUUID.ID);

        }

        #endregion

        #region IEquatable<TransactionUUID> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("Parameter myObject must not be null!");

            // Check if myObject can be cast to TransactionUUID
            var myTransactionUUID = myObject as TransactionID;
            if ((Object)myTransactionUUID == null)
                throw new ArgumentException("Parameter myObject could not be casted to type TransactionID!");

            return this.Equals(myTransactionUUID);

        }

        #endregion

        #region Equals(myTransactionUUID)

        public Boolean Equals(TransactionID myTransactionUUID)
        {

            // Check if myTransactionUUID is null
            if (myTransactionUUID == null)
                throw new ArgumentNullException("Parameter myTransactionUUID must not be null!");

            return ID.Equals(myTransactionUUID.ID);

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return ID.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {
            return ID;
        }

        #endregion

    }
}
