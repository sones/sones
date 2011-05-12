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
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// A collection of all params for a EdgeType definition
    /// </summary>
    public sealed class EdgeTypeParamsNode : AStructureNode, IAstNodeInit
    {
        private List<EdgeTypeParamDefinition> _Parameters;
        public EdgeTypeParamDefinition[] Parameters
        {
            get
            {
                if (_Parameters == null)
                    return new EdgeTypeParamDefinition[0];
                return _Parameters.ToArray();
            }
        }

        public EdgeTypeParamsNode()
        {
            _Parameters = new List<EdgeTypeParamDefinition>();
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            foreach (var child in parseNode.ChildNodes)
            {
                _Parameters.Add((child.AstNode as EdgeTypeParamNode).EdgeTypeParamDefinition);

            }
        }

        #endregion
    }
}
