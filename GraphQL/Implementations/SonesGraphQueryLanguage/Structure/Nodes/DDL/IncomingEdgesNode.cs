/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// A list of single BackwardEdge definition nodes.
    /// </summary>
    public sealed class IncomingEdgesNode : AStructureNode, IAstNodeInit
    {
        #region Data

        /// <summary>
        /// The information about the BackwardEdge: &lt;Type, Attribute, Visible AttributeName&gt;
        /// </summary>
        public List<IncomingEdgeDefinition> BackwardEdgeInformation
        {
            get { return _BackwardEdgeInformation; }
        }
        private List<IncomingEdgeDefinition> _BackwardEdgeInformation;

        #endregion

        #region constructor

        public IncomingEdgesNode()
        {
            _BackwardEdgeInformation = new List<IncomingEdgeDefinition>();
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                foreach (var _ParseTreeNode in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode as IncomingEdgeNode != null)
                    {
                        _BackwardEdgeInformation.Add(((IncomingEdgeNode)_ParseTreeNode.AstNode).BackwardEdgeDefinition);
                    }
                }
            }

        }

        #endregion
    }
}
