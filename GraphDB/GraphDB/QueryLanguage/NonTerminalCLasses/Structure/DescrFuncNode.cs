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


/* <id name="sones GraphDB – DescrFuncNode" />
 * <copyright file="DescrFuncNode.cs"
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
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescrFuncNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet>    _FunctionValues;
        private ABaseFunction                       _Function = null;
        #endregion

        #region constructor
        public DescrFuncNode()
        {
        }
        #endregion

        #region AStructureNode
        
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            _FunctionValues = new List<SelectionResultSet>();
            DescrFuncOutput Output = new DescrFuncOutput();
                        
            try
            {
                if (parseNode.HasChildNodes())
                {
                    _Function = typeManager.DBPluginManager.GetFunction(parseNode.ChildNodes[1].Token.ValueString.ToUpper());

                    if (_Function != null)
                        _FunctionValues.Add(new SelectionResultSet(Output.GenerateOutput(_Function, parseNode.ChildNodes[1].Token.ValueString, typeManager)));
                    else
                        throw new GraphDBException(new Error_AggregateOrFunctionDoesNotExist(parseNode.ChildNodes[1].Token.ValueString));
                    
                }
            }
            catch (Exception e)
            {
                throw e;                
            }
        }
       
        #endregion

        #region Accessors
        public override List<SelectionResultSet> Result
        { get { return _FunctionValues; } }
        #endregion
    }
}
