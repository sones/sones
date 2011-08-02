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

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class ExecuteVertexTypeManager: ATypeManager<IVertexType>
    {
        private readonly IDManager _idManager;

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
            throw new NotImplementedException();
        }

        public override void Load(TransactionToken myTransaction,
                                    SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
