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
        TypeName         = Int64.MinValue + 7,

        #endregion

        #region BaseType, Attribute, Index

        ID            = Int64.MinValue + 10,
        Name          = Int64.MinValue + 11,
        IsUserDefined = Int64.MinValue + 12,

        #endregion

        #region BaseType

        IsAbstract = Int64.MinValue + 20,
        IsSealed   = Int64.MinValue + 21,
        Attributes = Int64.MinValue + 22,
        Behaviour  = Int64.MinValue + 23,

        #endregion

        #region VertexType, EdgeType

        Parent   = Int64.MinValue + 30,
        Children = Int64.MinValue + 31,

        #endregion

        #region VertexType

        UniquenessDefinitions = Int64.MinValue + 40,
        Indices               = Int64.MinValue + 41,

        #endregion

        #region Attribute

        Type         = Int64.MinValue + 50,
        DefiningType = Int64.MinValue + 51,

        #endregion

        #region IncomingEdge, Property

        Multiplicity = Int64.MinValue + 60,

        #endregion

        #region OutgoingEdge

        EdgeType             = Int64.MinValue + 70,
        Source               = Int64.MinValue + 71,
        Target               = Int64.MinValue + 72,
        RelatedIncomingEdges = Int64.MinValue + 73,

        #endregion

        #region IncomingEdge

        RelatedEgde = Int64.MinValue + 80,

        #endregion

        #region Property

        IsMandatory = Int64.MinValue + 90,
        InIndices   = Int64.MinValue + 91,

        #endregion

        #region Index

        IndexedProperties  = Int64.MinValue + 100,
        DefiningVertexType = Int64.MinValue + 101,
        IndexClass         = Int64.MinValue + 102,
        IsSingleValue      = Int64.MinValue + 103,
        IsRange            = Int64.MinValue + 104,
        IsVersioned        = Int64.MinValue + 105,

        #endregion

        #region WeightedEdge

        Weight = Int64.MinValue + 110,

        #endregion
    }

}
