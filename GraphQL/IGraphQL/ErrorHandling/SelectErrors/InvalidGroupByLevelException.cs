using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// An invalid group level when adding a group element to a selection
    /// </summary>
    public sealed class InvalidGroupByLevelException : AGraphQLSelectException
    {
        public String IDChainDefinitionEdgesCount { get; private set; }
        public String IDChainDefinitionContentString { get; private set; }

        /// <summary>
        /// Creates a new InvalidGroupByLevelException exception
        /// </summary>
        /// <param name="myIDChainDefinitionEdgesCount">The count of edges of the IDChainDefinition</param>
        /// <param name="myIDChainDefinitionContentString"></param>
        public InvalidGroupByLevelException(String myIDChainDefinitionEdgesCount, String myIDChainDefinitionContentString)
        {
            IDChainDefinitionEdgesCount = myIDChainDefinitionEdgesCount;
            IDChainDefinitionContentString = myIDChainDefinitionContentString;
            _msg = String.Format("The level ({0}) greater than 1 is not allowed: '{1}'", IDChainDefinitionEdgesCount, IDChainDefinitionContentString);

        }

    }
}
