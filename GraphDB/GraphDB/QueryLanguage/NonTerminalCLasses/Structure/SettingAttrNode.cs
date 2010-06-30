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

/* <id name="sones GraphDB – ASettingAttrNode" />
 * <copyright file="ASettingAttrNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class SettingAttrNode : AStructureNode
    {
        #region Data
        Dictionary<string, List<TypeAttribute>> _AttrList;
        #endregion

        #region constructor
        public SettingAttrNode()
        {
            _AttrList = new Dictionary<string, List<TypeAttribute>>();
        }
        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode == null)
                return;

            if (parseNode.ChildNodes[1].HasChildNodes())
            {
                foreach (var Node in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (Node.HasChildNodes())
                    {
                        var typeAttr = (Node.ChildNodes[2].AstNode as IDNode).LastAttribute;
                        var typeOfNode = (Node.ChildNodes[2].AstNode as IDNode).LastType;

                        if (!_AttrList.ContainsKey(typeOfNode.Name))
                            _AttrList.Add(typeOfNode.Name, new List<TypeAttribute>());

                        if (!_AttrList[typeOfNode.Name].Contains(typeAttr))
                            _AttrList[typeOfNode.Name].Add(typeAttr);

                    }
                }
            }
        }

        #region Accessor

        public Dictionary<string, List<TypeAttribute>> Attributes
        { get { return _AttrList; } }

        #endregion
    }
}
