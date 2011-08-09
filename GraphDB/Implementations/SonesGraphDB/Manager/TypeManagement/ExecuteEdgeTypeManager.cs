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
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Request;
using sones.Library.Commons.VertexStore.Definitions;
using sones.GraphDB.Expression;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class ExecuteEdgeTypeManager: AExecuteTypeManager<IEdgeType>
    {
        #region data

        private readonly IDManager _idManager;
        
        #endregion

        #region constructor

        public ExecuteEdgeTypeManager(IDManager myIDManager)
        {
            _idManager = myIDManager;

            _baseTypes = new Dictionary<long, IBaseType>();
            _nameIndex = new Dictionary<String, long>();
        }

        #endregion

        #region ACheckTypeManager member

        public override IEdgeType AlterType(IRequestAlterType myAlterTypeRequest, 
                                            TransactionToken myTransactionToken, 
                                            SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IEdgeType> GetAllTypes(TransactionToken myTransaction,
                                                            SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<long, string> RemoveTypes(IEnumerable<IEdgeType> myTypes,
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

            foreach (var type in new[] { BaseTypes.Edge, BaseTypes.Orderable, BaseTypes.Weighted })
            {
                _baseTypes.Add((long)type, help[(long)type]);
                _nameIndex.Add(type.ToString(), (long)type);
            }
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _indexManager       = myMetaManager.IndexManager;
            _vertexManager      = myMetaManager.VertexManager;
            _baseTypeManager    = myMetaManager.BaseTypeManager;
            _baseStorageManager = myMetaManager.BaseGraphStorageManager;
        }

        public override void Load(TransactionToken myTransaction,
                                    SecurityToken mySecurity)
        {
            LoadBaseType(
                myTransaction,
                mySecurity,
                BaseTypes.Edge,
                BaseTypes.Weighted,
                BaseTypes.Orderable);
        }

        protected override IEdgeType CreateType(IVertex myVertex)
        {
            var result = new EdgeType(myVertex, _baseStorageManager);

            _baseTypes.Add(result.ID, result);
            _nameIndex.Add(result.Name, result.ID);

            return result;
        }

        #endregion

        #region private helper

        private void LoadBaseType(TransactionToken myTransaction, 
                                    SecurityToken mySecurity, 
                                    params BaseTypes[] myBaseTypes)
        {
            foreach (var baseType in myBaseTypes)
            {
                var vertex = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, 
                                                                                    myTransaction, 
                                                                                    (long)baseType, 
                                                                                    (long)BaseTypes.EdgeType, 
                                                                                    String.Empty);
                
                if (vertex == null)
                    throw new BaseEdgeTypeNotExistException(baseType.ToString(), "Could not load base edge type.");

                _baseTypes.Add((long)baseType, new EdgeType(vertex, _baseStorageManager));
            }
        }

        /// <summary>
        /// Adds a type by reading out the predefinitions and stores all attributes and the type.
        /// </summary>
        /// <param name="myTypePredefinitions">The predefinitions for the creation.</param>
        /// <param name="myTransaction">TransactionToken</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <returns>The created types.</returns>
        protected override IEnumerable<IEdgeType> Add(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                        TransactionToken myTransaction,
                                                        SecurityToken mySecurity)
        {
            var typePredefinitions = myTypePredefinitions as IEnumerable<EdgeTypePredefinition>;

            //Perf: count is necessary, fast if it is an ICollection
            var count = typePredefinitions.Count();

            //This operation reserves #count ids for this operation.
            var firstTypeID = _idManager.EdgeTypeID.ReserveIDs(count);

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
                                                                              kvp => kvp.Value as IEnumerable<ATypePredefinition>));

            CanAddCheckWithFS(defsTopologically, defsByVertexName, myTransaction, mySecurity);

            var typeInfos = GenerateTypeInfos(defsTopologically, defsByVertexName, firstTypeID, myTransaction, mySecurity);

            //we can add each type separately
            var creationDate = DateTime.UtcNow.ToBinary();
            var resultPos = 0;

            var result = new IVertex[count];

            //now we store each vertex type
            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                var newEdgeType = _baseStorageManager.StoreEdgeType(
                     _vertexManager.ExecuteManager.VertexStore,
                     typeInfos[current.Value.TypeName].VertexInfo,
                     current.Value.TypeName,
                     current.Value.Comment,
                     creationDate,
                     current.Value.IsAbstract,
                     current.Value.IsSealed,
                     true,
                     typeInfos[current.Value.SuperTypeName].VertexInfo,
                     mySecurity,
                     myTransaction);

                result[resultPos++] = newEdgeType;

                //get the matching index and add the created type name to it
                _indexManager.GetIndex(BaseUniqueIndex.EdgeTypeDotName)
                                .Add(current.Value.TypeName, typeInfos[current.Value.TypeName].VertexInfo.VertexID);

                _nameIndex.Add(current.Value.TypeName, typeInfos[current.Value.TypeName].VertexInfo.VertexID);
            }

            #region Store Attributes

            //The order of adds is important. 
            //First property, then outgoing edges (that might point to properties) and finally incoming edges (that might point to outgoing edges).
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

            var resultTypes = new EdgeType[result.Length];

            //reload the IVertex objects, that represents the type.
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction,
                                                                                result[i].VertexID,
                                                                                result[i].VertexTypeID, String.Empty);

                var newVertexType = new EdgeType(result[i], _baseStorageManager);

                resultTypes[i] = newVertexType;

                _baseTypes.Add(typeInfos[newVertexType.Name].VertexInfo.VertexID, newVertexType);
            }

            #endregion

            //CleanUpTypes();

            return resultTypes;
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
                    throw new InvalidBaseTypeException(myTopologicallySortedPointer.Value.SuperTypeName);

                if (parent.GetProperty<bool>((long)AttributeDefinitions.BaseTypeDotIsSealed))
                    //The parent type is sealed.
                    throw new SealedBaseTypeException(myTopologicallySortedPointer.Value.TypeName, 
                                                        parent.GetPropertyAsString((long)AttributeDefinitions.AttributeDotName));

                var parentType = new EdgeType(parent, _baseStorageManager);
                var attributeNames = parentType.GetAttributeDefinitions(true).Select(_ => _.Name);

                myAttributes[myTopologicallySortedPointer.Value.TypeName] = new HashSet<string>(attributeNames);
            }
            else
            {
                myAttributes[myTopologicallySortedPointer.Value.TypeName] = new HashSet<string>(myAttributes[parentPredef.Value.TypeName]);
            }

            var attributeNamesSet = myAttributes[myTopologicallySortedPointer.Value.TypeName];

            CheckPropertiesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
        }

        /// <summary>
        /// Does the necessary checks for can add with the use of the FS.
        /// </summary>
        /// <param name="myDefsTopologically">A topologically sorted list of vertex type predefinitions. 
        ///     <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myDefsByName">The same vertex type predefinitions as in 
        ///     <paramref name="myDefsTpologically"/>, but indexed by their name. 
        ///     <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        ///     <remarks><paramref name="myDefsTopologically"/> and 
        ///     <paramref name="myDefsByName"/> must contain the same vertex type predefinitions. This is never checked.</remarks>
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
            }
        }

        protected override Dictionary<String, TypeInfo> GenerateTypeInfos(
                                                            LinkedList<ATypePredefinition> myDefsSortedTopologically,
                                                            IDictionary<string, ATypePredefinition> myDefsByName,
                                                            long myFirstID,
                                                            TransactionToken myTransaction,
                                                            SecurityToken mySecurity)
        {
            var neededTypes = new HashSet<string>();

            foreach (var def in myDefsByName)
            {
                neededTypes.Add(def.Value.TypeName);
                neededTypes.Add(def.Value.SuperTypeName);
            }

            //At most all vertex types are needed.
            var result = new Dictionary<String, TypeInfo>((int)myFirstID + myDefsByName.Count);

            foreach (var type in neededTypes)
            {
                if (myDefsByName.ContainsKey(type))
                {
                    result.Add(type, new TypeInfo
                    {
                        AttributeCountWithParents = myDefsByName[type].AttributeCount,
                        VertexInfo = new VertexInformation((long)BaseTypes.EdgeType, myFirstID++)
                    });
                }
                else
                {
                    var vertex = _vertexManager.ExecuteManager.GetSingleVertex(
                                    new BinaryExpression(new SingleLiteralExpression(type),
                                                            BinaryOperator.Equals,
                                                            _EdgeTypeNameExpression),
                                    myTransaction, mySecurity);

                    IEdgeType neededType = new EdgeType(vertex, _baseStorageManager);

                    result.Add(type, new TypeInfo
                    {
                        AttributeCountWithParents = neededType.GetAttributeDefinitions(true).LongCount(),
                        VertexInfo = new VertexInformation((long)BaseTypes.EdgeType, _baseStorageManager.GetUUID(vertex))
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
                    .GetSingleVertex(new BinaryExpression(_EdgeTypeNameExpression,
                                                            BinaryOperator.Equals,
                                                            new SingleLiteralExpression(myTypeName)),
                                        myTransaction, mySecurity);

            #endregion
        }

        #endregion
    }
}
