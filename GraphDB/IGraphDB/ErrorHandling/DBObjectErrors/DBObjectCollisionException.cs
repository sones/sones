using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// A DBObject collision occurred
    /// </summary>
    public sealed class DBObjectCollisionException : AGraphDBObjectException
    {
        public String Object { get; private set; }

        /// <summary>
        /// Creates a new DBObjectCollisionException exception
        /// </summary>
        /// <param name="myOBject">The given DBObject</param>
        public DBObjectCollisionException(String myOBject)
        {
            Object = myOBject;
            _msg = String.Format("A DBObject collision occurred. The DBObject with attributes \"{0}\" has been inserted with a UUID that already exists!", Object);
        }     

    }
}

