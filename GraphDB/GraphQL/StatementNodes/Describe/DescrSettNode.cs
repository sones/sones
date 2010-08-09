/* <id name="DescrSettNode" />
 * <copyright file="DescrSettNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using sones.GraphDB.Managers.Structures.Describe;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class DescribeSettingNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeSettingDefinition; }
        }
        private ADescribeDefinition _DescribeSettingDefinition;

        #endregion

        #region AStructureNode

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {

                if (parseNode.ChildNodes[1].AstNode is DescribeSettItemNode)
                {
                    _DescribeSettingDefinition = ((DescribeSettItemNode)parseNode.ChildNodes[1].AstNode).DescribeDefinition;
                }

                if (parseNode.ChildNodes[1].AstNode is DescribeSettingsItemsNode)
                {
                    _DescribeSettingDefinition = ((DescribeSettingsItemsNode)parseNode.ChildNodes[1].AstNode).DescribeDefinition;
                }

            }

        }
     
        #endregion

    }

}
