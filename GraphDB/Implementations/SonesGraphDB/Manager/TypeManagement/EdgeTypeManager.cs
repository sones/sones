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
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class EdgeTypeManager : IManagerOf<IEdgeTypeHandler>
    {
        #region Data

        private readonly CheckEdgeTypeManager _check;
        private readonly ExecuteEdgeTypeManager _execute;

        #endregion

        public EdgeTypeManager(IDManager myIDManager)
        {
            _check = new CheckEdgeTypeManager();
            _execute = new ExecuteEdgeTypeManager(myIDManager);
        }

        #region IManagerOf<IEdgeTypeManager> Members

        public IEdgeTypeHandler CheckManager
        {
            get { return _check; }
        }

        public IEdgeTypeHandler ExecuteManager
        {
            get { return _execute; }
        }

        public IEdgeTypeHandler UndoManager
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IManager Members

        void IManager.Initialize(IMetaManager myMetaManager)
        {
            _execute.Initialize(myMetaManager);
        }

        void IManager.Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _execute.Load(myTransaction, mySecurity);
        }

        #endregion
    }
}
