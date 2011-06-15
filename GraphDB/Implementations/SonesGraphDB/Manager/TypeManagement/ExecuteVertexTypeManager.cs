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
        private readonly IDictionary<long, IVertexType> _baseTypes = new Dictionary<long, IVertexType>();
        private readonly IDictionary<String, long> _nameIndex = new Dictionary<String, long>();
        private IManagerOf<IVertexHandler> _vertexManager;
        private IIndexManager _indexManager;
        private IManagerOf<IEdgeTypeHandler> _edgeManager;
        private readonly IDManager _idManager;

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
        private readonly IExpression _vertexTypeIDExpression = new PropertyExpression(BaseTypes.VertexType.ToString(), "VertexID");

        /// <summary>
        /// A property expression on OutgoingEdge.Name
        /// </summary>
        private readonly IExpression _attributeNameExpression = new PropertyExpression(BaseTypes.OutgoingEdge.ToString(), "Name");


        #region IVertexTypeManager Members

        public override IVertexType GetVertexType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get static types

            if (_baseTypes.ContainsKey(myTypeId))
            {
                return _baseTypes[myTypeId];
            }

            #endregion


            #region get from fs

            var vertex = Get(myTypeId, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with ID {0} was not found.", myTypeId));

            var result = new VertexType(vertex);
            _baseTypes.Add(result.ID, result);
            _nameIndex.Add(result.Name, result.ID);
            return result;
        
            #endregion
        }

        public override IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new ArgumentOutOfRangeException("myTypeName", "The type name must contain at least one character.");

            #region get static types

            if (_nameIndex.ContainsKey(myTypeName))
            {
                return _baseTypes[_nameIndex[myTypeName]];
            }

            #endregion

            #region get from fs

            var vertex = Get(myTypeName, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            var result = new VertexType(vertex);
            _baseTypes.Add(result.ID, result);
            _nameIndex.Add(result.Name, result.ID);
            return result;


            #endregion
        }

        public override IEnumerable<IVertexType> GetAllVertexTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.GetVertices(BaseTypes.VertexType.ToString(), myTransaction, mySecurity, false);

            return vertices == null ? Enumerable.Empty<IVertexType>() : vertices.Select(x => new VertexType(x));
        }

        public override IEnumerable<IVertexType> AddVertexTypes(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return Add(myVertexTypeDefinitions, myTransaction, mySecurity);
        }

        public override Dictionary<Int64, String> RemoveVertexTypes(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, bool myIgnoreReprimands = false)
        {
            return Remove(myVertexTypes, myTransaction, mySecurity, myIgnoreReprimands);
        }

        public override IEnumerable<long> ClearDB(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return Clear(myTransaction, mySecurity);
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

            var result = new IVertex[count];

            //now we store each vertex type
            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
               var newVertexType = BaseGraphStorageManager.StoreVertexType(
                    _vertexManager.ExecuteManager.VertexStore,
                    typeInfos[current.Value.VertexTypeName].VertexInfo,
                    current.Value.VertexTypeName,
                    current.Value.Comment,
                    creationDate,
                    current.Value.IsAbstract,
                    current.Value.IsSealed,
                    true,
                    typeInfos[current.Value.SuperVertexTypeName].VertexInfo,
                    null, 
                    mySecurity,
                    myTransaction);

                 result[resultPos++] = newVertexType;
                _indexManager.GetIndex(BaseUniqueIndex.VertexTypeDotName).Add(current.Value.VertexTypeName, typeInfos[current.Value.VertexTypeName].VertexInfo.VertexID);

                _nameIndex.Add(current.Value.VertexTypeName, typeInfos[current.Value.VertexTypeName].VertexInfo.VertexID);
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

            var resultTypes = new VertexType[result.Length];

            //reload the IVertex objects, that represents the type.
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction,
                                                                                result[i].VertexID,
                                                                                result[i].VertexTypeID, String.Empty);
                var newVertexType = new VertexType(result[i]);
                resultTypes[i] = newVertexType;
                _baseTypes.Add(typeInfos[newVertexType.Name].VertexInfo.VertexID, newVertexType);    

            }

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

                    foreach (var unique in resultTypes[resultPos].ParentVertexType.GetUniqueDefinitions(true))
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

                    foreach (var index in resultTypes[resultPos].ParentVertexType.GetIndexDefinitions(true))
                    {
                        _indexManager.CreateIndex(
                            new IndexPredefinition().AddProperty(index.IndexedProperties.Select(x => x.Name)).SetVertexType(current.Value.VertexTypeName).SetIndexType(index.IndexTypeName),
                            mySecurity,
                            myTransaction);
                    }

                    #endregion

                }
            }
            #endregion

            CleanUpTypes();

            return resultTypes;
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
            IDictionary<string, VertexTypePredefinition> myDefsByName,
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
                if (myDefsByName.ContainsKey(group.Key))
                    continue;

                var vertex = Get(group.Key, myTransaction, mySecurity);
                if (vertex == null)
                    throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key,
                                                                group.Select(x => x.AttributeName));
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

        private void ConnectVertexToUniqueIndex(TypeInfo myTypeInfo, IEnumerable<IIndexDefinition> myIndexDefinitions, SecurityToken mySecurity, TransactionToken myTransaction)
        {
            _vertexManager.ExecuteManager.VertexStore.UpdateVertex(
                            mySecurity,
                            myTransaction,
                            myTypeInfo.VertexInfo.VertexID,
                            myTypeInfo.VertexInfo.VertexTypeID,
                            new VertexUpdateDefinition(
                                myHyperEdgeUpdate: new HyperEdgeUpdate(new Dictionary<long, HyperEdgeUpdateDefinition>
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
            IDictionary<string, VertexTypePredefinition> myDefsByName,
            long myFirstID,
            TransactionToken myTransaction,
            SecurityToken mySecurity)
        {
            var neededVertexTypes = new HashSet<string>();

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
                    var vertex = _vertexManager.ExecuteManager.GetSingleVertex(new BinaryExpression(new SingleLiteralExpression(vertexType), BinaryOperator.Equals, _vertexTypeNameExpression), myTransaction, mySecurity);
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
                if (!result.ContainsKey(current.Value.VertexTypeName)) 
                    continue;

                var info = result[current.Value.VertexTypeName];
                info.AttributeCountWithParents = info.AttributeCountWithParents + result[current.Value.SuperVertexTypeName].AttributeCountWithParents;
                result[current.Value.VertexTypeName] = info;
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
        /// <param name="myTypeId"></param>
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

        /// <summary>
        /// Removes the given types from the graphDB.
        /// </summary>
        /// <param name="myVertexTypes">The types to delete.</param>
        /// <param name="myTransaction">Transaction token.</param>
        /// <param name="mySecurity">Security Token.</param>
        /// <param name="myIgnoreReprimands">True means, that reprimands (IncomingEdges) on the types wich should be removed are ignored.</param>
        /// <returns>Set of deleted vertex type IDs.</returns>
        private Dictionary<Int64, String> Remove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, bool myIgnoreReprimands = false)
        {
            //the attribute types on delete types which have to be removed
            var toDeleteAttributeDefinitions = new List<IAttributeDefinition>();

            //the indices on the delete types
            var toDeleteIndexDefinitions = new List<IIndexDefinition>();

            #region get propertydefinitions
            
            //get child vertex types
            foreach (var delType in myVertexTypes)
            {
                //just check edges if reprimands should not be ignored
                if (!myIgnoreReprimands)
                {
                    //TODO: use vertex and check if incoming edges existing of user defined types, instead of foreach
                    var VertexOfDelType = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction, delType.ID, (long)BaseTypes.VertexType, null, 0L);
                    
                    #region check that no other type has a outgoing edge pointing on delete type
                    
                    //get all vertex types and check if they have outgoing edges with target vertex the delete type
                    foreach (var type in GetAllVertexTypes(myTransaction, mySecurity))
                    {
                        if (type.ID != delType.ID && type.GetOutgoingEdgeDefinitions(false).Any(outEdgeDef => outEdgeDef.TargetVertexType.ID.Equals(delType.ID)))
                        {
                            throw new VertexTypeRemoveException(delType.Name, "There are other types which have outgoing edges pointing to the type, which should be removed.");
                        }
                    }

                    #endregion

                    #region check if there are incoming edges of target vertices of outgoing edges of the deleting type

                    foreach (var outEdge in delType.GetOutgoingEdgeDefinitions(false))
                    {
                        if (outEdge.TargetVertexType.GetIncomingEdgeDefinitions(true).Any(inEdge => inEdge.RelatedEdgeDefinition.ID.Equals(outEdge.ID) && inEdge.RelatedType.ID != delType.ID) && !myIgnoreReprimands)
                        {
                            throw new VertexTypeRemoveException(delType.Name, "There are other types which have incoming edges, whose related type is a outgoing edge of the type which should be removed.");
                        }
                    }

                    #endregion
                }

                toDeleteAttributeDefinitions.AddRange(delType.GetAttributeDefinitions(false));

                toDeleteIndexDefinitions.AddRange(delType.GetIndexDefinitions(false));
            }
            
            #endregion

            //the IDs of the deleted vertices
            var deletedTypeIDs = new Dictionary<Int64, String>( myVertexTypes.ToDictionary(key => key.ID, item => item.Name) );

            #region remove indices
            
            //remove indices on types
            foreach (var index in toDeleteIndexDefinitions)
            {
                _indexManager.RemoveIndexInstance(index.ID, myTransaction, mySecurity);
            }
            
            #endregion

            #region remove attribute types on delete types
            
            //delete attribute vertices
            foreach (var attr in toDeleteAttributeDefinitions)
            {
                switch (attr.Kind)
                {
                    case (AttributeType.Property):
                        if (!_vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.Property))
                            throw new VertexTypeRemoveException(myVertexTypes.Where(x => x.HasProperty(attr.Name)).First().Name, "The Property " + attr.Name + " could not be removed.");
                        break;

                    case (AttributeType.IncomingEdge):
                        if(!_vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.IncomingEdge))
                            throw new VertexTypeRemoveException(myVertexTypes.Where(x => x.HasIncomingEdge(attr.Name)).First().Name, "The IncomingEdge " + attr.Name + " could not be removed.");
                        break;

                    case (AttributeType.OutgoingEdge):
                        if(!_vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.OutgoingEdge))
                            throw new VertexTypeRemoveException(myVertexTypes.Where(x => x.HasOutgoingEdge(attr.Name)).First().Name, "The OutgoingEdge " + attr.Name + " could not be removed.");
                        break;

                    case (AttributeType.BinaryProperty):
                        if(!_vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.BinaryProperty))
                            throw new VertexTypeRemoveException(myVertexTypes.Where(x => x.HasBinaryProperty(attr.Name)).First().Name, "The BinaryProperty " + attr.Name + " could not be removed.");
                        break;

                    default:
                        break;
                }
            } 
            
            #endregion

            #region remove vertex type names from index

            foreach (var vertexType in myVertexTypes)
            {
                var result = _indexManager.GetIndex(BaseUniqueIndex.VertexTypeDotName);

                if (result != null)
                {
                    if(!result.Remove(vertexType.Name))
                        throw new VertexTypeRemoveException(vertexType.Name, "Error during delete the Index on type.");
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
                if(!_vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, type.ID, (long)BaseTypes.VertexType))
                    if (_vertexManager.ExecuteManager.VertexStore.VertexExists(mySecurity, myTransaction, type.ID, (long)BaseTypes.VertexType))
                        throw new VertexTypeRemoveException(type.Name, "Could not remove the vertex representing the type.");

            } 
            
            #endregion

            toDeleteAttributeDefinitions.Clear();
            toDeleteIndexDefinitions.Clear();

            CleanUpTypes();

            return deletedTypeIDs;
        }

        /// <summary>
        /// Clears the graphDB, removes all user defined types.
        /// </summary>
        /// <param name="myTransaction">Transaction token.</param>
        /// <param name="mySecurity">Security token.</param>
        /// <returns>Set of deleted vertex type IDs.</returns>
        private IEnumerable<long> Clear(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            //get all UserDefined types and delete them
            var toDeleteVertexTypes = GetAllVertexTypes(myTransaction, mySecurity).Where(x => x.IsUserDefined == true);

            return RemoveVertexTypes(toDeleteVertexTypes, myTransaction, mySecurity, true).Select(x => x.Key).ToList();
        }

        private void LoadBaseType(TransactionToken myTransaction, SecurityToken mySecurity, params BaseTypes[] myBaseTypes)
        {
            foreach (var baseType in myBaseTypes)
            {
                var vertex = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction, (long)baseType, (long)BaseTypes.VertexType, String.Empty);
                if (vertex == null)
                    //TODO: better exception
                    throw new Exception("Could not load base vertex type.");
                _baseTypes.Add((long)baseType, new VertexType(vertex));
                _nameIndex.Add(baseType.ToString(), (long)baseType);
            }
        }

        private long GetMaxID(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.VertexStore.GetVerticesByTypeID(mySecurity, myTransaction, myTypeID);
            
            if (vertices == null)
                //TODO better exception here
                throw new Exception("The base vertex types are not available.");

            return (vertices.CountIsGreater(0))
                ? vertices.Max(x => x.VertexID)
                : Int64.MinValue;
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _edgeManager = myMetaManager.EdgeTypeManager;
            _indexManager = myMetaManager.IndexManager;
            _vertexManager = myMetaManager.VertexManager;
        }

        public override void Load(TransactionToken myTransaction, SecurityToken mySecurity)
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

            TruncateVertexType(vertexType.ID, myTransactionToken, mySecurityToken);
        }

        public override IVertexType AlterVertexType(RequestAlterVertexType myAlterVertexTypeRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var vertexType = GetVertexType(myAlterVertexTypeRequest.VertexTypeName, myTransactionToken, mySecurityToken);

            #region remove stuff

            CheckRemoveOutgoingEdges(myAlterVertexTypeRequest, vertexType, mySecurityToken, myTransactionToken);

            //done
            RemoveMandatoryConstraint(myAlterVertexTypeRequest.ToBeRemovedMandatories, vertexType, myTransactionToken,
                                      mySecurityToken);
            //done
            RemoveUniqueConstraint(myAlterVertexTypeRequest.ToBeRemovedUniques, vertexType, myTransactionToken,
                                      mySecurityToken);

            //done
            RemoveIndices(myAlterVertexTypeRequest.ToBeRemovedIndices, vertexType, myTransactionToken, mySecurityToken);

            //done
            RemoveAttributes(myAlterVertexTypeRequest.ToBeRemovedIncomingEdges,
                             myAlterVertexTypeRequest.ToBeRemovedOutgoingEdges,
                             myAlterVertexTypeRequest.ToBeRemovedProperties, vertexType, myTransactionToken,
                             mySecurityToken);

            #endregion

            #region add stuff

            //done
            AddMandatoryConstraint(myAlterVertexTypeRequest.ToBeAddedMandatories, vertexType, myTransactionToken,
                                   mySecurityToken);

            //done
            AddUniqueConstraint(myAlterVertexTypeRequest.ToBeAddedUniques, myTransactionToken,
                                mySecurityToken);

            //done
            AddIndices(myAlterVertexTypeRequest.ToBeAddedIndices, vertexType, myTransactionToken, mySecurityToken);

            //done
            AddAttributes(myAlterVertexTypeRequest.ToBeAddedBinaryProperties,
                          myAlterVertexTypeRequest.ToBeAddedIncomingEdges,
                          myAlterVertexTypeRequest.ToBeAddedOutgoingEdges,
                          myAlterVertexTypeRequest.ToBeAddedProperties,
                          vertexType, myTransactionToken, mySecurityToken);

            //done
            RenameAttributes(myAlterVertexTypeRequest.ToBeRenamedProperties, vertexType, myTransactionToken,
                             mySecurityToken);

            #endregion

            #region misc

            //done
            ChangeCommentOnVertexType(vertexType, myAlterVertexTypeRequest.AlteredComment, myTransactionToken,mySecurityToken);
            
            //done
            RenameVertexType(vertexType, myAlterVertexTypeRequest.AlteredVertexTypeName, myTransactionToken, mySecurityToken);

            #endregion

            CleanUpTypes();

            return GetVertexType(vertexType.ID, myTransactionToken, mySecurityToken);
        }

        private void CheckRemoveOutgoingEdges(RequestAlterVertexType myAlterVertexTypeRequest, IVertexType myVertexType, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myAlterVertexTypeRequest.ToBeRemovedOutgoingEdges == null)
                return;

            #region get the list of incoming edges that will be deleted too

            var toBeRemovedIncomingEdgeIDs = new List<long>();

            if (myAlterVertexTypeRequest.ToBeRemovedIncomingEdges != null)
                toBeRemovedIncomingEdgeIDs.AddRange(
                    myAlterVertexTypeRequest.ToBeRemovedIncomingEdges.Select(
                        _ => myVertexType.GetIncomingEdgeDefinition(_).ID));

            #endregion

            foreach (var aOutgoingEdge in myAlterVertexTypeRequest.ToBeRemovedOutgoingEdges)
            {
                var attrDef = myVertexType.GetOutgoingEdgeDefinition(aOutgoingEdge);

                var vertex = _vertexManager.ExecuteManager.GetVertex((long) BaseTypes.OutgoingEdge, attrDef.ID, null,
                                                                     null, myTransactionToken, mySecurityToken);

                var incomingEdges = vertex.GetIncomingVertices((long) BaseTypes.IncomingEdge,
                                                               (long)
                                                               AttributeDefinitions.IncomingEdgeDotRelatedEgde);

                foreach (var incomingEdge in incomingEdges)
                {
                    if (!toBeRemovedIncomingEdgeIDs.Contains(incomingEdge.VertexID))
                    {
                        //TODO a better exception here
                        throw new Exception(
                            "The outgoing edge can not be removed, because there are related incoming edges.");
                    }
                }
            }
        }

        /// <summary>
        /// Renames attributes
        /// </summary>
        /// <param name="myToBeRenamedAttributes"></param>
        /// <param name="vertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void RenameAttributes(Dictionary<string, string> myToBeRenamedAttributes, IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (!myToBeRenamedAttributes.IsNotNullOrEmpty()) 
                return;

            foreach (var aToBeRenamedAttribute in myToBeRenamedAttributes)
            {
                VertexUpdateDefinition update = new VertexUpdateDefinition(null, new StructuredPropertiesUpdate(new Dictionary<long, IComparable> { { (long)AttributeDefinitions.AttributeDotName, aToBeRenamedAttribute.Value } }));
                var attribute = vertexType.GetAttributeDefinition(aToBeRenamedAttribute.Key);

                switch (attribute.Kind)
                {
                    case AttributeType.Property:
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, attribute.ID, (long)BaseTypes.Property, update);
                        break;

                    case AttributeType.IncomingEdge:
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, attribute.ID, (long)BaseTypes.IncomingEdge, update);

                        break;
                    case AttributeType.OutgoingEdge:
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, attribute.ID, (long)BaseTypes.OutgoingEdge, update);

                        break;
                    case AttributeType.BinaryProperty:
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, attribute.ID, (long)BaseTypes.BinaryProperty, update);

                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Adds attributes
        /// </summary>
        /// <param name="myToBeAddedBinaryProperties"></param>
        /// <param name="myToBeAddedIncomingEdges"></param>
        /// <param name="myToBeAddedOutgoingEdges"></param>
        /// <param name="myToBeAddedProperties"></param>
        /// <param name="vertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void AddAttributes(
            IEnumerable<BinaryPropertyPredefinition> myToBeAddedBinaryProperties, 
            IEnumerable<IncomingEdgePredefinition> myToBeAddedIncomingEdges, 
            IEnumerable<OutgoingEdgePredefinition> myToBeAddedOutgoingEdges, 
            IEnumerable<PropertyPredefinition> myToBeAddedProperties, 
            IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            
            if (myToBeAddedProperties.IsNotNullOrEmpty())
            {
                ProcessAddPropery(myToBeAddedProperties, mySecurityToken, myTransactionToken, vertexType);

                CleanUpTypes();
            }

            if (myToBeAddedBinaryProperties.IsNotNullOrEmpty())
            {
                ProcessAddBinaryPropery(myToBeAddedBinaryProperties, mySecurityToken, myTransactionToken, vertexType);
            }

            if (myToBeAddedOutgoingEdges.IsNotNullOrEmpty())
            {
                CleanUpTypes();
                ProcessAddOutgoingEdges(myToBeAddedOutgoingEdges, mySecurityToken, myTransactionToken, vertexType);
            }

            if (myToBeAddedIncomingEdges.IsNotNullOrEmpty())
            {
                CleanUpTypes();
                ProcessAddIncomingEdges(myToBeAddedIncomingEdges, mySecurityToken, myTransactionToken, vertexType);
            }

        }

        private void ProcessAddOutgoingEdges(IEnumerable<OutgoingEdgePredefinition> myToBeAddedOutgoingEdges, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexType vertexType)
        {
            foreach (var aToBeAddedOutgoingEdge in myToBeAddedOutgoingEdges)
            {
                var edgeType = _edgeManager.ExecuteManager.GetEdgeType(aToBeAddedOutgoingEdge.EdgeType, myTransactionToken, mySecurityToken);
                var innerEdgeType = (aToBeAddedOutgoingEdge.InnerEdgeType != null)
                                        ? _edgeManager.ExecuteManager.GetEdgeType(aToBeAddedOutgoingEdge.InnerEdgeType, myTransactionToken, mySecurityToken)
                                        : null;
                VertexInformation? innerEdgeTypeInfo = null;
                if (innerEdgeType != null)
                    innerEdgeTypeInfo = new VertexInformation((long) BaseTypes.EdgeType, innerEdgeType.ID);

                var targetVertexType = GetVertexType(aToBeAddedOutgoingEdge.AttributeType, myTransactionToken, mySecurityToken);

                BaseGraphStorageManager.StoreOutgoingEdge(_vertexManager.ExecuteManager.VertexStore,
                    new VertexInformation((long)BaseTypes.OutgoingEdge,
                        _idManager[(long)BaseTypes.Attribute].GetNextID()), aToBeAddedOutgoingEdge.AttributeName,
                        aToBeAddedOutgoingEdge.Comment,
                        true,
                        DateTime.UtcNow.ToBinary(),
                        aToBeAddedOutgoingEdge.Multiplicity,
                        new VertexInformation((long)BaseTypes.VertexType, vertexType.ID),
                        new VertexInformation((long)BaseTypes.EdgeType, edgeType.ID),
                        innerEdgeTypeInfo,
                        new VertexInformation((long)BaseTypes.VertexType, targetVertexType.ID),
                        mySecurityToken, myTransactionToken);
            }
        }

        private void ProcessAddPropery(IEnumerable<PropertyPredefinition> myToBeAddedProperties, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexType vertexType)
        {
            foreach (var aProperty in myToBeAddedProperties)
            {
                BaseGraphStorageManager.StoreProperty(
                    _vertexManager.ExecuteManager.VertexStore,
                    new VertexInformation((long)BaseTypes.Property,
                        _idManager[(long)BaseTypes.Attribute].GetNextID()),
                        aProperty.AttributeName,
                        aProperty.Comment,
                        DateTime.UtcNow.ToBinary(),
                        aProperty.IsMandatory,
                        aProperty.Multiplicity,
                        aProperty.DefaultValue,
                        true,
                        new VertexInformation(
                            (long)BaseTypes.VertexType,
                            vertexType.ID),
                        ConvertBasicType(aProperty.AttributeType),
                        mySecurityToken,
                        myTransactionToken);
            }
        }

        private void ProcessAddIncomingEdges(IEnumerable<IncomingEdgePredefinition> myToBeAddedIncomingEdges, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexType vertexType)
        {
            foreach (var aIncomingEdgeProperty in myToBeAddedIncomingEdges)
            {
                var targetVertexType = GetVertexType(GetTargetVertexTypeFromAttributeType(aIncomingEdgeProperty.AttributeType), myTransactionToken, mySecurityToken);
                var targetOutgoingEdge = targetVertexType.GetOutgoingEdgeDefinition(GetTargetEdgeNameFromAttributeType(aIncomingEdgeProperty.AttributeType));

                BaseGraphStorageManager.StoreIncomingEdge(
                    _vertexManager.ExecuteManager.VertexStore,
                    new VertexInformation((long)BaseTypes.IncomingEdge, _idManager[(long)BaseTypes.Attribute].GetNextID()),
                        aIncomingEdgeProperty.AttributeName,
                        aIncomingEdgeProperty.Comment,
                        true,
                        DateTime.UtcNow.ToBinary(),
                        new VertexInformation(
                            (long)BaseTypes.VertexType,
                            vertexType.ID),
                            new VertexInformation((long)BaseTypes.OutgoingEdge, targetOutgoingEdge.ID),
                        mySecurityToken,
                        myTransactionToken);
            }
        }

        private void ProcessAddBinaryPropery(IEnumerable<BinaryPropertyPredefinition> myToBeAddedBinaryProperties, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IVertexType vertexType)
        {
            foreach (var aBinaryProperty in myToBeAddedBinaryProperties)
            {
                BaseGraphStorageManager.StoreBinaryProperty(
                    _vertexManager.ExecuteManager.VertexStore, 
                    new VertexInformation((long)BaseTypes.BinaryProperty,
                        _idManager[(long)BaseTypes.Attribute].GetNextID()), 
                        aBinaryProperty.AttributeName, 
                        aBinaryProperty.Comment, 
                        true, 
                        DateTime.UtcNow.ToBinary(), 
                        new VertexInformation(
                            (long)BaseTypes.VertexType, 
                            vertexType.ID), 
                        mySecurityToken, 
                        myTransactionToken);
            }
        }

        /// <summary>
        /// Adds indices
        /// </summary>
        /// <param name="myToBeAddedIndices"></param>
        /// <param name="vertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void AddIndices(IEnumerable<IndexPredefinition> myToBeAddedIndices, IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (!myToBeAddedIndices.IsNotNullOrEmpty()) 
                return;

            foreach (var aToBeAddedIndex in myToBeAddedIndices)
            {
                var predef = (aToBeAddedIndex.Name == null)
                                ? new IndexPredefinition()
                                : new IndexPredefinition(aToBeAddedIndex.Name);

                predef.AddProperty(aToBeAddedIndex.Properties).SetEdition(aToBeAddedIndex.Edition).SetVertexType(vertexType.Name);

                _indexManager.CreateIndex(predef, mySecurityToken, myTransactionToken);
            }
        }

        /// <summary>
        /// Adds a unique constraint
        /// </summary>
        /// <param name="myToBeAddedUniques"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void AddUniqueConstraint(IEnumerable<UniquePredefinition> myToBeAddedUniques, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (!myToBeAddedUniques.IsNotNullOrEmpty()) 
                return;

            foreach (var aUniqueConstraint in myToBeAddedUniques)
            {
                var predef = new IndexPredefinition();
                predef.AddProperty(aUniqueConstraint.Properties);

                _indexManager.CreateIndex(predef, mySecurityToken, myTransactionToken);
            }
        }

        /// <summary>
        /// Adds a mandatory constraint
        /// </summary>
        /// <param name="myToBeAddedMandatories"></param>
        /// <param name="vertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void AddMandatoryConstraint(IEnumerable<MandatoryPredefinition> myToBeAddedMandatories, IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (myToBeAddedMandatories.IsNotNullOrEmpty())
            {
                foreach (var aMandatory in myToBeAddedMandatories)
                {
                    var property = vertexType.GetPropertyDefinition(aMandatory.MandatoryAttribute);
                    var defaultValue = property.DefaultValue;

                    //get new mandatory value and set it
                    if (aMandatory.DefaultValue != null)
                    {
                        var defaultValueUpdate = new VertexUpdateDefinition(null, new StructuredPropertiesUpdate(new Dictionary<long, IComparable> { { (long)AttributeDefinitions.PropertyDotDefaultValue, aMandatory.DefaultValue.ToString() } }));
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, property.ID, (long)BaseTypes.Property, defaultValueUpdate);

                        defaultValue = aMandatory.DefaultValue.ToString();
                    }

                    var vertexDefaultValueUpdate = new VertexUpdateDefinition(null, new StructuredPropertiesUpdate(new Dictionary<long, IComparable> { { property.ID, defaultValue } }));

                    foreach (var aVertexType in vertexType.GetDescendantVertexTypesAndSelf())
                    {
                        foreach (var aVertexWithoutPropery in _vertexManager.ExecuteManager.VertexStore.GetVerticesByTypeID(mySecurityToken, myTransactionToken, vertexType.ID).Where(_ => !_.HasProperty(property.ID)).ToList())
                        {
                            if (defaultValue != null)
                            {
                                //update
                                _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, aVertexWithoutPropery.VertexID, aVertexWithoutPropery.VertexTypeID, vertexDefaultValueUpdate);
                            }
                            else
                            {
                                throw new MandatoryConstraintViolationException(aMandatory.MandatoryAttribute);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes attributes
        /// </summary>
        /// <param name="myToBeRemovedIncomingEdges"></param>
        /// <param name="myToBeRemovedOutgoingEdges"></param>
        /// <param name="myToBeRemovedProperties"></param>
        /// <param name="vertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void RemoveAttributes(IEnumerable<string> myToBeRemovedIncomingEdges, IEnumerable<string> myToBeRemovedOutgoingEdges, IEnumerable<string> myToBeRemovedProperties, IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (myToBeRemovedIncomingEdges.IsNotNullOrEmpty() || myToBeRemovedOutgoingEdges.IsNotNullOrEmpty() || myToBeRemovedProperties.IsNotNullOrEmpty())
            {
                if (myToBeRemovedIncomingEdges.IsNotNullOrEmpty())
                {
                    ProcessIncomingEdgeRemoval(myToBeRemovedIncomingEdges, vertexType, myTransactionToken, mySecurityToken);
                }

                if (myToBeRemovedOutgoingEdges.IsNotNullOrEmpty())
                {
                    ProcessOutgoingEdgeRemoval(myToBeRemovedOutgoingEdges, vertexType, myTransactionToken, mySecurityToken);
                }

                if (myToBeRemovedProperties.IsNotNullOrEmpty())
                {
                    ProcessPropertyRemoval(myToBeRemovedProperties, vertexType, myTransactionToken, mySecurityToken);
                }
            }
        }

        private void ProcessPropertyRemoval(IEnumerable<string> myToBeRemovedProperties, IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            foreach (var aProperty in myToBeRemovedProperties)
            {
                #region remove related indices
                
                var propertyDefinition = vertexType.GetPropertyDefinition(aProperty);

                foreach (var aIndexDefinition in propertyDefinition.InIndices)
                {
                    _indexManager.RemoveIndexInstance(aIndexDefinition.ID, myTransactionToken, mySecurityToken);
                }

                #endregion

                _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurityToken, myTransactionToken, propertyDefinition.ID, (long)BaseTypes.Property);
            }
        }

        private void ProcessOutgoingEdgeRemoval(IEnumerable<string> myToBeRemovedOutgoingEdges, IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            foreach (var aOutgoingEdge in myToBeRemovedOutgoingEdges)
            {
                var outgoingEdgeDefinition = vertexType.GetOutgoingEdgeDefinition(aOutgoingEdge);

                _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurityToken, myTransactionToken, outgoingEdgeDefinition.ID, (long)BaseTypes.OutgoingEdge);
            }
        }

        private void ProcessIncomingEdgeRemoval(IEnumerable<string> myToBeRemovedIncomingEdges, IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            foreach (var aIncomingEdge in myToBeRemovedIncomingEdges)
            {
                var incomingEdgeDefinition = vertexType.GetIncomingEdgeDefinition(aIncomingEdge);

                _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurityToken, myTransactionToken, incomingEdgeDefinition.ID, (long)BaseTypes.IncomingEdge);
            }
        }

        /// <summary>
        /// Removes indices
        /// </summary>
        /// <param name="myToBeRemovedIndices"></param>
        /// <param name="vertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void RemoveIndices(Dictionary<string, string> myToBeRemovedIndices, IVertexType vertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (myToBeRemovedIndices.IsNotNullOrEmpty())
            {
                foreach (var aIndex in myToBeRemovedIndices)
                {
                    //find the source
                    IIndexDefinition sourceIndexDefinition = vertexType.GetIndexDefinitions(false).Where(_ => _.Name == aIndex.Key && _.Edition == aIndex.Value).FirstOrDefault();

                    foreach (var aVertexType in vertexType.GetDescendantVertexTypes())
                    {
                        foreach (var aInnerIndex in aVertexType.GetIndexDefinitions(false).Where(_=>_.SourceIndex.ID == sourceIndexDefinition.ID))
                        {
                            _indexManager.RemoveIndexInstance(aInnerIndex.ID, myTransactionToken, mySecurityToken);                                                             
                        }
                    }

                    _indexManager.RemoveIndexInstance(sourceIndexDefinition.ID, myTransactionToken, mySecurityToken);                                                             
                }
            }
        }

        /// <summary>
        /// Removes unique constaints
        /// </summary>
        /// <param name="myUniqueConstraints"></param>
        /// <param name="myVertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void RemoveUniqueConstraint(IEnumerable<string> myUniqueConstraints, IVertexType myVertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (myUniqueConstraints.IsNotNullOrEmpty())
            {
                foreach (var aUniqueConstraint in myUniqueConstraints)
                {
                    foreach (var item in myVertexType.GetDescendantVertexTypesAndSelf())
                    {
                        foreach (var aUniqueDefinition in item.GetUniqueDefinitions(false))
                        {
                            if (aUniqueDefinition.CorrespondingIndex.IndexedProperties.All(_ => _.Name == aUniqueConstraint))
                            {
                                _indexManager.RemoveIndexInstance(aUniqueDefinition.CorrespondingIndex.ID, myTransactionToken, mySecurityToken);                                 
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Removes mandatory constraits
        /// </summary>
        /// <param name="myToBeRemovedMandatories"></param>
        /// <param name="myVertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void RemoveMandatoryConstraint(IEnumerable<string> myToBeRemovedMandatories, IVertexType myVertexType, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (myToBeRemovedMandatories.IsNotNullOrEmpty())
            {
                var update = new VertexUpdateDefinition(null, new StructuredPropertiesUpdate(new Dictionary<long, IComparable> { { (long)AttributeDefinitions.PropertyDotIsMandatory, false } }));

                foreach (var aMandatory in myToBeRemovedMandatories)
                {
                    IPropertyDefinition property = myVertexType.GetPropertyDefinition(aMandatory);

                    _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, property.ID, (long)BaseTypes.Property, null);
                }
            }
        }

        /// <summary>
        /// Change the comment on the vertex type
        /// </summary>
        /// <param name="vertexType"></param>
        /// <param name="myNewComment"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void ChangeCommentOnVertexType(IVertexType vertexType, string myNewComment, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (!String.IsNullOrEmpty(myNewComment))
            {
                var update = new VertexUpdateDefinition(myNewComment);

                _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, vertexType.ID, (long)BaseTypes.VertexType, update);
            }
        }

        /// <summary>
        /// Renames a vertex type
        /// </summary>
        /// <param name="vertexType"></param>
        /// <param name="myNewVertexTypeName"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void RenameVertexType(IVertexType vertexType, string myNewVertexTypeName, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            if (myNewVertexTypeName != null)
            {
                var update = new VertexUpdateDefinition(null, new StructuredPropertiesUpdate(new Dictionary<long, IComparable> { { (long)AttributeDefinitions.BaseTypeDotName, myNewVertexTypeName } }));

                _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, myTransactionToken, vertexType.ID, (long)BaseTypes.VertexType, update);
            }
        }

        public override bool HasVertexType(string myAlteredVertexTypeName, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (String.IsNullOrWhiteSpace(myAlteredVertexTypeName))
                throw new ArgumentOutOfRangeException("myAlteredVertexTypeName", "The type name must contain at least one character.");

            #region get static types

            if (_nameIndex.ContainsKey(myAlteredVertexTypeName))
            {
                return true;
            }

            #endregion

            var vertex = Get(myAlteredVertexTypeName, myTransactionToken, mySecurityToken);

            return vertex != null;
        }

        public override void CleanUpTypes()
        {
            var help = new Dictionary<long, IVertexType>(_baseTypes);

            _baseTypes.Clear();
            _nameIndex.Clear();

            foreach (var type in new[] { BaseTypes.Attribute, BaseTypes.BaseType, BaseTypes.BinaryProperty, BaseTypes.EdgeType, BaseTypes.IncomingEdge, BaseTypes.Index, BaseTypes.OutgoingEdge, BaseTypes.VertexType })
            {
                _baseTypes.Add((long)type, help[(long)type]);
                _nameIndex.Add(type.ToString(), (long)type);
            }
        }
    }
}
