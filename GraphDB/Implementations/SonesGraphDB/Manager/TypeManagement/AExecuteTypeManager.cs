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
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Request;
using System.Collections;
using sones.GraphDB.ErrorHandling;
using sones.Library.LanguageExtensions;
using sones.Library.Commons.VertexStore.Definitions;
using sones.GraphDB.Manager.Index;
using sones.Library.Commons.VertexStore.Definitions.Update;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal abstract class AExecuteTypeManager<T> : ATypeManager<T>
        where T : IBaseType
    {
        #region Data

        protected IDManager                     _idManager;
        protected IIndexManager                 _indexManager;

        protected IDictionary<long, IBaseType>  _baseTypes;
        protected IDictionary<String, long>     _nameIndex;

        /// <summary>
        /// A property expression on VertexType.Name
        /// </summary>
        protected readonly IExpression _VertexTypeNameExpression        
                                        = new PropertyExpression(BaseTypes.VertexType.ToString(), "Name");
        /// <summary>
        /// A property expression on EdgeType.Name
        /// </summary>
        protected readonly IExpression _EdgeTypeNameExpression          
                                        = new PropertyExpression(BaseTypes.EdgeType.ToString(), "Name");
        /// <summary>
        /// A property expression on VertexType.ID
        /// </summary>
        protected readonly IExpression _VertexTypeVertexIDExpression    
                                        = new PropertyExpression(BaseTypes.VertexType.ToString(), "VertexID");
        /// <summary>
        /// A property expression on EdgeType.ID
        /// </summary>
        protected readonly IExpression _EdgeDotEdgeTypeIDExpression     
                                        = new PropertyExpression(BaseTypes.EdgeType.ToString(), "VertexID");
        /// <summary>
        /// A property expression on OutgoingEdge.Name
        /// </summary>
        protected readonly IExpression _attributeNameExpression         
                                        = new PropertyExpression(BaseTypes.OutgoingEdge.ToString(), "Name");

        #endregion

        #region constructor

        public AExecuteTypeManager(IDManager myIDManager)
        {
            _idManager = myIDManager;
        }

        #endregion

        #region ATypeManager member

        public override T GetType(long myTypeId,
                                    Int64 myTransaction,
                                    SecurityToken mySecurity)
        {
            #region get static types

            if (_baseTypes.ContainsKey(myTypeId))
                return (T)_baseTypes[myTypeId];

            #endregion

            #region get from fs

            var vertex = Get(myTypeId, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with ID {0} was not found.", myTypeId));

            return CreateType(vertex);

            #endregion
        }

        public override T GetType(string myTypeName,
                                    Int64 myTransaction,
                                    SecurityToken mySecurity)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new EmptyTypeNameException("The type name must contain at least one character.");

            #region get static types

            if (_nameIndex.ContainsKey(myTypeName))
                return (T)_baseTypes[_nameIndex[myTypeName]];

            #endregion

            #region get from fs

            var vertex = Get(myTypeName, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A type with name {0} was not found.", myTypeName));

            if (_baseTypes.ContainsKey(vertex.VertexID))
                return (T)_baseTypes[vertex.VertexID];

            return CreateType(vertex);

            #endregion
        }

        public override abstract IEnumerable<T> GetAllTypes(Int64 myTransaction,
                                                                SecurityToken mySecurity);

        public override IEnumerable<T> AddTypes(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                Int64 myTransaction,
                                                SecurityToken mySecurity)
        {
            return Add(myTypePredefinitions, myTransaction, mySecurity);
        }

        public override Dictionary<long, string> RemoveTypes(IEnumerable<T> myTypes,
                                                                Int64 myTransaction,
                                                                SecurityToken mySecurity,
                                                                bool myIgnoreReprimands = false)
        {
            if (myTypes == null)
                throw new ArgumentNullException("myTypes");

            return Remove(myTypes, myTransaction, mySecurity, myIgnoreReprimands);
        }

        public override IEnumerable<long> ClearTypes(Int64 myTransaction,
                                                        SecurityToken mySecurity)
        {
            //get all UserDefined types
            var toDeleteVertexTypes = GetAllTypes(myTransaction, 
                                                    mySecurity).Where(_ => _.IsUserDefined == true);

            //delete them, by ignoring there reprimands
            return RemoveTypes(toDeleteVertexTypes, 
                                myTransaction, 
                                mySecurity, 
                                true).Select(_ => _.Key).ToList();
        }

        public override abstract void TruncateType(long myTypeID,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken);

        public override abstract void TruncateType(string myTypeName,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken);

        public override bool HasType(string myTypeName,
                                        Int64 myTransactionToken,
                                        SecurityToken mySecurityToken)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new EmptyTypeNameException("The type name must contain at least one character.");

            #region get static types

            if (_nameIndex.ContainsKey(myTypeName))
                return true;

            #endregion

            var vertex = Get(myTypeName, myTransactionToken, mySecurityToken);

            return vertex != null;
        }

        public override abstract void CleanUpTypes();

        public override T AlterType(IRequestAlterType myAlterTypeRequest,
                                    Int64 myTransactionToken,
                                    SecurityToken mySecurityToken,
                                    out RequestUpdate myUpdateRequest)
        {
            CheckRequestType(myAlterTypeRequest);
            
            var type = GetType(myAlterTypeRequest.TypeName, myTransactionToken, mySecurityToken);

            RequestUpdate updateRequest = (myAlterTypeRequest is RequestAlterVertexType) ? 
                                            new RequestUpdate(new RequestGetVertices(type.ID)) 
                                            : new RequestUpdate();

            #region remove stuff

            AlterType_Remove(myAlterTypeRequest, 
                                type, 
                                myTransactionToken, 
                                mySecurityToken,
                                ref updateRequest);

            CleanUpTypes();

            #endregion

            #region add stuff

            AlterType_Add(myAlterTypeRequest,
                            type,
                            myTransactionToken,
                            mySecurityToken,
                            ref updateRequest);

            CleanUpTypes();

            #endregion

            type = GetType(myAlterTypeRequest.TypeName,
                                myTransactionToken,
                                mySecurityToken);

            RenamesAndComment(myAlterTypeRequest,
                                type, 
                                myTransactionToken,
                                mySecurityToken);

            CleanUpTypes();

            CallRebuildIndices(myTransactionToken, mySecurityToken);

            myUpdateRequest = updateRequest;

            return GetType(type.ID, myTransactionToken, mySecurityToken);
        }

        public override abstract void Initialize(IMetaManager myMetaManager);

        public override abstract void Load(Int64 myTransaction,
                                            SecurityToken mySecurity);

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override abstract bool IsTypeBaseType(long myTypeID);

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override abstract bool IsTypeBaseType(string myTypeName);

        #endregion

        #region abstract helper methods

        /// <summary>
        /// Creates a new type.
        /// </summary>
        /// <param name="myVertex">The vertex which is used for creation.</param>
        /// <returns>The created type.</returns>
        protected abstract T CreateType(IVertex myVertex);

        /// Does the necessary checks for can add with the use of the FS.
        /// </summary>
        /// <param name="myDefsTopologically">A topologically sorted list of type predefinitions. 
        /// <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myDefsByName">The same type predefinitions as in <paramref name="myDefsTpologically"/>
        /// , but indexed by their name. <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <remarks><paramref name="myDefsTopologically"/> and 
        /// <paramref name="myDefsByName"/> must contain the same type predefinitions. This is never checked.</remarks>
        /// The predefinitions are checked one by one in topologically order. 
        protected abstract void CanAddCheckWithFS(
                                    LinkedList<ATypePredefinition> myDefsTopologically,
                                    IDictionary<string, ATypePredefinition> myDefsByName,
                                    Int64 myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Checks if the attribute names on type definitions are unique, containing parent myAttributes.
        /// </summary>
        /// <param name="myTopologicallySortedPointer">A pointer to a type predefinitions in a topologically sorted linked list.</param>
        /// <param name="myAttributes">A dictionary type name to attribute names, 
        /// that is build up during the process of CanAddCheckWithFS.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        protected abstract void CanAddCheckAttributeNameUniquenessWithFS(
                                    LinkedListNode<ATypePredefinition> myTopologicallySortedPointer,
                                    IDictionary<string, HashSet<string>> myAttributes,
                                    Int64 myTransaction,
                                    SecurityToken mySecurity);

        /// <summary>
        /// Generates TypeInfo's of the given types.
        /// </summary>
        /// <param name="myDefsSortedTopologically">The predefintions of the to be created types.</param>
        /// <param name="myDefsByName">The predefintions of the to be created types.</param>
        /// <param name="myFirstID">The first id.</param>
        /// <param name="myTransaction">Int64</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <returns>The created TypeInfo's.</returns>
        protected abstract Dictionary<String, TypeInfo> 
                            GenerateTypeInfos(  
                                LinkedList<ATypePredefinition> myDefsSortedTopologically,
                                IDictionary<string, ATypePredefinition> myDefsByName,
                                long myFirstID,
                                Int64 myTransaction,
                                SecurityToken mySecurity);

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeName"/>.
        /// </summary>
        /// <param name="myTypeName">The vertex type name.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given name or <c>NULL</c>, if not present.</returns>
        protected abstract IVertex Get(string myTypeName,
                                        Int64 myTransaction,
                                        SecurityToken mySecurity);

        /// <summary>
        /// Gets an IVertex representing the type given by <paramref name="myTypeName"/>.
        /// </summary>
        /// <param name="myTypeName">The type id.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the type with the given name or <c>NULL</c>, if not present.</returns>
        protected abstract IVertex Get(long myTypeID,
                                        Int64 myTransaction,
                                        SecurityToken mySecurity);

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
        protected abstract IEnumerable<T> StoreTypeAndAttributes(
                                            LinkedList<ATypePredefinition> myDefsTopologically,
                                            Dictionary<String, TypeInfo> myTypeInfos,
                                            long myCreationDate,
                                            int myResultPos,
                                            Int64 myTransactionToken,
                                            SecurityToken mySecurityToken,
                                            ref IVertex[] myResult);

        /// <summary>
        /// Reservs myCountOfNeededIDs type ids in id manager depending on type and gets the first reserved id.
        /// </summary>
        /// <param name="myCountOfNeededIDs">Count of to be reserved ids.</param>
        /// <returns>The first reserved id.</returns>
        protected abstract long GetFirstTypeID(int myCountOfNeededIDs);

        /// <summary>
        /// Removes the given types from the graphDB.
        /// </summary>
        /// <param name="myVertexTypes">The types to delete.</param>
        /// <param name="myTransaction">Transaction token.</param>
        /// <param name="mySecurity">Security Token.</param>
        /// <param name="myIgnoreReprimands">True means, that reprimands (IncomingEdges) 
        /// on the types wich should be removed are ignored.</param>
        /// <returns>Set of deleted type IDs.</returns>
        protected abstract Dictionary<Int64, String> Remove(IEnumerable<T> myTypes,
                                                            Int64 myTransaction,
                                                            SecurityToken mySecurity,
                                                            bool myIgnoreReprimands = false);

        /// <summary>
        /// Checks if the given parameter type is valid.
        /// </summary>
        /// <param name="myRequest">The parameter to be checked.</param>
        protected override abstract void CheckRequestType(IRequestAlterType myRequest);

        /// <summary>
        /// All to be removed things of the alter type request are going to be removed inside this method,
        /// the related operations will be executed inside here.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <param name="myUpdateRequest">A reference to an update request to update relevant vertices.</param>
        protected abstract void AlterType_Remove(IRequestAlterType myAlterTypeRequest,
                                                    T myType,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken,
                                                    ref RequestUpdate myUpdateRequest);

        /// <summary>
        /// All to be added things of the alter type request are going to be added inside this method,
        /// the related operations will be executed inside here.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="myUpdateRequest">A reference to an update request to update relevant vertices.</param>
        protected abstract void AlterType_Add(IRequestAlterType myAlterTypeRequest,
                                                    T myType,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken,
                                                    ref RequestUpdate myUpdateRequest);

        /// <summary>
        /// Adds the specified properties to the given type and stores them.
        /// </summary>
        /// <param name="myToBeAddedProperties">The to be added properties.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <returns>A dictionary with to be added attributes and default value</returns>returns>
        protected abstract Dictionary<long, IComparable> ProcessAddPropery(
            IEnumerable<PropertyPredefinition> myToBeAddedProperties,
            Int64 myTransactionToken,
            SecurityToken mySecurityToken,
            T myType);

        /// <summary>
        /// Defines specified attributes in the given type and stores them.
        /// </summary>
        /// <param name="myToBeDefinedAttributes">The attributes to be defined</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <param name="myType">The type to be altered.</param>
        /// <returns>A dictionary with to be defined attributes and default value</returns>returns>
        protected abstract Dictionary<long, IComparable> ProcessDefineAttributes(
            IEnumerable<UnknownAttributePredefinition> myToBeDefinedAttributes,
            Int64 myTransactionToken,
            SecurityToken mySecurityToken,
            T myType);

        /// <summary>
        /// Renames attributes.
        /// </summary>
        /// <param name="myToBeRenamedAttributes">The to be renamed attributes.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected abstract void RenameAttributes(Dictionary<string, string> myToBeRenamedAttributes,
                                                    T myType,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken);
        
        /// <summary>
        /// Change the comment on the type.
        /// </summary>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myNewComment">The new comment.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected abstract void ChangeCommentOnType(T myType,
                                                    string myNewComment,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken);
        
        /// <summary>
        /// Renames a type.
        /// </summary>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myNewTypeName">The new type name.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected abstract void RenameType(T myType, 
                                            string myNewTypeName, 
                                            Int64 myTransactionToken, 
                                            SecurityToken mySecurityToken);

        /// <summary>
        /// Calls the RebuildIndices method of the index manager.
        /// </summary>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected abstract void CallRebuildIndices(Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken);
        
        #endregion

        #region helper structure

        protected struct TypeInfo
        {
            public VertexInformation VertexInfo;
            public long AttributeCountWithParents;
        }

        #endregion

        #region private methods

        /// <summary>
        /// <summary>
        /// Adds a type by reading out the predefinitions and stores all attributes and the type.
        /// </summary>
        /// <param name="myTypePredefinitions">The predefinitions for the creation.</param>
        /// <param name="myTransaction">Int64</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <returns>The created types.</returns>
        protected IEnumerable<T> Add(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                        Int64 myTransaction,
                                        SecurityToken mySecurity)
        {
            #region preparations

            var typePredefinitions = myTypePredefinitions;

            //Perf: count is necessary, fast if it is an ICollection
            var count = typePredefinitions.Count();

            //This operation reserves #count ids for this operation.
            var firstTypeID = GetFirstTypeID(count);

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
                                                                              kvp => kvp.Value));

            CanAddCheckWithFS(defsTopologically, 
                                defsByVertexName, 
                                myTransaction, 
                                mySecurity);

            var typeInfos = GenerateTypeInfos(defsTopologically, 
                                                defsByVertexName, 
                                                firstTypeID, 
                                                myTransaction, 
                                                mySecurity);

            //we can add each type separately
            var creationDate = DateTime.UtcNow.ToBinary();
            var resultPos = 0;

            var result = new IVertex[count];

            #endregion

            #region store

            var resultTypes = StoreTypeAndAttributes(defsTopologically,
                                                        typeInfos,
                                                        creationDate,
                                                        resultPos,
                                                        myTransaction,
                                                        mySecurity,
                                                        ref result);

            #endregion

            CleanUpTypes();

            return resultTypes;
        }

        protected void StoreProperties(LinkedList<ATypePredefinition> myDefsTopologically,
                                        Dictionary<String, TypeInfo> myTypeInfos,
                                        long myCreationDate,
                                        Int64 myTransactionToken,
                                        SecurityToken mySecurityToken)
        {
            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                if (current.Value.Properties == null)
                    continue;

                var firstAttrID = _idManager.GetVertexTypeUniqeID((long)BaseTypes.Attribute)
                                            .ReserveIDs(current.Value.PropertyCount);

                var currentExternID = myTypeInfos[current.Value.TypeName]
                                        .AttributeCountWithParents - 
                                        current.Value.PropertyCount - 1;

                foreach (var prop in current.Value.Properties)
                {
                    _baseStorageManager.StoreProperty(
                        _vertexManager.ExecuteManager.VertexStore,
                        new VertexInformation((long)BaseTypes.Property, firstAttrID++),
                        prop.AttributeName,
                        prop.Comment,
                        myCreationDate,
                        prop.IsMandatory,
                        prop.Multiplicity,
                        prop.DefaultValue,
                        true,
                        myTypeInfos[current.Value.TypeName].VertexInfo,
                        ConvertBasicType(prop.AttributeType),
                        mySecurityToken,
                        myTransactionToken);
                }

            }
        }

        /// <summary>
        /// Checks if the name of the given vertex type predefinition is not used in FS before.
        /// </summary>
        /// <param name="myTypePredefinition">The name of this vertex type definition will be checked.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        protected void CanAddCheckVertexNameUniqueWithFS(ATypePredefinition myTypePredefinition, 
                                                            Int64 myTransaction, 
                                                            SecurityToken mySecurity)
        {
            if (Get(myTypePredefinition.TypeName, myTransaction, mySecurity) != null)
                throw new DuplicatedTypeNameException(myTypePredefinition.TypeName);
        }

        /// <summary>
        /// Gets the parent predefinition of the given predefinition.
        /// </summary>
        /// <param name="myCurrent">The predefinition of that the parent predefinition is searched.</param>
        /// <returns>The link to the parent predefinition of the 
        /// <paramref name="myCurrent"/> predefinition, otherwise <c>NULL</c>.</returns>
        protected static LinkedListNode<ATypePredefinition> 
                            GetParentPredefinitionOnTopologicallySortedList(
                                LinkedListNode<ATypePredefinition> myCurrent)
        {
            for (var parent = myCurrent.Previous; parent != null; parent = parent.Previous)
            {
                if (parent.Value.TypeName.Equals(myCurrent.Value.SuperTypeName))
                    return parent;
            }
            return null;
        }

        protected VertexInformation ConvertBasicType(string myBasicTypeName)
        {
            return _baseTypeManager.ConvertBaseType(myBasicTypeName);
        }

        /// <summary>
        /// Calls methods to rename attrributes and changing the comment.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected void RenamesAndComment(IRequestAlterType myAlterTypeRequest,
                                            T myType,
                                            Int64 myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            RenameAttributes(myAlterTypeRequest.ToBeRenamedProperties,
                                           myType,
                                           myTransactionToken,
                                           mySecurityToken);

            ChangeCommentOnType(myType,
                                myAlterTypeRequest.AlteredComment,
                                myTransactionToken,
                                mySecurityToken);

            RenameType(myType,
                        myAlterTypeRequest.AlteredTypeName,
                        myTransactionToken,
                        mySecurityToken);
        }

        /// <summary>
        /// Removes properties.
        /// </summary>
        /// <param name="myToBeRemovedProperties">The to be removed edges.</param>
        /// <param name="myType">The to be altered type.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <returns>A list with the deleted property id's.</returns>
        protected IEnumerable<long> ProcessPropertyRemoval(IEnumerable<string> myToBeRemovedProperties,
                                                            T myType,
                                                            Int64 myTransactionToken,
                                                            SecurityToken mySecurityToken)
        {
            List<long> removed = null;

            foreach (var aProperty in myToBeRemovedProperties)
            {
                #region remove related indices

                var propertyDefinition = myType.GetPropertyDefinition(aProperty);

                foreach (var aIndexDefinition in propertyDefinition.InIndices)
                {
                    _indexManager.RemoveIndexInstance(aIndexDefinition.ID,
                                                        myTransactionToken,
                                                        mySecurityToken);
                }

                #endregion

                removed = removed ?? new List<long>();
                removed.Add(propertyDefinition.ID);

                _vertexManager.ExecuteManager.VertexStore.RemoveVertex(mySecurityToken,
                                                                        myTransactionToken,
                                                                        propertyDefinition.ID,
                                                                        (long)BaseTypes.Property);
            }

            return removed;
        }

        #endregion
    }
}
