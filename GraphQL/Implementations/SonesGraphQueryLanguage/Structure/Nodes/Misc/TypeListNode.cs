using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using System.Collections.Generic;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.Misc
{
    public sealed class VertexTypeListNode : AStructureNode, IAstNodeInit
    {
        #region Properties

        public List<TypeReferenceDefinition> Types { get; private set; }

        #endregion

        #region Constructor

        public VertexTypeListNode()
        {
            Types = new List<TypeReferenceDefinition>();
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                foreach (var child in parseNode.ChildNodes)
                {
                    if (child.AstNode is ATypeNode)
                    {
                        var tr = (child.AstNode as ATypeNode).ReferenceAndType;
                        if (!Types.Contains(tr))
                        {
                            Types.Add(tr);
                        }
                        else
                        {
                            throw new DuplicateReferenceOccurrenceException(tr.TypeName);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
