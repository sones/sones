/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;

namespace sones.GraphDB.TypeManagement.Base
{

    internal enum AttributeDefinitions : long
    {
        #region Vertex

        VertexDotVertexID = Int64.MinValue,
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
        IndexDotSourceIndex,


        #endregion

        #region WeightedEdge

        WeightedEdgeDotWeight = Int64.MinValue + 16 * 9,

        #endregion


        #region Orderable

        OrderableEdgeDotOrder = Int64.MinValue + 16 * 10,

        #endregion
    }

}
