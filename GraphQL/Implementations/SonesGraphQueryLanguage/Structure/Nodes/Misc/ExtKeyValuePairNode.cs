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
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.DML;

namespace sones.GraphQL.Structure.Nodes.Misc
{
    /// <summary>
    /// This node is requested in case of a keyvaluepair nonterminal.
    /// </summary>
    public sealed class ExtKeyValuePairNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public KeyValuePair<String, IEnumerable<object>> KeyValue { get; private set; }

        #endregion

        #region constructor

        public ExtKeyValuePairNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode.ChildNodes[2]))
                KeyValue =
                    new KeyValuePair<string, IEnumerable<object>>(
                            parseNode.ChildNodes[0].Token.ValueString,
                            ((parseNode
                                .ChildNodes[2]
                                .AstNode as CollectionOfBasicDBObjectsNode)
                                    .CollectionDefinition.TupleDefinition as TupleDefinition)
                                        .TupleElements.Select(_ => (_.Value as ValueDefinition).Value));
            else
                KeyValue = 
                    new KeyValuePair<string, IEnumerable<object>>(
                            parseNode.ChildNodes[0].Token.ValueString, 
                            new List<object> { parseNode.ChildNodes[2].Token.Value });
        }

        #endregion
    }
}
