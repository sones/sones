using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structure.Nodes.DML;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This node is requested in case of an SetRefNode statement.
    /// </summary>
    public sealed class SetRefNode : AStructureNode, IAstNodeInit
    {
        public SetRefDefinition SetRefDefinition { get; private set; }

        #region Data

        public Boolean IsREFUUID = false;

        #endregion

        #region constructor

        public SetRefNode()
        {

        }

        #endregion


        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var grammar = (SonesGQLGrammar)context.Language.Grammar;
            if (parseNode.ChildNodes[0].Term == grammar.S_REFUUID || parseNode.ChildNodes[0].Term == grammar.S_REFERENCEUUID)
            {
                IsREFUUID = true;
            }

            if (parseNode.ChildNodes.Count >3)
            {
                var tupleNode = parseNode.ChildNodes[4].AstNode as TupleNode;

                if (tupleNode == null)
                {
                    throw new NotImplementedQLException("");
                }

                Dictionary<string, object> parameters = null;
                if (parseNode.ChildNodes[5].AstNode is ParametersNode)
                {
                    parameters = (parseNode.ChildNodes[5].AstNode as ParametersNode).ParameterValues;
                }

                String referencedVertexType = parseNode.ChildNodes[2].Token.ValueString;

                SetRefDefinition = new SetRefDefinition(tupleNode.TupleDefinition, IsREFUUID, referencedVertexType, parameters);
            }
            else
            {
                var tupleNode = parseNode.ChildNodes[1].AstNode as TupleNode;

                if (tupleNode == null)
                {
                    throw new NotImplementedQLException("");
                }

                Dictionary<string, object> parameters = null;
                if (parseNode.ChildNodes[2].AstNode is ParametersNode)
                {
                    parameters = (parseNode.ChildNodes[2].AstNode as ParametersNode).ParameterValues;
                }

                SetRefDefinition = new SetRefDefinition(tupleNode.TupleDefinition, IsREFUUID, String.Empty, parameters);
            }

            
        }

        #endregion
    }
}
