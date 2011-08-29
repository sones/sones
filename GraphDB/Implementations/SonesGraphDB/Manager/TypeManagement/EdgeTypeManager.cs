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
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class EdgeTypeManager: IManagerOf<ITypeHandler<IEdgeType>>
    {
        #region Data

        private readonly CheckEdgeTypeManager _check;
        private readonly ExecuteEdgeTypeManager _execute;

        #endregion

        #region c'tor

        public EdgeTypeManager(IDManager myIDManager)
        {
            _check = new CheckEdgeTypeManager();
            _execute = new ExecuteEdgeTypeManager(myIDManager);
        }

        #endregion

        #region IManagerOf<IVertexTypeManager> Members

        public ITypeHandler<IEdgeType> CheckManager
        {
            get {  return _check; }
        }

        public ITypeHandler<IEdgeType> ExecuteManager
        {
            get { return _execute; }
        }

        public ITypeHandler<IEdgeType> UndoManager
        {
            get { throw new NotImplementedException(); }
        }

        void IManager.Initialize(IMetaManager myMetaManager)
        {
            _execute.Initialize(myMetaManager);
            _check.Initialize(myMetaManager);
        }

        void IManager.Load(Int64 myTransaction, SecurityToken mySecurity)
        {
            _execute.Load(myTransaction, mySecurity);
            _check.Load(myTransaction, mySecurity);
        }

        #endregion
    }
}
