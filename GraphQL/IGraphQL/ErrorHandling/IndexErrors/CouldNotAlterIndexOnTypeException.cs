using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// Could not alter the index on a type
    /// </summary>
    public sealed class CouldNotAlterIndexOnTypeException : AGraphQLIndexException
    {
        public String IndexType { private set; get; }

        /// <summary>
        /// Creates a new CouldNotAlterIndexOnTypeException exception
        /// </summary>
        /// <param name="myIndexType"></param>
        public CouldNotAlterIndexOnTypeException(String myIndexType)
        {
            IndexType = myIndexType;
            _msg = String.Format("Could not alter index on type \"{0}\".", IndexType);
        }      

    }

}
