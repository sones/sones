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

using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
namespace sones.GraphQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This node is requested in case of an aggregate statement.
    /// </summary>
    public sealed class AggregateNode : FuncCallNode
    {
        public AggregateDefinition AggregateDefinition { get; private set; }

        #region constructor

        public AggregateNode()
        {

        }

        #endregion

        public void Aggregate_Init(ParsingContext context, ParseTreeNode parseNode)
        {
            base.Init(context, parseNode);

            AggregateDefinition = new AggregateDefinition(new ChainPartAggregateDefinition(base.FuncDefinition));
        }
    }
}
