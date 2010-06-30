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


/* <id name="sones GraphDB – DescrAggrNode" />
 * <copyright file="DescrAggrNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescrAggrNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet> _AggrValues;
        private ABaseAggregate _Aggregate = null;
        #endregion

        #region constructor
        public DescrAggrNode()
        {
        }
        #endregion        

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            _AggrValues = new List<SelectionResultSet>();
            DescrAggrsOutput Output = new DescrAggrsOutput();
            
            if (parseNode.HasChildNodes())
            {
                _Aggregate = dbContext.DBPluginManager.GetAggregate(parseNode.ChildNodes[1].Token.ValueString.ToUpper());
                if (_Aggregate != null)
                    _AggrValues.Add(new SelectionResultSet(Output.GenerateOutput(_Aggregate, parseNode.ChildNodes[1].Token.ValueString)));
                else
                    throw new GraphDBException(new Error_AggregateOrFunctionDoesNotExist(parseNode.ChildNodes[1].Token.ValueString));
            }
        }

        #region Accessor
        public override List<SelectionResultSet> Result
        { get { return _AggrValues; } }
        #endregion
    }
}
