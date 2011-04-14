using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// An Unique constraint violation on an index of a type has occurred
    /// </summary>
    public sealed class IndexUniqueConstrainViolationException : AGraphDBIndexException
    {
        public String TypeName { get; private set; }
        public String IndexName { get; private set; }

        /// <summary>
        /// Creates a new UniqueConstrainViolationException exception
        /// </summary>
        /// <param name="myTypeName">The name of the given type</param>
        /// <param name="myIndexName">The name of the given index</param>
        public IndexUniqueConstrainViolationException(String myTypeName, String myIndexName)
        {
            TypeName = myTypeName;
            IndexName = myIndexName;
            _msg = String.Format("Unique constraint violation on index {0} of type {1}", IndexName, TypeName);
        }
    }
}

