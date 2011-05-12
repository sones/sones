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
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class SelectValueAssignmentNode : AStructureNode, IAstNodeInit
    {
        public SelectValueAssignment ValueAssignment { get; private set; }

        public SelectValueAssignmentNode()
        {
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (!(parseNode.ChildNodes != null && parseNode.ChildNodes.Count > 0))
            {
                return;
            }

            #region Static select

            if (parseNode.ChildNodes[0].Token.KeyTerm == ((SonesGQLGrammar)context.Language.Grammar).S_EQUALS)
            {
                ValueAssignment = new SelectValueAssignment(SelectValueAssignment.ValueAssignmentTypes.Always, new ValueDefinition(parseNode.ChildNodes[1].Token.Value));
            }
            else
            {
                ValueAssignment = new SelectValueAssignment(SelectValueAssignment.ValueAssignmentTypes.IfNotExists, new ValueDefinition(parseNode.ChildNodes[1].Token.Value));
            }

            #endregion

        }

        #endregion
    }
}
