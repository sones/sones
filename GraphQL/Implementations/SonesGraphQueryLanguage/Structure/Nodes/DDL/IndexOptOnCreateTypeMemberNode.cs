using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using System.Collections.Generic;
using sones.Library.LanguageExtensions;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class IndexOptOnCreateTypeMemberNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String _IndexName;
        private String _Edition;
        private String _IndexType;
        private List<IndexAttributeDefinition> _IndexAttributeDefinitions { get; set; }

        #endregion

        #region Properties

        public IndexDefinition IndexDefinition { get; private set; }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes.Count < 1)
            {
                throw new ArgumentException("No index definitions found!");
            }

            foreach (var child in parseNode.ChildNodes)
            {

                if (child.AstNode != null)
                {

                    if (child.AstNode is IndexNameOptNode)
                    {
                        _IndexName = (child.AstNode as IndexNameOptNode).IndexName;
                    }

                    else if (child.AstNode is EditionOptNode)
                    {
                        _Edition = (child.AstNode as EditionOptNode).IndexEdition;
                    }

                    else if (child.AstNode is IndexTypeOptNode)
                    {
                        _IndexType = (child.AstNode as IndexTypeOptNode).IndexType;
                    }

                    else if (child.AstNode is IndexAttributeListNode)
                    {
                        _IndexAttributeDefinitions = (child.AstNode as IndexAttributeListNode).IndexAttributes;
                    }

                }

            }


            #region Validation

            if (_IndexAttributeDefinitions.IsNullOrEmpty())
            {
                throw new ArgumentException("No attributes given for index!");
            }

            #endregion

            IndexDefinition = new IndexDefinition(_IndexName, _Edition, _IndexType, _IndexAttributeDefinitions);
        }

        #endregion
    }
}
