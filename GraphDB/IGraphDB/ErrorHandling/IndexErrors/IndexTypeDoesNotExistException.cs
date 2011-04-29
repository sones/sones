using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The index type does not exists
    /// </summary>
    public sealed class IndexTypeDoesNotExistException : AGraphDBIndexException
    {
        public String IndexName { get; private set; }
        public String TypeName { get; private set; }

        /// <summary>
        /// Creates a new IndexTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myIndexTypeName"></param>
        public IndexTypeDoesNotExistException(String myType, String myIndexName)
        {
            IndexName = myIndexName;
            TypeName = myType;

            _msg = String.Format("The index named \"{0}\" does not exist on type \"{1}\"!", IndexName, TypeName);
        }
      
    }
}
