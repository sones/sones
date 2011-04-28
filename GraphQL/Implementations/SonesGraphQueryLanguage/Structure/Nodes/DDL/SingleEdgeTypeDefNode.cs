using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is the edgeType definition. A simple EdgyTypeList, BackwardEdge (make an implicit backward edge visible to the user)
    /// or a own defined edge derived from AListEdgeType, AListBaseEdgeType, ASingleEdgeType
    /// </summary>
    public sealed class SingleEdgeTypeDefNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public String Type { get { return _Type; } }
        String _Type;

        public String Name { get { return _Name; } }
        String _Name = null;

        /// <summary>
        /// The characteristics of an edge (Backward, Mandatory, Unique - valid for a combination of typeattributes
        /// </summary>
        public TypeCharacteristics TypeCharacteristics { get { return _TypeCharacteristics; } }
        TypeCharacteristics _TypeCharacteristics = null;

        /// <summary>
        /// An edge type definition - resolved by the name and found in the typeManager EdgeTypesLookup table
        /// </summary>
        public String EdgeType { get; private set; }

        public EdgeTypeParamDefinition[] Parameters { get; private set; }

        #endregion

        #region Constructor

        public SingleEdgeTypeDefNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region TypeEdge class

            //if (!dbContext.DBPluginManager.HasEdgeType(myParseTreeNode.ChildNodes[0].Token.ValueString))
            //    throw new GraphDBException(new Error_EdgeTypeDoesNotExist(myParseTreeNode.ChildNodes[0].Token.ValueString));

            //_EdgeType = dbContext.DBPluginManager.GetEdgeType(myParseTreeNode.ChildNodes[0].Token.ValueString);

            EdgeType = parseNode.ChildNodes[0].Token.ValueString;

            if (parseNode.ChildNodes[1].AstNode != null)
            {
                Parameters = ((EdgeTypeParamsNode)parseNode.ChildNodes[1].AstNode).Parameters;
            }

            #endregion

            _Name = parseNode.ChildNodes[3].Token.ValueString;

            _Type = SonesGQLConstants.SingleType;
        }

        #endregion
    }
}
