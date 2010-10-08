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
        private static Object LockObject    = new Object();

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
               // var LockObject      = new Object();

                lock (typeof(TimestampNonce))
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


        //public TimestampNonce()
        //{
        //    LockObject = new Object();
        //}

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
