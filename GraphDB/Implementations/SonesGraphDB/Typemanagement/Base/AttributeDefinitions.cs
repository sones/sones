using System;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.Base
{

    internal enum AttributeDefinitions : long
    {
        #region Vertex

        VertexDotUUID = Int64.MinValue,
        VertexDotCreationDate,
        VertexDotModificationDate,
        VertexDotRevision,
        VertexDotEdition,
        VertexDotComment,
        VertexDotTypeID,
        VertexDotTypeName,

        #endregion

        #region BaseType

        BaseTypeDotName = Int64.MinValue + 16,
        BaseTypeDotIsUserDefined,
        BaseTypeDotIsAbstract,
        BaseTypeDotIsSealed,
        BaseTypeDotAttributes,
        BaseTypeDotBehaviour,

        #endregion

        #region VertexType

        VertexTypeDotParent = Int64.MinValue + 16 * 2,
        VertexTypeDotChildren,
        VertexTypeDotUniquenessDefinitions,
        VertexTypeDotIndices,

        #endregion

        #region EdgeType

        EdgeTypeDotParent = Int64.MinValue + 16 * 3,
        EdgeTypeDotChildren,

        #endregion

        #region Attribute

        AttributeDotName = Int64.MinValue + 16 * 4,
        AttributeDotIsUserDefined,
        AttributeDotType,
        AttributeDotDefiningType,

        #endregion

        #region OutgoingEdge

        OutgoingEdgeDotEdgeType = Int64.MinValue + 16 * 5,
        OutgoingEdgeDotInnerEdgeType,
        OutgoingEdgeDotSource,
        OutgoingEdgeDotTarget,
        OutgoingEdgeDotRelatedIncomingEdges,
        OutgoingEdgeDotMultiplicity,

        #endregion

        #region IncomingEdge

        IncomingEdgeDotRelatedEgde = Int64.MinValue + 16 * 6,

        #endregion

        #region Property

        PropertyDotIsMandatory = Int64.MinValue + 16 * 7,
        PropertyDotInIndices,
        PropertyDotDefaultValue,
        PropertyDotMultiplicity,
        PropertyDotBaseType,


        #endregion

        #region Index

        IndexDotName = Int64.MinValue + 16 * 8,
        IndexDotIsUserDefined,
        IndexDotIndexedProperties,
        IndexDotDefiningVertexType,
        IndexDotIndexClass,
        IndexDotIsSingleValue,
        IndexDotIsRange,
        IndexDotIsVersioned,

        #endregion

        #region WeightedEdge

        WeightedEdgeDotWeight = Int64.MinValue + 16 * 9,

        #endregion


        #region Orderable

        OrderableEdgeDotOrder = Int64.MinValue + 16 * 10,

        #endregion
    }

}
