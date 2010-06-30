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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;
using System.Runtime.Serialization;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.Exceptions;

namespace sones.GraphFS.Objects
{

    public class TypedMetadataKey<T> : IFastSerialize, IComparable, IComparable<TypedMetadataKey<T>>
        where T : IComparable, IComparable<T>
    {


        #region Properties

        private T      _Key;

        public T Key
        {
            get
            {
                return _Key;
            }
        }


        private Type _Type;

        public Type Type
        {
            get
            {
                return _Type;
            }
        }

        #endregion


        public TypedMetadataKey(T myKey, Type myType)
        {

            if (myKey == null)
                throw new ArgumentNullException("Parameter 'myKey' must not be null!");

            if (myType == null)
                throw new ArgumentNullException("Parameter 'myType' must not be null!");

            _Key  = myKey;
            _Type = myType;

        }



        #region Object-specific methods

        #region Operator overloading

        #region Equals(myObject)

        /// <summary>
        /// Compares this entity with the given object.
        /// </summary>
        /// <param name="myObject">An object of type TypedMetadataKey<T></param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            TypedMetadataKey<T> myTypedMetadataKey = myObject as TypedMetadataKey<T>;
            if ((Object) myTypedMetadataKey == null)
                throw new ArgumentException("myObject is not of type myTypedMetadataKey<PT>!");

            return Equals(myTypedMetadataKey);

        }

        #endregion

        #region Equals(myEntity)

        /// <summary>
        /// Compares this right with the given TypedMetadataKey<T>.
        /// </summary>
        /// <param name="myRight">an TypedMetadataKey<T> object</param>
        /// <returns>true|false</returns>
        public Boolean Equals(TypedMetadataKey<T> myTypedMetadataKey)
        {

            // Check if myTypedMetadataKey is null
            if ((Object) myTypedMetadataKey == null)
                throw new ArgumentNullException("myTypedMetadataKey must not be null!");

            // Check if the inner fields have the same values

            if (!_Key.Equals(myTypedMetadataKey.Key))
                return false;

            if (_Type   != myTypedMetadataKey.Type)
                return false;

            return true;

        }

        #endregion

        #region Operator == (myTypedMetadataKey1, myTypedMetadataKey2)

        public static Boolean operator == (TypedMetadataKey<T> myTypedMetadataKey1, TypedMetadataKey<T> myTypedMetadataKey2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myTypedMetadataKey1, myTypedMetadataKey2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myTypedMetadataKey1 == null) || ((Object) myTypedMetadataKey2 == null))
                return false;

            return myTypedMetadataKey1.Equals(myTypedMetadataKey2);

        }

        #endregion

        #region Operator != (myTypedMetadataKey1, myTypedMetadataKey2)

        public static Boolean operator != (TypedMetadataKey<T> myTypedMetadataKey1, TypedMetadataKey<T> myTypedMetadataKey2)
        {
            return !(myTypedMetadataKey1 == myTypedMetadataKey2);
        }

        #endregion

        #region Operator  < (myTypedMetadataKey1, myTypedMetadataKey2)

        public static Boolean operator < (TypedMetadataKey<T> myTypedMetadataKey1, TypedMetadataKey<T> myTypedMetadataKey2)
        {

            // Check if myTypedMetadataKey1 is null
            if (myTypedMetadataKey1 == null)
                throw new ArgumentNullException("myTypedMetadataKey1 must not be null!");

            // Check if myTypedMetadataKey2 is null
            if (myTypedMetadataKey2 == null)
                throw new ArgumentNullException("myTypedMetadataKey2 must not be null!");


            Int32 _TypedMetadataKeysNameComparison = myTypedMetadataKey1.Key.CompareTo(myTypedMetadataKey2.Key);

            if (_TypedMetadataKeysNameComparison < 0)
                return true;

            if (_TypedMetadataKeysNameComparison > 0)
                return false;


            return false;

        }

        #endregion

        #region Operator  > (myTypedMetadataKey1, myTypedMetadataKey2)

        public static Boolean operator > (TypedMetadataKey<T> myTypedMetadataKey1, TypedMetadataKey<T> myTypedMetadataKey2)
        {

            // Check if myTypedMetadataKey1 is null
            if (myTypedMetadataKey1 == null)
                throw new ArgumentNullException("myTypedMetadataKey1 must not be null!");

            // Check if myTypedMetadataKey2 is null
            if (myTypedMetadataKey2 == null)
                throw new ArgumentNullException("myTypedMetadataKey2 must not be null!");


            Int32 _TypedMetadataKeysNameComparison = myTypedMetadataKey1.Key.CompareTo(myTypedMetadataKey2.Key);

            if (_TypedMetadataKeysNameComparison > 0)
                return true;

            if (_TypedMetadataKeysNameComparison < 0)
                return false;


            return false;

        }

        #endregion

        #region Operator <= (myTypedMetadataKey1, myTypedMetadataKey2)

        public static Boolean operator <= (TypedMetadataKey<T> myTypedMetadataKey1, TypedMetadataKey<T> myTypedMetadataKey2)
        {
            return !(myTypedMetadataKey1 > myTypedMetadataKey2);
        }

        #endregion

        #region Operator >= (myTypedMetadataKey1, myTypedMetadataKey2)

        public static Boolean operator >= (TypedMetadataKey<T> myTypedMetadataKey1, TypedMetadataKey<T> myTypedMetadataKey2)
        {
            return !(myTypedMetadataKey1 < myTypedMetadataKey2);
        }

        #endregion

        #region GetHashCode()

        public override int GetHashCode()
        {
            return _Key.GetHashCode() ^ _Type.GetHashCode();
        }

        #endregion

        #endregion

        #region IComparable Member

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            TypedMetadataKey<T> myTypedMetadataKey = myObject as TypedMetadataKey<T>;
            if ( (Object) myTypedMetadataKey == null)
                throw new ArgumentException("myObject is not of type TypedMetadataKey<PT>!");

            return CompareTo(myTypedMetadataKey);

        }

        #endregion

        #region IComparable<TypedMetadataKey<T>> Member

        public Int32 CompareTo(TypedMetadataKey<T> myTypedMetadataKey)
        {

            // Check if myTypedMetadataKey is null
            if (myTypedMetadataKey == null)
                throw new ArgumentNullException("myTypedMetadataKey must not be null!");

            if (this < myTypedMetadataKey) return -1;
            if (this > myTypedMetadataKey) return +1;

            return 0;

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

            #region Data

            SerializationWriter writer;

            #endregion

            try
            {

                #region Init SerializationWriter

                writer = new SerializationWriter();

                #endregion

                writer.WriteObject(_Key);
                writer.WriteObject(_Type);

                isDirty = false;

                return writer.ToArray();

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

        }

        #endregion

        #region Deserialize(mySerializedData)

        public void Deserialize(Byte[] mySerializedData)
        {

            #region Data

            SerializationReader reader;

            #endregion

            #region Init reader

            reader = new SerializationReader(mySerializedData);

            #endregion

            try
            {

                _Key  = (T)    reader.ReadObject();
                _Type = (Type) reader.ReadObject();

            }

            catch (Exception e)
            {
                throw new PandoraFSException_EntityCouldNotBeDeserialized("Right could not be deserialized!\n\n" + e);
            }

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
