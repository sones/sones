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
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.DDL;
using sones.GraphQL.ErrorHandling;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.Misc
{
    /// <summary>
    /// This node is requested in case of a keyvaluelist nonterminal.
    /// </summary>
    public sealed class KeyValueListNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public Dictionary<String, String> KeyValues { get; private set; }

        #endregion

        #region constructor

        public KeyValueListNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            KeyValues = new Dictionary<string, string>();

            if (HasChildNodes(parseNode))
            {
                foreach (var aChildNode in parseNode.ChildNodes)
                {
                    var aKeyValuePairNode = (KeyValuePairNode)aChildNode.AstNode;
                    KeyValues.Add(aKeyValuePairNode.KeyValue.Key, aKeyValuePairNode.KeyValue.Value);
                }
            }
        }

        #endregion
    }
}
