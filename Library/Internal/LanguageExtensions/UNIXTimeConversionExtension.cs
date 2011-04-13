using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.LanguageExtensions
{
   public static class UNIXTimeConversionExtension
    {
        #region UNIXTime conversion

        private static DateTime _UNIXEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
    
        public static Int64 ToUnixTimeStamp(this DateTime myDateTime)  
        {
            return myDateTime.Subtract(_UNIXEpoch).Ticks;
        }  
  
        public static DateTime FromUnixTimeStamp(this Int64 myTimestamp)  
        {
            return _UNIXEpoch.AddTicks(myTimestamp);
        }

        #endregion
    }
}
