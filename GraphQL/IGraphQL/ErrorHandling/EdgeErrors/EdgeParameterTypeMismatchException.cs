using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The type of the edge parameter does not match
    /// </summary>
    public sealed class EdgeParameterTypeMismatchException : AGraphQLEdgeException
    {
        public String CurrentType { get; private set; }
        public String[] ExpectedTypes { get; private set; }

        /// <summary>
        /// Creates a new EdgeParameterTypeMismatchException exception
        /// </summary>
        /// <param name="currentType"> The current edge parameter type</param>
        /// <param name="expectedTypes">A list of expected types</param>
        public EdgeParameterTypeMismatchException(String currentType, params String[] expectedTypes)
        {
            CurrentType = currentType;
            ExpectedTypes = expectedTypes;
            _msg = String.Format("The type [{0}] is not valid. Please use one of [{1}].", CurrentType, ExpectedTypes);
        }
        
    }
}
