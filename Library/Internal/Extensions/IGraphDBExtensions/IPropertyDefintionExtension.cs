using System;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;
using sones.Constants;

namespace sones.GraphDB.Extensions
{
    public static class IPropertyDefintionExtension
    {
        public static bool HasValue(this IPropertyDefinition myProperty, IEdge myEdge)
        {
            if (myProperty == null)
                throw new NullReferenceException();

            return myEdge.HasProperty(myProperty.ID);
        }

        public static bool HasValue(this IPropertyDefinition myProperty, IVertex myVertex)
        {
            if (myProperty == null)
                throw new NullReferenceException();

            if (myProperty.RelatedType == null)
                throw new ArgumentException("A property with nor related type is not allowed.");

            if (!myProperty.RelatedType.Name.Equals(GlobalConstants.Vertex))
                return myVertex.HasProperty(myProperty.ID);

            switch (myProperty.Name)
            {
                case GlobalConstants.VertexDotComment:
                    return myVertex.Comment != null;

                case GlobalConstants.VertexDotCreationDate:
                case GlobalConstants.VertexDotEdition:
                case GlobalConstants.VertexDotModificationDate:
                case GlobalConstants.VertexDotRevision:
                case GlobalConstants.VertexDotVertexTypeID:
                case GlobalConstants.VertexDotVertexTypeName:
                case GlobalConstants.VertexDotVertexID:
                    return true;

                default:
                    throw new Exception(
                        "A new property was added to the vertex type Vertex, but this switch stement was not changed.");

            }
        }


        public static IComparable GetValue(this IPropertyDefinition myProperty, IEdge myEdge)
        {
            if (myProperty == null)
                throw new NullReferenceException();

            return myEdge.GetProperty<IComparable>(myProperty.ID);
        }

        public static IComparable GetValue(this IPropertyDefinition myProperty, IVertex myVertex)
        {
            if (myProperty == null)
                throw new NullReferenceException();

            if (myProperty.RelatedType == null)
                throw new ArgumentException("A property with nor related type is not allowed.");

            if (!myProperty.RelatedType.Name.Equals(GlobalConstants.Vertex))
                return myVertex.GetProperty<IComparable>(myProperty.ID);

            switch (myProperty.Name)
            {
                case GlobalConstants.VertexDotComment:
                    return myVertex.Comment;

                case GlobalConstants.VertexDotCreationDate:
                    return myVertex.CreationDate;

                case GlobalConstants.VertexDotEdition:
                    return myVertex.EditionName;

                case GlobalConstants.VertexDotModificationDate:
                    return myVertex.ModificationDate;

                case GlobalConstants.VertexDotRevision:
                    return myVertex.VertexRevisionID;

                case GlobalConstants.VertexDotVertexTypeID:
                    return myVertex.VertexTypeID;

                case GlobalConstants.VertexDotVertexTypeName:
                    throw new NotImplementedException();

                case GlobalConstants.VertexDotVertexID:
                    return myVertex.VertexID;

                default:
                    throw new System.Exception(
                        "A new property was added to the vertex type Vertex, but this switch stement was not changed.");

            }
        }
    }
}
