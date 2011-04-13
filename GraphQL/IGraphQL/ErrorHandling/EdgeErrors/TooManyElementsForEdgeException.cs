using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// Too many elements for a type of an edge 
    /// </summary>
    public sealed class TooManyElementsForEdgeException : AGraphQLEdgeException
    {
        public UInt64 CurrentElements { get; private set; }
        public String EdgeTypeName { get; private set; }

        /// <summary>
        /// Creates a new TooManyElementsForEdgeException exception
        /// </summary>
        /// <param name="edgeTypeName">The name of the edge type</param>
        /// <param name="currentElements">The current count of elements</param>
        public TooManyElementsForEdgeException(String edgeTypeName, UInt64 currentElements)
        {
            CurrentElements = currentElements;
            EdgeTypeName = edgeTypeName;
            _msg = String.Format("The edge [{0}] does not take {1} elements.", EdgeTypeName, CurrentElements);
        }

    }
}
