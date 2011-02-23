using sones.ErrorHandling;
using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown exception
    /// </summary>
    public sealed class UnknownException : AGraphDBException
    {

        public Exception ThrownException { get; private set; }

        #region constructor

        public UnknownException(Exception e)
        {
            ThrownException = e;
        }

        #endregion

        public override ushort ErrorCode
        {
            get { return ErrorCodes.Unknown; }
        }
    }
}