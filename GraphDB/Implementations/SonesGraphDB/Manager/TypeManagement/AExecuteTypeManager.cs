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
    internal abstract class AExecuteTypeManager<T>: ATypeManager<T>
        where T: IBaseType
    {
        #region ATypeManager member

        public override abstract T GetType(long myTypeId, 
                                            TransactionToken myTransaction, 
                                            SecurityToken mySecurity);

        public override abstract T GetType(string myTypeName, 
                                            TransactionToken myTransaction, 
                                            SecurityToken mySecurity);

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
                                                SecurityToken mySecurityToken, 
                                                TransactionToken myTransactionToken);

        public override abstract void CleanUpTypes();

        public override abstract void Initialize(IMetaManager myMetaManager);

        public override abstract void Load(TransactionToken myTransaction, 
                                            SecurityToken mySecurity);

        #endregion
    }
}
