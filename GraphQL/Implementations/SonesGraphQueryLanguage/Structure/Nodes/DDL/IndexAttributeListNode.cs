using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using System.Collections.Generic;
using sones.Library.LanguageExtensions;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeList node.
    /// </summary>
    public sealed class IndexAttributeListNode : AStructureNode, IAstNodeInit
    {
        #region properties

        public List<IndexAttributeDefinition> IndexAttributes { get; private set; }

        #endregion

        #region constructor

        public IndexAttributeListNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region Data

            IndexAttributes = new List<IndexAttributeDefinition>();

            foreach (ParseTreeNode aNode in parseNode.ChildNodes)
            {
                if ((aNode.AstNode as IndexAttributeNode) != null)
                {
                    IndexAttributes.Add((aNode.AstNode as IndexAttributeNode).IndexAttributeDefinition);
                }
            }

            #endregion
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            return IndexAttributes.ToContentString();

        }

        #endregion
    }
}
