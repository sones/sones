using System;
using System.Collections.Generic;
using System.Text;
using sones.Libraries.Exceptions;

namespace sones.Lib
{
    /// <summary>
    /// This implements a BitWise Manipulation Helper - you can set and get particular Bits of
    /// an Object.
    /// </summary>
    public static class BitHelper
    {
        #region IsBitTrue
        /// <summary>
        /// Checks a particular byte if a bit is set or not
        /// </summary>
        /// <param name="Input">the input byte that is checked</param>
        /// <param name="ObjectStreams">the position in the 8-Bit input byte</param>
        /// <returns>true if bit is 1 false if it is 0</returns>
        public static bool IsBitTrue(Byte Input, Byte Position)
        {
            if (Position <= 0)
                throw new BitHelperException("Position you gave is out of bounds. We start at 1 and end at 8.");
            if (Position > 8)
                throw new BitHelperException("Position you gave is out of bounds. We start at 1 and end at 8.");
            int nPwr = (int)(Position == 64 ? -2 : 2); // Determine what Power of 2 to use
             return (((long)Math.Pow(nPwr, Position - 1)) & Input) > 0;
         }
        #endregion

        #region BitSet
        /// <summary>
        /// Sets a particular bit in an input byte and returns the new Byte value
        /// </summary>
        /// <param name="Input">the Input Byte</param>
        /// <param name="ObjectStreams">on which position should we set, it starts with 1 and ends with 8</param>
        /// <param name="BitValue">set it to true or false, 1 or 0</param>
        /// <returns>the byte with the set</returns>
        public static byte BitSet(byte Input, Byte Position, bool BitValue) 
        {
            if (Position <= 0) 
                throw new BitHelperException("Position you gave is out of bounds. We start at 1 and end at 8.");
            if (Position > 8)
                throw new BitHelperException("Position you gave is out of bounds. We start at 1 and end at 8.");

            int nPwr = (int)(Position == 64 ? -2 : 2); // Determine what Power of 2 to use
            if (BitValue)
            {
                if ((IsBitTrue(Input, Position)) && (BitValue))
                    return Input;

                Input ^= (byte)(Math.Pow(nPwr, Position - 1));
            }
            else
            {
                if ((!IsBitTrue(Input, Position)) && (!BitValue))
                return Input;
                Input ^= (byte)(Math.Pow(nPwr, Position - 1));
            }
            return Input;
        }
        #endregion
    }
}
