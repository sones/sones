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

/* <id name="sones GraphDB – DescrTypeNode" />
 * <copyright file="DescrTypeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{

    public class DescribeTypeNode : ADescrNode
    {

        #region Data

        private GraphDBType              _DBType = null;
        private List<SelectionResultSet> _TypeValues;
        
        #endregion

        #region Properties

        public override List<SelectionResultSet> Result
        {
            get
            {
                return _TypeValues;
            }
        }

        #endregion

        #region Constructor
        
        public DescribeTypeNode()
        {            
        }

        #endregion

        #region ASructureNode
        
        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            var _DBContext      = myCompilerContext.IContext as DBContext;

            Debug.Assert(_DBContext != null, "myCompilerContext.IContext must not be null!");

            var _DBTypeManager  = _DBContext.DBTypeManager;

            _TypeValues = new List<SelectionResultSet>();
            _DBType     = _DBTypeManager.GetTypeByName(myParseTreeNode.ChildNodes[1].Token.ValueString);

            if (_DBType != null)
            {

                var _DBObjectReadouts = new List<DBObjectReadout>();
                var _DescrTypesOutput = new DescrTypesOutput();

                _TypeValues.Add(new SelectionResultSet(_DescrTypesOutput.GenerateOutput(_DBContext, _DBType)));

            }

            else
                throw new GraphDBException(new Error_TypeDoesNotExist(myParseTreeNode.ChildNodes[1].Token.ValueString));

        }        

        #endregion

    }

}
