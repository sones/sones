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


/* PandoraFS - PasswordHash
 * Achim Friedland, 2009
 *  
 * A PasswordHash is a hashed password.
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
using sones.Lib.Cryptography.IntegrityCheck;
using System.Threading;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.NewFastSerializer;
using sones.Lib;

#endregion

namespace sones.Lib.DataStructures.PasswordHash
{

    /// <summary>
    /// A PasswordHash is a hashed password.
    /// </summary>
    
    
    [AllowNonEmptyConstructor]
    public class PasswordHash : IFastSerialize, IComparable, IComparable<PasswordHash>
    {

        
        #region Data

        protected IntegrityCheckTypes _HashingAlgorithm;
        protected Byte[]              _Seed;
        protected Byte[]              _PasswordHash;

        protected DateTime            _LastPasswordCheck;

        protected const Double        MIN_SECONDS_BETWEEN_PASSWORD_CHECKS = 120;

        #endregion


        #region Constructors

        #region PasswordHash
        public PasswordHash()
        {
            _Seed = BitConverter.GetBytes(new Random().Next());
            _PasswordHash = Encoding.UTF8.GetBytes("");
            _LastPasswordCheck = DateTime.MinValue;
        }
        #endregion

        #region PasswordHash(myString)

        public PasswordHash(String myString)
        {
            _Seed               = BitConverter.GetBytes(new Random().Next());
            _PasswordHash       = Encoding.UTF8.GetBytes(myString);
            _LastPasswordCheck  = DateTime.MinValue;
        }

        #endregion

        #region PasswordHash(mySerializedData)

        public PasswordHash(Byte[] mySerializedData)
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData);

        }

        #endregion

        #endregion


        #region Object-specific methods and properties

        #region Length

        public UInt64 Length
        {

            get
            {
                return (UInt64) _PasswordHash.LongLength;
            }

        }

        #endregion

        #region ToByteArray()

        public Byte[] ToByteArray()
        {
            return _PasswordHash;
        }

        #endregion


        #region CheckPassword(myPassword)

        /// <summary>
        /// Checks the given password against the stored PasswordHash.
        /// Your may not call this methods more often than once within MIN_SECONDS_BETWEEN_PASSWORD_CHECKS.
        /// </summary>
        /// <param name="myPassword">the password to check</param>
        public Boolean CheckPassword(String myPassword)
        {

            if (DateTime.Now.Subtract(_LastPasswordCheck) < TimeSpan.FromSeconds(MIN_SECONDS_BETWEEN_PASSWORD_CHECKS))
                return false;

            // The password is valid...
            if (_PasswordHash.CompareByteArray(Encoding.UTF8.GetBytes(myPassword)) == 0)
                return true;

            // ...or it is invalid!
            return false;

        }

        #endregion

        #region ChangePassword(myOldPassword, myNewPassword)

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="myOldPassword">the old password</param>
        /// <param name="myNewPassword">the new password</param>
        public void ChangePassword(String myOldPassword, String myNewPassword)
        {

            #region Input Exceptions

            if (myOldPassword == null)
                throw new ArgumentNullException("The old password must not be null!");

            if (myNewPassword == null)
                throw new ArgumentNullException("The new password must not be null!");

            #endregion

            if (CheckPassword(myOldPassword))
                _PasswordHash = Encoding.UTF8.GetBytes(myNewPassword);

            else
                throw new Exception("The password could not be changed as the old password is invalid!");

        }

        #endregion


        #region Operator overloading

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("Parameter myObject must not be null!");

            // Check if myObject can be cast to PasswordHash
            PasswordHash myPasswordHash = myObject as PasswordHash;
            if ( (Object) myPasswordHash == null)
                throw new ArgumentException("Parameter myObject could not be casted to type PasswordHash!");

            return this.Equals(myPasswordHash);

        }

        #endregion

        #region Equals(myPasswordHash)

        public Boolean Equals(PasswordHash myPasswordHash)
        {

            // Check if myPasswordHash is null
            if ( (Object) myPasswordHash == null)
                throw new ArgumentNullException("Parameter myPasswordHash must not be null!");

            // Check if the arrays have the same length
            if ((UInt64)_PasswordHash.LongLength != myPasswordHash.Length)
                return false;

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ((UInt64)_PasswordHash.LongLength); i++)
                if (_PasswordHash[i] != myPasswordHash.ToByteArray()[i])
                    return false;

            return true;

        }

        #endregion

        #region Operator == (myPasswordHash1, myPasswordHash2)

        public static Boolean operator == (PasswordHash myPasswordHash1, PasswordHash myPasswordHash2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myPasswordHash1, myPasswordHash2))
                return true;

            // If one is null, but not both, return false.
            if (( (Object) myPasswordHash1 == null) || ( (Object) myPasswordHash2 == null))
                return false;

            return myPasswordHash1.Equals(myPasswordHash2);

        }

        #endregion

        #region Operator != (myPasswordHash1, myPasswordHash2)

        public static Boolean operator != (PasswordHash myPasswordHash1, PasswordHash myPasswordHash2)
        {
            return !myPasswordHash1.Equals(myPasswordHash2);
        }

        #endregion

        #region Operator <  (myPasswordHash1, myPasswordHash2)

        public static Boolean operator < (PasswordHash myPasswordHash1, PasswordHash myPasswordHash2)
        {

            // Check the length of the arrays
            if (myPasswordHash1.Length < myPasswordHash2.Length)
                return true;

            if (myPasswordHash1.Length > myPasswordHash2.Length)
                return false;

            Byte[] _PasswordHash1 = myPasswordHash1.ToByteArray();
            Byte[] _PasswordHash2 = myPasswordHash1.ToByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ( (UInt64) myPasswordHash1.Length); i++)
            {
                if (_PasswordHash1[i] < _PasswordHash2[i]) return true;
                if (_PasswordHash1[i] > _PasswordHash2[i]) return false;
            }

            return false;

        }

        #endregion

        #region Operator >  (myPasswordHash1, myPasswordHash2)

        public static Boolean operator > (PasswordHash myPasswordHash1, PasswordHash myPasswordHash2)
        {

            // Check the length of the arrays
            if (myPasswordHash1.Length > myPasswordHash2.Length)
                return true;

            if (myPasswordHash1.Length < myPasswordHash2.Length)
                return false;

            Byte[] _PasswordHash1 = myPasswordHash1.ToByteArray();
            Byte[] _PasswordHash2 = myPasswordHash1.ToByteArray();

            // Check if the inner array of bytes have the same values
            for (UInt64 i = 0; i < ( (UInt64) myPasswordHash1.Length); i++)
            {
                if (_PasswordHash1[i] > _PasswordHash2[i]) return true;
                if (_PasswordHash1[i] < _PasswordHash2[i]) return false;
            }

            return false;

        }

        #endregion

        #region Operator <= (myPasswordHash1, myPasswordHash2)

        public static Boolean operator <= (PasswordHash myPasswordHash1, PasswordHash myPasswordHash2)
        {
            return !(myPasswordHash1 > myPasswordHash2);
        }

        #endregion

        #region Operator >= (myPasswordHash1, myPasswordHash2)

        public static Boolean operator >= (PasswordHash myPasswordHash1, PasswordHash myPasswordHash2)
        {
            return !(myPasswordHash1 < myPasswordHash2);
        }

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return _PasswordHash.ToHexString().GetHashCode();
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
            PasswordHash myPasswordHash = myObject as PasswordHash;
            if ( (Object) myPasswordHash == null)
                throw new ArgumentException("myObject is not of type PasswordHash!");

            if (this < (PasswordHash) myObject) return -1;
            if (this > (PasswordHash) myObject) return +1;
            
            return 0;

        }

        #endregion

        #region IComparable<PasswordHash> Member

        public Int32 CompareTo(PasswordHash myPasswordHash)
        {

            // Check if myObject is null
            if (myPasswordHash == null)
                throw new ArgumentNullException();

            if (this < myPasswordHash) return -1;
            if (this > myPasswordHash) return +1;

            return  0;

        }

        #endregion


        #region ToString()

        public override String ToString()
        {

            return String.Concat(BitConverter.GetBytes((Byte)_HashingAlgorithm).ToHexString(), "/",
                                 _Seed.ToHexString(), "/",
                                 _PasswordHash.ToHexString());

        }

        #endregion

        #region ToHexString(mySeperatorTypes)

        public String ToHexString(SeperatorTypes mySeperatorTypes)
        {

            return String.Concat(BitConverter.GetBytes((Byte)_HashingAlgorithm).ToHexString(), "/",
                                 _Seed.ToHexString(mySeperatorTypes), "/",
                                 _PasswordHash.ToHexString(mySeperatorTypes));

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
            return _PasswordHash;
        }

        #endregion

        #region Deserialize(myData)

        public void Deserialize(Byte[] myData)
        {
            _PasswordHash = myData;
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
            mySerializationWriter.WriteByte((Byte)_HashingAlgorithm);
            mySerializationWriter.Write(_Seed);
            mySerializationWriter.Write(_PasswordHash);
            mySerializationWriter.WriteDateTime(_LastPasswordCheck);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            _HashingAlgorithm = (IntegrityCheckTypes)mySerializationReader.ReadOptimizedByte();
            _Seed = mySerializationReader.ReadByteArray();
            _PasswordHash = mySerializationReader.ReadByteArray();
            _LastPasswordCheck = mySerializationReader.ReadDateTimeOptimized();
        }

        #endregion
    }

}
