using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    /// <summary>
    /// This static class contains all attribute definitions.
    /// </summary>
    /// We use a summary file, that contains attribute definitions, because some attributes are used in multiple vertex types
    /// and we want to be sure, that the AttributeIDs are similar for VertexTypes
    /// 
    internal static class AttributeDefinitions
    {
        #region Vertex

        /// <summary>
        /// Stores how many attributes are at least in vertex type Vertex
        /// </summary>
        private const Int64 VertexOffset = 5;

        internal static readonly IPropertyDefinition IDOnVertex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue, 
            BaseType = typeof(UInt64),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "UUID"
        };
        
        internal static readonly IPropertyDefinition CreationOnVertex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + 1 ,
            BaseType = typeof(DateTime),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "CreationDate"
        };

        internal static readonly IPropertyDefinition ModifificationOnVertex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + 2,
            BaseType = typeof(DateTime),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "ModificationDate"
        };
        
        internal static readonly IPropertyDefinition RevisionOnVertex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + 3, 
            BaseType = typeof(String),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "Revision"
        };

        internal static readonly IPropertyDefinition EditionOnVertex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + 4, 
            BaseType = typeof(String),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "Edition"
        };

        #endregion

        #region all types

        /// <summary>
        /// Stores how many attributes are atleast in all types
        /// </summary>
        private const Int64 AllTypesOffset = VertexOffset + 4;

        internal static readonly IPropertyDefinition ID = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + VertexOffset,
            BaseType = typeof(UInt64),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "ID"
        };

        internal static readonly IPropertyDefinition Name = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + VertexOffset + 1,
            BaseType = typeof(String),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "Name"
        };

        internal static readonly IPropertyDefinition IsUserDefined = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + VertexOffset + 2,
            BaseType = typeof(Boolean),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsUserDefined"
        };

        internal static readonly IPropertyDefinition Comment = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + VertexOffset + 3,
            BaseType = typeof(String),
            IsMandatory = false,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "Comment"
        };

        #endregion

        #region BaseType

        private const Int64 BaseTypeOffset = AllTypesOffset + 4;

        internal static readonly IPropertyDefinition IsAbstractOnBaseType = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset,
            BaseType = typeof(Boolean),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsAbstract"
        };

        internal static readonly IPropertyDefinition IsSealedOnBaseType = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 1,
            BaseType = typeof(Boolean),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsSealed"
        };

        internal static readonly IIncomingEdgeDefinition AttributesOnBaseType = new IncomingEdgeDefinition() 
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 2,
            Name = "Attributes", 
            RelatedEdgeDefinition = DefiningTypeOnAttribute
        };

        internal static readonly IPropertyDefinition BehaviourOnBaseType = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 3,
            BaseType = typeof(String), 
            IsMandatory = false, 
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "Behaviour"
        };

        #endregion

        #region VertexType

        private const Int64 VertexTypeOffset = BaseTypeOffset + 4;

        internal static readonly IOutgoingEdgeDefinition ParentOnVertexType = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset,
            EdgeType = NormalEdgeType.Instance,
            Name = "Parent",
            SourceVertexType = VertexTypeVertexType.Instance,
            TargetVertexType = VertexTypeVertexType.Instance
        };

        internal static readonly IIncomingEdgeDefinition ChildrenOnVertexType = new IncomingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset + 1,
            Name = "Children", 
            RelatedEdgeDefinition = ParentOnVertexType
        };

        internal static readonly IOutgoingEdgeDefinition UniquenessOnVertexType = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset + 2,
            EdgeType = NormalEdgeType.Instance,
            Name = "UniquenessDefinitions",
            SourceVertexType = VertexTypeVertexType.Instance,
            TargetVertexType = IndexVertexType.Instance
        };

        internal static readonly IIncomingEdgeDefinition IndicesOnVertexType = new IncomingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset + 3, 
            Name = "Indices", 
            RelatedEdgeDefinition = DefiningVertexTypeOnIndex
        };

        #endregion

        #region EdgeType

        private const Int64 EdgeTypeOffset = BaseTypeOffset + 2;

        internal static readonly IOutgoingEdgeDefinition ParentOnEdgeType = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset,
            EdgeType = NormalEdgeType.Instance,
            Name = "Parent",
            SourceVertexType = EdgeVertexType.Instance,
            TargetVertexType = EdgeVertexType.Instance
        };

        internal static readonly IIncomingEdgeDefinition ChildrenOnEdgeType = new IncomingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset + 1, 
            Name = "Children", 
            RelatedEdgeDefinition = ParentOnEdgeType
        };

        #endregion

        #region Attribute

        private const Int64 AttributeOffset = AllTypesOffset + 2;

        internal static readonly IOutgoingEdgeDefinition TypeOnAttribute = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset ,
            EdgeType = NormalEdgeType.Instance,
            Name = "Type",
            SourceVertexType = AttributeVertexType.Instance,
            TargetVertexType = BaseTypeVertexType.Instance
        };

        internal static readonly IOutgoingEdgeDefinition DefiningTypeOnAttribute = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 1,
            EdgeType = NormalEdgeType.Instance,
            Name = "DefiningType",
            SourceVertexType = AttributeVertexType.Instance,
            TargetVertexType = BaseTypeVertexType.Instance
        };

        #endregion

        #region Edge

        private const Int64 EdgeOffset = AttributeOffset + 1;

        internal static readonly IOutgoingEdgeDefinition EdgeTypeOnEdge = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AttributeOffset,
            EdgeType = NormalEdgeType.Instance,
            Name = "EdgeType",
            SourceVertexType = EdgeVertexType.Instance,
            TargetVertexType = EdgeTypeVertexType.Instance
        };

        #endregion

        #region Property

        private const Int64 PropertyOffset = AttributeOffset + 2;

        internal static readonly IPropertyDefinition IsMandatoryOnProperty = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + AttributeOffset,
            BaseType = typeof(Boolean),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsMandatory"
        };

        internal static readonly IIncomingEdgeDefinition InIndicesOnProperty = new IncomingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AttributeOffset + 1,
            Name = "InIndices",
            RelatedEdgeDefinition = IndexedPropertiesOnIndex
        };

        #endregion

        #region Index

        private const Int64 IndexOffset = AllTypesOffset + 6;

        internal static readonly IOutgoingEdgeDefinition IndexedPropertiesOnIndex = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset,
            EdgeType = NormalEdgeType.Instance,
            Name = "IndexedProperties",
            SourceVertexType = IndexVertexType.Instance,
            TargetVertexType = PropertyVertexType.Instance
        };

        internal static readonly IOutgoingEdgeDefinition DefiningVertexTypeOnIndex = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 1,
            EdgeType = NormalEdgeType.Instance,
            Name = "DefiningVertexType",
            SourceVertexType = IndexVertexType.Instance,
            TargetVertexType = VertexTypeVertexType.Instance
        };

        internal static readonly IPropertyDefinition TypeOnIndex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 2,
            BaseType = typeof(String),
            IsMandatory = true,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "Type"
        };

        internal static readonly IPropertyDefinition IsSingleOnIndex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 3,
            BaseType = typeof(Boolean),
            IsMandatory = false,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsSingleValueIndex"
        };

        internal static readonly IPropertyDefinition IsRangeOnIndex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 4,
            BaseType = typeof(Boolean),
            IsMandatory = false,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsRangeIndex"
        };

        internal static readonly IPropertyDefinition IsVersionedOnIndex = new PropertyDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 5,
            BaseType = typeof(Boolean),
            IsMandatory = false,
            Multiplicity = TypesOfMultiplicity.Single,
            Name = "IsVersionedIndex"
        };

        #endregion

    }

}
