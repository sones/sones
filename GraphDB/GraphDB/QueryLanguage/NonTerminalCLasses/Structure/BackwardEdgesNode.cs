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

/* <id name="PandoraDB – BackwardEdgesNode" />
 * <copyright file="BackwardEdgesNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A list of single BackwardEdge definition nodes.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.Lib.DataStructures;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class BackwardEdgesNode : AStructureNode
    {
        #region Data

        /// <summary>
        /// The information about the BackwardEdge: &lt;Type, Attribute, Visible AttributeName&gt;
        /// </summary>
        public List<BackwardEdgeNode> BackwardEdgeInformation
        {
            get { return _BackwardEdgeInformation; }
        }
        private List<BackwardEdgeNode> _BackwardEdgeInformation;

        #endregion

        #region constructor

        public BackwardEdgesNode()
        {
            _BackwardEdgeInformation = new List<BackwardEdgeNode>();
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                foreach (ParseTreeNode node in parseNode.ChildNodes[1].ChildNodes)
                {
                    _BackwardEdgeInformation.Add((BackwardEdgeNode)node.AstNode);
                }
            }
        }

    }
}
