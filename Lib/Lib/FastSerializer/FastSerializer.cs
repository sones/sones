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


//// TODO (ahzf): Fix enum serialization, avoid the whole classpath within the outputstream!

//// Remove the DEBUG condition if you want to always check for not optimizable values at the small expense of runtime speed
//#if DEBUG
//#define THROW_IF_NOT_OPTIMIZABLE
//#endif

//using System;
//using System.Collections;
//using System.Collections.Specialized;
//using System.Diagnostics;
//using System.IO;
//using System.Runtime.Serialization.Formatters;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Text;

//using System.Collections.Generic;

////[assembly: CLSCompliant(true)]

//namespace sones.Lib.Serializer
//{
//    /// <summary>
//    /// A SerializationWriter instance is used to store values and objects in a byte array.
//    ///
//    /// Once an instance is created, use the various methods to store the required data.
//    /// ToArray() will return a byte[] containing all of the data required for deserialization.
//    /// This can be stored in the SerializationInfo parameter in an ISerializable.GetObjectData() method.
//    /// <para/>
//    /// As an alternative to ToArray(), if you want to apply some post-processing to the serialized bytes, 
//    /// such as compression, call AppendTokenTables first to ensure that the string and object token tables 
//    /// are appended to the stream, and then cast BaseStream to MemoryStream. You can then access the
//    /// MemoryStream's internal buffer as follows:
//    /// <para/>
//    /// <example><code>
//    /// writer.AppendTokenTables();
//    /// MemoryStream stream = (MemoryStream) writer.BaseStream;
//    ///	serializedData = MiniLZO.Compress(stream.GetBuffer(), (int) stream.ObjectLength);
//    /// </code></example>
//    /// </summary>
//    public sealed class SerializationWriter : BinaryWriter
//    {
//        #region Static
//        /// <summary>
//        /// Default capacity for the underlying MemoryStream
//        /// </summary>
//        public static int DefaultCapacity = 1024;

//        /// <summary>
//        /// The Default setting for the OptimizeForSize property.
//        /// </summary>
//        public static bool DefaultOptimizeForSize = true;

//        /// <summary>
//        /// The Default setting for the PreserveDecimalScale property.
//        /// </summary>
//        public static bool DefaultPreserveDecimalScale = false;

//        /// <summary>
//        /// Holds a list of optional IFastSerializationTypeSurrogate instances which
//        /// SerializationWriter and SerializationReader will use to serialize objects
//        /// not directly supported.
//        /// It is important to use the same list on both client and server ends to ensure
//        /// that the same surrogated-types are supported.
//        /// </summary>
//        public static List<IFastSerializationTypeSurrogate> TypeSurrogates
//        {
//            get { return typeSurrogates; }
//        } static List<IFastSerializationTypeSurrogate> typeSurrogates = new List<IFastSerializationTypeSurrogate>();

//        /// <summary>
//        /// Section masks used for packing DateTime values
//        /// </summary>
//        internal static readonly BitVector32.Section DateYearMask = BitVector32.CreateSection(9999); //14 bits

//        internal static readonly BitVector32.Section DateMonthMask = BitVector32.CreateSection(12, DateYearMask); // 4 bits
//        internal static readonly BitVector32.Section DateDayMask = BitVector32.CreateSection(31, DateMonthMask); // 5 bits

//        internal static readonly BitVector32.Section DateHasTimeOrKindMask = BitVector32.CreateSection(1, DateDayMask); // 1 bit  total= 3 bytes

//        /// <summary>
//        /// Section masks used for packing TimeSpan values
//        /// </summary>
//        internal static readonly BitVector32.Section IsNegativeSection = BitVector32.CreateSection(1); //1 bit

//        internal static readonly BitVector32.Section HasDaysSection = BitVector32.CreateSection(1, IsNegativeSection); //1 bit
//        internal static readonly BitVector32.Section HasTimeSection = BitVector32.CreateSection(1, HasDaysSection); //1 bit
//        internal static readonly BitVector32.Section HasSecondsSection = BitVector32.CreateSection(1, HasTimeSection); //1 bit
//        internal static readonly BitVector32.Section HasMillisecondsSection = BitVector32.CreateSection(1, HasSecondsSection); //1 bit
//        internal static readonly BitVector32.Section HoursSection = BitVector32.CreateSection(23, HasMillisecondsSection); // 5 bits
//        internal static readonly BitVector32.Section MinutesSection = BitVector32.CreateSection(59, HoursSection); // 6 bits  total = 2 bytes
//        internal static readonly BitVector32.Section SecondsSection = BitVector32.CreateSection(59, MinutesSection); // 6 bits total = 3 bytes
//        internal static readonly BitVector32.Section MillisecondsSection = BitVector32.CreateSection(1024, SecondsSection); // 10 bits - total 31 bits = 4 bytes

//        /// <summary>
//        /// Holds the highest Int16 that can be optimized into less than the normal 2 bytes
//        /// </summary>
//        public const short HighestOptimizable16BitValue = 127; // 0x7F

//        /// <summary>
//        /// Holds the highest Int32 that can be optimized into less than the normal 4 bytes
//        /// </summary>
//        public const int HighestOptimizable32BitValue = 2097151; // 0x001FFFFF

//        /// <summary>
//        /// Holds the highest Int64 that can be optimized into less than the normal 8 bytes
//        /// </summary>
//        public const long HighestOptimizable64BitValue = 562949953421311; // 0x0001FFFFFFFFFFFF

//        // The short at which optimization fails because it takes more than 2 bytes
//        public const short OptimizationFailure16BitValue = 16384;

//        // The int at which optimization fails because it takes more than 4 bytes
//        public const int OptimizationFailure32BitValue = 268435456; // 0x10000000

//        // The long at which optimization fails because it takes more than 8 bytes
//        public const long OptimizationFailure64BitValue = 72057594037927936; // 0x0100000000000000

//        // Marker to denote that all elements in a typed array are optimizable
//        private static readonly BitArray FullyOptimizableTypedArray = new BitArray(0);
//        #endregion Static

//        #region Constructors
//        /// <summary>
//        /// Creates a FastSerializer with the Default Capacity (1kb)
//        /// </summary>
//        public SerializationWriter() : this(new MemoryStream(DefaultCapacity)) { }

//        /// <summary>
//        /// Creates a FastSerializer with the specified capacity
//        /// </summary>
//        /// <param name="capacity"></param>
//        public SerializationWriter(int capacity) : this(new MemoryStream(capacity)) { }

//        /// <summary>
//        /// Creates a FastSerializer around the specified stream
//        /// Note: The stream must be seekable in this version to allow the token table 
//        /// offset to be written on completion 
//        /// </summary>
//        /// <param name="stream">The seekable stream in which to store data</param>
//        private SerializationWriter(Stream stream)
//            : base(stream)
//        {
//            // The underlying BinaryWriter class will have already checked for null and not writable status
//            if (!stream.CanSeek) throw new InvalidOperationException("Stream must be seekable");

//            // Write placeholder for token tables offset
//            Write(0);

//            objectTokens = new ArrayList();
//            objectTokenLookup = new Hashtable();
//            stringLookup = new UniqueStringList();
//        }
//        #endregion Constructors

//        #region Fields
//        private UniqueStringList stringLookup;
//        private ArrayList objectTokens;
//        private Hashtable objectTokenLookup;
//        #endregion Fields

//        #region Properties
//        /// <summary>
//        /// Gets or Sets a boolean flag to indicate whether to optimize for size (default)
//        /// by storing data as packed bits or sections where possible.
//        /// Setting this value to false will turn off this optimization and store
//        /// data directly which increases the speed.
//        /// Note: This only affects optimization of data passed to the WriteObject method
//        /// and direct calls to the WriteOptimized methods will always pack data into
//        /// the smallest space where possible.
//        /// </summary>
//        public bool OptimizeForSize
//        {
//            get { return optimizeForSize; }
//            set { optimizeForSize = value; }
//        } bool optimizeForSize = DefaultOptimizeForSize;

//        /// <summary>
//        /// Gets or Sets a boolean flag to indicate whether to preserve the scale within
//        /// a Decimal value when it would have no effect on the represented value.
//        /// Note: a 2m value and a 2.00m value represent the same value but internally they 
//        /// are stored differently - the former has a value of 2 and a scale of 0 and
//        /// the latter has a value of 200 and a scale of 2. 
//        /// The scaling factor also preserves any trailing zeroes in a Decimal number. 
//        /// Trailing zeroes do not affect the value of a Decimal number in arithmetic or 
//        /// comparison operations. However, trailing zeroes can be revealed by the ToString 
//        /// method if an appropriate format string is applied.
//        /// From a serialization point of view, the former will take 2 bytes whereas the 
//        /// latter would take 4 bytes, therefore it is preferable to not save the scale where
//        /// it doesn't affect the represented value.
//        /// </summary>
//        public bool PreserveDecimalScale
//        {
//            get { return preserveDecimalScale; }
//            set { preserveDecimalScale = value; }
//        } bool preserveDecimalScale = DefaultPreserveDecimalScale;
//        #endregion Properties


//        /// <summary>
//        /// Returns true is the given object can be stored more efficently than
//        /// using the .NET standed serialisator.
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public static Boolean IsFastSerializeable(object value)
//        {

//            if (value == null)
//                return true;

//            else if (value is string)
//                return true;

//            else if (value is Int32)
//                return true;

//            else if (value == DBNull.Value)
//                return true;

//            else if (value is Boolean)
//                return true;

//            else if (value is Decimal)
//                return true;

//            else if (value is DateTime)
//                return true;

//            else if (value is Double)
//                return true;

//            else if (value is Single)
//                return true;

//            else if (value is Int16)
//                return true;

//            else if (value is Guid)
//                return true;

//            else if (value is Int64)
//                return true;

//            else if (value is Byte)
//                return true;

//            else if (value is Char)
//                return true;

//            else if (value is SByte)
//                return true;

//            else if (value is UInt32)
//                return true;

//            else if (value is UInt16)
//                return true;

//            else if (value is UInt64)
//                return true;

//            else if (value is TimeSpan)
//                return true;

//            else if (value is Array)
//                return true;

//            else if (value is Type)
//                return true;

//            else if (value is BitArray)
//                return true;

//            else if (value is BitVector32)
//                return true;

//            else if (isTypeRecreatable(value.GetType()))
//                return true;

//            else if (value is SingletonTypeWrapper)
//                return true;

//            else if (value is ArrayList)
//                return true;

//            else if (value is Enum)
//                return true;

//            return false;

//        }



//        #region Methods
//        /// <summary>
//        /// Writes an ArrayList into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 1 byte upwards depending on data content
//        /// Notes:
//        /// A null Arraylist takes 1 byte.
//        /// An empty ArrayList takes 2 bytes.
//        /// The contents are stored using WriteOptimized(ArrayList) which should be used
//        /// if the ArrayList is guaranteed never to be null.
//        /// </summary>
//        /// <param name="value">The ArrayList to store.</param>
//        public void Write(ArrayList value)
//        {
//            if (value == null)
//                writeTypeCode(SerializedType.NullType);
//            else
//            {
//                writeTypeCode(SerializedType.ArrayListType);
//                WriteOptimized(value);
//            }
//        }

//        /// <summary>
//        /// Writes a BitArray value into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 1 byte upwards depending on data content
//        /// Notes:
//        /// A null BitArray takes 1 byte.
//        /// An empty BitArray takes 2 bytes.
//        /// </summary>
//        /// <param name="value">The BitArray value to store.</param>
//        public void Write(BitArray value)
//        {
//            if (value == null)
//                writeTypeCode(SerializedType.NullType);
//            else
//            {
//                writeTypeCode(SerializedType.BitArrayType);
//                WriteOptimized(value);
//            }
//        }

//        /// <summary>
//        /// Writes a BitVector32 into the stream.
//        /// Stored myNumberOfBytes: 4 bytes.
//        /// </summary>
//        /// <param name="value">The BitVector32 to store.</param>
//        public void Write(BitVector32 value)
//        {
//            base.Write(value.Data);
//        }

//        /// <summary>
//        /// Writes a DateTime value into the stream.
//        /// Stored myNumberOfBytes: 8 bytes
//        /// </summary>
//        /// <param name="value">The DateTime value to store.</param>
//        public void Write(DateTime value)
//        {
//            Write(value.ToBinary());
//        }

//        /// <summary>
//        /// Writes a Guid into the stream.
//        /// Stored myNumberOfBytes: 16 bytes.
//        /// </summary>
//        /// <param name="value"></param>
//        public void Write(Guid value)
//        {
//            base.Write(value.ToByteArray());
//        }

//        /// <summary>
//        /// Allows any object implementing IOwnedDataSerializable to serialize itself
//        /// into this SerializationWriter.
//        /// A context may also be used to give the object an indication of what data
//        /// to store. As an example, using a BitVector32 gives a list of flags and
//        /// the object can conditionally store data depending on those flags.
//        /// </summary>
//        /// <param name="target">The IOwnedDataSerializable object to ask for owned data</param>
//        /// <param name="context">An arbtritrary object but BitVector32 recommended</param>
//        public void Write(IOwnedDataSerializable target, object context)
//        {
//            target.SerializeOwnedData(this, context);
//        }

//        /// <summary>
//        /// Stores an object into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 1 byte upwards depending on type and/or content.
//        /// 
//        /// 1 byte: null, DBNull.Value, Boolean
//        /// 
//        /// 1 to 2 bytes: Int16, UInt16, Byte, SByte, Char, 
//        /// 
//        /// 1 to 4 bytes: Int32, UInt32, Single, BitVector32
//        /// 
//        /// 1 to 8 bytes: DateTime, TimeSpan, Double, Int64, UInt64
//        /// 
//        /// 1 or 16 bytes: Guid
//        /// 
//        /// 1 plus content: string, object[], byte[], char[], BitArray, myObjectStream, ArrayList
//        /// 
//        /// Any other object be stored using a .Net Binary formatter but this should 
//        /// only be allowed as a last resort:
//        /// Since this is effectively a different serialization session, there is a 
//        /// possibility of the same shared object being serialized twice or, if the 
//        /// object has a reference directly or indirectly back to the parent object, 
//        /// there is a risk of looping which will throw an exception.
//        /// 
//        /// The type of object is checked with the most common types being checked first.
//        /// Each 'section' can be reordered to provide optimum speed but the check for
//        /// null should always be first and the default serialization always last.
//        /// 
//        /// Once the type is identified, a SerializedType byte is stored in the stream
//        /// followed by the data for the object (certain types/values may not require
//        /// _Storage of data as the SerializedType may imply the value).
//        /// 
//        /// For certain objects, if the value is within a certain range then optimized
//        /// _Storage may be used. If the value doesn't meet the required optimization
//        /// criteria then the value is stored directly.
//        /// The checks for optimization may be disabled by setting the OptimizeForSize
//        /// property to false in which case the value is stored directly. This could 
//        /// result in a slightly larger stream but there will be a speed increate to
//        /// compensate.
//        /// </summary>
//        /// <param name="value">The object to store.</param>
//        public void WriteObject(object value)
//        {
//            if (value == null)
//                writeTypeCode(SerializedType.NullType);

//            else if (value is string)
//                WriteOptimized((string)value);

//            else if (value is Int32)
//            {
//                Int32 int32Value = (int)value;
//                if (int32Value == (Int32)0)
//                    writeTypeCode(SerializedType.ZeroInt32Type);
//                else if (int32Value == (Int32)(-1))
//                    writeTypeCode(SerializedType.MinusOneInt32Type);
//                else if (int32Value == (Int32)1)
//                    writeTypeCode(SerializedType.OneInt32Type);
//                else
//                {
//                    if (optimizeForSize)
//                    {
//                        if (int32Value > 0)
//                        {
//                            if (int32Value <= HighestOptimizable32BitValue)
//                            {
//                                writeTypeCode(SerializedType.OptimizedInt32Type);
//                                write7bitEncodedSigned32BitValue(int32Value);
//                                return;
//                            }
//                        }
//                        else
//                        {
//                            Int32 positiveInt32Value = -(int32Value + 1);
//                            if (positiveInt32Value <= HighestOptimizable32BitValue)
//                            {
//                                writeTypeCode(SerializedType.OptimizedInt32NegativeType);
//                                write7bitEncodedSigned32BitValue(positiveInt32Value);
//                                return;
//                            }
//                        }
//                    }

//                    writeTypeCode(SerializedType.Int32Type);
//                    Write(int32Value);
//                }
//            }

//            else if (value == DBNull.Value)
//            {
//                writeTypeCode(SerializedType.DBNullType);
//            }

//            else if (value is Boolean)
//                writeTypeCode((bool)value ? SerializedType.BooleanTrueType : SerializedType.BooleanFalseType);

//            else if (value is Decimal)
//            {
//                Decimal decimalValue = (Decimal)value;
//                if (decimalValue == (Decimal)0)
//                    writeTypeCode(SerializedType.ZeroDecimalType);
//                else if (decimalValue == (Decimal)1)
//                    writeTypeCode(SerializedType.OneDecimalType);
//                else
//                {
//                    writeTypeCode(SerializedType.DecimalType);
//                    WriteOptimized(decimalValue);
//                }
//            }

//            else if (value is DateTime)
//            {
//                DateTime dateTimeValue = (DateTime)value;
//                if (dateTimeValue == DateTime.MinValue)
//                    writeTypeCode(SerializedType.MinDateTimeType);
//                else if (dateTimeValue == DateTime.MaxValue)
//                    writeTypeCode(SerializedType.MaxDateTimeType);
//                else if (optimizeForSize && (dateTimeValue.Ticks % TimeSpan.TicksPerMillisecond) == 0)
//                {
//                    writeTypeCode(SerializedType.OptimizedDateTimeType);
//                    WriteOptimized(dateTimeValue);
//                }
//                else
//                {
//                    writeTypeCode(SerializedType.DateTimeType);
//                    Write(dateTimeValue);
//                }
//            }

//            else if (value is Double)
//            {
//                Double doubleValue = (Double)value;
//                if (doubleValue == (Double)0)
//                    writeTypeCode(SerializedType.ZeroDoubleType);
//                else if (doubleValue == (Double)1)
//                    writeTypeCode(SerializedType.OneDoubleType);
//                else
//                {
//                    writeTypeCode(SerializedType.DoubleType);
//                    Write(doubleValue);
//                }
//            }

//            else if (value is Single)
//            {
//                Single singleValue = (Single)value;
//                if (singleValue == (Single)0)
//                    writeTypeCode(SerializedType.ZeroSingleType);
//                else if (singleValue == (Single)1)
//                    writeTypeCode(SerializedType.OneSingleType);
//                else
//                {
//                    writeTypeCode(SerializedType.SingleType);
//                    Write(singleValue);
//                }
//            }

//            else if (value is Int16)
//            {
//                Int16 int16Value = (Int16)value;
//                if (int16Value == (Int16)0)
//                    writeTypeCode(SerializedType.ZeroInt16Type);
//                else if (int16Value == (Int16)(-1))
//                    writeTypeCode(SerializedType.MinusOneInt16Type);
//                else if (int16Value == (Int16)1)
//                    writeTypeCode(SerializedType.OneInt16Type);
//                else
//                {
//                    if (optimizeForSize)
//                    {
//                        if (int16Value > 0)
//                        {
//                            if (int16Value <= HighestOptimizable16BitValue)
//                            {
//                                writeTypeCode(SerializedType.OptimizedInt16Type);
//                                write7bitEncodedSigned32BitValue((int)int16Value);
//                                return;
//                            }
//                        }
//                        else
//                        {
//                            Int32 positiveInt16Value = (-(int16Value + 1));
//                            if (positiveInt16Value <= HighestOptimizable16BitValue)
//                            {
//                                writeTypeCode(SerializedType.OptimizedInt16NegativeType);
//                                write7bitEncodedSigned32BitValue(positiveInt16Value);
//                                return;
//                            }
//                        }

//                    }

//                    writeTypeCode(SerializedType.Int16Type);
//                    Write(int16Value);
//                }
//            }

//            else if (value is Guid)
//            {
//                Guid guidValue = (Guid)value;
//                if (guidValue == Guid.Empty)
//                    writeTypeCode(SerializedType.EmptyGuidType);
//                else
//                {
//                    writeTypeCode(SerializedType.GuidType);
//                    Write(guidValue);
//                }
//            }

//            else if (value is Int64)
//            {
//                Int64 int64Value = (Int64)value;
//                if (int64Value == (Int64)0)
//                    writeTypeCode(SerializedType.ZeroInt64Type);
//                else if (int64Value == (Int64)(-1))
//                    writeTypeCode(SerializedType.MinusOneInt64Type);
//                else if (int64Value == (Int64)1)
//                    writeTypeCode(SerializedType.OneInt64Type);
//                else
//                {
//                    if (optimizeForSize)
//                    {
//                        if (int64Value > 0)
//                        {
//                            if (int64Value <= HighestOptimizable64BitValue)
//                            {
//                                writeTypeCode(SerializedType.OptimizedInt64Type);
//                                write7bitEncodedSigned64BitValue(int64Value);
//                                return;
//                            }
//                        }
//                        else
//                        {
//                            Int64 positiveInt64Value = -(int64Value + 1);
//                            if (positiveInt64Value <= HighestOptimizable64BitValue)
//                            {
//                                writeTypeCode(SerializedType.OptimizedInt64NegativeType);
//                                write7bitEncodedSigned64BitValue(positiveInt64Value);
//                                return;
//                            }
//                        }
//                    }

//                    writeTypeCode(SerializedType.Int64Type);
//                    Write(int64Value);
//                }
//            }

//            else if (value is Byte)
//            {
//                Byte byteValue = (Byte)value;
//                if (byteValue == (Byte)0)
//                    writeTypeCode(SerializedType.ZeroByteType);
//                else if (byteValue == (Byte)1)
//                    writeTypeCode(SerializedType.OneByteType);
//                else
//                {
//                    writeTypeCode(SerializedType.ByteType);
//                    Write(byteValue);
//                }
//            }

//            else if (value is Char)
//            {
//                Char charValue = (Char)value;
//                if (charValue == (Char)0)
//                    writeTypeCode(SerializedType.ZeroCharType);
//                else if (charValue == (Char)1)
//                    writeTypeCode(SerializedType.OneCharType);
//                else
//                {
//                    writeTypeCode(SerializedType.CharType);
//                    Write(charValue);
//                }
//            }

//            else if (value is SByte)
//            {
//                SByte sbyteValue = (SByte)value;
//                if (sbyteValue == (SByte)0)
//                    writeTypeCode(SerializedType.ZeroSByteType);
//                else if (sbyteValue == (SByte)1)
//                    writeTypeCode(SerializedType.OneSByteType);
//                else
//                {
//                    writeTypeCode(SerializedType.SByteType);
//                    Write(sbyteValue);
//                }
//            }

//            else if (value is UInt32)
//            {
//                UInt32 uint32Value = (UInt32)value;
//                if (uint32Value == (UInt32)0)
//                    writeTypeCode(SerializedType.ZeroUInt32Type);
//                else if (uint32Value == (UInt32)1)
//                    writeTypeCode(SerializedType.OneUInt32Type);
//                else if (optimizeForSize && uint32Value <= HighestOptimizable32BitValue)
//                {
//                    writeTypeCode(SerializedType.OptimizedUInt32Type);
//                    write7bitEncodedUnsigned32BitValue(uint32Value);
//                }
//                else
//                {
//                    writeTypeCode(SerializedType.UInt32Type);
//                    Write(uint32Value);
//                }
//            }

//            else if (value is UInt16)
//            {
//                UInt16 uint16Value = (UInt16)value;
//                if (uint16Value == (UInt16)0)
//                    writeTypeCode(SerializedType.ZeroUInt16Type);
//                else if (uint16Value == (UInt16)1)
//                    writeTypeCode(SerializedType.OneUInt16Type);
//                else if (optimizeForSize && uint16Value <= HighestOptimizable16BitValue)
//                {
//                    writeTypeCode(SerializedType.OptimizedUInt16Type);
//                    write7bitEncodedUnsigned32BitValue((uint)uint16Value);
//                }
//                else
//                {
//                    writeTypeCode(SerializedType.UInt16Type);
//                    Write(uint16Value);
//                }
//            }

//            else if (value is UInt64)
//            {
//                UInt64 uint64Value = (UInt64)value;
//                if (uint64Value == (UInt64)0)
//                    writeTypeCode(SerializedType.ZeroUInt64Type);
//                else if (uint64Value == (UInt64)1)
//                    writeTypeCode(SerializedType.OneUInt64Type);
//                else if (optimizeForSize && uint64Value <= HighestOptimizable64BitValue)
//                {
//                    writeTypeCode(SerializedType.OptimizedUInt64Type);
//                    WriteOptimized(uint64Value);
//                }
//                else
//                {
//                    writeTypeCode(SerializedType.UInt64Type);
//                    Write(uint64Value);
//                }
//            }

//            else if (value is TimeSpan)
//            {
//                TimeSpan timeSpanValue = (TimeSpan)value;
//                if (timeSpanValue == TimeSpan.Zero)
//                    writeTypeCode(SerializedType.ZeroTimeSpanType);
//                else if (optimizeForSize && (timeSpanValue.Ticks % TimeSpan.TicksPerMillisecond) == 0)
//                {
//                    writeTypeCode(SerializedType.OptimizedTimeSpanType);
//                    WriteOptimized(timeSpanValue);
//                }
//                else
//                {
//                    writeTypeCode(SerializedType.TimeSpanType);
//                    Write(timeSpanValue);
//                }
//            }


//            else if (value is Array)
//            {
//                writeTypedArray((Array)value, true);
//            }

//            else if (value is Type)
//            {
//                writeTypeCode(SerializedType.TypeType);
//                WriteOptimized((value as Type));
//            }

//            else if (value is BitArray)
//            {
//                writeTypeCode(SerializedType.BitArrayType);
//                WriteOptimized((BitArray)value);
//            }

//            else if (value is BitVector32)
//            {
//                writeTypeCode(SerializedType.BitVector32Type);
//                Write((BitVector32)value);
//            }

//            else if (isTypeRecreatable(value.GetType()))
//            {
//                writeTypeCode(SerializedType.OwnedDataSerializableAndRecreatableType);
//                WriteOptimized(value.GetType());
//                Write((IOwnedDataSerializable)value, null);
//            }

//            else if (value is SingletonTypeWrapper)
//            {
//                writeTypeCode(SerializedType.SingleInstanceType);
//                Type singletonType = (value as SingletonTypeWrapper).WrappedType;
//                if (singletonType.AssemblyQualifiedName.IndexOf(", mscorlib,") == -1)
//                    WriteStringDirect(singletonType.AssemblyQualifiedName);
//                else
//                {
//                    WriteStringDirect(singletonType.FullName);
//                }
//            }

//            else if (value is ArrayList)
//            {
//                writeTypeCode(SerializedType.ArrayListType);
//                WriteOptimized((value as ArrayList));
//            }

//            else if (value is Enum)
//            {
//                Type enumType = value.GetType();
//                Type underlyingType = Enum.GetUnderlyingType(enumType);

//                if (underlyingType == typeof(int) || underlyingType == typeof(uint))
//                {
//                    uint uint32Value = underlyingType == typeof(int) ? (uint)(int)value : (uint)value;
//                    if (uint32Value <= HighestOptimizable32BitValue)
//                    {
//                        writeTypeCode(SerializedType.OptimizedEnumType);
//                        WriteOptimized(enumType);
//                        write7bitEncodedUnsigned32BitValue(uint32Value);
//                    }
//                    else
//                    {
//                        writeTypeCode(SerializedType.EnumType);
//                        WriteOptimized(enumType);
//                        Write(uint32Value);
//                    }
//                }
//                else if (underlyingType == typeof(long) || underlyingType == typeof(ulong))
//                {
//                    ulong uint64value = underlyingType == typeof(long) ? (ulong)(long)value : (ulong)value;
//                    if (uint64value <= HighestOptimizable64BitValue)
//                    {
//                        writeTypeCode(SerializedType.OptimizedEnumType);
//                        WriteOptimized(enumType);
//                        write7bitEncodedUnsigned64BitValue(uint64value);
//                    }
//                    else
//                    {
//                        writeTypeCode(SerializedType.EnumType);
//                        WriteOptimized(enumType);
//                        Write(uint64value);
//                    }
//                }
//                else
//                {
//                    writeTypeCode(SerializedType.EnumType);
//                    WriteOptimized(enumType);
//                    if (underlyingType == typeof(byte))
//                        Write((byte)value);
//                    else if (underlyingType == typeof(sbyte))
//                        Write((sbyte)value);
//                    else if (underlyingType == typeof(short))
//                        Write((short)value);
//                    else
//                    {
//                        Write((ushort)value);
//                    }
//                }
//            }
//            else
//            {
//                Type valueType = value.GetType();
//                IFastSerializationTypeSurrogate typeSurrogate = findSurrogateForType(valueType);
//                if (typeSurrogate != null)
//                {
//                    writeTypeCode(SerializedType.SurrogateHandledType);
//                    WriteOptimized(valueType);
//                    typeSurrogate.Serialize(this, value);
//                }
//                else
//                {
//                    writeTypeCode(SerializedType.OtherType);
//                    createBinaryFormatter().Serialize(BaseStream, value);
//                }
//            }
//        }

//        /// <summary>
//        /// Calls WriteOptimized(string).
//        /// This override to hide base BinaryWriter.Write(string).
//        /// </summary>
//        /// <param name="value">The string to store.</param>
//        public override void Write(string value)
//        {
//            WriteOptimized(value);
//        }

//        /// <summary>
//        /// Writes a TimeSpan value into the stream.
//        /// Stored myNumberOfBytes: 8 bytes
//        /// </summary>
//        /// <param name="value">The TimeSpan value to store.</param>
//        public void Write(TimeSpan value)
//        {
//            Write(value.Ticks);
//        }

//        /// <summary>
//        /// Stores a myObjectStream object into the stream.
//        /// Stored myNumberOfBytes: Depends on the length of the myObjectStream's name and whether the fullyQualified parameter is set.
//        /// A null myObjectStream takes 1 byte.
//        /// </summary>
//        /// <param name="value">The myObjectStream to store.</param>
//        /// <param name="fullyQualified">true to store the AssemblyQualifiedName or false to store the FullName. </param>
//        public void Write(Type value, bool fullyQualified)
//        {
//            if (value == null)
//                writeTypeCode(SerializedType.NullType);
//            else
//            {
//                writeTypeCode(SerializedType.TypeType);
//                WriteOptimized(fullyQualified ? value.AssemblyQualifiedName : value.FullName);
//            }
//        }

//        /// <summary>
//        /// Writes an non-null ArrayList into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 1 byte upwards depending on data content
//        /// Notes:
//        /// An empty ArrayList takes 1 byte.
//        /// </summary>
//        /// <param name="value">The ArrayList to store. Must not be null.</param>
//        public void WriteOptimized(ArrayList value)
//        {
//            checkOptimizable(value != null, "Cannot optimize a null ArrayList");

//            writeObjectArray(value.ToArray());
//        }

//        /// <summary>
//        /// Writes a BitArray into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 1 byte upwards depending on data content
//        /// Notes:
//        /// An empty BitArray takes 1 byte.
//        /// </summary>
//        /// <param name="value">The BitArray value to store. Must not be null.</param>
//        public void WriteOptimized(BitArray value)
//        {
//            checkOptimizable(value != null, "Cannot optimize a null BitArray");

//            write7bitEncodedSigned32BitValue(value.Length);

//            if (value.Length > 0)
//            {
//                byte[] data = new byte[(value.Length + 7) / 8];
//                value.CopyTo(data, 0);
//                base.Write(data, 0, data.Length);
//            }
//        }

//        /// <summary>
//        /// Writes a BitVector32 into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 1 to 4 bytes. (.Net is 4 bytes)
//        ///  1 to  7 bits takes 1 byte
//        ///  8 to 14 bits takes 2 bytes
//        /// 15 to 21 bits takes 3 bytes
//        /// 22 to 28 bits takes 4 bytes
//        /// -------------------------------------------------------------------
//        /// 29 to 32 bits takes 5 bytes - use Write(BitVector32) method instead
//        /// 
//        /// Try to order the BitVector32 masks so that the highest bits are least-likely
//        /// to be set.
//        /// </summary>
//        /// <param name="value">The BitVector32 to store. Must not use more than 28 bits.</param>
//        public void WriteOptimized(BitVector32 value)
//        {
//            checkOptimizable(value.Data < OptimizationFailure32BitValue && value.Data >= 0, "BitVector32 value is not optimizable");

//            write7bitEncodedSigned32BitValue(value.Data);
//        }

//        /// <summary>
//        /// Writes a DateTime value into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 3 bytes to 7 bytes (.Net is 8 bytes)
//        /// Notes:
//        /// A DateTime containing only a date takes 3 bytes
//        /// (except a .NET 2.0 Date with a specified DateTimeKind which will take a minimum
//        /// of 5 bytes - no further optimization for this situation felt necessary since it
//        /// is unlikely that a DateTimeKind would be specified without hh:mm also)
//        /// Date plus hh:mm takes 5 bytes.
//        /// Date plus hh:mm:ss takes 6 bytes.
//        /// Date plus hh:mm:ss.fff takes 7 bytes.
//        /// </summary>
//        /// <param name="value">The DateTime value to store. Must not contain sub-millisecond data.</param>
//        public void WriteOptimized(DateTime value)
//        {
//            checkOptimizable((value.Ticks % TimeSpan.TicksPerMillisecond) == 0, "Cannot optimize a DateTime with sub-millisecond accuracy");

//            BitVector32 dateMask = new BitVector32();
//            dateMask[DateYearMask] = value.Year;
//            dateMask[DateMonthMask] = value.Month;
//            dateMask[DateDayMask] = value.Day;

//            int initialData = 0;
//            bool writeAdditionalData = value != value.Date;

//            initialData = (int)value.Kind;
//            writeAdditionalData |= initialData != 0;

//            dateMask[DateHasTimeOrKindMask] = writeAdditionalData ? 1 : 0;

//            // Store 3 bytes of Date information
//            int dateMaskData = dateMask.Data;
//            Write((byte)dateMaskData);
//            Write((byte)(dateMaskData >> 8));
//            Write((byte)(dateMaskData >> 16));

//            if (writeAdditionalData)
//            {
//                checkOptimizable((value.Ticks % TimeSpan.TicksPerMillisecond) == 0, "Cannot optimize a DateTime with sub-millisecond accuracy");
//                encodeTimeSpan(value.TimeOfDay, true, initialData);
//            }
//        }

//        /// <summary>
//        /// Writes a Decimal value into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 1 byte to 14 bytes (.Net is 16 bytes)
//        /// Restrictions: None
//        /// </summary>
//        /// <param name="value">The Decimal value to store</param>
//        public void WriteOptimized(Decimal value)
//        {
//            int[] data = Decimal.GetBits(value);
//            byte scale = (byte)(data[3] >> 16);
//            byte flags = 0;
//            if (scale != 0 && !preserveDecimalScale && optimizeForSize)
//            {
//                decimal normalized = Decimal.Truncate(value);
//                if (normalized == value)
//                {
//                    data = Decimal.GetBits(normalized);
//                    scale = 0;
//                }
//            }

//            if ((data[3] & -2147483648) != 0) flags |= 0x01;
//            if (scale != 0) flags |= 0x02;

//            if (data[0] == 0)
//                flags |= 0x04;
//            else if (data[0] <= HighestOptimizable32BitValue && data[0] >= 0)
//            {
//                flags |= 0x20;
//            }

//            if (data[1] == 0)
//                flags |= 0x08;
//            else if (data[1] <= HighestOptimizable32BitValue && data[1] >= 0)
//            {
//                flags |= 0x40;
//            }

//            if (data[2] == 0)
//                flags |= 0x10;
//            else if (data[2] <= HighestOptimizable32BitValue && data[2] >= 0)
//            {
//                flags |= 0x80;
//            }

//            Write(flags);
//            if (scale != 0) Write(scale);
//            if ((flags & 0x04) == 0) if ((flags & 0x20) != 0) write7bitEncodedSigned32BitValue(data[0]); else Write(data[0]);
//            if ((flags & 0x08) == 0) if ((flags & 0x40) != 0) write7bitEncodedSigned32BitValue(data[1]); else Write(data[1]);
//            if ((flags & 0x10) == 0) if ((flags & 0x80) != 0) write7bitEncodedSigned32BitValue(data[2]); else Write(data[2]);
//        }

//        /// <summary>
//        /// Write an Int16 value using the fewest number of bytes possible.
//        /// </summary>
//        /// <remarks>
//        /// 0x0000 - 0x007f (0 to 127) takes 1 byte
//        /// 0x0080 - 0x03FF (128 to 16,383) takes 2 bytes
//        /// ----------------------------------------------------------------
//        /// 0x0400 - 0x7FFF (16,384 to 32,767) takes 3 bytes
//        /// All negative numbers take 3 bytes
//        /// 
//        /// Only call this method if the value is known to be between 0 and 
//        /// 16,383 otherwise use Write(Int16 value)
//        /// </remarks>
//        /// <param name="value">The Int16 to store. Must be between 0 and 16,383 inclusive.</param>
//        public void WriteOptimized(short value)
//        {
//            checkOptimizable(value < OptimizationFailure16BitValue && value >= 0, "Int16 value is not optimizable");

//            write7bitEncodedSigned32BitValue(value);
//        }

//        /// <summary>
//        /// Write an Int32 value using the fewest number of bytes possible.
//        /// </summary>
//        /// <remarks>
//        /// 0x00000000 - 0x0000007f (0 to 127) takes 1 byte
//        /// 0x00000080 - 0x000003FF (128 to 16,383) takes 2 bytes
//        /// 0x00000400 - 0x001FFFFF (16,384 to 2,097,151) takes 3 bytes
//        /// 0x00200000 - 0x0FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
//        /// ----------------------------------------------------------------
//        /// 0x10000000 - 0x07FFFFFF (268,435,456 and above) takes 5 bytes
//        /// All negative numbers take 5 bytes
//        /// 
//        /// Only call this method if the value is known to be between 0 and 
//        /// 268,435,455 otherwise use Write(Int32 value)
//        /// </remarks>
//        /// <param name="value">The Int32 to store. Must be between 0 and 268,435,455 inclusive.</param>
//        public void WriteOptimized(int value)
//        {
//            checkOptimizable(value < OptimizationFailure32BitValue && value >= 0, "Int32 value is not optimizable");

//            write7bitEncodedSigned32BitValue(value);
//        }

//        /// <summary>
//        /// Write an Int64 value using the fewest number of bytes possible.
//        /// </summary>
//        /// <remarks>
//        /// 0x0000000000000000 - 0x000000000000007f (0 to 127) takes 1 byte
//        /// 0x0000000000000080 - 0x00000000000003FF (128 to 16,383) takes 2 bytes
//        /// 0x0000000000000400 - 0x00000000001FFFFF (16,384 to 2,097,151) takes 3 bytes
//        /// 0x0000000000200000 - 0x000000000FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
//        /// 0x0000000010000000 - 0x00000007FFFFFFFF (268,435,456 to 34,359,738,367) takes 5 bytes
//        /// 0x0000000800000000 - 0x000003FFFFFFFFFF (34,359,738,368 to 4,398,046,511,103) takes 6 bytes
//        /// 0x0000040000000000 - 0x0001FFFFFFFFFFFF (4,398,046,511,104 to 562,949,953,421,311) takes 7 bytes
//        /// 0x0002000000000000 - 0x00FFFFFFFFFFFFFF (562,949,953,421,312 to 72,057,594,037,927,935) takes 8 bytes
//        /// ------------------------------------------------------------------
//        /// 0x0100000000000000 - 0x7FFFFFFFFFFFFFFF (72,057,594,037,927,936 to 9,223,372,036,854,775,807) takes 9 bytes
//        /// 0x7FFFFFFFFFFFFFFF - 0xFFFFFFFFFFFFFFFF (9,223,372,036,854,775,807 and above) takes 10 bytes
//        /// All negative numbers take 10 bytes
//        /// 
//        /// Only call this method if the value is known to be between 0 and
//        /// 72,057,594,037,927,935 otherwise use Write(Int64 value)
//        /// </remarks>
//        /// <param name="value">The Int64 to store. Must be between 0 and 72,057,594,037,927,935 inclusive.</param>
//        public void WriteOptimized(long value)
//        {
//            checkOptimizable(value < OptimizationFailure64BitValue && value >= 0, "long value is not optimizable");

//            write7bitEncodedSigned64BitValue(value);
//        }

//        /// <summary>
//        /// Writes a string value into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 1 byte upwards depending on string length
//        /// Notes:
//        /// Encodes null, Empty, 'Y', 'N', ' ' values as a single byte
//        /// Any other single char string is stored as two bytes
//        /// All other strings are stored in a string token list:
//        /// 
//        /// The TypeCode representing the current string token list is written first (1 byte), 
//        /// followed by the string token itself (1-4 bytes)
//        /// 
//        /// When the current string list has reached 128 values then a new string list
//        /// is generated and that is used for generating future string tokens. This continues
//        /// until the maximum number (128) of string lists is in use, after which the string 
//        /// lists are used in a round-robin fashion.
//        /// By doing this, more lists are created with fewer items which allows a smaller 
//        /// token size to be used for more strings.
//        /// 
//        /// The first 16,384 strings will use a 1 byte token.
//        /// The next 2,097,152 strings will use a 2 byte token. (This should suffice for most uses!)
//        /// The next 268,435,456 strings will use a 3 byte token. (My, that is a lot!!)
//        /// The next 34,359,738,368 strings will use a 4 byte token. (only shown for completeness!!!)
//        /// </summary>
//        /// <param name="value">The string to store.</param>
//        public void WriteOptimized(string value)
//        {
//            if (value == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (value.Length == 1)
//            {
//                char singleChar = value[0];
//                if (singleChar == 'Y')
//                    writeTypeCode(SerializedType.YStringType);
//                else if (singleChar == 'N')
//                    writeTypeCode(SerializedType.NStringType);
//                else if (singleChar == ' ')
//                    writeTypeCode(SerializedType.SingleSpaceType);
//                else
//                {
//                    writeTypeCode(SerializedType.SingleCharStringType);
//                    Write(singleChar);
//                }
//            }
//            else if (value.Length == 0)
//                writeTypeCode(SerializedType.EmptyStringType);
//            else
//            {
//                int stringIndex = stringLookup.Add(value);

//                Write((byte)(stringIndex % 128));
//                write7bitEncodedSigned32BitValue(stringIndex >> 7);
//            }
//        }

//        /// <summary>
//        /// Writes a TimeSpan value into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 2 bytes to 8 bytes (.Net is 8 bytes)
//        /// Notes:
//        /// hh:mm (time) are always stored together and take 2 bytes.
//        /// If seconds are present then 3 bytes unless (time) is not present in which case 2 bytes
//        /// since the seconds are stored in the minutes position.
//        /// If milliseconds are present then 4 bytes.
//        /// In addition, if days are present they will add 1 to 4 bytes to the above.
//        /// </summary>
//        /// <param name="value">The TimeSpan value to store. Must not contain sub-millisecond data.</param>
//        public void WriteOptimized(TimeSpan value)
//        {
//            checkOptimizable((value.Ticks % TimeSpan.TicksPerMillisecond) == 0, "Cannot optimize a TimeSpan with sub-millisecond accuracy");

//            encodeTimeSpan(value, false, 0);
//        }

//        /// <summary>
//        /// Stores a non-null myObjectStream object into the stream.
//        /// Stored myNumberOfBytes: Depends on the length of the myObjectStream's name.
//        /// If the type is a System type (mscorlib) then it is stored without assembly name information,
//        /// otherwise the myObjectStream's AssemblyQualifiedName is used.
//        /// </summary>
//        /// <param name="value">The myObjectStream to store. Must not be null.</param>
//        public void WriteOptimized(Type value)
//        {
//            checkOptimizable(value != null, "Cannot optimize a null Type");

//            WriteOptimized(value.AssemblyQualifiedName.IndexOf(", mscorlib,") == -1 ? value.AssemblyQualifiedName : value.FullName);
//        }

//        /// <summary>
//        /// Write a UInt16 value using the fewest number of bytes possible.
//        /// </summary>
//        /// <remarks>
//        /// 0x0000 - 0x007f (0 to 127) takes 1 byte
//        /// 0x0080 - 0x03FF (128 to 16,383) takes 2 bytes
//        /// ----------------------------------------------------------------
//        /// 0x0400 - 0xFFFF (16,384 to 65,536) takes 3 bytes
//        /// 
//        /// Only call this method if the value is known to  be between 0 and 
//        /// 16,383 otherwise use Write(UInt16 value)
//        /// </remarks>
//        /// <param name="value">The UInt16 to store. Must be between 0 and 16,383 inclusive.</param>
//        ////[CLSCompliant(false)]
//        public void WriteOptimized(ushort value)
//        {
//            checkOptimizable(value < OptimizationFailure16BitValue, "UInt16 value is not optimizable");

//            write7bitEncodedUnsigned32BitValue(value);
//        }

//        /// <summary>
//        /// Write a UInt32 value using the fewest number of bytes possible.
//        /// </summary>
//        /// </remarks>
//        /// 0x00000000 - 0x0000007f (0 to 127) takes 1 byte
//        /// 0x00000080 - 0x000003FF (128 to 16,383) takes 2 bytes
//        /// 0x00000400 - 0x001FFFFF (16,384 to 2,097,151) takes 3 bytes
//        /// 0x00200000 - 0x0FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
//        /// ----------------------------------------------------------------
//        /// 0x10000000 - 0xFFFFFFFF (268,435,456 and above) takes 5 bytes
//        /// 
//        /// Only call this method if the value is known to  be between 0 and 
//        /// 268,435,455 otherwise use Write(UInt32 value)
//        /// </remarks>
//        /// <param name="value">The UInt32 to store. Must be between 0 and 268,435,455 inclusive.</param>
//        //[CLSCompliant(false)]
//        public void WriteOptimized(uint value)
//        {
//            checkOptimizable(value < OptimizationFailure32BitValue, "UInt32 value is not optimizable");

//            write7bitEncodedUnsigned32BitValue(value);
//        }

//        /// <summary>
//        /// Write a UInt64 value using the fewest number of bytes possible.
//        /// </summary>
//        /// <remarks>
//        /// 0x0000000000000000 - 0x000000000000007f (0 to 127) takes 1 byte
//        /// 0x0000000000000080 - 0x00000000000003FF (128 to 16,383) takes 2 bytes
//        /// 0x0000000000000400 - 0x00000000001FFFFF (16,384 to 2,097,151) takes 3 bytes
//        /// 0x0000000000200000 - 0x000000000FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
//        /// 0x0000000010000000 - 0x00000007FFFFFFFF (268,435,456 to 34,359,738,367) takes 5 bytes
//        /// 0x0000000800000000 - 0x000003FFFFFFFFFF (34,359,738,368 to 4,398,046,511,103) takes 6 bytes
//        /// 0x0000040000000000 - 0x0001FFFFFFFFFFFF (4,398,046,511,104 to 562,949,953,421,311) takes 7 bytes
//        /// 0x0002000000000000 - 0x00FFFFFFFFFFFFFF (562,949,953,421,312 to 72,057,594,037,927,935) takes 8 bytes
//        /// ------------------------------------------------------------------
//        /// 0x0100000000000000 - 0x7FFFFFFFFFFFFFFF (72,057,594,037,927,936 to 9,223,372,036,854,775,807) takes 9 bytes
//        /// 0x7FFFFFFFFFFFFFFF - 0xFFFFFFFFFFFFFFFF (9,223,372,036,854,775,807 and above) takes 10 bytes
//        /// 
//        /// Only call this method if the value is known to be between 0 and
//        /// 72,057,594,037,927,935 otherwise use Write(UInt64 value)
//        /// </remarks>
//        /// <param name="value">The UInt64 to store. Must be between 0 and 72,057,594,037,927,935 inclusive.</param>
//        //[CLSCompliant(false)]
//        public void WriteOptimized(ulong value)
//        {
//            checkOptimizable(value < OptimizationFailure64BitValue, "ulong value is not optimizable");

//            write7bitEncodedUnsigned64BitValue(value);
//        }

//        /// <summary>
//        /// Writes a Boolean[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// Calls WriteOptimized(Boolean[]).
//        /// </summary>
//        /// <param name="values">The Boolean[] to store.</param>
//        public void Write(bool[] values)
//        {
//            WriteOptimized(values);
//        }

//        /// <summary>
//        /// Writes a Byte[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Byte[] to store.</param>
//        public override void Write(byte[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes a Char[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Char[] to store.</param>
//        public override void Write(char[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes a DateTime[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The DateTime[] to store.</param>
//        public void Write(DateTime[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeArray(values, null);
//            }
//        }

//        /// <summary>
//        /// Writes a Decimal[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// Calls WriteOptimized(Decimal[]).
//        /// </summary>
//        /// <param name="values">The Decimal[] to store.</param>
//        public void Write(decimal[] values)
//        {
//            WriteOptimized(values);
//        }

//        /// <summary>
//        /// Writes a Double[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Double[] to store.</param>
//        public void Write(double[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes a Single[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Single[] to store.</param>
//        public void Write(float[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes a Guid[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Guid[] to store.</param>
//        public void Write(Guid[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes an Int32[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Int32[] to store.</param>
//        public void Write(int[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeArray(values, null);
//            }
//        }

//        /// <summary>
//        /// Writes an Int64[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Int64[] to store.</param>
//        public void Write(long[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeArray(values, null);
//            }
//        }

//        /// <summary>
//        /// Writes an object[] into the stream.
//        /// Stored myNumberOfBytes: 2 bytes upwards depending on data content
//        /// Notes:
//        /// A null object[] takes 1 byte.
//        /// An empty object[] takes 2 bytes.
//        /// The contents of the array will be stored optimized.
//        /// </summary>
//        /// <param name="values">The object[] to store.</param>
//        public void Write(object[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyObjectArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.ObjectArrayType);
//                writeObjectArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes an SByte[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The SByte[] to store.</param>
//        //[CLSCompliant(false)]
//        public void Write(sbyte[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes an Int16[]or a null into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// Calls WriteOptimized(decimal[]).
//        /// </summary>
//        /// <param name="values">The Int16[] to store.</param>
//        public void Write(short[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes a TimeSpan[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The TimeSpan[] to store.</param>
//        public void Write(TimeSpan[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeArray(values, null);
//            }
//        }

//        /// <summary>
//        /// Writes a UInt32[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The UInt32[] to store.</param>
//        //[CLSCompliant(false)]
//        public void Write(uint[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeArray(values, null);
//            }
//        }

//        /// <summary>
//        /// Writes a UInt64[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The UInt64[] to store.</param>
//        //[CLSCompliant(false)]
//        public void Write(ulong[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeArray(values, null);
//            }
//        }

//        /// <summary>
//        /// Writes a UInt16[] into the stream.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The UInt16[] to store.</param>
//        //[CLSCompliant(false)]
//        public void Write(ushort[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes an optimized Boolean[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// Stored as a BitArray.
//        /// </summary>
//        /// <param name="values">The Boolean[] to store.</param>
//        public void WriteOptimized(bool[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.FullyOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes a DateTime[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The DateTime[] to store.</param>
//        public void WriteOptimized(DateTime[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                BitArray optimizeFlags = null;
//                int notOptimizable = 0;
//                int notWorthOptimizingLimit = 1 + (int)(values.Length * (optimizeForSize ? 0.8f : 0.6f));
//                for (int i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
//                {
//                    if (values[i].Ticks % TimeSpan.TicksPerMillisecond != 0)
//                        notOptimizable++;
//                    else
//                    {
//                        if (optimizeFlags == null) optimizeFlags = new BitArray(values.Length);
//                        optimizeFlags[i] = true;
//                    }
//                }

//                if (notOptimizable == 0)
//                    optimizeFlags = FullyOptimizableTypedArray;
//                else if (notOptimizable >= notWorthOptimizingLimit)
//                {
//                    optimizeFlags = null;
//                }

//                writeArray(values, optimizeFlags);
//            }
//        }

//        /// <summary>
//        /// Writes a Decimal[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Decimal[] to store.</param>
//        public void WriteOptimized(decimal[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.FullyOptimizedTypedArrayType);
//                writeArray(values);
//            }
//        }

//        /// <summary>
//        /// Writes a not-null object[] into the stream using the fewest number of bytes possible.
//        /// Stored myNumberOfBytes: 2 bytes upwards depending on data content
//        /// Notes:
//        /// An empty object[] takes 1 byte.
//        /// The contents of the array will be stored optimized.
//        /// </summary>
//        /// <param name="values">The object[] to store. Must not be null.</param>
//        public void WriteOptimized(object[] values)
//        {
//            checkOptimizable(values != null, "Cannot optimize a null object[]");

//            writeObjectArray(values);
//        }

//        /// <summary>
//        /// Writes a pair of object[] arrays into the stream using the fewest number of bytes possible.
//        /// The arrays must not be null and must have the same length
//        /// The first array's values are written optimized
//        /// The second array's values are compared against the first and, where identical, will be stored
//        /// using a single byte.
//        /// Useful for storing entity data where there is a before-change and after-change set of value pairs
//        /// and, typically, only a few of the values will have changed.
//        /// </summary>
//        /// <param name="values1">The first object[] value which must not be null and must have the same length as values2</param>
//        /// <param name="values2">The second object[] value which must not be null and must have the same length as values1</param>
//        public void WriteOptimized(object[] values1, object[] values2)
//        {
//            checkOptimizable(values1 != null && values2 != null, "Cannot optimimize an object[] pair that is null");
//            checkOptimizable(values1.Length == values2.Length, "Cannot optimize an object[] pair with different lengths");

//            writeObjectArray(values1);
//            int lastIndex = values2.Length - 1;
//            for (int i = 0; i < values2.Length; i++)
//            {
//                object value2 = values2[i];

//                if (value2 == null ? values1[i] == null : value2.Equals(values1[i]))
//                {
//                    int duplicates = 0;
//                    for (; i < lastIndex && (values2[i + 1] == null ? values1[i + 1] == null : values2[i + 1].Equals(values1[i + 1])); i++) duplicates++;
//                    if (duplicates == 0)
//                        writeTypeCode(SerializedType.DuplicateValueType);
//                    else
//                    {
//                        writeTypeCode(SerializedType.DuplicateValueSequenceType);
//                        write7bitEncodedSigned32BitValue(duplicates);
//                    }
//                }
//                else if (value2 == null)
//                {
//                    int duplicates = 0;
//                    for (; i < lastIndex && values2[i + 1] == null; i++) duplicates++;
//                    if (duplicates == 0)
//                        writeTypeCode(SerializedType.NullType);
//                    else
//                    {
//                        writeTypeCode(SerializedType.NullSequenceType);
//                        write7bitEncodedSigned32BitValue(duplicates);
//                    }
//                }
//                else if (value2 == DBNull.Value)
//                {
//                    int duplicates = 0;
//                    for (; i < lastIndex && values2[i + 1] == DBNull.Value; i++) duplicates++;
//                    if (duplicates == 0)
//                        writeTypeCode(SerializedType.DBNullType);
//                    else
//                    {
//                        writeTypeCode(SerializedType.DBNullSequenceType);
//                        write7bitEncodedSigned32BitValue(duplicates);
//                    }
//                }
//                else
//                {
//                    WriteObject(value2);
//                }
//            }
//        }

//        /// <summary>
//        /// Writes an Int16[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Int16[] to store.</param>
//        public void WriteOptimized(short[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                BitArray optimizeFlags = null;
//                int notOptimizable = 0;
//                int notWorthOptimizingLimit = 1 + (int)(values.Length * (optimizeForSize ? 0.8f : 0.6f));
//                for (int i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
//                {
//                    if (values[i] < 0 || values[i] > HighestOptimizable16BitValue)
//                        notOptimizable++;
//                    else
//                    {
//                        if (optimizeFlags == null) optimizeFlags = new BitArray(values.Length);
//                        optimizeFlags[i] = true;
//                    }
//                }

//                if (notOptimizable == 0)
//                    optimizeFlags = FullyOptimizableTypedArray;
//                else if (notOptimizable >= notWorthOptimizingLimit)
//                {
//                    optimizeFlags = null;
//                }

//                writeArray(values, optimizeFlags);
//            }
//        }


//        /// <summary>
//        /// Writes an Int32[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Int32[] to store.</param>
//        public void WriteOptimized(int[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                BitArray optimizeFlags = null;
//                int notOptimizable = 0;
//                int notWorthOptimizingLimit = 1 + (int)(values.Length * (optimizeForSize ? 0.8f : 0.6f));
//                for (int i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
//                {
//                    if (values[i] < 0 || values[i] > HighestOptimizable32BitValue)
//                        notOptimizable++;
//                    else
//                    {
//                        if (optimizeFlags == null) optimizeFlags = new BitArray(values.Length);
//                        optimizeFlags[i] = true;
//                    }
//                }

//                if (notOptimizable == 0)
//                    optimizeFlags = FullyOptimizableTypedArray;
//                else if (notOptimizable >= notWorthOptimizingLimit)
//                {
//                    optimizeFlags = null;
//                }

//                writeArray(values, optimizeFlags);
//            }
//        }

//        /// <summary>
//        /// Writes an Int64[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The Int64[] to store.</param>
//        public void WriteOptimized(long[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                BitArray optimizeFlags = null;
//                int notOptimizable = 0;
//                int notWorthOptimizingLimit = 1 + (int)(values.Length * (optimizeForSize ? 0.8f : 0.6f));

//                for (int i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
//                {
//                    if (values[i] < 0 || values[i] > HighestOptimizable64BitValue)
//                        notOptimizable++;
//                    else
//                    {
//                        if (optimizeFlags == null) optimizeFlags = new BitArray(values.Length);
//                        optimizeFlags[i] = true;
//                    }
//                }

//                if (notOptimizable == 0)
//                    optimizeFlags = FullyOptimizableTypedArray;
//                else if (notOptimizable >= notWorthOptimizingLimit)
//                {
//                    optimizeFlags = null;
//                }

//                writeArray(values, optimizeFlags);
//            }
//        }

//        /// <summary>
//        /// Writes a TimeSpan[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The TimeSpan[] to store.</param>
//        public void WriteOptimized(TimeSpan[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                BitArray optimizeFlags = null;
//                int notOptimizable = 0;
//                int notWorthOptimizingLimit = 1 + (int)(values.Length * (optimizeForSize ? 0.8f : 0.6f));
//                for (int i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
//                {
//                    if (values[i].Ticks % TimeSpan.TicksPerMillisecond != 0)
//                        notOptimizable++;
//                    else
//                    {
//                        if (optimizeFlags == null) optimizeFlags = new BitArray(values.Length);
//                        optimizeFlags[i] = true;
//                    }
//                }

//                if (notOptimizable == 0)
//                    optimizeFlags = FullyOptimizableTypedArray;
//                else if (notOptimizable >= notWorthOptimizingLimit)
//                {
//                    optimizeFlags = null;
//                }

//                writeArray(values, optimizeFlags);
//            }
//        }

//        /// <summary>
//        /// Writes a UInt16[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The UInt16[] to store.</param>
//        //[CLSCompliant(false)]
//        public void WriteOptimized(ushort[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                BitArray optimizeFlags = null;
//                int notOptimizable = 0;
//                int notWorthOptimizingLimit = 1 + (int)(values.Length * (optimizeForSize ? 0.8f : 0.6f));
//                for (int i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
//                {
//                    if (values[i] > HighestOptimizable16BitValue)
//                        notOptimizable++;
//                    else
//                    {
//                        if (optimizeFlags == null) optimizeFlags = new BitArray(values.Length);
//                        optimizeFlags[i] = true;
//                    }
//                }

//                if (notOptimizable == 0)
//                    optimizeFlags = FullyOptimizableTypedArray;
//                else if (notOptimizable >= notWorthOptimizingLimit)
//                {
//                    optimizeFlags = null;
//                }

//                writeArray(values, optimizeFlags);
//            }
//        }

//        /// <summary>
//        /// Writes a UInt32[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The UInt32[] to store.</param>
//        //[CLSCompliant(false)]
//        public void WriteOptimized(uint[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                BitArray optimizeFlags = null;
//                int notOptimizable = 0;
//                int notWorthOptimizingLimit = 1 + (int)(values.Length * (optimizeForSize ? 0.8f : 0.6f));
//                for (int i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
//                {
//                    if (values[i] > HighestOptimizable32BitValue)
//                        notOptimizable++;
//                    else
//                    {
//                        if (optimizeFlags == null) optimizeFlags = new BitArray(values.Length);
//                        optimizeFlags[i] = true;
//                    }
//                }

//                if (notOptimizable == 0)
//                    optimizeFlags = FullyOptimizableTypedArray;
//                else if (notOptimizable >= notWorthOptimizingLimit)
//                {
//                    optimizeFlags = null;
//                }

//                writeArray(values, optimizeFlags);
//            }
//        }

//        /// <summary>
//        /// Writes a UInt64[] into the stream using the fewest possible bytes.
//        /// Notes:
//        /// A null or empty array will take 1 byte.
//        /// </summary>
//        /// <param name="values">The UInt64[] to store.</param>
//        //[CLSCompliant(false)]
//        public void WriteOptimized(ulong[] values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else if (values.Length == 0)
//                writeTypeCode(SerializedType.EmptyTypedArrayType);
//            else
//            {
//                BitArray optimizeFlags = null;
//                int notOptimizable = 0;
//                int notWorthOptimizingLimit = 1 + (int)(values.Length * (optimizeForSize ? 0.8f : 0.6f));
//                for (int i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
//                {
//                    if (values[i] > HighestOptimizable64BitValue)
//                        notOptimizable++;
//                    else
//                    {
//                        if (optimizeFlags == null) optimizeFlags = new BitArray(values.Length);
//                        optimizeFlags[i] = true;
//                    }
//                }

//                if (notOptimizable == 0)
//                    optimizeFlags = FullyOptimizableTypedArray;
//                else if (notOptimizable >= notWorthOptimizingLimit)
//                {
//                    optimizeFlags = null;
//                }

//                writeArray(values, optimizeFlags);
//            }
//        }

//        /// <summary>
//        /// Writes a Nullable type into the stream.
//        /// Synonym for WriteObject().
//        /// </summary>
//        /// <param name="value">The Nullable value to store.</param>
//        public void WriteNullable(ValueType value)
//        {
//            WriteObject(value);
//        }

//        /// <summary>
//        /// Writes a non-null generic Dictionary into the stream.
//        /// </summary>
//        /// <remarks>
//        /// The key and value types themselves are not stored - they must be 
//        /// supplied at deserialization time.
//        /// <para/>
//        /// An array of keys is stored followed by an array of values.
//        /// </remarks>
//        /// <typeparam name="K">The key myObjectStream.</typeparam>
//        /// <typeparam name="V">The value myObjectStream.</typeparam>
//        /// <param name="value">The generic dictionary.</param>
//        public void Write<K, V>(Dictionary<K, V> value)
//        {
//            K[] keys = new K[value.Count];
//            value.Keys.CopyTo(keys, 0);

//            V[] values = new V[value.Count];
//            value.Values.CopyTo(values, 0);

//            writeTypedArray(keys, false);
//            writeTypedArray(values, false);
//        }

//        /// <summary>
//        /// Writes a non-null generic List into the stream.
//        /// </summary>
//        /// <remarks>
//        /// The list type itself is not stored - it must be supplied
//        /// at deserialization time.
//        /// <para/>
//        /// The list contents are stored as an array.
//        /// </remarks>
//        /// <typeparam name="T">The list myObjectStream.</typeparam>
//        /// <param name="value">The generic List.</param>
//        public void Write<T>(List<T> value)
//        {
//            writeTypedArray(value.ToArray(), false);
//        }

//        /// <summary>
//        /// Writes a null or a typed array into the stream.
//        /// </summary>
//        /// <param name="values">The array to store.</param>
//        public void WriteTypedArray(Array values)
//        {
//            if (values == null)
//                writeTypeCode(SerializedType.NullType);
//            else
//            {
//                writeTypedArray(values, true);
//            }
//        }

//        /// <summary>
//        /// Writes the contents of the string and object token tables into the stream.
//        /// Also SerializedSuperblock the starting offset into the first 4 bytes of the stream.
//        /// Notes:
//        /// Called automatically by ToArray().
//        /// Can be used to ensure that the complete graph is written before using an
//        /// alternate technique of extracting a Byte[] such as using compression on
//        /// the underlying stream.
//        /// </summary>
//        /// <returns>The length of the string and object tables.</returns>
//        public int AppendTokenTables()
//        {
//            long currentPosition = BaseStream.Position;
//            BaseStream.Position = 0;
//            Write((int)currentPosition);
//            BaseStream.Position = currentPosition;

//            int stringTokensCount = stringLookup.Count;
//            write7bitEncodedSigned32BitValue(stringLookup.Count);
//            for (int i = 0; i < stringTokensCount; i++)
//            {
//                base.Write(stringLookup[i]);
//            }

//            write7bitEncodedSigned32BitValue(objectTokens.Count);
//            for (int i = 0; i < objectTokens.Count; i++)
//            {
//                WriteObject(objectTokens[i]);
//            }

//            return (int)(BaseStream.Position - currentPosition);
//        }

//        /// <summary>
//        /// Returns a byte[] containing all of the serialized data.
//        /// 
//        /// The current implementation has the data in 3 sections:
//        /// 1) A 4 byte Int32 giving the offset to the 3rd section.
//        /// 2) The main serialized data.
//        /// 3) The serialized string tokenization lists and object
//        ///    tokenization lists.
//        /// 
//        /// Only call this method once all of the data has been serialized.
//        /// 
//        /// This method appends all of the tokenized data (string and object)
//        /// to the end of the stream and ensures that the first four bytes
//        /// reflect the offset of the tokenized data so that it can be
//        /// deserialized first.
//        /// This is the reason for requiring a rewindable stream.
//        /// 
//        /// Future implementations may also allow the serialized data to be
//        /// accessed via 2 byte[] arrays. This would remove the requirement
//        /// for a rewindable stream opening the possibility of streaming the
//        /// serialized data directly over the network allowing simultaneous
//        /// of partially simultaneous deserialization.
//        /// </summary>
//        /// <returns>A byte[] containing all serialized data.</returns>
//        public byte[] ToArray()
//        {
//            AppendTokenTables();
//            return (BaseStream as MemoryStream).ToArray();
//        }

//        /// <summary>
//        /// Writes a byte[] directly into the stream.
//        /// The size of the array is not stored so only use this method when
//        /// the number of bytes will be known at deserialization time.
//        /// 
//        /// A null value will throw an exception
//        /// </summary>
//        /// <param name="value">The byte[] to store. Must not be null.</param>
//        public void WriteBytesDirect(byte[] value)
//        {
//            base.Write(value);
//        }

//        /// <summary>
//        /// Writes a non-null string directly to the stream without tokenization.
//        /// </summary>
//        /// <param name="value">The string to store. Must not be null.</param>
//        public void WriteStringDirect(string value)
//        {
//            checkOptimizable(value != null, "Cannot directly write a null string");

//            base.Write(value);
//        }

//        /// <summary>
//        /// Writes a token (an Int32 taking 1 to 4 bytes) into the stream that represents the object instance.
//        /// The same token will always be used for the same object instance.
//        /// 
//        /// The object will be serialized once and recreated at deserialization time.
//        /// Calls to SerializationReader.ReadTokenizedObject() will retrieve the same object instance.
//        /// 
//        /// </summary>
//        /// <param name="value">The object to tokenize. Must not be null and must not be a string.</param>
//        public void WriteTokenizedObject(object value)
//        {
//            WriteTokenizedObject(value, false);
//        }

//        /// <summary>
//        /// Writes a token (an Int32 taking 1 to 4 bytes) into the stream that represents the object instance.
//        /// The same token will always be used for the same object instance.
//        /// 
//        /// When recreateFromType is set to true, the object's myObjectStream will be stored and the object recreated using 
//        /// Activator.GetInstance with a parameterless contructor. This is useful for stateless, factory-type classes.
//        /// 
//        /// When recreateFromType is set to false, the object will be serialized once and recreated at deserialization time.
//        /// 
//        /// Calls to SerializationReader.ReadTokenizedObject() will retrieve the same object instance.
//        /// </summary>
//        /// <param name="value">The object to tokenize. Must not be null and must not be a string.</param>
//        /// <param name="recreateFromType">true if the object can be recreated using a parameterless constructor; 
//        /// false if the object should be serialized as-is</param>
//        public void WriteTokenizedObject(object value, bool recreateFromType)
//        {
//            checkOptimizable(value != null, "Cannot write a null tokenized object");
//            checkOptimizable(!(value is string), "Use Write(string) instead of WriteTokenizedObject()");

//            if (recreateFromType) value = new SingletonTypeWrapper(value);

//            object token = objectTokenLookup[value];
//            if (token != null)
//                write7bitEncodedSigned32BitValue((int)token);
//            else
//            {
//                int newToken = objectTokens.Count;
//                objectTokens.Add(value);
//                objectTokenLookup[value] = newToken;
//                write7bitEncodedSigned32BitValue(newToken);
//            }
//        }
//        #endregion Methods

//        #region Private Methods
//        internal static IFastSerializationTypeSurrogate findSurrogateForType(Type type)
//        {
//            foreach (IFastSerializationTypeSurrogate surrogate in TypeSurrogates)
//            {
//                if (surrogate.SupportsType(type)) return surrogate;
//            }
//            return null;
//        }

//        private static BinaryFormatter createBinaryFormatter()
//        {
//            BinaryFormatter result = new BinaryFormatter();
//            result.AssemblyFormat = FormatterAssemblyStyle.Full;
//            return result;
//        }

//        /// <summary>
//        /// Encodes a TimeSpan into the fewest number of bytes.
//        /// Has been separated from the WriteOptimized(TimeSpan) method so that WriteOptimized(DateTime)
//        /// can also use this for .NET 2.0 DateTimeKind information.
//        /// By taking advantage of the fact that a DateTime's TimeOfDay portion will never use the IsNegative
//        /// and HasDays flags, we can use these 2 bits to store the DateTimeKind and, since DateTimeKind is
//        /// unlikely to be set without a Time, we need no additional bytes to support a .NET 2.0 DateTime.
//        /// </summary>
//        /// <param name="value">The TimeSpan to store.</param>
//        /// <param name="partOfDateTime">True if the TimeSpan is the TimeOfDay from a DateTime; False if a real TimeSpan.</param>
//        /// <param name="initialData">The intial data for the BitVector32 - contains DateTimeKind or 0</param>
//        private void encodeTimeSpan(TimeSpan value, bool partOfDateTime, int initialData)
//        {
//            BitVector32 packedData = new BitVector32(initialData);
//            int days;
//            int hours = Math.Abs(value.Hours);
//            int minutes = Math.Abs(value.Minutes);
//            int seconds = Math.Abs(value.Seconds);
//            int milliseconds = Math.Abs(value.Milliseconds);
//            bool hasTime = hours != 0 || minutes != 0;
//            int optionalBytes = 0;

//            if (partOfDateTime)
//                days = 0;
//            else
//            {
//                days = Math.Abs(value.Days);
//                packedData[IsNegativeSection] = value.Ticks < 0 ? 1 : 0;
//                packedData[HasDaysSection] = days != 0 ? 1 : 0;
//            }

//            if (hasTime)
//            {
//                packedData[HasTimeSection] = 1;
//                packedData[HoursSection] = hours;
//                packedData[MinutesSection] = minutes;
//            }

//            if (seconds != 0)
//            {
//                packedData[HasSecondsSection] = 1;
//                if (!hasTime && milliseconds == 0) // If only seconds are present then we can use the minutes slot to save a byte
//                    packedData[MinutesSection] = seconds;
//                else
//                {
//                    packedData[SecondsSection] = seconds;
//                    optionalBytes++;
//                }
//            }

//            if (milliseconds != 0)
//            {
//                packedData[HasMillisecondsSection] = 1;
//                packedData[MillisecondsSection] = milliseconds;
//                optionalBytes = 2;
//            }

//            int data = packedData.Data;
//            Write((byte)data);
//            Write((byte)(data >> 8)); // Always SerializedSuperblock minimum of two bytes
//            if (optionalBytes > 0) Write((byte)(data >> 16));
//            if (optionalBytes > 1) Write((byte)(data >> 24));

//            if (days != 0)
//            {
//                write7bitEncodedSigned32BitValue(days);
//            }
//        }

//        /// <summary>
//        /// Checks whether an optimization condition has been met and throw an exception if not.
//        /// 
//        /// This method has been made conditional on THROW_IF_NOT_OPTIMIZABLE being set at compile time.
//        /// By default, this is set if DEBUG is set but could be set explicitly if exceptions are required and
//        /// the evaluation overhead is acceptable. 
//        /// If not set, then this method and all references to it are removed at compile time.
//        /// 
//        /// Leave at the default for optimum usage.
//        /// </summary>
//        /// <param name="condition">An expression evaluating to true if the optimization condition is met, false otherwise.</param>
//        /// <param name="message">The message to include in the exception should the optimization condition not be met.</param>
//        [Conditional("THROW_IF_NOT_OPTIMIZABLE")]
//        private static void checkOptimizable(bool condition, string message)
//        {
//            if (!condition) throw new OptimizationException(message);
//        }

//        /// <summary>
//        /// Stores a 32-bit signed value into the stream using 7-bit encoding.
//        /// 
//        /// The value is written 7 bits at a time (starting with the least-significant bits) until there are no more bits to SerializedSuperblock.
//        /// The eighth bit of each byte stored is used to indicate whether there are more bytes following this one.
//        /// 
//        /// See Write(Int32) for details of the values that are optimizable.
//        /// </summary>
//        /// <param name="value">The Int32 value to encode.</param>
//        private void write7bitEncodedSigned32BitValue(int value)
//        {
//            uint unsignedValue = unchecked((uint)value);
//            while (unsignedValue >= 0x80)
//            {
//                Write((byte)(unsignedValue | 0x80));
//                unsignedValue >>= 7;
//            }
//            Write((byte)unsignedValue);
//        }

//        /// <summary>
//        /// Stores a 64-bit signed value into the stream using 7-bit encoding.
//        /// 
//        /// The value is written 7 bits at a time (starting with the least-significant bits) until there are no more bits to SerializedSuperblock.
//        /// The eighth bit of each byte stored is used to indicate whether there are more bytes following this one.
//        /// 
//        /// See Write(Int64) for details of the values that are optimizable.
//        /// </summary>
//        /// <param name="value">The Int64 value to encode.</param>
//        private void write7bitEncodedSigned64BitValue(long value)
//        {
//            ulong unsignedValue = unchecked((ulong)value);
//            while (unsignedValue >= 0x80)
//            {
//                Write((byte)(unsignedValue | 0x80));
//                unsignedValue >>= 7;
//            }
//            Write((byte)unsignedValue);
//        }

//        /// <summary>
//        /// Stores a 32-bit unsigned value into the stream using 7-bit encoding.
//        /// 
//        /// The value is written 7 bits at a time (starting with the least-significant bits) until there are no more bits to SerializedSuperblock.
//        /// The eighth bit of each byte stored is used to indicate whether there are more bytes following this one.
//        /// 
//        /// See Write(UInt32) for details of the values that are optimizable.
//        /// </summary>
//        /// <param name="value">The UInt32 value to encode.</param>
//        private void write7bitEncodedUnsigned32BitValue(uint value)
//        {
//            while (value >= 0x80)
//            {
//                Write((byte)(value | 0x80));
//                value >>= 7;
//            }
//            Write((byte)value);
//        }

//        /// <summary>
//        /// Stores a 64-bit unsigned value into the stream using 7-bit encoding.
//        /// 
//        /// The value is written 7 bits at a time (starting with the least-significant bits) until there are no more bits to SerializedSuperblock.
//        /// The eighth bit of each byte stored is used to indicate whether there are more bytes following this one.
//        /// 
//        /// See Write(ULong) for details of the values that are optimizable.
//        /// </summary>
//        /// <param name="value">The ULong value to encode.</param>
//        private void write7bitEncodedUnsigned64BitValue(ulong value)
//        {
//            while (value >= 0x80)
//            {
//                Write((byte)(value | 0x80));
//                value >>= 7;
//            }
//            Write((byte)value);
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null Boolean[].
//        /// </summary>
//        /// <remarks>
//        /// Stored as a BitArray for optimization.
//        /// </remarks>
//        /// <param name="values">The Boolean[] to store.</param>
//        private void writeArray(bool[] values)
//        {
//            WriteOptimized(new BitArray(values));
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null Byte[].
//        /// </summary>
//        /// <param name="values">The Byte[] to store.</param>
//        private void writeArray(byte[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            if (values.Length > 0) base.Write(values);
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null Char[].
//        /// </summary>
//        /// <param name="values">The Char[] to store.</param>
//        private void writeArray(char[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            if (values.Length > 0) base.Write(values);
//        }

//        /// <summary>
//        /// Internal implementation to SerializedSuperblock a non, null DateTime[] using a BitArray to 
//        /// determine which elements are optimizable.
//        /// </summary>
//        /// <param name="values">The DateTime[] to store.</param>
//        /// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
//        /// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
//        /// if none of the elements are optimizable.</param>
//        private void writeArray(DateTime[] values, BitArray optimizeFlags)
//        {
//            writeTypedArrayTypeCode(optimizeFlags, values.Length);

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                    Write(values[i]);
//                else
//                {
//                    WriteOptimized(values[i]);
//                }
//            }
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null Decimal[].
//        /// </summary>
//        /// <remarks>
//        /// All elements are stored optimized.
//        /// </remarks>
//        /// <param name="values">The Decimal[] to store.</param>
//        private void writeArray(decimal[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            for (int i = 0; i < values.Length; i++)
//            {
//                WriteOptimized(values[i]);
//            }
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null Double[].
//        /// </summary>
//        /// <param name="values">The Double[] to store.</param>
//        private void writeArray(double[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            foreach (double value in values)
//            {
//                Write(value);
//            }
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null Single[].
//        /// </summary>
//        /// <param name="values">The Single[] to store.</param>
//        private void writeArray(float[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            foreach (float value in values)
//            {
//                Write(value);
//            }
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null Guid[].
//        /// </summary>
//        /// <param name="values">The Guid[] to store.</param>
//        private void writeArray(Guid[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            foreach (Guid value in values)
//            {
//                Write(value);
//            }
//        }

//        /// <summary>
//        /// Internal implementation to SerializedSuperblock a non-null Int16[] using a BitArray to determine which elements are optimizable.
//        /// </summary>
//        /// <param name="values">The Int16[] to store.</param>
//        /// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
//        /// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
//        /// if none of the elements are optimizable.</param>
//        private void writeArray(short[] values, BitArray optimizeFlags)
//        {
//            writeTypedArrayTypeCode(optimizeFlags, values.Length);

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                    Write(values[i]);
//                else
//                {
//                    write7bitEncodedSigned32BitValue(values[i]);
//                }
//            }
//        }

//        /// <summary>
//        /// Internal implementation to SerializedSuperblock a non-null Int32[] using a BitArray to determine which elements are optimizable.
//        /// </summary>
//        /// <param name="values">The Int32[] to store.</param>
//        /// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
//        /// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
//        /// if none of the elements are optimizable.</param>
//        private void writeArray(int[] values, BitArray optimizeFlags)
//        {
//            writeTypedArrayTypeCode(optimizeFlags, values.Length);

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                    Write(values[i]);
//                else
//                {
//                    write7bitEncodedSigned32BitValue(values[i]);
//                }
//            }
//        }

//        /// <summary>
//        /// Internal implementation to writes a non-null Int64[] using a BitArray to determine which elements are optimizable.
//        /// </summary>
//        /// <param name="values">The Int64[] to store.</param>
//        /// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
//        /// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
//        /// if none of the elements are optimizable.</param>
//        private void writeArray(long[] values, BitArray optimizeFlags)
//        {
//            writeTypedArrayTypeCode(optimizeFlags, values.Length);

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                    Write(values[i]);
//                else
//                {
//                    write7bitEncodedSigned64BitValue(values[i]);
//                }
//            }
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null SByte[].
//        /// </summary>
//        /// <param name="values">The SByte[] to store.</param>
//        private void writeArray(sbyte[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            foreach (sbyte value in values)
//            {
//                Write(value);
//            }
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null Int16[].
//        /// </summary>
//        /// <param name="values">The Int16[] to store.</param>
//        private void writeArray(short[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            foreach (short value in values)
//            {
//                Write(value);
//            }
//        }

//        /// <summary>
//        /// Internal implementation to SerializedSuperblock a non-null TimeSpan[] using a BitArray to determine which elements are optimizable.
//        /// </summary>
//        /// <param name="values">The TimeSpan[] to store.</param>
//        /// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
//        /// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
//        /// if none of the elements are optimizable.</param>
//        private void writeArray(TimeSpan[] values, BitArray optimizeFlags)
//        {
//            writeTypedArrayTypeCode(optimizeFlags, values.Length);

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                    Write(values[i]);
//                else
//                {
//                    WriteOptimized(values[i]);
//                }
//            }
//        }

//        /// <summary>
//        /// Internal implementation to SerializedSuperblock a non-null UInt16[] using a BitArray to determine which elements are optimizable.
//        /// </summary>
//        /// <param name="values">The UInt16[] to store.</param>
//        /// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
//        /// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
//        /// if none of the elements are optimizable.</param>
//        private void writeArray(ushort[] values, BitArray optimizeFlags)
//        {
//            writeTypedArrayTypeCode(optimizeFlags, values.Length);

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                    Write(values[i]);
//                else
//                {
//                    write7bitEncodedUnsigned32BitValue(values[i]);
//                }
//            }
//        }

//        /// <summary>
//        /// Internal implementation to SerializedSuperblock a non-null UInt32[] using a BitArray to determine which elements are optimizable.
//        /// </summary>
//        /// <param name="values">The UInt32[] to store.</param>
//        /// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
//        /// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
//        /// if none of the elements are optimizable.</param>
//        private void writeArray(uint[] values, BitArray optimizeFlags)
//        {
//            writeTypedArrayTypeCode(optimizeFlags, values.Length);

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                    Write(values[i]);
//                else
//                {
//                    write7bitEncodedUnsigned32BitValue(values[i]);
//                }
//            }
//        }

//        /// <summary>
//        /// Internal implementation to store a non-null UInt16[].
//        /// </summary>
//        /// <param name="values">The UIn16[] to store.</param>
//        private void writeArray(ushort[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            foreach (ushort value in values)
//            {
//                Write(value);
//            }
//        }

//        /// <summary>
//        /// Internal implementation to SerializedSuperblock a non-null UInt64[] using a BitArray to determine which elements are optimizable.
//        /// </summary>
//        /// <param name="values">The UInt64[] to store.</param>
//        /// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
//        /// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
//        /// if none of the elements are optimizable.</param>
//        private void writeArray(ulong[] values, BitArray optimizeFlags)
//        {
//            writeTypedArrayTypeCode(optimizeFlags, values.Length);

//            for (int i = 0; i < values.Length; i++)
//            {
//                if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                    Write(values[i]);
//                else
//                {
//                    write7bitEncodedUnsigned64BitValue(values[i]);
//                }
//            }
//        }

//        /// <summary>
//        /// Writes the values in the non-null object[] into the stream.
//        /// 
//        /// Sequences of null values and sequences of DBNull.Values are stored with a flag and optimized count.
//        /// Other values are stored using WriteObject().
//        /// 
//        /// This routine is called by the Write(object[]), WriteOptimized(object[]) and Write(object[], object[])) methods.
//        /// </summary>
//        /// <param name="values"></param>
//        private void writeObjectArray(object[] values)
//        {
//            write7bitEncodedSigned32BitValue(values.Length);
//            int lastIndex = values.Length - 1;
//            for (int i = 0; i < values.Length; i++)
//            {
//                object value = values[i];

//                if (i < lastIndex && (value == null ? values[i + 1] == null : value.Equals(values[i + 1])))
//                {
//                    int duplicates = 1;
//                    if (value == null)
//                    {
//                        writeTypeCode(SerializedType.NullSequenceType);
//                        for (i++; i < lastIndex && values[i + 1] == null; i++) duplicates++;
//                    }
//                    else if (value == DBNull.Value)
//                    {
//                        writeTypeCode(SerializedType.DBNullSequenceType);
//                        for (i++; i < lastIndex && values[i + 1] == DBNull.Value; i++) duplicates++;
//                    }
//                    else
//                    {
//                        writeTypeCode(SerializedType.DuplicateValueSequenceType);
//                        for (i++; i < lastIndex && value.Equals(values[i + 1]); i++) duplicates++;
//                        WriteObject(value);
//                    }
//                    write7bitEncodedSigned32BitValue(duplicates);
//                }
//                else
//                {
//                    WriteObject(value);
//                }

//            }
//        }

//        /// <summary>
//        /// Stores the specified SerializedType code into the stream.
//        /// 
//        /// By using a centralized method, it is possible to collect statistics for the
//        /// type of data being stored in DEBUG mode.
//        /// 
//        /// Use the DumpTypeUsage() method to show a list of used SerializedTypes and
//        /// the number of times each has been used. This method and the collection code
//        /// will be optimized out when compiling in Release mode.
//        /// </summary>
//        /// <param name="typeCode">The SerializedType to store.</param>
//        private void writeTypeCode(SerializedType typeCode)
//        {
//            Write((byte)typeCode);
//#if DEBUG
//            typeUsage[(int)typeCode]++;
//#endif
//        }

//        ///// <summary>
//        ///// Examines a typed array to determine the element type, stores the correct SerializedType code
//        ///// and calls the correct method to store the array.
//        ///// 
//        ///// This has been separated into a separate method because .NET 2.0 has a 'feature' where the 'is'
//        ///// operator seems to use covariance on value typed arrays (even though the documentation says this
//        ///// should only occur on reference typed arrays). This did not happen on .NET 1.1.
//        ///// 
//        ///// This means that "is int[]", returns true for a uint[] under .NET 2.0, but false under .NET 1.1
//        ///// By looking for an Array object in the WriteObject loop and then finding the element type, 
//        ///// we can use the same code for both version.
//        ///// </summary>
//        ///// <param name="value">The typed array to store.</param>

//        /// <summary>
//        /// Internal implementation to SerializedSuperblock a non-null typed array into the stream.
//        /// </summary>
//        /// <remarks>
//        /// Checks first to see if the element type is a primitive type and calls the 
//        /// correct routine if so. Otherwise determines the best, optimized method
//        /// to store the array contents.
//        /// <para/>
//        /// An array of object elements never stores its type.
//        /// </remarks>
//        /// <param name="value">The non-null typed array to store.</param>
//        /// <param name="storeType">True if the type should be stored; false otherwise</param>
//        private void writeTypedArray(Array value, bool storeType)
//        {
//            Type elementType = value.GetType().GetElementType();
//            if (elementType == typeof(object)) storeType = false;

//            if (elementType == typeof(string))
//            {
//                writeTypeCode(SerializedType.StringArrayType);
//                WriteOptimized((object[])value);
//            }

//            else if (elementType == typeof(Int32))
//            {
//                writeTypeCode(SerializedType.Int32ArrayType);
//                if (optimizeForSize) WriteOptimized((Int32[])value); else Write((Int32[])value);
//            }

//            else if (elementType == typeof(Int16))
//            {
//                writeTypeCode(SerializedType.Int16ArrayType);
//                if (optimizeForSize) WriteOptimized((Int16[])value); else Write((Int16[])value);
//            }

//            else if (elementType == typeof(Int64))
//            {
//                writeTypeCode(SerializedType.Int64ArrayType);
//                if (optimizeForSize) WriteOptimized((Int64[])value); else Write((Int64[])value);
//            }

//            else if (elementType == typeof(UInt32))
//            {
//                writeTypeCode(SerializedType.UInt32ArrayType);
//                if (optimizeForSize) WriteOptimized((UInt32[])value); else Write((UInt32[])value);
//            }

//            else if (elementType == typeof(UInt16))
//            {
//                writeTypeCode(SerializedType.UInt16ArrayType);
//                if (optimizeForSize) WriteOptimized((UInt16[])value); else Write((UInt16[])value);
//            }

//            else if (elementType == typeof(UInt64))
//            {
//                writeTypeCode(SerializedType.UInt64ArrayType);
//                if (optimizeForSize) WriteOptimized((UInt64[])value); else Write((UInt64[])value);
//            }

//            else if (elementType == typeof(Single))
//            {
//                writeTypeCode(SerializedType.SingleArrayType);
//                writeArray((Single[])value);
//            }

//            else if (elementType == typeof(Double))
//            {
//                writeTypeCode(SerializedType.DoubleArrayType);
//                writeArray((Double[])value);
//            }

//            else if (elementType == typeof(Decimal))
//            {
//                writeTypeCode(SerializedType.DecimalArrayType);
//                writeArray((Decimal[])value);
//            }

//            else if (elementType == typeof(DateTime))
//            {
//                writeTypeCode(SerializedType.DateTimeArrayType);
//                if (optimizeForSize) WriteOptimized((DateTime[])value); else Write((DateTime[])value);
//            }

//            else if (elementType == typeof(TimeSpan))
//            {
//                writeTypeCode(SerializedType.TimeSpanArrayType);
//                if (optimizeForSize) WriteOptimized((TimeSpan[])value); else Write((TimeSpan[])value);
//            }

//            else if (elementType == typeof(Guid))
//            {
//                writeTypeCode(SerializedType.GuidArrayType);
//                writeArray((Guid[])value);
//            }

//            else if (elementType == typeof(SByte))
//            {
//                writeTypeCode(SerializedType.SByteArrayType);
//                writeArray((SByte[])value);
//            }

//            else if (elementType == typeof(Boolean))
//            {
//                writeTypeCode(SerializedType.BooleanArrayType);
//                writeArray((bool[])value);
//            }

//            else if (elementType == typeof(Byte))
//            {
//                writeTypeCode(SerializedType.ByteArrayType);
//                writeArray((Byte[])value);
//            }

//            else if (elementType == typeof(Char))
//            {
//                writeTypeCode(SerializedType.CharArrayType);
//                writeArray((Char[])value);
//            }

//            else if (value.Length == 0)
//            {
//                writeTypeCode(elementType == typeof(object) ? SerializedType.EmptyObjectArrayType : SerializedType.EmptyTypedArrayType);
//                if (storeType) WriteOptimized(elementType);
//            }

//            else if (elementType == typeof(object))
//            {
//                writeTypeCode(SerializedType.ObjectArrayType);
//                writeObjectArray((object[])value);
//            }

//            else
//            {
//                BitArray optimizeFlags = isTypeRecreatable(elementType) ? FullyOptimizableTypedArray : null;

//                if (!elementType.IsValueType)
//                {
//                    if (optimizeFlags == null || !arrayElementsAreSameType((object[])value, elementType))
//                    {
//                        if (!storeType)
//                            writeTypeCode(SerializedType.ObjectArrayType);
//                        else
//                        {
//                            writeTypeCode(SerializedType.OtherTypedArrayType);
//                            WriteOptimized(elementType);
//                        }
//                        writeObjectArray((object[])value);
//                        return;
//                    }
//                    else
//                    {
//                        for (int i = 0; i < value.Length; i++)
//                        {
//                            if (value.GetValue(i) == null)
//                            {
//                                if (optimizeFlags == FullyOptimizableTypedArray) optimizeFlags = new BitArray(value.Length);
//                                optimizeFlags[i] = true;
//                            }
//                        }
//                    }
//                }

//                writeTypedArrayTypeCode(optimizeFlags, value.Length);
//                if (storeType) WriteOptimized(elementType);

//                for (int i = 0; i < value.Length; i++)
//                {
//                    if (optimizeFlags == null)
//                        WriteObject(value.GetValue(i));
//                    else if (optimizeFlags == FullyOptimizableTypedArray || !optimizeFlags[i])
//                    {
//                        Write((IOwnedDataSerializable)value.GetValue(i), null);
//                    }
//                }
//            }

//        }

//        /// <summary>
//        /// Checks whether instances of a myObjectStream can be created.
//        /// </summary>
//        /// <remarks>
//        /// A Value myObjectStream only needs to implement IOwnedDataSerializable. 
//        /// A Reference myObjectStream needs to implement IOwnedDataSerializableAndRecreatable and provide a default constructor.
//        /// </remarks>
//        /// <param name="type">The myObjectStream to check</param>
//        /// <returns>true if the myObjectStream is recreatable; false otherwise.</returns>
//        private static bool isTypeRecreatable(Type type)
//        {
//            if (type.IsValueType)
//                return typeof(IOwnedDataSerializable).IsAssignableFrom(type);
//            else
//            {
//                return typeof(IOwnedDataSerializableAndRecreatable).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
//            }
//        }

//        /// <summary>
//        /// Checks whether each element in an array is of the same type.
//        /// </summary>
//        /// <param name="values">The array to check</param>
//        /// <param name="elementType">The expected element type.</param>
//        /// <returns></returns>
//        private static bool arrayElementsAreSameType(object[] values, Type elementType)
//        {
//            foreach (object value in values)
//            {
//                if (value != null && value.GetType() != elementType) return false;
//            }
//            return true;
//        }

//        /// <summary>
//        /// Writes the TypeCode for the Typed Array followed by the number of elements.
//        /// </summary>
//        /// <param name="optimizeFlags"></param>
//        /// <param name="length"></param>
//        private void writeTypedArrayTypeCode(BitArray optimizeFlags, int length)
//        {
//            if (optimizeFlags == null)
//                writeTypeCode(SerializedType.NonOptimizedTypedArrayType);
//            else if (optimizeFlags == FullyOptimizableTypedArray)
//                writeTypeCode(SerializedType.FullyOptimizedTypedArrayType);
//            else
//            {
//                writeTypeCode(SerializedType.PartiallyOptimizedTypedArrayType);
//                WriteOptimized(optimizeFlags);
//            }

//            write7bitEncodedSigned32BitValue(length);
//        }
//        #endregion Private Methods

//        #region Singleton myObjectStream Wrapper
//        /// <summary>
//        /// Private class used to wrap an object that is to be tokenized, and recreated at deserialization by its type.
//        /// </summary>
//        private class SingletonTypeWrapper
//        {
//            public SingletonTypeWrapper(object value)
//            {
//                wrappedType = value.GetType();
//            }

//            public Type WrappedType
//            {
//                get { return wrappedType; }
//            }

//            Type wrappedType;

//            public override bool Equals(object obj)
//            {
//                return wrappedType.Equals((obj as SingletonTypeWrapper).wrappedType);
//            }

//            public override int GetHashCode()
//            {
//                return wrappedType.GetHashCode();
//            }
//        }
//        #endregion Singleton myObjectStream Wrapper

//        #region myObjectStream Usage (Debug mode only)
//#if DEBUG
//        public int[] typeUsage = new int[256];
//#endif

//        [Conditional("DEBUG")]
//        public void DumpTypeUsage()
//        {
//            StringBuilder sb = new StringBuilder("Type Usage Dump\r\n---------------\r\n");
//            for (int i = 0; i < 256; i++)
//            {
//#if DEBUG
//                if (typeUsage[i] != 0) sb.AppendFormat("{0, 8:n0}: {1}\r\n", typeUsage[i], (SerializedType)i);
//#endif
//            }
//            Console.WriteLine(sb);
//        }
//        #endregion myObjectStream Usage (Debug mode only)

//        #region UniqueStringList Nested Class
//        /// <summary>
//        /// Provides a faster way to store string tokens both maintaining the order that they were added and
//        /// providing a fast lookup.
//        /// 
//        /// Based on code developed by ewbi at http://ewbi.blogs.com/develops/2006/10/uniquestringlis.html
//        /// </summary>
//        private sealed class UniqueStringList
//        {
//            #region Static
//            private const float LoadFactor = .72f;

//            // Based on Golden Primes (as far as possible from nearest two powers of two)
//            // at http://planetmath.org/encyclopedia/GoodHashTablePrimes.html
//            private static readonly int[] primeNumberList = new int[]
//                {
//                    // 193, 769, 3079, 12289, 49157 removed to allow quadrupling of bucket table size
//                    // for smaller size then reverting to doubling
//                    389, 1543, 6151, 24593, 98317, 196613, 393241, 786433, 1572869, 3145739, 6291469,
//                    12582917, 25165843, 50331653, 100663319, 201326611, 402653189, 805306457, 1610612741
//                };
//            #endregion Static

//            #region Fields
//            private string[] stringList;
//            private int[] buckets;
//            private int bucketListCapacity;
//            private int stringListIndex;
//            private int loadLimit;
//            private int primeNumberListIndex;
//            #endregion Fields

//            #region Constructors
//            public UniqueStringList()
//            {
//                bucketListCapacity = primeNumberList[primeNumberListIndex++];
//                stringList = new string[bucketListCapacity];
//                buckets = new int[bucketListCapacity];
//                loadLimit = (int)(bucketListCapacity * LoadFactor);
//            }
//            #endregion Constructors

//            #region Properties
//            public string this[int index]
//            {
//                get { return stringList[index]; }
//            }

//            public int Count
//            {
//                get { return stringListIndex; }
//            }
//            #endregion Properties

//            #region Methods
//            public int Add(string value)
//            {
//                int bucketIndex = getBucketIndex(value);
//                int index = buckets[bucketIndex];
//                if (index == 0)
//                {
//                    stringList[stringListIndex++] = value;
//                    buckets[bucketIndex] = stringListIndex;
//                    if (stringListIndex > loadLimit) expand();
//                    return stringListIndex - 1;
//                }
//                return index - 1;
//            }
//            #endregion Methods

//            #region Private Methods
//            private void expand()
//            {
//                bucketListCapacity = primeNumberList[primeNumberListIndex++];
//                buckets = new int[bucketListCapacity];
//                string[] newStringlist = new string[bucketListCapacity];
//                stringList.CopyTo(newStringlist, 0);
//                stringList = newStringlist;
//                reindex();
//            }

//            private void reindex()
//            {
//                loadLimit = (int)(bucketListCapacity * LoadFactor);
//                for (int stringIndex = 0; stringIndex < stringListIndex; stringIndex++)
//                {
//                    int index = getBucketIndex(stringList[stringIndex]);
//                    buckets[index] = stringIndex + 1;
//                }
//            }

//            private int getBucketIndex(string value)
//            {
//                int hashCode = value.GetHashCode() & 0x7fffffff;
//                int bucketIndex = hashCode % bucketListCapacity;
//                int increment = (bucketIndex > 1) ? bucketIndex : 1;
//                int i = bucketListCapacity;
//                while (0 < i--)
//                {
//                    int stringIndex = buckets[bucketIndex];
//                    if (stringIndex == 0) return bucketIndex;
//                    if (string.CompareOrdinal(value, stringList[stringIndex - 1]) == 0) return bucketIndex;
//                    bucketIndex = (bucketIndex + increment) % bucketListCapacity; // Probe.
//                }
//                throw new InvalidOperationException("Failed to locate a bucket.");
//            }
//            #endregion Private Methods
//        }
//        #endregion UniqueStringList Nested Class
//    }

//    /// <summary>
//    /// A SerializationReader instance is used to read stored values and objects from a byte array.
//    ///
//    /// Once an instance is created, use the various methods to read the required data.
//    /// The data read MUST be exactly the same type and in the same order as it was written.
//    /// </summary>
//    public sealed class SerializationReader : BinaryReader
//    {
//        #region Static
//        // Marker to denote that all elements in a value array are optimizable
//        private static readonly BitArray FullyOptimizableTypedArray = new BitArray(0);
//        #endregion Static

//        #region Constructor
//        /// <summary>
//        /// Creates a SerializationReader using a byte[] previous created by SerializationWriter
//        /// 
//        /// A MemoryStream is used to access the data without making a copy of it.
//        /// </summary>
//        /// <param name="data">The byte[] containining serialized data.</param>
//        public SerializationReader(byte[] data) : this(new MemoryStream(data)) { }

//        /// <summary>
//        /// Creates a SerializationReader based on the passed Stream.
//        /// </summary>
//        /// <param name="stream">The stream containing the serialized data</param>
//        private SerializationReader(Stream stream)
//            : base(stream)
//        {
//            endPosition = ReadInt32();
//            stream.Position = endPosition;

//            stringTokenList = new string[ReadOptimizedInt32()];
//            for (int i = 0; i < stringTokenList.Length; i++)
//            {
//                stringTokenList[i] = base.ReadString();
//            }

//            objectTokens = new object[ReadOptimizedInt32()];
//            for (int i = 0; i < objectTokens.Length; i++)
//            {
//                objectTokens[i] = ReadObject();
//            }
//            stream.Position = 4;
//        }
//        #endregion Constructor

//        #region Fields
//        private string[] stringTokenList;
//        private object[] objectTokens;
//        int endPosition;
//        #endregion Fields

//        #region Properties
//        /// <summary>
//        /// Returns the number of bytes or serialized remaining to be processed.
//        /// Useful for checking that deserialization is complete.
//        /// 
//        /// Warning: Retrieving the ObjectStreams in certain stream types can be expensive,
//        /// e.g. a _FileStream, so use sparingly unless known to be a MemoryStream.
//        /// </summary>
//        public int BytesRemaining
//        {
//            get { return endPosition - (int)BaseStream.Position; }
//        }
//        #endregion Properties

//        #region Methods
//        /// <summary>
//        /// Returns an ArrayList or null from the stream.
//        /// </summary>
//        /// <returns>An ArrayList instance.</returns>
//        public ArrayList ReadArrayList()
//        {
//            if (readTypeCode() == SerializedType.NullType) return null;

//            return new ArrayList(ReadOptimizedObjectArray());
//        }

//        /// <summary>
//        /// Returns a BitArray or null from the stream.
//        /// </summary>
//        /// <returns>A BitArray instance.</returns>
//        public BitArray ReadBitArray()
//        {
//            if (readTypeCode() == SerializedType.NullType) return null;
//            return ReadOptimizedBitArray();
//        }

//        /// <summary>
//        /// Returns a BitVector32 value from the stream.
//        /// </summary>
//        /// <returns>A BitVector32 value.</returns>
//        public BitVector32 ReadBitVector32()
//        {
//            return new BitVector32(ReadInt32());
//        }

//        /// <summary>
//        /// Reads the specified number of bytes directly from the stream.
//        /// </summary>
//        /// <param name="count">The number of bytes to read</param>
//        /// <returns>A byte[] containing the read bytes</returns>
//        public byte[] ReadBytesDirect(int count)
//        {
//            return base.ReadBytes(count);
//        }

//        /// <summary>
//        /// Returns a DateTime value from the stream.
//        /// </summary>
//        /// <returns>A DateTime value.</returns>
//        public DateTime ReadDateTime()
//        {
//            return DateTime.FromBinary(ReadInt64());
//        }

//        /// <summary>
//        /// Returns a Guid value from the stream.
//        /// </summary>
//        /// <returns>A DateTime value.</returns>
//        public Guid ReadGuid()
//        {
//            return new Guid(ReadBytes(16));
//        }

//        /// <summary>
//        /// Returns an object based on the SerializedType read next from the stream.
//        /// </summary>
//        /// <returns>An object instance.</returns>
//        public object ReadObject()
//        {
//            return processObject((SerializedType)ReadByte());
//        }

//        /// <summary>
//        /// Called ReadOptimizedString().
//        /// This override to hide base BinaryReader.ReadString().
//        /// </summary>
//        /// <returns>A string value.</returns>
//        public override string ReadString()
//        {
//            return ReadOptimizedString();
//        }

//        /// <summary>
//        /// Returns a string value from the stream.
//        /// </summary>
//        /// <returns>A string value.</returns>
//        public string ReadStringDirect()
//        {
//            return base.ReadString();
//        }

//        /// <summary>
//        /// Returns a TimeSpan value from the stream.
//        /// </summary>
//        /// <returns>A TimeSpan value.</returns>
//        public TimeSpan ReadTimeSpan()
//        {
//            return new TimeSpan(ReadInt64());
//        }

//        /// <summary>
//        /// Returns a myObjectStream or null from the stream.
//        /// 
//        /// Throws an exception if the myObjectStream cannot be found.
//        /// </summary>
//        /// <returns>A myObjectStream instance.</returns>
//        public Type ReadType()
//        {
//            return ReadType(true);
//        }

//        /// <summary>
//        /// Returns a myObjectStream or null from the stream.
//        /// 
//        /// Throws an exception if the myObjectStream cannot be found and throwOnError is true.
//        /// </summary>
//        /// <returns>A myObjectStream instance.</returns>
//        public Type ReadType(bool throwOnError)
//        {
//            if (readTypeCode() == SerializedType.NullType) return null;
//            return Type.GetType(ReadOptimizedString(), throwOnError);
//        }

//        /// <summary>
//        /// Returns an ArrayList from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>An ArrayList instance.</returns>
//        public ArrayList ReadOptimizedArrayList()
//        {
//            return new ArrayList(ReadOptimizedObjectArray());
//        }

//        /// <summary>
//        /// Returns a BitArray from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A BitArray instance.</returns>
//        public BitArray ReadOptimizedBitArray()
//        {
//            int length = ReadOptimizedInt32();
//            if (length == 0)
//                return FullyOptimizableTypedArray;
//            else
//            {
//                BitArray result = new BitArray(base.ReadBytes((length + 7) / 8));
//                result.Length = length;
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns a BitVector32 value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A BitVector32 value.</returns>
//        public BitVector32 ReadOptimizedBitVector32()
//        {
//            return new BitVector32(Read7BitEncodedInt());
//        }

//        /// <summary>
//        /// Returns a DateTime value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A DateTime value.</returns>
//        public DateTime ReadOptimizedDateTime()
//        {
//            // Read date information from first three bytes
//            BitVector32 dateMask = new BitVector32(ReadByte() | (ReadByte() << 8) | (ReadByte() << 16));
//            DateTime result = new DateTime(
//                    dateMask[SerializationWriter.DateYearMask],
//                    dateMask[SerializationWriter.DateMonthMask],
//                    dateMask[SerializationWriter.DateDayMask]
//            );

//            if (dateMask[SerializationWriter.DateHasTimeOrKindMask] == 1)
//            {
//                byte initialByte = ReadByte();
//                DateTimeKind dateTimeKind = (DateTimeKind)(initialByte & 0x03);
//                initialByte &= 0xfc; // Remove the IsNegative and HasDays flags which are never true for a DateTime
//                if (dateTimeKind != DateTimeKind.Unspecified) result = DateTime.SpecifyKind(result, dateTimeKind);
//                if (initialByte == 0)
//                    ReadByte(); // No need to call decodeTimeSpan if there is no time information
//                else
//                {
//                    result = result.Add(decodeTimeSpan(initialByte));
//                }
//            }
//            return result;
//        }

//        /// <summary>
//        /// Returns a Decimal value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A Decimal value.</returns>
//        public Decimal ReadOptimizedDecimal()
//        {
//            byte flags = ReadByte();
//            int lo = 0;
//            int mid = 0;
//            int hi = 0;
//            byte scale = 0;

//            if ((flags & 0x02) != 0) scale = ReadByte();

//            if ((flags & 4) == 0) if ((flags & 32) != 0) lo = ReadOptimizedInt32(); else lo = ReadInt32();
//            if ((flags & 8) == 0) if ((flags & 64) != 0) mid = ReadOptimizedInt32(); else mid = ReadInt32();
//            if ((flags & 16) == 0) if ((flags & 128) != 0) hi = ReadOptimizedInt32(); else hi = ReadInt32();

//            return new decimal(lo, mid, hi, (flags & 0x01) != 0, scale);
//        }

//        /// <summary>
//        /// Returns an Int32 value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>An Int32 value.</returns>
//        public int ReadOptimizedInt32()
//        {
//            int result = 0;
//            int bitShift = 0;
//            while (true)
//            {
//                byte nextByte = ReadByte();
//                result |= ((int)nextByte & 0x7f) << bitShift;
//                bitShift += 7;
//                if ((nextByte & 0x80) == 0) return result;
//            }
//        }

//        /// <summary>
//        /// Returns an Int16 value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>An Int16 value.</returns>
//        public short ReadOptimizedInt16()
//        {
//            return (short)ReadOptimizedInt32();
//        }

//        /// <summary>
//        /// Returns an Int64 value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>An Int64 value.</returns>
//        public long ReadOptimizedInt64()
//        {
//            long result = 0;
//            int bitShift = 0;
//            while (true)
//            {
//                byte nextByte = ReadByte();
//                result |= ((long)nextByte & 0x7f) << bitShift;
//                bitShift += 7;
//                if ((nextByte & 0x80) == 0) return result;
//            }
//        }

//        /// <summary>
//        /// Returns an object[] from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>An object[] instance.</returns>
//        public object[] ReadOptimizedObjectArray()
//        {
//            return ReadOptimizedObjectArray(null);
//        }

//        /// <summary>
//        /// Returns an object[] from the stream that was stored optimized.
//        /// The returned array will be typed according to the specified element type
//        /// and the resulting array can be cast to the expected type.
//        /// e.g.
//        /// string[] myStrings = (string[]) reader.ReadOptimizedObjectArray(typeof(string));
//        /// 
//        /// An exception will be thrown if any of the deserialized values cannot be
//        /// cast to the specified elementType.
//        /// 
//        /// </summary>
//        /// <param name="elementType">The myObjectStream of the expected array elements. null will return a plain object[].</param>
//        /// <returns>An object[] instance.</returns>
//        public object[] ReadOptimizedObjectArray(Type elementType)
//        {
//            int length = ReadOptimizedInt32();
//            object[] result = (object[])(elementType == null ? new object[length] : Array.CreateInstance(elementType, length));
//            for (int i = 0; i < result.Length; i++)
//            {
//                SerializedType t = (SerializedType)ReadByte();

//                if (t == SerializedType.NullSequenceType)
//                    i += ReadOptimizedInt32();
//                else if (t == SerializedType.DuplicateValueSequenceType)
//                {
//                    object target = result[i] = ReadObject();
//                    int duplicates = ReadOptimizedInt32();
//                    while (duplicates-- > 0) result[++i] = target;
//                }
//                else if (t == SerializedType.DBNullSequenceType)
//                {
//                    int duplicates = ReadOptimizedInt32();
//                    result[i] = DBNull.Value;
//                    while (duplicates-- > 0) result[++i] = DBNull.Value;
//                }
//                else if (t != SerializedType.NullType)
//                {
//                    result[i] = processObject(t);
//                }
//            }
//            return result;
//        }

//        /// <summary>
//        /// Returns a pair of object[] arrays from the stream that were stored optimized.
//        /// </summary>
//        /// <returns>A pair of object[] arrays.</returns>
//        public void ReadOptimizedObjectArrayPair(out object[] values1, out object[] values2)
//        {
//            values1 = ReadOptimizedObjectArray(null);
//            values2 = new object[values1.Length];

//            for (int i = 0; i < values2.Length; i++)
//            {
//                SerializedType t = (SerializedType)ReadByte();

//                if (t == SerializedType.DuplicateValueSequenceType)
//                {
//                    values2[i] = values1[i];
//                    int duplicates = ReadOptimizedInt32();
//                    while (duplicates-- > 0) values2[++i] = values1[i];
//                }
//                else if (t == SerializedType.DuplicateValueType)
//                {
//                    values2[i] = values1[i];
//                }
//                else if (t == SerializedType.NullSequenceType)
//                {
//                    i += ReadOptimizedInt32();
//                }
//                else if (t == SerializedType.DBNullSequenceType)
//                {
//                    int duplicates = ReadOptimizedInt32();
//                    values2[i] = DBNull.Value;
//                    while (duplicates-- > 0) values2[++i] = DBNull.Value;
//                }
//                else if (t != SerializedType.NullType)
//                {
//                    values2[i] = processObject(t);
//                }
//            }
//        }

//        /// <summary>
//        /// Returns a string value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A string value.</returns>
//        public string ReadOptimizedString()
//        {
//            SerializedType typeCode = readTypeCode();

//            if (typeCode < SerializedType.NullType)
//                return readTokenizedString((int)typeCode);

//            else if (typeCode == SerializedType.NullType)
//                return null;

//            else if (typeCode == SerializedType.YStringType)
//                return "Y";

//            else if (typeCode == SerializedType.NStringType)
//                return "N";

//            else if (typeCode == SerializedType.SingleCharStringType)
//                return Char.ToString(ReadChar());

//            else if (typeCode == SerializedType.SingleSpaceType)
//                return " ";

//            else if (typeCode == SerializedType.EmptyStringType)
//                return string.Empty;

//            else
//            {
//                throw new InvalidOperationException("Unrecognized TypeCode");
//            }
//        }

//        /// <summary>
//        /// Returns a TimeSpan value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A TimeSpan value.</returns>
//        public TimeSpan ReadOptimizedTimeSpan()
//        {
//            return decodeTimeSpan(ReadByte());
//        }

//        /// <summary>
//        /// Returns a myObjectStream from the stream.
//        /// 
//        /// Throws an exception if the myObjectStream cannot be found.
//        /// </summary>
//        /// <returns>A myObjectStream instance.</returns>
//        public Type ReadOptimizedType()
//        {
//            return ReadOptimizedType(true);
//        }

//        /// <summary>
//        /// Returns a myObjectStream from the stream.
//        /// 
//        /// Throws an exception if the myObjectStream cannot be found and throwOnError is true.
//        /// </summary>
//        /// <returns>A myObjectStream instance.</returns>
//        public Type ReadOptimizedType(bool throwOnError)
//        {
//            return Type.GetType(ReadOptimizedString(), throwOnError);
//        }

//        /// <summary>
//        /// Returns a UInt16 value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A UInt16 value.</returns>
//        //[CLSCompliant(false)]
//        public ushort ReadOptimizedUInt16()
//        {
//            return (ushort)ReadOptimizedUInt32();
//        }

//        /// <summary>
//        /// Returns a UInt32 value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A UInt32 value.</returns>
//        //[CLSCompliant(false)]
//        public uint ReadOptimizedUInt32()
//        {
//            uint result = 0;
//            int bitShift = 0;
//            while (true)
//            {
//                byte nextByte = ReadByte();
//                result |= ((uint)nextByte & 0x7f) << bitShift;
//                bitShift += 7;
//                if ((nextByte & 0x80) == 0) return result;
//            }
//        }

//        /// <summary>
//        /// Returns a UInt64 value from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>A UInt64 value.</returns>
//        //[CLSCompliant(false)]
//        public ulong ReadOptimizedUInt64()
//        {
//            ulong result = 0;
//            int bitShift = 0;
//            while (true)
//            {
//                byte nextByte = ReadByte();
//                result |= ((ulong)nextByte & 0x7f) << bitShift;
//                bitShift += 7;
//                if ((nextByte & 0x80) == 0) return result;
//            }
//        }

//        /// <summary>
//        /// Returns a typed array from the stream.
//        /// </summary>
//        /// <returns>A typed array.</returns>
//        public Array ReadTypedArray()
//        {
//            return (Array)processArrayTypes(readTypeCode(), null);
//        }

//        /// <summary>
//        /// Returns a new, simple generic dictionary populated with keys and values from the stream.
//        /// </summary>
//        /// <typeparam name="K">The key myObjectStream.</typeparam>
//        /// <typeparam name="V">The value myObjectStream.</typeparam>
//        /// <returns>A new, simple, populated generic Dictionary.</returns>
//        public Dictionary<K, V> ReadDictionary<K, V>()
//        {
//            Dictionary<K, V> result = new Dictionary<K, V>();
//            ReadDictionary(result);
//            return result;
//        }

//        /// <summary>
//        /// Populates a pre-existing generic dictionary with keys and values from the stream.
//        /// This allows a generic dictionary to be created without using the default constructor.
//        /// </summary>
//        /// <typeparam name="K">The key myObjectStream.</typeparam>
//        /// <typeparam name="V">The value myObjectStream.</typeparam>
//        public void ReadDictionary<K, V>(Dictionary<K, V> dictionary)
//        {

//            K[] keys = (K[])processArrayTypes(readTypeCode(), typeof(K));
//            V[] values = (V[])processArrayTypes(readTypeCode(), typeof(V));

//            if (dictionary == null) dictionary = new Dictionary<K, V>(keys.Length);
//            for (int i = 0; i < keys.Length; i++)
//            {
//                dictionary.Add(keys[i], values[i]);
//            }
//        }

//        /// <summary>
//        /// Returns a generic List populated with values from the stream.
//        /// </summary>
//        /// <typeparam name="T">The list myObjectStream.</typeparam>
//        /// <returns>A new generic List.</returns>
//        public List<T> ReadList<T>()
//        {
//            return new List<T>((T[])processArrayTypes(readTypeCode(), typeof(T)));
//        }

//        /// <summary>
//        /// Returns a Nullable struct from the stream.
//        /// The value returned must be cast to the correct Nullable type.
//        /// Synonym for ReadObject();
//        /// </summary>
//        /// <returns>A struct value or null</returns>
//        public ValueType ReadNullable()
//        {
//            return (ValueType)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Boolean from the stream.
//        /// </summary>
//        /// <returns>A Nullable Boolean.</returns>
//        public Boolean? ReadNullableBoolean()
//        {
//            return (bool?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Byte from the stream.
//        /// </summary>
//        /// <returns>A Nullable Byte.</returns>
//        public Byte? ReadNullableByte()
//        {
//            return (byte?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Char from the stream.
//        /// </summary>
//        /// <returns>A Nullable Char.</returns>
//        public Char? ReadNullableChar()
//        {
//            return (char?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable DateTime from the stream.
//        /// </summary>
//        /// <returns>A Nullable DateTime.</returns>
//        public DateTime? ReadNullableDateTime()
//        {
//            return (DateTime?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Decimal from the stream.
//        /// </summary>
//        /// <returns>A Nullable Decimal.</returns>
//        public Decimal? ReadNullableDecimal()
//        {
//            return (decimal?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Double from the stream.
//        /// </summary>
//        /// <returns>A Nullable Double.</returns>
//        public Double? ReadNullableDouble()
//        {
//            return (double?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Guid from the stream.
//        /// </summary>
//        /// <returns>A Nullable Guid.</returns>
//        public Guid? ReadNullableGuid()
//        {
//            return (Guid?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Int16 from the stream.
//        /// </summary>
//        /// <returns>A Nullable Int16.</returns>
//        public Int16? ReadNullableInt16()
//        {
//            return (short?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Int32 from the stream.
//        /// </summary>
//        /// <returns>A Nullable Int32.</returns>
//        public Int32? ReadNullableInt32()
//        {
//            return (int?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Int64 from the stream.
//        /// </summary>
//        /// <returns>A Nullable Int64.</returns>
//        public Int64? ReadNullableInt64()
//        {
//            return (long?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable SByte from the stream.
//        /// </summary>
//        /// <returns>A Nullable SByte.</returns>
//        //[CLSCompliant(false)]
//        public SByte? ReadNullableSByte()
//        {
//            return (sbyte?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable Single from the stream.
//        /// </summary>
//        /// <returns>A Nullable Single.</returns>
//        public Single? ReadNullableSingle()
//        {
//            return (float?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable TimeSpan from the stream.
//        /// </summary>
//        /// <returns>A Nullable TimeSpan.</returns>
//        public TimeSpan? ReadNullableTimeSpan()
//        {
//            return (TimeSpan?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable UInt16 from the stream.
//        /// </summary>
//        /// <returns>A Nullable UInt16.</returns>
//        //[CLSCompliant(false)]
//        public UInt16? ReadNullableUInt16()
//        {
//            return (ushort?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable UInt32 from the stream.
//        /// </summary>
//        /// <returns>A Nullable UInt32.</returns>
//        //[CLSCompliant(false)]
//        public UInt32? ReadNullableUInt32()
//        {
//            return (uint?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Nullable UInt64 from the stream.
//        /// </summary>
//        /// <returns>A Nullable UInt64.</returns>
//        //[CLSCompliant(false)]
//        public UInt64? ReadNullableUInt64()
//        {
//            return (ulong?)ReadObject();
//        }

//        /// <summary>
//        /// Returns a Byte[] from the stream.
//        /// </summary>
//        /// <returns>A Byte instance; or null.</returns>
//        public byte[] ReadByteArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new byte[0];
//            else
//            {
//                return readByteArray();
//            }
//        }

//        /// <summary>
//        /// Returns a Char[] from the stream.
//        /// </summary>
//        /// <returns>A Char[] value; or null.</returns>
//        public char[] ReadCharArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new char[0];
//            else
//            {
//                return readCharArray();
//            }
//        }

//        /// <summary>
//        /// Returns a Double[] from the stream.
//        /// </summary>
//        /// <returns>A Double[] instance; or null.</returns>
//        public double[] ReadDoubleArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new double[0];
//            else
//            {
//                return readDoubleArray();
//            }
//        }

//        /// <summary>
//        /// Returns a Guid[] from the stream.
//        /// </summary>
//        /// <returns>A Guid[] instance; or null.</returns>
//        public Guid[] ReadGuidArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new Guid[0];
//            else
//            {
//                return readGuidArray();
//            }
//        }

//        /// <summary>
//        /// Returns an Int16[] from the stream.
//        /// </summary>
//        /// <returns>An Int16[] instance; or null.</returns>
//        public short[] ReadInt16Array()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new short[0];
//            else
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
//                short[] result = new short[ReadOptimizedInt32()];
//                for (int i = 0; i < result.Length; i++)
//                {
//                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                        result[i] = ReadInt16();
//                    else
//                    {
//                        result[i] = ReadOptimizedInt16();
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns an object[] or null from the stream.
//        /// </summary>
//        /// <returns>A DateTime value.</returns>
//        public object[] ReadObjectArray()
//        {
//            return ReadObjectArray(null);
//        }

//        /// <summary>
//        /// Returns an object[] or null from the stream.
//        /// The returned array will be typed according to the specified element type
//        /// and the resulting array can be cast to the expected type.
//        /// e.g.
//        /// string[] myStrings = (string[]) reader.ReadObjectArray(typeof(string));
//        /// 
//        /// An exception will be thrown if any of the deserialized values cannot be
//        /// cast to the specified elementType.
//        /// 
//        /// </summary>
//        /// <param name="elementType">The myObjectStream of the expected array elements. null will return a plain object[].</param>
//        /// <returns>An object[] instance.</returns>
//        public object[] ReadObjectArray(Type elementType)
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyObjectArrayType)
//                return elementType == null ? new object[0] : (object[])Array.CreateInstance(elementType, 0);
//            else if (t == SerializedType.EmptyTypedArrayType)
//                throw new Exception();
//            else
//            {
//                return ReadOptimizedObjectArray(elementType);
//            }
//        }

//        /// <summary>
//        /// Returns a Single[] from the stream.
//        /// </summary>
//        /// <returns>A Single[] instance; or null.</returns>
//        public float[] ReadSingleArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new float[0];
//            else
//            {
//                return readSingleArray();
//            }
//        }

//        /// <summary>
//        /// Returns an SByte[] from the stream.
//        /// </summary>
//        /// <returns>An SByte[] instance; or null.</returns>
//        //[CLSCompliant(false)]
//        public sbyte[] ReadSByteArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new sbyte[0];
//            else
//            {
//                return readSByteArray();
//            }
//        }

//        /// <summary>
//        /// Returns a string[] or null from the stream.
//        /// </summary>
//        /// <returns>An string[] instance.</returns>
//        public string[] ReadStringArray()
//        {
//            return (string[])ReadObjectArray(typeof(string));
//        }

//        /// <summary>
//        /// Returns a UInt16[] from the stream.
//        /// </summary>
//        /// <returns>A UInt16[] instance; or null.</returns>
//        //[CLSCompliant(false)]
//        public ushort[] ReadUInt16Array()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new ushort[0];
//            else
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
//                ushort[] result = new ushort[ReadOptimizedUInt32()];
//                for (int i = 0; i < result.Length; i++)
//                {
//                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                        result[i] = ReadUInt16();
//                    else
//                    {
//                        result[i] = ReadOptimizedUInt16();
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns a Boolean[] from the stream.
//        /// </summary>
//        /// <returns>A Boolean[] instance; or null.</returns>
//        public bool[] ReadBooleanArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new bool[0];
//            else
//            {
//                return readBooleanArray();
//            }
//        }

//        /// <summary>
//        /// Returns a DateTime[] from the stream.
//        /// </summary>
//        /// <returns>A DateTime[] instance; or null.</returns>
//        public DateTime[] ReadDateTimeArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new DateTime[0];
//            else
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
//                DateTime[] result = new DateTime[ReadOptimizedInt32()];
//                for (int i = 0; i < result.Length; i++)
//                {
//                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                        result[i] = ReadDateTime();
//                    else
//                    {
//                        result[i] = ReadOptimizedDateTime();
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns a Decimal[] from the stream.
//        /// </summary>
//        /// <returns>A Decimal[] instance; or null.</returns>
//        public decimal[] ReadDecimalArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new decimal[0];
//            else
//            {
//                return readDecimalArray();
//            }
//        }

//        /// <summary>
//        /// Returns an Int32[] from the stream.
//        /// </summary>
//        /// <returns>An Int32[] instance; or null.</returns>
//        public int[] ReadInt32Array()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new int[0];
//            else
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
//                int[] result = new int[ReadOptimizedInt32()];
//                for (int i = 0; i < result.Length; i++)
//                {
//                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                        result[i] = ReadInt32();
//                    else
//                    {
//                        result[i] = ReadOptimizedInt32();
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns an Int64[] from the stream.
//        /// </summary>
//        /// <returns>An Int64[] instance; or null.</returns>
//        public long[] ReadInt64Array()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new long[0];
//            else
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
//                long[] result = new long[ReadOptimizedInt64()];
//                for (int i = 0; i < result.Length; i++)
//                {
//                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                        result[i] = ReadInt64();
//                    else
//                    {
//                        result[i] = ReadOptimizedInt64();
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns a string[] from the stream that was stored optimized.
//        /// </summary>
//        /// <returns>An string[] instance.</returns>
//        public string[] ReadOptimizedStringArray()
//        {
//            return (string[])ReadOptimizedObjectArray(typeof(string));
//        }

//        /// <summary>
//        /// Returns a TimeSpan[] from the stream.
//        /// </summary>
//        /// <returns>A TimeSpan[] instance; or null.</returns>
//        public TimeSpan[] ReadTimeSpanArray()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new TimeSpan[0];
//            else
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
//                TimeSpan[] result = new TimeSpan[ReadOptimizedInt32()];
//                for (int i = 0; i < result.Length; i++)
//                {
//                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                        result[i] = ReadTimeSpan();
//                    else
//                    {
//                        result[i] = ReadOptimizedTimeSpan();
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns a UInt[] from the stream.
//        /// </summary>
//        /// <returns>A UInt[] instance; or null.</returns>
//        //[CLSCompliant(false)]
//        public uint[] ReadUInt32Array()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new uint[0];
//            else
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
//                uint[] result = new uint[ReadOptimizedUInt32()];
//                for (int i = 0; i < result.Length; i++)
//                {
//                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                        result[i] = ReadUInt32();
//                    else
//                    {
//                        result[i] = ReadOptimizedUInt32();
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns a UInt64[] from the stream.
//        /// </summary>
//        /// <returns>A UInt64[] instance; or null.</returns>
//        //[CLSCompliant((false))]
//        public ulong[] ReadUInt64Array()
//        {
//            SerializedType t = readTypeCode();
//            if (t == SerializedType.NullType)
//                return null;
//            else if (t == SerializedType.EmptyTypedArrayType)
//                return new ulong[0];
//            else
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(t);
//                ulong[] result = new ulong[ReadOptimizedInt64()];
//                for (int i = 0; i < result.Length; i++)
//                {
//                    if (optimizeFlags == null || (optimizeFlags != FullyOptimizableTypedArray && !optimizeFlags[i]))
//                        result[i] = ReadUInt64();
//                    else
//                    {
//                        result[i] = ReadOptimizedUInt64();
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// Returns a Boolean[] from the stream.
//        /// </summary>
//        /// <returns>A Boolean[] instance; or null.</returns>
//        public bool[] ReadOptimizedBooleanArray()
//        {
//            return ReadBooleanArray();
//        }

//        /// <summary>
//        /// Returns a DateTime[] from the stream.
//        /// </summary>
//        /// <returns>A DateTime[] instance; or null.</returns>
//        public DateTime[] ReadOptimizedDateTimeArray()
//        {
//            return ReadDateTimeArray();
//        }

//        /// <summary>
//        /// Returns a Decimal[] from the stream.
//        /// </summary>
//        /// <returns>A Decimal[] instance; or null.</returns>
//        public decimal[] ReadOptimizedDecimalArray()
//        {
//            return ReadDecimalArray();
//        }

//        /// <summary>
//        /// Returns a Int16[] from the stream.
//        /// </summary>
//        /// <returns>An Int16[] instance; or null.</returns>
//        public short[] ReadOptimizedInt16Array()
//        {
//            return ReadInt16Array();
//        }

//        /// <summary>
//        /// Returns a Int32[] from the stream.
//        /// </summary>
//        /// <returns>An Int32[] instance; or null.</returns>
//        public int[] ReadOptimizedInt32Array()
//        {
//            return ReadInt32Array();
//        }

//        /// <summary>
//        /// Returns a Int64[] from the stream.
//        /// </summary>
//        /// <returns>A Int64[] instance; or null.</returns>
//        public long[] ReadOptimizedInt64Array()
//        {
//            return ReadInt64Array();
//        }

//        /// <summary>
//        /// Returns a TimeSpan[] from the stream.
//        /// </summary>
//        /// <returns>A TimeSpan[] instance; or null.</returns>
//        public TimeSpan[] ReadOptimizedTimeSpanArray()
//        {
//            return ReadTimeSpanArray();
//        }

//        /// <summary>
//        /// Returns a UInt16[] from the stream.
//        /// </summary>
//        /// <returns>A UInt16[] instance; or null.</returns>
//        //[CLSCompliant(false)]
//        public ushort[] ReadOptimizedUInt16Array()
//        {
//            return ReadUInt16Array();
//        }

//        /// <summary>
//        /// Returns a UInt32[] from the stream.
//        /// </summary>
//        /// <returns>A UInt32[] instance; or null.</returns>
//        //[CLSCompliant(false)]
//        public uint[] ReadOptimizedUInt32Array()
//        {
//            return ReadUInt32Array();
//        }

//        /// <summary>
//        /// Returns a UInt64[] from the stream.
//        /// </summary>
//        /// <returns>A UInt64[] instance; or null.</returns>
//        //[CLSCompliant(false)]
//        public ulong[] ReadOptimizedUInt64Array()
//        {
//            return ReadUInt64Array();
//        }

//        /// <summary>
//        /// Allows an existing object, implementing IOwnedDataSerializable, to 
//        /// retrieve its owned data from the stream.
//        /// </summary>
//        /// <param name="target">Any IOwnedDataSerializable object.</param>
//        /// <param name="context">An optional, arbitrary object to allow context to be provided.</param>
//        public void ReadOwnedData(IOwnedDataSerializable target, object context)
//        {
//            target.DeserializeOwnedData(this, context);
//        }

//        /// <summary>
//        /// Returns the object associated with the object token read next from the stream.
//        /// </summary>
//        /// <returns>An object.</returns>
//        public object ReadTokenizedObject()
//        {
//            return objectTokens[ReadOptimizedInt32()];
//        }
//        #endregion Methods

//        #region Private Methods
//        /// <summary>
//        /// Returns a TimeSpan decoded from packed data.
//        /// This routine is called from ReadOptimizedDateTime() and ReadOptimizedTimeSpan().
//        /// <remarks>
//        /// This routine uses a parameter to allow ReadOptimizedDateTime() to 'peek' at the
//        /// next byte and extract the DateTimeKind from bits one and two (IsNegative and HasDays)
//        /// which are never set for a Time portion of a DateTime.
//        /// </remarks>
//        /// </summary>
//        /// <param name="initialByte">The first of two always-present bytes.</param>
//        /// <returns>A decoded TimeSpan</returns>
//        private TimeSpan decodeTimeSpan(byte initialByte)
//        {
//            bool hasTime;
//            bool hasSeconds;
//            bool hasMilliseconds;
//            long ticks = 0;

//            BitVector32 packedData = new BitVector32(initialByte | (ReadByte() << 8)); // Read first two bytes
//            hasTime = packedData[SerializationWriter.HasTimeSection] == 1;
//            hasSeconds = packedData[SerializationWriter.HasSecondsSection] == 1;
//            hasMilliseconds = packedData[SerializationWriter.HasMillisecondsSection] == 1;

//            if (hasMilliseconds)
//                packedData = new BitVector32(packedData.Data | (ReadByte() << 16) | (ReadByte() << 24));
//            else if (hasSeconds && hasTime)
//            {
//                packedData = new BitVector32(packedData.Data | (ReadByte() << 16));
//            }

//            if (hasTime)
//            {
//                ticks += packedData[SerializationWriter.HoursSection] * TimeSpan.TicksPerHour;
//                ticks += packedData[SerializationWriter.MinutesSection] * TimeSpan.TicksPerMinute;
//            }

//            if (hasSeconds)
//            {
//                ticks += packedData[(!hasTime && !hasMilliseconds) ? SerializationWriter.MinutesSection
//                                                                     : SerializationWriter.SecondsSection] * TimeSpan.TicksPerSecond;
//            }

//            if (hasMilliseconds)
//            {
//                ticks += packedData[SerializationWriter.MillisecondsSection] * TimeSpan.TicksPerMillisecond;
//            }

//            if (packedData[SerializationWriter.HasDaysSection] == 1)
//            {
//                ticks += ReadOptimizedInt32() * TimeSpan.TicksPerDay;
//            }

//            if (packedData[SerializationWriter.IsNegativeSection] == 1)
//            {
//                ticks = -ticks;
//            }

//            return new TimeSpan(ticks);
//        }

//        /// <summary>
//        /// Creates a BitArray representing which elements of a typed array
//        /// are serializable.
//        /// </summary>
//        /// <param name="serializedType">The type of typed array.</param>
//        /// <returns>A BitArray denoting which elements are serializable.</returns>
//        private BitArray readTypedArrayOptimizeFlags(SerializedType serializedType)
//        {
//            BitArray optimizableFlags = null;
//            if (serializedType == SerializedType.FullyOptimizedTypedArrayType)
//                optimizableFlags = FullyOptimizableTypedArray;
//            else if (serializedType == SerializedType.PartiallyOptimizedTypedArrayType)
//            {
//                optimizableFlags = ReadOptimizedBitArray();
//            }
//            return optimizableFlags;
//        }

//        /// <summary>
//        /// Returns an object based on supplied SerializedType.
//        /// </summary>
//        /// <returns>An object instance.</returns>
//        private object processObject(SerializedType typeCode)
//        {
//            if (typeCode == SerializedType.NullType)
//                return null;

//            else if (typeCode == SerializedType.Int32Type)
//                return ReadInt32();

//            else if (typeCode == SerializedType.EmptyStringType)
//                return string.Empty;

//            else if (typeCode < SerializedType.NullType)
//                return readTokenizedString((int)typeCode);

//            else if (typeCode == SerializedType.BooleanFalseType)
//                return false;

//            else if (typeCode == SerializedType.ZeroInt32Type)
//                return (Int32)0;

//            else if (typeCode == SerializedType.OptimizedInt32Type)
//                return ReadOptimizedInt32();

//            else if (typeCode == SerializedType.OptimizedInt32NegativeType)
//                return -ReadOptimizedInt32() - 1;

//            else if (typeCode == SerializedType.DecimalType)
//                return ReadOptimizedDecimal();

//            else if (typeCode == SerializedType.ZeroDecimalType)
//                return (Decimal)0;

//            else if (typeCode == SerializedType.YStringType)
//                return "Y";

//            else if (typeCode == SerializedType.DateTimeType)
//                return ReadDateTime();

//            else if (typeCode == SerializedType.OptimizedDateTimeType)
//                return ReadOptimizedDateTime();

//            else if (typeCode == SerializedType.SingleCharStringType)
//                return Char.ToString(ReadChar());

//            else if (typeCode == SerializedType.SingleSpaceType)
//                return " ";

//            else if (typeCode == SerializedType.OneInt32Type)
//                return (Int32)1;

//            else if (typeCode == SerializedType.OptimizedInt16Type)
//                return ReadOptimizedInt16();

//            else if (typeCode == SerializedType.OptimizedInt16NegativeType)
//                return -ReadOptimizedInt16() - 1;

//            else if (typeCode == SerializedType.OneDecimalType)
//                return (Decimal)1;

//            else if (typeCode == SerializedType.BooleanTrueType)
//                return true;

//            else if (typeCode == SerializedType.NStringType)
//                return "N";

//            else if (typeCode == SerializedType.DBNullType)
//                return DBNull.Value;

//            else if (typeCode == SerializedType.ObjectArrayType)
//                return ReadOptimizedObjectArray();

//            else if (typeCode == SerializedType.EmptyObjectArrayType)
//                return new object[0];

//            else if (typeCode == SerializedType.MinusOneInt32Type)
//                return (Int32)(-1);

//            else if (typeCode == SerializedType.MinusOneInt64Type)
//                return (Int64)(-1);

//            else if (typeCode == SerializedType.MinusOneInt16Type)
//                return (Int16)(-1);

//            else if (typeCode == SerializedType.MinDateTimeType)
//                return DateTime.MinValue;

//            else if (typeCode == SerializedType.GuidType)
//                return ReadGuid();

//            else if (typeCode == SerializedType.EmptyGuidType)
//                return Guid.Empty;

//            else if (typeCode == SerializedType.TimeSpanType)
//                return ReadTimeSpan();

//            else if (typeCode == SerializedType.MaxDateTimeType)
//                return DateTime.MaxValue;

//            else if (typeCode == SerializedType.ZeroTimeSpanType)
//                return TimeSpan.Zero;

//            else if (typeCode == SerializedType.OptimizedTimeSpanType)
//                return ReadOptimizedTimeSpan();

//            else if (typeCode == SerializedType.DoubleType)
//                return ReadDouble();

//            else if (typeCode == SerializedType.ZeroDoubleType)
//                return (Double)0;

//            else if (typeCode == SerializedType.Int64Type)
//                return ReadInt64();

//            else if (typeCode == SerializedType.ZeroInt64Type)
//                return (Int64)0;

//            else if (typeCode == SerializedType.OptimizedInt64Type)
//                return ReadOptimizedInt64();

//            else if (typeCode == SerializedType.OptimizedInt64NegativeType)
//                return -ReadOptimizedInt64() - 1;

//            else if (typeCode == SerializedType.Int16Type)
//                return ReadInt16();

//            else if (typeCode == SerializedType.ZeroInt16Type)
//                return (Int16)0;

//            else if (typeCode == SerializedType.SingleType)
//                return ReadSingle();

//            else if (typeCode == SerializedType.ZeroSingleType)
//                return (Single)0;

//            else if (typeCode == SerializedType.ByteType)
//                return ReadByte();

//            else if (typeCode == SerializedType.ZeroByteType)
//                return (Byte)0;

//            else if (typeCode == SerializedType.OtherType)
//                return new BinaryFormatter().Deserialize(BaseStream);

//            else if (typeCode == SerializedType.UInt16Type)
//                return ReadUInt16();

//            else if (typeCode == SerializedType.ZeroUInt16Type)
//                return (UInt16)0;

//            else if (typeCode == SerializedType.UInt32Type)
//                return ReadUInt32();

//            else if (typeCode == SerializedType.ZeroUInt32Type)
//                return (UInt32)0;

//            else if (typeCode == SerializedType.OptimizedUInt32Type)
//                return ReadOptimizedUInt32();

//            else if (typeCode == SerializedType.UInt64Type)
//                return ReadUInt64();

//            else if (typeCode == SerializedType.ZeroUInt64Type)
//                return (UInt64)0;

//            else if (typeCode == SerializedType.OptimizedUInt64Type)
//                return ReadOptimizedUInt64();

//            else if (typeCode == SerializedType.BitVector32Type)
//                return ReadBitVector32();

//            else if (typeCode == SerializedType.CharType)
//                return ReadChar();

//            else if (typeCode == SerializedType.ZeroCharType)
//                return (Char)0;

//            else if (typeCode == SerializedType.SByteType)
//                return ReadSByte();

//            else if (typeCode == SerializedType.ZeroSByteType)
//                return (SByte)0;

//            else if (typeCode == SerializedType.OneByteType)
//                return (Byte)1;

//            else if (typeCode == SerializedType.OneDoubleType)
//                return (Double)1;

//            else if (typeCode == SerializedType.OneCharType)
//                return (Char)1;

//            else if (typeCode == SerializedType.OneInt16Type)
//                return (Int16)1;

//            else if (typeCode == SerializedType.OneInt64Type)
//                return (Int64)1;

//            else if (typeCode == SerializedType.OneUInt16Type)
//                return (UInt16)1;

//            else if (typeCode == SerializedType.OptimizedUInt16Type)
//                return ReadOptimizedUInt16();

//            else if (typeCode == SerializedType.OneUInt32Type)
//                return (UInt32)1;

//            else if (typeCode == SerializedType.OneUInt64Type)
//                return (UInt64)1;

//            else if (typeCode == SerializedType.OneSByteType)
//                return (SByte)1;

//            else if (typeCode == SerializedType.OneSingleType)
//                return (Single)1;

//            else if (typeCode == SerializedType.BitArrayType)
//                return ReadOptimizedBitArray();

//            else if (typeCode == SerializedType.TypeType)
//                return Type.GetType(ReadOptimizedString(), false);

//            else if (typeCode == SerializedType.ArrayListType)
//                return ReadOptimizedArrayList();

//            else if (typeCode == SerializedType.SingleInstanceType)
//            {
//                try
//                {
//                    Type type = Type.GetType(ReadStringDirect());
//                    return Activator.CreateInstance(type, true);
//                }
//                catch
//                {
//                    return null;
//                }
//            }

//            else if (typeCode == SerializedType.OwnedDataSerializableAndRecreatableType)
//            {
//                Type structType = ReadOptimizedType();
//                object result = Activator.CreateInstance(structType);
//                ReadOwnedData((IOwnedDataSerializable)result, null);
//                return result;
//            }

//            else if (typeCode == SerializedType.OptimizedEnumType)
//            {
//                Type enumType = ReadOptimizedType();
//                Type underlyingType = Enum.GetUnderlyingType(enumType);
//                if (underlyingType == typeof(int) || underlyingType == typeof(uint) || underlyingType == typeof(long) || underlyingType == typeof(ulong))
//                    return Enum.ToObject(enumType, ReadOptimizedUInt64());
//                else
//                {
//                    return Enum.ToObject(enumType, ReadUInt64());
//                }
//            }

//            else if (typeCode == SerializedType.EnumType)
//            {
//                Type enumType = ReadOptimizedType();
//                Type underlyingType = Enum.GetUnderlyingType(enumType);
//                if (underlyingType == typeof(Int32))
//                    return Enum.ToObject(enumType, ReadInt32());
//                else if (underlyingType == typeof(Byte))
//                    return Enum.ToObject(enumType, ReadByte());
//                else if (underlyingType == typeof(Int16))
//                    return Enum.ToObject(enumType, ReadInt16());
//                else if (underlyingType == typeof(UInt32))
//                    return Enum.ToObject(enumType, ReadUInt32());
//                else if (underlyingType == typeof(Int64))
//                    return Enum.ToObject(enumType, ReadInt64());
//                else if (underlyingType == typeof(SByte))
//                    return Enum.ToObject(enumType, ReadSByte());
//                else if (underlyingType == typeof(UInt16))
//                    return Enum.ToObject(enumType, ReadUInt16());
//                else
//                {
//                    return Enum.ToObject(enumType, ReadUInt64());
//                }
//            }

//            else if (typeCode == SerializedType.SurrogateHandledType)
//            {
//                Type serializedType = ReadOptimizedType();
//                IFastSerializationTypeSurrogate typeSurrogate = SerializationWriter.findSurrogateForType(serializedType);
//                return typeSurrogate.Deserialize(this, serializedType);
//            }

//            else
//            {
//                object result = processArrayTypes(typeCode, null);
//                if (result != null) return result;
//                throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
//            }
//        }

//        /// <summary>
//        /// Determine whether the passed-in type code refers to an array type
//        /// and deserializes the array if it is.
//        /// Returns null if not an array type.
//        /// </summary>
//        /// <param name="typeCode">The SerializedType to check.</param>
//        /// <param name="defaultElementType">The myObjectStream of array element; null if to be read from stream.</param>
//        /// <returns></returns>
//        private object processArrayTypes(SerializedType typeCode, Type defaultElementType)
//        {
//            if (typeCode == SerializedType.StringArrayType)
//                return ReadOptimizedStringArray();

//            else if (typeCode == SerializedType.Int32ArrayType)
//                return ReadInt32Array();

//            else if (typeCode == SerializedType.Int64ArrayType)
//                return ReadInt64Array();

//            else if (typeCode == SerializedType.DecimalArrayType)
//                return readDecimalArray();

//            else if (typeCode == SerializedType.TimeSpanArrayType)
//                return ReadTimeSpanArray();

//            else if (typeCode == SerializedType.UInt32ArrayType)
//                return ReadUInt32Array();

//            else if (typeCode == SerializedType.UInt64ArrayType)
//                return ReadUInt64Array();

//            else if (typeCode == SerializedType.DateTimeArrayType)
//                return ReadDateTimeArray();

//            else if (typeCode == SerializedType.BooleanArrayType)
//                return readBooleanArray();

//            else if (typeCode == SerializedType.ByteArrayType)
//                return readByteArray();

//            else if (typeCode == SerializedType.CharArrayType)
//                return readCharArray();

//            else if (typeCode == SerializedType.DoubleArrayType)
//                return readDoubleArray();

//            else if (typeCode == SerializedType.SingleArrayType)
//                return readSingleArray();

//            else if (typeCode == SerializedType.GuidArrayType)
//                return readGuidArray();

//            else if (typeCode == SerializedType.SByteArrayType)
//                return readSByteArray();

//            else if (typeCode == SerializedType.Int16ArrayType)
//                return ReadInt16Array();

//            else if (typeCode == SerializedType.UInt16ArrayType)
//                return ReadUInt16Array();

//            else if (typeCode == SerializedType.EmptyTypedArrayType)
//                return Array.CreateInstance(defaultElementType != null ? defaultElementType : ReadOptimizedType(), 0);

//            else if (typeCode == SerializedType.OtherTypedArrayType)
//                return ReadOptimizedObjectArray(ReadOptimizedType());

//            else if (typeCode == SerializedType.ObjectArrayType)
//                return ReadOptimizedObjectArray(defaultElementType);

//            else if (typeCode == SerializedType.FullyOptimizedTypedArrayType ||
//                     typeCode == SerializedType.PartiallyOptimizedTypedArrayType ||
//                     typeCode == SerializedType.NonOptimizedTypedArrayType)
//            {
//                BitArray optimizeFlags = readTypedArrayOptimizeFlags(typeCode);
//                int length = ReadOptimizedInt32();
//                if (defaultElementType == null) defaultElementType = ReadOptimizedType();

//                Array result = Array.CreateInstance(defaultElementType, length);

//                for (int i = 0; i < length; i++)
//                {
//                    if (optimizeFlags == null)
//                        result.SetValue(ReadObject(), i);
//                    else if (optimizeFlags == FullyOptimizableTypedArray || !optimizeFlags[i])
//                    {
//                        IOwnedDataSerializable value = (IOwnedDataSerializable)Activator.CreateInstance(defaultElementType);
//                        ReadOwnedData(value, null);
//                        result.SetValue(value, i);
//                    }
//                }

//                return result;
//            }

//            return null;
//        }

//        /// <summary>
//        /// Returns the string value associated with the string token read next from the stream.
//        /// </summary>
//        /// <returns>A DateTime value.</returns>
//        private string readTokenizedString(int bucket)
//        {
//            return stringTokenList[(ReadOptimizedInt32() << 7) + bucket];
//        }

//        /// <summary>
//        /// Returns the SerializedType read next from the stream.
//        /// </summary>
//        /// <returns>A SerializedType value.</returns>
//        private SerializedType readTypeCode()
//        {
//            return (SerializedType)ReadByte();
//        }

//        /// <summary>
//        /// Internal implementation returning a Bool[].
//        /// </summary>
//        /// <returns>A Bool[].</returns>
//        private bool[] readBooleanArray()
//        {
//            BitArray bitArray = ReadOptimizedBitArray();
//            bool[] result = new bool[bitArray.Count];
//            for (int i = 0; i < result.Length; i++)
//            {
//                result[i] = bitArray[i];
//            }

//            return result;
//        }

//        /// <summary>
//        /// Internal implementation returning a Byte[].
//        /// </summary>
//        /// <returns>A Byte[].</returns>
//        private byte[] readByteArray()
//        {
//            return base.ReadBytes(ReadOptimizedInt32());
//        }

//        /// <summary>
//        /// Internal implementation returning a Char[].
//        /// </summary>
//        /// <returns>A Char[].</returns>
//        private char[] readCharArray()
//        {
//            return base.ReadChars(ReadOptimizedInt32());
//        }

//        /// <summary>
//        /// Internal implementation returning a Decimal[].
//        /// </summary>
//        /// <returns>A Decimal[].</returns>
//        private decimal[] readDecimalArray()
//        {
//            decimal[] result = new decimal[ReadOptimizedInt32()];
//            for (int i = 0; i < result.Length; i++)
//            {
//                result[i] = ReadOptimizedDecimal();
//            }

//            return result;
//        }

//        /// <summary>
//        /// Internal implementation returning a Double[].
//        /// </summary>
//        /// <returns>A Double[].</returns>
//        private double[] readDoubleArray()
//        {
//            double[] result = new double[ReadOptimizedInt32()];
//            for (int i = 0; i < result.Length; i++)
//            {
//                result[i] = ReadDouble();
//            }

//            return result;
//        }

//        /// <summary>
//        /// Internal implementation returning a Guid[].
//        /// </summary>
//        /// <returns>A Guid[].</returns>
//        private Guid[] readGuidArray()
//        {
//            Guid[] result = new Guid[ReadOptimizedInt32()];
//            for (int i = 0; i < result.Length; i++)
//            {
//                result[i] = ReadGuid();
//            }

//            return result;
//        }

//        /// <summary>
//        /// Internal implementation returning an SByte[].
//        /// </summary>
//        /// <returns>An SByte[].</returns>
//        private sbyte[] readSByteArray()
//        {
//            sbyte[] result = new sbyte[ReadOptimizedInt32()];
//            for (int i = 0; i < result.Length; i++)
//            {
//                result[i] = ReadSByte();
//            }

//            return result;
//        }

//        /// <summary>
//        /// Internal implementation returning a Single[].
//        /// </summary>
//        /// <returns>A Single[].</returns>
//        private float[] readSingleArray()
//        {
//            float[] result = new float[ReadOptimizedInt32()];
//            for (int i = 0; i < result.Length; i++)
//            {
//                result[i] = ReadSingle();
//            }
//            return result;
//        }
//        #endregion Private Methods

//        #region Debug
//        [Conditional("DEBUG")]
//        public void DumpStringTables(ArrayList list)
//        {
//            list.AddRange(stringTokenList);
//        }
//        #endregion Debug
//    }

//    /// <summary>
//    /// Exception thrown when a value being optimized does not meet the required criteria for optimization.
//    /// </summary>
//    public class OptimizationException : Exception
//    {
//        public OptimizationException(string message) : base(message) { }
//    }

//    /// <summary>
//    /// Allows a class to specify that it can be recreated during deserialization using a default constructor
//    /// and then calling DeserializeOwnedData()
//    /// </summary>
//    public interface IOwnedDataSerializableAndRecreatable : IOwnedDataSerializable { }

//    /// <summary>
//    /// Allows a class to save/retrieve their internal data to/from an existing SerializationWriter/SerializationReader.
//    /// </summary>
//    public interface IOwnedDataSerializable
//    {
//        /// <summary>
//        /// Lets the implementing class store internal data directly into a SerializationWriter.
//        /// </summary>
//        /// <param name="writer">The SerializationWriter to use</param>
//        /// <param name="context">Optional context to use as a hint as to what to store (BitVector32 is useful)</param>
//        void SerializeOwnedData(SerializationWriter writer, object context);

//        /// <summary>
//        /// Lets the implementing class retrieve internal data directly from a SerializationReader.
//        /// </summary>
//        /// <param name="reader">The SerializationReader to use</param>
//        /// <param name="context">Optional context to use as a hint as to what to retrieve (BitVector32 is useful) </param>
//        void DeserializeOwnedData(SerializationReader reader, object context);
//    }

//    /// <summary>
//    /// Interface to allow helper classes to be used to serialize objects
//    /// that are not directly supported by SerializationWriter/SerializationReader
//    /// </summary>
//    public interface IFastSerializationTypeSurrogate
//    {
//        /// <summary>
//        /// Allows a surrogate to be queried as to whether a particular type is supported
//        /// </summary>
//        /// <param name="type">The type being queried</param>
//        /// <returns>true if the type is supported; otherwise false</returns>
//        bool SupportsType(Type type);
//        /// <summary>
//        /// FastSerializes the object into the SerializationWriter.
//        /// </summary>
//        /// <param name="writer">The SerializationWriter into which the object is to be serialized.</param>
//        /// <param name="value">The object to serialize.</param>
//        void Serialize(SerializationWriter writer, object value);
//        /// <summary>
//        /// Deserializes an object of the supplied type from the SerializationReader.
//        /// </summary>
//        /// <param name="reader">The SerializationReader containing the serialized object.</param>
//        /// <param name="type">The type of object required to be deserialized.</param>
//        /// <returns></returns>
//        object Deserialize(SerializationReader reader, Type type);
//    }

//    /// <summary>
//    /// Stores information about a type or type/value.
//    /// Internal use only.
//    /// </summary>
//    internal enum SerializedType : byte
//    {
//        // Codes 0 to 127 reserved for String token tables

//        NullType = 128,            // Used for all null values
//        NullSequenceType,          // Used internally to identify sequences of null values in object[]
//        DBNullType,                // Used for DBNull.Value
//        DBNullSequenceType,        // Used internally to identify sequences of DBNull.Value values in object[] (DataSets)
//        OtherType,                 // Used for any unrecognized types - uses an internal BinaryWriter/Reader.

//        BooleanTrueType,           // Stores Boolean type and values
//        BooleanFalseType,

//        ByteType,                  // Standard numeric value types
//        SByteType,
//        CharType,
//        DecimalType,
//        DoubleType,
//        SingleType,
//        Int16Type,
//        Int32Type,
//        Int64Type,
//        UInt16Type,
//        UInt32Type,
//        UInt64Type,

//        ZeroByteType,              // Optimization to store type and a zero value - all numeric value types
//        ZeroSByteType,
//        ZeroCharType,
//        ZeroDecimalType,
//        ZeroDoubleType,
//        ZeroSingleType,
//        ZeroInt16Type,
//        ZeroInt32Type,
//        ZeroInt64Type,
//        ZeroUInt16Type,
//        ZeroUInt32Type,
//        ZeroUInt64Type,

//        OneByteType,               // Optimization to store type and a one value - all numeric value types
//        OneSByteType,
//        OneCharType,
//        OneDecimalType,
//        OneDoubleType,
//        OneSingleType,
//        OneInt16Type,
//        OneInt32Type,
//        OneInt64Type,
//        OneUInt16Type,
//        OneUInt32Type,
//        OneUInt64Type,

//        MinusOneInt16Type,         // Optimization to store type and a minus one value - Signed Integer types only
//        MinusOneInt32Type,
//        MinusOneInt64Type,

//        OptimizedInt16Type,        // Optimizations for specific value types
//        OptimizedInt16NegativeType,
//        OptimizedUInt16Type,
//        OptimizedInt32Type,
//        OptimizedInt32NegativeType,
//        OptimizedUInt32Type,
//        OptimizedInt64Type,
//        OptimizedInt64NegativeType,
//        OptimizedUInt64Type,
//        OptimizedDateTimeType,
//        OptimizedTimeSpanType,


//        EmptyStringType,           // String type and optimizations
//        SingleSpaceType,
//        SingleCharStringType,
//        YStringType,
//        NStringType,

//        DateTimeType,              // Date type and optimizations
//        MinDateTimeType,
//        MaxDateTimeType,

//        TimeSpanType,              // TimeSpan type and optimizations
//        ZeroTimeSpanType,

//        GuidType,                  // Guid type and optimizations
//        EmptyGuidType,

//        BitVector32Type,           // Specific optimization for BitVector32 type

//        DuplicateValueType,        // Used internally by Optimized object[] pair to identify values in the 
//        // second array that are identical to those in the first
//        DuplicateValueSequenceType,

//        BitArrayType,              // Specific optimization for BitArray

//        TypeType,                  // Identifies a myObjectStream type 

//        SingleInstanceType,        // Used internally to identify that a single instance object should be created
//        // (by storing the myObjectStream and using Activator.GetInstance() at deserialization time)

//        ArrayListType,             // Specific optimization for ArrayList type


//        ObjectArrayType,           // Array types
//        EmptyTypedArrayType,
//        EmptyObjectArrayType,

//        NonOptimizedTypedArrayType, // Identifies a typed array and how it is optimized
//        FullyOptimizedTypedArrayType,
//        PartiallyOptimizedTypedArrayType,
//        OtherTypedArrayType,

//        BooleanArrayType,
//        ByteArrayType,
//        CharArrayType,
//        DateTimeArrayType,
//        DecimalArrayType,
//        DoubleArrayType,
//        SingleArrayType,
//        GuidArrayType,
//        Int16ArrayType,
//        Int32ArrayType,
//        Int64ArrayType,
//        SByteArrayType,
//        TimeSpanArrayType,
//        UInt16ArrayType,
//        UInt32ArrayType,
//        UInt64ArrayType,
//        StringArrayType,

//        OwnedDataSerializableAndRecreatableType,

//        EnumType,
//        OptimizedEnumType,

//        SurrogateHandledType,
//        // Placeholders to indicate number of myObjectStream Codes remaining
//        ObjectLocatorType,
//        Reserved23,
//        Reserved22,
//        Reserved21,
//        Reserved20,
//        Reserved19,
//        Reserved18,
//        Reserved17,
//        Reserved16,
//        Reserved15,
//        Reserved14,
//        Reserved13,
//        Reserved12,
//        Reserved11,
//        Reserved10,
//        Reserved9,
//        Reserved8,
//        Reserved7,
//        Reserved6,
//        Reserved5,
//        Reserved4,
//        Reserved3,
//        Reserved2,
//        Reserved1
//    }
//}
