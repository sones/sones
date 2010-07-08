/*
 * CreateIndexAttributeNode
 * (c) Achim Friedland, 2009 - 2010
 */

#region usings

using System;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeNode node.
    /// </summary>

    public class IndexAttributeNode : AStructureNode, IAstNodeInit
    {

        #region Properties

        private String _IndexAttribute  = null;
        private String _OrderDirection  = null;
        private String _IndexType       = null;

        #endregion

        #region Constructor

        public IndexAttributeNode()
        { }

        #endregion

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.ChildNodes[0].HasChildNodes())
            {

                if (myParseTreeNode.ChildNodes[0].ChildNodes[0].ChildNodes.Count > 1)
                {

                    _IndexType = ((ATypeNode) myParseTreeNode.ChildNodes[0].ChildNodes[0].ChildNodes[0].AstNode).DBTypeStream.Name;

                    if (((IDNode) myParseTreeNode.ChildNodes[0].ChildNodes[0].ChildNodes[2].AstNode).IsValidated == false)
                        throw new GraphDBException(new Error_IndexTypesOverlap());
                    else
                        _IndexAttribute = ((IDNode) myParseTreeNode.ChildNodes[0].ChildNodes[0].ChildNodes[2].AstNode).LastAttribute.Name;                        

                }

                else
                {
                    _IndexAttribute = myParseTreeNode.ChildNodes[0].ChildNodes[0].Token.ValueString;
                }

            }

            if(myParseTreeNode.ChildNodes.Count > 1 && myParseTreeNode.ChildNodes[1].HasChildNodes())
                _OrderDirection = myParseTreeNode.ChildNodes[1].FirstChild.Token.ValueString;

            else
                _OrderDirection = String.Empty;

        }

        #endregion

        #region Accessessors

        public String IndexAttribute { get { return _IndexAttribute; } }
        public String OrderDirection { get { return _OrderDirection; } }
        public String IndexTypes     { get { return _IndexType; } }

        #endregion

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            if (_OrderDirection.Equals(String.Empty))
                return String.Concat(_IndexAttribute);

            else
                return String.Concat(_IndexAttribute, " ", _OrderDirection);

        }

        #endregion

    }

}
