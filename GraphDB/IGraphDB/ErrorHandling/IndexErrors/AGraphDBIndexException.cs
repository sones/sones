using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    public abstract class AGraphDBIndexException : AGraphDBException
    {
        public override ushort ErrorCode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
