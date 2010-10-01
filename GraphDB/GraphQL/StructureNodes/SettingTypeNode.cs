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
