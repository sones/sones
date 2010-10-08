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
