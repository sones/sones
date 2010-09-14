using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib
{
    public class Exceptions
    {


        #region GraphFSException Superclass

        public class FastSerializeException : ApplicationException
        {
            public FastSerializeException(String message)
                : base(message)
            {
                // do nothing extra
            }
        }

        #endregion

        #region FastSerializeTypeSurrogate

        public class FastSerializeSurrogateTypeCodeExistException : FastSerializeException
        {
            public FastSerializeSurrogateTypeCodeExistException(String message)
                : base(message)
            {
                // do nothing extra
            }
        }

        #endregion
    }
}
