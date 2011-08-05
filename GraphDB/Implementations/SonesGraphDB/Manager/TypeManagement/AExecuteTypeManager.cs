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

namespace sones.GraphDB.Manager.TypeManagement
{
    internal abstract class AExecuteTypeManager<T> : ATypeManager<T>
        where T : IBaseType
    {
        #region Data

        protected IDictionary<long, IBaseType> _baseTypes;
        protected IDictionary<String, long> _nameIndex;

        /// <summary>
        /// A property expression on VertexType.Name
        /// </summary>
        protected readonly IExpression _vertexTypeNameExpression = new PropertyExpression(BaseTypes.VertexType.ToString(), "Name");
        /// <summary>
        /// A property expression on VertexType.ID
        /// </summary>
        protected readonly IExpression _vertexTypeIDExpression = new PropertyExpression(BaseTypes.VertexType.ToString(), "VertexID");

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

        public override abstract IEnumerable<T> AddTypes(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                            TransactionToken myTransaction,
                                                            SecurityToken mySecurity);

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

        public override abstract bool HasType(string myTypeName,
                                                TransactionToken myTransactionToken,
                                                SecurityToken mySecurityToken);

        public override abstract void CleanUpTypes();

        public override abstract void Initialize(IMetaManager myMetaManager);

        public override abstract void Load(TransactionToken myTransaction,
                                            SecurityToken mySecurity);

        #endregion

        #region abstract helper methods

        protected abstract T CreateType(IVertex myVertex);

        #endregion

        #region private methods

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

            return _vertexManager.ExecuteManager.GetSingleVertex(new BinaryExpression(_vertexTypeIDExpression, 
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
        private IVertex Get(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.ExecuteManager.GetSingleVertex(new BinaryExpression(_vertexTypeNameExpression, 
                                                                                        BinaryOperator.Equals, 
                                                                                        new SingleLiteralExpression(myTypeName)), 
                                                                    myTransaction, mySecurity);

            #endregion
        }

        #endregion
    }
}
