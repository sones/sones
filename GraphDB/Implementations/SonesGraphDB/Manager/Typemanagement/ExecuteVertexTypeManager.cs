using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Expression;
using sones.GraphDB.Manager.BaseGraph;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.Vertex;
using sones.GraphDB.Request;
using sones.GraphDB.Request.CreateVertexTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.VertexStore.Definitions.Update;
using sones.Library.LanguageExtensions;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class ExecuteVertexTypeManager: AVertexTypeManager
    {
        private IDictionary<string, IVertexType> _baseTypes = new Dictionary<String, IVertexType>();
        private IManagerOf<IVertexHandler> _vertexManager;
        private IIndexManager _indexManager;
        private IManagerOf<IEdgeTypeHandler> _edgeManager;
        private IDManager _idManager;

        public ExecuteVertexTypeManager(IDManager myIDManager)
        {
            _idManager = myIDManager;
        }

        /// <summary>
        /// A property expression on VertexType.Name
        /// </summary>
        private readonly IExpression _vertexTypeNameExpression = new PropertyExpression(BaseTypes.VertexType.ToString(), "Name");

        /// <summary>
        /// A property expression on VertexType.ID
        /// </summary>
        private readonly IExpression _vertexTypeIDExpression = new PropertyExpression(BaseTypes.VertexType.ToString(), "UUID");

        /// <summary>
        /// A property expression on OutgoingEdge.Name
        /// </summary>
        private readonly IExpression _attributeNameExpression = new PropertyExpression(BaseTypes.OutgoingEdge.ToString(), "Name");


        #region IVertexTypeManager Members

        public override IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new ArgumentOutOfRangeException("myTypeName", "The type name must contain at least one character.");

            #region get static types

            if (_baseTypes.ContainsKey(myTypeName))
            {
                return _baseTypes[myTypeName];
            }

            #endregion

            #region get from fs

            var vertex = Get(myTypeName, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            return new VertexType(vertex);

            #endregion
        }

        public override IEnumerable<IVertexType> GetAllVertexTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.GetVertices(BaseTypes.VertexType.ToString(), myTransaction, mySecurity);

            if (vertices == null)
                return Enumerable.Empty<IVertexType>();

            return vertices.Select(x => new VertexType(x));
        }

        public override IEnumerable<IVertexType> AddVertexTypes(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return Add(myVertexTypeDefinitions, myTransaction, mySecurity);
        }

        public override IEnumerable<long> RemoveVertexTypes(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return Remove(myVertexTypes, myTransaction, mySecurity);
        }

        public override void UpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            Update(myVertexTypeDefinitions, myTransaction, mySecurity);
        }

        public override IVertexType GetVertexType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get static types

            if (Enum.IsDefined(typeof(BaseTypes), myTypeId) && _baseTypes.ContainsKey(((BaseTypes)myTypeId).ToString()))
            {
                return _baseTypes[((BaseTypes)myTypeId).ToString()];
            }

            #endregion


            #region get from fs

            var vertex = Get(myTypeId, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with ID {0} was not found.", myTypeId));

            return new VertexType(vertex);
        
            #endregion
        }

        #endregion

        private IEnumerable<IVertexType> Add(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            //Perf: count is necessary, fast if it is an ICollection
            var count = myVertexTypeDefinitions.Count();

            //This operation reserves #count ids for this operation.
            var firstTypeID = _idManager.VertexTypeID.ReserveIDs(count);

            //Contains dictionary of vertex name to vertex predefinition.
            var defsByVertexName = CanAddCheckDuplicates(myVertexTypeDefinitions);

            //Contains dictionary of parent vertex name to list of vertex predefinitions.
            var defsByParentVertexName = myVertexTypeDefinitions
                .GroupBy(def => def.SuperVertexTypeName)
                .ToDictionary(group => group.Key, group => group.AsEnumerable());

            //Contains list of vertex predefinitions sorted topologically.
            var defsTopologically = CanAddSortTopolocically(defsByVertexName, defsByParentVertexName);

            CanAddCheckWithFS(defsTopologically, defsByVertexName, myTransaction, mySecurity);

            var typeInfos = GenerateTypeInfos(defsTopologically, defsByVertexName, firstTypeID, myTransaction, mySecurity);

            //we can add each type separately
            var creationDate = DateTime.UtcNow.ToBinary();
            var resultPos = 0;

            var result = new IVertexType[count];

            //now we store each vertex type
            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                result[resultPos++] = new VertexType(BaseGraphStorageManager.StoreVertexType(
                    _vertexManager.ExecuteManager.VertexStore,
                    typeInfos[current.Value.VertexTypeName].VertexInfo,
                    current.Value.VertexTypeName,
                    current.Value.Comment,
                    creationDate,
                    current.Value.IsAbstract,
                    current.Value.IsSealed,
                    true,
                    typeInfos[current.Value.SuperVertexTypeName].VertexInfo,
                    null, //TODO uniques
                    mySecurity,
                    myTransaction));

                _indexManager.GetIndex(BaseUniqueIndex.VertexTypeDotName).Add(current.Value.VertexTypeName, typeInfos[current.Value.VertexTypeName].VertexInfo.VertexID);
                
            }

            #region Store Attributes

            //The order of adds is important. First property, then outgoing edges (that might point to properties) and finally incoming edges (that might point to outgoing edges)
            //Do not try to merge it into one for block.

            #region Store properties

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.Properties == null)
                    continue;

                var firstAttrID = _idManager[(long)BaseTypes.Attribute].ReserveIDs(current.Value.PropertyCount);
                var currentExternID = typeInfos[current.Value.VertexTypeName].AttributeCountWithParents - current.Value.PropertyCount - 1;

                foreach (var prop in current.Value.Properties)
                {
                    BaseGraphStorageManager.StoreProperty(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.Property, firstAttrID++),
                        prop.AttributeName,
                        prop.Comment,
                        creationDate,
                        prop.IsMandatory,
                        prop.Multiplicity,
                        prop.DefaultValue,
                        true,
                        typeInfos[current.Value.VertexTypeName].VertexInfo,
                        ConvertBasicType(prop.AttributeType),
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store binary properties

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.BinaryProperties == null)
                    continue;

                var firstAttrID = _idManager[(long)BaseTypes.Attribute].ReserveIDs(current.Value.BinaryPropertyCount);
                var currentExternID = typeInfos[current.Value.VertexTypeName].AttributeCountWithParents - current.Value.PropertyCount - current.Value.BinaryPropertyCount - 1;

                foreach (var prop in current.Value.BinaryProperties)
                {
                    BaseGraphStorageManager.StoreBinaryProperty(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.BinaryProperty, firstAttrID++),
                        prop.AttributeName,
                        prop.Comment,
                        true,
                        creationDate,
                        typeInfos[current.Value.VertexTypeName].VertexInfo,
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store outgoing edges

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.OutgoingEdges == null)
                    continue;

                var firstAttrID = _idManager[(long)BaseTypes.Attribute].ReserveIDs(current.Value.OutgoingEdgeCount);
                var currentExternID = typeInfos[current.Value.VertexTypeName].AttributeCountWithParents - current.Value.PropertyCount - current.Value.OutgoingEdgeCount - current.Value.BinaryPropertyCount - 1;

                foreach (var edge in current.Value.OutgoingEdges)
                {
                    VertexInformation? innerEdgeType = null;
                    if (edge.Multiplicity == EdgeMultiplicity.MultiEdge)
                    {
                        innerEdgeType = new VertexInformation((long)BaseTypes.EdgeType, _edgeManager.ExecuteManager.GetEdgeType(edge.InnerEdgeType, myTransaction, mySecurity).ID);
                    }
                    BaseGraphStorageManager.StoreOutgoingEdge(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.OutgoingEdge, firstAttrID++),
                        edge.AttributeName,
                        edge.Comment,
                        true,
                        creationDate,
                        edge.Multiplicity,
                        typeInfos[current.Value.VertexTypeName].VertexInfo,
                        new VertexInformation((long)BaseTypes.EdgeType, _edgeManager.ExecuteManager.GetEdgeType(edge.EdgeType, myTransaction, mySecurity).ID),
                        innerEdgeType,
                        typeInfos[edge.AttributeType].VertexInfo,
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store incoming edges

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.IncomingEdges == null)
                    continue;

                var firstAttrID = _idManager[(long)BaseTypes.Attribute].ReserveIDs(current.Value.IncomingEdgeCount);
                var currentExternID = typeInfos[current.Value.VertexTypeName].AttributeCountWithParents - current.Value.PropertyCount - current.Value.BinaryPropertyCount - current.Value.OutgoingEdgeCount - current.Value.IncomingEdgeCount - 1;

                foreach (var edge in current.Value.IncomingEdges)
                {

                    BaseGraphStorageManager.StoreIncomingEdge(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.IncomingEdge, firstAttrID++),
                        edge.AttributeName,
                        edge.Comment,
                        true,
                        creationDate,
                        typeInfos[current.Value.VertexTypeName].VertexInfo,
                        GetOutgoingEdgeVertexInformation(GetTargetVertexTypeFromAttributeType(edge.AttributeType), GetTargetEdgeNameFromAttributeType(edge.AttributeType), myTransaction, mySecurity),
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion


            #endregion

            #region Add Indices
            if (_indexManager != null)
            {
                var uniqueIdx = _indexManager.GetBestMatchingIndexName(true, false, false);
                var indexIdx = _indexManager.GetBestMatchingIndexName(false, false, false);

                resultPos = 0;
                for (var current = defsTopologically.First; current != null; current = current.Next, resultPos++)
                {
                    #region Uniqueness

                    #region own uniques

                    if (current.Value.Uniques != null)
                    {
                        var indexPredefs = current.Value.Uniques.Select(unique =>
                            new IndexPredefinition().AddProperty(unique.Properties).SetIndexType(uniqueIdx).SetVertexType(current.Value.VertexTypeName));

                        var indexDefs = indexPredefs.Select(indexPredef => _indexManager.CreateIndex(indexPredef, mySecurity, myTransaction, false)).ToArray();

                        //only own unique indices are connected to the vertex type on the UniquenessDefinitions attribute
                        ConnectVertexToUniqueIndex(typeInfos[current.Value.VertexTypeName], indexDefs, mySecurity, myTransaction);
                    }

                    #endregion

                    #region parent uniques

                    foreach (var unique in result[resultPos].ParentVertexType.GetUniqueDefinitions(true))
                    {
                        _indexManager.CreateIndex(
                            new IndexPredefinition().AddProperty(unique.UniquePropertyDefinitions.Select(x => x.Name)).SetIndexType(uniqueIdx).SetVertexType(unique.DefiningVertexType.Name),
                            mySecurity,
                            myTransaction,
                            false);
                    }

                    #endregion

                    #endregion

                    #region Indices

                    if (current.Value.Indices != null)
                        foreach (var index in current.Value.Indices)
                        {
                            _indexManager.CreateIndex(index, mySecurity, myTransaction);
                        }

                    foreach (var index in result[resultPos].ParentVertexType.GetIndexDefinitions(true))
                    {
                        _indexManager.CreateIndex(
                            new IndexPredefinition(index.Name).AddProperty(index.IndexedProperties.Select(x => x.Name)).SetVertexType(current.Value.VertexTypeName).SetIndexType(index.IndexTypeName),
                            mySecurity,
                            myTransaction);
                    }

                    #endregion

                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// Does the necessary checks for can add with the use of the FS.
        /// </summary>
        /// <param name="myDefsTopologically">A topologically sorted list of vertex type predefinitions. <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myDefsByName">The same vertex type predefinitions as in <paramref name="myDefsTpologically"/>, but indexed by their name. <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <remarks><paramref name="myDefsTopologically"/> and <paramref name="myDefsByName"/> must contain the same vertex type predefinitions. This is never checked.</remarks>
        /// The predefinitions are checked one by one in topologically order. 
        private void CanAddCheckWithFS(
            LinkedList<VertexTypePredefinition> myDefsTopologically,
            Dictionary<String, VertexTypePredefinition> myDefsByName,
            TransactionToken myTransaction, SecurityToken mySecurity)
        {

            //Contains the vertex type name to the attribute names of the vertex type.
            var attributes = new Dictionary<String, HashSet<String>>(myDefsTopologically.Count);

            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                CanAddCheckVertexNameUniqueWithFS(current.Value, myTransaction, mySecurity);
                CanAddCheckAttributeNameUniquenessWithFS(current, attributes, myTransaction, mySecurity);
                CanAddCheckOutgoingEdgeTargets(current.Value, myDefsByName, myTransaction, mySecurity);
                CanAddCheckIncomingEdgeSources(current.Value, myDefsByName, myTransaction, mySecurity);
            }
        }

        /// <summary>
        /// Checks if the attribute names on vertex type definitions are unique, containing parent myAttributes.
        /// </summary>
        /// <param name="myTopologicallySortedPointer">A pointer to a vertex type predefinitions in a topologically sorted linked list.</param>
        /// <param name="myAttributes">A dictionary vertex type name to attribute names, that is build up during the process of CanAddCheckWithFS.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckAttributeNameUniquenessWithFS(LinkedListNode<VertexTypePredefinition> myTopologicallySortedPointer, IDictionary<string, HashSet<string>> myAttributes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var parentPredef = GetParentPredefinitionOnTopologicallySortedList(myTopologicallySortedPointer);

            if (parentPredef == null)
            {
                //Get the parent type from FS.
                var parent = Get(myTopologicallySortedPointer.Value.SuperVertexTypeName, myTransaction, mySecurity);

                if (parent == null)
                    //No parent type was found.
                    throw new InvalidBaseVertexTypeException(myTopologicallySortedPointer.Value.SuperVertexTypeName);

                if (parent.GetProperty<bool>((long)AttributeDefinitions.BaseTypeDotIsSealed))
                    //The parent type is sealed.
                    throw new SealedBaseVertexTypeException(myTopologicallySortedPointer.Value.VertexTypeName, parent.GetPropertyAsString((long)AttributeDefinitions.AttributeDotName));

                var parentType = new VertexType(parent);
                var attributeNames = parentType.GetAttributeDefinitions(true).Select(_=>_.Name);

                myAttributes[myTopologicallySortedPointer.Value.VertexTypeName] = new HashSet<string>(attributeNames);
            }
            else
            {
                myAttributes[myTopologicallySortedPointer.Value.VertexTypeName] = new HashSet<string>(myAttributes[parentPredef.Value.VertexTypeName]);
            }

            var attributeNamesSet = myAttributes[myTopologicallySortedPointer.Value.VertexTypeName];

            CheckIncomingEdgesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
            CheckOutgoingEdgesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
            CheckPropertiesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
        }

        /// <summary>
        /// Checks if an incoming myEdge has a coresponding outgoing myEdge.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type definition of that the incoming edges will be checked,</param>
        /// <param name="myDefsByName">The vertex type predefinitions indexed by their name that are alse defined in this CanAdd operation.<remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckIncomingEdgeSources(VertexTypePredefinition myVertexTypePredefinition, IDictionary<string, VertexTypePredefinition> myDefsByName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (myVertexTypePredefinition.IncomingEdges == null)
                return;

            var grouped = myVertexTypePredefinition.IncomingEdges.GroupBy(x => GetTargetVertexTypeFromAttributeType(x.AttributeType));
            foreach (var group in grouped)
            {
                if (!myDefsByName.ContainsKey(group.Key))
                {
                    var vertex = Get(group.Key, myTransaction, mySecurity);
                    if (vertex == null)
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key, group.Select(x => x.AttributeName));

                    var attributes = vertex.GetIncomingVertices((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.AttributeDotDefiningType);
                    foreach (var edge in group)
                    {
                        if (!attributes.Any(outgoing => GetTargetVertexTypeFromAttributeType(edge.AttributeName).Equals(outgoing.GetPropertyAsString((long)AttributeDefinitions.AttributeDotName))))
                            throw new OutgoingEdgeNotFoundException(myVertexTypePredefinition, edge);
                    }
                }
                else
                {
                    var target = myDefsByName[group.Key];

                    foreach (var edge in group)
                    {
                        if (!target.OutgoingEdges.Any(outgoing => GetTargetEdgeNameFromAttributeType(edge.AttributeType).Equals(outgoing.AttributeName)))
                            throw new OutgoingEdgeNotFoundException(myVertexTypePredefinition, edge);
                    }

                }
            }
        }

        /// <summary>
        /// Checks if outgoing edges have a valid target.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type definition of that the outgoing edges will be checked,</param>
        /// <param name="myDefsByName">The vertex type predefinitions indexed by their name that are alse defined in this CanAdd operation.<remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckOutgoingEdgeTargets(VertexTypePredefinition myVertexTypePredefinition, IDictionary<string, VertexTypePredefinition> myDefsByName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (myVertexTypePredefinition.OutgoingEdges == null)
                return;

            var grouped = myVertexTypePredefinition.OutgoingEdges.GroupBy(x => x.AttributeType);
            foreach (var group in grouped)
            {
                if (!myDefsByName.ContainsKey(group.Key))
                {
                    var vertex = Get(group.Key, myTransaction, mySecurity);
                    if (vertex == null)
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key, group.Select(x => x.AttributeName));

                }
            }
        }

        /// <summary>
        /// Checks if the name of the given vertex type predefinition is not used in FS before.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The name of this vertex type definition will be checked.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckVertexNameUniqueWithFS(VertexTypePredefinition myVertexTypePredefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (Get(myVertexTypePredefinition.VertexTypeName, myTransaction, mySecurity) != null)
                throw new DuplicatedVertexTypeNameException(myVertexTypePredefinition.VertexTypeName);
        }

        private void ConnectVertexToUniqueIndex(TypeInfo myTypeInfo, IIndexDefinition[] myIndexDefinitions, SecurityToken mySecurity, TransactionToken myTransaction)
        {
            _vertexManager.ExecuteManager.VertexStore.UpdateVertex(
                            mySecurity,
                            myTransaction,
                            myTypeInfo.VertexInfo.VertexID,
                            myTypeInfo.VertexInfo.VertexTypeID,
                            new VertexUpdateDefinition(
                                myHyperEdgeUpdate: new HyperEdgeUpdate(
                                    myUpdated: new Dictionary<long, HyperEdgeUpdateDefinition>
                                {
                                    {
                                        (long)AttributeDefinitions.VertexTypeDotUniquenessDefinitions, 
                                        new HyperEdgeUpdateDefinition((long)BaseTypes.Edge, myToBeUpdatedSingleEdges: myIndexDefinitions.Select(x=>IndexDefinitionToSingleEdgeUpdate(myTypeInfo.VertexInfo, x)))
                                    }
                                }
                            )));
        }

        private struct TypeInfo
        {
            public VertexInformation VertexInfo;
            public long AttributeCountWithParents;
        }

        private Dictionary<String, TypeInfo> GenerateTypeInfos(
            LinkedList<VertexTypePredefinition> myDefsSortedTopologically,
            Dictionary<String, VertexTypePredefinition> myDefsByName,
            long myFirstID,
            TransactionToken myTransaction,
            SecurityToken mySecurity)
        {
            HashSet<String> neededVertexTypes = new HashSet<string>();

            foreach (var def in myDefsByName)
            {
                neededVertexTypes.Add(def.Value.VertexTypeName);
                neededVertexTypes.Add(def.Value.SuperVertexTypeName);
                if (def.Value.OutgoingEdges != null)
                    foreach (var edge in def.Value.OutgoingEdges)
                    {
                        neededVertexTypes.Add(edge.AttributeType);
                    }
            }

            //At most all vertex types are needed.
            var result = new Dictionary<String, TypeInfo>((int)myFirstID + myDefsByName.Count);
            foreach (var vertexType in neededVertexTypes)
            {
                if (myDefsByName.ContainsKey(vertexType))
                {
                    result.Add(vertexType, new TypeInfo
                    {
                        AttributeCountWithParents = myDefsByName[vertexType].AttributeCount,
                        VertexInfo = new VertexInformation((long)BaseTypes.VertexType, myFirstID++)
                    });
                }
                else
                {
                    var vertex = _vertexManager.ExecuteManager.GetSingleVertex(new BinaryExpression(new Expression.SingleLiteralExpression(vertexType), BinaryOperator.Equals, _vertexTypeNameExpression), myTransaction, mySecurity);
                    IVertexType neededVertexType = new VertexType(vertex);
                    result.Add(vertexType, new TypeInfo
                    {
                        AttributeCountWithParents = neededVertexType.GetAttributeDefinitions(true).LongCount(),
                        VertexInfo = new VertexInformation((long)BaseTypes.VertexType, BaseGraphStorageManager.GetUUID(vertex))
                    });
                }
            }

            //accumulate attribute counts
            for (var current = myDefsSortedTopologically.First; current != null; current = current.Next)
            {
                if (result.ContainsKey(current.Value.VertexTypeName))
                {
                    var info = result[current.Value.VertexTypeName];
                    info.AttributeCountWithParents = info.AttributeCountWithParents + result[current.Value.SuperVertexTypeName].AttributeCountWithParents;
                    result[current.Value.VertexTypeName] = info;
                }
            }



            return result;
        }

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeName"/>.
        /// </summary>
        /// <param name="myTypeName">The vertex type name.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given name or <c>NULL</c>, if not present.</returns>
        private IVertex Get(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.ExecuteManager.GetSingleVertex(new BinaryExpression(_vertexTypeNameExpression, BinaryOperator.Equals, new SingleLiteralExpression(myTypeName)), myTransaction, mySecurity);

            #endregion
        }

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeID"/>.
        /// </summary>
        /// <param name="myTypeName">The vertex type ID.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given ID or <c>NULL</c>, if not present.</returns>
        private IVertex Get(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.ExecuteManager.GetSingleVertex(new BinaryExpression(_vertexTypeIDExpression, BinaryOperator.Equals, new SingleLiteralExpression(myTypeId)), myTransaction, mySecurity);

            #endregion
        }

        private VertexInformation GetOutgoingEdgeVertexInformation(string myVertexType, string myEdgeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.GetVertices(new BinaryExpression(new SingleLiteralExpression(myEdgeName), BinaryOperator.Equals, _attributeNameExpression), false, myTransaction, mySecurity).ToArray();
            var vertex = vertices.First(x => IsAttributeOnVertexType(myVertexType, x));
            return new VertexInformation(vertex.VertexTypeID, vertex.VertexID);
        }

        private static VertexInformation ConvertBasicType(string myBasicTypeName)
        {
            BasicTypes resultType;
            if (!Enum.TryParse(myBasicTypeName, out resultType))
                throw new NotImplementedException("User defined base types are not implemented yet.");

            return DBCreationManager.BasicTypesVertices[resultType];
        }

        /// <summary>
        /// Gets the parent predefinition of the given predefinition.
        /// </summary>
        /// <param name="myCurrent">The predefinition of that the parent vertex predefinition is searched.</param>
        /// <returns>The link to the parent predefinition of the <paramref name="myCurrent"/> predefinition, otherwise <c>NULL</c>.</returns>
        private static LinkedListNode<VertexTypePredefinition> GetParentPredefinitionOnTopologicallySortedList(LinkedListNode<VertexTypePredefinition> myCurrent)
        {
            for (var parent = myCurrent.Previous; parent != null; parent = parent.Previous)
            {
                if (parent.Value.VertexTypeName.Equals(myCurrent.Value.SuperVertexTypeName))
                    return parent;
            }
            return null;
        }

        private static SingleEdgeUpdateDefinition IndexDefinitionToSingleEdgeUpdate(VertexInformation mySourceVertex, IIndexDefinition myDefinition)
        {
            return new SingleEdgeUpdateDefinition(mySourceVertex, new VertexInformation((long)BaseTypes.Index, myDefinition.ID), (long)BaseTypes.Edge);
        }

        private static bool IsAttributeOnVertexType(String myVertexTypeName, IVertex myAttributeVertex)
        {
            var vertexTypeVertex = myAttributeVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.AttributeDotDefiningType).GetTargetVertex();
            var name = vertexTypeVertex.GetPropertyAsString((long)AttributeDefinitions.BaseTypeDotName);
            return name.Equals(myVertexTypeName);
        }

        private IEnumerable<long> Remove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            //the attribute types on delete types which have to be removed
            var toDeleteAttributeDefinitions = new List<IAttributeDefinition>();

            //the indices on the delete types
            var toDeleteIndexDefinitions = new List<IUniqueDefinition>();

            #region check if specified types can be removed
            //get child vertex types and check if they are specified by user
            foreach (var delType in myVertexTypes)
            {
                #region check that the remove type is no base type

                if(!delType.HasParentType)
                    continue;

                if (delType.ParentVertexType.ID.Equals((long)BaseTypes.BaseType) && IsBaseType(delType.ID))
                    //Exception that base type cannot be deleted
                    throw new VertexTypeRemoveException(delType.Name, "A BaseType connot be removed.");

                #endregion

                #region check that existing child types are specified

                foreach (var child in delType.GetChildVertexTypes())
                    if (!myVertexTypes.Contains(child))
                        //all child types has to be specified by user
                        throw new VertexTypeRemoveException(delType.Name, "The given type has child types and cannot be removed."); 
                
                #endregion

                #region check that the delete type has no incoming edges

                if (delType.HasIncomingEdges(false))
                    //all incoming edges has to be deletet by user
                    throw new VertexTypeRemoveException(delType.Name, "The given type has incoming edges and cannot be removed.");

                #endregion

                #region check that no other type has a outgoing edge pointing on delete type

                //get all vertex types and check if they have outgoing edges with target vertex the delete type
                foreach (var type in GetAllVertexTypes(myTransaction, mySecurity))
                {
                    if (type is BaseType)
                        continue;

                    foreach (var outEdgeDef in type.GetOutgoingEdgeDefinitions(false))
                    {
                        if (outEdgeDef.TargetVertexType.ID.Equals(delType.ID))
                            //there is a outgoing edge from a vertex type which points to a vertex type which schould be removed
                            throw new VertexTypeRemoveException(delType.Name, "There are other types which have outgoing edges pointing to the type, which should be removed.");
                        
                    }
                }

                #endregion

                #region check if there are incoming edges of target vertices of outgoing edges of the deleting type

                foreach (var outEdge in delType.GetOutgoingEdgeDefinitions(false))
                {
                    foreach (var inEdge in outEdge.TargetVertexType.GetIncomingEdgeDefinitions(true))
                    {
                        if (inEdge.RelatedEdgeDefinition.ID.Equals(outEdge.ID))
                            //
                            throw new VertexTypeRemoveException(delType.Name, "There are other types which have incoming edges, whose related type is a outgoing edge of the type which should be removed.");
                    }
                }

                #endregion

                toDeleteAttributeDefinitions.AddRange(delType.GetAttributeDefinitions(false));

                toDeleteIndexDefinitions.AddRange(delType.GetUniqueDefinitions(false));
            }
            #endregion

            //the IDs of the deleted vertices
            var deletedTypeIDs = new List<long>( myVertexTypes.Select(item => item.ID) );

            #region remove indices
            //remove indices on types
            foreach (var index in toDeleteIndexDefinitions)
            {
                _indexManager.RemoveIndexInstance(index.CorrespondingIndex.ID, myTransaction, mySecurity);
            } 
            #endregion

            #region remove attribute types on delete types
            //delete attribute vertices
            foreach (var attr in toDeleteAttributeDefinitions)
            {
                switch (attr.Kind)
                {
                    case (AttributeType.Property):
                        _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.Property);
                        break;

                    case (AttributeType.IncomingEdge):
                        _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.IncomingEdge);
                        break;

                    case (AttributeType.OutgoingEdge):
                        _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.OutgoingEdge);
                        break;

                    case (AttributeType.BinaryProperty):
                        _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.BinaryProperty);
                        break;

                    default:
                        break;
                }
            } 
            #endregion

            #region remove vertices
            //delete the vertices
            foreach (var type in myVertexTypes)
            {
                //removes the instances of the VertexType
               _vertexManager.ExecuteManager.VertexStore.RemoveVertices(mySecurity, myTransaction, type.ID);

                //removes the vertexType
                _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, type.ID, (long)BaseTypes.VertexType);

            } 
            #endregion

            #region remove vertex type names from index

            foreach (var vertexType in myVertexTypes)
                _indexManager.GetIndex(BaseUniqueIndex.VertexTypeDotName).Remove(vertexType.Name);

            #endregion

            return deletedTypeIDs;

        }

        /// <summary>
        /// TODO find better check method
        /// Checks if the given type is a base type
        /// </summary>
        private bool IsBaseType(long myTypeID)
        {
            return ((long)BaseTypes.Attribute).Equals(myTypeID) || 
                        ((long)BaseTypes.BaseType).Equals(myTypeID) || 
                        ((long)BaseTypes.BinaryProperty).Equals(myTypeID) ||
                        ((long)BaseTypes.Edge).Equals(myTypeID) ||
                        ((long)BaseTypes.EdgeType).Equals(myTypeID) ||
                        ((long)BaseTypes.IncomingEdge).Equals(myTypeID) ||
                        ((long)BaseTypes.Index).Equals(myTypeID) ||
                        ((long)BaseTypes.Orderable).Equals(myTypeID) ||
                        ((long)BaseTypes.OutgoingEdge).Equals(myTypeID) ||
                        ((long)BaseTypes.Property).Equals(myTypeID) ||
                        ((long)BaseTypes.Vertex).Equals(myTypeID) ||
                        ((long)BaseTypes.VertexType).Equals(myTypeID) ||
                        ((long)BaseTypes.Weighted).Equals(myTypeID);
        }

        private void Update(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        private void LoadBaseType(TransactionToken myTransaction, SecurityToken mySecurity, params BaseTypes[] myBaseTypes)
        {
            foreach (var baseType in myBaseTypes)
            {
                var vertex = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction, (long)baseType, (long)BaseTypes.VertexType, String.Empty);
                if (vertex == null)
                    //TODO: better exception
                    throw new Exception("Could not load base vertex type.");
                _baseTypes.Add(baseType.ToString(), new VertexType(vertex));
            }
        }

        private long GetMaxID(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.GetVertices(myTypeID, myTransaction, mySecurity);
            if (vertices == null)
                //TODO better exception here
                throw new Exception("The base vertex types are not available.");

            return (vertices.CountIsGreater(0))
                ? vertices.Max(x => x.VertexID)
                : Int64.MinValue;
        }

        public void Initialize(IMetaManager myMetaManager)
        {
            _edgeManager = myMetaManager.EdgeTypeManager;
            _indexManager = myMetaManager.IndexManager;
            _vertexManager = myMetaManager.VertexManager;
        }

        public void Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _idManager.VertexTypeID.SetToMaxID(GetMaxID((long)BaseTypes.VertexType, myTransaction, mySecurity) + 1);
            _idManager[(long)BaseTypes.Attribute].SetToMaxID(
                        Math.Max(
                            GetMaxID((long)BaseTypes.Property, myTransaction, mySecurity),
                            Math.Max(
                                GetMaxID((long)BaseTypes.OutgoingEdge, myTransaction, mySecurity),
                                Math.Max(
                                    GetMaxID((long)BaseTypes.IncomingEdge, myTransaction, mySecurity),
                                    GetMaxID((long)BaseTypes.BinaryProperty, myTransaction, mySecurity)))) + 1);


            LoadBaseType(
                myTransaction,
                mySecurity,
                BaseTypes.Attribute,
                BaseTypes.BaseType,
                BaseTypes.BinaryProperty,
                BaseTypes.EdgeType,
                BaseTypes.IncomingEdge,
                BaseTypes.Index,
                BaseTypes.OutgoingEdge,
                BaseTypes.Property,
                BaseTypes.VertexType);

        }

        public override void TruncateVertexType(long myVertexTypeID, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            var vertexType = GetVertexType(myVertexTypeID, myTransactionToken, mySecurityToken);

            #region remove all vertices of this type

            _vertexManager.ExecuteManager.VertexStore.RemoveVertices(mySecurityToken, myTransactionToken, myVertexTypeID);

            #endregion

            #region rebuild indices

            _indexManager.RebuildIndices(myVertexTypeID, myTransactionToken, mySecurityToken);

            #endregion
        }

        public override void TruncateVertexType(String myVertexTypeName, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            var vertexType = GetVertexType(myVertexTypeName, myTransactionToken, mySecurityToken);

            this.TruncateVertexType(vertexType.ID, myTransactionToken, mySecurityToken);
        }

        public override void AlterVertexType(RequestAlterVertexType myAlterVertexTypeRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var vertexType = GetVertexType(myAlterVertexTypeRequest.VertexTypeName, myTransactionToken, mySecurityToken);

            #region checks

            CheckToBeAddedAttributes(myAlterVertexTypeRequest, vertexType);
            CheckToBeRemovedAttributes(myAlterVertexTypeRequest, vertexType);
            CheckToBeRenamedAttributes(myAlterVertexTypeRequest, vertexType);

            #endregion

        }

        /// <summary>
        /// Checks the to be renamed attributes
        /// </summary>
        /// <param name="myAlterVertexTypeRequest"></param>
        /// <param name="vertexType"></param>
        private static void CheckToBeRenamedAttributes(RequestAlterVertexType myAlterVertexTypeRequest, IVertexType vertexType)
        {
            foreach (var aToBeRenamedAttributes in myAlterVertexTypeRequest.ToBeRenamedProperties)
            {
                if (!CheckOldName(aToBeRenamedAttributes.Key, vertexType))
                {
                    throw new InvalidAlterVertexTypeException(String.Format("It is not possible to rename {0} into {1}. The to be renamed attribute does not exist."));                    
                }

                if (!CheckNewName(aToBeRenamedAttributes.Value, vertexType))
	            {
                    throw new InvalidAlterVertexTypeException(String.Format("It is not possible to rename {0} into {1}. The new attribute name already exists."));
	            }
            }
        }

        /// <summary>
        /// Checks if the new name of the attribute already exists
        /// </summary>
        /// <param name="myNewAttributeName"></param>
        /// <param name="vertexType"></param>
        /// <returns></returns>
        private static bool CheckNewName(string myNewAttributeName, IVertexType vertexType)
        {
            foreach (var aVertexType in vertexType.GetChildVertexTypes(true, true))
            {
                if( aVertexType.HasBinaryProperty(myNewAttributeName) ||
                    aVertexType.HasOutgoingEdge(myNewAttributeName) ||
                    aVertexType.HasIncomingEdge(myNewAttributeName) ||
                    aVertexType.HasProperty(myNewAttributeName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the old name exists on the given vertex type
        /// </summary>
        /// <param name="myOldAttributeName"></param>
        /// <param name="vertexType"></param>
        /// <returns></returns>
        private static bool CheckOldName(string myOldAttributeName, IVertexType vertexType)
        {
            return vertexType.HasProperty(myOldAttributeName) || vertexType.HasOutgoingEdge(myOldAttributeName) || vertexType.HasIncomingEdge(myOldAttributeName);
        }

        /// <summary>
        /// Checks if the to be removed attributes exists on this type
        /// </summary>
        /// <param name="myAlterVertexTypeRequest"></param>
        /// <param name="vertexType"></param>
        private static void CheckToBeRemovedAttributes(RequestAlterVertexType myAlterVertexTypeRequest, IVertexType vertexType)
        {
            #region properties

            foreach (var aToBeDeletedAttribute in myAlterVertexTypeRequest.ToBeRemovedProperties)
            {
                if (!vertexType.HasProperty(aToBeDeletedAttribute))
                {
                    throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute);
                }
            }

            #endregion

            #region outgoing Edges

            foreach (var aToBeDeletedAttribute in myAlterVertexTypeRequest.ToBeRemovedOutgoingEdges)
            {
                if (!vertexType.HasOutgoingEdge(aToBeDeletedAttribute))
                {
                    throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute);
                }
            }

            #endregion

            #region incoming edges

            foreach (var aToBeDeletedAttribute in myAlterVertexTypeRequest. ToBeRemovedIncomingEdges)
            {
                if (!vertexType.HasIncomingEdge(aToBeDeletedAttribute))
                {
                    throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute);
                }
            }

            #endregion
        }

        /// <summary>
        /// Checks if the to be added attributes exist in the given vertex type or derived oness
        /// </summary>
        /// <param name="myAlterVertexTypeRequest"></param>
        /// <param name="vertexType"></param>
        private static void CheckToBeAddedAttributes(RequestAlterVertexType myAlterVertexTypeRequest, IVertexType vertexType)
        {
            foreach (var aVertexType in vertexType.GetChildVertexTypes(true, true))
            {
                #region binary properties

                foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedBinaryProperties)
                {
                    if (aVertexType.HasBinaryProperty(aToBeAddedAttribute.AttributeName))
                    {
                        throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region outgoing edges

                foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedOutgoingEdges)
                {
                    if (aVertexType.HasOutgoingEdge(aToBeAddedAttribute.AttributeName))
                    {
                        throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region Incoming edges

                foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedIncomingEdges)
                {
                    if (aVertexType.HasIncomingEdge(aToBeAddedAttribute.AttributeName))
                    {
                        throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region property

                foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedProperties)
                {
                    if (aVertexType.HasProperty(aToBeAddedAttribute.AttributeName))
                    {
                        throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region unknown attributes

                foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedUnknownAttributes)
                {
                    if (aVertexType.HasProperty(aToBeAddedAttribute.AttributeName) ||
                        aVertexType.HasOutgoingEdge(aToBeAddedAttribute.AttributeName) ||
                        aVertexType.HasIncomingEdge(aToBeAddedAttribute.AttributeName) ||
                        aVertexType.HasBinaryProperty(aToBeAddedAttribute.AttributeName))
                    {
                        throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }


                #endregion
            }
        }
    }


}
