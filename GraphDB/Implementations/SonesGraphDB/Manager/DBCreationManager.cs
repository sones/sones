using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.VertexStore;
using sones.Library.LanguageExtensions;
using sones.Library.Commons.VertexStore.Definitions;
using sones.GraphDB.TypeManagement.Base;
using System.Resources;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.BaseGraph;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A class that creates the base graph.
    /// </summary>
    internal class DBCreationManager
    {
        
        private readonly Dictionary<AttributeDefinitions, VertexInformation> _infos;

        private readonly SecurityToken _security;
        private readonly TransactionToken _transaction;

        #region Vertex information

        #region Attribute

        private static readonly byte AttributeOffset = 0;

        private readonly VertexInformation _Attribute                 = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.Attribute);
        private readonly VertexInformation _AttributeDotID            = new VertexInformation((long)BaseTypes.Property    , AttributeOffset + (long)AttributeDefinitions.ID);
        private readonly VertexInformation _AttributeDotIsUserDefined = new VertexInformation((long)BaseTypes.Property    , AttributeOffset + (long)AttributeDefinitions.IsUserDefined);
        private readonly VertexInformation _AttributeDotName          = new VertexInformation((long)BaseTypes.Property    , AttributeOffset + (long)AttributeDefinitions.Name);
        private readonly VertexInformation _AttributeDotDefiningType  = new VertexInformation((long)BaseTypes.OutgoingEdge, AttributeOffset + (long)AttributeDefinitions.DefiningType);

        #endregion

        #region BaseType

        private static readonly byte BaseTypeOffset = AttributeOffset + 4;

        private readonly VertexInformation _BaseType                 = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.BaseType);
        private readonly VertexInformation _BaseTypeDotID            = new VertexInformation((long)BaseTypes.Property    , BaseTypeOffset + (long)AttributeDefinitions.ID);
        private readonly VertexInformation _BaseTypeDotName          = new VertexInformation((long)BaseTypes.Property    , BaseTypeOffset + (long)AttributeDefinitions.Name);
        private readonly VertexInformation _BaseTypeDotIsUserDefined = new VertexInformation((long)BaseTypes.Property    , BaseTypeOffset + (long)AttributeDefinitions.IsUserDefined);
        private readonly VertexInformation _BaseTypeDotIsAbstract    = new VertexInformation((long)BaseTypes.Property    , BaseTypeOffset + (long)AttributeDefinitions.IsAbstract);
        private readonly VertexInformation _BaseTypeDotIsSealed      = new VertexInformation((long)BaseTypes.Property    , BaseTypeOffset + (long)AttributeDefinitions.IsSealed);
        private readonly VertexInformation _BaseTypeDotBehaviour     = new VertexInformation((long)BaseTypes.Property    , BaseTypeOffset + (long)AttributeDefinitions.Behaviour);
        private readonly VertexInformation _BaseTypeDotAttributes    = new VertexInformation((long)BaseTypes.IncomingEdge, BaseTypeOffset + (long)AttributeDefinitions.Attributes);

        #endregion

        #region Edge

        private static readonly byte EdgeOffset = BaseTypeOffset + 7;

        private readonly VertexInformation _Edge = new VertexInformation((long)BaseTypes.VertexType, EdgeOffset + (long)BaseTypes.Edge);

        #endregion

        #region EdgeType

        private static readonly byte EdgeTypeOffset = EdgeOffset;

        private readonly VertexInformation _EdgeType            = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.EdgeType);
        private readonly VertexInformation _EdgeTypeDotParent   = new VertexInformation((long)BaseTypes.OutgoingEdge, EdgeTypeOffset + (long)AttributeDefinitions.Parent);
        private readonly VertexInformation _EdgeTypeDotChildren = new VertexInformation((long)BaseTypes.IncomingEdge, EdgeTypeOffset + (long)AttributeDefinitions.Children);

        #endregion

        #region IncomingEdge

        private static readonly byte IncomingEdgeOffset = EdgeTypeOffset + 2;

        private readonly VertexInformation _IncomingEdge               = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.IncomingEdge);
        private readonly VertexInformation _IncomingEdgeDotRelatedEdge = new VertexInformation((long)BaseTypes.OutgoingEdge, IncomingEdgeOffset +(long)AttributeDefinitions.RelatedEgde);

        #endregion

        #region Index

        private static readonly byte IndexOffset = IncomingEdgeOffset + 1;

        private readonly VertexInformation _Index                      = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.Index);
        private readonly VertexInformation _IndexDotIndexedProperties  = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.IndexedProperties);
        private readonly VertexInformation _IndexDotDefiningVertexType = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.DefiningVertexType);
        private readonly VertexInformation _IndexDotID                 = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.ID);
        private readonly VertexInformation _IndexDotName               = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.Name);
        private readonly VertexInformation _IndexDotIsUserDefined      = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.IsUserDefined);
        private readonly VertexInformation _IndexDotIndexClass         = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.IndexClass);
        private readonly VertexInformation _IndexDotIsSingleValue      = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.IsSingleValue);
        private readonly VertexInformation _IndexDotIsRange            = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.IsRange);
        private readonly VertexInformation _IndexDotIsVersioned        = new VertexInformation((long)BaseTypes.Property  , IndexOffset + (long)AttributeDefinitions.IsVersioned);

        #endregion

        #region OutgoingEdge

        private static readonly byte OutgoingEdgeOffset = IndexOffset + 9;

        private readonly VertexInformation _OutgoingEdge                        = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.OutgoingEdge);
        private readonly VertexInformation _OutgoingEdgeDotRelatedIncomingEdges = new VertexInformation((long)BaseTypes.IncomingEdge, OutgoingEdgeOffset + (long)AttributeDefinitions.RelatedIncomingEdges);
        private readonly VertexInformation _OutgoingEdgeDotEdgeType             = new VertexInformation((long)BaseTypes.OutgoingEdge, OutgoingEdgeOffset + (long)AttributeDefinitions.EdgeType);
        private readonly VertexInformation _OutgoingEdgeDotSource               = new VertexInformation((long)BaseTypes.OutgoingEdge, OutgoingEdgeOffset + (long)AttributeDefinitions.Source);
        private readonly VertexInformation _OutgoingEdgeDotTarget               = new VertexInformation((long)BaseTypes.OutgoingEdge, OutgoingEdgeOffset + (long)AttributeDefinitions.Target);
        private readonly VertexInformation _OutgoingEdgeDotMultiplicity         = new VertexInformation((long)BaseTypes.Property    , OutgoingEdgeOffset + (long)AttributeDefinitions.Multiplicity);

        #endregion

        #region Property

        private static readonly byte PropertyOffset = OutgoingEdgeOffset + 5;

        private readonly VertexInformation _Property                = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.Property);
        private readonly VertexInformation _PropertyDotType         = new VertexInformation((long)BaseTypes.OutgoingEdge, PropertyOffset + (long)AttributeDefinitions.Type);
        private readonly VertexInformation _PropertyDotIsMandatory  = new VertexInformation((long)BaseTypes.Property    , PropertyOffset + (long)AttributeDefinitions.IsMandatory);
        private readonly VertexInformation _PropertyDotInIndices    = new VertexInformation((long)BaseTypes.IncomingEdge, PropertyOffset + (long)AttributeDefinitions.InIndices);
        private readonly VertexInformation _PropertyDotMultiplicity = new VertexInformation((long)BaseTypes.Property    , PropertyOffset + (long)AttributeDefinitions.Multiplicity );

        #endregion

        #region Vertex

        private static readonly byte VertexOffset = PropertyOffset + 4;

        private readonly VertexInformation _Vertex                    = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.Vertex);
        private readonly VertexInformation _VertexDotUUID             = new VertexInformation((long)BaseTypes.Property  , VertexOffset + (long)AttributeDefinitions.UUID);
        private readonly VertexInformation _VertexDotCreationDate     = new VertexInformation((long)BaseTypes.Property  , VertexOffset + (long)AttributeDefinitions.CreationDate);
        private readonly VertexInformation _VertexDotModificationDate = new VertexInformation((long)BaseTypes.Property  , VertexOffset + (long)AttributeDefinitions.ModificationDate);
        private readonly VertexInformation _VertexDotRevision         = new VertexInformation((long)BaseTypes.Property  , VertexOffset + (long)AttributeDefinitions.Revision);
        private readonly VertexInformation _VertexDotEdition          = new VertexInformation((long)BaseTypes.Property  , VertexOffset + (long)AttributeDefinitions.Edition);
        private readonly VertexInformation _VertexDotComment          = new VertexInformation((long)BaseTypes.Property  , VertexOffset + (long)AttributeDefinitions.Comment);
        private readonly VertexInformation _VertexDotTypeID           = new VertexInformation((long)BaseTypes.Property  , VertexOffset + (long)AttributeDefinitions.TypeID);
        private readonly VertexInformation _VertexDotTypeName         = new VertexInformation((long)BaseTypes.Property  , VertexOffset + (long)AttributeDefinitions.TypeName);

        #endregion

        #region VertexType

        private static readonly byte VertexTypeOffset = VertexOffset + 8;

        private readonly VertexInformation _VertexType                     = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.VertexType);
        private readonly VertexInformation _VertexTypeDotParent            = new VertexInformation((long)BaseTypes.OutgoingEdge, VertexTypeOffset + (long)AttributeDefinitions.Parent);
        private readonly VertexInformation _VertexTypeDotChildren          = new VertexInformation((long)BaseTypes.IncomingEdge, VertexTypeOffset + (long)AttributeDefinitions.Children);
        private readonly VertexInformation _VertexTypeDotUniqueDefinitions = new VertexInformation((long)BaseTypes.OutgoingEdge, VertexTypeOffset + (long)AttributeDefinitions.UniquenessDefinitions);
        private readonly VertexInformation _VertexTypeDotIndices           = new VertexInformation((long)BaseTypes.IncomingEdge, VertexTypeOffset + (long)AttributeDefinitions.Indices);

        #endregion

        #region WeightedEdge

        private static readonly byte WeightedEdgeOffset = VertexTypeOffset + 4;

        private readonly VertexInformation _WeightedEdge          = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.WeightedEdge);
        private readonly VertexInformation _WeightedEdgeDotWeight = new VertexInformation((long)BaseTypes.Property  , WeightedEdgeOffset + (long)AttributeDefinitions.Weight);

        #endregion

        #region OrderableEdge

        private static readonly byte OrderableEdgeOffset = WeightedEdgeOffset + 1;

        private readonly VertexInformation _OrderableEdge         = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.OrderableEdge);
        private readonly VertexInformation _OrderableEdgeDotOrder = new VertexInformation((long)BaseTypes.Property, OrderableEdgeOffset + (long)AttributeDefinitions.Order);

        #endregion

        #region Basic types

        public static readonly Dictionary<BasicTypes, VertexInformation> BasicTypesVertices = new Dictionary<BasicTypes, VertexInformation>
        {
            { BasicTypes.Boolean , _BaseTypeBoolean },
            { BasicTypes.Byte    , _BaseTypeByte },
            { BasicTypes.Char    , _BaseTypeChar },
            { BasicTypes.DateTime, _BaseTypeDateTime },
            { BasicTypes.Double  , _BaseTypeDouble },
            { BasicTypes.Int16   , _BaseTypeInt16 },
            { BasicTypes.Int32   , _BaseTypeInt32 },
            { BasicTypes.Int64   , _BaseTypeInt64 },
            { BasicTypes.SByte   , _BaseTypeSByte },
            { BasicTypes.Single  , _BaseTypeSingle },
            { BasicTypes.String  , _BaseTypeString },
            { BasicTypes.TimeSpan, _BaseTypeTimeSpan },
            { BasicTypes.UInt16  , _BaseTypeUInt16 },
            { BasicTypes.UInt32  , _BaseTypeUInt32 },
            { BasicTypes.UInt64  , _BaseTypeUInt64 },
        };

        private static readonly VertexInformation _BaseTypeInt32    = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Int32);
        private static readonly VertexInformation _BaseTypeString   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.String);
        private static readonly VertexInformation _BaseTypeDateTime = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.DateTime);
        private static readonly VertexInformation _BaseTypeDouble   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Double);
        private static readonly VertexInformation _BaseTypeBoolean  = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Boolean);
        private static readonly VertexInformation _BaseTypeInt64    = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Int64);
        private static readonly VertexInformation _BaseTypeChar     = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Char);
        private static readonly VertexInformation _BaseTypeByte     = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Byte);
        private static readonly VertexInformation _BaseTypeSingle   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Single);
        private static readonly VertexInformation _BaseTypeSByte    = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.SByte);
        private static readonly VertexInformation _BaseTypeInt16    = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Int16);
        private static readonly VertexInformation _BaseTypeUInt32   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.UInt32);
        private static readonly VertexInformation _BaseTypeUInt64   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.UInt64);
        private static readonly VertexInformation _BaseTypeUInt16   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.UInt16);
        private static readonly VertexInformation _BaseTypeTimeSpan = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.TimeSpan);

        #endregion

        #endregion

        #region Indices

        public static readonly Dictionary<BaseUniqueIndex, VertexInformation> BaseUniqueIndicesVertices = new Dictionary<BaseUniqueIndex, VertexInformation>
        {
            { BaseUniqueIndex.AttributeDotID, _BaseUniqueIndexAttributeDotID },
            { BaseUniqueIndex.AttributeDotName, _BaseUniqueIndexAttributeDotName },
            { BaseUniqueIndex.BaseTypeDotID, _BaseUniqueIndexBaseTypeDotID },
            { BaseUniqueIndex.BaseTypeDotName, _BaseUniqueIndexBaseTypeDotName },
            { BaseUniqueIndex.IndexDotID, _BaseUniqueIndexIndexDotID },
            { BaseUniqueIndex.IndexDotName, _BaseUniqueIndexIndexDotName },
        };

        private static readonly VertexInformation _BaseUniqueIndexAttributeDotID   = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.AttributeDotID );
        private static readonly VertexInformation _BaseUniqueIndexAttributeDotName = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.AttributeDotName );
        private static readonly VertexInformation _BaseUniqueIndexBaseTypeDotID    = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.BaseTypeDotID );
        private static readonly VertexInformation _BaseUniqueIndexBaseTypeDotName    = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.BaseTypeDotName );
        private static readonly VertexInformation _BaseUniqueIndexIndexDotID          = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.IndexDotID);
        private static readonly VertexInformation _BaseUniqueIndexIndexDotName          = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.IndexDotName);

        #endregion

        #region C'tor

        /// <summary>
        /// Creates a new instance of DBCreationManager.
        /// </summary>
        /// <param name="mySecurityToken">The root security token... can be left out</param>
        /// <param name="myTransactionToken">The root transaction token... can be left out</param>
        public DBCreationManager(SecurityToken mySecurityToken = null, TransactionToken myTransactionToken = null)
        {
            _security = mySecurityToken;
            _transaction = myTransactionToken;
        }
 
	    #endregion

        #region CreateBaseGraph(IVertexStore)

        /// <summary>
        /// Create the base graph via a vertex store.
        /// </summary>
        /// <param name="myStore">The vertex store, that stores the base graph.</param>
        public void CreateBaseGraph(IVertexStore myStore)
        {
            myStore.CheckNull("myStore");
            
            var creationDate = DateTime.UtcNow.ToBinary();

            AddBasicTypes(myStore, creationDate);
            AddVertexTypes(myStore, creationDate);
            AddEdgeTypes(myStore, creationDate);
            AddIndices(myStore, creationDate);
        }

        #endregion

        #region Base types

        private void AddBasicTypes(IVertexStore myStore, long myCreationDate)
        {
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeBoolean, BasicTypes.Boolean, "BaseTypeBooleanComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeByte, BasicTypes.Byte, "BaseTypeByteComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeChar, BasicTypes.Char, "BaseTypeCharComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeDateTime, BasicTypes.DateTime, "BaseTypeDateTimeComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeDouble, BasicTypes.Double, "BaseTypeDoubleComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeInt16, BasicTypes.Int16, "BaseTypeInt16Comment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeInt32, BasicTypes.Int32, "BaseTypeInt32Comment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeInt64, BasicTypes.Int64, "BaseTypeInt64Comment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeSByte, BasicTypes.SByte, "BaseTypeSByteComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeSingle, BasicTypes.Single, "BaseTypeSingleComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeString, BasicTypes.String, "BaseTypeStringComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeTimeSpan, BasicTypes.TimeSpan, "BaseTypeTimeSpanComment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeUInt16, BasicTypes.UInt16, "BaseTypeUInt16Comment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeUInt32, BasicTypes.UInt32, "BaseTypeUInt32Comment", myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeUInt64, BasicTypes.UInt64, "BaseTypeUInt64Comment", myCreationDate, _security, _transaction);
        }

        #endregion

        #region IndexManager

        private void AddIndices(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Index

            BaseGraphStorageManager.StoreIndex(myStore, _BaseUniqueIndexIndexDotID, BaseTypes.Index, "IndexDotIDIndexComment", myCreationDate, null, true, false, false, _Index, _IndexDotID.SingleEnumerable().ToList(), _security, _transaction);
            BaseGraphStorageManager.StoreIndex(myStore, _BaseUniqueIndexIndexDotName, BaseTypes.Index, "IndexDotNameIndexComment", myCreationDate, null, true, false, false, _Index, _IndexDotName.SingleEnumerable().ToList(), _security, _transaction);


            #endregion

            #region Attribute

            BaseGraphStorageManager.StoreIndex(myStore, _BaseUniqueIndexAttributeDotID, BaseTypes.Index, "AttributeDotIDIndexComment", myCreationDate, null, true, false, false, _Index, _AttributeDotID.SingleEnumerable().ToList(), _security, _transaction);
            BaseGraphStorageManager.StoreIndex(myStore, _BaseUniqueIndexAttributeDotName, BaseTypes.Index, "AttributeDotNameIndexComment", myCreationDate, null, true, false, false, _Index, _AttributeDotName.SingleEnumerable().ToList(), _security, _transaction);

            #endregion

            #region BaseType

            BaseGraphStorageManager.StoreIndex(myStore, _BaseUniqueIndexBaseTypeDotID, BaseTypes.Index, "BaseTypeDotIDIndexComment", myCreationDate, null, true, false, false, _Index, _BaseTypeDotID.SingleEnumerable().ToList(), _security, _transaction);
            BaseGraphStorageManager.StoreIndex(myStore, _BaseUniqueIndexBaseTypeDotName, BaseTypes.Index, "BaseTypeDotNameIndexComment", myCreationDate, null, true, false, false, _Index, _BaseTypeDotName.SingleEnumerable().ToList(), _security, _transaction);

            #endregion

        }

        #endregion

        #region EdgeTypeManager

        private void AddEdgeTypes(IVertexStore myStore, Int64 myCreationDate)
        {
            AddEdge(myStore, myCreationDate);
            AddWeightedEdge(myStore, myCreationDate);
            AddOrderableEdge(myStore, myCreationDate);
        }

        private void AddOrderableEdge(IVertexStore myStore, long myCreationDate)
        {
            #region WeightedEdge vertex

            BaseGraphStorageManager.StoreEdgeType(myStore, _OrderableEdge, BaseTypes.OrderableEdge, "OrderableEdgeEdgeComment", myCreationDate, false, true, _OrderableEdge, _security, _transaction);

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _OrderableEdgeDotOrder, AttributeDefinitions.Order, "OrderComment", myCreationDate, true, PropertyMultiplicity.Single, null, _OrderableEdge, _BaseTypeUInt64, _security, _transaction);

            #endregion
        }

        private void AddWeightedEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region WeightedEdge vertex

            BaseGraphStorageManager.StoreEdgeType(myStore, _WeightedEdge, BaseTypes.WeightedEdge, "WeightedEdgeComment", myCreationDate, false, true, _WeightedEdge, _security, _transaction);

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _WeightedEdgeDotWeight, AttributeDefinitions.Weight, "WeightedComment", myCreationDate, true, PropertyMultiplicity.Single, null, _WeightedEdge, _BaseTypeDouble, _security, _transaction);

            #endregion
        }

        private void AddEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region WeightedEdge vertex

            BaseGraphStorageManager.StoreEdgeType(myStore, _Edge, BaseTypes.Edge, "EdgeComment", myCreationDate, false, true, null, _security, _transaction);

            #endregion
        }

        #endregion

        #region VertexTypeManager

        private void AddVertexTypes(IVertexStore myStore, Int64 myCreationDate)
        {
            AddVertex(myStore, myCreationDate);
            AddBaseType(myStore, myCreationDate);
            AddVertexType(myStore, myCreationDate);
            AddEdgeType(myStore, myCreationDate);
            AddAttribute(myStore, myCreationDate);
            AddOutgoingEdge(myStore, myCreationDate);
            AddIncomingEdge(myStore, myCreationDate);
            AddProperty(myStore, myCreationDate);
            AddIndex(myStore, myCreationDate);
        }

        private void AddVertex(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Vertex vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _Vertex, BaseTypes.VertexType, "VertexComment", myCreationDate, true, false, null, null, _security, _transaction); //TODO uniques
                

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotUUID, AttributeDefinitions.UUID, "UUIDComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Vertex, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotCreationDate, AttributeDefinitions.CreationDate, "CreationDateComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Vertex, _BaseTypeDateTime, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotModificationDate, AttributeDefinitions.ModificationDate, "ModificationDateComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Vertex, _BaseTypeDateTime, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotRevision, AttributeDefinitions.Revision, "RevisionComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Vertex, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotEdition, AttributeDefinitions.Edition, "EditionComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Vertex, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotTypeName, AttributeDefinitions.TypeName, "TypeNameComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Vertex, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotTypeID, AttributeDefinitions.TypeID, "TypeIDComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Vertex, _BaseTypeInt64, _security, _transaction);

            #endregion
        }

        private void AddIndex(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Index vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _Index, BaseTypes.Index, "IndexComment", myCreationDate, false, true, _Vertex, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _IndexDotIndexedProperties, AttributeDefinitions.IndexedProperties, "IndexedPropertiesComment", myCreationDate, EdgeMultiplicity.HyperEdge, _Index, _OrderableEdge, _Property, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _IndexDotDefiningVertexType, AttributeDefinitions.DefiningVertexType, "DefiningVertexTypeComment", myCreationDate, EdgeMultiplicity.SingleEdge, _Index, _Edge, _VertexType, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotID, AttributeDefinitions.ID, "IDComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Index, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotName, AttributeDefinitions.Name, "NameComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Index, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIsUserDefined, AttributeDefinitions.IsUserDefined, "IsUserDefinedComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Index, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIndexClass, AttributeDefinitions.IndexClass, "IndexClassComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Index, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIsSingleValue, AttributeDefinitions.IsSingleValue, "IsSingleValueComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Index, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIsRange, AttributeDefinitions.IsRange, "IsRangeComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Index, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIsVersioned, AttributeDefinitions.IsVersioned, "IsVersionedComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Index, _BaseTypeBoolean, _security, _transaction);

            #endregion
        }

        private void AddProperty(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Property vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _Property, BaseTypes.Property, "PropertyComment", myCreationDate, false, true, _Attribute, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _PropertyDotMultiplicity, AttributeDefinitions.Multiplicity, "PropertyMultiplicityComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Property, _BaseTypeByte, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _PropertyDotIsMandatory, AttributeDefinitions.IsMandatory, "IsMandatoryComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Property, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _PropertyDotType, AttributeDefinitions.Type, "TypeComment", myCreationDate, EdgeMultiplicity.SingleEdge, _Property, _Edge, _BaseType, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _PropertyDotInIndices, AttributeDefinitions.InIndices, "inIndicesComment", myCreationDate, _Property, _IndexDotIndexedProperties, _security, _transaction);

            #endregion
        }

        private void AddIncomingEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region IncomingEdge vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _IncomingEdge, BaseTypes.IncomingEdge, "IncomingEdgeComment", myCreationDate, false, true, _Attribute, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _IncomingEdgeDotRelatedEdge, AttributeDefinitions.RelatedEgde, "RelatedEdgeComment", myCreationDate, EdgeMultiplicity.SingleEdge, _IncomingEdge, _Edge, _OutgoingEdge, _security, _transaction);

            #endregion
        }

        private void AddOutgoingEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region OutgoingEdge vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _OutgoingEdge, BaseTypes.OutgoingEdge, "OutgoingEdgeComment", myCreationDate, false, true, _Attribute, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreIncomingEdge(myStore, _OutgoingEdgeDotRelatedIncomingEdges, AttributeDefinitions.RelatedIncomingEdges, "RelatedIncomingEdgesComment", myCreationDate, _OutgoingEdge, _IncomingEdgeDotRelatedEdge, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _OutgoingEdgeDotMultiplicity, AttributeDefinitions.Multiplicity, "EdgeMultiplicityComment", myCreationDate, true, PropertyMultiplicity.Single, null, _OutgoingEdge, _BaseTypeByte, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotEdgeType, AttributeDefinitions.EdgeType, "EdgeTypeComment", myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, _EdgeType, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotSource, AttributeDefinitions.Source, "SourceComment", myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, _VertexType, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotTarget, AttributeDefinitions.Target, "TargetComment", myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, _VertexType, _security, _transaction);

            #endregion
        }

        private void AddAttribute(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Attribute vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _Attribute, BaseTypes.Attribute, "AttributeComment", myCreationDate, true, false, _Vertex, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _AttributeDotIsUserDefined, AttributeDefinitions.IsUserDefined, "IsUserDefinedComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Attribute, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _AttributeDotName, AttributeDefinitions.Name, "NameComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Attribute, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _AttributeDotID, AttributeDefinitions.ID, "IDComment", myCreationDate, true, PropertyMultiplicity.Single, null, _Attribute, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _AttributeDotDefiningType, AttributeDefinitions.DefiningType, "DefiningTypeComment", myCreationDate, EdgeMultiplicity.SingleEdge, _Attribute, _Edge, _BaseType, _security, _transaction);

            #endregion
        }

        private void AddEdgeType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region EdgeType vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _EdgeType, BaseTypes.EdgeType, "EdgeTypeComment", myCreationDate, false, true, _BaseType, null, _security, _transaction); //TODO uniques


            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _EdgeTypeDotParent, AttributeDefinitions.Parent, "ParentEdgeTypeComment", myCreationDate, EdgeMultiplicity.SingleEdge, _EdgeType, _Edge, _EdgeType, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _EdgeTypeDotChildren, AttributeDefinitions.Children, "ChildrenEdgeTypeComment", myCreationDate, _EdgeType, _EdgeTypeDotParent, _security, _transaction);

            #endregion            
        }

        private void AddVertexType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region VertexType vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _VertexType, BaseTypes.VertexType, "VertexTypeComment", myCreationDate, false, true, _BaseType, null, _security, _transaction); //TODO uniques


            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _VertexTypeDotParent, AttributeDefinitions.Parent, "ParentVertexTypeComment", myCreationDate, EdgeMultiplicity.SingleEdge, _VertexType, _Edge, _VertexType, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _VertexTypeDotUniqueDefinitions, AttributeDefinitions.UniquenessDefinitions, "UniqueDefinitionsComment", myCreationDate, EdgeMultiplicity.HyperEdge, _VertexType, _Edge, _Index, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _VertexTypeDotChildren, AttributeDefinitions.Children, "ChildrenVertexTypeComment", myCreationDate, _VertexType, _VertexTypeDotParent, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _VertexTypeDotIndices, AttributeDefinitions.Indices, "IndicesComment", myCreationDate, _VertexType, _IndexDotDefiningVertexType, _security, _transaction);

            #endregion
        }

        private void AddBaseType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region BaseType vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _BaseType, BaseTypes.BaseType, "BaseTypeComment", myCreationDate, false, false, _Vertex, null, _security, _transaction); //TODO uniques


            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotID, AttributeDefinitions.ID, "IDComment", myCreationDate, true, PropertyMultiplicity.Single, null, _BaseType, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotName, AttributeDefinitions.Name, "NameComment", myCreationDate, true, PropertyMultiplicity.Single, null, _BaseType, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotIsUserDefined, AttributeDefinitions.IsUserDefined, "IsUserDefinedComment", myCreationDate, true, PropertyMultiplicity.Single, null, _BaseType, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotIsAbstract, AttributeDefinitions.IsAbstract, "IsAbstractComment", myCreationDate, true, PropertyMultiplicity.Single, null, _BaseType, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotIsSealed, AttributeDefinitions.IsSealed, "IsSealedComment", myCreationDate, true, PropertyMultiplicity.Single, null, _BaseType, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotBehaviour, AttributeDefinitions.Behaviour, "BehaviourComment", myCreationDate, false, PropertyMultiplicity.Single, null, _BaseType, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _BaseTypeDotAttributes, AttributeDefinitions.Attributes, "AttributesComment", myCreationDate, _BaseType, _AttributeDotDefiningType, _security, _transaction);

            #endregion
        }

        #endregion



    }
}
