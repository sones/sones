using System;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.Base
{
    internal enum AttributeDefinitions: long
    {
        #region Vertex

        UUID             = Int64.MinValue,
        CreationDate     = Int64.MinValue + 1,
        ModificationDate = Int64.MinValue + 2,
        Revision         = Int64.MinValue + 3,
        Edition          = Int64.MinValue + 4,
        Comment          = Int64.MinValue + 5,
        TypeID           = Int64.MinValue + 6,

        #endregion

        #region BaseType, Attribute, Index

        ID            = Int64.MinValue + 7,
        Name          = Int64.MinValue + 8,
        IsUserDefined = Int64.MinValue + 9,

        #endregion

        #region BaseType

        IsAbstract = Int64.MinValue + 10,
        IsSealed   = Int64.MinValue + 11,
        Attributes = Int64.MinValue + 12,
        Behaviour  = Int64.MinValue + 13,

        #endregion

        #region VertexType, EdgeType

        Parent   = Int64.MinValue + 14,
        Children = Int64.MinValue + 15,

        #endregion

        #region VertexType

        UniquenessDefinitions = Int64.MinValue + 16,
        Indices               = Int64.MinValue + 17,

        #endregion

        #region Attribute

        Type         = Int64.MinValue + 18,
        DefiningType = Int64.MinValue + 19,

        #endregion

        #region OutgoingEdge

        EdgeType             = Int64.MinValue + 20,
        Source               = Int64.MinValue + 21,
        Target               = Int64.MinValue + 22,
        RelatedIncomingEdges = Int64.MinValue + 23,

        #endregion

        #region IncomingEdge

        RelatedEgde = Int64.MinValue + 24,

        #endregion

        #region Property

        IsMandatory = Int64.MinValue + 25,
        InIndices   = Int64.MinValue + 26,

        #endregion

        #region Index

        IndexedProperties  = Int64.MinValue + 27,
        DefiningVertexType = Int64.MinValue + 28,
        IndexClass         = Int64.MinValue + 29,
        IsSingleValue      = Int64.MinValue + 30,
        IsRange            = Int64.MinValue + 31,
        IsVersioned        = Int64.MinValue + 32,

        #endregion

        #region WeightedEdge

        Weight = Int64.MinValue + 33,

        #endregion
    }

}
