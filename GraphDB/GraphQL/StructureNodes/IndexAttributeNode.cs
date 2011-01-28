/*
 * CreateIndexAttributeNode
 * (c) Stefan Licht, 2009 - 2010
 * (c) Achim Friedland, 2009 - 2010
 */

#region usings

using System;
using System.Linq;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Errors;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeNode node.
    /// </summary>

    public class IndexAttributeNode : AStructureNode, IAstNodeInit
    {

        #region Properties

        public IndexAttributeDefinition IndexAttributeDefinition { get; private set; }

        #endregion

        #region Data

        private IDChainDefinition _IndexAttribute = null;
        private String _OrderDirection = null;
        private String _IndexType = null;

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

                if (myParseTreeNode.ChildNodes[0].ChildNodes[0].AstNode is IDNode)
                {
                    _IndexAttribute = (myParseTreeNode.ChildNodes[0].ChildNodes[0].AstNode as IDNode).IDChainDefinition;
                    ParsingResult.PushIExceptional((myParseTreeNode.ChildNodes[0].ChildNodes[0].AstNode as IDNode).ParsingResult);
                }
                else
                {
                    ParsingResult.PushIError(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), myParseTreeNode.ChildNodes[0].ChildNodes[0].AstNode .GetType().ToString()));
                }

            }

            if (myParseTreeNode.ChildNodes.Count > 1 && myParseTreeNode.ChildNodes[1].HasChildNodes())
            {
                _OrderDirection = myParseTreeNode.ChildNodes[1].FirstChild.Token.ValueString;
            }
            else
            {
                _OrderDirection = String.Empty;
            }
            #region index attribute validation

            if (_IndexAttribute.Count() > 2)
            {
                ParsingResult.PushIError(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Only one attribute and one optional function are allowed! e.g. Name.TOUPPER()"));
            }

            #endregion

            IndexAttributeDefinition = new IndexAttributeDefinition(_IndexAttribute, _IndexType, _OrderDirection);

        }

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
