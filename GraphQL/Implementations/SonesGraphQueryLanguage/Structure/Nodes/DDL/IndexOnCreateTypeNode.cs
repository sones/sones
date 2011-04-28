using System;
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class IndexOnCreateTypeNode : AStructureNode, IAstNodeInit
    {
        #region Properties

        public List<IndexDefinition> ListOfIndexDefinitions { get; private set; }

        #endregion

        #region constructors

        public IndexOnCreateTypeNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            ListOfIndexDefinitions = new List<IndexDefinition>();

            if (parseNode.ChildNodes[1].AstNode is IndexOptOnCreateTypeMemberNode)
            {
                var aIDX = (IndexOptOnCreateTypeMemberNode)parseNode.ChildNodes[1].AstNode;

                ListOfIndexDefinitions.Add(aIDX.IndexDefinition);
            }

            else
            {
                var idcs = parseNode.ChildNodes[1].ChildNodes.Select(child =>
                {
                    return ((IndexOptOnCreateTypeMemberNode)child.AstNode).IndexDefinition;
                });
                ListOfIndexDefinitions.AddRange(idcs);
            }
        }

        #endregion
    }
}
