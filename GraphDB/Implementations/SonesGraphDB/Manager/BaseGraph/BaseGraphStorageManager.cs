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
using sones.GraphDB.Manager.TypeManagement;

namespace sones.GraphDB.Manager.BaseGraph
{
    /// <summary>
    /// This class eases the storage of vertices that are part of the base vertex types.
    /// </summary>
    public class BaseGraphStorageManager : IManager
    {
        #region Manager

        private IMetaManager    _metaManager;
        private BaseTypeManager _baseTypeManager;

        #endregion

        #region Edge information

        private const Int64 _EdgeEdgeType = (long)BaseTypes.Edge;

        #region Attribute

        private  readonly Tuple<Int64, Int64> _EdgeAttributeDotDefiningType = Tuple.Create((long)AttributeDefinitions.AttributeDotDefiningType, _EdgeEdgeType);

        #endregion

        #region EdgeType

        private  readonly Tuple<Int64, Int64> _EdgeEdgeTypeDotParent = Tuple.Create((long)AttributeDefinitions.EdgeTypeDotParent, _EdgeEdgeType);

        #endregion

        #region IncomingEdge

        private  readonly Tuple<Int64, Int64> _EdgeIncomingEdgeDotRelatedEdge = Tuple.Create((long)AttributeDefinitions.IncomingEdgeDotRelatedEgde, _EdgeEdgeType);

        #endregion

        #region Index

        private  readonly Tuple<Int64, Int64> _EdgeIndexDotIndexedProperties  = Tuple.Create((long)AttributeDefinitions.IndexDotIndexedProperties, _EdgeEdgeType);
        private  readonly Tuple<Int64, Int64> _EdgeIndexDotDefiningVertexType = Tuple.Create((long)AttributeDefinitions.IndexDotDefiningVertexType, _EdgeEdgeType);
        private  readonly Tuple<Int64, Int64> _EdgeIndexDotSource             = Tuple.Create((long)AttributeDefinitions.IndexDotSourceIndex, _EdgeEdgeType);

        #endregion

        #region OutgoingEdge

        private  readonly Tuple<Int64, Int64> _EdgeOutgoingEdgeDotEdgeType      = Tuple.Create((long)AttributeDefinitions.OutgoingEdgeDotEdgeType, _EdgeEdgeType);
        private  readonly Tuple<Int64, Int64> _EdgeOutgoingEdgeDotInnerEdgeType = Tuple.Create((long)AttributeDefinitions.OutgoingEdgeDotInnerEdgeType, _EdgeEdgeType);
        private  readonly Tuple<Int64, Int64> _EdgeOutgoingEdgeDotSource        = Tuple.Create((long)AttributeDefinitions.OutgoingEdgeDotSource, _EdgeEdgeType);
        private  readonly Tuple<Int64, Int64> _EdgeOutgoingEdgeDotTarget        = Tuple.Create((long)AttributeDefinitions.OutgoingEdgeDotTarget, _EdgeEdgeType);

        #endregion

        #region Property

        private  readonly Tuple<Int64, Int64> _EdgePropertyDotBaseType = Tuple.Create((long)AttributeDefinitions.PropertyDotBaseType, _EdgeEdgeType);

        #endregion

        #region VertexType

        private  readonly Tuple<Int64, Int64> _EdgeVertexTypeDotParent            = Tuple.Create((long)AttributeDefinitions.VertexTypeDotParent, _EdgeEdgeType);
        private  readonly Tuple<Int64, Int64> _EdgeVertexTypeDotUniqueDefinitions = Tuple.Create((long)AttributeDefinitions.VertexTypeDotUniquenessDefinitions, _EdgeEdgeType);

        #endregion

        #endregion

        #region Store

        /// <summary>
        /// Gets the vertex that represents the parent type.
        /// </summary>
        /// <returns>An IVertex that represents the parent type, if existing otherwise <c>NULL</c>.</returns>
        public  IVertex GetParentVertexType(IVertex myVertex)
        {
            return myVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.VertexTypeDotParent).GetTargetVertex();
        }
        
        public Type GetBaseType(String myTypeName)
        {
            return _baseTypeManager.GetBaseType(myTypeName);
        }

        #region Outgoing edge

        /// <summary>
        /// Transforms an IVertex in an outgoing edge definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an outgoing edge definition.</param>
        /// <returns>An outgoing edge definition.</returns>
        public  IOutgoingEdgeDefinition CreateOutgoingEdgeDefinition(IVertex myOutgoingEdgeVertex, IVertexType myRelatedType = null)
        {
            var edgeType = GetEdgeType(myOutgoingEdgeVertex);
            var name = GetAttributeDotName(myOutgoingEdgeVertex);
            var target = GetTargetVertexType(myOutgoingEdgeVertex);
            var multiplicity = GetEdgeMultiplicity(myOutgoingEdgeVertex);
            var relatedType = myRelatedType ?? GetDefiningType(myOutgoingEdgeVertex) as VertexType;
            var innerEdgeType = (multiplicity == EdgeMultiplicity.MultiEdge)
                ? GetInnerEdgeType(myOutgoingEdgeVertex)
                : null;
            var isUserDefined = GetAttributeDotIsUserDefined(myOutgoingEdgeVertex);

            return new OutgoingEdgeDefinition
            {
                EdgeType = edgeType,
                InnerEdgeType = innerEdgeType,
                Multiplicity = multiplicity,
                Name = name,
                SourceVertexType = relatedType,
                TargetVertexType = target,
                RelatedType = relatedType,
                ID = myOutgoingEdgeVertex.VertexID,
                IsUserDefined = isUserDefined,
            };
        }

        private  String GetAttributeDotName(IVertex myAttributeVertex)
        {
            return myAttributeVertex.GetPropertyAsString((long)AttributeDefinitions.AttributeDotName);
        }

        public  void StoreOutgoingEdge(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            String myComment,
            bool myIsUserDefined,
            Int64 myCreationDate,
            EdgeMultiplicity myMultiplicity,
            VertexInformation myDefiningType,
            VertexInformation myEdgeType,
            VertexInformation? myInnerEdgeType, //not mandatory, might be null
            VertexInformation myTarget,
            SecurityToken mySecurity,
            Int64 myTransaction)
        {
            var singleEdges = new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { _EdgeAttributeDotDefiningType, myDefiningType },
                    { _EdgeOutgoingEdgeDotSource, myDefiningType },
                    { _EdgeOutgoingEdgeDotEdgeType, myEdgeType },
                    { _EdgeOutgoingEdgeDotTarget, myTarget }
                };

            if (myMultiplicity == EdgeMultiplicity.MultiEdge)
            {
                singleEdges.Add(_EdgeOutgoingEdgeDotInnerEdgeType, myInnerEdgeType.Value);
            }

            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                singleEdges,
                null,
                new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.AttributeDotName, myName },
                    { (long) AttributeDefinitions.AttributeDotIsUserDefined, myIsUserDefined },
                    { (long) AttributeDefinitions.OutgoingEdgeDotMultiplicity, (byte) myMultiplicity },
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
        public  IIncomingEdgeDefinition CreateIncomingEdgeDefinition(IVertex myVertex, IBaseType myDefiningType = null)
        {
            var attributeID = GetUUID(myVertex);
            var name = GetAttributeDotName(myVertex);
            var related = GetRelatedOutgoingEdgeDefinition(myVertex);
            var definingType = myDefiningType ?? GetDefiningType(myVertex);
            var isUserDefined = GetAttributeDotIsUserDefined(myVertex);

            return new IncomingEdgeDefinition
            {
                Name = name,
                RelatedEdgeDefinition = related,
                RelatedType = definingType,
                ID = myVertex.VertexID,
                IsUserDefined = isUserDefined,
            };
        }


        public  void StoreIncomingEdge(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            String myComment,
            bool myIsUserDefined,
            Int64 myCreationDate,
            VertexInformation myDefiningType,
            VertexInformation myRelatedIncomingEdge,
            SecurityToken mySecurity,
            Int64 myTransaction)
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
                    { (long) AttributeDefinitions.AttributeDotName, myName },
                    { (long) AttributeDefinitions.AttributeDotIsUserDefined, myIsUserDefined },
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
        public  IPropertyDefinition CreatePropertyDefinition(IVertex myVertex, IBaseType myDefiningType = null)
        {
            var attributeID = GetUUID(myVertex);
            var baseType = GetBaseType(myVertex);
            var isMandatory = GetIsMandatory(myVertex);
            var multiplicity = GetPropertyMultiplicity(myVertex);
            var name = GetAttributeDotName(myVertex);
            var defaultValue = GetDefaultValue(myVertex, baseType);
            var definingType = myDefiningType ?? GetDefiningType(myVertex);
            var inIndices = GetInIndices(myVertex);
            var isUserDefined = GetAttributeDotIsUserDefined(myVertex);

            return new PropertyDefinition
            {
                BaseType = baseType,
                IsMandatory = isMandatory,
                Multiplicity = multiplicity,
                Name = name,
                RelatedType = definingType,
                DefaultValue = defaultValue,
                InIndices = inIndices,
                ID = myVertex.VertexID,
                IsUserDefined = isUserDefined,
            };
        }

        public  void StoreProperty(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            String myComment,
            Int64 myCreationDate,
            bool myIsMandatory,
            PropertyMultiplicity myMultiplicity,
            String myDefaultValue,
            bool myIsUserDefined,
            VertexInformation myDefiningType,
            VertexInformation myBasicType,
            SecurityToken mySecurity,
            Int64 myTransaction)
        {
            var props = new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.AttributeDotName, myName },
                    { (long) AttributeDefinitions.AttributeDotIsUserDefined, myIsUserDefined },
                    { (long) AttributeDefinitions.PropertyDotIsMandatory, myIsMandatory },
                    { (long) AttributeDefinitions.PropertyDotMultiplicity, (byte) myMultiplicity },
                };

            if (myDefaultValue != null)
                props.Add((long) AttributeDefinitions.PropertyDotDefaultValue, myDefaultValue);

            Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { _EdgeAttributeDotDefiningType, myDefiningType },
                    { _EdgePropertyDotBaseType, myBasicType },
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
        public  IBinaryPropertyDefinition CreateBinaryPropertyDefinition(IVertex myVertex, IVertexType myDefiningType = null)
        {
            var attributeID = GetUUID(myVertex);
            var name = GetAttributeDotName(myVertex);
            var definingType = myDefiningType ?? GetDefiningType(myVertex) as IVertexType;
            var isUserDefined = GetAttributeDotIsUserDefined(myVertex);

            return new BinaryPropertyDefinition
            {
                IsUserDefined = isUserDefined,
                Name = name,
                RelatedType = definingType,
                ID = myVertex.VertexID,
            };
        }

        public  void StoreBinaryProperty(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            String myComment,
            bool myIsUserDefined,
            Int64 myCreationDate,
            VertexInformation myDefiningType,
            SecurityToken mySecurity,
            Int64 myTransaction)
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
                    { (long) AttributeDefinitions.AttributeDotName, myName },
                    { (long) AttributeDefinitions.AttributeDotIsUserDefined, myIsUserDefined },
                },
                null,
                mySecurity,
                myTransaction);
        }

        #endregion

        #region Basic type

        public  IVertex StoreBasicType(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            bool myIsUserDefined,
            String myComment,
            Int64 myCreationDate,
            SecurityToken mySecurity,
            Int64 myTransaction)
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
                    { (long) AttributeDefinitions.BaseTypeDotName, myName },
                    { (long) AttributeDefinitions.BaseTypeDotIsUserDefined, myIsUserDefined },
                    { (long) AttributeDefinitions.BaseTypeDotIsAbstract, false },
                    { (long) AttributeDefinitions.BaseTypeDotIsSealed, true },
                    //{ (long) AttributeDefinitions.Behaviour, null },
                },
                null,
                mySecurity,
                myTransaction);

        }

        #endregion

        #region Vertex type

        public IVertex StoreVertexType(
            IVertexStore myStore,
            VertexInformation myVertex,
            BaseTypes myType,
            String myComment,
            Int64 myCreationDate,
            bool myIsAbstract,
            bool myIsSealed,
            bool myIsUserDefined,
            VertexInformation? myParent,
            IEnumerable<VertexInformation> myUniques,
            SecurityToken mySecurity,
            Int64 myTransaction)
        {
            return StoreVertexType(myStore, myVertex, myType.ToString(), myComment, myCreationDate, myIsAbstract, myIsSealed, myIsUserDefined, myParent, myUniques, mySecurity, myTransaction);
        }

        public IVertex StoreVertexType(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            String myComment,
            Int64 myCreationDate,
            bool myIsAbstract,
            bool myIsSealed,
            bool myIsUserDefined,
            VertexInformation? myParent,
            IEnumerable<VertexInformation> myUniques,
            SecurityToken mySecurity,
            Int64 myTransaction)
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
                    { (long) AttributeDefinitions.BaseTypeDotName, myName },
                    { (long) AttributeDefinitions.BaseTypeDotIsUserDefined, myIsUserDefined },
                    { (long) AttributeDefinitions.BaseTypeDotIsAbstract, myIsAbstract },
                    { (long) AttributeDefinitions.BaseTypeDotIsSealed, myIsSealed },
                    //{ (long) AttributeDefinitions.Behaviour, null },
                },
                null,
                mySecurity,
                myTransaction);

        }

        #endregion

        #region Edge type

        public IVertex StoreEdgeType(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            String myComment,
            Int64 myCreationDate,
            bool myIsAbstract,
            bool myIsSealed,
            bool myIsUserDefined,
            VertexInformation? myParent,
            SecurityToken mySecurity,
            Int64 myTransaction)
        {
            return Store(myStore,
                            myVertex,
                            myComment,
                            myCreationDate,
                            (myParent == null)
                                ? null
                                : new Dictionary<Tuple<long, long>, VertexInformation>
                                {
                                    { _EdgeEdgeTypeDotParent, myParent.Value },
                                },
                            null,
                            new Dictionary<long, IComparable>
                            {
                                { (long) AttributeDefinitions.BaseTypeDotName, myName },
                                { (long) AttributeDefinitions.BaseTypeDotIsUserDefined, myIsUserDefined },
                                { (long) AttributeDefinitions.BaseTypeDotIsAbstract, myIsAbstract },
                                { (long) AttributeDefinitions.BaseTypeDotIsSealed, myIsSealed },
                                //{ (long) AttributeDefinitions.Behaviour, null },
                            },
                            null,
                            mySecurity,
                            myTransaction);
        }

        #endregion

        #region Index
        
        public IIndexDefinition CreateIndexDefinition(IVertex myIndexVertex, IVertexType myDefiningVertexType = null)
        {
            var id = GetUUID(myIndexVertex);
            var props = GetIndexedProperties(myIndexVertex);
            var typeName = GetIndexTypeName(myIndexVertex);
            var edition = myIndexVertex.EditionName;
            var isUserDefined = GetIndexDotIsUserDefined(myIndexVertex);
            var name = GetIndexDotName(myIndexVertex);
            var range = myIndexVertex.GetProperty<bool>((long)AttributeDefinitions.IndexDotIsRange);
            var version = myIndexVertex.GetProperty<bool>((long)AttributeDefinitions.IndexDotIsVersioned);
            var sourceIndex = (myIndexVertex.HasOutgoingEdge((long)AttributeDefinitions.IndexDotSourceIndex))
                ? CreateIndexDefinition(myIndexVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.IndexDotSourceIndex).GetTargetVertex())
                : null;

            myDefiningVertexType = myDefiningVertexType ?? GetDefiningVertexType(myIndexVertex);

            return new IndexDefinition
            {
                ID = id,
                IndexedProperties = props,
                Edition = edition,
                IndexTypeName = typeName,
                IsUserdefined = isUserDefined,
                Name = name,
                VertexType = myDefiningVertexType,
                IsRange = range,
                IsVersioned = version,
                SourceIndex = sourceIndex,
            };
        }

        private bool GetIndexDotIsUserDefined(IVertex myIndexVertex)
        {
            return myIndexVertex.GetProperty<bool>((long)AttributeDefinitions.IndexDotIsUserDefined);
        }

        private String GetIndexDotName(IVertex myIndexVertex)
        {
            return myIndexVertex.GetPropertyAsString((long)AttributeDefinitions.IndexDotName);
        }

        public IVertex StoreIndex(
            IVertexStore myStore,
            VertexInformation myVertex,
            String myName,
            String myComment,
            Int64 myCreationDate,
            String myIndexClass,
            //bool myIsSingleValue,
            bool myIsRange,
            bool myIsVersioned,
            bool myIsUserDefined,
            IDictionary<String, object> myOptions,
            VertexInformation myDefiningVertexType,
            VertexInformation? mySourceIndex, 
            IList<VertexInformation> myIndexedProperties,
            SecurityToken mySecurity,
            Int64 myTransaction)
        {
            var props = new Dictionary<long, IComparable>
                {
                    { (long) AttributeDefinitions.IndexDotName, myName },
                    { (long) AttributeDefinitions.IndexDotIsUserDefined, myIsUserDefined },
                    //{ (long) AttributeDefinitions.IndexDotIsSingleValue, myIsSingleValue},
                    { (long) AttributeDefinitions.IndexDotIsRange, myIsRange },
                    { (long) AttributeDefinitions.IndexDotIsVersioned, myIsVersioned },
                };

            var single = new Dictionary<Tuple<long, long>, VertexInformation>
                {
                    { _EdgeIndexDotDefiningVertexType, myDefiningVertexType }
                };

            if (myIndexClass != null)
                props.Add((long) AttributeDefinitions.IndexDotIndexClass, myIndexClass);

            if (mySourceIndex.HasValue)
                single.Add( _EdgeIndexDotSource, mySourceIndex.Value);

            return Store(
                myStore,
                myVertex,
                myComment,
                myCreationDate,
                single,
                new Dictionary<Tuple<long, long>, IEnumerable<VertexInformation>>
                {
                    { _EdgeIndexDotIndexedProperties, myIndexedProperties }
                },
                props,
                myOptions,
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
        private IVertex Store(
            IVertexStore myStore,
            VertexInformation mySource,
            String myComment,
            Int64 myCreationDate,
            IDictionary<Tuple<Int64, Int64>, VertexInformation> mySingleEdges,
            IDictionary<Tuple<Int64, Int64>, IEnumerable<VertexInformation>> myHyperEdges,
            IDictionary<Int64, IComparable> myStructuredProperties,
            IDictionary<String, Object> myUnstructuredProperties,
            SecurityToken mySecurity,
            Int64 myTransaction)
        {
            VertexAddDefinition def = new VertexAddDefinition(
                mySource.VertexID,
                mySource.VertexTypeID,
                mySource.VertexEditionName,
                CreateHyperEdgeDefinitions(myHyperEdges, mySource, myCreationDate),
                CreateSingleEdgeDefinitions(mySingleEdges, mySource, myCreationDate),
                null,
                null,
                myComment,
                myCreationDate,
                myCreationDate,
                myStructuredProperties,
                myUnstructuredProperties);

            return myStore.AddVertex(mySecurity, myTransaction, def);
        }

        private IEnumerable<SingleEdgeAddDefinition> CreateSingleEdgeDefinitions(
            IDictionary<Tuple<long, long>, VertexInformation> mySingleEdges,
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
        private IEnumerable<HyperEdgeAddDefinition> CreateHyperEdgeDefinitions(
            IDictionary<Tuple<Int64, Int64>, IEnumerable<VertexInformation>> myEdges,
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
                                { (long)AttributeDefinitions.OrderableEdgeDotOrder, pos },
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
        private IVertexType GetTargetVertexType(IVertex myOutgoingEdge)
        {
            var vertex = myOutgoingEdge.GetOutgoingSingleEdge((long)AttributeDefinitions.OutgoingEdgeDotTarget).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An outgoing edge has no vertex that represents its target vertex type.");

            return new VertexType(vertex, this);
        }

        private IBaseType GetDefiningType(IVertex myAttributeVertex)
        {
            var edge = myAttributeVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.AttributeDotDefiningType);

            if (edge == null)
                throw new UnknownDBException("An attribute has no vertex that represents its defining type.");

            var vertex = edge.GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An attribute has no vertex that represents its defining type.");

            switch (vertex.VertexTypeID)
            {
                case (long)BaseTypes.VertexType: return new VertexType(vertex, this);
                case (long)BaseTypes.EdgeType: return new EdgeType(vertex, this);
                default: throw new UnknownDBException("The defining attribute vertex neither point to an edge nor to an vertex type vertex.");
            }
        }

        private IEdgeType GetInnerEdgeType(IVertex myOutgoingEdgeVertex)
        {
            var vertex = myOutgoingEdgeVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.OutgoingEdgeDotInnerEdgeType).GetTargetVertex();
            if (vertex == null)
                throw new UnknownDBException("An outgoing edge has no vertex that represents its inner edge type.");

            return new EdgeType(vertex, this);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="myOutgoingEdge">A vertex that represents an outgoing edge.</param>
        /// <returns>The edge type of the outgoing edge.</returns>
        private  IEdgeType GetEdgeType(IVertex myOutgoingEdge)
        {
            var vertex = myOutgoingEdge.GetOutgoingSingleEdge((long)AttributeDefinitions.OutgoingEdgeDotEdgeType).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An outgoing edge has no vertex that represents its edge type.");

            return new EdgeType(vertex, this);
        }

        
        private  EdgeMultiplicity GetEdgeMultiplicity(IVertex myOutgoingEdgeVertex)
        {
            var multID = myOutgoingEdgeVertex.GetProperty<Byte>((long)AttributeDefinitions.OutgoingEdgeDotMultiplicity);

            if (!Enum.IsDefined(typeof(EdgeMultiplicity), multID))
                throw new UnknownDBException("The value for the edge multiplicity is incorrect.");

            return (EdgeMultiplicity)multID;

        }

        private   PropertyMultiplicity GetPropertyMultiplicity(IVertex myVertex)
        {
            var multID = myVertex.GetProperty<Byte>((long)AttributeDefinitions.PropertyDotMultiplicity);

            if (!Enum.IsDefined(typeof(PropertyMultiplicity), multID))
                throw new UnknownDBException("The value for the property multiplicity is incorrect.");

            return (PropertyMultiplicity)multID;
        }

        private  bool GetIsMandatory(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>((long)AttributeDefinitions.PropertyDotIsMandatory);
        }

        private  IComparable GetDefaultValue(IVertex myPropertyVertex, Type myPropertyType)
        {
            if (!myPropertyVertex.HasProperty((long)AttributeDefinitions.PropertyDotDefaultValue))
                return null;

            var val = myPropertyVertex.GetPropertyAsString((long)AttributeDefinitions.PropertyDotDefaultValue);

            return (IComparable)Convert.ChangeType(val, myPropertyType);
        }

        private bool GetAttributeDotIsUserDefined(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>((long)AttributeDefinitions.AttributeDotIsUserDefined);
        }

        private Type GetBaseType(IVertex myVertex)
        {
            var typeID = GetUUID(myVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.PropertyDotBaseType).GetTargetVertex());

            return _baseTypeManager.GetBaseType(typeID);
        }

        /// <summary>
        /// Creates an outgoing edge definition from a vertex that represents an incoming edge definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an incoming edge definition.</param>
        /// <returns>An outgoing edge definition.</returns>
        private  IOutgoingEdgeDefinition GetRelatedOutgoingEdgeDefinition(IVertex myVertex)
        {
            var vertex = myVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.IncomingEdgeDotRelatedEgde).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An incoming edge definition has no vertex that represents its related outgoing edge definition.");

            return CreateOutgoingEdgeDefinition(vertex);
        }

        private  IEnumerable<IIndexDefinition> GetInIndices(IVertex myVertex)
        {
            if (myVertex.HasIncomingVertices((long)BaseTypes.Index, (long)AttributeDefinitions.IndexDotIndexedProperties))
            {
                var indices = myVertex.GetIncomingVertices((long)BaseTypes.Index, (long)AttributeDefinitions.IndexDotIndexedProperties);

                return indices.Select(_ => CreateIndexDefinition(_));
            }
            return Enumerable.Empty<IIndexDefinition>();
        }


        private  IVertexType GetDefiningVertexType(IVertex myIndexVertex)
        {
            var edge = myIndexVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.IndexDotDefiningVertexType);

            if (edge == null)
                throw new UnknownDBException("An index has no vertex that represents its defining vertex type.");

            var vertex = edge.GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An index has no vertex that represents its defining vertex type.");

            return new VertexType(vertex, this);
        }

        private  String GetIndexTypeName(IVertex myIndexVertex)
        {
            if (myIndexVertex.HasProperty((long)AttributeDefinitions.IndexDotIndexClass))
                return myIndexVertex.GetPropertyAsString((long)AttributeDefinitions.IndexDotIndexClass);

            return null;
        }

        private  IList<IPropertyDefinition> GetIndexedProperties(IVertex myIndexVertex)
        {
            var edge = myIndexVertex.GetOutgoingHyperEdge((long)AttributeDefinitions.IndexDotIndexedProperties);
            if (edge == null)
                throw new UnknownDBException("An index has no vertex that represents its indexed properties.");


            var vertices = edge.GetAllEdges();
            if (vertices == null)
                throw new UnknownDBException("An index has no vertex that represents its indexed properties.");

            vertices = vertices.OrderBy(x => x.GetProperty<int>((long)AttributeDefinitions.OrderableEdgeDotOrder));

            return vertices.Select(x=> CreatePropertyDefinition(x.GetTargetVertex())).ToArray();

        }




        #endregion

        public  IEnumerable<IBinaryPropertyDefinition> GetBinaryPropertiesFromFS(IVertex myTypeVertex, IVertexType myBaseType = null)
        {
            return GetAttributeVertices(myTypeVertex, (long)BaseTypes.BinaryProperty).Select(x => CreateBinaryPropertyDefinition(x, myBaseType));
        }

        public  IEnumerable<IPropertyDefinition> GetPropertiesFromFS(IVertex myTypeVertex, IBaseType myBaseType = null)
        {
            return GetAttributeVertices(myTypeVertex, (long)BaseTypes.Property).Select(x => CreatePropertyDefinition(x, myBaseType));
        }

        public  IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgesFromFS(IVertex myTypeVertex, IVertexType myBaseType = null)
        {
            return GetAttributeVertices(myTypeVertex, (long)BaseTypes.OutgoingEdge).Select(x => CreateOutgoingEdgeDefinition(x, myBaseType));
        }

        public  IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgesFromFS(IVertex myTypeVertex, IVertexType myBaseType = null)
        {
            return GetAttributeVertices(myTypeVertex, (long)BaseTypes.IncomingEdge).Select(x => CreateIncomingEdgeDefinition(x, myBaseType));
        }

        private  IEnumerable<IVertex> GetAttributeVertices(IVertex myTypeVertex, long myAttributeVertexID)
        {
            if (myTypeVertex.HasIncomingVertices(myAttributeVertexID, (long)AttributeDefinitions.AttributeDotDefiningType))
            {
                var vertices = myTypeVertex.GetIncomingVertices(myAttributeVertexID, (long)AttributeDefinitions.AttributeDotDefiningType);
                foreach (var vertex in vertices)
                {
                    if (vertex == null)
                        throw new UnknownDBException("An element in attributes list is NULL.");

                    yield return vertex;
                }
            }
            yield break;
        }


        #region Get attributes

        public  string GetComment(IVertex myVertex)
        {
            return myVertex.Comment;
        }

        public long GetUUID(IVertex myVertex)
        {
            return myVertex.VertexID;
        }

        #endregion
        

        #region IManager Members

        public void Initialize(IMetaManager myMetaManager)
        {
            _metaManager = myMetaManager;            
        }

        public void Load(Int64 myTransaction, SecurityToken mySecurity)
        {
            _baseTypeManager = _metaManager.BaseTypeManager;
        }

        #endregion
    }
}
