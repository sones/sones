/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

#region Usings

using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using sones.Library.NewFastSerializer;

#endregion

namespace sones.Library.Serializer
{
    #region NotFastSerialiable Attribute
    /// <summary>
    /// Use this attribute to mark member variables that should not be serialized using the FastReflectionSerializer
    /// </summary>
    public class NotFastSerializable : Attribute
    {
    }
    #endregion

    /// <summary>
    /// This class will serialize/deserialize any public field/member variable of an ob
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FastReflectionSerializer<T> where T : new()
    {
        #region Constructor
        public FastReflectionSerializer()
        {
        }
        #endregion

        #region Serialize
        /// <summary>
        /// this method serializes the Input object into a byte array data stream. Every public member variable that
        /// does not have the NotFastSerializable attribut will be serialized
        /// </summary>
        /// <param name="Input">the Input object</param>
        /// <returns>a byte array containing the data stream</returns>
        public byte[] Serialize(T Input)
        {
            Type reflectiontype = typeof(T);
            MD5 MD5CheckSum = MD5.Create();
            StringBuilder HashString = new StringBuilder();
            // warm up the FastSerializerEngine...
            SerializationWriter writer = new SerializationWriter();

            bool serialize = true;
            // reflect the given object...
            foreach (FieldInfo field in reflectiontype.GetFields())
            {
                if (field.IsPublic)
                {
                    serialize = true;
                    #region Check for NotFastSerializable Attribute
                    foreach (Attribute attr in field.GetCustomAttributes(true))
                    {
                        NotFastSerializable tattr = attr as NotFastSerializable;
                        if (null != tattr)
                        {
                            serialize = false;
                            break;
                        }
                    }
                    #endregion
                    if (serialize)
                    {
                        HashString = HashString.Append(field.FieldType.ToString());

                        object fieldvalue = field.GetValue(Input);
                        // SerializedSuperblock it to the serializer stream
                        writer.WriteObject(fieldvalue);
                    }
                }
            }

            // build the final byte array
            
            byte[] HashValue = MD5CheckSum.ComputeHash(Encoding.UTF8.GetBytes(HashString.ToString()));
            byte[] SerializedBytes = writer.ToArray();

            byte[] Result = new byte[HashValue.Length + SerializedBytes.Length];
            Array.Copy(HashValue, 0, Result, 0, HashValue.Length);
            Array.Copy(SerializedBytes, 0, Result, HashValue.Length, SerializedBytes.Length);
            

            // return the values...
            return Result;
        }
        #endregion

        #region DeSerialize
        /// <summary>
        /// this method deserializes a given byte array whose contents hopefully match the object it ought to be
        /// deserialized to.
        /// </summary>
        /// <param name="Input">an byte array of data containing serialized data</param>
        /// <returns>an object</returns>
        public T DeSerialize(byte[] Input)
        {
            Type reflectiontype = typeof(T);
            List<FieldInfo> Fields = new List<FieldInfo>();

            MD5 MD5CheckSum = MD5.Create();
            StringBuilder HashString = new StringBuilder();

            T Output = new T();

            bool serialize = true;
            // reflect the given object...
            foreach (FieldInfo field in reflectiontype.GetFields())
            {
                // check if this field is actually public
                if (field.IsPublic)
                {
                    serialize = true;
                    #region Check for NotFastSerializable Attribute
                    foreach (Attribute attr in field.GetCustomAttributes(true))
                    {
                        NotFastSerializable tattr = attr as NotFastSerializable;
                        if (null != tattr)
                        {
                            serialize = false;
                            break;
                        }
                    }
                    #endregion
                    if (serialize)
                    {
                        HashString = HashString.Append(field.FieldType.ToString());
                        Fields.Add(field);
                    }
                }
            }
            
            byte[] ToBeCheckedHashValue = new byte[MD5CheckSum.HashSize];
            Array.Copy(Input, ToBeCheckedHashValue, MD5CheckSum.HashSize);
            var hashValue = MD5CheckSum.ComputeHash(Encoding.UTF8.GetBytes(HashString.ToString()));
            
            if (hashValue.CompareByteArray(ToBeCheckedHashValue) != 0)
                throw new FastReflectionSerializerExceptions_DeSerializeHashNotValid("This seems not to be the right object or object version, cannot deserialize!");

            byte[] InputBytes = new byte[Input.Length - MD5CheckSum.HashSize];
            Array.Copy(Input, MD5CheckSum.HashSize, InputBytes, 0, InputBytes.Length);

            // warm up the FastSerializerEngine...
            SerializationReader reader = new SerializationReader(InputBytes);

            // deserialize
            foreach (FieldInfo field in Fields)
            {
                field.SetValue((object)Output, (object)reader.ReadObject());
            }

            return Output;
        }
        #endregion
    }

    public static class ByteArrayExtension
    {
        #region CompareByteArray(myByteArray2)

        /// <summary>
        /// Compares two byte arrays bytewise
        /// </summary>
        /// <param name="myArray1">Array 1</param>
        /// <param name="myArray2">Array 2</param>
        /// <returns></returns>
        public static Int32 CompareByteArray(this Byte[] myByteArray, Byte[] myByteArray2)
        {

            if (myByteArray.Length < myByteArray2.Length)
                return -1;

            if (myByteArray.Length > myByteArray2.Length)
                return 1;

            for (int i = 0; i <= myByteArray.Length - 1; i++)
            {

                if (myByteArray[i] < myByteArray2[i])
                    return -1;

                if (myByteArray[i] > myByteArray2[i])
                    return 1;

            }

            return 0;

        }

        #endregion
    }
}
