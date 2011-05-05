using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.Misc;
using System.Collections.Generic;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class VertexTypeVertexElementNode : AStructureNode, IAstNodeInit
    {
        public String ReferencedVertexTypeName {get; private set;}
        public List<Tuple<Int64, Dictionary<String, object>>> VertexIDs { get; private set; }

        public VertexTypeVertexElementNode()
        {
            VertexIDs = new List<Tuple<long, Dictionary<String, object>>>();
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            ReferencedVertexTypeName = parseNode.ChildNodes[1].Token.ValueString;

            var tupleNode = parseNode.ChildNodes[3].AstNode as TupleNode;

            if (tupleNode == null)
            {
                throw new NotImplementedQLException("");
            }

            foreach (var aTupleElement in tupleNode.TupleDefinition)
            {
                VertexIDs.Add(new Tuple<Int64, Dictionary<String, object>>(Convert.ToInt64(((ValueDefinition)aTupleElement.Value).Value), aTupleElement.Parameters));
            }
        }

        #endregion
    }
}
