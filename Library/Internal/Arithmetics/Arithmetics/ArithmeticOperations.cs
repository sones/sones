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

        public static IComparable Sub(Type myType, IComparable myMinuend, IComparable mySubtrahend)
        {
            throw new NotImplementedException();
        }

        public static IComparable Mul(Type myType, IComparable myLeft, IComparable myRight)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Divides the given operands, depending on the type
        /// </summary>
        /// <param name="myType">The type of the operand</param>
        /// <param name="myDivident">The Dividend of the operation</param>
        /// <param name="myDivisor">The Divisor of the operation</param>
        /// <exception cref="InvalidCastException">Is thrown if operation fails</exception>
        /// <returns>Result of the operation in case of valid type, else NULL or an Exception is thrown</returns>
        public static IComparable Div(Type myType, IComparable myDivident, IComparable myDivisor)
        {
            //make sure that either the dividend and the divisor are not NULL and not 0
            if ((myDivident != null) &&
                (myDivisor != null) &&
                (myDivident.CompareTo(0) != 0) &&
                (myDivisor.CompareTo(0) != 0))
            {

                #region check types and calculate


                if (myType == typeof(Int16))
                {
                    return Convert.ToInt16(myDivident) / Convert.ToInt16(myDivisor);
                }
                else if (myType == typeof(UInt16))
                {
                    return Convert.ToUInt16(myDivident) / Convert.ToUInt16(myDivisor);
                }
                else if (myType == typeof(Int32))
                {
                    return Convert.ToInt32(myDivident) / Convert.ToInt32(myDivisor);
                }
                else if (myType == typeof(UInt32))
                {
                    return Convert.ToUInt32(myDivident) / Convert.ToUInt32(myDivisor);
                }
                else if (myType == typeof(Int64))
                {
                    return Convert.ToInt64(myDivident) / Convert.ToInt64(myDivisor);
                }
                else if (myType == typeof(UInt64))
                {
                    return Convert.ToUInt64(myDivident) / Convert.ToUInt64(myDivisor);
                }
                else if (myType == typeof(Double))
                {
                    return Convert.ToDouble(myDivident) / Convert.ToDouble(myDivisor);
                }
                else if (myType == typeof(Single))
                {
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
