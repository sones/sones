/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace sones.Lib.NewFastSerializer
{

    /// <summary>
    /// A SerializationReader instance is used to read stored values and objects from a byte array.
    ///
    /// Once an instance is created, use the various methods to read the required data.
    /// The data read MUST be exactly the same type and in the same order as it was written.
    /// </summary>
    public sealed class SerializationReader : BinaryReader
    {

        #region Static
        
        // Marker to denote that all elements in a value array are optimizable
        private static readonly BitArray FullyOptimizableTypedArray = new BitArray(0);
        
        #endregion Static

        #region Fields

        private String[] stringTokenList = null;
        private Object[] objectTokens = null;
        UInt64           endPosition;

        #endregion Fields

        #region Properties
        
        /// <summary>
        /// Returns the number of bytes or serialized remaining to be processed.
        /// Useful for checking that deserialization is complete.
        /// 
        /// Warning: Retrieving the ObjectStreams in certain stream types can be expensive,
        /// e.g. a _FileStream, so use sparingly unless known to be a MemoryStream.
        /// </summary>
        public UInt64 BytesRemaining
        {
            get
            {
                return endPosition - (UInt64) BaseStream.Position;
            }
        }
        
        #endregion


        #region Constructor
        
        /// <summary>
        /// Creates a SerializationReader using a byte[] previous created by SerializationWriter
        /// 
        /// A MemoryStream is used to access the data without making a copy of it.
        /// </summary>
        /// <param name="data">The byte[] containining serialized data.</param>
        public SerializationReader(Byte[] data)
            : this(new MemoryStream(data))
        {
        }

        /// <summary>
        /// Creates a SerializationReader based on the passed Stream.
        /// </summary>
        /// <param name="stream">The stream containing the serialized data</param>
        private SerializationReader(Stream stream)
            : base(stream)
        {

            endPosition = stream.ULength();
            //endPosition = ReadInt32();
            //stream.Position = endPosition;

            //stringTokenList = new string[ReadOptimizedInt32()];
            //for (int i = 0; i < stringTokenList.Length; i++)
            //{
            //    stringTokenList[i] = base.ReadString();
            //}

            //objectTokens = new object[ReadOptimizedInt32()];
            //for (int i = 0; i < objectTokens.Length; i++)
            //{
            //    objectTokens[i] = ReadObject();
            //}

            //stream.Position = 4;

        }

        #endregion Constructor

        

        #region Methods
        /// <summary>
        /// Returns an ArrayList or null from the stream.
        /// </summary>
        /// <returns>An ArrayList instance.</returns>
        public ArrayList ReadArrayList()
        {
            if (readTypeCode() == SerializedType.NullType) return null;

            return new ArrayList(ReadOptimizedObjectArray());
        }

        /// <summary>
        /// Returns a BitArray or null from the stream.
        /// </summary>
        /// <returns>A BitArray instance.</returns>
        public BitArray ReadBitArray()
        {
            if (readTypeCode() == SerializedType.NullType) return null;
            return ReadOptimizedBitArray();
        }

        /// <summary>
        /// Returns a BitVector32 value from the stream.
        /// </summary>
        /// <returns>A BitVector32 value.</returns>
        public BitVector32 ReadBitVector32()
        {
            return new BitVector32(base.ReadInt32());
        }

        
        /// <summary>
        /// Reads the specified number of bytes directly from the stream.
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>A byte[] containing the read bytes</returns>
        public byte[] ReadBytesDirect(int count)
        {
            return base.ReadBytes(count);
        }


        #region DateTime

        public DateTime ReadDateTimeOptimized()
        {
            var typeCode = readTypeCode();

            switch (typeCode)
            { 
                case SerializedType.MinDateTimeType:
                    return DateTime.MinValue;

                case SerializedType.MaxDateTimeType:
                    return DateTime.MaxValue;

                case SerializedType.OptimizedDateTimeType:
                    return ReadOptimizedDateTime();

                case SerializedType.DateTimeType:
                    return ReadDateTime();

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        
        }

        /// <summary>
        /// Returns a DateTime value from the stream that was stored optimized.
        /// </summary>
        /// <returns>A DateTime value.</returns>
        private DateTime ReadOptimizedDateTime()
        {
            // Read date information from first three bytes
            BitVector32 dateMask = new BitVector32(base.ReadByte() | (base.ReadByte() << 8) | (base.ReadByte() << 16));
            DateTime result = new DateTime(
                    dateMask[SerializationWriter.DateYearMask],
                    dateMask[SerializationWriter.DateMonthMask],
                    dateMask[SerializationWriter.DateDayMask]
            );

            if (dateMask[SerializationWriter.DateHasTimeOrKindMask] == 1)
            {
                byte initialByte = base.ReadByte();
                DateTimeKind dateTimeKind = (DateTimeKind)(initialByte & 0x03);
                initialByte &= 0xfc; // Remove the IsNegative and HasDays flags which are never true for a DateTime
                if (dateTimeKind != DateTimeKind.Unspecified) result = DateTime.SpecifyKind(result, dateTimeKind);
                if (initialByte == 0)
                    base.ReadByte(); // No need to call decodeTimeSpan if there is no time information
                else
                {
                    result = result.Add(decodeTimeSpan(initialByte));
                }
            }
            return result;
        }
        
        /// <summary>
        /// Returns a DateTime value from the stream.
        /// </summary>
        /// <returns>A DateTime value.</returns>
        private DateTime ReadDateTime()
        {
            return DateTime.FromBinary(base.ReadInt64());
        }

        #endregion

        #region GUID

        /// <summary>
        /// Returns a Guid value from the stream.
        /// </summary>
        /// <returns>A DateTime value.</returns>
        public Guid ReadGuid()
        {
            return new Guid(ReadBytes(16));
        }

        #endregion


        #region Object
        /// <summary>
        /// Returns an object based on the SerializedType read next from the stream.
        /// </summary>
        /// <returns>An object instance.</returns>
        public object ReadObject()
        {
            return processObject((SerializedType)base.ReadByte());
        }
        #endregion


        #region Enum

        /// <summary>
        /// need to read enum values
        /// </summary>
        /// <returns>the enum bytes</returns>
        public Byte ReadOptimizedByte()
        {
            var typeCode = readTypeCode();

            switch (typeCode)
            { 
                case SerializedType.ZeroByteType:
                    return (Byte)0;
                
                case SerializedType.OneByteType:
                    return (Byte)1;

                case SerializedType.ByteType:
                    return base.ReadByte();
            
                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        }

        #endregion

        #region String

        /// <summary>
        /// Called ReadOptimizedString().
        /// This override to hide base BinaryReader.ReadString().
        /// </summary>
        /// <returns>A string value.</returns>
        public override string ReadString()
        {            
            return ReadOptimizedString();
        }

        /// <summary>
        /// Returns a string value from the stream.
        /// </summary>
        /// <returns>A string value.</returns>
        public string ReadStringDirect()
        {
            return base.ReadString();
        }

        /// <summary>
        /// Returns a string value from the stream that was stored optimized.
        /// </summary>
        /// <returns>A string value.</returns>
        private string ReadOptimizedString()
        {
            SerializedType typeCode = readTypeCode();

            /*if (typeCode < SerializedType.NullType)
                return readTokenizedString((int)typeCode);

            else if (typeCode == SerializedType.NullType)
                return null;

            else if (typeCode == SerializedType.YStringType)
                return "Y";

            else if (typeCode == SerializedType.NStringType)
                return "N";

            else if (typeCode == SerializedType.SingleCharStringType)
                return Char.ToString(ReadChar());

            else if (typeCode == SerializedType.SingleSpaceType)
                return " ";

            else if (typeCode == SerializedType.EmptyStringType)
                return string.Empty;*/

            if (typeCode == SerializedType.NullType)
                return null;

            if (typeCode == SerializedType.StringType)
                return ReadStringDirect();
            else
            {
                throw new InvalidOperationException("Unrecognized TypeCode");
            }
        }

        #endregion

        #region TimeSpan
        /// <summary>
        /// Returns a TimeSpan value from the stream.
        /// </summary>
        /// <returns>A TimeSpan value.</returns>
        private TimeSpan ReadTimeSpan()
        {
            return new TimeSpan(base.ReadInt64());
        }

        /// <summary>
        /// Returns a TimeSpan value from the stream that was stored optimized.
        /// </summary>
        /// <returns>A TimeSpan value.</returns>
        private TimeSpan ReadOptimizedTimeSpan()
        {
            return decodeTimeSpan(base.ReadByte());
        }

        public TimeSpan ReadTimeSpanOptimized()
        {
            var typeCode = readTypeCode();

            switch (typeCode)
            { 
                case SerializedType.ZeroTimeSpanType:
                    return TimeSpan.Zero;

                case SerializedType.OptimizedTimeSpanType:
                    return ReadOptimizedTimeSpan();

                case SerializedType.TimeSpanType:
                    return ReadTimeSpan();

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }

        }

        #endregion

        #region Type

        public Type ReadTypeOptimized()
        {
            var typeCode = readTypeCode();
            return ReadOptimizedType();
        }

        /// <summary>
        /// Returns a myObjectStream from the stream.
        /// 
        /// Throws an exception if the myObjectStream cannot be found.
        /// </summary>
        /// <returns>A myObjectStream instance.</returns>
        private Type ReadOptimizedType()
        {
            return ReadOptimizedType(true);
        }

        /// <summary>
        /// Returns a myObjectStream from the stream.
        /// 
        /// Throws an exception if the myObjectStream cannot be found and throwOnError is true.
        /// </summary>
        /// <returns>A myObjectStream instance.</returns>
        private Type ReadOptimizedType(bool throwOnError)
        {
            return Type.GetType(ReadOptimizedString(), throwOnError);
        }
        
        /// <summary>
        /// Returns a myObjectStream or null from the stream.
        /// 
        /// Throws an exception if the myObjectStream cannot be found.
        /// </summary>
        /// <returns>A myObjectStream instance.</returns>
        private Type ReadType()
        {
            return ReadType(true);
        }

        /// <summary>
        /// Returns a myObjectStream or null from the stream.
        /// 
        /// Throws an exception if the myObjectStream cannot be found and throwOnError is true.
        /// </summary>
        /// <returns>A myObjectStream instance.</returns>
        private Type ReadType(bool throwOnError)
        {
            if (readTypeCode() == SerializedType.NullType) return null;
            return Type.GetType(ReadOptimizedString(), throwOnError);
        }

        #endregion

        #region Arrays

        /// <summary>
        /// Returns an ArrayList from the stream that was stored optimized.
        /// </summary>
        /// <returns>An ArrayList instance.</returns>
        private ArrayList ReadOptimizedArrayList()
        {
            return new ArrayList(ReadOptimizedObjectArray());
        }

        /// <summary>
        /// Returns a BitArray from the stream that was stored optimized.
        /// </summary>
        /// <returns>A BitArray instance.</returns>
        private BitArray ReadOptimizedBitArray()
        {
            int length = ReadOptimizedInt32();
            if (length == 0)
                return FullyOptimizableTypedArray;
            else
            {
                BitArray result = new BitArray(base.ReadBytes((length + 7) / 8));
                result.Length = length;
                return result;
            }
        }

        /// <summary>
        /// Returns an object[] from the stream that was stored optimized.
        /// </summary>
        /// <returns>An object[] instance.</returns>
        private object[] ReadOptimizedObjectArray()
        {
            return ReadOptimizedObjectArray(null);
        }

        /// <summary>
        /// Returns an object[] from the stream that was stored optimized.
        /// The returned array will be typed according to the specified element type
        /// and the resulting array can be cast to the expected type.
        /// e.g.
        /// string[] myStrings = (String[]) reader.ReadOptimizedObjectArray(typeof(String));
        /// 
        /// An exception will be thrown if any of the deserialized values cannot be
        /// cast to the specified elementType.
        /// 
        /// </summary>
        /// <param name="elementType">The myObjectStream of the expected array elements. null will return a plain object[].</param>
        /// <returns>An object[] instance.</returns>
        private object[] ReadOptimizedObjectArray(Type elementType)
        {
            int length = ReadOptimizedInt32();
            object[] result = (object[])(elementType == null ? new object[length] : Array.CreateInstance(elementType, length));
            for (int i = 0; i < result.Length; i++)
            {
                SerializedType t = readTypeCode();

                if (t == SerializedType.NullSequenceType)
                    i += ReadOptimizedInt32();
                else if (t == SerializedType.DuplicateValueSequenceType)
                {
                    object target = result[i] = ReadObject();
                    int duplicates = ReadOptimizedInt32();
                    while (duplicates-- > 0) result[++i] = target;
                }
                else if (t == SerializedType.DBNullSequenceType)
                {
                    int duplicates = ReadOptimizedInt32();
                    result[i] = DBNull.Value;
                    while (duplicates-- > 0) result[++i] = DBNull.Value;
                }
                else if (t != SerializedType.NullType)
                {
                    result[i] = processObject(t);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a pair of object[] arrays from the stream that were stored optimized.
        /// </summary>
        /// <returns>A pair of object[] arrays.</returns>
        private void ReadOptimizedObjectArrayPair(out object[] values1, out object[] values2)
        {
            values1 = ReadOptimizedObjectArray(null);
            values2 = new object[values1.Length];

            for (int i = 0; i < values2.Length; i++)
            {
                SerializedType t = readTypeCode();

                if (t == SerializedType.DuplicateValueSequenceType)
                {
                    values2[i] = values1[i];
                    int duplicates = ReadOptimizedInt32();
                    while (duplicates-- > 0) values2[++i] = values1[i];
                }
                else if (t == SerializedType.DuplicateValueType)
                {
                    values2[i] = values1[i];
                }
                else if (t == SerializedType.NullSequenceType)
                {
                    i += ReadOptimizedInt32();
                }
                else if (t == SerializedType.DBNullSequenceType)
                {
                    int duplicates = ReadOptimizedInt32();
                    values2[i] = DBNull.Value;
                    while (duplicates-- > 0) values2[++i] = DBNull.Value;
                }
                else if (t != SerializedType.NullType)
                {
                    values2[i] = processObject(t);
                }
            }
        }

        /// <summary>
        /// Returns a typed array from the stream.
        /// </summary>
        /// <returns>A typed array.</returns>
        private Array ReadTypedArray()
        {
            return (Array)processArrayTypes(readTypeCode(), null);
        }


        /// <summary>
        /// Returns a Byte[] from the stream.
        /// </summary>
        /// <returns>A Byte instance; or null.</returns>
        public byte[] ReadByteArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new byte[0];
            else
            {
                return readByteArray();
            }
        }

        /// <summary>
        /// Returns a Char[] from the stream.
        /// </summary>
        /// <returns>A Char[] value; or null.</returns>
        public char[] ReadCharArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new char[0];
            else
            {
                return readCharArray();
            }
        }

        /// <summary>
        /// Returns a Double[] from the stream.
        /// </summary>
        /// <returns>A Double[] instance; or null.</returns>
        public double[] ReadDoubleArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new double[0];
            else
            {
                return readDoubleArray();
            }
        }

        /// <summary>
        /// Returns a Guid[] from the stream.
        /// </summary>
        /// <returns>A Guid[] instance; or null.</returns>
        public Guid[] ReadGuidArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new Guid[0];
            else
            {
                return readGuidArray();
            }
        }

        /// <summary>
        /// Returns an Int16[] from the stream.
        /// </summary>
        /// <returns>An Int16[] instance; or null.</returns>
        public short[] ReadInt16Array()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new short[0];
            else
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
                short[] result = new short[ReadOptimizedInt32()];
                for (int i = 0; i < result.Length; i++)
                {
                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
                        result[i] = ReadInt16();
                    else
                    {
                        result[i] = ReadOptimizedInt16();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns an object[] or null from the stream.
        /// </summary>
        /// <returns>A DateTime value.</returns>
        public object[] ReadObjectArray()
        {
            return ReadObjectArray(null);
        }

        /// <summary>
        /// Returns an object[] or null from the stream.
        /// The returned array will be typed according to the specified element type
        /// and the resulting array can be cast to the expected type.
        /// e.g.
        /// string[] myStrings = (String[]) reader.ReadObjectArray(typeof(String));
        /// 
        /// An exception will be thrown if any of the deserialized values cannot be
        /// cast to the specified elementType.
        /// 
        /// </summary>
        /// <param name="elementType">The myObjectStream of the expected array elements. null will return a plain object[].</param>
        /// <returns>An object[] instance.</returns>
        public object[] ReadObjectArray(Type elementType)
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyObjectArrayType)
                return elementType == null ? new object[0] : (object[])Array.CreateInstance(elementType, 0);
            else if (t == SerializedType.EmptyTypedArrayType)
                throw new Exception();
            else
            {
                return ReadOptimizedObjectArray(elementType);
            }
        }

        /// <summary>
        /// Returns a Single[] from the stream.
        /// </summary>
        /// <returns>A Single[] instance; or null.</returns>
        public float[] ReadSingleArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new float[0];
            else
            {
                return readSingleArray();
            }
        }

        /// <summary>
        /// Returns an SByte[] from the stream.
        /// </summary>
        /// <returns>An SByte[] instance; or null.</returns>
        //[CLSCompliant(false)]
        public sbyte[] ReadSByteArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new sbyte[0];
            else
            {
                return readSByteArray();
            }
        }

        /// <summary>
        /// Returns a string[] or null from the stream.
        /// </summary>
        /// <returns>An string[] instance.</returns>
        public string[] ReadStringArray()
        {
            return (String[])ReadObjectArray(typeof(String));
        }

        /// <summary>
        /// Returns a UInt16[] from the stream.
        /// </summary>
        /// <returns>A UInt16[] instance; or null.</returns>
        //[CLSCompliant(false)]
        public ushort[] ReadUInt16Array()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new ushort[0];
            else
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
                ushort[] result = new ushort[ReadOptimizedUInt32()];
                for (int i = 0; i < result.Length; i++)
                {
                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
                        result[i] = base.ReadUInt16();
                    else
                    {
                        result[i] = ReadOptimizedUInt16();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns a Boolean[] from the stream.
        /// </summary>
        /// <returns>A Boolean[] instance; or null.</returns>
        public bool[] ReadBooleanArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new bool[0];
            else
            {
                return readBooleanArray();
            }
        }

        /// <summary>
        /// Returns a DateTime[] from the stream.
        /// </summary>
        /// <returns>A DateTime[] instance; or null.</returns>
        public DateTime[] ReadDateTimeArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new DateTime[0];
            else
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
                DateTime[] result = new DateTime[ReadOptimizedInt32()];
                for (int i = 0; i < result.Length; i++)
                {
                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
                        result[i] = ReadDateTime();
                    else
                    {
                        result[i] = ReadOptimizedDateTime();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns a Decimal[] from the stream.
        /// </summary>
        /// <returns>A Decimal[] instance; or null.</returns>
        public decimal[] ReadDecimalArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new decimal[0];
            else
            {
                return readDecimalArray();
            }
        }

        /// <summary>
        /// Returns an Int32[] from the stream.
        /// </summary>
        /// <returns>An Int32[] instance; or null.</returns>
        public int[] ReadInt32Array()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new int[0];
            else
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
                int[] result = new int[ReadOptimizedInt32()];
                for (int i = 0; i < result.Length; i++)
                {
                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
                        result[i] = base.ReadInt32();
                    else
                    {
                        result[i] = ReadOptimizedInt32();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns an Int64[] from the stream.
        /// </summary>
        /// <returns>An Int64[] instance; or null.</returns>
        public long[] ReadInt64Array()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new long[0];
            else
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
                long[] result = new long[ReadOptimizedInt64()];
                for (int i = 0; i < result.Length; i++)
                {
                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
                        result[i] = base.ReadInt64();
                    else
                    {
                        result[i] = ReadOptimizedInt64();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns a string[] from the stream that was stored optimized.
        /// </summary>
        /// <returns>An string[] instance.</returns>
        public string[] ReadOptimizedStringArray()
        {
            return (String[])ReadOptimizedObjectArray(typeof(String));
        }

        /// <summary>
        /// Returns a TimeSpan[] from the stream.
        /// </summary>
        /// <returns>A TimeSpan[] instance; or null.</returns>
        public TimeSpan[] ReadTimeSpanArray()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new TimeSpan[0];
            else
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
                TimeSpan[] result = new TimeSpan[ReadOptimizedInt32()];
                for (int i = 0; i < result.Length; i++)
                {
                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
                        result[i] = ReadTimeSpan();
                    else
                    {
                        result[i] = ReadOptimizedTimeSpan();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns a UInt[] from the stream.
        /// </summary>
        /// <returns>A UInt[] instance; or null.</returns>
        //[CLSCompliant(false)]
        public uint[] ReadUInt32Array()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new uint[0];
            else
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
                uint[] result = new uint[ReadOptimizedUInt32()];
                for (int i = 0; i < result.Length; i++)
                {
                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
                        result[i] = base.ReadUInt32();
                    else
                    {
                        result[i] = ReadOptimizedUInt32();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns a UInt64[] from the stream.
        /// </summary>
        /// <returns>A UInt64[] instance; or null.</returns>
        //[CLSCompliant((false))]
        public ulong[] ReadUInt64Array()
        {
            SerializedType t = readTypeCode();
            if (t == SerializedType.NullType)
                return null;
            else if (t == SerializedType.EmptyTypedArrayType)
                return new ulong[0];
            else
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
                ulong[] result = new ulong[ReadOptimizedInt64()];
                for (int i = 0; i < result.Length; i++)
                {
                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
                        result[i] = base.ReadUInt64();
                    else
                    {
                        result[i] = ReadOptimizedUInt64();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns a Boolean[] from the stream.
        /// </summary>
        /// <returns>A Boolean[] instance; or null.</returns>
        private bool[] ReadOptimizedBooleanArray()
        {
            return ReadBooleanArray();
        }

        /// <summary>
        /// Returns a DateTime[] from the stream.
        /// </summary>
        /// <returns>A DateTime[] instance; or null.</returns>
        private DateTime[] ReadOptimizedDateTimeArray()
        {
            return ReadDateTimeArray();
        }

        /// <summary>
        /// Returns a Decimal[] from the stream.
        /// </summary>
        /// <returns>A Decimal[] instance; or null.</returns>
        private decimal[] ReadOptimizedDecimalArray()
        {
            return ReadDecimalArray();
        }

        /// <summary>
        /// Returns a Int16[] from the stream.
        /// </summary>
        /// <returns>An Int16[] instance; or null.</returns>
        private short[] ReadOptimizedInt16Array()
        {
            return ReadInt16Array();
        }

        /// <summary>
        /// Returns a Int32[] from the stream.
        /// </summary>
        /// <returns>An Int32[] instance; or null.</returns>
        private int[] ReadOptimizedInt32Array()
        {
            return ReadInt32Array();
        }

        /// <summary>
        /// Returns a Int64[] from the stream.
        /// </summary>
        /// <returns>A Int64[] instance; or null.</returns>
        private long[] ReadOptimizedInt64Array()
        {
            return ReadInt64Array();
        }

        /// <summary>
        /// Returns a TimeSpan[] from the stream.
        /// </summary>
        /// <returns>A TimeSpan[] instance; or null.</returns>
        private TimeSpan[] ReadOptimizedTimeSpanArray()
        {
            return ReadTimeSpanArray();
        }

        /// <summary>
        /// Returns a UInt16[] from the stream.
        /// </summary>
        /// <returns>A UInt16[] instance; or null.</returns>
        //[CLSCompliant(false)]
        private ushort[] ReadOptimizedUInt16Array()
        {
            return ReadUInt16Array();
        }

        /// <summary>
        /// Returns a UInt32[] from the stream.
        /// </summary>
        /// <returns>A UInt32[] instance; or null.</returns>
        //[CLSCompliant(false)]
        private uint[] ReadOptimizedUInt32Array()
        {
            return ReadUInt32Array();
        }

        /// <summary>
        /// Returns a UInt64[] from the stream.
        /// </summary>
        /// <returns>A UInt64[] instance; or null.</returns>
        //[CLSCompliant(false)]
        private ulong[] ReadOptimizedUInt64Array()
        {
            return ReadUInt64Array();
        }

        /// <summary>
        /// Internal implementation returning a Bool[].
        /// </summary>
        /// <returns>A Bool[].</returns>
        private bool[] readBooleanArray()
        {
            BitArray bitArray = ReadOptimizedBitArray();
            bool[] result = new bool[bitArray.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = bitArray[i];
            }

            return result;
        }

        /// <summary>
        /// Internal implementation returning a Byte[].
        /// </summary>
        /// <returns>A Byte[].</returns>
        private byte[] readByteArray()
        {
            return base.ReadBytes(ReadOptimizedInt32());
        }

        /// <summary>
        /// Internal implementation returning a Char[].
        /// </summary>
        /// <returns>A Char[].</returns>
        private char[] readCharArray()
        {
            return base.ReadChars(ReadOptimizedInt32());
        }

        /// <summary>
        /// Internal implementation returning a Decimal[].
        /// </summary>
        /// <returns>A Decimal[].</returns>
        private decimal[] readDecimalArray()
        {
            decimal[] result = new decimal[ReadOptimizedInt32()];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ReadOptimizedDecimal();
            }

            return result;
        }

        /// <summary>
        /// Internal implementation returning a Double[].
        /// </summary>
        /// <returns>A Double[].</returns>
        private double[] readDoubleArray()
        {
            double[] result = new double[ReadOptimizedInt32()];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = base.ReadDouble();
            }

            return result;
        }

        /// <summary>
        /// Internal implementation returning a Guid[].
        /// </summary>
        /// <returns>A Guid[].</returns>
        private Guid[] readGuidArray()
        {
            Guid[] result = new Guid[ReadOptimizedInt32()];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ReadGuid();
            }

            return result;
        }

        /// <summary>
        /// Internal implementation returning an SByte[].
        /// </summary>
        /// <returns>An SByte[].</returns>
        private sbyte[] readSByteArray()
        {
            sbyte[] result = new sbyte[ReadOptimizedInt32()];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ReadSByte();
            }

            return result;
        }

        /// <summary>
        /// Internal implementation returning a Single[].
        /// </summary>
        /// <returns>A Single[].</returns>
        private float[] readSingleArray()
        {
            float[] result = new float[ReadOptimizedInt32()];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ReadSingle();
            }
            return result;
        }

        #endregion

        #region BitVector32

        /// <summary>
        /// Returns a BitVector32 value from the stream that was stored optimized.
        /// </summary>
        /// <returns>A BitVector32 value.</returns>
        private BitVector32 ReadOptimizedBitVector32()
        {
            return new BitVector32(Read7BitEncodedInt());
        }        

        #endregion

        #region Decimal
        /// <summary>
        /// Returns a Decimal value from the stream that was stored optimized.
        /// </summary>
        /// <returns>A Decimal value.</returns>
        private Decimal ReadOptimizedDecimal()
        {
            byte flags = base.ReadByte();
            int lo = 0;
            int mid = 0;
            int hi = 0;
            byte scale = 0;

            if ((flags & 0x02) != 0) scale = base.ReadByte();

            if ((flags & 4) == 0) if ((flags & 32) != 0) lo = ReadOptimizedInt32(); else lo = base.ReadInt32();
            if ((flags & 8) == 0) if ((flags & 64) != 0) mid = ReadOptimizedInt32(); else mid = base.ReadInt32();
            if ((flags & 16) == 0) if ((flags & 128) != 0) hi = ReadOptimizedInt32(); else hi = base.ReadInt32();

            return new decimal(lo, mid, hi, (flags & 0x01) != 0, scale);
        }

        #endregion
        
        #region Int32
        /// <summary>
        /// Returns an Int32 value from the stream that was stored optimized.
        /// </summary>
        /// <returns>An Int32 value.</returns>
        private int ReadOptimizedInt32()
        {
            int result = 0;
            int bitShift = 0;
            while (true)
            {
                byte nextByte = base.ReadByte();
                result |= ((int)nextByte & 0x7f) << bitShift;
                bitShift += 7;
                if ((nextByte & 0x80) == 0) return result;
            }
        }

        public override Int32 ReadInt32()
        {
            var typeCode = readTypeCode();

            switch (typeCode)
            {
                case SerializedType.ZeroInt32Type:
                    return 0;

                case SerializedType.MinusOneInt32Type:
                    return -1;

                case SerializedType.OneInt32Type:
                    return 1;

                case SerializedType.OptimizedInt32Type:
                    return ReadOptimizedInt32();

                case SerializedType.OptimizedInt32NegativeType:
                    return -ReadOptimizedInt32() - 1;

                case SerializedType.Int32Type:
                    return base.ReadInt32();

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        }

        #endregion

        #region Int16

        /// <summary>
        /// Returns an Int16 value from the stream that was stored optimized.
        /// </summary>
        /// <returns>An Int16 value.</returns>
        private short ReadOptimizedInt16()
        {
            return (short)ReadOptimizedInt32();
        }

        /// <summary>
        /// Returns a UInt16 value from the stream that was stored optimized.
        /// </summary>
        /// <returns>A UInt16 value.</returns>
        //[CLSCompliant(false)]
        private ushort ReadOptimizedUInt16()
        {
            return (ushort)ReadOptimizedUInt32();
        }


        public override UInt16 ReadUInt16()
        {
            var typeCode = readTypeCode();

            switch (typeCode)
            {
                case SerializedType.ZeroUInt16Type:
                    return 0;

                case SerializedType.OneUInt16Type:
                    return 1;

                case SerializedType.OptimizedUInt16Type:
                    return ReadOptimizedUInt16();

                case SerializedType.UInt16Type:
                    return base.ReadUInt16();

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        }

        #endregion

        #region UInt64

        public override UInt64 ReadUInt64()
        {
            var typeCode = readTypeCode();

            switch(typeCode)
            {
                case SerializedType.ZeroUInt64Type:
                    return 0;

                case SerializedType.OneUInt64Type:
                    return 1;

                case SerializedType.OptimizedUInt64Type:
                    return ReadOptimizedUInt64();

                case SerializedType.UInt64Type:
                    return base.ReadUInt64();

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }            
        }

        /// <summary>
        /// Returns a UInt64 value from the stream that was stored optimized.
        /// </summary>
        /// <returns>A UInt64 value.</returns>
        //[CLSCompliant(false)]
        private ulong ReadOptimizedUInt64()
        {
            ulong result = 0;
            int bitShift = 0;
            while (true)
            {
                byte nextByte = base.ReadByte();
                result |= ((ulong)nextByte & 0x7f) << bitShift;
                bitShift += 7;
                if ((nextByte & 0x80) == 0) return result;
            }
        }

        #endregion        

        #region Int64

        public override Int64 ReadInt64()
        {
            var typeCode = readTypeCode();

            switch (typeCode)
            { 
                case SerializedType.ZeroInt64Type:
                    return 0;

                case SerializedType.OneInt64Type:
                    return 1;

                case SerializedType.MinusOneInt64Type:
                    return -1;

                case SerializedType.OptimizedInt64Type:
                    return ReadOptimizedInt64();

                case SerializedType.OptimizedInt64NegativeType:
                    return -ReadOptimizedInt64() - 1;

                case SerializedType.Int64Type:
                    return base.ReadInt64();

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        }

        /// <summary>
        /// Returns an Int64 value from the stream that was stored optimized.
        /// </summary>
        /// <returns>An Int64 value.</returns>
        private long ReadOptimizedInt64()
        {
            long result = 0;
            int bitShift = 0;
            while (true)
            {
                byte nextByte = base.ReadByte();
                result |= ((long)nextByte & 0x7f) << bitShift;
                bitShift += 7;
                if ((nextByte & 0x80) == 0) return result;
            }
        }

        #endregion

        #region Double

        public override Double ReadDouble()
        {
            var typeCode = readTypeCode();

            switch (typeCode)
            { 
                case SerializedType.ZeroDoubleType:
                    return 0.0;

                case SerializedType.OneDoubleType:
                    return 1.0;

                case SerializedType.DoubleType:
                    return base.ReadDouble();

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        }

        #endregion

        #region UInt32

        public override UInt32 ReadUInt32()
        {
            var typeCode = readTypeCode();

            switch (typeCode)
            { 
                case SerializedType.ZeroUInt32Type:
                    return 0;
                
                case SerializedType.OneUInt32Type:
                    return 1;
                
                case SerializedType.OptimizedUInt32Type:
                    return ReadOptimizedUInt32();

                case SerializedType.UInt32Type:
                    return base.ReadUInt32();

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        }    

        /// <summary>
        /// Returns a UInt32 value from the stream that was stored optimized.
        /// </summary>
        /// <returns>A UInt32 value.</returns>
        //[CLSCompliant(false)]
        private uint ReadOptimizedUInt32()
        {
            uint result = 0;
            int bitShift = 0;
            while (true)
            {
                byte nextByte = base.ReadByte();
                result |= ((uint)nextByte & 0x7f) << bitShift;
                bitShift += 7;
                if ((nextByte & 0x80) == 0) return result;
            }
        }

        #endregion                

        #region List types

        /// <summary>
        /// Returns a new, simple generic dictionary populated with keys and values from the stream.
        /// </summary>
        /// <typeparam name="K">The key myObjectStream.</typeparam>
        /// <typeparam name="V">The value myObjectStream.</typeparam>
        /// <returns>A new, simple, populated generic Dictionary.</returns>
        public Dictionary<K, V> ReadDictionary<K, V>()
        {
            Dictionary<K, V> result = new Dictionary<K, V>();
            ReadDictionary(result);
            return result;
        }

        /// <summary>
        /// Populates a pre-existing generic dictionary with keys and values from the stream.
        /// This allows a generic dictionary to be created without using the default constructor.
        /// </summary>
        /// <typeparam name="K">The key myObjectStream.</typeparam>
        /// <typeparam name="V">The value myObjectStream.</typeparam>
        public void ReadDictionary<K, V>(Dictionary<K, V> dictionary)
        {

            K[] keys = (K[])processArrayTypes(readTypeCode(), typeof(K));
            V[] values = (V[])processArrayTypes(readTypeCode(), typeof(V));

            if (dictionary == null) dictionary = new Dictionary<K, V>(keys.Length);
            for (int i = 0; i < keys.Length; i++)
            {
                dictionary.Add(keys[i], values[i]);
            }
        }

        /// <summary>
        /// Returns a generic List populated with values from the stream.
        /// </summary>
        /// <typeparam name="T">The list myObjectStream.</typeparam>
        /// <returns>A new generic List.</returns>
        public List<T> ReadList<T>()
        {
            return new List<T>((T[])processArrayTypes(readTypeCode(), typeof(T)));
        }

        #endregion

        #region nullable values

        /// <summary>
        /// Returns a Nullable struct from the stream.
        /// The value returned must be cast to the correct Nullable type.
        /// Synonym for ReadObject();
        /// </summary>
        /// <returns>A struct value or null</returns>
        public ValueType ReadNullable()
        {
            return (ValueType)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Boolean from the stream.
        /// </summary>
        /// <returns>A Nullable Boolean.</returns>
        public Boolean? ReadNullableBoolean()
        {
            return (bool?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Byte from the stream.
        /// </summary>
        /// <returns>A Nullable Byte.</returns>
        public Byte? ReadNullableByte()
        {
            return (byte?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Char from the stream.
        /// </summary>
        /// <returns>A Nullable Char.</returns>
        public Char? ReadNullableChar()
        {
            return (char?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable DateTime from the stream.
        /// </summary>
        /// <returns>A Nullable DateTime.</returns>
        public DateTime? ReadNullableDateTime()
        {
            return (DateTime?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Decimal from the stream.
        /// </summary>
        /// <returns>A Nullable Decimal.</returns>
        public Decimal? ReadNullableDecimal()
        {
            return (decimal?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Double from the stream.
        /// </summary>
        /// <returns>A Nullable Double.</returns>
        public Double? ReadNullableDouble()
        {
            return (double?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Guid from the stream.
        /// </summary>
        /// <returns>A Nullable Guid.</returns>
        public Guid? ReadNullableGuid()
        {
            return (Guid?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Int16 from the stream.
        /// </summary>
        /// <returns>A Nullable Int16.</returns>
        public Int16? ReadNullableInt16()
        {
            return (short?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Int32 from the stream.
        /// </summary>
        /// <returns>A Nullable Int32.</returns>
        public Int32? ReadNullableInt32()
        {
            return (int?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Int64 from the stream.
        /// </summary>
        /// <returns>A Nullable Int64.</returns>
        public Int64? ReadNullableInt64()
        {
            return (long?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable SByte from the stream.
        /// </summary>
        /// <returns>A Nullable SByte.</returns>
        //[CLSCompliant(false)]
        public SByte? ReadNullableSByte()
        {
            return (sbyte?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable Single from the stream.
        /// </summary>
        /// <returns>A Nullable Single.</returns>
        public Single? ReadNullableSingle()
        {
            return (float?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable TimeSpan from the stream.
        /// </summary>
        /// <returns>A Nullable TimeSpan.</returns>
        public TimeSpan? ReadNullableTimeSpan()
        {
            return (TimeSpan?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable UInt16 from the stream.
        /// </summary>
        /// <returns>A Nullable UInt16.</returns>
        //[CLSCompliant(false)]
        public UInt16? ReadNullableUInt16()
        {
            return (ushort?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable UInt32 from the stream.
        /// </summary>
        /// <returns>A Nullable UInt32.</returns>
        //[CLSCompliant(false)]
        public UInt32? ReadNullableUInt32()
        {
            return (uint?)ReadObject();
        }

        /// <summary>
        /// Returns a Nullable UInt64 from the stream.
        /// </summary>
        /// <returns>A Nullable UInt64.</returns>
        //[CLSCompliant(false)]
        public UInt64? ReadNullableUInt64()
        {
            return (ulong?)ReadObject();
        }

        #endregion        

        #region Boolean

        public override Boolean ReadBoolean()
        {
            var typeCode = (SerializedType)base.ReadByte();

            switch (typeCode)
            { 
                case SerializedType.BooleanFalseType:
                    return false;

                case SerializedType.BooleanTrueType:
                    return true;

                default:
                    throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        }

        #endregion      

        /// <summary>
        /// Allows an existing object, implementing IOwnedDataSerializable, to 
        /// retrieve its owned data from the stream.
        /// </summary>
        /// <param name="target">Any IOwnedDataSerializable object.</param>
        /// <param name="context">An optional, arbitrary object to allow context to be provided.</param>
        public void ReadOwnedData(IOwnedDataSerializable target, object context)
        {
            target.DeserializeOwnedData(this, context);
        }

        /// <summary>
        /// Returns the object associated with the object token read next from the stream.
        /// </summary>
        /// <returns>An object.</returns>
        public object ReadTokenizedObject()
        {
            return objectTokens[ReadOptimizedInt32()];
        }
        #endregion Methods

        #region Private Methods
        /// <summary>
        /// Returns a TimeSpan decoded from packed data.
        /// This routine is called from ReadOptimizedDateTime() and ReadOptimizedTimeSpan().
        /// <remarks>
        /// This routine uses a parameter to allow ReadOptimizedDateTime() to 'peek' at the
        /// next byte and extract the DateTimeKind from bits one and two (IsNegative and HasDays)
        /// which are never set for a Time portion of a DateTime.
        /// </remarks>
        /// </summary>
        /// <param name="initialByte">The first of two always-present bytes.</param>
        /// <returns>A decoded TimeSpan</returns>
        private TimeSpan decodeTimeSpan(byte initialByte)
        {
            bool hasTime;
            bool hasSeconds;
            bool hasMilliseconds;
            long ticks = 0;

            BitVector32 packedData = new BitVector32(initialByte | (base.ReadByte() << 8)); // Read first two bytes
            hasTime = packedData[SerializationWriter.HasTimeSection] == 1;
            hasSeconds = packedData[SerializationWriter.HasSecondsSection] == 1;
            hasMilliseconds = packedData[SerializationWriter.HasMillisecondsSection] == 1;

            if (hasMilliseconds)
                packedData = new BitVector32(packedData.Data | (base.ReadByte() << 16) | (base.ReadByte() << 24));
            else if (hasSeconds && hasTime)
            {
                packedData = new BitVector32(packedData.Data | (base.ReadByte() << 16));
            }

            if (hasTime)
            {
                ticks += packedData[SerializationWriter.HoursSection] * TimeSpan.TicksPerHour;
                ticks += packedData[SerializationWriter.MinutesSection] * TimeSpan.TicksPerMinute;
            }

            if (hasSeconds)
            {
                ticks += packedData[(!hasTime && !hasMilliseconds) ? SerializationWriter.MinutesSection
                                                                     : SerializationWriter.SecondsSection] * TimeSpan.TicksPerSecond;
            }

            if (hasMilliseconds)
            {
                ticks += packedData[SerializationWriter.MillisecondsSection] * TimeSpan.TicksPerMillisecond;
            }

            if (packedData[SerializationWriter.HasDaysSection] == 1)
            {
                ticks += ReadOptimizedInt32() * TimeSpan.TicksPerDay;
            }

            if (packedData[SerializationWriter.IsNegativeSection] == 1)
            {
                ticks = -ticks;
            }

            return new TimeSpan(ticks);
        }

        /// <summary>
        /// Creates a BitArray representing which elements of a typed array
        /// are serializable.
        /// </summary>
        /// <param name="serializedType">The type of typed array.</param>
        /// <returns>A BitArray denoting which elements are serializable.</returns>
        private BitArray readTypedArrayOptimizeFlags(SerializedType serializedType)
        {
            BitArray optimizableFlags = null;
            if (serializedType == SerializedType.FullyOptimizedTypedArrayType)
                optimizableFlags = FullyOptimizableTypedArray;
            else if (serializedType == SerializedType.PartiallyOptimizedTypedArrayType)
            {
                optimizableFlags = ReadOptimizedBitArray();
            }
            return optimizableFlags;
        }

        /// <summary>
        /// Returns an object based on supplied SerializedType.
        /// </summary>
        /// <returns>An object instance.</returns>
        private object processObject(SerializedType typeCode)
        {
            if (typeCode == SerializedType.NullType)
                return null;

            else if (typeCode == SerializedType.Int32Type)
                return base.ReadInt32();

            else if (typeCode == SerializedType.EmptyStringType)
                return string.Empty;

            else if (typeCode < SerializedType.NullType)
                return readTokenizedString((int)typeCode);

            else if (typeCode == SerializedType.BooleanFalseType)
                return false;

            else if (typeCode == SerializedType.ZeroInt32Type)
                return (Int32)0;

            else if (typeCode == SerializedType.OptimizedInt32Type)
                return ReadOptimizedInt32();

            else if (typeCode == SerializedType.OptimizedInt32NegativeType)
                return -ReadOptimizedInt32() - 1;

            else if (typeCode == SerializedType.DecimalType)
                return ReadOptimizedDecimal();

            else if (typeCode == SerializedType.ZeroDecimalType)
                return (Decimal)0;

            else if (typeCode == SerializedType.YStringType)
                return "Y";

            else if (typeCode == SerializedType.DateTimeType)
                return ReadDateTime();

            else if (typeCode == SerializedType.OptimizedDateTimeType)
                return ReadOptimizedDateTime();

            else if (typeCode == SerializedType.SingleCharStringType)
                return Char.ToString(ReadChar());

            else if (typeCode == SerializedType.SingleSpaceType)
                return " ";

            else if (typeCode == SerializedType.OneInt32Type)
                return (Int32)1;

            else if (typeCode == SerializedType.OptimizedInt16Type)
                return ReadOptimizedInt16();

            else if (typeCode == SerializedType.OptimizedInt16NegativeType)
                return -ReadOptimizedInt16() - 1;

            else if (typeCode == SerializedType.OneDecimalType)
                return (Decimal)1;

            else if (typeCode == SerializedType.BooleanTrueType)
                return true;

            else if (typeCode == SerializedType.NStringType)
                return "N";

            else if (typeCode == SerializedType.DBNullType)
                return DBNull.Value;

            else if (typeCode == SerializedType.ObjectArrayType)
                return ReadOptimizedObjectArray();

            else if (typeCode == SerializedType.EmptyObjectArrayType)
                return new object[0];

            else if (typeCode == SerializedType.MinusOneInt32Type)
                return (Int32)(-1);

            else if (typeCode == SerializedType.MinusOneInt64Type)
                return (Int64)(-1);

            else if (typeCode == SerializedType.MinusOneInt16Type)
                return (Int16)(-1);

            else if (typeCode == SerializedType.MinDateTimeType)
                return DateTime.MinValue;

            else if (typeCode == SerializedType.GuidType)
                return ReadGuid();

            else if (typeCode == SerializedType.EmptyGuidType)
                return Guid.Empty;

            else if (typeCode == SerializedType.TimeSpanType)
                return ReadTimeSpan();

            else if (typeCode == SerializedType.MaxDateTimeType)
                return DateTime.MaxValue;

            else if (typeCode == SerializedType.ZeroTimeSpanType)
                return TimeSpan.Zero;

            else if (typeCode == SerializedType.OptimizedTimeSpanType)
                return ReadOptimizedTimeSpan();

            else if (typeCode == SerializedType.DoubleType)
                return base.ReadDouble();

            else if (typeCode == SerializedType.ZeroDoubleType)
                return (Double)0;

            else if (typeCode == SerializedType.Int64Type)
                return base.ReadInt64();

            else if (typeCode == SerializedType.ZeroInt64Type)
                return (Int64)0;

            else if (typeCode == SerializedType.OptimizedInt64Type)
                return ReadOptimizedInt64();

            else if (typeCode == SerializedType.OptimizedInt64NegativeType)
                return -ReadOptimizedInt64() - 1;

            else if (typeCode == SerializedType.Int16Type)
                return ReadInt16();

            else if (typeCode == SerializedType.ZeroInt16Type)
                return (Int16)0;

            else if (typeCode == SerializedType.SingleType)
                return ReadSingle();

            else if (typeCode == SerializedType.ZeroSingleType)
                return (Single)0;

            else if (typeCode == SerializedType.ByteType)
                return base.ReadByte();

            else if (typeCode == SerializedType.ZeroByteType)
                return (Byte)0;

            else if (typeCode == SerializedType.OtherType)
                return new BinaryFormatter().Deserialize(BaseStream);

            else if (typeCode == SerializedType.UInt16Type)
                return base.ReadUInt16();

            else if (typeCode == SerializedType.ZeroUInt16Type)
                return (UInt16)0;

            else if (typeCode == SerializedType.UInt32Type)
                return base.ReadUInt32();

            else if (typeCode == SerializedType.ZeroUInt32Type)
                return (UInt32)0;

            else if (typeCode == SerializedType.OptimizedUInt32Type)
                return ReadOptimizedUInt32();

            else if (typeCode == SerializedType.UInt64Type)
                return base.ReadUInt64();

            else if (typeCode == SerializedType.ZeroUInt64Type)
                return (UInt64)0;

            else if (typeCode == SerializedType.OptimizedUInt64Type)
                return ReadOptimizedUInt64();

            else if (typeCode == SerializedType.BitVector32Type)
                return ReadBitVector32();

            else if (typeCode == SerializedType.CharType)
                return ReadChar();

            else if (typeCode == SerializedType.ZeroCharType)
                return (Char)0;

            else if (typeCode == SerializedType.SByteType)
                return ReadSByte();

            else if (typeCode == SerializedType.ZeroSByteType)
                return (SByte)0;

            else if (typeCode == SerializedType.OneByteType)
                return (Byte)1;

            else if (typeCode == SerializedType.OneDoubleType)
                return (Double)1;

            else if (typeCode == SerializedType.OneCharType)
                return (Char)1;

            else if (typeCode == SerializedType.OneInt16Type)
                return (Int16)1;

            else if (typeCode == SerializedType.OneInt64Type)
                return (Int64)1;

            else if (typeCode == SerializedType.OneUInt16Type)
                return (UInt16)1;

            else if (typeCode == SerializedType.OptimizedUInt16Type)
                return ReadOptimizedUInt16();

            else if (typeCode == SerializedType.OneUInt32Type)
                return (UInt32)1;

            else if (typeCode == SerializedType.OneUInt64Type)
                return (UInt64)1;

            else if (typeCode == SerializedType.OneSByteType)
                return (SByte)1;

            else if (typeCode == SerializedType.OneSingleType)
                return (Single)1;

            else if (typeCode == SerializedType.BitArrayType)
                return ReadOptimizedBitArray();

            else if (typeCode == SerializedType.TypeType)
                return Type.GetType(ReadOptimizedString(), false);

            else if (typeCode == SerializedType.ArrayListType)
                return ReadOptimizedArrayList();

            else if (typeCode == SerializedType.SingleInstanceType)
            {
                try
                {
                    Type type = Type.GetType(ReadStringDirect());
                    return Activator.CreateInstance(type, true);
                }
                catch
                {
                    return null;
                }
            }

            else if (typeCode == SerializedType.OwnedDataSerializableAndRecreatableType)
            {
                Type structType = ReadOptimizedType();
                object result = Activator.CreateInstance(structType);
                ReadOwnedData((IOwnedDataSerializable)result, null);
                return result;
            }

            else if (typeCode == SerializedType.OptimizedEnumType)
            {
                Type enumType = ReadOptimizedType();
                Type underlyingType = Enum.GetUnderlyingType(enumType);
                if (underlyingType == typeof(int) || underlyingType == typeof(uint) || underlyingType == typeof(long) || underlyingType == typeof(ulong))
                    return Enum.ToObject(enumType, ReadOptimizedUInt64());
                else
                {
                    return Enum.ToObject(enumType, base.ReadUInt64());
                }
            }

            else if (typeCode == SerializedType.EnumType)
            {
                Type enumType = ReadOptimizedType();
                Type underlyingType = Enum.GetUnderlyingType(enumType);
                if (underlyingType == typeof(Int32))
                    return Enum.ToObject(enumType, base.ReadInt32());
                else if (underlyingType == typeof(Byte))
                    return Enum.ToObject(enumType, base.ReadByte());
                else if (underlyingType == typeof(Int16))
                    return Enum.ToObject(enumType, ReadInt16());
                else if (underlyingType == typeof(UInt32))
                    return Enum.ToObject(enumType, base.ReadUInt32());
                else if (underlyingType == typeof(Int64))
                    return Enum.ToObject(enumType, base.ReadInt64());
                else if (underlyingType == typeof(SByte))
                    return Enum.ToObject(enumType, ReadSByte());
                else if (underlyingType == typeof(UInt16))
                    return Enum.ToObject(enumType, ReadUInt16());
                else
                {
                    return Enum.ToObject(enumType, base.ReadUInt64());
                }
            }

            else if (typeCode == SerializedType.SurrogateHandledType)
            {
                //Type serializedType = ReadOptimizedType();
                UInt32 SurrogateTypeCode = ReadOptimizedUInt32();
                IFastSerializationTypeSurrogate typeSurrogate = SerializationWriter.findSurrogateForType(SurrogateTypeCode);
                if (typeSurrogate != null)
                    return typeSurrogate.Deserialize(this, typeSurrogate.GetType());
                else
                    return null;
            }

            else if (typeCode == SerializedType.StringType)
            {
                return ReadStringDirect();
            }

            else
            {
                object result = processArrayTypes(typeCode, null);
                if (result != null) return result;
                throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
            }
        }

        /// <summary>
        /// Determine whether the passed-in type code refers to an array type
        /// and deserializes the array if it is.
        /// Returns null if not an array type.
        /// </summary>
        /// <param name="typeCode">The SerializedType to check.</param>
        /// <param name="defaultElementType">The myObjectStream of array element; null if to be read from stream.</param>
        /// <returns></returns>
        private object processArrayTypes(SerializedType typeCode, Type defaultElementType)
        {
            if (typeCode == SerializedType.StringArrayType)
                return ReadOptimizedStringArray();

            else if (typeCode == SerializedType.Int32ArrayType)
                return ReadInt32Array();

            else if (typeCode == SerializedType.Int64ArrayType)
                return ReadInt64Array();

            else if (typeCode == SerializedType.DecimalArrayType)
                return readDecimalArray();

            else if (typeCode == SerializedType.TimeSpanArrayType)
                return ReadTimeSpanArray();

            else if (typeCode == SerializedType.UInt32ArrayType)
                return ReadUInt32Array();

            else if (typeCode == SerializedType.UInt64ArrayType)
                return ReadUInt64Array();

            else if (typeCode == SerializedType.DateTimeArrayType)
                return ReadDateTimeArray();

            else if (typeCode == SerializedType.BooleanArrayType)
                return readBooleanArray();

            else if (typeCode == SerializedType.ByteArrayType)
                return readByteArray();

            else if (typeCode == SerializedType.CharArrayType)
                return readCharArray();

            else if (typeCode == SerializedType.DoubleArrayType)
                return readDoubleArray();

            else if (typeCode == SerializedType.SingleArrayType)
                return readSingleArray();

            else if (typeCode == SerializedType.GuidArrayType)
                return readGuidArray();

            else if (typeCode == SerializedType.SByteArrayType)
                return readSByteArray();

            else if (typeCode == SerializedType.Int16ArrayType)
                return ReadInt16Array();

            else if (typeCode == SerializedType.UInt16ArrayType)
                return ReadUInt16Array();

            else if (typeCode == SerializedType.EmptyTypedArrayType)
                return Array.CreateInstance(defaultElementType != null ? defaultElementType : ReadOptimizedType(), 0);

            else if (typeCode == SerializedType.OtherTypedArrayType)
                return ReadOptimizedObjectArray(ReadOptimizedType());

            else if (typeCode == SerializedType.ObjectArrayType)
                return ReadOptimizedObjectArray(defaultElementType);

            else if (typeCode == SerializedType.FullyOptimizedTypedArrayType ||
                     typeCode == SerializedType.PartiallyOptimizedTypedArrayType ||
                     typeCode == SerializedType.NonOptimizedTypedArrayType)
            {
                BitArray optimizeFlags = readTypedArrayOptimizeFlags(typeCode);
                int length = ReadOptimizedInt32();
                if (defaultElementType == null) defaultElementType = ReadOptimizedType();

                Array result = Array.CreateInstance(defaultElementType, length);

                for (int i = 0; i < length; i++)
                {
                    if (optimizeFlags == null)
                        result.SetValue(ReadObject(), i);
                    else if (optimizeFlags == FullyOptimizableTypedArray || !optimizeFlags[i])
                    {
                        var value = (IOwnedDataSerializable)Activator.CreateInstance(defaultElementType);
                        ReadOwnedData(value, null);
                        result.SetValue(value, i);
                    }
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Returns the string value associated with the string token read next from the stream.
        /// </summary>
        /// <returns>A DateTime value.</returns>
        private string readTokenizedString(int bucket)
        {
            return stringTokenList[(ReadOptimizedInt32() << 7) + bucket];
        }

        /// <summary>
        /// Returns the SerializedType read next from the stream.
        /// </summary>
        /// <returns>A SerializedType value.</returns>
        private SerializedType readTypeCode()
        {
            return (SerializedType)base.ReadByte();
        }
        
        #endregion Private Methods

        #region Debug
        [Conditional("DEBUG")]
        public void DumpStringTables(ArrayList list)
        {
            list.AddRange(stringTokenList);
        }
        #endregion Debug

    }

}
