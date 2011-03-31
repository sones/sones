using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A duplicate attribute selection is not allowed
    /// </summary>
    public sealed class DuplicateAttributeSelectionException : AGraphQLSelectException
    {
        public String SelectionAlias { get; private set; }

        /// <summary>
        /// Creates a new DuplicateAttributeSelectionException exception
        /// </summary>
        /// <param name="mySelectionAlias">The alias to use</param>
        public DuplicateAttributeSelectionException(String mySelectionAlias)
        {
            SelectionAlias = mySelectionAlias;
        }

        public override string ToString()
        {
            return String.Format("You cannot select \"{0}\" more than one time. Try to use an alias.", SelectionAlias);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.DuplicateAttributeSelection; }
        }
    }
}
