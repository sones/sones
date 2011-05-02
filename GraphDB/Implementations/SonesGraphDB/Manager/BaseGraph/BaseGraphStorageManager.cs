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
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ErrorHandling;
using sones.Library.Commons.VertexStore.Definitions.Update;
using sones.GraphDB.Index;

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

        /// <summary>
        /// Gets the vertex that represents the parent type.
        /// </summary>
        /// <returns>An IVertex that represents the parent type, if existing otherwise <c>NULL</c>.</returns>
        public static IVertex GetParent(IVertex myVertex)
        {
            return myVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.Parent).GetTargetVertex();
        }


        public static Type GetBaseType(String myTypeName)
        {
            BasicTypes type;
            if (!Enum.TryParse(myTypeName, out type))
                throw new NotImplementedException("User defined base types are not implemented yet.");

            return GetBaseType(type);
        }

        public static bool GetIsUserDefined(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>((long)AttributeDefinitions.IsUserDefined);
        }

        public static string GetComment(IVertex myVertex)
        {
            if (myVertex.HasProperty((long)AttributeDefinitions.Comment))
            {
                return myVertex.GetPropertyAsString((long)AttributeDefinitions.Comment);                
            }

            return null;
        }

        public static long GetID(IVertex myVertex)
        {
            return myVertex.GetProperty<long>((long)AttributeDefinitions.ID);
        }

        public static String GetName(IVertex myVertex)
        {
            return myVertex.GetPropertyAsString((long)AttributeDefinitions.Name);
        }

        public static bool GetIsSealed(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>((long)AttributeDefinitions.IsSealed);
        }

        public static bool GetIsAbstract(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>((long)AttributeDefinitions.IsAbstract);
        }



        #region Outgoing edge

        /// <summary>
        /// Transforms an IVertex in an outgoing edge definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an outgoing edge definition.</param>
        /// <returns>An outgoing edge definition.</returns>
        public static IOutgoingEdgeDefinition CreateOutgoingEdgeDefinition(IVertex myOutgoingEdgeVertex, IVertexType myRelatedType = null)
        {
            var attributeID = GetID(myOutgoingEdgeVertex);
            var edgeType = GetEdgeType(myOutgoingEdgeVertex);
            var name = GetName(myOutgoingEdgeVertex);
            var target = GetTargetVertexType(myOutgoingEdgeVertex);
            var multiplicity = GetEdgeMultiplicity(myOutgoingEdgeVertex);
            var relatedType = myRelatedType ?? GetDefiningType(myOutgoingEdgeVertex) as VertexType;

            return new OutgoingEdgeDefinition
            {
                AttributeID = attributeID,
                EdgeType = edgeType,
                Multiplicity = multiplicity,
                Name = name,
                SourceVertexType = myRelatedType,
                TargetVertexType = target,
                RelatedType = relatedType,
                
            };
        }

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

        /// <summary>
        /// Transforms an IVertex in an incoming edge definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an incoming edge definition.</param>
        /// <returns>An incoming edge definition.</returns>
        public static IIncomingEdgeDefinition CreateIncomingEdgeDefinition(IVertex myVertex, IBaseType myDefiningType = null)
        {
            var attributeID = GetID(myVertex);
            var name = GetName(myVertex);
            var related = GetRelatedOutgoingEdgeDefinition(myVertex);
            var definingType = myDefiningType ?? GetDefiningType(myVertex);

            return new IncomingEdgeDefinition
            {
                AttributeID = attributeID,
                Name = name,
                RelatedEdgeDefinition = related,
                RelatedType = definingType,
            };
        }


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

        /// <summary>
        /// Transforms an IVertex in a property definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents a property definition.</param>
        /// <returns>A property definition.</returns>
        public static IPropertyDefinition CreatePropertyDefinition(IVertex myVertex, IBaseType myDefiningType = null)
        {
            var attributeID = GetID(myVertex);
            var baseType = GetBaseType(myVertex);
            var isMandatory = GetIsMandatory(myVertex);
            var multiplicity = GetPropertyMultiplicity(myVertex);
            var name = GetName(myVertex);
            var defaultValue = GetDefaultValue(myVertex, baseType);
            var definingType = myDefiningType ?? GetDefiningType(myVertex);

            return new PropertyDefinition
            {
                AttributeID = attributeID,
                BaseType = baseType,
                IsMandatory = isMandatory,
                Multiplicity = multiplicity,
                Name = name,
                RelatedType = definingType,
                DefaultValue = defaultValue
            };
        }


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

        /// <summary>
        /// Transforms an IVertex in a binary property definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents a property definition.</param>
        /// <returns>A property definition.</returns>
        public static IBinaryPropertyDefinition CreateBinaryPropertyDefinition(IVertex myVertex, IVertexType myDefiningType = null)
        {
            var attributeID = GetID(myVertex);
            var name = GetName(myVertex);
            var definingType = myDefiningType ?? GetDefiningType(myVertex) as IVertexType;

            return new BinaryPropertyDefinition
            {
                AttributeID = attributeID,
                Name = name,
                RelatedType = definingType
            };
        }

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
                myUniques == (null)
                    ? null
                    : new Dictionary<Tuple<long, long>, IEnumerable<Library.Commons.VertexStore.Definitions.VertexInformation>>
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
        
        public static IIndexDefinition CreateIndexDefinition(IVertex myIndexVertex, IVertexType myDefiningVertexType = null)
        {
            var id = GetID(myIndexVertex);
            var props = GetIndexedProperties(myIndexVertex);
            var typeName = GetIndexTypeName(myIndexVertex);
            var isUserDefined = GetIsUserDefined(myIndexVertex);
            var name = GetName(myIndexVertex);
            myDefiningVertexType = myDefiningVertexType ?? GetDefiningVertexType(myIndexVertex);

            return new IndexDefinition
            {
                ID = id,
                IndexedProperties = props,
                IndexTypeName = typeName,
                IsUserdefined = isUserDefined,
                Name = name,
                VertexType = myDefiningVertexType,
            };
        }

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
            IList<VertexInformation> myIndexedProperties,
            SecurityToken mySecurity,
            TransactionToken myTransaction)
        {
            var props = new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.ID, (long) myType },
                    { (long) AttributeDefinitions.Name, myType.ToString() },
                    { (long) AttributeDefinitions.IsUserDefined, false },
                    { (long) AttributeDefinitions.IsSingleValue, myIsSingleValue},
                    { (long) AttributeDefinitions.IsRange, myIsRange },
                    { (long) AttributeDefinitions.IsVersioned, myIsVersioned },
                };

            if (myIndexClass == null)
                props.Add((long) AttributeDefinitions.IndexClass, myIndexClass);

            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { _EdgeIndexDotDefiningVertexType, myDefiningVertexType }
                },
                new Dictionary<Tuple<long, long>, IEnumerable<VertexInformation>>
                {
                    { _EdgeIndexDotIndexedProperties, myIndexedProperties }
                },
                props,
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
            if (mySingleEdges == null)
                return null;

            List<SingleEdgeAddDefinition> result = new List<SingleEdgeAddDefinition>(mySingleEdges.Count);
            long edgeID;
            long edgeTypeID;
            foreach (var edge in mySingleEdges)
            {
                edgeID = edge.Key.Item1;
                edgeTypeID = edge.Key.Item2;

                result.Add(
                    new SingleEdgeAddDefinition(
                        edgeID,
                        edgeTypeID,
                        mySource,
                        edge.Value,
                        null,
                        myCreationDate,
                        myCreationDate,
                        null,
                        null));
            }

            return result;
                    
                
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

                var singleEdges = (edge.Value == null)
                ? null
                : edge.Value.Select((vertexInfo, pos) => new SingleEdgeAddDefinition(
                    edgeID,
                    edgeTypeID,
                    mySource,
                    vertexInfo,
                    null,
                    myCreationDate,
                    myCreationDate,
                    (edge.Value is IList<VertexInformation>)
                        ? new Dictionary<long, IComparable> 
                            {
                                { (long)AttributeDefinitions.Order, pos },
                            }
                        : null,
                    null)).ToArray();

                result.Add(
                    new HyperEdgeAddDefinition(
                        edgeID,
                        edgeTypeID,
                        mySource,
                        singleEdges,
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

        /// <summary>
        /// Gets the target vertex of an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdge">A vertex that represents an outgoing edge.</param>
        /// <returns>The target vertex type of the outgoing edge.</returns>
        private static IVertexType GetTargetVertexType(IVertex myOutgoingEdge)
        {
            var vertex = myOutgoingEdge.GetOutgoingSingleEdge((long)AttributeDefinitions.Target).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An outgoing edge has no vertex that represents its target vertex type.");

            return new VertexType(vertex);
        }

        private static IBaseType GetDefiningType(IVertex myAttributeVertex)
        {
            var edge = myAttributeVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.DefiningType);

            if (edge == null)
                throw new UnknownDBException("An attribute has no vertex that represents its defining type.");

            var vertex = edge.GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An attribute has no vertex that represents its defining type.");

            switch (vertex.VertexTypeID)
            {
                case (long)BaseTypes.VertexType: return new VertexType(vertex);
                case (long)BaseTypes.EdgeType: return new EdgeType(vertex);
                default: throw new UnknownDBException("The defining attribute vertex neither point to an edge nor to an vertex type vertex.");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="myOutgoingEdge">A vertex that represents an outgoing edge.</param>
        /// <returns>The edge type of the outgoing edge.</returns>
        private static IEdgeType GetEdgeType(IVertex myOutgoingEdge)
        {
            var vertex = myOutgoingEdge.GetOutgoingSingleEdge((long)AttributeDefinitions.EdgeType).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An outgoing edge has no vertex that represents its edge type.");

            return new EdgeType(vertex);
        }

        
        private static EdgeMultiplicity GetEdgeMultiplicity(IVertex myOutgoingEdgeVertex)
        {
            var multID = myOutgoingEdgeVertex.GetProperty<Byte>((long)AttributeDefinitions.Multiplicity);

            if (!Enum.IsDefined(typeof(EdgeMultiplicity), multID))
                throw new UnknownDBException("The value for the edge multiplicity is incorrect.");

            return (EdgeMultiplicity)multID;

        }

        private static PropertyMultiplicity GetPropertyMultiplicity(IVertex myVertex)
        {
            var multID = myVertex.GetProperty<Byte>((long)AttributeDefinitions.Multiplicity);

            if (!Enum.IsDefined(typeof(PropertyMultiplicity), multID))
                throw new UnknownDBException("The value for the property multiplicity is incorrect.");

            return (PropertyMultiplicity)multID;
        }

        private static bool GetIsMandatory(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>((long)AttributeDefinitions.IsMandatory);
        }

        private static IComparable GetDefaultValue(IVertex myPropertyVertex, Type myPropertyType)
        {
            if (!myPropertyVertex.HasProperty((long)AttributeDefinitions.DefaultValue))
                return null;

            var val = myPropertyVertex.GetPropertyAsString((long)AttributeDefinitions.DefaultValue);

            return (IComparable)Convert.ChangeType(val, myPropertyType);
        }

        private static Type GetBaseType(BasicTypes type)
        {
            switch (type)
            {
                case BasicTypes.Boolean: return typeof(Boolean);
                case BasicTypes.Byte: return typeof(Byte);
                case BasicTypes.Char: return typeof(Char);
                case BasicTypes.DateTime: return typeof(DateTime);
                case BasicTypes.Double: return typeof(Double);
                case BasicTypes.Int16: return typeof(Int16);
                case BasicTypes.Int32: return typeof(Int32);
                case BasicTypes.Int64: return typeof(Int64);
                case BasicTypes.SByte: return typeof(SByte);
                case BasicTypes.Single: return typeof(Single);
                case BasicTypes.String: return typeof(String);
                case BasicTypes.TimeSpan: return typeof(TimeSpan);
                case BasicTypes.UInt16: return typeof(UInt16);
                case BasicTypes.UInt32: return typeof(UInt32);
                case BasicTypes.UInt64: return typeof(UInt64);
                default: throw new UnknownDBException("BasicTypes enumeration was modified, but this function was not adapted.");
            }
        }

        private static Type GetBaseType(IVertex myVertex)
        {
            var typeID = GetID(myVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.Type).GetTargetVertex());
            if (!Enum.IsDefined(typeof(BasicTypes), typeID))
                throw new NotImplementedException("User defined base types are not implemented yet.");

            BasicTypes type = (BasicTypes)typeID;
            return GetBaseType(type);
        }

        /// <summary>
        /// Creates an outgoing edge definition from a vertex that represents an incoming edge definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an incoming edge definition.</param>
        /// <returns>An outgoing edge definition.</returns>
        private static IOutgoingEdgeDefinition GetRelatedOutgoingEdgeDefinition(IVertex myVertex)
        {
            var vertex = myVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.RelatedEgde).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An incoming edge definition has no vertex that represents its related outgoing edge definition.");

            return CreateOutgoingEdgeDefinition(vertex);
        }

        private static IVertexType GetDefiningVertexType(IVertex myIndexVertex)
        {
            var edge = myIndexVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.DefiningVertexType);

            if (edge == null)
                throw new UnknownDBException("An index has no vertex that represents its defining vertex type.");

            var vertex = edge.GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An index has no vertex that represents its defining vertex type.");

            return new VertexType(vertex);
        }

        private static String GetIndexTypeName(IVertex myIndexVertex)
        {
            return myIndexVertex.GetPropertyAsString((long)AttributeDefinitions.IndexClass);
        }

        private static IList<IPropertyDefinition> GetIndexedProperties(IVertex myIndexVertex)
        {
            var edge = myIndexVertex.GetOutgoingHyperEdge((long)AttributeDefinitions.IndexedProperties);
            if (edge == null)
                throw new UnknownDBException("An index has no vertex that represents its indexed properties.");

            
            var vertices = edge.GetTargetVertices();
            if (vertices == null)
                throw new UnknownDBException("An index has no vertex that represents its indexed properties.");

            vertices = vertices.OrderBy(x => x.GetProperty<int>((long)AttributeDefinitions.Order));

            return vertices.Select(x=> CreatePropertyDefinition(x)).ToArray();

        }




        #endregion

        public static IEnumerable<IBinaryPropertyDefinition> GetBinaryPropertiesFromFS(IVertex myTypeVertex, IVertexType myBaseType = null)
        {
            return GetAttributeVertices(myTypeVertex, (long)BaseTypes.BinaryProperty).Select(x => CreateBinaryPropertyDefinition(x, myBaseType));
        }

        public static IEnumerable<IPropertyDefinition> GetPropertiesFromFS(IVertex myTypeVertex, IBaseType myBaseType = null)
        {
            return GetAttributeVertices(myTypeVertex, (long)BaseTypes.Property).Select(x => CreatePropertyDefinition(x, myBaseType));
        }

        public static IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgesFromFS(IVertex myTypeVertex, IVertexType myBaseType = null)
        {
            return GetAttributeVertices(myTypeVertex, (long)BaseTypes.OutgoingEdge).Select(x => CreateOutgoingEdgeDefinition(x, myBaseType));
        }

        public static IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgesFromFS(IVertex myTypeVertex, IVertexType myBaseType = null)
        {
            return GetAttributeVertices(myTypeVertex, (long)BaseTypes.IncomingEdge).Select(x => CreateIncomingEdgeDefinition(x, myBaseType));
        }

        private static IEnumerable<IVertex> GetAttributeVertices(IVertex myTypeVertex, long myAttributeVertexID)
        {
            if (myTypeVertex.HasIncomingVertices(myAttributeVertexID, (long)AttributeDefinitions.DefiningType))
            {
                var vertices = myTypeVertex.GetIncomingVertices(myAttributeVertexID, (long)AttributeDefinitions.DefiningType);
                foreach (var vertex in vertices)
                {
                    if (vertex == null)
                        throw new UnknownDBException("An element in attributes list is NULL.");

                    yield return vertex;
                }
            }
            yield break;
        }

    }
}
