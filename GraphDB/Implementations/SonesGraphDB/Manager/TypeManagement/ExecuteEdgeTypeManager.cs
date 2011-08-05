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

        //public override IEdgeType GetType(long myTypeId,
        //                                    TransactionToken myTransaction,
        //                                    SecurityToken mySecurity)
        //{
        //    #region get static types

        //    if (_baseTypes.ContainsKey(myTypeId))
        //        return _baseTypes[myTypeId] as IEdgeType;

        //    #endregion


        //    #region get from fs

        //    var vertex = Get(myTypeId, myTransaction, mySecurity);

        //    if (vertex == null)
        //        throw new KeyNotFoundException(string.Format("A vertex type with ID {0} was not found.", myTypeId));

        //    var result = new EdgeType(vertex, _baseStorageManager);

        //    _baseTypes.Add(result.ID, result);
        //    _nameIndex.Add(result.Name, result.ID);

        //    return result;

        //    #endregion
        //}

        //public override IEdgeType GetType(string myTypeName,
        //                                    TransactionToken myTransaction,
        //                                    SecurityToken mySecurity)
        //{
        //    throw new NotImplementedException();
        //}

        public override IEnumerable<IEdgeType> GetAllTypes(TransactionToken myTransaction,
                                                            SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IEdgeType> AddTypes(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                            TransactionToken myTransaction,
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

        public override bool HasType(string myTypeName,
                                        TransactionToken myTransactionToken,
                                        SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public override void CleanUpTypes()
        {
            throw new NotImplementedException();
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _vertexManager = myMetaManager.VertexManager;
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

        private void LoadBaseType(TransactionToken myTransaction, SecurityToken mySecurity, params BaseTypes[] myBaseTypes)
        {
            foreach (var baseType in myBaseTypes)
            {
                var vertex = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction, (long)baseType, (long)BaseTypes.EdgeType, String.Empty);
                
                if (vertex == null)
                    throw new BaseEdgeTypeNotExistException(baseType.ToString(), "Could not load base edge type.");

                _baseTypes.Add((long)baseType, new EdgeType(vertex, _baseStorageManager));
            }
        }

        #endregion
    }
}
