using System;
using sones.Library.ErrorHandling;

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
        }

        public override string ToString()
        {
            return String.Format("Could not alter index on type \"{0}\".", IndexType);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.CouldNotAlterIndexOnType; }
        } 

    }

}
