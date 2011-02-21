using sones.ErrorHandling;
using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown exception
    /// </summary>
    public sealed class UnknownException : AGraphDBException
    {

        public Exception InnerException { get; private set; }

        #region constructor

        public UnknownException(Exception e)
        {
            InnerException = e;
        }

        #endregion

        public override ushort ErrorCode
        {
            get { return 0; }
        }
    }
}