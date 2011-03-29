using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sones.Library.LanguageExtensions
{
    public static class StreamExtension
    {
        #region Stream

        public static UInt64 ULength(this Stream myStream)
        {

            var _ReturnValue = myStream.Length;

            return (_ReturnValue >= 0) ? (UInt64)_ReturnValue : 0;

        }

        #endregion
    }

}
