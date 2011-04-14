using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The number of parameters of an IncomingEdge does not match
    /// </summary>
    public sealed class EdgeParameterCountMismatchException : AGraphQLEdgeException
    {
        public String Edge { get; private set; }
        public UInt32 CurrentNumOfParams { get; private set; }
        public UInt32 ExpectedNumOfParams { get; private set; }

        /// <summary>
        /// Creates a new EdgeParameterCountMismatchException exception
        /// </summary>
        /// <param name="IncomingEdge">The IncomingEdge</param>
        /// <param name="currentNumOfParams">The current count of parameters</param>
        /// <param name="expectedNumOfParams">The expected count of parameters</param>
        public EdgeParameterCountMismatchException(String edge, UInt32 currentNumOfParams, UInt32 expectedNumOfParams)
        {
            Edge = edge;
            CurrentNumOfParams = currentNumOfParams;
            ExpectedNumOfParams = expectedNumOfParams;
            _msg = String.Format("The edge [{0}] expects [{1}] params but found [{2}].", Edge, ExpectedNumOfParams, CurrentNumOfParams);
        }
        
    }
    
}
