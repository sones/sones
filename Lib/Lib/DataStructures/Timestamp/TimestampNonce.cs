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

/*
 * TimestampNonce
 * (c) Achim Friedland, 2009
 */

#region Usings

using System;

#endregion

namespace sones.Lib.DataStructures.Timestamp
{

    /// <summary>
    /// This static datastructure will return the actual timestamp,
    /// but will also ensure that the return value is unique.
    /// </summary>

    public static class TimestampNonce
    {

        #region Data

        private static UInt64 LastTimestamp = TimestampNonce.Ticks;

        #endregion


        #region Properties

        #region Now

        public static DateTime Now
        {
            get
            {
                return new DateTime((Int64)GetNonce);
            }
        }

        #endregion

        #region Ticks

        public static UInt64 Ticks
        {
            get
            {
                return GetNonce;
            }
        }

        #endregion

        #region (private) GetNonce

        private static UInt64 GetNonce
        {
            get
            {

                var ActualTimestamp = (UInt64) DateTime.Now.Ticks;
                var LockObject      = new Object();

                lock (LockObject)
                {
                    // A while-loop just be be reaaaally sure ;)
                    while (ActualTimestamp <= LastTimestamp)
                    {
                        ++LastTimestamp;
                        return LastTimestamp;
                    }
                }

                LastTimestamp = ActualTimestamp;

                return ActualTimestamp;

            }
        }

        #endregion

        #endregion


        //#region GetHashCode()

        //public override Int32 GetHashCode()
        //{
        //    return LastTimestamp.GetHashCode();
        //}

        //#endregion

        #region AsString()

        public static String AsString()
        {
            return new DateTime((Int64)GetNonce).ToString();
        }

        #endregion

        #region AsString(myFormat)

        public static String AsString(String myFormat)
        {
            return new DateTime((Int64)GetNonce).ToString(myFormat);
        }

        #endregion

    }

}
