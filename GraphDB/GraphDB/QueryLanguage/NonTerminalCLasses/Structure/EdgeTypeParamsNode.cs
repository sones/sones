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

/* <id name="sones GraphDB – EdgeTypeParamsNode" />
 * <copyright file="EdgeTypeParamsNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A collection of all params for a EdgeType definition</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    /// <summary>
    /// A collection of all params for a EdgeType definition
    /// </summary>
    public class EdgeTypeParamsNode : AStructureNode, IAstNodeInit
    {

        private List<EdgeTypeParamNode> _Parameters;
        public EdgeTypeParamNode[] Parameters
        {
            get {
                if (_Parameters == null)
                    return new EdgeTypeParamNode[0];
                return _Parameters.ToArray(); 
            }
        }

        public EdgeTypeParamsNode()
        {
            _Parameters = new List<EdgeTypeParamNode>();
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            foreach (var child in parseNode.ChildNodes)
            {
                _Parameters.Add(child.AstNode as EdgeTypeParamNode);
                /*
                if (child.AstNode != null && child.AstNode is AStructureNode)
                    _Parameters.Add(child.AstNode);
                else
                    _Parameters.Add(child.Token.Value);
                  * */
            }
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }
}
