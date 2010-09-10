/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* <id name="GraphDB – ASettingScopeNode" />
 * <copyright file="ASettingScopeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <developer>Stefan Licht</developer>
 * <summary></summary>
 */

#region Usings

using sones.GraphDB.Managers.Structures.Setting;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class SettingScopeNode : AStructureNode, IAstNodeInit
    {

        #region Data

        public ASettingDefinition SettingDefinition { get; private set; }

        #endregion

        #region constructor

        public SettingScopeNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode == null)
                return;

            if (parseNode.ChildNodes[0].AstNode != null)
            {

                if (parseNode.ChildNodes[0].AstNode is SettingAttrNode)
                {
                    SettingDefinition = new SettingAttributeDefinition((parseNode.ChildNodes[0].AstNode as SettingAttrNode).Attributes);
                }

                else if (parseNode.ChildNodes[0].AstNode is SettingTypeNode)
                {
                    SettingDefinition = new SettingTypeDefinition((parseNode.ChildNodes[0].AstNode as SettingTypeNode).Types);
                }

            }

            else if (parseNode.ChildNodes[0] != null)
            {

                switch (parseNode.ChildNodes[0].Term.Name.ToUpper())
                { 
                    case "DB":
                        SettingDefinition = new SettingDBDefinition();
                    break;
                    case "SESSION":
                        SettingDefinition = new SettingSessionDefinition();
                    break;

                    default:
                        SettingDefinition = new SettingDBDefinition();
                    break;
                }

            }
            
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
