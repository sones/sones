/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* <id name="sones GraphDB – DescrSettItemNode" />
 * <copyright file="DescrSettItemNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Settings;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Managers.Structures.Describe;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
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
