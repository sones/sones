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
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Nodes;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;

namespace sones.GraphQL.StatementNodes
{
    /// <summary>
    /// The abstract class for all statements in GraphDB.
    /// </summary>
    public abstract class AStatement : AStructureNode
    {

        #region General Command Infos

        public abstract String StatementName { get; }
        public abstract TypesOfStatements TypeOfStatement { get; }

        #endregion

        #region abstract Methods

        public abstract QueryResult Execute(IGraphDB myGraphDB, 
                                            IGraphQL myGraphQL, 
                                            GQLPluginManager myPluginManager, 
                                            String myQuery, 
                                            SecurityToken mySecurityToken, 
                                            Int64 myTransactionToken);

        #endregion
    }
}
