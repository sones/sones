using System;

namespace sones.Library.PropertyHyperGraph
{
    public sealed class VertexRevisionID : IComparable, IComparable<VertexRevisionID>, IEquatable<VertexRevisionID>
    {
        #region Properties

        #region Timestamp

        /// <summary>
        /// The timestamp of this revision.
        /// </summary>
        public readonly long Timestamp;

        #endregion

        #region UUID

        /// <summary>
        /// A unique identification of the generation process of this revision.
        /// </summary>
        public readonly UInt64 ID;

        #endregion

        #endregion

        #region Constructor

        #region RevisionID(myUUID, myTimeStamp)

        /// <summary>
        /// A constructor used for generating an RevisionID based on the actual
        /// DateTime and the given (system) UUID.
        /// </summary>
        /// <param name="myTimeStamp">Any timestamp</param> 
        /// <param name="myID">An unique identification for this generation process</param>
        public VertexRevisionID(long myTimeStamp, UInt64 myID = 0UL)
        {
            Timestamp = myTimeStamp;

            ID = myID;
        }

        #endregion

        #endregion

        #region Operator overloading

        #region Operator == (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator ==(VertexRevisionID myObjectRevisionID1, VertexRevisionID myObjectRevisionID2)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(myObjectRevisionID1, myObjectRevisionID2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myObjectRevisionID1 == null) || ((Object) myObjectRevisionID2 == null))
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
            return myObjectRevisionID1.Timestamp < myObjectRevisionID2.Timestamp;
        }

        #endregion

        #region Operator > (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator >(VertexRevisionID myObjectRevisionID1, VertexRevisionID myObjectRevisionID2)
        {
            return myObjectRevisionID1.Timestamp > myObjectRevisionID2.Timestamp;
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
            var revisionID = myObject as VertexRevisionID;
            if ((Object) revisionID == null)
                throw new ArgumentException("myObject is not of type RevisionID!");

            if (this < revisionID) return -1;
            if (this > revisionID) return +1;

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

        #region IEquatable<VertexRevisionID> Members

        public Boolean Equals(VertexRevisionID myObjectRevisionID)
        {
            // If parameter is null return false:
            if ((Object) myObjectRevisionID == null)
                return false;

            // Check if the inner fields have the same values
            if (Timestamp != myObjectRevisionID.Timestamp)
                return false;

            return ID == myObjectRevisionID.ID;
        }

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return Timestamp.GetHashCode() ^ ID.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Returns a formated string representation of this revision
        /// </summary>
        /// <returns>A formated string representation of this revision</returns>
        public override String ToString()
        {
            return String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", Timestamp, ID);
        }

        #endregion

        public override Boolean Equals(Object myObject)
        {
            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to RevisionID return false.
            var revisionID = myObject as VertexRevisionID;

            return (Object) revisionID != null && Equals(revisionID);
        }
    }
}