/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* PandoraFS - PublicKey
 * Achim Friedland, 2009
 *  
 * A PublicKey is a class for storing any sort of public keys.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using sones.Lib.Serializer;
using System.Runtime.Serialization;
using sones.Lib;
using sones.Lib.NewFastSerializer;
using sones.Lib;

#endregion

namespace sones.Lib.DataStructures.PublicKey
{

    /// <summary>
    /// A PublicKey is a class for storing any sort of public keys.
    /// </summary>
    public class PublicKey : IFastSerialize, IComparable, IComparable<PublicKey>
    {

        
        #region Data

        protected Byte[] _PublicKey;

        #endregion


        #region Constructors

        #region PublicKey()

        public PublicKey()
        {
            _PublicKey = new Byte[0];
        }

        #endregion

        #region PublicKey(myInt32)

        public PublicKey(Int32 myInt32)
        {
            _PublicKey = BitConverter.GetBytes(myInt32);
        }

        #endregion

        #region PublicKey(mySerializedData)

        public PublicKey(Byte[] mySerializedData)
        {
            _PublicKey = mySerializedData;
        }

        #endregion

        #endregion


        #region Object-specific methods and properties

        #region Length

        public UInt64 Length
        {

            get
            {
                return (UInt64) _PublicKey.LongLength;
            }

        }

        #endregion


        #region ToByteArray()

        public Byte[] ToByteArray()
        {
            return _PublicKey;
        }

        #endregion


        #region Operator overloading

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to Point return false.
            PublicKey myPublicKey = myObject as PublicKey;
            if ( (Object) myPublicKey == null)
                return false;

            // Check if the arrays have the same length
            if ( (UInt64) _PublicKey.LongLength != myPublicKey.Length)
                return false;

            Byte[] myPublicKey_Array = myPublicKey.ToByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i=0; i<((UInt64) _PublicKey.LongLength); i++)
                if (_PublicKey[i] != myPublicKey_Array[i])
                    return false;

            return true;

        }

        #endregion

        #region Equals(myPublicKey)

        public Boolean Equals(PublicKey myPublicKey)
        {

            // If parameter is null return false:
            if ( (Object) myPublicKey == null)
                return false;

            // Check if the arrays have the same length
            if ((UInt64)_PublicKey.LongLength != myPublicKey.Length)
                return false;

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ((UInt64)_PublicKey.LongLength); i++)
                if (_PublicKey[i] != myPublicKey.ToByteArray()[i])
                    return false;

            return true;

        }

        #endregion

        #region Operator == (myPublicKey1, myPublicKey2)

        public static Boolean operator == (PublicKey myPublicKey1, PublicKey myPublicKey2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myPublicKey1, myPublicKey2))
                return true;

            // If one is null, but not both, return false.
            if (( (Object) myPublicKey1 == null) || ( (Object) myPublicKey2 == null))
                return false;

            return myPublicKey1.Equals(myPublicKey2);

        }

        #endregion

        #region Operator != (myPublicKey1, myPublicKey2)

        public static Boolean operator != (PublicKey myPublicKey1, PublicKey myPublicKey2)
        {
            return !myPublicKey1.Equals(myPublicKey2);
        }

        #endregion

        #region Operator < (myPublicKey1, myPublicKey2)

        public static Boolean operator < (PublicKey myPublicKey1, PublicKey myPublicKey2)
        {

            // Check the length of the arrays
            if (myPublicKey1.Length < myPublicKey2.Length)
                return true;

            if (myPublicKey1.Length > myPublicKey2.Length)
                return false;

            Byte[] _PublicKey1 = myPublicKey1.ToByteArray();
            Byte[] _PublicKey2 = myPublicKey1.ToByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ( (UInt64) myPublicKey1.Length); i++)
            {
                if (_PublicKey1[i] < _PublicKey2[i]) return true;
                if (_PublicKey1[i] > _PublicKey2[i]) return false;
            }

            return false;

        }

        #endregion

        #region Operator > (myPublicKey1, myPublicKey2)

        public static Boolean operator > (PublicKey myPublicKey1, PublicKey myPublicKey2)
        {
            return !(myPublicKey1 < myPublicKey2);
        }

        #endregion

        #region Operator <= (myPublicKey1, myPublicKey2)

        public static Boolean operator <= (PublicKey myPublicKey1, PublicKey myPublicKey2)
        {

            // Check the length of the arrays
            if (myPublicKey1.Length < myPublicKey2.Length)
                return true;

            if (myPublicKey1.Length > myPublicKey2.Length)
                return false;

            Byte[] _PublicKey1 = myPublicKey1.ToByteArray();
            Byte[] _PublicKey2 = myPublicKey1.ToByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ( (UInt64) myPublicKey1.Length); i++)
            {
                if (_PublicKey1[i] < _PublicKey2[i]) return true;
                if (_PublicKey1[i] > _PublicKey2[i]) return false;
            }

            return true;

        }

        #endregion

        #region Operator >= (myPublicKey1, myPublicKey2)

        public static Boolean operator >= (PublicKey myPublicKey1, PublicKey myPublicKey2)
        {

            // Check the length of the arrays
            if (myPublicKey1.Length < myPublicKey2.Length)
                return false;

            if (myPublicKey1.Length > myPublicKey2.Length)
                return true;

            Byte[] _PublicKey1 = myPublicKey1.ToByteArray();
            Byte[] _PublicKey2 = myPublicKey1.ToByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ((UInt64)myPublicKey1.Length); i++)
            {
                if (_PublicKey1[i] < _PublicKey2[i]) return false;
                if (_PublicKey1[i] > _PublicKey2[i]) return true;
            }

            return true;

        }

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return _PublicKey.GetHashCode();
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
            PublicKey myPublicKey = myObject as PublicKey;
            if ( (Object) myPublicKey == null)
                throw new ArgumentException("myObject is not of type PublicKey!");

            if (this < (PublicKey) myObject) return -1;
            if (this > (PublicKey) myObject) return +1;

            return 0;

        }

        #endregion

        #region IComparable<PublicKey> Member

        public Int32 CompareTo(PublicKey myPublicKey)
        {

             // Check if myObject is null
            if (myPublicKey == null)
                throw new ArgumentNullException();

            if (this < myPublicKey) return -1;
            if (this > myPublicKey) return +1;

            return  0;

        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return _PublicKey.ToHexString();
        }

        #endregion

        #region ToHexString(mySeperatorTypes)

        public String ToHexString(SeperatorTypes mySeperatorTypes)
        {
            return _PublicKey.ToHexString(mySeperatorTypes);
        }

        #endregion

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

        #region Serialize()

        public Byte[] Serialize()
        {
            return _PublicKey;
        }

        #endregion

        #region Deserialize(myData)

        public void Deserialize(Byte[] myData)
        {
            _PublicKey = myData;
        }

        #endregion

        #endregion



        #region IFastSerialize Members


        public byte[] Serialize(SerializationWriter mySerializationWriter)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(SerializationReader mySerializationReader)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFastSerialize Members


        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}