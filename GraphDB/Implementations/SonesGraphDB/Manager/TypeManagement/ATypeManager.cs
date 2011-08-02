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
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal abstract class ATypeManager<T>: ITypeHandler<T>
        where T: IBaseType
    {
        #region ITypeManager Members

        public abstract T GetType(long myTypeId, 
                                    TransactionToken myTransaction, 
                                    SecurityToken mySecurity);

        public abstract T GetType(string myTypeName, 
                                    TransactionToken myTransaction, 
                                    SecurityToken mySecurity);

        public abstract IEnumerable<T> GetAllTypes(TransactionToken myTransaction, 
                                                    SecurityToken mySecurity);
        
        public abstract IEnumerable<T> AddTypes(IEnumerable<ATypePredefinition> myTypePredefinitions, 
                                                    TransactionToken myTransaction, 
                                                    SecurityToken mySecurity);

        public abstract Dictionary<Int64, String> RemoveTypes(IEnumerable<T> myTypes, 
                                                                TransactionToken myTransaction, 
                                                                SecurityToken mySecurity, 
                                                                bool myIgnoreReprimands = false);

        public abstract IEnumerable<long> ClearTypes(TransactionToken myTransaction, 
                                                        SecurityToken mySecurity);

        public abstract void TruncateType(long myTypeID, 
                                            TransactionToken myTransactionToken, 
                                            SecurityToken mySecurityToken);

        public abstract void TruncateType(String myTypeName, 
                                            TransactionToken myTransactionToken, 
                                            SecurityToken mySecurityToken);

        //public abstract IBaseType AlterVertexType(RequestAlterVertexType myAlterVertexTypeRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        public abstract bool HasType(string myTypeName, 
                                        SecurityToken mySecurityToken, 
                                        TransactionToken myTransactionToken);

        public abstract void CleanUpTypes();

        #endregion

        #region IManager Members

        public abstract void Initialize(IMetaManager myMetaManager);

        public abstract void Load(TransactionToken myTransaction, 
                                    SecurityToken mySecurity);

        #endregion
    }
}
