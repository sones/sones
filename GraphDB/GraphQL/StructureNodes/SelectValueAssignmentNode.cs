/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/*
 * SelectValueAssignmentNode
 * (c) Stefan Licht, 2009-2010
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Select;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{
    public class SelectValueAssignmentNode : AStructureNode
    {

        public SelectValueAssignment ValueAssignment { get; private set; }

        public SelectValueAssignmentNode()
        {
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (!parseNode.HasChildNodes())
            {
                return;
            }

            #region Static select

            if (parseNode.ChildNodes[0].Token.AsSymbol == GetGraphQLGrammar(context).S_EQUALS)
            {
                ValueAssignment = new SelectValueAssignment(SelectValueAssignment.ValueAssignmentTypes.Always, new ValueDefinition(parseNode.ChildNodes[1].Token.Value));
                //ValueAssignment = new Tuple<ValueAssignmentType, object>(ValueAssignmentType.Always, parseNode.ChildNodes[2].Token.Value);
            }
            else
            {
                ValueAssignment = new SelectValueAssignment(SelectValueAssignment.ValueAssignmentTypes.IfNotExists, new ValueDefinition(parseNode.ChildNodes[1].Token.Value));
                //ValueAssignment = new Tuple<ValueAssignmentType, object>(ValueAssignmentType.IfNotExists, parseNode.ChildNodes[2].Token.Value);
            }

            #endregion


        }

    }

}
