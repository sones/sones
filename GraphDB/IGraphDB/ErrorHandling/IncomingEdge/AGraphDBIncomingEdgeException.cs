using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    public abstract class AGraphDBIncomingEdgeException : AGraphDBException
    {
        public override ushort ErrorCode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
