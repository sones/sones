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
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// Contains the default value for an attribute.
    /// </summary>
    public sealed class AttrDefaultValueNode : AStructureNode, IAstNodeInit
    {

        #region Properties

        public string Value { get; private set; }

        #endregion

        #region constructors

        public AttrDefaultValueNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                Value = parseNode.ChildNodes[1].Token.ValueString;
            }
            else
            {
                Value = null;
            }
        }

        #endregion
    }
}
