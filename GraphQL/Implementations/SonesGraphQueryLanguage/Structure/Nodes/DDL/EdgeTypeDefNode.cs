using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.Misc;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is the edgeType definition. A simple EdgyTypeList, BackwardEdge (make an implicit backward edge visible to the user)
    /// or a own defined edge derived from AListEdgeType, AListBaseEdgeType, ASingleEdgeType
    /// </summary>
    public sealed class EdgeTypeDefNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public String Type { get; private set; }

        public String Name { get; private set; }

        /// <summary>
        /// The characteristics of an edge (Backward, Mandatory, Unique - valid for a combination of typeattributes
        /// </summary>
        public TypeCharacteristics TypeCharacteristics { get; private set; }

        /// <summary>
        /// An edge type definition - resolved by the name and found in the typeManager EdgeTypesLookup table
        /// </summary>
        public String EdgeType { get; private set; }

        #endregion

        #region constructor

        public EdgeTypeDefNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region TypeEdge class

            EdgeType = parseNode.ChildNodes[3].Token.ValueString;

            #endregion

            if (parseNode.FirstChild.Term is IdentifierTerminal)
            {
                #region simple id

                Type = SonesGQLConstants.SingleType;

                Name = parseNode.ChildNodes[0].Token.ValueString;

                #endregion
            }

            else if (parseNode.FirstChild.Term.Name.ToUpper().Equals(SonesGQLConstants.INCOMINGEDGE.ToUpper()))
            {
                Name = ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition.Reference.Item2.Name;
                TypeCharacteristics = new TypeCharacteristics();
                TypeCharacteristics.IsIncomingEdge = true;
            }

            else
            {

                Name = parseNode.ChildNodes[2].Token.ValueString;

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == SonesGQLGrammar.TERMINAL_SET)
                {
                    Type = SonesGQLGrammar.TERMINAL_SET;
                }

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == SonesGQLGrammar.TERMINAL_LIST)
                {
                    Type = SonesGQLGrammar.TERMINAL_LIST;
                }


            }
        }

        #endregion
    }
}
