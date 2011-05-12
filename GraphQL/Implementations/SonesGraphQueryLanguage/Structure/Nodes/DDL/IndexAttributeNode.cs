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
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeNode node.
    /// </summary>
    public sealed class IndexAttributeNode : AStructureNode, IAstNodeInit
    {
        #region Properties

        public IndexAttributeDefinition IndexAttributeDefinition { get; private set; }

        #endregion

        #region Data

        private IDChainDefinition _IndexAttribute = null;
        private String _OrderDirection = null;
        private String _IndexType = null;

        #endregion

        #region Constructor

        public IndexAttributeNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode myParseTreeNode)
        {
            if (HasChildNodes(myParseTreeNode.ChildNodes[0]))
            {

                if (myParseTreeNode.ChildNodes[0].ChildNodes[0].AstNode is IDNode)
                {
                    _IndexAttribute = (myParseTreeNode.ChildNodes[0].ChildNodes[0].AstNode as IDNode).IDChainDefinition;
                }
                else
                {
                    throw new NotImplementedQLException(myParseTreeNode.ChildNodes[0].ChildNodes[0].AstNode.GetType().ToString());
                }

            }

            if (myParseTreeNode.ChildNodes.Count > 1 && HasChildNodes(myParseTreeNode.ChildNodes[1]))
            {
                _OrderDirection = myParseTreeNode.ChildNodes[1].FirstChild.Token.ValueString;
            }
            else
            {
                _OrderDirection = String.Empty;
            }
            #region index attribute validation

            if (_IndexAttribute.Count() > 2)
            {
                throw new NotImplementedQLException("Only one attribute and one optional function are allowed! e.g. Name.TOUPPER()");
            }

            #endregion

            IndexAttributeDefinition = new IndexAttributeDefinition(_IndexAttribute, _IndexType, _OrderDirection);

        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            if (_OrderDirection.Equals(String.Empty))
                return String.Concat(_IndexAttribute);

            else
                return String.Concat(_IndexAttribute, " ", _OrderDirection);

        }

        #endregion
    }
}
