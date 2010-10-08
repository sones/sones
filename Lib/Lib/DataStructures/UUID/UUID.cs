/*
 * UUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using sones.Lib;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

#endregion

namespace sones.Lib.DataStructures.UUID
{

    /// <summary>
    /// An UUID is universal unique identificator.
    /// </summary>    

    public class UUID : ASurrogateUUID, IComparable, IComparable<UUID>, IEquatable<UUID>, IFastSerialize, IEstimable
    {


        #region TypeCode
        public override UInt32 TypeCode { get { return 200; } }
        #endregion

        #region Data

        protected Byte[] _UUID;

        #endregion

        #region Properties

        #region Length

        public UInt64 Length
        {
            get
            {
                return _UUID.ULongLength();
            }
        }

        #endregion

        #endregion


        #region Constructors

        #region UUID()

        /// <summary>
        /// Generates a new UUID
        /// </summary>
        public UUID()
        {
            //_UUID = new Byte[0];
            _UUID = Guid.NewGuid().ToByteArray();
        }

        #endregion

        #region UUID(myInt32)

        /// <summary>
        /// Generates a UUID based on the content of an Int32
        /// </summary>
        /// <param name="myInt32">A Int32</param>
        public UUID(Int32 myInt32)
            : this(Math.Abs(myInt32).ToString())
        {
        }

        #endregion

        #region UUID(myUInt64)

        /// <summary>
        /// Generates a UUID based on the content of an UInt64
        /// </summary>
        /// <param name="myUInt64">A UInt64</param>
        public UUID(UInt64 myUInt64)
            : this(myUInt64.ToString())
        {
        }

        #endregion

        #region UUID(myString)

        /// <summary>
        /// Generates a UUID based on the content of myString.
        /// </summary>
        /// <param name="myString">A string</param>
        public UUID(String myString)
        {
            _UUID = ByteArrayHelper.FromHexString(myString);
        }

        #endregion

        #region UUID(mySerializedData)

        public UUID(Byte[] mySerializedData)
        {
            _UUID = mySerializedData;
        }

        #endregion

        #region UUID(ref mySerializationReader)

        public UUID(ref SerializationReader mySerializationReader)
        {
            Deserialize(ref mySerializationReader);
        }

        #endregion

        #region UUID(myUUID)

        /// <summary>
        /// Generates a UUID based on the content of myUUID
        /// </summary>
        /// <param name="myUUID">A UUID</param>
        public UUID(UUID myUUID)
        {
            var _ByteArray = myUUID.GetByteArray();
            _UUID = new Byte[_ByteArray.LongLength];
            Array.Copy(_ByteArray, 0, _UUID, 0, _ByteArray.LongLength);
        }

        #endregion

        #endregion


        #region Object-specific methods

        #region GetByteArray()

        public Byte[] GetByteArray()
        {
            return _UUID;
        }

        #endregion

        #endregion


        #region NewUUID

        public static UUID NewUUID
        {
            get
            {
                return new UUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (myUUID1, myUUID2)

        public static Boolean operator == (UUID myUUID1, UUID myUUID2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myUUID1, myUUID2))
                return true;

            // If one is null, but not both, return false.
            if (( (Object) myUUID1 == null) || ( (Object) myUUID2 == null))
                return false;

            return myUUID1.Equals(myUUID2);

        }

        #endregion

        #region Operator != (myUUID1, myUUID2)

        public static Boolean operator != (UUID myUUID1, UUID myUUID2)
        {
            return !(myUUID1 == myUUID2);
        }

        #endregion

        #region Operator < (myUUID1, myUUID2)

        public static Boolean operator < (UUID myUUID1, UUID myUUID2)
        {

            // Check if myUUID1 is null
            if ( (Object) myUUID1 == null)
                throw new ArgumentNullException("Parameter myUUID1 must not be null!");

            // Check if myUUID2 is null
            if ( (Object) myUUID2 == null)
                throw new ArgumentNullException("Parameter myUUID2 must not be null!");


            // Check the length of the arrays
            if (myUUID1.Length < myUUID2.Length)
                return true;

            if (myUUID1.Length > myUUID2.Length)
                return false;


            Byte[] _UUID1 = myUUID1.GetByteArray();
            Byte[] _UUID2 = myUUID2.GetByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ( (UInt64) myUUID1.Length); i++)
            {
                if (_UUID1[i] < _UUID2[i]) return true;
                if (_UUID1[i] > _UUID2[i]) return false;
            }

            return false;

        }

        #endregion

        #region Operator > (myUUID1, myUUID2)

        public static Boolean operator > (UUID myUUID1, UUID myUUID2)
        {

            // Check if myUUID1 is null
            if ( (Object) myUUID1 == null)
                throw new ArgumentNullException("Parameter myUUID1 must not be null!");

            // Check if myUUID2 is null
            if ( (Object) myUUID2 == null)
                throw new ArgumentNullException("Parameter myUUID2 must not be null!");


            // Check the length of the arrays
            if (myUUID1.Length > myUUID2.Length)
                return true;

            if (myUUID1.Length < myUUID2.Length)
                return false;


            Byte[] _UUID1 = myUUID1.GetByteArray();
            Byte[] _UUID2 = myUUID2.GetByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ( (UInt64) myUUID1.Length); i++)
            {
                if (_UUID1[i] > _UUID2[i]) return true;
                if (_UUID1[i] < _UUID2[i]) return false;
            }

            return false;

        }

        #endregion

        #region Operator <= (myUUID1, myUUID2)

        public static Boolean operator <= (UUID myUUID1, UUID myUUID2)
        {
            return !(myUUID1 > myUUID2);
        }

        #endregion

        #region Operator >= (myUUID1, myUUID2)

        public static Boolean operator >= (UUID myUUID1, UUID myUUID2)
        {
            return !(myUUID1 < myUUID2);
        }

        #endregion

        #endregion


        #region IComparable Members

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an UUID object
            var myUUID = myObject as UUID;
            if ( (Object) myUUID == null)
                throw new ArgumentException("myObject is not of type UUID!");

            return CompareTo(myUUID);

        }

        #endregion

        #region IComparable<UUID> Members

        public Int32 CompareTo(UUID myUUID)
        {

            // Check if myUUID is null
            if (myUUID == null)
                throw new ArgumentNullException("myUUID must not be null!");

            if (this < myUUID) return -1;
            if (this > myUUID) return +1;

            return 0;

        }

        #endregion

        #region IEquatable<UUID> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
            {
                return false;
                //throw new ArgumentNullException("Parameter myObject must not be null!");
            }

            // Check if myObject can be cast to UUID
            var myUUID = myObject as UUID;
            if ((Object) myUUID == null)
                throw new ArgumentException("Parameter myObject could not be casted to type UUID!");

            return this.Equals(myUUID);

        }

        #endregion

        #region Equals(myUUID)

        public Boolean Equals(UUID anotherUUID)
        {

            // Check if myUUID is null
            if (anotherUUID == null)
            {
                throw new ArgumentNullException("Parameter myUUID must not be null!");
            }

            // Check if the arrays have the same length
            var myLength = _UUID.ULongLength();
            var anotherLength = anotherUUID.Length;

            if (myLength != anotherLength)
                return false;

            if (this.GetHashCode() != anotherUUID.GetHashCode())
            {
                return false;
            }

            // Check if the inner array of bytes have the same values
            var anotherUUIDArray = anotherUUID.GetByteArray();

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


        #region Like ICloneable Members

        public UUID Clone()
        {
            return new UUID(_UUID);
        }

        #endregion


        #region IFastSerialize Members

        #region isDirty

        private Boolean _isDirty = false;

        public Boolean isDirty
        {

            get
            {
                return _isDirty;
            }

            set
            {
                _isDirty = value;
            }

        }

        #endregion

        #region ModificationTime

        public DateTime ModificationTime
        {

            get
            {
                throw new NotImplementedException();
            }

        }

        #endregion

        //#region Serialize()

        //public Byte[] Serialize()
        //{
        //    return _UUID;
        //}

        //#endregion

        //#region Deserialize(myData)

        //public void Deserialize(Byte[] myData)
        //{
        //    _UUID = myData;
        //}

        //#endregion

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            Serialize(ref mySerializationWriter, this);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            Deserialize(ref mySerializationReader, this);
        }

        private void Serialize(ref SerializationWriter mySerializationWriter, UUID myValue)
        {
            mySerializationWriter.Write(myValue._UUID);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, UUID myValue)
        {
            myValue._UUID = mySerializationReader.ReadByteArray();
            return myValue;
        }

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public override bool SupportsType(Type type)
        {
            return type == this.GetType();
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (UUID)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            UUID thisObject = (UUID)Activator.CreateInstance(type);

            return Deserialize(ref reader, thisObject);
        }

        #endregion


        #region GetHashCode()
        private Int32 _hashcode = 0;

        public override Int32 GetHashCode()
        {
            if (_hashcode == 0)
            {
                int byteIndex = 0;
                int num1;
                int num2 = 0x1505;
                int num3 = num2;
                byte currentByte;

                while (byteIndex < _UUID.Length)
                {
                    currentByte = _UUID[byteIndex];
                    num1 = (int)currentByte;
                    num2 = ((num2 << 5) + num2) ^ num1;

                    if (byteIndex == _UUID.Length - 1)
                        break;

                    num1 = _UUID[byteIndex + 1];
                    if (num1 == 0)
                    {
                        break;
                    }
                    num3 = ((num3 << 5) + num3) ^ num1;
                    byteIndex += 2;
                }

                _hashcode = (num2 + (num3 * 0x5d588b65));
            }

            return _hashcode;
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Returns a formated string representation of this revision as HEX
        /// </summary>
        /// <returns>A formated string representation of this revision</returns>
        public override String ToString()
        {
            return ToString(SeperatorTypes.NONE);
        }

        #endregion

        #region ToString(mySeperatorTypes)

        /// <summary>
        /// Returns a formated string representation of this revision as HEX
        /// </summary>
        /// <returns>A formated string representation of this revision</returns>
        public String ToString(SeperatorTypes mySeperatorTypes)
        {
            return _UUID.ToHexString(mySeperatorTypes);
        }

        #endregion

        public ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.ClassDefaultSize + _UUID.ULongLength() + EstimatedSizeConstants.UInt32 + EstimatedSizeConstants.Int32;
        }
    }

}
