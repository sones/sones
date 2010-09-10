/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="PandoraDB – DescrFunctionsNode" />
 * <copyright file="DescrFunctionsNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
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
    public class DescrFunctionsNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet> _Functions;
        #endregion

        #region constructor
        public DescrFunctionsNode()
        {
        }
        #endregion

        #region AStructureNode
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            _Functions = new List<SelectionResultSet>();

            try
            {
                DescrFuncOutput Output = new DescrFuncOutput();
                Dictionary<string, ABaseFunction> Functions = typeManager.DBPluginManager.GetAllFunctions();


                foreach (var Func in Functions)
                    _Functions.Add(new SelectionResultSet(Output.GenerateOutput(Func.Value, Func.Value.FunctionName, typeManager)));

            }
            catch (Exception e)
            {
                throw new GraphDBException(new Error_UnknownDBError(e));
            }
        }

        #endregion


        #region ADescrNode
        public override List<SelectionResultSet> Result
        {
            get { return _Functions; }
        }
        #endregion
    }
}
