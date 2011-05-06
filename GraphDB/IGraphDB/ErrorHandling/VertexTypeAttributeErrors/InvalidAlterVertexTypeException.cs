using System;

namespace sones.GraphDB.ErrorHandling
{

    public sealed class InvalidAlterVertexTypeException : AGraphDBVertexAttributeException
    {
        public InvalidAlterVertexTypeException(String myInfo)
        {
            _msg = myInfo;
        }

    }
}
