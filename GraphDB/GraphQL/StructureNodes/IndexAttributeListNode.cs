/* <id name="GraphDB – CreateIndexAttributeList Node" />
 * <copyright file="CreateIndexAttributeListNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary>This node is requested in case of an CreateIndexAttributeList node.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;

using sones.Lib;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeList node.
    /// </summary>
    public class IndexAttributeListNode : AStructureNode, IAstNodeInit
    {

        #region properties

        public List<IndexAttributeDefinition> IndexAttributes { get; private set; }

        #endregion

        #region constructor

        public IndexAttributeListNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region Data

            IndexAttributes = new List<IndexAttributeDefinition>();

            foreach(ParseTreeNode aNode in parseNode.ChildNodes)
            {
                if ((aNode.AstNode as IndexAttributeNode) != null)
                {
                    ParsingResult.PushIExceptional((aNode.AstNode as IndexAttributeNode).ParsingResult);
                    IndexAttributes.Add((aNode.AstNode as IndexAttributeNode).IndexAttributeDefinition);
                }
            }
            
            #endregion

        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
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
