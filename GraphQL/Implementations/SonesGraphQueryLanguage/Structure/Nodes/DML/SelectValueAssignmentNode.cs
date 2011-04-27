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
