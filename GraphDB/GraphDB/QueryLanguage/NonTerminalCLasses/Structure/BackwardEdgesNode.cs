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


/* <id name="sones GraphDB – BackwardEdgesNode" />
 * <copyright file="BackwardEdgesNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
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
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class BackwardEdgesNode : AStructureNode
    {
        #region Data

        /// <summary>
        /// The information about the BackwardEdge: &lt;Type, Attribute, Visible AttributeName&gt;
        /// </summary>
        public List<BackwardEdgeDefinition> BackwardEdgeInformation
        {
            get { return _BackwardEdgeInformation; }
        }
        private List<BackwardEdgeDefinition> _BackwardEdgeInformation;

        #endregion

        #region constructor

        public BackwardEdgesNode()
        {
            _BackwardEdgeInformation = new List<BackwardEdgeDefinition>();
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                foreach (ParseTreeNode node in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (node.AstNode as BackwardEdgeNode != null)
                    {
                        _BackwardEdgeInformation.Add(((BackwardEdgeNode)node.AstNode).BackwardEdgeDefinition);
                    }
                }
            }
        }

    }
}
