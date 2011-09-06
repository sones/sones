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
using sones.Library.ErrorHandling;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class ExecuteVertexTypeManager : AExecuteTypeManager<IVertexType>
    {
        #region Data

        private IManagerOf<ITypeHandler<IEdgeType>> _edgeManager;

        #endregion

        #region constructor

        public ExecuteVertexTypeManager(IDManager myIDManager)
            : base(myIDManager)
        {
            //_idManager = myIDManager;

            _baseTypes = new Dictionary<long, IBaseType>();
            _nameIndex = new Dictionary<String, long>();
        }

        #endregion

        #region ACheckTypeManager member

        public override IEnumerable<IVertexType> GetAllTypes(Int64 myTransaction,
                                                                SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.GetVertices(BaseTypes.VertexType.ToString(),
                                                                        myTransaction,
                                                                        mySecurity,
                                                                        false);

            return vertices == null ? Enumerable.Empty<IVertexType>()
                                    : vertices.Select(x => new VertexType(x, _baseStorageManager));
        }

        public override void TruncateType(long myTypeID,
                                            Int64 myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            var vertexType = GetType(myTypeID, myTransactionToken, mySecurityToken);

            if (IsTypeBaseType(myTypeID))
                throw new InvalidTypeException("[BaseType] " + myTypeID.ToString(), "userdefined type");

            #region remove all vertices of this type

            _vertexManager.ExecuteManager.VertexStore.RemoveVertices(mySecurityToken, myTransactionToken, myTypeID);

            #endregion

            #region rebuild indices

            _indexManager.RebuildIndices(myTypeID, myTransactionToken, mySecurityToken);

            #endregion
        }

        public override void TruncateType(string myTypeName,
                                            Int64 myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            var vertexType = GetType(myTypeName, myTransactionToken, mySecurityToken);

            if (IsTypeBaseType(myTypeName))
                throw new InvalidTypeException("[BaseType] " + myTypeName, "userdefined type");

            #region remove all vertices of this type

            _vertexManager.ExecuteManager.VertexStore.RemoveVertices(mySecurityToken, myTransactionToken, vertexType.ID);

            #endregion

            #region rebuild indices

            _indexManager.RebuildIndices(vertexType.ID, myTransactionToken, mySecurityToken);

            #endregion
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
                                            BaseTypes.Property,
                                            BaseTypes.VertexType,
                                            BaseTypes.Vertex })
            {
                _baseTypes.Add((long)type, help[(long)type]);
                _nameIndex.Add(type.ToString(), (long)type);
            }
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _edgeManager = myMetaManager.EdgeTypeManager;
            _indexManager = myMetaManager.IndexManager;
            _vertexManager = myMetaManager.VertexManager;
            _baseTypeManager = myMetaManager.BaseTypeManager;
            _baseStorageManager = myMetaManager.BaseGraphStorageManager;
        }

        public override void Load(Int64 myTransaction,
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
                BaseTypes.VertexType,
                BaseTypes.Vertex);
        }

        protected override IVertexType CreateType(IVertex myVertex)
        {
            var result = new VertexType(myVertex, _baseStorageManager);

            _baseTypes.Add(result.ID, result);
            _nameIndex.Add(result.Name, result.ID);

            return result;
        }

        /// <summary>
        /// Checks if the given parameter type is valid.
        /// </summary>
        /// <param name="myRequest">The parameter to be checked.</param>
        protected override void CheckRequestType(IRequestAlterType myRequest)
        {
            if (!(myRequest is RequestAlterVertexType))
                throw new InvalidParameterTypeException("AlterTypeRequest", myRequest.GetType().Name, typeof(RequestAlterVertexType).Name);
        }

        /// <summary>
        /// Adds the specified properties to the given type and stores them.
        /// </summary>
        /// <param name="myToBeAddedProperties">The to be added properties.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <returns>A dictionary with to be added attributes and default value</returns>returns>
        protected override Dictionary<long, IComparable> ProcessAddPropery(
            IEnumerable<PropertyPredefinition> myToBeAddedProperties, 
            Int64 myTransactionToken, 
            SecurityToken mySecurityToken, 
            IVertexType myType)
        {
            Dictionary<long, IComparable> dict = null;

            foreach (var aProperty in myToBeAddedProperties)
            {
                dict = dict ?? new Dictionary<long, IComparable>();

                var id = _idManager
                            .GetVertexTypeUniqeID((long)BaseTypes.Attribute)
                            .GetNextID();

                dict.Add(id, aProperty.DefaultValue);

                _baseStorageManager.StoreProperty(
                    _vertexManager.ExecuteManager.VertexStore,
                    new VertexInformation(
                        (long)BaseTypes.Property,
                        id),
                    aProperty.AttributeName,
                    aProperty.Comment,
                    DateTime.UtcNow.ToBinary(),
                    aProperty.IsMandatory,
                    aProperty.Multiplicity,
                    aProperty.DefaultValue,
                    true,
                    new VertexInformation(
                        (long)BaseTypes.VertexType,
                        myType.ID),
                    ConvertBasicType(aProperty.AttributeType),
                    mySecurityToken,
                    myTransactionToken);
            }

            return dict;
        }

        /// <summary>
        /// Defines specified attributes in the given type and stores them.
        /// </summary>
        /// <param name="myToBeDefinedAttributes">The attributes to be defined</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <param name="myType">The type to be altered.</param>
        /// <returns>A dictionary with to be defined attributes and default value</returns>returns>
        protected override Dictionary<long, IComparable> ProcessDefineAttributes(
            IEnumerable<UnknownAttributePredefinition> myToBeDefinedAttributes,
            Int64 myTransactionToken,
            SecurityToken mySecurityToken,
            IVertexType myType)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #region private helper

        #region abstract private helper

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override bool IsTypeBaseType(long myTypeID)
        {
            return ((long)BaseTypes.Attribute).Equals(myTypeID) ||
                        ((long)BaseTypes.BaseType).Equals(myTypeID) ||
                        ((long)BaseTypes.BinaryProperty).Equals(myTypeID) ||
                        ((long)BaseTypes.EdgeType).Equals(myTypeID) ||
                        ((long)BaseTypes.IncomingEdge).Equals(myTypeID) ||
                        ((long)BaseTypes.Index).Equals(myTypeID) ||
                        ((long)BaseTypes.OutgoingEdge).Equals(myTypeID) ||
                        ((long)BaseTypes.Property).Equals(myTypeID) ||
                        ((long)BaseTypes.Vertex).Equals(myTypeID) ||
                        ((long)BaseTypes.VertexType).Equals(myTypeID);
        }

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override bool IsTypeBaseType(String myTypeName)
        {
            BaseTypes type;
            if (!Enum.TryParse(myTypeName, out type))
                return false;

            return true;
        }

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeID"/>.
        /// </summary>
        /// <param name="myTypeId"></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given ID or <c>NULL</c>, if not present.</returns>
        protected override IVertex Get(long myTypeId,
                                        Int64 myTransaction,
                                        SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.ExecuteManager
                    .GetSingleVertex(new BinaryExpression(_VertexTypeVertexIDExpression,
                                                            BinaryOperator.Equals,
                                                            new SingleLiteralExpression(myTypeId)),
                                        myTransaction, mySecurity);

            #endregion
        }

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeName"/>.
        /// </summary>
        /// <param name="myTypeName">The vertex type name.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given name or <c>NULL</c>, if not present.</returns>
        protected override IVertex Get(string myTypeName,
                                        Int64 myTransaction,
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
                                                    Int64 myTransaction, SecurityToken mySecurity)
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
                                                                            Int64 myTransaction,
                                                                            SecurityToken mySecurity)
        {
            var parentPredef = GetParentPredefinitionOnTopologicallySortedList(myTopologicallySortedPointer);

            if (parentPredef == null)
            {
                //Get the parent type from FS.
                var parent = Get(myTopologicallySortedPointer.Value.SuperTypeName, myTransaction, mySecurity);

                if (parent == null)
                    //No parent type was found.
                    throw new TypeDoesNotExistException<IVertexType>(myTopologicallySortedPointer.Value.SuperTypeName);

                if (parent.GetProperty<bool>((long)AttributeDefinitions.BaseTypeDotIsSealed))
                    //The parent type is sealed.
                    throw new SealedBaseVertexTypeException(myTopologicallySortedPointer.Value.TypeName,
                                                            parent.GetPropertyAsString((long)AttributeDefinitions.AttributeDotName));

                var parentType = new VertexType(parent, _baseStorageManager);
                var attributeNames = parentType.GetAttributeDefinitions(true).Select(_ => _.Name);

                myAttributes[myTopologicallySortedPointer.Value.TypeName] 
                    = new HashSet<string>(attributeNames);
            }
            else
            {
                myAttributes[myTopologicallySortedPointer.Value.TypeName] 
                    = new HashSet<string>(myAttributes[parentPredef.Value.TypeName]);
            }

            var attributeNamesSet = myAttributes[myTopologicallySortedPointer.Value.TypeName];

            CheckIncomingEdgesUniqueName(myTopologicallySortedPointer.Value as VertexTypePredefinition,
                                            attributeNamesSet);

            CheckOutgoingEdgesUniqueName(myTopologicallySortedPointer.Value as VertexTypePredefinition,
                                            attributeNamesSet);

            CheckPropertiesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
        }

        /// <summary>
        /// Reservs myCountOfNeededIDs type ids in id manager depending on type and gets the first reserved id.
        /// </summary>
        /// <param name="myCountOfNeededIDs">Count of to be reserved ids.</param>
        /// <returns>The first reserved id.</returns>
        protected override long GetFirstTypeID(int myCountOfNeededIDs)
        {
            return _idManager.VertexTypeID.ReserveIDs(myCountOfNeededIDs);
        }

        protected override Dictionary<String, TypeInfo> GenerateTypeInfos(
                                                            LinkedList<ATypePredefinition> myDefsSortedTopologically,
                                                            IDictionary<string, ATypePredefinition> myDefsByName,
                                                            long myFirstID,
                                                            Int64 myTransaction,
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
        /// Calls the needed store methods depending on the typemanger.
        /// </summary>
        /// <param name="myDefsTopologically">The topologically sorted type predefinitions.</param>
        /// <param name="myTypeInfos">The created type infos.</param>
        /// <param name="myCreationDate">The creation date.</param>
        /// <param name="myResultPos">The result position.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <param name="myResult">Ref on result array.</param>
        protected override IEnumerable<IVertexType> StoreTypeAndAttributes(LinkedList<ATypePredefinition> myDefsTopologically,
                                                                            Dictionary<String, TypeInfo> myTypeInfos,
                                                                            long myCreationDate,
                                                                            int myResultPos,
                                                                            Int64 myTransactionToken,
                                                                            SecurityToken mySecurityToken,
                                                                            ref IVertex[] myResult)
        {
            var vertexDefsTopologically = ConvertLinkedList(myDefsTopologically);

            #region store vertex type

            StoreVertexType(vertexDefsTopologically,
                            myTypeInfos,
                            myCreationDate,
                            myResultPos,
                            myTransactionToken,
                            mySecurityToken,
                            ref myResult);

            #endregion

            #region Store Attributes

            //The order of adds is important. First property, then outgoing edges (that might point to properties) and finally incoming edges (that might point to outgoing edges)
            //Do not try to merge it into one for block.

            #region Store properties

            StoreProperties(myDefsTopologically,
                            myTypeInfos,
                            myCreationDate,
                            myTransactionToken,
                            mySecurityToken);

            #endregion

            #region Store binary properties

            StoreBinaryProperties(vertexDefsTopologically,
                                    myTypeInfos,
                                    myCreationDate,
                                    myTransactionToken,
                                    mySecurityToken);

            #endregion

            #region Store outgoing edges

            StoreOutgoingEdges(vertexDefsTopologically,
                                myTypeInfos,
                                myCreationDate,
                                myTransactionToken,
                                mySecurityToken);

            #endregion

            #region Store incoming edges

            StoreIncomingEdges(vertexDefsTopologically,
                                myTypeInfos,
                                myCreationDate,
                                myTransactionToken,
                                mySecurityToken);

            #endregion

            var resultTypes = new VertexType[myResult.Length];

            #region reload the stored types

            //reload the IVertex objects, that represents the type.
            for (int i = 0; i < myResult.Length; i++)
            {
                myResult[i] = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurityToken, myTransactionToken,
                                                                                myResult[i].VertexID,
                                                                                myResult[i].VertexTypeID, String.Empty);

                var newVertexType = new VertexType(myResult[i], _baseStorageManager);

                resultTypes[i] = newVertexType;

                _baseTypes.Add(myTypeInfos[newVertexType.Name].VertexInfo.VertexID, newVertexType);
            }

            #endregion

            #endregion

            #region Add Indices

            if (_indexManager != null)
            {
                var uniqueIdx = _indexManager.GetBestMatchingIndexName(true, false, false);

                var indexIdx = _indexManager.GetBestMatchingIndexName(false, false, false);

                myResultPos = 0;
                for (var current = vertexDefsTopologically.First; current != null; current = current.Next, myResultPos++)
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
                                                                                                mySecurityToken,
                                                                                                myTransactionToken,
                                                                                                false)).ToArray();

                        //only own unique indices are connected to the vertex type on the UniquenessDefinitions attribute
                        ConnectVertexToUniqueIndex(myTypeInfos[current.Value.TypeName], indexDefs, mySecurityToken, myTransactionToken);
                    }

                    #endregion

                    #region parent uniques

                    foreach (var unique in resultTypes[myResultPos].ParentVertexType.GetUniqueDefinitions(true))
                    {
                        _indexManager.CreateIndex(
                            new IndexPredefinition(unique.DefiningVertexType.Name)
                                                    .AddProperty(unique.UniquePropertyDefinitions.Select(x => x.Name))
                                                    .SetIndexType(uniqueIdx),
                            mySecurityToken,
                            myTransactionToken,
                            false);
                    }

                    #endregion

                    #endregion

                    #region Indices

                    if (current.Value.Indices != null)
                        foreach (var index in current.Value.Indices)
                        {
                            _indexManager.CreateIndex(index, mySecurityToken, myTransactionToken);
                        }

                    foreach (var index in resultTypes[myResultPos].ParentVertexType.GetIndexDefinitions(true))
                    {
                        _indexManager.CreateIndex(
                            new IndexPredefinition(current.Value.TypeName)
                                                    .AddProperty(index.IndexedProperties.Select(x => x.Name))
                                                    .SetIndexType(index.IndexTypeName),
                            mySecurityToken,
                            myTransactionToken);
                    }

                    #endregion
                }
            }
            #endregion

            return resultTypes.AsEnumerable<IVertexType>();
        }

        /// <summary>
        /// Removes the given types from the graphDB.
        /// </summary>
        /// <param name="myVertexTypes">The types to delete.</param>
        /// <param name="myTransaction">Transaction token.</param>
        /// <param name="mySecurity">Security Token.</param>
        /// <param name="myIgnoreReprimands">True means, that reprimands (IncomingEdges) on the types wich should be removed are ignored.</param>
        /// <returns>Set of deleted type IDs.</returns>
        protected override Dictionary<Int64, String> Remove(IEnumerable<IVertexType> myTypes,
                                                            Int64 myTransaction,
                                                            SecurityToken mySecurity,
                                                            bool myIgnoreReprimands = false)
        {
            //the attribute types on delete types which have to be removed
            var toDeleteAttributeDefinitions = new List<IAttributeDefinition>();

            //the indices on the delete types
            var toDeleteIndexDefinitions = new List<IIndexDefinition>();

            #region get propertydefinitions

            //get child vertex types
            foreach (var delType in myTypes)
            {
                try
                {
                    //check if type exists
                    var temp = GetType(delType.ID, myTransaction, mySecurity);
                }
                catch (Exception exception)
                {
                    throw new TypeRemoveException<IVertexType>(delType.Name, exception);
                }

                if (!delType.HasParentType)
                    //type must be base type because there is no parent type, Exception that base type cannot be deleted
                    throw new TypeRemoveException<IVertexType>(delType.Name, "A BaseType connot be removed.");

                if (IsTypeBaseType(delType.ID))
                    //Exception that base type cannot be deleted
                    throw new TypeRemoveException<IVertexType>(delType.Name, "A BaseType connot be removed.");

                //just check edges if reprimands should not be ignored
                if (!myIgnoreReprimands)
                {
                    #region check that existing child types are specified

                    if (!delType.GetDescendantVertexTypes().All(child => myTypes.Contains(child)))
                        throw new TypeRemoveException<IVertexType>(delType.Name, "The given type has child types and cannot be removed.");

                    #endregion

                    //TODO: use vertex and check if incoming edges existing of user defined types, instead of foreach
                    var VertexOfDelType = _vertexManager.ExecuteManager
                                            .GetSingleVertex(new BinaryExpression(_VertexTypeVertexIDExpression,
                                                                                    BinaryOperator.Equals,
                                                                                    new SingleLiteralExpression(delType.ID)),
                                                                myTransaction, mySecurity);

                    #region check that no other type has a outgoing edge pointing on delete type

                    //get all vertex types and check if they have outgoing edges with target vertex the delete type
                    foreach (var type in GetAllTypes(myTransaction, mySecurity))
                    {
                        if (type.ID != delType.ID && type.GetOutgoingEdgeDefinitions(false)
                                                            .Any(outEdgeDef => outEdgeDef.TargetVertexType.ID.Equals(delType.ID)))
                        {
                            throw new TypeRemoveException<IVertexType>(delType.Name,
                                        "There are other types which have outgoing edges pointing to the type, which should be removed.");
                        }
                    }

                    #endregion

                    #region check if there are incoming edges of target vertices of outgoing edges of the deleting type

                    foreach (var outEdge in delType.GetOutgoingEdgeDefinitions(false))
                    {
                        if (outEdge.TargetVertexType.GetIncomingEdgeDefinitions(true)
                                                    .Any(inEdge => inEdge.RelatedEdgeDefinition.ID.Equals(outEdge.ID) &&
                                inEdge.RelatedType.ID != delType.ID) &&
                                !myIgnoreReprimands)
                            throw new TypeRemoveException<IVertexType>(delType.Name,
                                        "There are other types which have incoming edges, whose related type is a outgoing edge of the type which should be removed.");
                    }

                    #endregion
                }

                toDeleteAttributeDefinitions.AddRange(delType.GetAttributeDefinitions(false));

                toDeleteIndexDefinitions.AddRange(delType.GetIndexDefinitions(false));
            }

            #endregion

            //the IDs of the deleted vertices
            var deletedTypeIDs = new Dictionary<Int64, String>(myTypes.ToDictionary(key => key.ID, item => item.Name));

            #region remove indices

            //remove indices on types
            foreach (var index in toDeleteIndexDefinitions)
                _indexManager.RemoveIndexInstance(index.ID, myTransaction, mySecurity);

            #endregion

            #region remove attribute types on delete types

            //delete attribute vertices
            foreach (var attr in toDeleteAttributeDefinitions)
            {
                switch (attr.Kind)
                {
                    case (AttributeType.Property):
                        if (!_vertexManager.ExecuteManager.VertexStore
                                .RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.Property))
                            throw new VertexTypeRemoveException(myTypes.Where(x => x.HasProperty(attr.Name)).First().Name,
                                        "The Property " + attr.Name + " could not be removed.");
                        break;

                    case (AttributeType.IncomingEdge):
                        if (!_vertexManager.ExecuteManager.VertexStore
                                .RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.IncomingEdge))
                            throw new VertexTypeRemoveException(myTypes.Where(x => x.HasIncomingEdge(attr.Name)).First().Name,
                                        "The IncomingEdge " + attr.Name + " could not be removed.");
                        break;

                    case (AttributeType.OutgoingEdge):
                        if (!_vertexManager.ExecuteManager.VertexStore
                                .RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.OutgoingEdge))
                            throw new VertexTypeRemoveException(myTypes.Where(x => x.HasOutgoingEdge(attr.Name)).First().Name,
                                        "The OutgoingEdge " + attr.Name + " could not be removed.");
                        break;

                    case (AttributeType.BinaryProperty):
                        if (!_vertexManager.ExecuteManager.VertexStore
                                .RemoveVertex(mySecurity, myTransaction, attr.ID, (long)BaseTypes.BinaryProperty))
                            throw new VertexTypeRemoveException(myTypes.Where(x => x.HasBinaryProperty(attr.Name)).First().Name,
                                        "The BinaryProperty " + attr.Name + " could not be removed.");
                        break;

                    default:
                        break;
                }
            }

            #endregion

            #region remove vertex type names from index

            foreach (var type in myTypes)
            {
                var result = _indexManager.GetIndex(BaseUniqueIndex.VertexTypeDotName);

                if (result != null)
                    if (!result.Remove(type.Name))
                        throw new TypeRemoveException<IVertexType>(type.Name, "Error during delete the Index on type.");
            }

            #endregion

            #region remove vertices

            //delete the vertices
            foreach (var type in myTypes)
            {

                //removes the instances of the VertexType
                _vertexManager.ExecuteManager.VertexStore.RemoveVertices(mySecurity, myTransaction, type.ID);

                //removes the vertexType
                if (!_vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurity, myTransaction, type.ID, (long)BaseTypes.VertexType))
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
        /// All to be removed things of the alter type request are going to be removed inside this method,
        /// the related operations will be executed inside here.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <param name="myUpdateRequest">A reference to an update request to update relevant vertices.</param>
        protected override void AlterType_Remove(IRequestAlterType myAlterTypeRequest,
                                                    IVertexType myType,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken,
                                                    ref RequestUpdate myUpdateRequest)
        {
            var request = myAlterTypeRequest as RequestAlterVertexType;

            CheckRemoveOutgoingEdges(request, 
                                        myType, 
                                        myTransactionToken, 
                                        mySecurityToken);

            RemoveMandatoryConstraint(request.ToBeRemovedMandatories, 
                                        myType, 
                                        myTransactionToken,
                                        mySecurityToken);

            RemoveUniqueConstraint(request.ToBeRemovedUniques, 
                                    myType, 
                                    myTransactionToken,
                                    mySecurityToken);

            RemoveIndices(request.ToBeRemovedIndices, 
                            myType, 
                            myTransactionToken, 
                            mySecurityToken);

            RemoveAttributes(request.ToBeRemovedIncomingEdges,
                             request.ToBeRemovedOutgoingEdges,
                             request.ToBeRemovedProperties, 
                             myType, 
                             myTransactionToken,
                             mySecurityToken,
                             ref myUpdateRequest);
        }

        /// <summary>
        /// All to be added things of the alter type request are going to be added inside this method,
        /// the related operations will be executed inside here.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="myUpdateRequest">A reference to an update request to update relevant vertices.</param>
        protected override void AlterType_Add(IRequestAlterType myAlterTypeRequest,
                                                IVertexType myType,
                                                Int64 myTransactionToken,
                                                SecurityToken mySecurityToken,
                                                ref RequestUpdate myUpdateRequest)
        {
            var request = myAlterTypeRequest as RequestAlterVertexType;

            AddAttributes(request.ToBeAddedBinaryProperties,
                          request.ToBeAddedIncomingEdges,
                          request.ToBeAddedOutgoingEdges,
                          request.ToBeAddedProperties,
                          request.ToBeDefinedAttributes,
                          myType, myTransactionToken, mySecurityToken);

            myType = GetType(request.TypeName, myTransactionToken, mySecurityToken);

            AddMandatoryConstraint(request.ToBeAddedMandatories, myType, myTransactionToken,
                                   mySecurityToken);

            var indexDefinitions = AddUniqueConstraint(request, myTransactionToken, mySecurityToken);

            AddIndices(request.ToBeAddedIndices, myType, myTransactionToken, mySecurityToken);

            var info = new TypeInfo();
            info.VertexInfo = new VertexInformation((long)BaseTypes.VertexType,
                                                    myType.ID);

            ConnectVertexToUniqueIndex(info, 
                                        indexDefinitions, 
                                        mySecurityToken, 
                                        myTransactionToken);
        }

        /// <summary>
        /// Renames attributes.
        /// </summary>
        /// <param name="myToBeRenamedAttributes">The to be renamed attributes.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected override void RenameAttributes(Dictionary<string, string> myToBeRenamedAttributes,
                                                    IVertexType myType,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken)
        {
            if (!myToBeRenamedAttributes.IsNotNullOrEmpty())
                return;

            foreach (var aToBeRenamedAttribute in myToBeRenamedAttributes)
            {
                VertexUpdateDefinition update = new VertexUpdateDefinition(null,
                                                                            new StructuredPropertiesUpdate(
                                                                                new Dictionary<long, IComparable> 
                                                                                    { { (long)AttributeDefinitions.AttributeDotName, 
                                                                                          aToBeRenamedAttribute.Value } }));

                var attribute = myType.GetAttributeDefinition(aToBeRenamedAttribute.Key);

                switch (attribute.Kind)
                {
                    case AttributeType.Property:
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken,
                                                                                myTransactionToken,
                                                                                attribute.ID,
                                                                                (long)BaseTypes.Property,
                                                                                update);
                        break;

                    case AttributeType.IncomingEdge:
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken,
                                                                                myTransactionToken,
                                                                                attribute.ID,
                                                                                (long)BaseTypes.IncomingEdge,
                                                                                update);
                        break;

                    case AttributeType.OutgoingEdge:
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken,
                                                                                myTransactionToken,
                                                                                attribute.ID,
                                                                                (long)BaseTypes.OutgoingEdge,
                                                                                update);
                        break;

                    case AttributeType.BinaryProperty:
                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken,
                                                                                myTransactionToken,
                                                                                attribute.ID,
                                                                                (long)BaseTypes.BinaryProperty,
                                                                                update);
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Change the comment on the type.
        /// </summary>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myNewComment">The new comment.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected override void ChangeCommentOnType(IVertexType myType,
                                                    string myNewComment,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken)
        {
            if (!String.IsNullOrWhiteSpace(myNewComment))
            {
                var update = new VertexUpdateDefinition(myNewComment);

                _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken,
                                                                        myTransactionToken,
                                                                        myType.ID,
                                                                        (long)BaseTypes.VertexType,
                                                                        update);
            }
        }

        /// <summary>
        /// Renames a type.
        /// </summary>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myNewTypeName">The new type name.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected override void RenameType(IVertexType myType,
                                            string myNewTypeName,
                                            Int64 myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            if (!String.IsNullOrWhiteSpace(myNewTypeName))
            {
                var update = new VertexUpdateDefinition(null,
                                                        new StructuredPropertiesUpdate(
                                                            new Dictionary<long, IComparable> 
                                                                { { (long)AttributeDefinitions.BaseTypeDotName, 
                                                                      myNewTypeName } }));

                _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken,
                                                                        myTransactionToken,
                                                                        myType.ID,
                                                                        (long)BaseTypes.VertexType,
                                                                        update);
            }
        }

        /// <summary>
        /// Calls the RebuildIndices method of the index manager.
        /// </summary>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected override void CallRebuildIndices(Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken)
        {
            _indexManager.RebuildIndices((long)BaseTypes.VertexType, 
                                            myTransactionToken, 
                                            mySecurityToken);
        }
        
        #endregion

        /// <summary>
        /// Gets the max id of the given type id.
        /// </summary>
        /// <param name="myTypeID">The type id.</param>
        /// <param name="myTransaction">Int64</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <returns>The max id.</returns>
        private long GetMaxID(long myTypeID,
                                Int64 myTransaction,
                                SecurityToken mySecurity)
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
        /// <param name="myTransaction">Int64</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <param name="myBaseTypes">The base types.</param>
        private void LoadBaseType(Int64 myTransaction,
                                    SecurityToken mySecurity,
                                    params BaseTypes[] myBaseTypes)
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
        /// Checks if an incoming myEdge has a coresponding outgoing myEdge.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type definition of that the incoming edges will be checked,</param>
        /// <param name="myDefsByName">The vertex type predefinitions indexed by their name that are alse defined in this CanAdd operation.
        /// <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckIncomingEdgeSources(VertexTypePredefinition myVertexTypePredefinition,
                                                    IDictionary<string, ATypePredefinition> myDefsByName,
                                                    Int64 myTransaction,
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
                                                    Int64 myTransaction,
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
                                                                    Int64 myTransaction,
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
                                                Int64 myTransaction)
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

        private void StoreVertexType(LinkedList<VertexTypePredefinition> myDefsTopologically,
                                        Dictionary<String, TypeInfo> myTypeInfos,
                                        long myCreationDate,
                                        int myResultPos,
                                        Int64 myTransactionToken,
                                        SecurityToken mySecurityToken,
                                        ref IVertex[] myResult)
        {
            //now we store each vertex type
            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                var newVertexType = _baseStorageManager.StoreVertexType(
                     _vertexManager.ExecuteManager.VertexStore,
                     myTypeInfos[current.Value.TypeName].VertexInfo,
                     current.Value.TypeName,
                     current.Value.Comment,
                     myCreationDate,
                     current.Value.IsAbstract,
                     current.Value.IsSealed,
                     true,
                     myTypeInfos[current.Value.SuperTypeName].VertexInfo,
                     null,
                     mySecurityToken,
                     myTransactionToken);

                myResult[myResultPos++] = newVertexType;

                _indexManager.GetIndex(BaseUniqueIndex.VertexTypeDotName)
                                .Add(current.Value.TypeName, myTypeInfos[current.Value.TypeName].VertexInfo.VertexID);

                _nameIndex.Add(current.Value.TypeName, myTypeInfos[current.Value.TypeName].VertexInfo.VertexID);
            }
        }

        private void StoreBinaryProperties(LinkedList<VertexTypePredefinition> myDefsTopologically,
                                            Dictionary<String, TypeInfo> myTypeInfos,
                                            long myCreationDate,
                                            Int64 myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.BinaryProperties == null)
                    continue;

                var firstAttrID = _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute).ReserveIDs(current.Value.BinaryPropertyCount);

                var currentExternID = myTypeInfos[current.Value.TypeName].AttributeCountWithParents
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
                        myCreationDate,
                        myTypeInfos[current.Value.TypeName].VertexInfo,
                        mySecurityToken,
                        myTransactionToken);
                }

            }
        }

        private void StoreOutgoingEdges(LinkedList<VertexTypePredefinition> myDefsTopologically,
                                        Dictionary<String, TypeInfo> myTypeInfos,
                                        long myCreationDate,
                                        Int64 myTransactionToken,
                                        SecurityToken mySecurityToken)
        {
            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.OutgoingEdges == null)
                    continue;

                var firstAttrID = _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute).ReserveIDs(current.Value.OutgoingEdgeCount);

                var currentExternID = myTypeInfos[current.Value.TypeName].AttributeCountWithParents
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
                                                                _edgeManager.ExecuteManager.GetType(edge.InnerEdgeType, myTransactionToken, mySecurityToken).ID);
                    }

                    _baseStorageManager.StoreOutgoingEdge(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.OutgoingEdge, firstAttrID++),
                        edge.AttributeName,
                        edge.Comment,
                        true,
                        myCreationDate,
                        edge.Multiplicity,
                        myTypeInfos[current.Value.TypeName].VertexInfo,
                        new VertexInformation((long)BaseTypes.EdgeType,
                                                _edgeManager.ExecuteManager.GetType(edge.EdgeType, myTransactionToken, mySecurityToken).ID),
                        innerEdgeType,
                        myTypeInfos[edge.AttributeType].VertexInfo,
                        mySecurityToken,
                        myTransactionToken);
                }

            }
        }

        private void StoreIncomingEdges(LinkedList<VertexTypePredefinition> myDefsTopologically,
                                        Dictionary<String, TypeInfo> myTypeInfos,
                                        long myCreationDate,
                                        Int64 myTransactionToken,
                                        SecurityToken mySecurityToken)
        {
            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.IncomingEdges == null)
                    continue;

                var firstAttrID = _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute)
                                                .ReserveIDs(current.Value.IncomingEdgeCount);

                var currentExternID = myTypeInfos[current.Value.TypeName].AttributeCountWithParents
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
                        myCreationDate,
                        myTypeInfos[current.Value.TypeName].VertexInfo,
                        GetOutgoingEdgeVertexInformation(GetTargetVertexTypeFromAttributeType(edge.AttributeType),
                                                            GetTargetEdgeNameFromAttributeType(edge.AttributeType),
                                                            myTransactionToken,
                                                            mySecurityToken),
                        mySecurityToken,
                        myTransactionToken);
                }
            }
        }

        /// <summary>
        /// Checks if the specified to be removed outgoing edges cvan be removed.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void CheckRemoveOutgoingEdges(RequestAlterVertexType myAlterTypeRequest, 
                                                IVertexType myType, 
                                                Int64 myTransactionToken,
                                                SecurityToken mySecurityToken)
        {
            if (myAlterTypeRequest.ToBeRemovedOutgoingEdges == null)
                return;

            #region get the list of incoming edges that will be deleted too

            var toBeRemovedIncomingEdgeIDs = new List<long>();

            if (myAlterTypeRequest.ToBeRemovedIncomingEdges != null)
                toBeRemovedIncomingEdgeIDs.AddRange(
                    myAlterTypeRequest.ToBeRemovedIncomingEdges.Select(
                        _ => myType.GetIncomingEdgeDefinition(_).ID));

            #endregion

            foreach (var aOutgoingEdge in myAlterTypeRequest.ToBeRemovedOutgoingEdges)
            {
                var attrDef = myType.GetOutgoingEdgeDefinition(aOutgoingEdge);

                var vertex = _vertexManager.ExecuteManager.GetVertex((long)BaseTypes.OutgoingEdge, attrDef.ID, null,
                                                                     null, myTransactionToken, mySecurityToken);

                var incomingEdges = vertex.GetIncomingVertices((long)BaseTypes.IncomingEdge,
                                                               (long)
                                                               AttributeDefinitions.IncomingEdgeDotRelatedEgde);

                foreach (var incomingEdge in incomingEdges)
                {
                    if (!toBeRemovedIncomingEdgeIDs.Contains(incomingEdge.VertexID))
                        //TODO a better exception here
                        throw new AttributeRemoveException(aOutgoingEdge, AttributeType.OutgoingEdge,
                            "The outgoing edge can not be removed, because there are related incoming edges.");
                }
            }
        }

        /// <summary>
        /// Removes mandatory constraits.
        /// </summary>
        /// <param name="myToBeRemovedMandatories">The mandatory attribute names which mandatory constraints should be removed.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void RemoveMandatoryConstraint(IEnumerable<string> myToBeRemovedMandatories, 
                                                IVertexType myType, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken)
        {
            if (myToBeRemovedMandatories.IsNotNullOrEmpty())
            {
                var update = new VertexUpdateDefinition(null, 
                                                        new StructuredPropertiesUpdate(
                                                            new Dictionary<long, IComparable> { 
                                                                { (long)AttributeDefinitions.PropertyDotIsMandatory, false } }));

                foreach (var aMandatory in myToBeRemovedMandatories)
                {
                    IPropertyDefinition property = myType.GetPropertyDefinition(aMandatory);

                    _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, 
                                                                            myTransactionToken, 
                                                                            property.ID, 
                                                                            (long)BaseTypes.Property, null);
                }
            }
        }

        /// <summary>
        /// Removes unique constaints.
        /// </summary>
        /// <param name="myUniqueConstraints">The names of the attributes which unique constraints should be removed.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void RemoveUniqueConstraint(IEnumerable<string> myUniqueConstraints, 
                                            IVertexType myType, 
                                            Int64 myTransactionToken, 
                                            SecurityToken mySecurityToken)
        {
            if (myUniqueConstraints.IsNotNullOrEmpty())
            {
                foreach (var aUniqueConstraint in myUniqueConstraints)
                {
                    foreach (var item in myType.GetDescendantVertexTypesAndSelf())
                    {
                        foreach (var aUniqueDefinition in item.GetUniqueDefinitions(false))
                        {
                            if (aUniqueDefinition.CorrespondingIndex
                                    .IndexedProperties.All(_ => _.Name == aUniqueConstraint))
                                _indexManager.RemoveIndexInstance(aUniqueDefinition.CorrespondingIndex.ID, 
                                                                    myTransactionToken, 
                                                                    mySecurityToken);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Removes indices.
        /// </summary>
        /// <param name="myToBeRemovedIndices">The attributes and index names which are to be deleted.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void RemoveIndices(Dictionary<string, string> myToBeRemovedIndices, 
                                    IVertexType myType, 
                                    Int64 myTransactionToken, 
                                    SecurityToken mySecurityToken)
        {
            if (myToBeRemovedIndices.IsNotNullOrEmpty())
            {
                foreach (var aIndex in myToBeRemovedIndices)
                {
                    //find the source
                    IIndexDefinition sourceIndexDefinition = myType.GetIndexDefinitions(false)
                                                                .Where(_ => _.Name == aIndex.Key && 
                                                                        (aIndex.Value == null || _.Edition == aIndex.Value)).FirstOrDefault();

                    foreach (var aVertexType in myType.GetDescendantVertexTypes())
                    {
                        foreach (var aInnerIndex in aVertexType.GetIndexDefinitions(false)
                                                    .Where(_ => _.SourceIndex.ID == sourceIndexDefinition.ID))
                        {
                            _indexManager.RemoveIndexInstance(aInnerIndex.ID, 
                                                                myTransactionToken, 
                                                                mySecurityToken);
                        }
                    }

                    if (sourceIndexDefinition != null)
                        _indexManager.RemoveIndexInstance(sourceIndexDefinition.ID, 
                                                            myTransactionToken, 
                                                            mySecurityToken);
                }
            }
        }

        /// <summary>
        /// Removes attributes.
        /// </summary>
        /// <param name="myToBeRemovedIncomingEdges">To be removed incoming edges.</param>
        /// <param name="myToBeRemovedOutgoingEdges">To be removed outgoing edges.</param>
        /// <param name="myToBeRemovedProperties">To be removed Proerties.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void RemoveAttributes(IEnumerable<string> myToBeRemovedIncomingEdges, 
                                        IEnumerable<string> myToBeRemovedOutgoingEdges, 
                                        IEnumerable<string> myToBeRemovedProperties, 
                                        IVertexType myType, 
                                        Int64 myTransactionToken, 
                                        SecurityToken mySecurityToken,
                                        ref RequestUpdate myUpdateRequest)
        {
            if (myToBeRemovedIncomingEdges.IsNotNullOrEmpty() || 
                myToBeRemovedOutgoingEdges.IsNotNullOrEmpty() || 
                myToBeRemovedProperties.IsNotNullOrEmpty())
            {
                if (myToBeRemovedIncomingEdges.IsNotNullOrEmpty())
                {
                    ProcessIncomingEdgeRemoval(myToBeRemovedIncomingEdges, 
                                                myType, 
                                                myTransactionToken, 
                                                mySecurityToken);

                    myUpdateRequest
                        .RemoveAlteredAttribute(myType
                                                .GetAttributeDefinitions(true)
                                                .Where(_ => myToBeRemovedIncomingEdges.Contains(_.Name)));
                }

                if (myToBeRemovedOutgoingEdges.IsNotNullOrEmpty())
                {
                    ProcessOutgoingEdgeRemoval(myToBeRemovedOutgoingEdges, 
                                                myType, 
                                                myTransactionToken, 
                                                mySecurityToken);

                    myUpdateRequest
                        .RemoveAlteredAttribute(myType
                                                .GetAttributeDefinitions(true)
                                                .Where(_ => myToBeRemovedOutgoingEdges.Contains(_.Name)));
                }

                if (myToBeRemovedProperties.IsNotNullOrEmpty())
                {
                    ProcessPropertyRemoval(myToBeRemovedProperties, 
                                            myType, 
                                            myTransactionToken, 
                                            mySecurityToken);

                    myUpdateRequest
                        .RemoveAlteredAttribute(myType
                                                .GetAttributeDefinitions(true)
                                                .Where(_ => myToBeRemovedProperties.Contains(_.Name)));
                }
            }
        }

        /// <summary>
        /// Removes incoming edges.
        /// </summary>
        /// <param name="myToBeRemovedIncomingEdges">The to be removed edges.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void ProcessIncomingEdgeRemoval(IEnumerable<string> myToBeRemovedIncomingEdges, 
                                                IVertexType myType, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken)
        {
            foreach (var aIncomingEdge in myToBeRemovedIncomingEdges)
            {
                var incomingEdgeDefinition = myType.GetIncomingEdgeDefinition(aIncomingEdge);

                _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurityToken, 
                                                                        myTransactionToken, 
                                                                        incomingEdgeDefinition.ID, 
                                                                        (long)BaseTypes.IncomingEdge);
            }
        }

        /// <summary>
        /// Removes outgoing edges.
        /// </summary>
        /// <param name="myToBeRemovedOutgoingEdges">The to be removed edges.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void ProcessOutgoingEdgeRemoval(IEnumerable<string> myToBeRemovedOutgoingEdges, 
                                                IVertexType myType, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken)
        {
            foreach (var aOutgoingEdge in myToBeRemovedOutgoingEdges)
            {
                var outgoingEdgeDefinition = myType.GetOutgoingEdgeDefinition(aOutgoingEdge);

                _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurityToken, 
                                                                        myTransactionToken, 
                                                                        outgoingEdgeDefinition.ID, 
                                                                        (long)BaseTypes.OutgoingEdge);
            }
        }

        /// <summary>
        /// Adds attributes.
        /// </summary>
        /// <param name="myToBeAddedBinaryProperties">The to be added binary properties.</param>
        /// <param name="myToBeAddedIncomingEdges">The to be added incoming edges.</param>
        /// <param name="myToBeAddedOutgoingEdges">The to be added outgoing edges.</param>
        /// <param name="myToBeAddedProperties">The to be added properties.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void AddAttributes(IEnumerable<BinaryPropertyPredefinition> myToBeAddedBinaryProperties,
                                    IEnumerable<IncomingEdgePredefinition> myToBeAddedIncomingEdges,
                                    IEnumerable<OutgoingEdgePredefinition> myToBeAddedOutgoingEdges,
                                    IEnumerable<PropertyPredefinition> myToBeAddedProperties,
                                    IEnumerable<UnknownAttributePredefinition> myToBeDefinedAttributes,
                                    IVertexType myType, 
                                    Int64 myTransactionToken, 
                                    SecurityToken mySecurityToken)
        {

            if (myToBeAddedProperties.IsNotNullOrEmpty())
            {
                ProcessAddPropery(myToBeAddedProperties, 
                                    myTransactionToken, 
                                    mySecurityToken, 
                                    myType);

                CleanUpTypes();
            }

            if (myToBeAddedBinaryProperties.IsNotNullOrEmpty())
            {
                ProcessAddBinaryPropery(myToBeAddedBinaryProperties, 
                                        myTransactionToken, 
                                        mySecurityToken, 
                                        myType);

                CleanUpTypes();
            }

            if (myToBeAddedOutgoingEdges.IsNotNullOrEmpty())
            {
                ProcessAddOutgoingEdges(myToBeAddedOutgoingEdges, 
                                        myTransactionToken, 
                                        mySecurityToken, 
                                        myType);
                
                CleanUpTypes();
            }

            if (myToBeAddedIncomingEdges.IsNotNullOrEmpty())
            {
                ProcessAddIncomingEdges(myToBeAddedIncomingEdges, 
                                        myTransactionToken, 
                                        mySecurityToken, 
                                        myType);
                
                CleanUpTypes();
            }

            if (myToBeDefinedAttributes.IsNotNullOrEmpty())
            {
                ProcessDefineAttributes(myToBeDefinedAttributes,
                                        myTransactionToken,
                                        mySecurityToken,
                                        myType);

                CleanUpTypes();
            }

        }

        /// <summary>
        /// Adds binary properties.
        /// </summary>
        /// <param name="myToBeAddedBinaryProperties">The to be added binary properties.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void ProcessAddBinaryPropery(IEnumerable<BinaryPropertyPredefinition> myToBeAddedBinaryProperties, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken, 
                                                IVertexType myType)
        {
            foreach (var aBinaryProperty in myToBeAddedBinaryProperties)
            {
                _baseStorageManager.StoreBinaryProperty(
                    _vertexManager.ExecuteManager.VertexStore, 
                    new VertexInformation((long)BaseTypes.BinaryProperty,
                        _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute).GetNextID()), 
                        aBinaryProperty.AttributeName, 
                        aBinaryProperty.Comment, 
                        true, 
                        DateTime.UtcNow.ToBinary(), 
                        new VertexInformation(
                            (long)BaseTypes.VertexType, 
                            myType.ID), 
                        mySecurityToken, 
                        myTransactionToken);
            }
        }

        /// <summary>
        /// Adds outgoing edges.
        /// </summary>
        /// <param name="myToBeAddedOutgoingEdges">The to be added outgoing edges.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void ProcessAddOutgoingEdges(IEnumerable<OutgoingEdgePredefinition> myToBeAddedOutgoingEdges, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken, 
                                                IVertexType myType)
        {
            foreach (var aToBeAddedOutgoingEdge in myToBeAddedOutgoingEdges)
            {
                var edgeType = _edgeManager.ExecuteManager.GetType(aToBeAddedOutgoingEdge.EdgeType, 
                                                                    myTransactionToken, 
                                                                    mySecurityToken);

                var innerEdgeType = (aToBeAddedOutgoingEdge.InnerEdgeType != null)
                                        ? _edgeManager.ExecuteManager.GetType(aToBeAddedOutgoingEdge.InnerEdgeType, 
                                                                                myTransactionToken, 
                                                                                mySecurityToken)
                                        : null;

                VertexInformation? innerEdgeTypeInfo = null;

                if (innerEdgeType != null)
                    innerEdgeTypeInfo = new VertexInformation((long)BaseTypes.EdgeType, innerEdgeType.ID);

                var targetVertexType = GetType(aToBeAddedOutgoingEdge.AttributeType, myTransactionToken, mySecurityToken);

                _baseStorageManager.StoreOutgoingEdge(_vertexManager.ExecuteManager.VertexStore,
                    new VertexInformation((long)BaseTypes.OutgoingEdge,
                        _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute).GetNextID()), 
                        aToBeAddedOutgoingEdge.AttributeName,
                        aToBeAddedOutgoingEdge.Comment,
                        true,
                        DateTime.UtcNow.ToBinary(),
                        aToBeAddedOutgoingEdge.Multiplicity,
                        new VertexInformation((long)BaseTypes.VertexType, myType.ID),
                        new VertexInformation((long)BaseTypes.EdgeType, edgeType.ID),
                        innerEdgeTypeInfo,
                        new VertexInformation((long)BaseTypes.VertexType, targetVertexType.ID),
                        mySecurityToken, myTransactionToken);
            }
        }

        /// <summary>
        /// Adds attributes.
        /// </summary>
        /// <param name="myToBeAddedIncomingEdges">The to be added incoming edges.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void ProcessAddIncomingEdges(IEnumerable<IncomingEdgePredefinition> myToBeAddedIncomingEdges, 
                                                Int64 myTransactionToken, 
                                                SecurityToken mySecurityToken, 
                                                IVertexType myType)
        {
            foreach (var aIncomingEdgeProperty in myToBeAddedIncomingEdges)
            {
                var targetVertexType = GetType(GetTargetVertexTypeFromAttributeType(aIncomingEdgeProperty.AttributeType), 
                                                myTransactionToken, 
                                                mySecurityToken);

                var targetOutgoingEdge = targetVertexType.GetOutgoingEdgeDefinition(GetTargetEdgeNameFromAttributeType(aIncomingEdgeProperty.AttributeType));

                _baseStorageManager.StoreIncomingEdge(
                    _vertexManager.ExecuteManager.VertexStore,
                    new VertexInformation((long)BaseTypes.IncomingEdge, 
                        _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute).GetNextID()),
                        aIncomingEdgeProperty.AttributeName,
                        aIncomingEdgeProperty.Comment,
                        true,
                        DateTime.UtcNow.ToBinary(),
                        new VertexInformation(
                            (long)BaseTypes.VertexType,
                            myType.ID),
                            new VertexInformation((long)BaseTypes.OutgoingEdge, targetOutgoingEdge.ID),
                        mySecurityToken,
                        myTransactionToken);
            }
        }

        /// <summary>
        /// Adds a mandatory constraint to the specified attributes.
        /// </summary>
        /// <param name="myToBeAddedMandatories">The attributes which should get the constraint.</param>
        /// <param name="myType"The to be altered type.></param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private void AddMandatoryConstraint(IEnumerable<MandatoryPredefinition> myToBeAddedMandatories, 
                                            IVertexType myType, 
                                            Int64 myTransactionToken, 
                                            SecurityToken mySecurityToken)
        {
            if (myToBeAddedMandatories.IsNotNullOrEmpty())
            {
                foreach (var aMandatory in myToBeAddedMandatories)
                {
                    var property = myType.GetPropertyDefinition(aMandatory.MandatoryAttribute);

                    var defaultValue = property.DefaultValue;

                    //get new mandatory value and set it
                    if (aMandatory.DefaultValue != null)
                    {
                        var defaultValueUpdate = new VertexUpdateDefinition(null, 
                                                                            new StructuredPropertiesUpdate(
                                                                                new Dictionary<long, IComparable> 
                                                                                    { { (long)AttributeDefinitions.PropertyDotDefaultValue, 
                                                                                          aMandatory.DefaultValue.ToString() } }));

                        _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, 
                                                                                myTransactionToken, 
                                                                                property.ID, 
                                                                                (long)BaseTypes.Property, 
                                                                                defaultValueUpdate);

                        defaultValue = aMandatory.DefaultValue.ToString();
                    }

                    var vertexDefaultValueUpdate = new VertexUpdateDefinition(null, 
                                                                                new StructuredPropertiesUpdate(
                                                                                    new Dictionary<long, IComparable> 
                                                                                        { { property.ID, defaultValue } }));

                    foreach (var aVertexType in myType.GetDescendantVertexTypesAndSelf())
                    {
                        foreach (var aVertexWithoutPropery in _vertexManager.ExecuteManager.VertexStore.GetVerticesByTypeID(mySecurityToken, 
                                                                                                                            myTransactionToken, 
                                                                                                                            myType.ID).Where(_ => !_.HasProperty(property.ID)).ToList())
                        {
                            if (defaultValue != null)
                                //update
                                _vertexManager.ExecuteManager.VertexStore.UpdateVertex(mySecurityToken, 
                                                                                        myTransactionToken, 
                                                                                        aVertexWithoutPropery.VertexID, 
                                                                                        aVertexWithoutPropery.VertexTypeID, 
                                                                                        vertexDefaultValueUpdate);
                            else
                                throw new MandatoryConstraintViolationException(aMandatory.MandatoryAttribute);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a unique constraint to the specified attributes.
        /// </summary>
        /// <param name="myToBeAddedUniques">The attributes which should get a unique constraint.</param>
        /// <param name="myTransactionToken">The TransaktionToken.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        private IEnumerable<IIndexDefinition> AddUniqueConstraint(RequestAlterVertexType myAlterVertexTypeRequest, 
                                                                    Int64 myTransactionToken, 
                                                                    SecurityToken mySecurityToken)
        {
            var indexDefs = new List<IIndexDefinition>();

            var uniques = myAlterVertexTypeRequest.ToBeAddedUniques;

            if (!uniques.IsNotNullOrEmpty()) 
                return indexDefs;

            foreach (var aUniqueConstraint in uniques)
            {
                var predef = new IndexPredefinition(myAlterVertexTypeRequest.TypeName)
                    .AddProperty(aUniqueConstraint.Properties);

                indexDefs.Add(_indexManager.CreateIndex(predef, mySecurityToken, myTransactionToken));
            }

            return indexDefs;
        }

        /// <summary>
        /// Adds indices.
        /// </summary>
        /// <param name="myToBeAddedIndices"></param>
        /// <param name="vertexType"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        private void AddIndices(IEnumerable<IndexPredefinition> myToBeAddedIndices, 
                                IVertexType myType, 
                                Int64 myTransactionToken, 
                                SecurityToken mySecurityToken)
        {
            if (!myToBeAddedIndices.IsNotNullOrEmpty())
                return;

            foreach (var aToBeAddedIndex in myToBeAddedIndices)
            {
                _indexManager.CreateIndex(aToBeAddedIndex, mySecurityToken, myTransactionToken);
            }
        }

        #endregion
    }
}
