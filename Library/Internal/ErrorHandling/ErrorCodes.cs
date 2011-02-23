using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.ErrorHandling
{
    /// <summary>
    /// Contains static ErrorCodes
    /// </summary>
    public static class ErrorCodes
    {
        #region IGraphFS (prefix: 1)

        public static UInt16 VertexDoesNotExist = 10;

        #endregion

        #region IGraphDB (prefix: 2)

        public static UInt16 Unknown = 20;

        #endregion

    }
}
