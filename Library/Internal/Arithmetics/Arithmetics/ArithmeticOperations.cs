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

using System;
using sones.Library.ErrorHandling;

namespace sones.Library.Arithmetics
{
    /// <summary>
    /// Includes arithmetic operations
    /// </summary>
    public static class ArithmeticOperations
    {
        /// <summary>
        /// Summerizes the given operands, denpending on the given type
        /// </summary>
        /// <param name="myType">The type of the operand</param>
        /// <param name="myLeft">The left operand</param>
        /// <param name="myRight">The right operand</param>
        /// <exception cref="InvalidCastException">Is thrown if operation fails</exception>
        /// <returns>Result of Summarization in case of valid type, else NULL or an Exception is thrown</returns>
        public static IComparable Add(Type myType, IComparable myLeft, IComparable myRight)
        {
            //make sure that either the dividend and the divisor are not NULL
            if ((myLeft != null) &&
                (myRight != null))
            {

                #region check types and calculate

                if (myType == typeof(Int16))
                {
                    return Convert.ToInt16(myLeft) + Convert.ToInt16(myRight);
                }
                else if (myType == typeof(UInt16))
                {
                    return Convert.ToUInt16(myLeft) + Convert.ToUInt16(myRight);
                }
                else if (myType == typeof(Int32))
                {
                    return Convert.ToInt32(myLeft) + Convert.ToInt32(myRight);
                }
                else if (myType == typeof(UInt32))
                {
                    return Convert.ToUInt32(myLeft) + Convert.ToUInt32(myRight);
                }
                else if (myType == typeof(Int64))
                {
                    return Convert.ToInt64(myLeft) + Convert.ToInt64(myRight);
                }
                else if (myType == typeof(UInt64))
                {
                    return Convert.ToUInt64(myLeft) + Convert.ToUInt64(myRight);
                }
                else if (myType == typeof(Double))
                {
                    return Convert.ToDouble(myLeft) + Convert.ToDouble(myRight);
                }
                else if (myType == typeof(Single))
                {
                    return Convert.ToSingle(myLeft) + Convert.ToSingle(myRight);
                }
                else
                {
                    throw new ArithmeticException();
                }

                #endregion

            }

            return null;
        }

        /// <summary>
        /// Subtracts the given operands, depending on the type
        /// </summary>
        /// <param name="myType">The type of the operands</param>
        /// <param name="mySubtrahend">The subtrahend of the operation</param>
        /// <param name="myMinuend">The Minuend of the operation</param>
        /// <returns></returns>
        public static IComparable Sub(Type myType, IComparable mySubtrahend, IComparable myMinuend)
        {
            //make sure that either the dividend and the divisor are not NULL and not 0
            if ((mySubtrahend != null) &&
                (myMinuend != null))
            {

                #region check types and calculate

                if (myType == typeof(Int16))
                {
                    return Convert.ToInt16(mySubtrahend) - Convert.ToInt16(myMinuend);
                }
                else if (myType == typeof(UInt16))
                {
                    return Convert.ToUInt16(mySubtrahend) - Convert.ToUInt16(myMinuend);
                }
                else if (myType == typeof(Int32))
                {
                    return Convert.ToInt32(mySubtrahend) - Convert.ToInt32(myMinuend);
                }
                else if (myType == typeof(UInt32))
                {
                    return Convert.ToUInt32(mySubtrahend) - Convert.ToUInt32(myMinuend);
                }
                else if (myType == typeof(Int64))
                {
                    return Convert.ToInt64(mySubtrahend) - Convert.ToInt64(myMinuend);
                }
                else if (myType == typeof(UInt64))
                {
                    return Convert.ToUInt64(mySubtrahend) - Convert.ToUInt64(myMinuend);
                }
                else if (myType == typeof(Double))
                {
                    return Convert.ToDouble(mySubtrahend) - Convert.ToDouble(myMinuend);
                }
                else if (myType == typeof(Single))
                {
                    return Convert.ToSingle(mySubtrahend) - Convert.ToSingle(myMinuend);
                }
                else
                {
                    throw new ArithmeticException();
                }
                #endregion

            }

            return null;
        }

        /// <summary>
        /// Multiplicates the given operands, depending on the type
        /// </summary>
        /// <param name="myType">The type of the operands</param>
        /// <param name="myLeft">The left operand of the operation</param>
        /// <param name="myRight">The right operand of the operation</param>
        /// <exception cref="InvalidCastException">Is thrown if operation fails</exception>
        /// <returns>Result of the operation in case of valid type, else NULL or an Exception is thrown</returns>
        public static IComparable Mul(Type myType, IComparable myLeft, IComparable myRight)
        {
            //make sure that either the dividend and the divisor are not NULL and not 0
            if ((myLeft != null) &&
                (myRight != null))
            {

                #region check types and calculate

                if (myType == typeof(Int16))
                {
                    return Convert.ToInt16(myLeft) * Convert.ToInt16(myRight);
                }
                else if (myType == typeof(UInt16))
                {
                    return Convert.ToUInt16(myLeft) * Convert.ToUInt16(myRight);
                }
                else if (myType == typeof(Int32))
                {
                    return Convert.ToInt32(myLeft) * Convert.ToInt32(myRight);
                }
                else if (myType == typeof(UInt32))
                {
                    return Convert.ToUInt32(myLeft) * Convert.ToUInt32(myRight);
                }
                else if (myType == typeof(Int64))
                {
                    return Convert.ToInt64(myLeft) * Convert.ToInt64(myRight);
                }
                else if (myType == typeof(UInt64))
                {
                    return Convert.ToUInt64(myLeft) * Convert.ToUInt64(myRight);
                }
                else if (myType == typeof(Double))
                {
                    return Convert.ToDouble(myLeft) * Convert.ToDouble(myRight);
                }
                else if (myType == typeof(Single))
                {
                    return Convert.ToSingle(myLeft) * Convert.ToSingle(myRight);
                }
                else
                {
                    throw new ArithmeticException();
                }
                #endregion

            }

            return null;
        }

        /// <summary>
        /// Divides the given operands, depending on the type
        /// </summary>
        /// <param name="myType">The type of the operands</param>
        /// <param name="myDivident">The Dividend of the operation</param>
        /// <param name="myDivisor">The Divisor of the operation</param>
        /// <exception cref="InvalidCastException">Is thrown if operation fails</exception>
        /// <returns>Result of the operation in case of valid type, else NULL or an Exception is thrown</returns>
        public static IComparable Div(Type myType, IComparable myDivident, IComparable myDivisor)
        {
            //make sure that either the dividend and the divisor are not NULL and not 0
            if ((myDivident != null) &&
                (myDivisor != null))
            {

                #region check types and calculate

                if (myType == typeof(Int16))
                {
                    if ((Convert.ToInt16(myDivident).CompareTo(Convert.ToInt64(0)) != 0) && (Convert.ToInt16(myDivisor).CompareTo(Convert.ToInt64(0)) != 0))
                        return Convert.ToInt16(myDivident) / Convert.ToInt16(myDivisor);
                }
                else if (myType == typeof(UInt16))
                {
                    if ((Convert.ToUInt16(myDivident).CompareTo(Convert.ToUInt64(0)) != 0) && (Convert.ToUInt16(myDivisor).CompareTo(Convert.ToUInt64(0)) != 0))
                        return Convert.ToUInt16(myDivident) / Convert.ToUInt16(myDivisor);
                }
                else if (myType == typeof(Int32))
                {
                    if ((Convert.ToInt32(myDivident).CompareTo(Convert.ToInt32(0)) != 0) && (Convert.ToInt32(myDivisor).CompareTo(Convert.ToInt32(0)) != 0))
                        return Convert.ToInt32(myDivident) / Convert.ToInt32(myDivisor);
                }
                else if (myType == typeof(UInt32))
                {
                    if ((Convert.ToUInt32(myDivident).CompareTo(Convert.ToUInt32(0)) != 0) && (Convert.ToUInt32(myDivisor).CompareTo(Convert.ToUInt32(0)) != 0))
                        return Convert.ToUInt32(myDivident) / Convert.ToUInt32(myDivisor);
                }
                else if (myType == typeof(Int64))
                {
                    if ((Convert.ToInt64(myDivident).CompareTo(0) != 0) && (Convert.ToInt64(myDivisor).CompareTo(0) != 0))
                        return Convert.ToInt64(myDivident) / Convert.ToInt64(myDivisor);
                }
                else if (myType == typeof(UInt64))
                {
                    if ((Convert.ToUInt64(myDivident).CompareTo(Convert.ToUInt64(0)) != 0) && (Convert.ToUInt64(myDivisor).CompareTo(Convert.ToUInt64(0)) != 0))
                        return Convert.ToUInt64(myDivident) / Convert.ToUInt64(myDivisor);
                }
                else if (myType == typeof(Double))
                {
                    if ((Convert.ToDouble(myDivident).CompareTo(Convert.ToDouble(0)) != 0) && (Convert.ToDouble(myDivisor).CompareTo(Convert.ToDouble(0)) != 0))
                        return Convert.ToDouble(myDivident) / Convert.ToDouble(myDivisor);
                }
                else if (myType == typeof(Single))
                {
                    if ((Convert.ToSingle(myDivident).CompareTo(Convert.ToSingle(0)) != 0) && (Convert.ToSingle(myDivisor).CompareTo(Convert.ToSingle(0)) != 0))
                        return Convert.ToSingle(myDivident) / Convert.ToSingle(myDivisor);
                }
                else
                {
                    throw new ArithmeticException();
                }
                #endregion

            }

            return null;
        }

    }
}
