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
using System.Collections.Generic;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class EdgeTypeEdgeElementNode : AStructureNode, IAstNodeInit
    {
        public String ReferencedEdgeTypeName { get; private set; }
        public List<Tuple<Int64, Dictionary<String, object>>> Edges { get; private set; }

        public EdgeTypeEdgeElementNode()
        {
            Edges = new List<Tuple<long, Dictionary<String, object>>>();
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            ReferencedEdgeTypeName = parseNode.ChildNodes[1].Token.ValueString;

            var tupleNode = parseNode.ChildNodes[3].AstNode as TupleNode;

            if (tupleNode == null)
            {
                throw new NotImplementedQLException("");
            }

            foreach (var aTupleElement in tupleNode.TupleDefinition)
            {
                Edges.Add(new Tuple<Int64, Dictionary<String, object>>(
                    Convert.ToInt64(((ValueDefinition)aTupleElement.Value).Value), 
                                    aTupleElement.Parameters));
            }
        }

        #endregion
    }
}
