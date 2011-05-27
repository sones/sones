using System;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.Extensions
{
    public static class IOutgoingEdgeDefinitionExtension
    {
        public static bool HasValue(this IOutgoingEdgeDefinition myProperty, IVertex myVertex)
        {
            if (myProperty == null)
                throw new NullReferenceException();

            return myVertex.HasOutgoingEdge(myProperty.ID);
        }

        public static IEdge GetEdge(this IOutgoingEdgeDefinition myProperty, IVertex myVertex)
        {
            if (myProperty == null)
                throw new NullReferenceException();

            return myVertex.GetOutgoingEdge(myProperty.ID);
        }

        public static IHyperEdge GetHyperEdge(this IOutgoingEdgeDefinition myProperty, IVertex myVertex)
        {
            if (myProperty == null)
                throw new NullReferenceException();

            return myVertex.GetOutgoingHyperEdge(myProperty.ID);
        }

        public static ISingleEdge GetSingleEdge(this IOutgoingEdgeDefinition myProperty, IVertex myVertex)
        {
            if (myProperty == null)
                throw new NullReferenceException();

            return myVertex.GetOutgoingSingleEdge(myProperty.ID);
        }

    }
}
