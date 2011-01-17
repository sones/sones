using System;
using System.Threading;

namespace GraphFS.Element
{
    public sealed class VertexRevisionID : IComparable, IComparable<VertexRevisionID>, IEquatable<VertexRevisionID>
    {

        #region Properties

        #region Timestamp

        /// <summary>
        /// The timestamp of this revision.
        /// </summary>
        public UInt64 Timestamp { get; private set; }

        #endregion

        #region UUID

        /// <summary>
        /// A unique identification of the generation process of this revision.
        /// </summary>
        public Guid UUID { get; private set; }

        #endregion

        #endregion

        #region Constructor

        #region RevisionID(myUUID, myTimeStamp)

        /// <summary>
        /// A constructor used for generating an RevisionID based on the actual
        /// DateTime and the given (system) UUID.
        /// </summary>
        /// <param name="myUUID">An unique identification for this generation process</param>
        /// <param name="myTimestamp">Any timestamp</param> 
        public VertexRevisionID(Guid myUUID, UInt64 myTimeStamp = 0UL)
        {
            Timestamp = myTimeStamp;
            UUID = myUUID;
        }

        #endregion

        #endregion

        #region Operator overloading

        #region Operator == (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator ==(VertexRevisionID myObjectRevisionID1, VertexRevisionID myObjectRevisionID2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myObjectRevisionID1, myObjectRevisionID2))
                return true;

            // If one is null, but not both, return false.
            if (((Object)myObjectRevisionID1 == null) || ((Object)myObjectRevisionID2 == null))
                return false;

            return myObjectRevisionID1.Equals(myObjectRevisionID2);

        }

        #endregion

        #region Operator != (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator !=(VertexRevisionID myObjectRevisionID1, VertexRevisionID myObjectRevisionID2)
        {
            return !(myObjectRevisionID1 == myObjectRevisionID2);
        }

        #endregion

        #region Operator < (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator <(VertexRevisionID myObjectRevisionID1, VertexRevisionID myObjectRevisionID2)
        {

            if (myObjectRevisionID1.Timestamp < myObjectRevisionID2.Timestamp)
                return true;

            if (myObjectRevisionID1.Timestamp > myObjectRevisionID2.Timestamp)
                return false;

            return false;

        }

        #endregion

        #region Operator > (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator >(VertexRevisionID myObjectRevisionID1, VertexRevisionID myObjectRevisionID2)
        {

            if (myObjectRevisionID1.Timestamp > myObjectRevisionID2.Timestamp)
                return true;

            if (myObjectRevisionID1.Timestamp < myObjectRevisionID2.Timestamp)
                return false;

            return false;

        }

        #endregion

        #region Operator <= (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator <=(VertexRevisionID myObjectRevisionID1, VertexRevisionID myObjectRevisionID2)
        {
            return !(myObjectRevisionID1 > myObjectRevisionID2);
        }

        #endregion

        #region Operator >= (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator >=(VertexRevisionID myObjectRevisionID1, VertexRevisionID myObjectRevisionID2)
        {
            return !(myObjectRevisionID1 < myObjectRevisionID2);
        }

        #endregion

        #endregion


        #region IComparable Member

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException();

            // If parameter cannot be cast to Point return false.
            var _RevisionID = myObject as VertexRevisionID;
            if ((Object)_RevisionID == null)
                throw new ArgumentException("myObject is not of type RevisionID!");

            if (this < _RevisionID) return -1;
            if (this > _RevisionID) return +1;

            return 0;

        }

        #endregion

        #region IComparable<ObjectRevisionID> Member

        public Int32 CompareTo(VertexRevisionID myObjectRevisionID)
        {

            // Check if myObjectRevisionID is null
            if (myObjectRevisionID == null)
                throw new ArgumentNullException();

            if (this < myObjectRevisionID) return -1;
            if (this > myObjectRevisionID) return +1;

            return 0;

        }

        #endregion

        #region IEquatable Members

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to RevisionID return false.
            var _RevisionID = myObject as VertexRevisionID;
            if ((Object)_RevisionID == null)
                return false;

            return Equals(_RevisionID);

        }

        #endregion

        #region IEquatable<ObjectRevisionID> Members

        public Boolean Equals(VertexRevisionID myObjectRevisionID)
        {

            // If parameter is null return false:
            if ((Object)myObjectRevisionID == null)
                return false;

            // Check if the inner fields have the same values
            if (Timestamp != myObjectRevisionID.Timestamp)
                return false;

            if (UUID != myObjectRevisionID.UUID)
                return false;

            return true;

        }

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return Timestamp.GetHashCode() ^ UUID.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Returns a formated string representation of this revision
        /// </summary>
        /// <returns>A formated string representation of this revision</returns>
        public override String ToString()
        {
            return String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime((Int64)Timestamp), UUID.ToString());
        }

        #endregion

    }
}
