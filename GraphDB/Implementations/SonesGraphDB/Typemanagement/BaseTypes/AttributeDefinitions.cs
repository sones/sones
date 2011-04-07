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
        /// Stores the offset for attribute ids for types that are base types (BaseType, Attribute, Index)
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

        /// <summary>
        /// Stores the offset for attribute ids for types that inherits from BaseType
        /// </summary>
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

        internal static readonly IOutgoingEdgeDefinition ParentOnVertexType = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset,
            EdgeType = NormalEdgeType.Instance,
            Name = "Parent",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.VertexType),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.VertexType)
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
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.VertexType),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.Index)
        };

        internal static readonly IIncomingEdgeDefinition IndicesOnVertexType = new IncomingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset + 3, 
            Name = "Indices", 
            RelatedEdgeDefinition = DefiningVertexTypeOnIndex
        };

        #endregion

        #region EdgeType

        internal static readonly IOutgoingEdgeDefinition ParentOnEdgeType = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset,
            EdgeType = NormalEdgeType.Instance,
            Name = "Parent",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.EdgeType),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.EdgeType)
        };

        internal static readonly IIncomingEdgeDefinition ChildrenOnEdgeType = new IncomingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + BaseTypeOffset + 1, 
            Name = "Children", 
            RelatedEdgeDefinition = ParentOnEdgeType
        };

        #endregion

        #region Attribute

        /// <summary>
        /// Stores the offset for attribute ids for types that inherits from Attribute
        /// </summary>
        private const Int64 AttributeOffset = AllTypesOffset + 2;

        internal static readonly IOutgoingEdgeDefinition TypeOnAttribute = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset ,
            EdgeType = NormalEdgeType.Instance,
            Name = "Type",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.Attribute),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.BaseType)
        };

        internal static readonly IOutgoingEdgeDefinition DefiningTypeOnAttribute = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 1,
            EdgeType = NormalEdgeType.Instance,
            Name = "DefiningType",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.Attribute),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.BaseType)
        };

        #endregion

        #region OutgoingEdge

        internal static readonly IOutgoingEdgeDefinition EdgeTypeOnOutgoingEdge = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AttributeOffset,
            EdgeType = NormalEdgeType.Instance,
            Name = "EdgeType",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.OutgoingEdge),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.EdgeType)
        };

        internal static readonly IOutgoingEdgeDefinition SourceOnOutgoingEdge = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AttributeOffset + 1,
            EdgeType = NormalEdgeType.Instance,
            Name = "Source",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.OutgoingEdge),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.VertexType)
        };

        internal static readonly IOutgoingEdgeDefinition TargetOnOutgoingEdge = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AttributeOffset + 2,
            EdgeType = NormalEdgeType.Instance,
            Name = "Target",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.OutgoingEdge),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.VertexType)
        };

        internal static readonly IIncomingEdgeDefinition RelatedIncomingEdgesOnOutgoingEdge = new IncomingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AttributeOffset + 3,
            Name = "RelatedIncomingEdges",
            RelatedEdgeDefinition = RelatedEdgeOnIncomingEdge
        };

        #endregion 
     
        #region IncomingEdge

        internal static readonly IOutgoingEdgeDefinition RelatedEdgeOnIncomingEdge = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AttributeOffset,
            EdgeType = NormalEdgeType.Instance, 
            Name = "RelatedEgde",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.IncomingEdge),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.OutgoingEdge),
        
        };

        #endregion

        #region Property

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

        internal static readonly IOutgoingEdgeDefinition IndexedPropertiesOnIndex = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset,
            EdgeType = NormalEdgeType.Instance,
            Name = "IndexedProperties",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.Index),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.Property)
        };

        internal static readonly IOutgoingEdgeDefinition DefiningVertexTypeOnIndex = new OutgoingEdgeDefinition()
        {
            AttributeID = Int64.MinValue + AllTypesOffset + 1,
            EdgeType = NormalEdgeType.Instance,
            Name = "DefiningVertexType",
            SourceVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.Index),
            TargetVertexType = BaseVertexTypeFactory.GetInstance(BaseVertexType.VertexType)
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
