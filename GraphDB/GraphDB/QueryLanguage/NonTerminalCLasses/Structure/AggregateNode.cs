/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* <id name="sones GraphDB – aggregate node" />
 * <copyright file="AggregateNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an aggregate statement.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    /// <summary>
    /// This node is requested in case of an aggregate statement.
    /// </summary>
    public class AggregateNode : FuncCallNode
    {

        #region Data

        private ABaseAggregate _Aggregate = null;

        #endregion

        #region constructor

        public AggregateNode()
        {
            
        }

        #endregion

        public new void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;

            base.GetContent(context, parseNode);
            _Aggregate = dbContext.DBPluginManager.GetAggregate(_FuncName);
        }

        public ABaseAggregate Aggregate { get { return _Aggregate; } }
    }
}
