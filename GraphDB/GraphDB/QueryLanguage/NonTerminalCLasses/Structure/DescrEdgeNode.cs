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

/* <id name="PandoraDB – DescrEdgeNode" />
 * <copyright file="DescrEdgeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescribeEdgeNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet> _EdgeNodeValues;
        private AEdgeType                       _Edge = null;
        #endregion

        #region constructor
        public DescribeEdgeNode()
        {
        }
        #endregion

        #region AStructureNode        

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            _EdgeNodeValues = new List<SelectionResultSet>();
            DescrEdgesOutput Output = new DescrEdgesOutput();
            
            if (parseNode.HasChildNodes())
            {
                _Edge = dbContext.DBPluginManager.GetEdgeType(parseNode.ChildNodes[1].Token.ValueString);

                if (_Edge != null)
                    _EdgeNodeValues.Add(new SelectionResultSet(Output.GenerateOutput(_Edge, parseNode.ChildNodes[1].Token.ValueString)));
                else
                    throw new GraphDBException(new Error_EdgeTypeDoesNotExist(parseNode.ChildNodes[1].Token.ValueString));
                
            }
            
        }        
       
        #endregion

        #region Accessor
        public override List<SelectionResultSet> Result
        { get { return _EdgeNodeValues; } }
        #endregion
    }
}
