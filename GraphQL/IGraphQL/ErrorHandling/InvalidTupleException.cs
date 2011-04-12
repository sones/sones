using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The tuple is not valid
    /// </summary>
    public sealed class InvalidTupleException : AGraphQLException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidTupleException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public InvalidTupleException(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return String.Format("The tuple is not valid: {0}", Info);
        }

    }
}
