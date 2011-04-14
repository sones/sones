using System;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    /// <summary>
    /// This static class contains all attribute definitions.
    /// </summary>
    /// We use a summary file, that contains attribute definitions, because some attributes are used in multiple parentVertex types
    /// and we want to be sure, that the AttributeIDs are similar for VertexTypes
    /// 
    internal static class AttributeDefinitions
    {
        #region Vertex

        /// <summary>
        /// Stores the offset for attribute ids for types that inherits from Vertex
        /// </summary>
        private const Int64 VertexOffset = 5;

        internal static readonly Tuple<long, String> IDOnVertex             = Tuple.Create(Int64.MinValue    , "UUID");
        internal static readonly Tuple<long, String> CreationOnVertex       = Tuple.Create(Int64.MinValue + 1, "CreationDate");
        internal static readonly Tuple<long, String> ModifificationOnVertex = Tuple.Create(Int64.MinValue + 2, "ModificationDate");
        internal static readonly Tuple<long, String> RevisionOnVertex       = Tuple.Create(Int64.MinValue + 3, "Revision");
        internal static readonly Tuple<long, String> EditionOnVertex        = Tuple.Create(Int64.MinValue + 4, "Edition");

        #endregion

        #region all types

        /// <summary>
        /// Stores the offset for attribute ids for types that are base types (BaseType, Attribute, Index)
        /// </summary>
        private const Int64 AllTypesOffset = VertexOffset + 4;

        internal static readonly Tuple<long, String> ID            = Tuple.Create(Int64.MinValue + VertexOffset    , "ID");
        internal static readonly Tuple<long, String> Name          = Tuple.Create(Int64.MinValue + VertexOffset + 1, "Name");
        internal static readonly Tuple<long, String> IsUserDefined = Tuple.Create(Int64.MinValue + VertexOffset + 2, "IsUserDefined");
        internal static readonly Tuple<long, String> Comment       = Tuple.Create(Int64.MinValue + VertexOffset + 3, "Comment");

        #endregion

        #region BaseType

        /// <summary>
        /// Stores the offset for attribute ids for types that inherits from BaseType
        /// </summary>
        private const Int64 BaseTypeOffset = AllTypesOffset + 4;

        internal static readonly Tuple<long, String> IsAbstractOnBaseType = Tuple.Create(Int64.MinValue + AllTypesOffset, "IsAbstract");
        internal static readonly Tuple<long, String> IsSealedOnBaseType   = Tuple.Create(Int64.MinValue + AllTypesOffset + 1, "IsSealed");
        internal static readonly Tuple<long, String> AttributesOnBaseType = Tuple.Create(Int64.MinValue + AllTypesOffset + 2, "Attributes");
        internal static readonly Tuple<long, String> BehaviourOnBaseType  = Tuple.Create(Int64.MinValue + AllTypesOffset + 3, "Behaviour");

        #endregion

        #region VertexType

        internal static readonly Tuple<long, String> ParentOnVertexType     = Tuple.Create(Int64.MinValue + BaseTypeOffset, "Parent");
        internal static readonly Tuple<long, String> ChildrenOnVertexType   = Tuple.Create(Int64.MinValue + BaseTypeOffset + 1, "Children");
        internal static readonly Tuple<long, String> UniquenessOnVertexType = Tuple.Create(Int64.MinValue + BaseTypeOffset + 2, "UniquenessDefinitions");
        internal static readonly Tuple<long, String> IndicesOnVertexType    = Tuple.Create(Int64.MinValue + BaseTypeOffset + 3, "Indices");

        #endregion

        #region EdgeType

        internal static readonly Tuple<long, String> ParentOnEdgeType   = Tuple.Create(Int64.MinValue + BaseTypeOffset, "Parent");
        internal static readonly Tuple<long, String> ChildrenOnEdgeType = Tuple.Create(Int64.MinValue + BaseTypeOffset + 1, "Children");

        #endregion

        #region Attribute

        /// <summary>
        /// Stores the offset for attribute ids for types that inherits from Attribute
        /// </summary>
        private const Int64 AttributeOffset = AllTypesOffset + 2;

        internal static readonly Tuple<long, String> TypeOnAttribute         = Tuple.Create(Int64.MinValue + AllTypesOffset, "Type");
        internal static readonly Tuple<long, String> DefiningTypeOnAttribute = Tuple.Create(Int64.MinValue + AllTypesOffset + 1, "DefiningType");

        #endregion

        #region OutgoingEdge

        internal static readonly Tuple<long, String> EdgeTypeOnOutgoingEdge             = Tuple.Create(Int64.MinValue + AttributeOffset, "EdgeType");
        internal static readonly Tuple<long, String> SourceOnOutgoingEdge               = Tuple.Create(Int64.MinValue + AttributeOffset + 1, "Source");
        internal static readonly Tuple<long, String> TargetOnOutgoingEdge               = Tuple.Create(Int64.MinValue + AttributeOffset + 2, "Target");
        internal static readonly Tuple<long, String> RelatedIncomingEdgesOnOutgoingEdge = Tuple.Create(Int64.MinValue + AttributeOffset + 3, "RelatedIncomingEdges");

        #endregion 
     
        #region IncomingEdge

        internal static readonly Tuple<long, String> RelatedEdgeOnIncomingEdge = Tuple.Create(Int64.MinValue + AttributeOffset, "RelatedEgde");

        #endregion

        #region Property

        internal static readonly Tuple<long, String> IsMandatoryOnProperty = Tuple.Create(Int64.MinValue + AttributeOffset, "IsMandatory");
        internal static readonly Tuple<long, String> InIndicesOnProperty   = Tuple.Create(Int64.MinValue + AttributeOffset + 1, "InIndices");

        #endregion

        #region Index

        internal static readonly Tuple<long, String> IndexedPropertiesOnIndex      = Tuple.Create(Int64.MinValue + AllTypesOffset, "IndexedProperties");
        internal static readonly Tuple<long, String> DefiningVertexTypeOnIndex     = Tuple.Create(Int64.MinValue + AllTypesOffset + 1, "DefiningVertexType");
        internal static readonly Tuple<long, String> TypeOnIndex                   = Tuple.Create(Int64.MinValue + AllTypesOffset + 2, "Type");
        internal static readonly Tuple<long, String> IsSingleOnIndex               = Tuple.Create(Int64.MinValue + AllTypesOffset + 3, "IsSingleValueIndex");
        internal static readonly Tuple<long, String> IsRangeOnIndex                = Tuple.Create(Int64.MinValue + AllTypesOffset + 4, "IsRangeIndex");
        internal static readonly Tuple<long, String> IsVersionedOnIndex            = Tuple.Create(Int64.MinValue + AllTypesOffset + 5, "IsVersionedIndex");

        #endregion

    }

}
