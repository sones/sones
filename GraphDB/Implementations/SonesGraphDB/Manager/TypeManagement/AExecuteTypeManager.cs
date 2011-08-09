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
        protected readonly IExpression _VertexTypeNameExpression    = new PropertyExpression(BaseTypes.VertexType.ToString(), "Name");
        /// <summary>
        /// A property expression on EdgeType.Name
        /// </summary>
        protected readonly IExpression _EdgeTypeNameExpression      = new PropertyExpression(BaseTypes.EdgeType.ToString(), "Name");
        /// <summary>
        /// A property expression on VertexType.ID
        /// </summary>
        protected readonly IExpression _vertexIDExpression          = new PropertyExpression(BaseTypes.VertexType.ToString(), "VertexID");
        /// <summary>
        /// A property expression on OutgoingEdge.Name
        /// </summary>
        protected readonly IExpression _attributeNameExpression     = new PropertyExpression(BaseTypes.OutgoingEdge.ToString(), "Name");

        #endregion

        #region ATypeManager member

        public override T GetType(long myTypeId,
                                    TransactionToken myTransaction,
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
                                    TransactionToken myTransaction,
                                    SecurityToken mySecurity)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new ArgumentOutOfRangeException("myTypeName", "The type name must contain at least one character.");

            #region get static types

            if (_nameIndex.ContainsKey(myTypeName))
            {
                return (T)_baseTypes[_nameIndex[myTypeName]];
            }

            #endregion

            #region get from fs

            var vertex = Get(myTypeName, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            return CreateType(vertex);

            #endregion
        }

        public override abstract IEnumerable<T> GetAllTypes(TransactionToken myTransaction,
                                                                SecurityToken mySecurity);

        public override IEnumerable<T> AddTypes(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                TransactionToken myTransaction,
                                                SecurityToken mySecurity)
        {
            return Add(myTypePredefinitions, myTransaction, mySecurity);
        }

        public override abstract Dictionary<long, string> RemoveTypes(IEnumerable<T> myTypes,
                                                                        TransactionToken myTransaction,
                                                                        SecurityToken mySecurity,
                                                                        bool myIgnoreReprimands = false);

        public override abstract IEnumerable<long> ClearTypes(TransactionToken myTransaction,
                                                                SecurityToken mySecurity);

        public override abstract void TruncateType(long myTypeID,
                                                    TransactionToken myTransactionToken,
                                                    SecurityToken mySecurityToken);

        public override abstract void TruncateType(string myTypeName,
                                                    TransactionToken myTransactionToken,
                                                    SecurityToken mySecurityToken);

        public override bool HasType(string myTypeName,
                                        TransactionToken myTransactionToken,
                                        SecurityToken mySecurityToken)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new EmptyTypeNameException("The type name must contain at least one character.");

            #region get static types

            if (_nameIndex.ContainsKey(myTypeName))
            {
                return true;
            }

            #endregion

            var vertex = Get(myTypeName, myTransactionToken, mySecurityToken);

            return vertex != null;
        }

        public override abstract void CleanUpTypes();

        public override abstract T AlterType(IRequestAlterType myAlterTypeRequest,
                                                TransactionToken myTransactionToken,
                                                SecurityToken mySecurityToken);

        public override abstract void Initialize(IMetaManager myMetaManager);

        public override abstract void Load(TransactionToken myTransaction,
                                            SecurityToken mySecurity);

        #endregion

        #region abstract helper methods

        /// <summary>
        /// Creates a new type.
        /// </summary>
        /// <param name="myVertex">The vertex which is used for creation.</param>
        /// <returns>The created type.</returns>
        protected abstract T CreateType(IVertex myVertex);

        /// <summary>
        /// Adds a type by reading out the predefinitions and stores all attributes and the type.
        /// </summary>
        /// <param name="myTypePredefinitions">The predefinitions for the creation.</param>
        /// <param name="myTransaction">TransactionToken</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <returns>The created types.</returns>
        protected abstract IEnumerable<T> Add(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                TransactionToken myTransaction,
                                                SecurityToken mySecurity);

        /// <summary>
        /// Does the necessary checks for can add with the use of the FS.
        /// </summary>
        /// <param name="myDefsTopologically">A topologically sorted list of type predefinitions. <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myDefsByName">The same type predefinitions as in <paramref name="myDefsTpologically"/>, but indexed by their name. <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <remarks><paramref name="myDefsTopologically"/> and <paramref name="myDefsByName"/> must contain the same type predefinitions. This is never checked.</remarks>
        /// The predefinitions are checked one by one in topologically order. 
        protected abstract void CanAddCheckWithFS(LinkedList<ATypePredefinition> myDefsTopologically,
                                                    IDictionary<string, ATypePredefinition> myDefsByName,
                                                    TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Checks if the attribute names on type definitions are unique, containing parent myAttributes.
        /// </summary>
        /// <param name="myTopologicallySortedPointer">A pointer to a type predefinitions in a topologically sorted linked list.</param>
        /// <param name="myAttributes">A dictionary type name to attribute names, that is build up during the process of CanAddCheckWithFS.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        protected abstract void CanAddCheckAttributeNameUniquenessWithFS(LinkedListNode<ATypePredefinition> myTopologicallySortedPointer,
                                                                            IDictionary<string, HashSet<string>> myAttributes,
                                                                            TransactionToken myTransaction,
                                                                            SecurityToken mySecurity);

        /// <summary>
        /// Generates TypeInfo's of the given types.
        /// </summary>
        /// <param name="myDefsSortedTopologically">The predefintions of the to be created types.</param>
        /// <param name="myDefsByName">The predefintions of the to be created types.</param>
        /// <param name="myFirstID">The first id.</param>
        /// <param name="myTransaction">TransactionToken</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <returns>The created TypeInfo's.</returns>
        protected abstract Dictionary<String, TypeInfo> GenerateTypeInfos(LinkedList<ATypePredefinition> myDefsSortedTopologically,
                                                                            IDictionary<string, ATypePredefinition> myDefsByName,
                                                                            long myFirstID,
                                                                            TransactionToken myTransaction,
                                                                            SecurityToken mySecurity);
        #endregion

        #region helper structure

        protected struct TypeInfo
        {
            public VertexInformation VertexInfo;
            public long AttributeCountWithParents;
        }

        #endregion

        #region private methods

        #region abstract helper

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeName"/>.
        /// </summary>
        /// <param name="myTypeName">The vertex type name.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given name or <c>NULL</c>, if not present.</returns>
        protected abstract IVertex Get(string myTypeName,
                                        TransactionToken myTransaction,
                                        SecurityToken mySecurity);

        #endregion

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeID"/>.
        /// </summary>
        /// <param name="myTypeId"></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given ID or <c>NULL</c>, if not present.</returns>
        protected IVertex Get(long myTypeId, 
                                TransactionToken myTransaction, 
                                SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.ExecuteManager
                    .GetSingleVertex(new BinaryExpression(_vertexIDExpression, 
                                                            BinaryOperator.Equals, 
                                                            new SingleLiteralExpression(myTypeId)), 
                                        myTransaction, mySecurity);

            #endregion
        }

        /// <summary>
        /// Checks if the name of the given vertex type predefinition is not used in FS before.
        /// </summary>
        /// <param name="myTypePredefinition">The name of this vertex type definition will be checked.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        protected void CanAddCheckVertexNameUniqueWithFS(ATypePredefinition myTypePredefinition, 
                                                            TransactionToken myTransaction, 
                                                            SecurityToken mySecurity)
        {
            if (Get(myTypePredefinition.TypeName, myTransaction, mySecurity) != null)
                throw new DuplicatedTypeNameException(myTypePredefinition.TypeName);
        }

        /// <summary>
        /// Gets the parent predefinition of the given predefinition.
        /// </summary>
        /// <param name="myCurrent">The predefinition of that the parent predefinition is searched.</param>
        /// <returns>The link to the parent predefinition of the <paramref name="myCurrent"/> predefinition, otherwise <c>NULL</c>.</returns>
        protected static LinkedListNode<ATypePredefinition> GetParentPredefinitionOnTopologicallySortedList(
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
               
        #endregion
    }
}
