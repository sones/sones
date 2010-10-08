/* <id name="GraphDB – Attribute Definition astnode" />
 * <copyright file="EdgeTraversalNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of attribute definition statement.</summary>
 */

#region Usings

using System;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public class EdgeInformationNode : AStructureNode
    {

        #region Data

        public String EdgeInformationName { get; private set; }
        public SelectionDelimiterNode Delimiter { get; private set; }

        #endregion

        #region constructor

        public EdgeInformationNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            Delimiter = (SelectionDelimiterNode)myParseTreeNode.FirstChild.AstNode;
            EdgeInformationName = myParseTreeNode.ChildNodes[1].Token.ValueString;
        }
    
    }

}
