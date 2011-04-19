using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The selection of a INSERTORUPDATE/INSERTORREPLACE statement returned more than one result
    /// </summary>
    public sealed class MultipleResultsException : AGraphQLSelectException
    {
        /// <summary>
        /// Creates a new MultipleResultsException exception
        /// </summary>
        public MultipleResultsException()
        {
            _msg = "The selection returned more than one result. This is not allowed for the INSERTORUPDATE/INSERTORREPLACE statement.";
        }        
    }
}

