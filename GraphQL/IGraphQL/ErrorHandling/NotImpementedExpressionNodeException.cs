using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// Currently the type has not been implemented for expressions
    /// </summary>
    public sealed class NotImpementedExpressionNodeException : AGraphQLException
    {
        public Type NodeType { get; private set; }
        
        /// <summary>
        /// Creates a new NotImpementedExpressionNodeException exception
        /// </summary>
        /// <param name="myNodeType">The current node type</param>
        public NotImpementedExpressionNodeException(Type myNodeType)
        {
            NodeType = myNodeType;
        }

        public override string ToString()
        {
            return String.Format("Currently the type {0} has not been implemented for expressions.", NodeType.Name);
        }

    }
}
