using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.VertexStore;
using sones.Library.LanguageExtensions;
using sones.Library.Commons.VertexStore.Definitions;
using sones.GraphDB.TypeManagement.Base;
using System.Resources;
using sones.Library.Commons.VertexStore.Definitions;
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
        #region Resource manager

        private readonly ResourceManager ResMgr = new ResourceManager("SonesGraphDB", typeof(DBCreationManager).Assembly);

        #endregion


        private readonly Dictionary<AttributeDefinitions, VertexInformation> _infos;

        private readonly SecurityToken _security;
        private readonly TransactionToken _transaction;

        #region Vertex information

        #region Attribute

        private readonly VertexInformation _Attribute                 = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.Attribute);
        private readonly VertexInformation _AttributeDotID            = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.ID);
        private readonly VertexInformation _AttributeDotIsUserDefined = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.IsUserDefined);
        private readonly VertexInformation _AttributeDotName          = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.Name);
        private readonly VertexInformation _AttributeDotDefiningType  = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.DefiningType);

        #endregion

        #region BaseType

        private readonly VertexInformation _BaseType                 = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.BaseType);
        private readonly VertexInformation _BaseTypeDotID            = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.ID);
        private readonly VertexInformation _BaseTypeDotName          = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.Name);
        private readonly VertexInformation _BaseTypeDotIsUserDefined = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.IsUserDefined);
        private readonly VertexInformation _BaseTypeDotIsAbstract    = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.IsAbstract);
        private readonly VertexInformation _BaseTypeDotIsSealed      = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.IsSealed);
        private readonly VertexInformation _BaseTypeDotBehaviour     = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.Behaviour);
        private readonly VertexInformation _BaseTypeDotAttributes    = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.Attributes);

        #endregion

        #region Edge

        private readonly VertexInformation _Edge = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.Edge);

        #endregion

        #region EdgeType

        private readonly VertexInformation _EdgeType            = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.EdgeType);
        private readonly VertexInformation _EdgeTypeDotParent   = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.Parent);
        private readonly VertexInformation _EdgeTypeDotChildren = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.Children);

        #endregion

        #region IncomingEdge

        private readonly VertexInformation _IncomingEdge               = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.IncomingEdge);
        private readonly VertexInformation _IncomingEdgeDotRelatedEdge = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.RelatedEgde);

        #endregion

        #region Index

        private readonly VertexInformation _Index                      = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.Index);
        private readonly VertexInformation _IndexDotIndexedProperties  = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.IndexedProperties);
        private readonly VertexInformation _IndexDotDefiningVertexType = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.DefiningVertexType);
        private readonly VertexInformation _IndexDotID                 = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.ID);
        private readonly VertexInformation _IndexDotName               = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.Name);
        private readonly VertexInformation _IndexDotIsUserDefined      = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.IsUserDefined);
        private readonly VertexInformation _IndexDotIndexClass         = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.IndexClass);
        private readonly VertexInformation _IndexDotIsSingleValue      = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.IsSingleValue);
        private readonly VertexInformation _IndexDotIsRange            = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.IsRange);
        private readonly VertexInformation _IndexDotIsVersioned        = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.IsVersioned);

        #endregion

        #region OutgoingEdge

        private readonly VertexInformation _OutgoingEdge                        = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.OutgoingEdge);
        private readonly VertexInformation _OutgoingEdgeDotRelatedIncomingEdges = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.RelatedIncomingEdges);
        private readonly VertexInformation _OutgoingEdgeDotEdgeType             = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.EdgeType);
        private readonly VertexInformation _OutgoingEdgeDotSource               = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.Source);
        private readonly VertexInformation _OutgoingEdgeDotTarget               = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.Target);
        private readonly VertexInformation _OutgoingEdgeDotMultiplicity         = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.Multiplicity);

        #endregion

        #region Property

        private readonly VertexInformation _Property                = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.Property);
        private readonly VertexInformation _PropertyDotType         = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.Type);
        private readonly VertexInformation _PropertyDotIsMandatory  = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.IsMandatory);
        private readonly VertexInformation _PropertyDotInIndices    = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.InIndices);
        private readonly VertexInformation _PropertyDotMultiplicity = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.Multiplicity );

        #endregion

        #region Vertex

        private readonly VertexInformation _Vertex                    = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.Vertex);
        private readonly VertexInformation _VertexDotUUID             = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.UUID);
        private readonly VertexInformation _VertexDotCreationDate     = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.CreationDate);
        private readonly VertexInformation _VertexDotModificationDate = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.ModificationDate);
        private readonly VertexInformation _VertexDotRevision         = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.Revision);
        private readonly VertexInformation _VertexDotEdition          = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.Edition);
        private readonly VertexInformation _VertexDotComment          = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.Comment);
        private readonly VertexInformation _VertexDotTypeID           = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.TypeID);
        private readonly VertexInformation _VertexDotTypeName         = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.TypeName);

        #endregion

        #region VertexType

        private readonly VertexInformation _VertexType                     = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.VertexType);
        private readonly VertexInformation _VertexTypeDotParent            = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.Parent);
        private readonly VertexInformation _VertexTypeDotChildren          = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.Children);
        private readonly VertexInformation _VertexTypeDotUniqueDefinitions = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.UniquenessDefinitions);
        private readonly VertexInformation _VertexTypeDotIndices           = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.Indices);

        #endregion

        #region WeightedEdge

        private readonly VertexInformation _WeightedEdge          = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.WeightedEdge);
        private readonly VertexInformation _WeightedEdgeDotWeight = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.Weight);

        #endregion

        #region Basic types

        private readonly VertexInformation _BaseTypeInt32    = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Int32);
        private readonly VertexInformation _BaseTypeString   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.String);
        private readonly VertexInformation _BaseTypeDateTime = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.DateTime);
        private readonly VertexInformation _BaseTypeDouble   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Double);
        private readonly VertexInformation _BaseTypeBoolean  = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Boolean);
        private readonly VertexInformation _BaseTypeInt64    = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Int64);
        private readonly VertexInformation _BaseTypeChar     = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Char);
        private readonly VertexInformation _BaseTypeByte     = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Byte);
        private readonly VertexInformation _BaseTypeSingle   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Single);
        private readonly VertexInformation _BaseTypeSByte    = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.SByte);
        private readonly VertexInformation _BaseTypeInt16    = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.Int16);
        private readonly VertexInformation _BaseTypeUInt32   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.UInt32);
        private readonly VertexInformation _BaseTypeUInt64   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.UInt64);
        private readonly VertexInformation _BaseTypeUInt16   = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.UInt16);
        private readonly VertexInformation _BaseTypeTimeSpan = new VertexInformation((long)BaseTypes.BaseType, (long)BasicTypes.TimeSpan);

        #endregion

        #endregion

        #region Indices


        #endregion

        #region C'tor

        /// <summary>
        /// Creates a new instance of DBCreationManager.
        /// </summary>
        /// <param name="mySecurityToken">The root security token</param>
        /// <param name="myTransactionToken">The root transaction token... can be left out</param>
        public DBCreationManager(SecurityToken mySecurityToken, TransactionToken myTransactionToken = null)
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
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeBoolean, BasicTypes.Boolean, ResMgr.GetString("BaseTypeBooleanComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeByte, BasicTypes.Byte, ResMgr.GetString("BaseTypeByteComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeChar, BasicTypes.Char, ResMgr.GetString("BaseTypeCharComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeDateTime, BasicTypes.DateTime, ResMgr.GetString("BaseTypeDateTimeComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeDouble, BasicTypes.Double, ResMgr.GetString("BaseTypeDoubleComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeInt16, BasicTypes.Int16, ResMgr.GetString("BaseTypeInt16Comment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeInt32, BasicTypes.Int32, ResMgr.GetString("BaseTypeInt32Comment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeInt64, BasicTypes.Int64, ResMgr.GetString("BaseTypeInt64Comment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeSByte, BasicTypes.SByte, ResMgr.GetString("BaseTypeSByteComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeSingle, BasicTypes.Single, ResMgr.GetString("BaseTypeSingleComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeString, BasicTypes.String, ResMgr.GetString("BaseTypeStringComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeTimeSpan, BasicTypes.TimeSpan, ResMgr.GetString("BaseTypeTimeSpanComment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeUInt16, BasicTypes.UInt16, ResMgr.GetString("BaseTypeUInt16Comment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeUInt32, BasicTypes.UInt32, ResMgr.GetString("BaseTypeUInt32Comment"), myCreationDate, _security, _transaction);
            BaseGraphStorageManager.StoreBasicType(myStore, _BaseTypeUInt64, BasicTypes.UInt64, ResMgr.GetString("BaseTypeUInt64Comment"), myCreationDate, _security, _transaction);
        }

        #endregion

        #region IndexManager

        private void AddIndices(IVertexStore myStore, Int64 myCreationDate)
        {
            AddUniqueIndexOnBaseTypeDotName(myStore, myCreationDate);
            AddUniqueIndexOnIndexDotName(myStore, myCreationDate);
            AddUniqueIndexOnBaseTypeDotID(myStore, myCreationDate);
            AddUniqueIndexOnIndexDotID(myStore, myCreationDate);
        }

        private static void AddUniqueIndexOnIndexDotID(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddUniqueIndexOnBaseTypeDotID(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddUniqueIndexOnIndexDotName(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddUniqueIndexOnBaseTypeDotName(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region EdgeTypeManager

        private void AddEdgeTypes(IVertexStore myStore, Int64 myCreationDate)
        {
            AddEdge(myStore, myCreationDate);
            AddWeightedEdge(myStore, myCreationDate);
        }

        private void AddWeightedEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region WeightedEdge vertex

            BaseGraphStorageManager.StoreEdgeType(myStore, _WeightedEdge, BaseTypes.WeightedEdge, ResMgr.GetString("WeightedEdgeComment"), myCreationDate, false, true, _Edge, _security, _transaction);

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _WeightedEdgeDotWeight, AttributeDefinitions.Weight, ResMgr.GetString("WeightedComment"), myCreationDate, true, PropertyMultiplicity.Single, _WeightedEdge, _BaseTypeDouble, _security, _transaction);

            #endregion
        }

        private void AddEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region WeightedEdge vertex

            BaseGraphStorageManager.StoreEdgeType(myStore, _Edge, BaseTypes.Edge, ResMgr.GetString("EdgeComment"), myCreationDate, false, true, null, _security, _transaction);

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

            BaseGraphStorageManager.StoreVertexType(myStore, _Vertex, BaseTypes.Vertex, ResMgr.GetString("VertexComment"), myCreationDate, true, false, null, null, _security, _transaction); //TODO uniques
                

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotUUID, AttributeDefinitions.UUID, ResMgr.GetString("UUIDComment"), myCreationDate, true, PropertyMultiplicity.Single, _Vertex, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotCreationDate, AttributeDefinitions.CreationDate, ResMgr.GetString("CreationDateComment"), myCreationDate, true, PropertyMultiplicity.Single, _Vertex, _BaseTypeDateTime, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotModificationDate, AttributeDefinitions.ModificationDate, ResMgr.GetString("ModificationDateComment"), myCreationDate, true, PropertyMultiplicity.Single, _Vertex, _BaseTypeDateTime, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotRevision, AttributeDefinitions.Revision, ResMgr.GetString("RevisionComment"), myCreationDate, true, PropertyMultiplicity.Single, _Vertex, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotEdition, AttributeDefinitions.Edition, ResMgr.GetString("EditionComment"), myCreationDate, true, PropertyMultiplicity.Single, _Vertex, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotTypeName, AttributeDefinitions.TypeName, ResMgr.GetString("TypeNameComment"), myCreationDate, true, PropertyMultiplicity.Single, _Vertex, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _VertexDotTypeID, AttributeDefinitions.TypeID, ResMgr.GetString("TypeIDComment"), myCreationDate, true, PropertyMultiplicity.Single, _Vertex, _BaseTypeInt64, _security, _transaction);

            #endregion
        }

        private void AddIndex(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Index vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _Index, BaseTypes.Index, ResMgr.GetString("IndexComment"), myCreationDate, false, true, _Vertex, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _IndexDotIndexedProperties, AttributeDefinitions.IndexedProperties, ResMgr.GetString("IndexedPropertiesComment"), myCreationDate, EdgeMultiplicity.HyperEdge, _Index, _Edge, _Property, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _IndexDotDefiningVertexType, AttributeDefinitions.DefiningVertexType, ResMgr.GetString("DefiningVertexTypeComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _Index, _Edge, _VertexType, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotID, AttributeDefinitions.ID, ResMgr.GetString("IDComment"), myCreationDate, true, PropertyMultiplicity.Single, _Index, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotName, AttributeDefinitions.Name, ResMgr.GetString("NameComment"), myCreationDate, true, PropertyMultiplicity.Single, _Index, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIsUserDefined, AttributeDefinitions.IsUserDefined, ResMgr.GetString("IsUserDefinedComment"), myCreationDate, true, PropertyMultiplicity.Single, _Index, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIndexClass, AttributeDefinitions.IndexClass, ResMgr.GetString("IndexClassComment"), myCreationDate, true, PropertyMultiplicity.Single, _Index, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIsSingleValue, AttributeDefinitions.IsSingleValue, ResMgr.GetString("IsSingleValueComment"), myCreationDate, true, PropertyMultiplicity.Single, _Index, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIsRange, AttributeDefinitions.IsRange, ResMgr.GetString("IsRangeComment"), myCreationDate, true, PropertyMultiplicity.Single, _Index, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _IndexDotIsVersioned, AttributeDefinitions.IsVersioned, ResMgr.GetString("IsVersionedComment"), myCreationDate, true, PropertyMultiplicity.Single, _Index, _BaseTypeBoolean, _security, _transaction);

            #endregion
        }

        private void AddProperty(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Property vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _Property, BaseTypes.Property, ResMgr.GetString("PropertyComment"), myCreationDate, false, true, _Attribute, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _PropertyDotMultiplicity, AttributeDefinitions.Multiplicity, ResMgr.GetString("PropertyMultiplicityComment"), myCreationDate, true, PropertyMultiplicity.Single, _Property, _BaseTypeByte, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _PropertyDotIsMandatory, AttributeDefinitions.IsMandatory, ResMgr.GetString("IsMandatoryComment"), myCreationDate, true, PropertyMultiplicity.Single, _Property, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _PropertyDotType, AttributeDefinitions.Type, ResMgr.GetString("TypeComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _Property, _Edge, _BaseType, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _PropertyDotInIndices, AttributeDefinitions.InIndices, ResMgr.GetString("inIndicesComment"), myCreationDate, _Property, _IndexDotIndexedProperties, _security, _transaction);

            #endregion
        }

        private void AddIncomingEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region IncomingEdge vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _IncomingEdge, BaseTypes.IncomingEdge, ResMgr.GetString("IncomingEdgeComment"), myCreationDate, false, true, _Attribute, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _IncomingEdgeDotRelatedEdge, AttributeDefinitions.RelatedEgde, ResMgr.GetString("RelatedEdgeComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _IncomingEdge, _Edge, _OutgoingEdge, _security, _transaction);

            #endregion
        }

        private void AddOutgoingEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region OutgoingEdge vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _OutgoingEdge, BaseTypes.OutgoingEdge, ResMgr.GetString("OutgoingEdgeComment"), myCreationDate, false, true, _Attribute, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreIncomingEdge(myStore, _OutgoingEdgeDotRelatedIncomingEdges, AttributeDefinitions.RelatedIncomingEdges, ResMgr.GetString("RelatedIncomingEdgesComment"), myCreationDate, _OutgoingEdge, _IncomingEdgeDotRelatedEdge, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _OutgoingEdgeDotMultiplicity, AttributeDefinitions.Multiplicity, ResMgr.GetString("EdgeMultiplicityComment"), myCreationDate, true, PropertyMultiplicity.Single, _OutgoingEdge, _BaseTypeByte, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotEdgeType, AttributeDefinitions.EdgeType, ResMgr.GetString("EdgeTypeComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, _EdgeType, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotSource, AttributeDefinitions.Source, ResMgr.GetString("SourceComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, _VertexType, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotTarget, AttributeDefinitions.Target, ResMgr.GetString("TargetComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, _VertexType, _security, _transaction);

            #endregion
        }

        private void AddAttribute(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Attribute vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _Attribute, BaseTypes.Attribute, ResMgr.GetString("AttributeComment"), myCreationDate, true, false, _Vertex, null, _security, _transaction); //TODO uniques

            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _AttributeDotIsUserDefined, AttributeDefinitions.IsUserDefined, ResMgr.GetString("IsUserDefinedComment"), myCreationDate, true, PropertyMultiplicity.Single, _Attribute, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _AttributeDotName, AttributeDefinitions.Name, ResMgr.GetString("NameComment"), myCreationDate, true, PropertyMultiplicity.Single, _Attribute, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _AttributeDotID, AttributeDefinitions.ID, ResMgr.GetString("IDComment"), myCreationDate, true, PropertyMultiplicity.Single, _Attribute, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _AttributeDotDefiningType, AttributeDefinitions.DefiningType, ResMgr.GetString("DefiningTypeComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _Attribute, _Edge, _BaseType, _security, _transaction);

            #endregion
        }

        private void AddEdgeType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region EdgeType vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _EdgeType, BaseTypes.EdgeType, ResMgr.GetString("EdgeTypeComment"), myCreationDate, false, true, _BaseType, null, _security, _transaction); //TODO uniques


            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _EdgeTypeDotParent, AttributeDefinitions.Parent, ResMgr.GetString("ParentEdgeTypeComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _EdgeType, _Edge, _EdgeType, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _EdgeTypeDotChildren, AttributeDefinitions.Children, ResMgr.GetString("ChildrenEdgeTypeComment"), myCreationDate, _EdgeType, _EdgeTypeDotParent, _security, _transaction);

            #endregion            
        }

        private void AddVertexType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region VertexType vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _VertexType, BaseTypes.VertexType, ResMgr.GetString("VertexTypeComment"), myCreationDate, false, true, _BaseType, null, _security, _transaction); //TODO uniques


            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _VertexTypeDotParent, AttributeDefinitions.Parent, ResMgr.GetString("ParentVertexTypeComment"), myCreationDate, EdgeMultiplicity.SingleEdge, _VertexType, _Edge, _VertexType, _security, _transaction);
            BaseGraphStorageManager.StoreOutgoingEdge(myStore, _VertexTypeDotUniqueDefinitions, AttributeDefinitions.UniquenessDefinitions, ResMgr.GetString("UniqueDefinitionsComment"), myCreationDate, EdgeMultiplicity.HyperEdge, _VertexType, _Edge, _Index, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _VertexTypeDotChildren, AttributeDefinitions.Children, ResMgr.GetString("ChildrenVertexTypeComment"), myCreationDate, _VertexType, _VertexTypeDotParent, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _VertexTypeDotIndices, AttributeDefinitions.Indices, ResMgr.GetString("IndicesComment"), myCreationDate, _VertexType, _IndexDotDefiningVertexType, _security, _transaction);

            #endregion
        }

        private void AddBaseType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region BaseType vertex

            BaseGraphStorageManager.StoreVertexType(myStore, _BaseType, BaseTypes.BaseType, ResMgr.GetString("BaseTypeComment"), myCreationDate, false, false, _Vertex, null, _security, _transaction); //TODO uniques


            #endregion

            #region Property vertices

            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotID, AttributeDefinitions.ID, ResMgr.GetString("IDComment"), myCreationDate, true, PropertyMultiplicity.Single, _BaseType, _BaseTypeInt64, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotName, AttributeDefinitions.Name, ResMgr.GetString("NameComment"), myCreationDate, true, PropertyMultiplicity.Single, _BaseType, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotIsUserDefined, AttributeDefinitions.IsUserDefined, ResMgr.GetString("IsUserDefinedComment"), myCreationDate, true, PropertyMultiplicity.Single, _BaseType, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotIsAbstract, AttributeDefinitions.IsAbstract, ResMgr.GetString("IsAbstractComment"), myCreationDate, true, PropertyMultiplicity.Single, _BaseType, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotIsSealed, AttributeDefinitions.IsSealed, ResMgr.GetString("IsSealedComment"), myCreationDate, true, PropertyMultiplicity.Single, _BaseType, _BaseTypeBoolean, _security, _transaction);
            BaseGraphStorageManager.StoreProperty(myStore, _BaseTypeDotBehaviour, AttributeDefinitions.Behaviour, ResMgr.GetString("BehaviourComment"), myCreationDate, false, PropertyMultiplicity.Single, _BaseType, _BaseTypeString, _security, _transaction);
            BaseGraphStorageManager.StoreIncomingEdge(myStore, _BaseTypeDotAttributes, AttributeDefinitions.Attributes, ResMgr.GetString("AttributesComment"), myCreationDate, _BaseType, _AttributeDotDefiningType, _security, _transaction);

            #endregion
        }

        #endregion



    }
}
