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
                try
                {
                    #region check types and calculate

                    if (myType == typeof(Int16))
                    {
                        return ((Int16)myLeft) + ((Int16)myRight);
                    }
                    else if (myType == typeof(UInt16))
                    {
                        return ((UInt16)myLeft) + ((UInt16)myRight);
                    }
                    else if (myType == typeof(Int32))
                    {
                        return ((Int32)myLeft) + ((Int32)myRight);
                    }
                    else if (myType == typeof(UInt32))
                    {
                        return ((UInt32)myLeft) + ((UInt32)myRight);
                    }
                    else if (myType == typeof(Int64))
                    {
                        return ((Int64)myLeft) + ((Int64)myRight);
                    }
                    else if (myType == typeof(UInt64))
                    {
                        return ((UInt64)myLeft) + ((UInt64)myRight);
                    }
                    else if (myType == typeof(Double))
                    {
                        return ((Double)myLeft) + ((Double)myRight);
                    }
                    else if (myType == typeof(Single))
                    {
                        return ((Single)myLeft) + ((Single)myRight);
                    }

                    #endregion
                }
                catch (Exception e)
                {
                    if (!(e is ASonesException))
                    {
                        throw new InvalidCastException("A cast to the given type " + myType.Name + " failed!");
                    }
                }
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
        /// <param name="myDividend">The Dividend of the operation</param>
        /// <param name="myDivisor">The Divisor of the operation</param>
        /// <exception cref="InvalidCastException">Is thrown if operation fails</exception>
        /// <returns>Result of the operation in case of valid type, else NULL or an Exception is thrown</returns>
        public static IComparable Div(Type myType, IComparable myDividend, IComparable myDivisor)
        {
            //make sure that either the dividend and the divisor are not NULL and not 0
            if ((myDividend != null) && 
                (myDivisor != null) && 
                (myDividend.CompareTo(0) != 0) && 
                (myDivisor.CompareTo(0) != 0))
            {
                try
                {
                    #region check types and calculate

                    if (myType == typeof(Int16))
                    {
                        return ((Int16)myDividend) / ((Int16)myDivisor);
                    }
                    else if (myType == typeof(UInt16))
                    {
                        return ((UInt16)myDividend) / ((UInt16)myDivisor);
                    }
                    else if (myType == typeof(Int32))
                    {
                        return ((Int32)myDividend) / ((Int32)myDivisor);
                    }
                    else if (myType == typeof(UInt32))
                    {
                        return ((UInt32)myDividend) / ((UInt32)myDivisor);
                    }
                    else if (myType == typeof(Int64))
                    {
                        return ((Int64)myDividend) / ((Int64)myDivisor);
                    }
                    else if (myType == typeof(UInt64))
                    {
                        return ((UInt64)myDividend) / ((UInt64)myDivisor);
                    }
                    else if (myType == typeof(Double))
                    {
                        return ((Double)myDividend) / ((Double)myDivisor);
                    }
                    else if (myType == typeof(Single))
                    {
                        return ((Single)myDividend) / ((Single)myDivisor);
                    }

                    #endregion
                }
                catch (Exception e)
                {
                    if (!(e is ASonesException))
                    {
                        throw new InvalidCastException("A cast to the given type " + myType.Name + " failed!");
                    }
                }
            }

            return null;
        }

    }
}
