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
using sones.Library.LanguageExtensions;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.BaseGraph;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Manager.Vertex;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.GraphDB.Request;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.VertexStore.Definitions.Update;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class ExecuteVertexTypeManager: AExecuteTypeManager<IVertexType>
    {
        #region Data

        private IManagerOf<ITypeHandler<IEdgeType>> _edgeManager;

        #endregion

        #region constructor

        public ExecuteVertexTypeManager(IDManager myIDManager)
        {
            _idManager = myIDManager;

            _baseTypes = new Dictionary<long, IBaseType>();
            _nameIndex = new Dictionary<String, long>();
        }

        #endregion

        #region ACheckTypeManager member

        public override IVertexType AlterType(IRequestAlterType myAlterTypeRequest, 
                                                TransactionToken myTransactionToken, 
                                                SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IVertexType> GetAllTypes(TransactionToken myTransaction,
                                                                SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.GetVertices(BaseTypes.VertexType.ToString(), 
                                                                        myTransaction, 
                                                                        mySecurity, 
                                                                        false);

            return vertices == null ? Enumerable.Empty<IVertexType>() 
                                    : vertices.Select(x => new VertexType(x, _baseStorageManager));
        }

        public override Dictionary<long, string> RemoveTypes(IEnumerable<IVertexType> myTypes,
                                                                TransactionToken myTransaction,
                                                                SecurityToken mySecurity,
                                                                bool myIgnoreReprimands = false)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<long> ClearTypes(TransactionToken myTransaction,
                                                        SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public override void TruncateType(long myTypeID,
                                            TransactionToken myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public override void TruncateType(string myTypeName,
                                            TransactionToken myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public override void CleanUpTypes()
        {
            var help = new Dictionary<long, IBaseType>(_baseTypes);

            _baseTypes.Clear();
            _nameIndex.Clear();

            foreach (var type in new[] { BaseTypes.Attribute, 
                                            BaseTypes.BaseType, 
                                            BaseTypes.BinaryProperty, 
                                            BaseTypes.EdgeType, 
                                            BaseTypes.IncomingEdge, 
                                            BaseTypes.Index, 
                                            BaseTypes.OutgoingEdge, 
                                            BaseTypes.VertexType })
            {
                _baseTypes.Add((long)type, help[(long)type]);
                _nameIndex.Add(type.ToString(), (long)type);
            }
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _edgeManager            = myMetaManager.EdgeTypeManager;
            _indexManager           = myMetaManager.IndexManager;
            _vertexManager          = myMetaManager.VertexManager;
            _baseTypeManager        = myMetaManager.BaseTypeManager;
            _baseStorageManager     = myMetaManager.BaseGraphStorageManager;
        }

        public override void Load(TransactionToken myTransaction,
                                    SecurityToken mySecurity)
        {
            _idManager.VertexTypeID.SetToMaxID(GetMaxID((long)BaseTypes.VertexType, 
                                                        myTransaction, 
                                                        mySecurity) + 1);
            _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute).SetToMaxID(
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

        protected override IVertexType CreateType(IVertex myVertex)
        {
            var result = new VertexType(myVertex, _baseStorageManager);

            _baseTypes.Add(result.ID, result);
            _nameIndex.Add(result.Name, result.ID);

            return result;
        }

        #endregion

        #region private helper

        #region abstract private helper

        /// <summary>
        /// Adds a type by reading out the predefinitions and stores all attributes and the type.
        /// </summary>
        /// <param name="myTypePredefinitions">The predefinitions for the creation.</param>
        /// <param name="myTransaction">TransactionToken</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <returns>The created types.</returns>
        protected override IEnumerable<IVertexType> Add(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                        TransactionToken myTransaction,
                                                        SecurityToken mySecurity)
        {
            #region preparations

            var typePredefinitions = myTypePredefinitions;// as IEnumerable<VertexTypePredefinition>;

            //Perf: count is necessary, fast if it is an ICollection
            var count = typePredefinitions.Count();

            //This operation reserves #count ids for this operation.
            var firstTypeID = _idManager.VertexTypeID.ReserveIDs(count);

            //Contains dictionary of vertex name to vertex predefinition.
            var defsByVertexName = CanAddCheckDuplicates(typePredefinitions);

            //Contains dictionary of parent vertex name to list of vertex predefinitions.
            var defsByParentVertexName = typePredefinitions
                .GroupBy(def => def.SuperTypeName)
                .ToDictionary(group => group.Key, group => group.AsEnumerable());

            //Contains list of vertex predefinitions sorted topologically.
            var defsTopologically = CanAddSortTopolocically(defsByVertexName, 
                                                                defsByParentVertexName
                                                                .ToDictionary(kvp => kvp.Key,
                                                                              kvp => kvp.Value));// as IEnumerable<ATypePredefinition>));

            CanAddCheckWithFS(defsTopologically, defsByVertexName, myTransaction, mySecurity);

            var typeInfos = GenerateTypeInfos(defsTopologically, defsByVertexName, firstTypeID, myTransaction, mySecurity);

            //we can add each type separately
            var creationDate = DateTime.UtcNow.ToBinary();
            var resultPos = 0;

            var result = new IVertex[count];

            #endregion

            #region store vertex type

            //now we store each vertex type
            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                var newVertexType = _baseStorageManager.StoreVertexType(
                     _vertexManager.ExecuteManager.VertexStore,
                     typeInfos[current.Value.TypeName].VertexInfo,
                     current.Value.TypeName,
                     current.Value.Comment,
                     creationDate,
                     current.Value.IsAbstract,
                     current.Value.IsSealed,
                     true,
                     typeInfos[current.Value.SuperTypeName].VertexInfo,
                     null,
                     mySecurity,
                     myTransaction);

                result[resultPos++] = newVertexType;

                _indexManager.GetIndex(BaseUniqueIndex.VertexTypeDotName)
                                .Add(current.Value.TypeName, typeInfos[current.Value.TypeName].VertexInfo.VertexID);

                _nameIndex.Add(current.Value.TypeName, typeInfos[current.Value.TypeName].VertexInfo.VertexID);
            }

            #endregion

            #region Store Attributes

            //The order of adds is important. First property, then outgoing edges (that might point to properties) and finally incoming edges (that might point to outgoing edges)
            //Do not try to merge it into one for block.

            #region Store properties

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.Properties == null)
                    continue;

                var firstAttrID = _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute)
                                            .ReserveIDs(current.Value.PropertyCount);

                var currentExternID = typeInfos[current.Value.TypeName].AttributeCountWithParents - current.Value.PropertyCount - 1;

                foreach (var prop in current.Value.Properties)
                {
                    _baseStorageManager.StoreProperty(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.Property, firstAttrID++),
                        prop.AttributeName,
                        prop.Comment,
                        creationDate,
                        prop.IsMandatory,
                        prop.Multiplicity,
                        prop.DefaultValue,
                        true,
                        typeInfos[current.Value.TypeName].VertexInfo,
                        ConvertBasicType(prop.AttributeType),
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store binary properties

            var vertexDefsTopologically = ConvertLinkedList(defsTopologically);

            for (var current = vertexDefsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.BinaryProperties == null)
                    continue;

                var firstAttrID = _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute).ReserveIDs(current.Value.BinaryPropertyCount);

                var currentExternID = typeInfos[current.Value.TypeName].AttributeCountWithParents 
                                        - current.Value.PropertyCount 
                                        - current.Value.BinaryPropertyCount 
                                        - 1;

                foreach (var prop in current.Value.BinaryProperties)
                {
                    _baseStorageManager.StoreBinaryProperty(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.BinaryProperty, firstAttrID++),
                        prop.AttributeName,
                        prop.Comment,
                        true,
                        creationDate,
                        typeInfos[current.Value.TypeName].VertexInfo,
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store outgoing edges

            for (var current = vertexDefsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.OutgoingEdges == null)
                    continue;

                var firstAttrID = _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute).ReserveIDs(current.Value.OutgoingEdgeCount);

                var currentExternID = typeInfos[current.Value.TypeName].AttributeCountWithParents 
                                        - current.Value.PropertyCount 
                                        - current.Value.OutgoingEdgeCount 
                                        - current.Value.BinaryPropertyCount 
                                        - 1;

                foreach (var edge in current.Value.OutgoingEdges)
                {
                    VertexInformation? innerEdgeType = null;
                    if (edge.Multiplicity == EdgeMultiplicity.MultiEdge)
                    {
                        innerEdgeType = new VertexInformation((long)BaseTypes.EdgeType, 
                                                                _edgeManager.ExecuteManager.GetType(edge.InnerEdgeType, myTransaction, mySecurity).ID);
                    }

                    _baseStorageManager.StoreOutgoingEdge(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.OutgoingEdge, firstAttrID++),
                        edge.AttributeName,
                        edge.Comment,
                        true,
                        creationDate,
                        edge.Multiplicity,
                        typeInfos[current.Value.TypeName].VertexInfo,
                        new VertexInformation((long)BaseTypes.EdgeType, 
                                                _edgeManager.ExecuteManager.GetType(edge.EdgeType, myTransaction, mySecurity).ID),
                        innerEdgeType,
                        typeInfos[edge.AttributeType].VertexInfo,
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store incoming edges

            for (var current = vertexDefsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.IncomingEdges == null)
                    continue;

                var firstAttrID = _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute)
                                                .ReserveIDs(current.Value.IncomingEdgeCount);

                var currentExternID = typeInfos[current.Value.TypeName].AttributeCountWithParents 
                                        - current.Value.PropertyCount 
                                        - current.Value.BinaryPropertyCount 
                                        - current.Value.OutgoingEdgeCount 
                                        - current.Value.IncomingEdgeCount 
                                        - 1;

                foreach (var edge in current.Value.IncomingEdges)
                {

                    _baseStorageManager.StoreIncomingEdge(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.IncomingEdge, firstAttrID++),
                        edge.AttributeName,
                        edge.Comment,
                        true,
                        creationDate,
                        typeInfos[current.Value.TypeName].VertexInfo,
                        GetOutgoingEdgeVertexInformation(GetTargetVertexTypeFromAttributeType(edge.AttributeType), 
                                                            GetTargetEdgeNameFromAttributeType(edge.AttributeType), 
                                                            myTransaction, 
                                                            mySecurity),
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            var resultTypes = new VertexType[result.Length];

            #region reload the stored types

            //reload the IVertex objects, that represents the type.
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction,
                                                                                result[i].VertexID,
                                                                                result[i].VertexTypeID, String.Empty);

                var newVertexType = new VertexType(result[i], _baseStorageManager);
                
                resultTypes[i] = newVertexType;
                
                _baseTypes.Add(typeInfos[newVertexType.Name].VertexInfo.VertexID, newVertexType);
            }

            #endregion

            #endregion

            #region Add Indices

            if (_indexManager != null)
            {
                var uniqueIdx = _indexManager.GetBestMatchingIndexName(true, false, false);

                var indexIdx = _indexManager.GetBestMatchingIndexName(false, false, false);

                resultPos = 0;
                for (var current = vertexDefsTopologically.First; current != null; current = current.Next, resultPos++)
                {
                    #region Uniqueness

                    #region own uniques

                    if (current.Value.Uniques != null)
                    {
                        var indexPredefs = current.Value.Uniques.Select(unique =>
                            new IndexPredefinition(current.Value.TypeName)
                                                    .AddProperty(unique.Properties)
                                                    .SetIndexType(uniqueIdx));

                        var indexDefs = indexPredefs.Select(indexPredef => _indexManager
                                                                                .CreateIndex(indexPredef, 
                                                                                                mySecurity, 
                                                                                                myTransaction, 
                                                                                                false)).ToArray();

                        //only own unique indices are connected to the vertex type on the UniquenessDefinitions attribute
                        ConnectVertexToUniqueIndex(typeInfos[current.Value.TypeName], indexDefs, mySecurity, myTransaction);
                    }

                    #endregion

                    #region parent uniques

                    foreach (var unique in resultTypes[resultPos].ParentVertexType.GetUniqueDefinitions(true))
                    {
                        _indexManager.CreateIndex(
                            new IndexPredefinition(unique.DefiningVertexType.Name)
                                                    .AddProperty(unique.UniquePropertyDefinitions.Select(x => x.Name))
                                                    .SetIndexType(uniqueIdx),
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
                            new IndexPredefinition(current.Value.TypeName)
                                                    .AddProperty(index.IndexedProperties.Select(x => x.Name))
                                                    .SetIndexType(index.IndexTypeName),
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
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeName"/>.
        /// </summary>
        /// <param name="myTypeName">The vertex type name.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given name or <c>NULL</c>, if not present.</returns>
        protected override IVertex Get(string myTypeName,
                                        TransactionToken myTransaction,
                                        SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.ExecuteManager
                    .GetSingleVertex(new BinaryExpression(_VertexTypeNameExpression,
                                                            BinaryOperator.Equals,
                                                            new SingleLiteralExpression(myTypeName)),
                                        myTransaction, mySecurity);

            #endregion
        }

        #endregion

        /// <summary>
        /// Gets the max id of the given type id.
        /// </summary>
        /// <param name="myTypeID">The type id.</param>
        /// <param name="myTransaction">TransactionToken</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <returns>The max id.</returns>
        private long GetMaxID(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.VertexStore.GetVerticesByTypeID(mySecurity, 
                                                                                            myTransaction, 
                                                                                            myTypeID);

            if (vertices == null)
                throw new BaseVertexTypeNotExistException("The base vertex types are not available during loading the ExecuteVertexTypeManager.");

            return (vertices.CountIsGreater(0))
                ? vertices.Max(x => x.VertexID)
                : Int64.MinValue;
        }

        /// <summary>
        /// Loads the base types.
        /// </summary>
        /// <param name="myTransaction">TransactionToken</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <param name="myBaseTypes">The base types.</param>
        private void LoadBaseType(TransactionToken myTransaction, SecurityToken mySecurity, params BaseTypes[] myBaseTypes)
        {
            foreach (var baseType in myBaseTypes)
            {
                var vertex = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, 
                                                                                    myTransaction, 
                                                                                    (long)baseType, 
                                                                                    (long)BaseTypes.VertexType, 
                                                                                    String.Empty);

                if (vertex == null)
                    throw new BaseVertexTypeNotExistException(baseType.ToString(), "Could not load base vertex type during loading the ExecuteVertexTypeManager.");

                _baseTypes.Add((long)baseType, new VertexType(vertex, _baseStorageManager));
                _nameIndex.Add(baseType.ToString(), (long)baseType);
            }
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
        protected override void CanAddCheckWithFS(LinkedList<ATypePredefinition> myDefsTopologically,
                                                    IDictionary<string, ATypePredefinition> myDefsByName,
                                                    TransactionToken myTransaction, SecurityToken mySecurity)
        {

            //Contains the vertex type name to the attribute names of the vertex type.
            var attributes = new Dictionary<String, HashSet<String>>(myDefsTopologically.Count);

            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                CanAddCheckVertexNameUniqueWithFS(current.Value, myTransaction, mySecurity);
                CanAddCheckAttributeNameUniquenessWithFS(current, attributes, myTransaction, mySecurity);
                CanAddCheckOutgoingEdgeTargets(current.Value as VertexTypePredefinition, 
                                                myDefsByName, 
                                                myTransaction, 
                                                mySecurity);
                CanAddCheckIncomingEdgeSources(current.Value as VertexTypePredefinition, 
                                                myDefsByName, 
                                                myTransaction, 
                                                mySecurity);
            }
        }

        /// <summary>
        /// Checks if the attribute names on vertex type definitions are unique, containing parent myAttributes.
        /// </summary>
        /// <param name="myTopologicallySortedPointer">A pointer to a vertex type predefinitions in a topologically sorted linked list.</param>
        /// <param name="myAttributes">A dictionary vertex type name to attribute names, that is build up during the process of CanAddCheckWithFS.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        protected override void CanAddCheckAttributeNameUniquenessWithFS(LinkedListNode<ATypePredefinition> myTopologicallySortedPointer, 
                                                                            IDictionary<string, HashSet<string>> myAttributes, 
                                                                            TransactionToken myTransaction, 
                                                                            SecurityToken mySecurity)
        {
            var parentPredef = GetParentPredefinitionOnTopologicallySortedList(myTopologicallySortedPointer);

            if (parentPredef == null)
            {
                //Get the parent type from FS.
                var parent = Get(myTopologicallySortedPointer.Value.SuperTypeName, myTransaction, mySecurity);

                if (parent == null)
                    //No parent type was found.
                    throw new TypeDoesNotExistException(myTopologicallySortedPointer.Value.SuperTypeName, typeof(IVertexType).Name);

                if (parent.GetProperty<bool>((long)AttributeDefinitions.BaseTypeDotIsSealed))
                    //The parent type is sealed.
                    throw new SealedBaseVertexTypeException(myTopologicallySortedPointer.Value.TypeName, 
                                                            parent.GetPropertyAsString((long)AttributeDefinitions.AttributeDotName));

                var parentType = new VertexType(parent, _baseStorageManager);
                var attributeNames = parentType.GetAttributeDefinitions(true).Select(_ => _.Name);

                myAttributes[myTopologicallySortedPointer.Value.TypeName] = new HashSet<string>(attributeNames);
            }
            else
            {
                myAttributes[myTopologicallySortedPointer.Value.TypeName] = new HashSet<string>(myAttributes[parentPredef.Value.TypeName]);
            }

            var attributeNamesSet = myAttributes[myTopologicallySortedPointer.Value.TypeName];

            CheckIncomingEdgesUniqueName(myTopologicallySortedPointer.Value as VertexTypePredefinition, 
                                            attributeNamesSet);
            CheckOutgoingEdgesUniqueName(myTopologicallySortedPointer.Value as VertexTypePredefinition, 
                                            attributeNamesSet);
            CheckPropertiesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
        }

        /// <summary>
        /// Checks if an incoming myEdge has a coresponding outgoing myEdge.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type definition of that the incoming edges will be checked,</param>
        /// <param name="myDefsByName">The vertex type predefinitions indexed by their name that are alse defined in this CanAdd operation.
        /// <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckIncomingEdgeSources(VertexTypePredefinition myVertexTypePredefinition, 
                                                    IDictionary<string, ATypePredefinition> myDefsByName, 
                                                    TransactionToken myTransaction, 
                                                    SecurityToken mySecurity)
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
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, 
                                                                    group.Key, 
                                                                    group.Select(x => x.AttributeName));

                    var attributes = vertex.GetIncomingVertices((long)BaseTypes.OutgoingEdge, 
                                                                (long)AttributeDefinitions.AttributeDotDefiningType);

                    foreach (var edge in group)
                    {
                        if (!attributes.Any(outgoing => GetTargetVertexTypeFromAttributeType(edge.AttributeName)
                                                        .Equals(outgoing.GetPropertyAsString((long)AttributeDefinitions.AttributeDotName))))
                            throw new OutgoingEdgeNotFoundException(myVertexTypePredefinition, edge);
                    }
                }
                else
                {
                    var target = myDefsByName[group.Key] as VertexTypePredefinition;

                    foreach (var edge in group)
                    {
                        if (!target.OutgoingEdges.Any(outgoing => GetTargetEdgeNameFromAttributeType(edge.AttributeType)
                                                                    .Equals(outgoing.AttributeName)))
                            throw new OutgoingEdgeNotFoundException(myVertexTypePredefinition, edge);
                    }

                }
            }
        }

        /// <summary>
        /// Checks if outgoing edges have a valid target.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type definition of that the outgoing edges will be checked,</param>
        /// <param name="myDefsByName">The vertex type predefinitions indexed by their name that are alse defined in this CanAdd operation.
        /// <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckOutgoingEdgeTargets(VertexTypePredefinition myVertexTypePredefinition, 
                                                    IDictionary<string, ATypePredefinition> myDefsByName, 
                                                    TransactionToken myTransaction, 
                                                    SecurityToken mySecurity)
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

        protected override Dictionary<String, TypeInfo> GenerateTypeInfos(
                                                            LinkedList<ATypePredefinition> myDefsSortedTopologically,
                                                            IDictionary<string, ATypePredefinition> myDefsByName,
                                                            long myFirstID,
                                                            TransactionToken myTransaction,
                                                            SecurityToken mySecurity)
        {
            var neededVertexTypes = new HashSet<string>();

            foreach (var def in myDefsByName)
            {
                neededVertexTypes.Add(def.Value.TypeName);
                neededVertexTypes.Add(def.Value.SuperTypeName);

                var value = def.Value as VertexTypePredefinition;

                if (value.OutgoingEdges != null)
                    foreach (var edge in value.OutgoingEdges)
                        neededVertexTypes.Add(edge.AttributeType);
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
                    var vertex = _vertexManager.ExecuteManager.GetSingleVertex(
                                    new BinaryExpression(new SingleLiteralExpression(vertexType), 
                                                            BinaryOperator.Equals, 
                                                            _VertexTypeNameExpression), 
                                    myTransaction, mySecurity);

                    IVertexType neededVertexType = new VertexType(vertex, _baseStorageManager);

                    result.Add(vertexType, new TypeInfo
                    {
                        AttributeCountWithParents = neededVertexType.GetAttributeDefinitions(true).LongCount(),
                        VertexInfo = new VertexInformation((long)BaseTypes.VertexType, _baseStorageManager.GetUUID(vertex))
                    });
                }
            }

            //accumulate attribute counts
            for (var current = myDefsSortedTopologically.First; current != null; current = current.Next)
            {
                if (!result.ContainsKey(current.Value.TypeName))
                    continue;

                var info = result[current.Value.TypeName];

                info.AttributeCountWithParents = info.AttributeCountWithParents + result[current.Value.SuperTypeName].AttributeCountWithParents;

                result[current.Value.TypeName] = info;
            }

            return result;
        }

        /// <summary>
        /// Converts the given linked list of ATypePredefinitions to a linked list of vertex type predefintions.
        /// </summary>
        /// <param name="myPredefinitions">The linked list of the type predefinitions.</param>
        /// <returns></returns>
        private LinkedList<VertexTypePredefinition> ConvertLinkedList(IEnumerable<ATypePredefinition> myPredefinitions)
        {
            var list = new LinkedList<VertexTypePredefinition>();

            foreach (var item in myPredefinitions)
                list.AddLast(item as VertexTypePredefinition);

            return list;
        }

        private VertexInformation GetOutgoingEdgeVertexInformation(string myVertexType, 
                                                                    string myEdgeName, 
                                                                    TransactionToken myTransaction, 
                                                                    SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.GetVertices(
                                new BinaryExpression(
                                    new SingleLiteralExpression(myEdgeName), 
                                    BinaryOperator.Equals, 
                                    _attributeNameExpression), 
                                false, 
                                myTransaction, 
                                mySecurity).ToArray();
            
            var vertex = vertices.First(x => IsAttributeOnVertexType(myVertexType, x));
            
            return new VertexInformation(vertex.VertexTypeID, vertex.VertexID);
        }

        private static bool IsAttributeOnVertexType(String myVertexTypeName, 
                                                    IVertex myAttributeVertex)
        {
            var vertexTypeVertex = myAttributeVertex
                                    .GetOutgoingSingleEdge((long)AttributeDefinitions.AttributeDotDefiningType)
                                    .GetTargetVertex();

            var name = vertexTypeVertex.GetPropertyAsString((long)AttributeDefinitions.BaseTypeDotName);

            return name.Equals(myVertexTypeName);
        }

        private void ConnectVertexToUniqueIndex(TypeInfo myTypeInfo, 
                                                IEnumerable<IIndexDefinition> myIndexDefinitions, 
                                                SecurityToken mySecurity, 
                                                TransactionToken myTransaction)
        {
            _vertexManager.ExecuteManager.VertexStore
                .UpdateVertex(
                    mySecurity,
                    myTransaction,
                    myTypeInfo.VertexInfo.VertexID,
                    myTypeInfo.VertexInfo.VertexTypeID,
                    new VertexUpdateDefinition(
                        myHyperEdgeUpdate: new HyperEdgeUpdate(new Dictionary<long, HyperEdgeUpdateDefinition>
                        {
                            {
                                (long)AttributeDefinitions.VertexTypeDotUniquenessDefinitions, 
                                new HyperEdgeUpdateDefinition(
                                    (long)BaseTypes.Edge, 
                                    myToBeUpdatedSingleEdges: 
                                        myIndexDefinitions.Select(x => IndexDefinitionToSingleEdgeUpdate(myTypeInfo.VertexInfo, x)))
                            }
                        }
                    )));
        }

        private static SingleEdgeUpdateDefinition IndexDefinitionToSingleEdgeUpdate(VertexInformation mySourceVertex, 
                                                                                    IIndexDefinition myDefinition)
        {
            return new SingleEdgeUpdateDefinition(mySourceVertex, 
                                                    new VertexInformation((long)BaseTypes.Index, 
                                                                            myDefinition.ID), 
                                                    (long)BaseTypes.Edge);
        }

        #endregion
    }
}
