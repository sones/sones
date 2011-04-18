using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VertexStore;
using sones.Library.LanguageExtensions;
using sones.Library.VertexStore.Definitions;
using sones.GraphDB.TypeManagement.Base;
using System.Resources;

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

        private const Int64 _EdgeEdgeType = (long)BaseTypes.Edge;

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

        #endregion

        #region Property

        private readonly VertexInformation _Property               = new VertexInformation((long)BaseTypes.VertexType  , (long)BaseTypes.Property);
        private readonly VertexInformation _PropertyDotType        = new VertexInformation((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.Type);
        private readonly VertexInformation _PropertyDotIsMandatory = new VertexInformation((long)BaseTypes.Property    , (long)AttributeDefinitions.IsMandatory);
        private readonly VertexInformation _PropertyDotInIndices   = new VertexInformation((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.InIndices);

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

        #region C'tor
		
        /// <summary>
        /// Creates a new instance of DBCreationManager.
        /// </summary>
        public DBCreationManager()
        {
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

            AddBaseTypes(myStore, creationDate);
            AddVertexTypes(myStore, creationDate);
            AddEdgeTypes(myStore, creationDate);
            AddIndices(myStore, creationDate);
        }

        #endregion

        #region Base types

        private void AddBaseTypes(IVertexStore myStore, long creationDate)
        {
            throw new NotImplementedException();
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

        private static void AddEdgeTypes(IVertexStore myStore, Int64 myCreationDate)
        {
            AddEdge(myStore, myCreationDate);
            AddWeightedEdge(myStore, myCreationDate);
        }

        private static void AddWeightedEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
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
            #region vertex type vertex

            Store(
                myStore,
                _Vertex,
                ResMgr.GetString("VertexComment"),
                myCreationDate,
                null,
                null,
                new Dictionary<long, IComparable>
                {
                    { (long)AttributeDefinitions.ID            , (long) BaseTypes.Vertex },
                    { (long)AttributeDefinitions.TypeID        , (long) BaseTypes.VertexType },
                    { (long)AttributeDefinitions.Name          , BaseTypes.Vertex.ToString() },
                    { (long)AttributeDefinitions.IsUserDefined , false },
                    { (long)AttributeDefinitions.IsSealed      , false },
                },
                null);

            #endregion

            #region property vertices

            #region UUID property

            Store(
                myStore,
                _VertexDotUUID,
                ResMgr.GetString("UUIDComment"),
                myCreationDate,
                new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { Tuple.Create((long)AttributeDefinitions.DefiningType, _EdgeEdgeType), _Vertex},
                    { Tuple.Create((long)AttributeDefinitions.Type, _EdgeEdgeType), _BaseTypeUInt64}
                },
                null,
                new Dictionary<long, IComparable>
                {

                },
                null);

            #endregion

            #endregion

        }

        private static void AddIndex(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddProperty(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddIncomingEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddOutgoingEdge(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddAttribute(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddEdgeType(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddVertexType(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        private static void AddBaseType(IVertexStore myStore, Int64 myCreationDate)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Store
		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myStore"></param>
        /// <param name="myVertexID"></param>
        /// <param name="myVertexTypeID"></param>
        /// <param name="myEdition"></param>
        /// <param name="myComment"></param>
        /// <param name="myEdges"></param>
        /// <param name="myStructuredProperties"></param>
        /// <param name="myUnstructuredProperties"></param>
        private static void Store(
            IVertexStore myStore,
            VertexInformation mySource,
            String myComment,
            Int64 myCreationDate, 
            Dictionary<Tuple<Int64, Int64>, VertexInformation> mySingleEdges,
            Dictionary<Tuple<Int64, Int64>, IEnumerable<VertexInformation>> myHyperEdges,
            Dictionary<Int64, IComparable> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties)
        {
            VertexAddDefinition def = new VertexAddDefinition(
                mySource.VertexID,
                mySource.VertexTypeID,
                mySource.VertexEditionName,
                CreateHyperEdgeDefinitions(myHyperEdges, mySource, myCreationDate),
                CreateSingleEdgeDefinitions(mySingleEdges, mySource, myCreationDate),
                null,
                myComment,
                myCreationDate,
                myCreationDate,
                myStructuredProperties,
                myUnstructuredProperties);

            myStore.AddVertex(def);
        }

        private static IEnumerable<SingleEdgeAddDefinition> CreateSingleEdgeDefinitions(
            Dictionary<Tuple<long, long>, VertexInformation> mySingleEdges, 
            VertexInformation mySource, 
            long myCreationDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myEdges"></param>
        /// <param name="myVertexID"></param>
        /// <param name="myVertexTypeID"></param>
        /// <param name="myCreationDate"></param>
        /// <returns></returns>
        private static IEnumerable<HyperEdgeAddDefinition> CreateHyperEdgeDefinitions(
            Dictionary<Tuple<Int64, Int64>, IEnumerable<VertexInformation>> myEdges,
            VertexInformation mySource,
            Int64 myCreationDate)
        {
            if (myEdges == null)
                return null;

            List<HyperEdgeAddDefinition> result = new List<HyperEdgeAddDefinition>(myEdges.Count);
            long edgeID;
            long edgeTypeID;
            foreach (var edge in myEdges)
            {
                edgeID = edge.Key.Item1;
                edgeTypeID = edge.Key.Item2;
                result.Add(
                    new HyperEdgeAddDefinition(
                        edgeID,
                        edgeTypeID,
                        mySource,
                        edge.Value.Select(x => new SingleEdgeAddDefinition(
                            edgeID, 
                            edgeTypeID, 
                            mySource,
                            x,
                            null, 
                            myCreationDate, 
                            myCreationDate, 
                            null, 
                            null)),
                        null,
                        myCreationDate,
                        myCreationDate,
                        null,
                        null
                    ));
            }

            return result;
        }
 
	    #endregion    

    }
}
