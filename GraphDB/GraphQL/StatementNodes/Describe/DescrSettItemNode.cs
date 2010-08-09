/* <id name="DescrSettItemNode" />
 * <copyright file="DescrSettItemNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using sones.GraphDB.Managers.Structures.Describe;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class DescribeSettItemNode : ADescrNode
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

            TypesSettingScope? settingType;
            if (parseNode.HasChildNodes() && (parseNode.ChildNodes.Count >= 3))
            {

                switch (parseNode.ChildNodes[2].Token.Text.ToUpper())
                {
                    case "TYPE":
                        settingType = TypesSettingScope.TYPE;
                        _DescribeSettingDefinition = new DescribeSettingDefinition(settingType, parseNode.ChildNodes[0].Token.ValueString.ToUpper(), (parseNode.ChildNodes[3].AstNode as ATypeNode).ReferenceAndType.TypeName);
                        break;
                    case "ATTRIBUTE":
                        settingType = TypesSettingScope.ATTRIBUTE;
                        _DescribeSettingDefinition = new DescribeSettingDefinition(settingType, parseNode.ChildNodes[0].Token.ValueString.ToUpper(), myIDChain: (parseNode.ChildNodes[3].ChildNodes[2].AstNode as IDNode).IDChainDefinition);
                        break;
                    case "DB":
                        settingType = TypesSettingScope.DB;
                        _DescribeSettingDefinition = new DescribeSettingDefinition(settingType, parseNode.ChildNodes[0].Token.ValueString.ToUpper());
                        break;
                    case "SESSION":
                        settingType = TypesSettingScope.SESSION;
                        _DescribeSettingDefinition = new DescribeSettingDefinition(settingType, parseNode.ChildNodes[0].Token.ValueString.ToUpper());
                        break;
                    default:
                        settingType = null;
                        _DescribeSettingDefinition = new DescribeSettingDefinition(settingType, parseNode.ChildNodes[0].Token.ValueString.ToUpper());
                        break;
                }

            }
            else
            {
                _DescribeSettingDefinition = new DescribeSettingDefinition(null, parseNode.ChildNodes[0].Token.ValueString.ToUpper());
            }
            
        }
        #endregion

    }

}
