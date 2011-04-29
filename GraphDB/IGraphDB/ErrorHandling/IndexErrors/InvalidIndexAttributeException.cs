using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class InvalidIndexAttributeException : AGraphDBIndexException
    {
        public String InvalidAttribute { get; private set; }
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidIndexAttributeException exception
        /// </summary>
        /// <param name="myInvalidIndexAttribute">The name of the invalid vertex type</param>
        /// <param name="myInfo"></param>
        public InvalidIndexAttributeException(String myInvalidIndexAttribute, String myInfo)
            : base()
        {
            Info = myInfo;
            InvalidAttribute = myInvalidIndexAttribute;

            _msg = String.Format("The index definition is invalid, attribute {0} is not valid. \n\n{1}.", InvalidAttribute, Info);
        }
    }
}
