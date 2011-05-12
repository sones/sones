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
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager.TypeManagement
{
    public class CheckEdgeTypeManager: IEdgeTypeHandler
    {
        #region IEdgeTypeManager Members

        IEdgeType IEdgeTypeHandler.GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        IEdgeType IEdgeTypeHandler.GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        IEnumerable<IEdgeType> IEdgeTypeHandler.GetAllEdgeTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        IEdgeType IEdgeTypeHandler.AddEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        void IEdgeTypeHandler.RemoveEdgeTypes(IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        void IEdgeTypeHandler.UpdateEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        #endregion

        #region IManager Members

        void IManager.Initialize(IMetaManager myMetaManager)
        {
        }

        void IManager.Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        #endregion
    }
}
