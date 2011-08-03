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
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.BaseGraph;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ErrorHandling.Type;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class ExecuteVertexTypeManager: ATypeManager<IVertexType>
    {
        private readonly IDictionary<long, IVertexType>     _baseTypes = new Dictionary<long, IVertexType>();
        private readonly IDictionary<String, long>          _nameIndex = new Dictionary<String, long>();
        private readonly IDManager                          _idManager;
        private IManagerOf<IVertexHandler>                  _vertexManager;
        private IIndexManager                               _indexManager;
        private IManagerOf<ITypeHandler<IEdgeType>>         _edgeManager;
        private BaseGraphStorageManager                     _baseStorageManager;

        public ExecuteVertexTypeManager(IDManager myIDManager)
        {
            _idManager = myIDManager;
        }

        #region ACheckTypeManager member

        public override IVertexType GetType(long myTypeId,
                                            TransactionToken myTransaction,
                                            SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public override IVertexType GetType(string myTypeName,
                                            TransactionToken myTransaction,
                                            SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IVertexType> GetAllTypes(TransactionToken myTransaction,
                                                                SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IVertexType> AddTypes(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                            TransactionToken myTransaction,
                                                            SecurityToken mySecurity)
        {
            throw new NotImplementedException();
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

        public override bool HasType(string myTypeName,
                                        SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public override void CleanUpTypes()
        {
            throw new NotImplementedException();
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _edgeManager = myMetaManager.EdgeTypeManager;
            _indexManager = myMetaManager.IndexManager;
            _vertexManager = myMetaManager.VertexManager;
            _baseTypeManager = myMetaManager.BaseTypeManager;
            _baseStorageManager = myMetaManager.BaseGraphStorageManager;
        }

        public override void Load(TransactionToken myTransaction,
                                    SecurityToken mySecurity)
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

        #endregion

        #region private helper

        private long GetMaxID(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.VertexStore.GetVerticesByTypeID(mySecurity, myTransaction, myTypeID);

            if (vertices == null)
                throw new BaseVertexTypeNotExistException("The base vertex types are not available during loading the ExecuteVertexTypeManager.");

            return (vertices.CountIsGreater(0))
                ? vertices.Max(x => x.VertexID)
                : Int64.MinValue;
        }

        private void LoadBaseType(TransactionToken myTransaction, SecurityToken mySecurity, params BaseTypes[] myBaseTypes)
        {
            foreach (var baseType in myBaseTypes)
            {
                var vertex = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction, (long)baseType, (long)BaseTypes.VertexType, String.Empty);

                if (vertex == null)
                    throw new BaseVertexTypeNotExistException(baseType.ToString(), "Could not load base vertex type during loading the ExecuteVertexTypeManager.");

                _baseTypes.Add((long)baseType, new VertexType(vertex, _baseStorageManager));
                _nameIndex.Add(baseType.ToString(), (long)baseType);
            }
        }

        #endregion
    }
}
