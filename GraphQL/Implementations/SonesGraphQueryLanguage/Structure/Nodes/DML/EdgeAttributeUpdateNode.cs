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

using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.Misc;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of attribute assign statement.
    /// </summary>
    public sealed class EdgeAttributeUpdateNode : AStructureNode, IAstNodeInit
    {

        #region Data

        public AAttributeAssignOrUpdate AttributeValue { get; private set; }

        private IDChainDefinition _AttributeIDNode = null;

        #endregion

        #region constructor

        public EdgeAttributeUpdateNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get myAttributeName

            _AttributeIDNode = ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition;

            #endregion

            var _Node = parseNode.ChildNodes[2];

            if ((_Node.AstNode is CollectionOfEdgesNode))
            {
                #region collection like list

                AttributeValue = new AttributeAssignOrUpdateList((_Node.AstNode as CollectionOfEdgesNode).CollectionDefinition, _AttributeIDNode, true);

                #endregion
            }
            else
            {
                throw new NotImplementedQLException("");
            }
        }

        #endregion
    }
}
