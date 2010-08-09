/* <id name="GraphDB – ASettingTypeNode" />
 * <copyright file="ASettingTypeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class SettingTypeNode : AStructureNode
    {

        #region Properties

        public List<String> Types { get; private set; }

        #endregion

        #region Constructor

        public SettingTypeNode()
        {
            Types = new List<string>();
        }

        #endregion

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode == null)
                return;

            if (myParseTreeNode.HasChildNodes() && myParseTreeNode.ChildNodes[1].HasChildNodes())
            {
                foreach (var Node in myParseTreeNode.ChildNodes[1].ChildNodes)
                {

                    if (!Types.Contains((Node.AstNode as ATypeNode).ReferenceAndType.TypeName))
                    {
                        Types.Add((Node.AstNode as ATypeNode).ReferenceAndType.TypeName);
                    }

                }
            }

        }

        #endregion

    }

}
