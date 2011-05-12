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
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class UniqueAttributesOptNode : AStructureNode, IAstNodeInit
    {
        private List<String> _UniqueAttributes;
        public List<String> UniqueAttributes
        {
            get { return _UniqueAttributes; }
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                _UniqueAttributes = new List<String>();

                if (HasChildNodes(parseNode.ChildNodes[1]))
                {
                    _UniqueAttributes = (from c in parseNode.ChildNodes[1].ChildNodes select c.Token.ValueString).ToList();
                }

            }
        }

        #endregion
    }
}
