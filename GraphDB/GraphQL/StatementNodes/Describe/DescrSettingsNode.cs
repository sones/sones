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

    public class DescribeSettingsNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeSettingDefinition; }
        }
        private DescribeSettingDefinition _DescribeSettingDefinition;

        #endregion
        
        #region AStructureNode

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            _DescribeSettingDefinition = new DescribeSettingDefinition();

        }

        #endregion

    }

}
