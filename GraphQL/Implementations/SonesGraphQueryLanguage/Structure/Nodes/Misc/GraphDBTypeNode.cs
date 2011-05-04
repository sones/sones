using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.DDL;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.Misc
{
    /// <summary>
    /// This node is requested in case of a GraphType statement.
    /// </summary>
    public sealed class VertexTypeNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public DBTypeOfAttributeDefinition DBTypeDefinition { get; private set; }

        #endregion

        #region constructor

        public VertexTypeNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.FirstChild.Term is IdentifierTerminal)
            {

                #region simple id

                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    Type = SonesGQLConstants.SingleType,
                    Name = parseNode.ChildNodes[0].Token.ValueString
                };

                #endregion

            }

            else if (parseNode.FirstChild.Term.Name.ToUpper().Equals(SonesGQLConstants.INCOMINGEDGE.ToUpper()))
            {

                #region BackwardedgeDefinition

                var _TypeCharacteristics = new TypeCharacteristics();
                _TypeCharacteristics.IsIncomingEdge = true;
                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    TypeCharacteristics = _TypeCharacteristics,
                    Name = parseNode.ChildNodes[0].Token.ValueString,
                    Type = SonesGQLConstants.INCOMINGEDGE
                };

                #endregion

            }

            else if (parseNode.FirstChild.AstNode is EdgeTypeDefNode)
            {

                #region EdgeType definition

                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    Type = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).Type,
                    Name = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).Name,
                    TypeCharacteristics = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).TypeCharacteristics,
                    EdgeType = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).EdgeType,
                };

                #endregion

            }

            else if (parseNode.FirstChild.AstNode is SingleEdgeTypeDefNode)
            {

                #region Single edge type definition

                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    Type = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).Type,
                    Name = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).Name,
                    TypeCharacteristics = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).TypeCharacteristics,
                    EdgeType = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).EdgeType,
                };

                #endregion

            }

            else if (parseNode.ChildNodes.Count >= 2)
            {

                String type;

                #region set
                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == SonesGQLGrammar.TERMINAL_SET)
                {
                    type = SonesGQLGrammar.TERMINAL_SET;
                }
                #endregion

                #region list
                else if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == SonesGQLGrammar.TERMINAL_LIST)
                {
                    type = SonesGQLGrammar.TERMINAL_LIST;
                }
                #endregion

                else
                {
                    throw new NotImplementedQLException("");
                }

                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    Type = type,
                    Name = parseNode.ChildNodes[2].Token.ValueString
                };
            }

            else
            {
                throw new ArgumentException("Invalid Graph type definition...");
            }
        }

        #endregion
    }
}
