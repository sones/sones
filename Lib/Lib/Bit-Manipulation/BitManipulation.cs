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
