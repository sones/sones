using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    public class AGraphDBVertexAttributeException : AGraphDBException
    {
        public override ushort ErrorCode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
