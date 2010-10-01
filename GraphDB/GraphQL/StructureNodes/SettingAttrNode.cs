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

/* <id name="GraphDB – ASettingAttrNode" />
 * <copyright file="ASettingAttrNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class SettingAttrNode : AStructureNode
    {

        /// <summary>
        /// TypeName,Attribute
        /// </summary>
        public Dictionary<String, List<IDChainDefinition>> Attributes { get; private set; }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            Attributes = new Dictionary<String, List<IDChainDefinition>>();

            if (parseNode == null)
                return;

            if (parseNode.ChildNodes[1].HasChildNodes())
            {
                foreach (var Node in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (Node.HasChildNodes())
                    {

                        if (!Attributes.ContainsKey((Node.ChildNodes[0].AstNode as ATypeNode).ReferenceAndType.TypeName))
                        {
                            Attributes.Add((Node.ChildNodes[0].AstNode as ATypeNode).ReferenceAndType.TypeName, new List<IDChainDefinition>());
                        }
                        Attributes[(Node.ChildNodes[0].AstNode as ATypeNode).ReferenceAndType.TypeName].Add((Node.ChildNodes[2].AstNode as IDNode).IDChainDefinition);

                    }
                }
            }

        }

    }

}
