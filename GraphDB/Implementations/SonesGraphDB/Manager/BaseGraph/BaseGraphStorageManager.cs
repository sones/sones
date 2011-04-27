using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.VertexStore;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.Manager.BaseGraph
{
    /// <summary>
    /// This class eases the storage of vertices that are part of the base vertex types.
    /// </summary>
    internal static class BaseGraphStorageManager
    {
        #region Edge information

        private const Int64 _EdgeEdgeType = (long)BaseTypes.Edge;

        #region Attribute

        private static readonly Tuple<Int64, Int64> _EdgeAttributeDotDefiningType = Tuple.Create((long)AttributeDefinitions.DefiningType, _EdgeEdgeType);

        #endregion

        #region EdgeType

        private static readonly Tuple<Int64, Int64> _EdgeEdgeTypeDotParent = Tuple.Create((long)AttributeDefinitions.Parent, _EdgeEdgeType);

        #endregion

        #region IncomingEdge

        private static readonly Tuple<Int64, Int64> _EdgeIncomingEdgeDotRelatedEdge = Tuple.Create((long)AttributeDefinitions.RelatedEgde, _EdgeEdgeType);

        #endregion

        #region Index

        private static readonly Tuple<Int64, Int64> _EdgeIndexDotIndexedProperties = Tuple.Create((long)AttributeDefinitions.IndexedProperties, _EdgeEdgeType);
        private static readonly Tuple<Int64, Int64> _EdgeIndexDotDefiningVertexType = Tuple.Create((long)AttributeDefinitions.DefiningVertexType, _EdgeEdgeType);

        #endregion

        #region OutgoingEdge

        private static readonly Tuple<Int64, Int64> _EdgeOutgoingEdgeDotEdgeType = Tuple.Create((long)AttributeDefinitions.EdgeType, _EdgeEdgeType);
        private static readonly Tuple<Int64, Int64> _EdgeOutgoingEdgeDotSource = Tuple.Create((long)AttributeDefinitions.Source, _EdgeEdgeType);
        private static readonly Tuple<Int64, Int64> _EdgeOutgoingEdgeDotTarget = Tuple.Create((long)AttributeDefinitions.Target, _EdgeEdgeType);

        #endregion

        #region Property

        private static readonly Tuple<Int64, Int64> _EdgePropertyDotType = Tuple.Create((long)AttributeDefinitions.Type, _EdgeEdgeType);

        #endregion

        #region VertexType

        private static readonly Tuple<Int64, Int64> _EdgeVertexTypeDotParent = Tuple.Create((long)AttributeDefinitions.Parent, _EdgeEdgeType);
        private static readonly Tuple<Int64, Int64> _EdgeVertexTypeDotUniqueDefinitions = Tuple.Create((long)AttributeDefinitions.UniquenessDefinitions, _EdgeEdgeType);

        #endregion

        #endregion



        #region Store

        #region Outgoing edge

        
        public static void StoreOutgoingEdge(
            IVertexStore myStore,
            VertexInformation myVertex,
            AttributeDefinitions myAttribute,
            String myComment,
            Int64 myCreationDate,
            EdgeMultiplicity myMultiplicity,
            VertexInformation myDefiningType,
            VertexInformation myEdgeType,
            VertexInformation myTarget,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            StoreOutgoingEdge(myStore, myVertex, (long)myAttribute, myAttribute.ToString(), myComment, myCreationDate, myMultiplicity, myDefiningType, myEdgeType, myTarget, mySecurity, myTransaction);
        }

        public static void StoreOutgoingEdge(
            IVertexStore myStore,
            VertexInformation myVertex,
            long myID,
            String myName,
            String myComment,
            Int64 myCreationDate,
            EdgeMultiplicity myMultiplicity,
            VertexInformation myDefiningType,
            VertexInformation myEdgeType,
            VertexInformation myTarget,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { _EdgeAttributeDotDefiningType, myDefiningType },
                    { _EdgeOutgoingEdgeDotSource, myDefiningType },
                    { _EdgeOutgoingEdgeDotEdgeType, myEdgeType },
                    { _EdgeOutgoingEdgeDotTarget, myTarget }
                },
                null,
                new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, myID },
                    { (long) AttributeDefinitions.Name, myName },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                    { (long) AttributeDefinitions.Multiplicity, (byte) myMultiplicity },
                },
                null,
                mySecurity,
                myTransaction
            );
        }

        #endregion

        #region Incoming edge

        public static void StoreIncomingEdge(
            IVertexStore myStore,
            VertexInformation myVertex,
            AttributeDefinitions myAttribute,
            String myComment,
            Int64 myCreationDate,
            VertexInformation myDefiningType,
            VertexInformation myRelatedIncomingEdge,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            StoreIncomingEdge(myStore, myVertex, (long)myAttribute, myAttribute.ToString(), myComment, myCreationDate, myDefiningType, myRelatedIncomingEdge, mySecurity, myTransaction);
        }

        public static void StoreIncomingEdge(
            IVertexStore myStore,
            VertexInformation myVertex,
            long myID,
            String myName,
            String myComment,
            Int64 myCreationDate,
            VertexInformation myDefiningType,
            VertexInformation myRelatedIncomingEdge,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { _EdgeAttributeDotDefiningType, myDefiningType },
                    { _EdgeIncomingEdgeDotRelatedEdge, myRelatedIncomingEdge}
                },
                null,
                new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, myID },
                    { (long) AttributeDefinitions.Name, myName },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                },
                null,
                mySecurity,
                myTransaction);
        }

        #endregion

        #region Property

        public static void StoreProperty(
            IVertexStore myStore,
            VertexInformation myVertex,
            AttributeDefinitions myAttribute,
            String myComment,
            Int64 myCreationDate,
            bool myIsMandatory,
            PropertyMultiplicity myMultiplicity,
            String myDefaultValue,
            VertexInformation myDefiningType,
            VertexInformation myBasicType,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            StoreProperty(myStore, myVertex, (long)myAttribute, myAttribute.ToString(), myComment, myCreationDate, myIsMandatory, myMultiplicity, myDefaultValue, myDefiningType, myBasicType, mySecurity, myTransaction);
        }

        public static void StoreProperty(
            IVertexStore myStore,
            VertexInformation myVertex,
            long myID,
            String myName,
            String myComment,
            Int64 myCreationDate,
            bool myIsMandatory,
            PropertyMultiplicity myMultiplicity,
            String myDefaultValue,
            VertexInformation myDefiningType,
            VertexInformation myBasicType,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            var props = new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, myID},
                    { (long) AttributeDefinitions.Name, myName },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                    { (long) AttributeDefinitions.IsMandatory, myIsMandatory },
                    { (long) AttributeDefinitions.Multiplicity, (byte) myMultiplicity },
                };

            if (myDefaultValue != null)
                props.Add((long) AttributeDefinitions.DefaultValue, myDefaultValue);

            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { _EdgeAttributeDotDefiningType, myDefiningType },
                    { _EdgePropertyDotType, myBasicType },
                },
                null,
                props,
                null,
                mySecurity,
                myTransaction);

        }

        #endregion

        #region Binary Property

        public static void StoreBinaryProperty(
            IVertexStore myStore,
            VertexInformation myVertex,
            long myID,
            String myName,
            String myComment,
            Int64 myCreationDate,
            VertexInformation myDefiningType,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { _EdgeAttributeDotDefiningType, myDefiningType },
                },
                null,
                new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, myID},
                    { (long) AttributeDefinitions.Name, myName },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                },
                null,
                mySecurity,
                myTransaction);
        }

        #endregion

        #region Basic type

        public static IVertex StoreBasicType(
            IVertexStore myStore,
            VertexInformation myVertex,
            BasicTypes myType,
            String myComment,
            Int64 myCreationDate,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            return Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                null,
                null,
                new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, (long) myType },
                    { (long) AttributeDefinitions.Name, myType.ToString() },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                    { (long) AttributeDefinitions.IsAbstract, false },
                    { (long) AttributeDefinitions.IsSealed, true },
                    //{ (long) AttributeDefinitions.Behaviour, null },
                },
                null,
                mySecurity,
                myTransaction);

        }

        #endregion

        #region Vertex type

        public static IVertex StoreVertexType(
            IVertexStore myStore,
            VertexInformation myVertex,
            BaseTypes myType,
            String myComment,
            Int64 myCreationDate,
            bool myIsAbstract,
            bool myIsSealed,
            VertexInformation? myParent,
            IEnumerable<VertexInformation> myUniques,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            return StoreVertexType(myStore, myVertex, myType.ToString(), myComment, myCreationDate, myIsAbstract, myIsSealed, myParent, myUniques, mySecurity, myTransaction);
        }

        public static IVertex StoreVertexType(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            String myComment,
            Int64 myCreationDate,
            bool myIsAbstract,
            bool myIsSealed,
            VertexInformation? myParent,
            IEnumerable<VertexInformation> myUniques,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            return Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                (myParent == null)
                    ? null
                    : new Dictionary<Tuple<long, long>, VertexInformation>
                    {
                        { _EdgeVertexTypeDotParent, myParent.Value },
                    },
                new Dictionary<Tuple<long, long>, IEnumerable<Library.Commons.VertexStore.Definitions.VertexInformation>>
                {
                    {_EdgeVertexTypeDotUniqueDefinitions, myUniques }
                },
                new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, myVertex.VertexID },
                    { (long) AttributeDefinitions.Name, myName },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                    { (long) AttributeDefinitions.IsAbstract, myIsAbstract },
                    { (long) AttributeDefinitions.IsSealed, myIsSealed },
                    //{ (long) AttributeDefinitions.Behaviour, null },
                },
                null,
                mySecurity,
                myTransaction);

        }

        #endregion

        #region Edge type

        public static void StoreEdgeType(
            IVertexStore myStore,
            VertexInformation myVertex,
            BaseTypes myType,
            String myComment,
            Int64 myCreationDate,
            bool myIsAbstract,
            bool myIsSealed,
            VertexInformation? myParent,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                (myParent == null)
                    ? null
                    : new Dictionary<Tuple<long, long>, VertexInformation>
                    {
                        { _EdgeVertexTypeDotParent, myParent.Value },
                    },
                null,
                new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, (long) myType },
                    { (long) AttributeDefinitions.Name, myType.ToString() },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                    { (long) AttributeDefinitions.IsAbstract, myIsAbstract },
                    { (long) AttributeDefinitions.IsSealed, myIsSealed },
                    //{ (long) AttributeDefinitions.Behaviour, null },
                },
                null,
                mySecurity,
                myTransaction);
        }

        #endregion

        #region Index

        public static void StoreIndex(
            IVertexStore myStore,
            VertexInformation myVertex,
            BaseTypes myType,
            String myComment,
            Int64 myCreationDate,
            String myIndexClass,
            bool myIsSingleValue,
            bool myIsRange,
            bool myIsVersioned,
            VertexInformation myDefiningVertexType,
            IEnumerable<VertexInformation> myIndexedProperties,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                new Dictionary<Tuple<long, long>, Library.Commons.VertexStore.Definitions.VertexInformation>
                {
                    { _EdgeIndexDotDefiningVertexType, myDefiningVertexType }
                },
                new Dictionary<Tuple<long, long>, IEnumerable<Library.Commons.VertexStore.Definitions.VertexInformation>>
                {
                    { _EdgeIndexDotIndexedProperties, myIndexedProperties }
                },
                new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, (long) myType },
                    { (long) AttributeDefinitions.Name, myType.ToString() },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                    { (long) AttributeDefinitions.IndexClass, myIndexClass },
                    { (long) AttributeDefinitions.IsSingleValue, myIsSingleValue},
                    { (long) AttributeDefinitions.IsRange, myIsRange },
                    { (long) AttributeDefinitions.IsUserDefined, myIsVersioned },
                },
                null,
                mySecurity,
                myTransaction);
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
        private static IVertex Store(
            IVertexStore myStore,
            VertexInformation mySource,
            String myComment,
            Int64 myCreationDate,
            Dictionary<Tuple<Int64, Int64>, VertexInformation> mySingleEdges,
            Dictionary<Tuple<Int64, Int64>, IEnumerable<VertexInformation>> myHyperEdges,
            Dictionary<Int64, IComparable> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
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

            return myStore.AddVertex(mySecurity, myTransaction, def);
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

        #endregion
    }
}
