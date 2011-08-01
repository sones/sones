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
using sones.Constants;
using sones.GraphDB.Manager.TypeManagement;
using sones.Library.VersionedPluginManager;
using sones.Library.UserdefinedDataType;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A class that creates the base graph.
    /// </summary>
    internal class DBCreationManager
    {
        
        //private readonly Dictionary<AttributeDefinitions, VertexInformation> _infos;

        private readonly SecurityToken _security;
        private readonly TransactionToken _transaction;
        private readonly BaseGraphStorageManager _storageManager;

        #region Vertex information

        #region Attribute
        
        private readonly VertexInformation _Attribute                 = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.Attribute);
        private readonly VertexInformation _AttributeDotIsUserDefined = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.AttributeDotIsUserDefined);
        private readonly VertexInformation _AttributeDotName          = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.AttributeDotName);
        private readonly VertexInformation _AttributeDotDefiningType  = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.AttributeDotDefiningType);

        #endregion

        #region BaseType

        private readonly VertexInformation _BaseType                 = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.BaseType);
        private readonly VertexInformation _BaseTypeDotName          = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.BaseTypeDotName);
        private readonly VertexInformation _BaseTypeDotIsUserDefined = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.BaseTypeDotIsUserDefined);
        private readonly VertexInformation _BaseTypeDotIsAbstract    = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.BaseTypeDotIsAbstract);
        private readonly VertexInformation _BaseTypeDotIsSealed      = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.BaseTypeDotIsSealed);
        private readonly VertexInformation _BaseTypeDotBehaviour     = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.BaseTypeDotBehaviour);
        private readonly VertexInformation _BaseTypeDotAttributes    = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.BaseTypeDotAttributes);

        #endregion

        #region Edge

        private readonly VertexInformation _Edge = new VertexInformation((long)BaseTypes.EdgeType, (long)BaseTypes.Edge);

        #endregion

        #region EdgeType

        private readonly VertexInformation _EdgeType            = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.EdgeType);
        private readonly VertexInformation _EdgeTypeDotParent   = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.EdgeTypeDotParent);
        private readonly VertexInformation _EdgeTypeDotChildren = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.EdgeTypeDotChildren);

        #endregion

        #region IncomingEdge

        private readonly VertexInformation _IncomingEdge               = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.IncomingEdge);
        private readonly VertexInformation _IncomingEdgeDotRelatedEdge = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.IncomingEdgeDotRelatedEgde);

        #endregion

        #region Index

        private readonly VertexInformation _Index                      = new VertexInformation((long)BaseTypes.VertexType   , (long)BaseTypes.Index);
        private readonly VertexInformation _IndexDotIndexedProperties  = new VertexInformation((long)BaseTypes.OutgoingEdge , (long)AttributeDefinitions.IndexDotIndexedProperties);
        private readonly VertexInformation _IndexDotDefiningVertexType = new VertexInformation((long)BaseTypes.OutgoingEdge , (long)AttributeDefinitions.IndexDotDefiningVertexType);
        private readonly VertexInformation _IndexDotName               = new VertexInformation((long)BaseTypes.Property     , (long)AttributeDefinitions.IndexDotName);
        private readonly VertexInformation _IndexDotIsUserDefined      = new VertexInformation((long)BaseTypes.Property     , (long)AttributeDefinitions.IndexDotIsUserDefined);
        private readonly VertexInformation _IndexDotIndexClass         = new VertexInformation((long)BaseTypes.Property     , (long)AttributeDefinitions.IndexDotIndexClass);
        private readonly VertexInformation _IndexDotIsSingleValue      = new VertexInformation((long)BaseTypes.Property     , (long)AttributeDefinitions.IndexDotIsSingleValue);
        private readonly VertexInformation _IndexDotIsRange            = new VertexInformation((long)BaseTypes.Property     , (long)AttributeDefinitions.IndexDotIsRange);
        private readonly VertexInformation _IndexDotIsVersioned        = new VertexInformation((long)BaseTypes.Property     , (long)AttributeDefinitions.IndexDotIsVersioned);

        #endregion

        #region OutgoingEdge

        private readonly VertexInformation _OutgoingEdge                        = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.OutgoingEdge);
        private readonly VertexInformation _OutgoingEdgeDotRelatedIncomingEdges = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.OutgoingEdgeDotRelatedIncomingEdges);
        private readonly VertexInformation _OutgoingEdgeDotEdgeType             = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.OutgoingEdgeDotEdgeType);
        private readonly VertexInformation _OutgoingEdgeDotInnerEdgeType        = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.OutgoingEdgeDotInnerEdgeType);
        private readonly VertexInformation _OutgoingEdgeDotSource               = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.OutgoingEdgeDotSource);
        private readonly VertexInformation _OutgoingEdgeDotTarget               = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.OutgoingEdgeDotTarget);
        private readonly VertexInformation _OutgoingEdgeDotMultiplicity         = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.OutgoingEdgeDotMultiplicity);

        #endregion

        #region Property

        private readonly VertexInformation _Property                = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.Property);
        private readonly VertexInformation _PropertyDotBaseType     = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.PropertyDotBaseType);
        private readonly VertexInformation _PropertyDotIsMandatory  = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.PropertyDotIsMandatory);
        private readonly VertexInformation _PropertyDotInIndices    = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.PropertyDotInIndices);
        private readonly VertexInformation _PropertyDotMultiplicity = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.PropertyDotMultiplicity );

        #endregion

        #region Vertex

        private readonly VertexInformation _Vertex                    = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.Vertex);
        private readonly VertexInformation _VertexDotVertexID         = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.VertexDotVertexID);
        private readonly VertexInformation _VertexDotCreationDate     = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.VertexDotCreationDate);
        private readonly VertexInformation _VertexDotModificationDate = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.VertexDotModificationDate);
        private readonly VertexInformation _VertexDotRevision         = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.VertexDotRevision);
        private readonly VertexInformation _VertexDotEdition          = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.VertexDotEdition);
        private readonly VertexInformation _VertexDotComment          = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.VertexDotComment);
        private readonly VertexInformation _VertexDotTypeID           = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.VertexDotVertexTypeID);
        private readonly VertexInformation _VertexDotTypeName         = new VertexInformation((long)BaseTypes.Property  , (long)AttributeDefinitions.VertexDotVertexTypeName);

        #endregion

        #region VertexType

        private readonly VertexInformation _VertexType                     = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.VertexType);
        private readonly VertexInformation _VertexTypeDotParent            = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.VertexTypeDotParent);
        private readonly VertexInformation _VertexTypeDotChildren          = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.VertexTypeDotChildren);
        private readonly VertexInformation _VertexTypeDotUniqueDefinitions = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.VertexTypeDotUniquenessDefinitions);
        private readonly VertexInformation _VertexTypeDotIndices           = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.VertexTypeDotIndices);

        #endregion

        #region WeightedEdge

        private readonly VertexInformation _WeightedEdge          = new VertexInformation((long)BaseTypes.EdgeType, (long)BaseTypes.Weighted);
        private readonly VertexInformation _WeightedEdgeDotWeight = new VertexInformation((long)BaseTypes.Property, (long)AttributeDefinitions.WeightedEdgeDotWeight);

        #endregion

        #region OrderableEdge

        private readonly VertexInformation _OrderableEdge         = new VertexInformation((long)BaseTypes.EdgeType, (long)BaseTypes.Orderable);
        private readonly VertexInformation _OrderableEdgeDotOrder = new VertexInformation((long)BaseTypes.Property, (long)AttributeDefinitions.OrderableEdgeDotOrder);

        #endregion

        #region BinaryProperty

        private readonly VertexInformation _BinaryProperty = new VertexInformation((long)BaseTypes.VertexType, (long)BaseTypes.BinaryProperty);

        #endregion

        #region Basic types

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

        private static readonly VertexInformation _BaseUniqueIndexBaseTypeDotName   = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.BaseTypeDotName);
        private static readonly VertexInformation _BaseUniqueIndexVertexTypeDotName = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.VertexTypeDotName);
        private static readonly VertexInformation _BaseUniqueIndexEdgeTypeDotName   = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.EdgeTypeDotName);

        private static readonly VertexInformation _BaseUniqueIndexIndexDotName = new VertexInformation((long)BaseTypes.Index, (long)BaseUniqueIndex.IndexDotName);

        #endregion

        #region C'tor

        /// <summary>
        /// Creates a new instance of DBCreationManager.
        /// </summary>
        /// <param name="mySecurityToken">The root security token... can be left out</param>
        /// <param name="myTransactionToken">The root transaction token... can be left out</param>
        public DBCreationManager(SecurityToken mySecurityToken, TransactionToken myTransactionToken, BaseGraphStorageManager myStorageManager)
        {
            _security = mySecurityToken;
            _transaction = myTransactionToken;
            _storageManager = myStorageManager;
        }
 
	    #endregion

        #region UserDefined Data Types

        public void AddUserDefinedDataTypes(IVertexStore myStore, AComponentPluginManager myPluginManager)
        {
            myStore.CheckNull("myStore");
            
            var creationDate = DateTime.UtcNow.ToBinary();

            List<AUserdefinedDataType> userdefinedDataTypePlugins = new List<AUserdefinedDataType>();

            foreach (var plugin in myPluginManager.GetPluginsForType<AUserdefinedDataType>())
            {
                userdefinedDataTypePlugins.Add((AUserdefinedDataType)plugin);
            }

            List<String> userdefinedTypes = new List<String>();
            var maxValue = long.MinValue;
            
            foreach (var vertex in myStore.GetVerticesByTypeID(_security, _transaction, (long)BaseTypes.BaseType))
            { 
                if(vertex.GetProperty<Boolean>((long)AttributeDefinitions.BaseTypeDotIsUserDefined))
                {
                    if (!userdefinedDataTypePlugins.Any(item => item.TypeName == vertex.GetProperty<String>((long)AttributeDefinitions.BaseTypeDotName)))
                    {
                        throw new Exception();
                    }
                    else
                    {
                        userdefinedTypes.Add(vertex.GetProperty<String>((long)AttributeDefinitions.BaseTypeDotName));
                    }
                }

                if (vertex.VertexID > maxValue)
                {
                    maxValue = vertex.VertexID;
                }
            }

            maxValue++;

            foreach (var type in userdefinedDataTypePlugins.Select(item => item.TypeName).Except(userdefinedTypes))
            {
                _storageManager.StoreBasicType(myStore, new VertexInformation((long)BaseTypes.BaseType, maxValue++), type, true, "UserdefinedType" + type, creationDate, _security, _transaction);
            }

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
            _storageManager.StoreBasicType(myStore, _BaseTypeBoolean, "Boolean", false, "BaseTypeBooleanComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeByte, "Byte", false, "BaseTypeByteComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeChar, "Char", false, "BaseTypeCharComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeDateTime, "DateTime", false, "BaseTypeDateTimeComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeDouble, "Double", false, "BaseTypeDoubleComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeInt16, "Int16", false, "BaseTypeInt16Comment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeInt32, "Int32", false, "BaseTypeInt32Comment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeInt64, "Int64", false, "BaseTypeInt64Comment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeSByte, "SByte", false, "BaseTypeSByteComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeSingle, "Single", false, "BaseTypeSingleComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeString, "String", false, "BaseTypeStringComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeTimeSpan, "TimeSpan", false, "BaseTypeTimeSpanComment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeUInt16, "UInt16", false, "BaseTypeUInt16Comment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeUInt32, "UInt32", false, "BaseTypeUInt32Comment", myCreationDate, _security, _transaction);
            _storageManager.StoreBasicType(myStore, _BaseTypeUInt64, "UInt64", false, "BaseTypeUInt64Comment", myCreationDate, _security, _transaction);
        }

        #endregion

        #region IndexManager

        private void AddIndices(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Index

            _storageManager.StoreIndex(myStore, _BaseUniqueIndexIndexDotName, "IndexDotName", "IndexDotNameIndexComment", myCreationDate, null, true, false, false, false, null, _Index, null, _IndexDotName.SingleEnumerable().ToList(), _security, _transaction);

            #endregion

            #region BaseType

            _storageManager.StoreIndex(myStore, _BaseUniqueIndexBaseTypeDotName  , "BaseTypeDotName"  , "BaseTypeDotNameIndexComment"  , myCreationDate, null, true, false, false, false, null, _BaseType  , null, _BaseTypeDotName.SingleEnumerable().ToList(), _security, _transaction);
            _storageManager.StoreIndex(myStore, _BaseUniqueIndexVertexTypeDotName, "VertexTypeDotName", "VertexTypeDotNameIndexComment", myCreationDate, null, true, false, false, false, null, _VertexType, _BaseUniqueIndexBaseTypeDotName, _BaseTypeDotName.SingleEnumerable().ToList(), _security, _transaction);
            _storageManager.StoreIndex(myStore, _BaseUniqueIndexEdgeTypeDotName, "EdgeTypeDotName", "EdgeTypeDotNameIndexComment", myCreationDate, null, true, false, false, false, null, _EdgeType, _BaseUniqueIndexBaseTypeDotName, _BaseTypeDotName.SingleEnumerable().ToList(), _security, _transaction);

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
            #region OrderableEdge vertex

            _storageManager.StoreEdgeType(myStore, _OrderableEdge, "Orderable", "OrderableEdgeEdgeComment", false, myCreationDate, false, true, _Edge, _security, _transaction);

            #endregion

            #region Property vertices

            _storageManager.StoreProperty(myStore, _OrderableEdgeDotOrder, "Order", "OrderComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _OrderableEdge, _BaseTypeUInt64, _security, _transaction);

            #endregion
        }

        private void AddWeightedEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region WeightedEdge vertex

            _storageManager.StoreEdgeType(myStore, _WeightedEdge, "Weighted", "WeightedEdgeComment", false, myCreationDate, false, true, _Edge, _security, _transaction);

            #endregion

            #region Property vertices

            _storageManager.StoreProperty(myStore, _WeightedEdgeDotWeight, "Weight", "WeightedComment", myCreationDate, true, PropertyMultiplicity.Single, "0.0", false, _WeightedEdge, _BaseTypeDouble, _security, _transaction);

            #endregion
        }

        private void AddEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region WeightedEdge vertex

            _storageManager.StoreEdgeType(myStore, _Edge, "Edge", "EdgeComment", false, myCreationDate, false, true, null, _security, _transaction);

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
            AddBinaryProperty(myStore, myCreationDate);
            AddIndex(myStore, myCreationDate);
        }

        private void AddVertex(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Vertex vertex

            _storageManager.StoreVertexType(myStore, _Vertex, GlobalConstants.Vertex, "VertexComment", myCreationDate, true, false, false, null, null, _security, _transaction);
                

            #endregion

            #region Property vertices

            _storageManager.StoreProperty(myStore, _VertexDotVertexID, GlobalConstants.VertexDotVertexID, "VertexIDComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Vertex, _BaseTypeInt64, _security, _transaction);
            _storageManager.StoreProperty(myStore, _VertexDotCreationDate, GlobalConstants.VertexDotCreationDate, "CreationDateComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Vertex, _BaseTypeInt64, _security, _transaction);
            _storageManager.StoreProperty(myStore, _VertexDotModificationDate, GlobalConstants.VertexDotModificationDate, "ModificationDateComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Vertex, _BaseTypeInt64, _security, _transaction);
            _storageManager.StoreProperty(myStore, _VertexDotRevision, GlobalConstants.VertexDotRevision, "RevisionComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Vertex, _BaseTypeInt64, _security, _transaction);
            _storageManager.StoreProperty(myStore, _VertexDotEdition, GlobalConstants.VertexDotEdition, "EditionComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Vertex, _BaseTypeString, _security, _transaction);
            _storageManager.StoreProperty(myStore, _VertexDotTypeName, GlobalConstants.VertexDotVertexTypeName, "TypeNameComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Vertex, _BaseTypeString, _security, _transaction);
            _storageManager.StoreProperty(myStore, _VertexDotTypeID, GlobalConstants.VertexDotVertexTypeID, "TypeIDComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Vertex, _BaseTypeInt64, _security, _transaction);
            _storageManager.StoreProperty(myStore, _VertexDotComment, GlobalConstants.VertexDotComment, "CommentComment", myCreationDate, false, PropertyMultiplicity.Single, null, false, _Vertex, _BaseTypeString, _security, _transaction);

            #endregion
        }

        private void AddIndex(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Index vertex

            _storageManager.StoreVertexType(myStore, _Index, BaseTypes.Index, "IndexComment", myCreationDate, false, true, false, _Vertex, new[] {_BaseUniqueIndexIndexDotName}, _security, _transaction); 

            #endregion

            #region Property vertices

            _storageManager.StoreOutgoingEdge(myStore, _IndexDotIndexedProperties, "IndexedProperties", "IndexedPropertiesComment", false, myCreationDate, EdgeMultiplicity.MultiEdge, _Index, _Edge, _OrderableEdge, _Property, _security, _transaction);
            _storageManager.StoreOutgoingEdge(myStore, _IndexDotDefiningVertexType, "DefiningVertexType", "DefiningVertexTypeComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _Index, _Edge, null, _VertexType, _security, _transaction);
            _storageManager.StoreProperty(myStore, _IndexDotName, "Name", "NameComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Index, _BaseTypeString, _security, _transaction);
            _storageManager.StoreProperty(myStore, _IndexDotIsUserDefined, "IsUserDefined", "IsUserDefinedComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Index, _BaseTypeBoolean, _security, _transaction);
            _storageManager.StoreProperty(myStore, _IndexDotIndexClass, "IndexClass", "IndexClassComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Index, _BaseTypeString, _security, _transaction);
            _storageManager.StoreProperty(myStore, _IndexDotIsSingleValue, "IsSingleValue", "IsSingleValueComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Index, _BaseTypeBoolean, _security, _transaction);
            _storageManager.StoreProperty(myStore, _IndexDotIsRange, "IsRange", "IsRangeComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Index, _BaseTypeBoolean, _security, _transaction);
            _storageManager.StoreProperty(myStore, _IndexDotIsVersioned, "IsVersioned", "IsVersionedComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Index, _BaseTypeBoolean, _security, _transaction);

            #endregion
        }

        private void AddBinaryProperty(IVertexStore myStore, long myCreationDate)
        {
            #region Property vertex

            _storageManager.StoreVertexType(myStore, _BinaryProperty, BaseTypes.BinaryProperty, "BinaryPropertyComment", myCreationDate, false, true, false, _Attribute, null, _security, _transaction); 

            #endregion

        }

        private void AddProperty(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Property vertex

            _storageManager.StoreVertexType(myStore, _Property, BaseTypes.Property, "PropertyComment", myCreationDate, false, true, false, _Attribute, null, _security, _transaction);

            #endregion

            #region Property vertices

            _storageManager.StoreProperty(myStore, _PropertyDotMultiplicity, "Multiplicity", "PropertyMultiplicityComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Property, _BaseTypeByte, _security, _transaction);
            _storageManager.StoreProperty(myStore, _PropertyDotIsMandatory, "IsMandatory", "IsMandatoryComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Property, _BaseTypeBoolean, _security, _transaction);
            _storageManager.StoreOutgoingEdge(myStore, _PropertyDotBaseType, "BaseType", "TypeComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _Property, _Edge, null, _BaseType, _security, _transaction);
            _storageManager.StoreIncomingEdge(myStore, _PropertyDotInIndices, "InIndices", "InIndicesComment", false, myCreationDate, _Property, _IndexDotIndexedProperties, _security, _transaction);

            #endregion
        }

        private void AddIncomingEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region IncomingEdge vertex

            _storageManager.StoreVertexType(myStore, _IncomingEdge, BaseTypes.IncomingEdge, "IncomingEdgeComment", myCreationDate, false, true, false, _Attribute, null, _security, _transaction); 

            #endregion

            #region Property vertices

            _storageManager.StoreOutgoingEdge(myStore, _IncomingEdgeDotRelatedEdge, "RelatedEgde", "RelatedEdgeComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _IncomingEdge, _Edge, null, _OutgoingEdge, _security, _transaction);

            #endregion
        }

        private void AddOutgoingEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            #region OutgoingEdge vertex

            _storageManager.StoreVertexType(myStore, _OutgoingEdge, BaseTypes.OutgoingEdge, "OutgoingEdgeComment", myCreationDate, false, true, false, _Attribute, null, _security, _transaction); 

            #endregion

            #region Property vertices

            _storageManager.StoreIncomingEdge(myStore, _OutgoingEdgeDotRelatedIncomingEdges, "RelatedIncomingEdges", "RelatedIncomingEdgesComment", false, myCreationDate, _OutgoingEdge, _IncomingEdgeDotRelatedEdge, _security, _transaction);
            _storageManager.StoreProperty(myStore, _OutgoingEdgeDotMultiplicity, "Multiplicity", "EdgeMultiplicityComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _OutgoingEdge, _BaseTypeByte, _security, _transaction);
            _storageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotEdgeType, "EdgeType", "EdgeTypeComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, null, _EdgeType, _security, _transaction);
            _storageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotInnerEdgeType, "InnerEdgeType", "InnerEdgeTypeComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, null, _EdgeType, _security, _transaction);
            _storageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotSource, "Source", "SourceComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, null, _VertexType, _security, _transaction);
            _storageManager.StoreOutgoingEdge(myStore, _OutgoingEdgeDotTarget, "Target", "TargetComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _OutgoingEdge, _Edge, null, _VertexType, _security, _transaction);

            #endregion
        }

        private void AddAttribute(IVertexStore myStore, Int64 myCreationDate)
        {
            #region Attribute vertex

            _storageManager.StoreVertexType(myStore, _Attribute, BaseTypes.Attribute, "AttributeComment", myCreationDate, true, false, false, _Vertex, null, _security, _transaction);

            #endregion

            #region Property vertices

            _storageManager.StoreProperty(myStore, _AttributeDotIsUserDefined, "IsUserDefined", "IsUserDefinedComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Attribute, _BaseTypeBoolean, _security, _transaction);
            _storageManager.StoreProperty(myStore, _AttributeDotName, "Name", "NameComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _Attribute, _BaseTypeString, _security, _transaction);
            _storageManager.StoreOutgoingEdge(myStore, _AttributeDotDefiningType, "DefiningType", "DefiningTypeComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _Attribute, _Edge, null, _BaseType, _security, _transaction);

            #endregion
        }

        private void AddEdgeType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region EdgeType vertex

            _storageManager.StoreVertexType(myStore, _EdgeType, BaseTypes.EdgeType, "EdgeTypeComment", myCreationDate, false, true, false, _BaseType, new[] {_BaseUniqueIndexEdgeTypeDotName}, _security, _transaction); 


            #endregion

            #region Property vertices

            _storageManager.StoreOutgoingEdge(myStore, _EdgeTypeDotParent, "Parent", "ParentEdgeTypeComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _EdgeType, _Edge, null, _EdgeType, _security, _transaction);
            _storageManager.StoreIncomingEdge(myStore, _EdgeTypeDotChildren, "Children", "ChildrenEdgeTypeComment", false, myCreationDate, _EdgeType, _EdgeTypeDotParent, _security, _transaction);

            #endregion            
        }

        private void AddVertexType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region VertexType vertex

            _storageManager.StoreVertexType(myStore, _VertexType, BaseTypes.VertexType, "VertexTypeComment", myCreationDate, false, true, false, _BaseType, new[] {_BaseUniqueIndexVertexTypeDotName}, _security, _transaction); 


            #endregion

            #region Property vertices

            _storageManager.StoreOutgoingEdge(myStore, _VertexTypeDotParent, "Parent", "ParentVertexTypeComment", false, myCreationDate, EdgeMultiplicity.SingleEdge, _VertexType, _Edge, null, _VertexType, _security, _transaction);
            _storageManager.StoreOutgoingEdge(myStore, _VertexTypeDotUniqueDefinitions, "UniquenessDefinitions", "UniqueDefinitionsComment", false, myCreationDate, EdgeMultiplicity.MultiEdge, _VertexType, _Edge, _Edge, _Index, _security, _transaction);
            _storageManager.StoreIncomingEdge(myStore, _VertexTypeDotChildren, "Children", "ChildrenVertexTypeComment", false, myCreationDate, _VertexType, _VertexTypeDotParent, _security, _transaction);
            _storageManager.StoreIncomingEdge(myStore, _VertexTypeDotIndices, "Indices", "IndicesComment", false, myCreationDate, _VertexType, _IndexDotDefiningVertexType, _security, _transaction);

            #endregion
        }

        private void AddBaseType(IVertexStore myStore, Int64 myCreationDate)
        {
            #region BaseType vertex

            _storageManager.StoreVertexType(myStore, _BaseType, BaseTypes.BaseType, "BaseTypeComment", myCreationDate, false, false, false, _Vertex, new[] {_BaseUniqueIndexBaseTypeDotName}, _security, _transaction); 


            #endregion

            #region Property vertices

            _storageManager.StoreProperty(myStore, _BaseTypeDotName, "Name", "NameComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _BaseType, _BaseTypeString, _security, _transaction);
            _storageManager.StoreProperty(myStore, _BaseTypeDotIsUserDefined, "IsUserDefined", "IsUserDefinedComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _BaseType, _BaseTypeBoolean, _security, _transaction);
            _storageManager.StoreProperty(myStore, _BaseTypeDotIsAbstract, "IsAbstract", "IsAbstractComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _BaseType, _BaseTypeBoolean, _security, _transaction);
            _storageManager.StoreProperty(myStore, _BaseTypeDotIsSealed, "IsSealed", "IsSealedComment", myCreationDate, true, PropertyMultiplicity.Single, null, false, _BaseType, _BaseTypeBoolean, _security, _transaction);
            _storageManager.StoreProperty(myStore, _BaseTypeDotBehaviour, "Behaviour", "BehaviourComment", myCreationDate, false, PropertyMultiplicity.Single, null, false, _BaseType, _BaseTypeString, _security, _transaction);
            _storageManager.StoreIncomingEdge(myStore, _BaseTypeDotAttributes, "Attributes", "AttributesComment", false, myCreationDate, _BaseType, _AttributeDotDefiningType, _security, _transaction);

            #endregion
        }

        #endregion

        public bool CheckBaseGraph(IVertexStore myVertexStore)
        {
            return myVertexStore.GetVertex(_security, _transaction, (long)BaseTypes.VertexType, (long)BaseTypes.VertexType, String.Empty) != null;
        }
    }
}
