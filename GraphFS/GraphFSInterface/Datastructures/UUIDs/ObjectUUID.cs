/*
 * ObjectUUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;

using sones.Lib;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public class ObjectUUID : UUID, IComparable, IComparable<ObjectUUID>, IEquatable<ObjectUUID>
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 231; } }

        #endregion

        #region Constructors

        #region ObjectUUID()

        public ObjectUUID()
            : this(Guid.NewGuid().ToString())
        { }

        #endregion

        #region ObjectUUID(myUInt64)

        public ObjectUUID(UInt64 myUInt64)
            : this(myUInt64.ToString())
        { }

        #endregion

        #region ObjectUUID(myString)

        /// <summary>
        /// Create a new ObjectUUID parsing the <paramref name="myString"/> as hex string.
        /// Do NOT use this for own created ObjectUUIDs!! User ObjectUUID.FromString instead!
        /// </summary>
        /// <param name="myString"></param>
        public ObjectUUID(String myString)
        {
            _UUID = Encoding.UTF8.GetBytes(myString);
        }

        #endregion

        #region ObjectUUID(mySerializedData)

        public ObjectUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion


        #region NewUUID

        public new static ObjectUUID NewUUID
        {
            get
            {
                return new ObjectUUID(Guid.NewGuid().ToString());
            }
        }

        #endregion


        #region Statics

        public static ObjectUUID FromString(String myString)
        {
            return new ObjectUUID(Encoding.UTF8.GetBytes(myString));
        }

        #endregion


        #region Operator overloading

        #region Operator == (myObjectUUID1, myObjectUUID2)

        public static Boolean operator == (ObjectUUID myObjectUUID1, ObjectUUID myObjectUUID2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myObjectUUID1, myObjectUUID2))
                return true;

            // If one is null, but not both, return false.
            if (( (Object) myObjectUUID1 == null) || ( (Object) myObjectUUID2 == null))
                return false;

            return myObjectUUID1.Equals(myObjectUUID2);

        }

        #endregion

        #region Operator != (myObjectUUID1, myObjectUUID2)

        public static Boolean operator != (ObjectUUID myObjectUUID1, ObjectUUID myObjectUUID2)
        {
            return !(myObjectUUID1 == myObjectUUID2);
        }

        #endregion

        #region Operator < (myObjectUUID1, myObjectUUID2)

        public static Boolean operator < (ObjectUUID myObjectUUID1, ObjectUUID myObjectUUID2)
        {

            // Check if myObjectUUID1 is null
            if ( (Object) myObjectUUID1 == null)
                throw new ArgumentNullException("Parameter myObjectUUID1 must not be null!");

            // Check if myObjectUUID2 is null
            if ( (Object) myObjectUUID2 == null)
                throw new ArgumentNullException("Parameter myObjectUUID2 must not be null!");


            // Check the length of the arrays
            if (myObjectUUID1.Length < myObjectUUID2.Length)
                return true;

            if (myObjectUUID1.Length > myObjectUUID2.Length)
                return false;


            Byte[] _ObjectUUID1 = myObjectUUID1.GetByteArray();
            Byte[] _ObjectUUID2 = myObjectUUID2.GetByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ( (UInt64) myObjectUUID1.Length); i++)
            {
                if (_ObjectUUID1[i] < _ObjectUUID2[i]) return true;
                if (_ObjectUUID1[i] > _ObjectUUID2[i]) return false;
            }

            return false;

        }

        #endregion

        #region Operator > (myObjectUUID1, myObjectUUID2)

        public static Boolean operator > (ObjectUUID myObjectUUID1, ObjectUUID myObjectUUID2)
        {

            // Check if myObjectUUID1 is null
            if ( (Object) myObjectUUID1 == null)
                throw new ArgumentNullException("Parameter myObjectUUID1 must not be null!");

            // Check if myObjectUUID2 is null
            if ( (Object) myObjectUUID2 == null)
                throw new ArgumentNullException("Parameter myObjectUUID2 must not be null!");


            // Check the length of the arrays
            if (myObjectUUID1.Length > myObjectUUID2.Length)
                return true;

            if (myObjectUUID1.Length < myObjectUUID2.Length)
                return false;


            Byte[] _ObjectUUID1 = myObjectUUID1.GetByteArray();
            Byte[] _ObjectUUID2 = myObjectUUID2.GetByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ( (UInt64) myObjectUUID1.Length); i++)
            {
                if (_ObjectUUID1[i] > _ObjectUUID2[i]) return true;
                if (_ObjectUUID1[i] < _ObjectUUID2[i]) return false;
            }

            return false;

        }

        #endregion

        #region Operator <= (myObjectUUID1, myObjectUUID2)

        public static Boolean operator <= (ObjectUUID myObjectUUID1, ObjectUUID myObjectUUID2)
        {
            return !(myObjectUUID1 > myObjectUUID2);
        }

        #endregion

        #region Operator >= (myObjectUUID1, myObjectUUID2)

        public static Boolean operator >= (ObjectUUID myObjectUUID1, ObjectUUID myObjectUUID2)
        {
            return !(myObjectUUID1 < myObjectUUID2);
        }

        #endregion

        #endregion


        #region Like ICloneable Members

        public new ObjectUUID Clone()
        {
            var newUUID = new ObjectUUID(_UUID);
            return newUUID;
        }

        #endregion

        #region IComparable Members

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an UUID object
            var myObjectUUID = myObject as ObjectUUID;
            if ( (Object) myObjectUUID == null)
                throw new ArgumentException("myObject is not of type ObjectUUID!");

            return CompareTo(myObjectUUID);

        }

        #endregion

        #region IComparable<ObjectUUID> Members

        public Int32 CompareTo(ObjectUUID myObjectUUID)
        {

            // Check if myUUID is null
            if (myObjectUUID == null)
                throw new ArgumentNullException("myObjectUUID must not be null!");

            if (this < myObjectUUID) return -1;
            if (this > myObjectUUID) return +1;

            return 0;

        }

        #endregion

        #region IEquatable<ObjectUUID> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("Parameter myObject must not be null!");

            // Check if myObject can be cast to UUID
            var myUUID = myObject as ObjectUUID;
            if ((Object) myUUID == null)
                throw new ArgumentException("Parameter myObject could not be casted to type ObjectUUID!");

            return this.Equals(myUUID);

        }

        #endregion

        #region Equals(myObjectUUID)

        public Boolean Equals(ObjectUUID myObjectUUID)
        {

            // Check if myObjectUUID is null
            if (myObjectUUID == null)
            {
                throw new ArgumentNullException("Parameter myObjectUUID must not be null!");
            }

            // Check if the arrays have the same length
            var myLength = _UUID.ULongLength();
            var anotherLength = myObjectUUID.Length;

            if (myLength != anotherLength)
                return false;

            if (this.GetHashCode() != myObjectUUID.GetHashCode())
            {
                return false;
            }

            // Check if the inner array of bytes have the same values
            var anotherUUIDArray = myObjectUUID.GetByteArray();

            for (var i = myLength; i > 0; i--)
            {
                if (_UUID[i - 1] != anotherUUIDArray[i - 1])
                {
                    return false;
                }
            }

            return true;

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {
            return Encoding.UTF8.GetString(_UUID);
        }

        #endregion

    }

}
