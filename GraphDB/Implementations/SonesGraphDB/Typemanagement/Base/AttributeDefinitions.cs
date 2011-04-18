using System;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.Base
{

    /// <summary>
    /// This static class contains all attribute definitions.
    /// </summary>
    /// We use a summary file, that contains attribute definitions, because some attributes are used in multiple vertex types
    /// and we want to be sure, that the AttributeIDs are similar for VertexTypes.
    internal static class AttributeDefinitions
    {
        public struct Info
        {
            public long ID;
            public String Name;
        }

        #region Vertex

        /// <summary>
        /// Stores the offset for attribute ids for types that inherits from Vertex
        /// </summary>
        private const Int64 VertexOffset = 6;

        public static readonly Info IDOnVertex             = new Info { ID = Int64.MinValue    , Name = "UUID" };
        public static readonly Info CreationOnVertex       = new Info { ID = Int64.MinValue + 1, Name = "CreationDate" };
        public static readonly Info ModifificationOnVertex = new Info { ID = Int64.MinValue + 2, Name = "ModificationDate" };
        public static readonly Info RevisionOnVertex       = new Info { ID = Int64.MinValue + 3, Name = "Revision" };
        public static readonly Info EditionOnVertex        = new Info { ID = Int64.MinValue + 4, Name = "Edition" };
        public static readonly Info Comment = new Info { ID = Int64.MinValue + VertexOffset + 3, Name = "Comment" };

        #endregion

        #region all types

        /// <summary>
        /// Stores the offset for attribute ids for types that are base types (BaseType, Attribute, Index)
        /// </summary>
        private const Int64 AllTypesOffset = VertexOffset + 3;

        public static readonly Info ID            = new Info { ID = Int64.MinValue + VertexOffset    , Name = "ID"};
        public static readonly Info Name          = new Info { ID = Int64.MinValue + VertexOffset + 1, Name = "Name"};
        public static readonly Info IsUserDefined = new Info { ID = Int64.MinValue + VertexOffset + 2, Name = "IsUserDefined"};

        #endregion

        #region BaseType

        /// <summary>
        /// Stores the offset for attribute ids for types that inherits from BaseType
        /// </summary>
        private const Int64 BaseTypeOffset = AllTypesOffset + 4;

        public static readonly Info IsAbstractOnBaseType = new Info { ID = Int64.MinValue + AllTypesOffset    , Name = "IsAbstract"};
        public static readonly Info IsSealedOnBaseType   = new Info { ID = Int64.MinValue + AllTypesOffset + 1, Name = "IsSealed"};
        public static readonly Info AttributesOnBaseType = new Info { ID = Int64.MinValue + AllTypesOffset + 2, Name = "Attributes"};
        public static readonly Info BehaviourOnBaseType  = new Info { ID = Int64.MinValue + AllTypesOffset + 3, Name = "Behaviour"};

        #endregion

        #region VertexType

        public static readonly Info ParentOnVertexType     = new Info { ID = Int64.MinValue + BaseTypeOffset    , Name = "Parent"};
        public static readonly Info ChildrenOnVertexType   = new Info { ID = Int64.MinValue + BaseTypeOffset + 1, Name = "Children"};
        public static readonly Info UniquenessOnVertexType = new Info { ID = Int64.MinValue + BaseTypeOffset + 2, Name = "UniquenessDefinitions"};
        public static readonly Info IndicesOnVertexType    = new Info { ID = Int64.MinValue + BaseTypeOffset + 3, Name = "Indices"};

        #endregion

        #region EdgeType

        public static readonly Info ParentOnEdgeType   = new Info { ID = Int64.MinValue + BaseTypeOffset    , Name = "Parent"};
        public static readonly Info ChildrenOnEdgeType = new Info { ID = Int64.MinValue + BaseTypeOffset + 1, Name = "Children"};

        #endregion

        #region Attribute

        /// <summary>
        /// Stores the offset for attribute ids for types that inherits from Attribute
        /// </summary>
        private const Int64 AttributeOffset = AllTypesOffset + 2;

        public static readonly Info TypeOnAttribute         = new Info { ID = Int64.MinValue + AllTypesOffset    , Name = "Type"};
        public static readonly Info DefiningTypeOnAttribute = new Info { ID = Int64.MinValue + AllTypesOffset + 1, Name = "DefiningType"};

        #endregion

        #region OutgoingEdge

        public static readonly Info EdgeTypeOnOutgoingEdge             = new Info { ID = Int64.MinValue + AttributeOffset    , Name = "EdgeType"};
        public static readonly Info SourceOnOutgoingEdge               = new Info { ID = Int64.MinValue + AttributeOffset + 1, Name = "Source"};
        public static readonly Info TargetOnOutgoingEdge               = new Info { ID = Int64.MinValue + AttributeOffset + 2, Name = "Target"};
        public static readonly Info RelatedIncomingEdgesOnOutgoingEdge = new Info { ID = Int64.MinValue + AttributeOffset + 3, Name = "RelatedIncomingEdges"};

        #endregion 
     
        #region IncomingEdge

        public static readonly Info RelatedEdgeOnIncomingEdge = new Info { ID = Int64.MinValue + AttributeOffset, Name = "RelatedEgde"};

        #endregion

        #region Property

        public static readonly Info IsMandatoryOnProperty = new Info { ID = Int64.MinValue + AttributeOffset    , Name = "IsMandatory"};
        public static readonly Info InIndicesOnProperty   = new Info { ID = Int64.MinValue + AttributeOffset + 1, Name = "InIndices"};

        #endregion

        #region Index

        public static readonly Info IndexedPropertiesOnIndex      = new Info { ID = Int64.MinValue + AllTypesOffset    , Name = "IndexedProperties"};
        public static readonly Info DefiningVertexTypeOnIndex     = new Info { ID = Int64.MinValue + AllTypesOffset + 1, Name = "DefiningVertexType"};
        public static readonly Info TypeOnIndex                   = new Info { ID = Int64.MinValue + AllTypesOffset + 2, Name = "Type"};
        public static readonly Info IsSingleOnIndex               = new Info { ID = Int64.MinValue + AllTypesOffset + 3, Name = "IsSingleValueIndex"};
        public static readonly Info IsRangeOnIndex                = new Info { ID = Int64.MinValue + AllTypesOffset + 4, Name = "IsRangeIndex"};
        public static readonly Info IsVersionedOnIndex            = new Info { ID = Int64.MinValue + AllTypesOffset + 5, Name = "IsVersionedIndex"};

        #endregion

    }

}
