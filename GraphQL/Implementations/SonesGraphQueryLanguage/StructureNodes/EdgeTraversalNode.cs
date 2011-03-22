using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.StructureNodes;
using Irony.Parsing;


namespace sones.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public class EdgeTraversalNode : AStructureNode
    {

        //#region Data

        //public String AttributeName { get; private set; }
        //public FuncCallNode FuncCall { get; private set; }
        //public SelectionDelimiterNode Delimiter { get; private set; }
        
        //#endregion

        #region constructor

        public EdgeTraversalNode()
        {
            
        }

        #endregion

        public void GetContent(ParsingContext myParsingContext, ParseTreeNode myParseTreeNode)
        {

            //Delimiter = (SelectionDelimiterNode)myParseTreeNode.FirstChild.AstNode;

            //if (myParseTreeNode.ChildNodes[1].AstNode == null)
            //{
            //    //AttributeName
            //    AttributeName = myParseTreeNode.ChildNodes[1].Token.ValueString;
            //}

            //else
            //{
            //    FuncCall = (FuncCallNode)myParseTreeNode.ChildNodes[1].AstNode;
            //}

        }
                
    }

}
