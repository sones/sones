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

/* <id name="PandoraDB – ASettingScopeNode" />
 * <copyright file="ASettingScopeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class SettingScopeNode : AStructureNode, IAstNodeInit
    {

        #region Data

        TypesSettingScope _Scope;
        public TypesSettingScope Scope
        {
            get { return _Scope; }
        }

        /// <summary>
        /// This is either a SettingAttrNode or a SettingTypeNode
        /// </summary>
        AStructureNode _SettingNode;
        public AStructureNode SettingNode
        {
            get { return _SettingNode; }
        }

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
                    _Scope = TypesSettingScope.ATTRIBUTE;
                    _SettingNode = parseNode.ChildNodes[0].AstNode as AStructureNode;
                }
                else if (parseNode.ChildNodes[0].AstNode is SettingTypeNode)
                {
                    _Scope = TypesSettingScope.TYPE;
                    _SettingNode = parseNode.ChildNodes[0].AstNode as AStructureNode;
                }
            }
            else if (parseNode.ChildNodes[0] != null)
            {
                switch (parseNode.ChildNodes[0].Term.Name.ToUpper())
                { 
                    case "DB":
                        _Scope = TypesSettingScope.DB;
                    break;
                    case "SESSION":
                        _Scope = TypesSettingScope.SESSION;
                    break;

                    default:
                        _Scope = TypesSettingScope.DB;
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
