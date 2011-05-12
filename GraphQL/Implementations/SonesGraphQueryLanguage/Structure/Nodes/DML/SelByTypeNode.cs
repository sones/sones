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

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class SelByTypeNode : AStructureNode, IAstNodeInit
    {
        #region Data

        String _TypeName;

        #endregion

        #region constructors

        public SelByTypeNode()
        { }

        #endregion 

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _TypeName = parseNode.ChildNodes[1].Token.Text;
        }

        #endregion

        #region Accessor

        public String TypeName { get { return _TypeName; } }

        #endregion
    }
}
