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
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.Library.NewFastSerializer
{

    /// <summary>
    /// Stores information about a type or type/value.
    /// Internal use only.
    /// </summary>
    internal enum SerializedType : byte
    {
        // Codes 0 to 127 reserved for String token tables

        NullType = 128,            // Used for all null values
        NullSequenceType,          // Used internally to identify sequences of null values in object[]
        DBNullType,                // Used for DBNull.Value
        DBNullSequenceType,        // Used internally to identify sequences of DBNull.Value values in object[] (DataSets)
        OtherType,                 // Used for any unrecognized types - uses an internal BinaryWriter/Reader.

        BooleanTrueType,           // Stores Boolean type and values
        BooleanFalseType,

        ByteType,                  // Standard numeric value types
        SByteType,
        CharType,
        DecimalType,
        DoubleType,
        SingleType,
        Int16Type,
        Int32Type,
        Int64Type,
        UInt16Type,
        UInt32Type,
        UInt64Type,

        ZeroByteType,              // Optimization to store type and a zero value - all numeric value types
        ZeroSByteType,
        ZeroCharType,
        ZeroDecimalType,
        ZeroDoubleType,
        ZeroSingleType,
        ZeroInt16Type,
        ZeroInt32Type,
        ZeroInt64Type,
        ZeroUInt16Type,
        ZeroUInt32Type,
        ZeroUInt64Type,

        OneByteType,               // Optimization to store type and a one value - all numeric value types
        OneSByteType,
        OneCharType,
        OneDecimalType,
        OneDoubleType,
        OneSingleType,
        OneInt16Type,
        OneInt32Type,
        OneInt64Type,
        OneUInt16Type,
        OneUInt32Type,
        OneUInt64Type,

        MinusOneInt16Type,         // Optimization to store type and a minus one value - Signed Integer types only
        MinusOneInt32Type,
        MinusOneInt64Type,

        OptimizedInt16Type,        // Optimizations for specific value types
        OptimizedInt16NegativeType,
        OptimizedUInt16Type,
        OptimizedInt32Type,
        OptimizedInt32NegativeType,
        OptimizedUInt32Type,
        OptimizedInt64Type,
        OptimizedInt64NegativeType,
        OptimizedUInt64Type,
        OptimizedDateTimeType,
        OptimizedTimeSpanType,


        EmptyStringType,           // String type and optimizations
        SingleSpaceType,
        SingleCharStringType,
        YStringType,
        NStringType,

        DateTimeType,              // Date type and optimizations
        MinDateTimeType,
        MaxDateTimeType,

        TimeSpanType,              // TimeSpan type and optimizations
        ZeroTimeSpanType,

        GuidType,                  // Guid type and optimizations
        EmptyGuidType,

        BitVector32Type,           // Specific optimization for BitVector32 type

        DuplicateValueType,        // Used internally by Optimized object[] pair to identify values in the 
        // second array that are identical to those in the first
        DuplicateValueSequenceType,

        BitArrayType,              // Specific optimization for BitArray

        TypeType,                  // Identifies a myObjectStream type 

        SingleInstanceType,        // Used internally to identify that a single instance object should be created
        // (by storing the myObjectStream and using Activator.GetInstance() at deserialization time)

        ArrayListType,             // Specific optimization for ArrayList type


        ObjectArrayType,           // Array types
        EmptyTypedArrayType,
        EmptyObjectArrayType,

        NonOptimizedTypedArrayType, // Identifies a typed array and how it is optimized
        FullyOptimizedTypedArrayType,
        PartiallyOptimizedTypedArrayType,
        OtherTypedArrayType,

        BooleanArrayType,
        ByteArrayType,
        CharArrayType,
        DateTimeArrayType,
        DecimalArrayType,
        DoubleArrayType,
        SingleArrayType,
        GuidArrayType,
        Int16ArrayType,
        Int32ArrayType,
        Int64ArrayType,
        SByteArrayType,
        TimeSpanArrayType,
        UInt16ArrayType,
        UInt32ArrayType,
        UInt64ArrayType,
        StringArrayType,

        OwnedDataSerializableAndRecreatableType,

        EnumType,
        OptimizedEnumType,

        SurrogateHandledType,
        // Placeholders to indicate number of myObjectStream Codes remaining
        ObjectLocatorType,
        StringType, 
        Reserved22,
        Reserved21,
        Reserved20,
        Reserved19,
        Reserved18,
        Reserved17,
        Reserved16,
        Reserved15,
        Reserved14,
        Reserved13,
        Reserved12,
        Reserved11,
        Reserved10,
        Reserved9,
        Reserved8,
        Reserved7,
        Reserved6,
        Reserved5,
        Reserved4,
        Reserved3,
        Reserved2,
        Reserved1

    }

}
