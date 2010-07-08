/* <id name="PandoraDB – CreateIndexAttributeList Node" />
 * <copyright file="CreateIndexAttributeListNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary>This node is requested in case of an CreateIndexAttributeList node.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeList node.
    /// </summary>
    public class IndexAttributeListNode : AStructureNode, IAstNodeInit
    {

        #region properties

        private List<IndexAttributeNode> _IndexAttributes = null;

        #endregion

        #region constructor

        public IndexAttributeListNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region Data

            _IndexAttributes = new List<IndexAttributeNode>();

            foreach(ParseTreeNode aNode in parseNode.ChildNodes)
            {
                _IndexAttributes.Add((IndexAttributeNode)aNode.AstNode);
            }
            
            #endregion

        }

        #region Accessessors

        public List<IndexAttributeNode> IndexAttributes { get { return _IndexAttributes; } }

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

            String _returnValue = String.Empty;

            for (int i=0;i<_IndexAttributes.Count-1;i++)
                _returnValue = _returnValue + _IndexAttributes[i].ToString() + ", ";

            return _returnValue + _IndexAttributes[_IndexAttributes.Count-1].ToString();

        }

        #endregion

    }//class
}//namespace
