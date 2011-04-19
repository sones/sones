using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The attribute for the index does not exist
    /// </summary>
    public sealed class IndexAttributeDoesNotExistException : AGraphDBIndexException
    {
        public String IndexAttributeName { get; private set; }

        /// <summary>
        /// Creates a new IndexAttributeDoesNotExistException exception
        /// </summary>
        /// <param name="myIndexAttributeName">The name of the  given index attribute</param>
        public IndexAttributeDoesNotExistException(String myIndexAttributeName)
        {
            IndexAttributeName = myIndexAttributeName;
            _msg = String.Format("The attribute \"{0}\" for the index does not exist!", IndexAttributeName);
        }
    }
}
