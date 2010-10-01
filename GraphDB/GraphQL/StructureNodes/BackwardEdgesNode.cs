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

/* <id name="GraphDB – BackwardEdgesNode" />
 * <copyright file="BackwardEdgesNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A list of single BackwardEdge definition nodes.</summary>
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
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
                foreach (var _ParseTreeNode in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode as BackwardEdgeNode != null)
                    {
                        _BackwardEdgeInformation.Add(((BackwardEdgeNode)_ParseTreeNode.AstNode).BackwardEdgeDefinition);
                    }
                }
            }

        }

    }

}
