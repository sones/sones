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
    public sealed class MandatoryOptNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private List<string> _MandAttribs;
        
        #endregion

        #region Accessor
        public List<string> MandatoryAttribs
        {
            get { return _MandAttribs; }
        }
        #endregion

        #region constructor
        public MandatoryOptNode()
        {
            _MandAttribs = new List<string>();
        }
        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {

            if (HasChildNodes(parseNode))
            {
                if (HasChildNodes(parseNode.ChildNodes[1]))
                {
                    _MandAttribs = (from Attr in parseNode.ChildNodes[1].ChildNodes select Attr.Token.ValueString).ToList();
                }
            }

        }

        #endregion
    }
}
