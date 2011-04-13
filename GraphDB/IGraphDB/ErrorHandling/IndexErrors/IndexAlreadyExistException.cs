using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The index already exists
    /// </summary>
    public sealed class IndexAlreadyExistException : AGraphDBIndexException
    {
        public String Index { get; private set; }

        /// <summary>
        /// Creates a new IndexAlreadyExistException exception
        /// </summary>
        /// <param name="myIndex">The name of the index</param>
        public IndexAlreadyExistException(String myIndex)
        {
            Index = myIndex;
            _msg = String.Format("The index \"{0}\" already exists!", Index);
        }               
    }
}
