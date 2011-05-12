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
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// One single BackwardEdge definition node.
    /// </summary>
    public sealed class IncomingEdgeNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public IncomingEdgeDefinition BackwardEdgeDefinition { get; private set; }

        /// <summary>
        /// The destination type of the backwardedge
        /// </summary>
        private String _TypeName;

        /// <summary>
        /// the destination attribute on the TypeName
        /// </summary>
        private String _TypeAttributeName;

        /// <summary>
        /// The real new name of the attribute
        /// </summary>
        private String _AttributeName;

        #endregion

        #region constructor

        public IncomingEdgeNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region Extract type and attribute

            //if (parseNode.ChildNodes.Count != 4)
            //    throw new Exception("This is not a [Type].[Attribute] definition: " + parseNode.ChildNodes[0].ToString());

            _TypeName = parseNode.ChildNodes[0].Token.ValueString;
            _TypeAttributeName = parseNode.ChildNodes[2].Token.ValueString;

            #endregion

            _AttributeName = parseNode.ChildNodes[3].Token.ValueString;

            BackwardEdgeDefinition = new IncomingEdgeDefinition(_AttributeName, _TypeName, _TypeAttributeName);

        }

        #endregion
    }
}
