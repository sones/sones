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

/* <id name="GraphDB – CreateIndexAttributeList Node" />
 * <copyright file="CreateIndexAttributeListNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary>This node is requested in case of an CreateIndexAttributeList node.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;

using sones.Lib;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeList node.
    /// </summary>
    public class IndexAttributeListNode : AStructureNode, IAstNodeInit
    {

        #region properties

        public List<IndexAttributeDefinition> IndexAttributes { get; private set; }

        #endregion

        #region constructor

        public IndexAttributeListNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region Data

            IndexAttributes = new List<IndexAttributeDefinition>();

            foreach(ParseTreeNode aNode in parseNode.ChildNodes)
            {
                if ((aNode.AstNode as IndexAttributeNode) != null)
                {
                    IndexAttributes.Add((aNode.AstNode as IndexAttributeNode).IndexAttributeDefinition);
                }
            }
            
            #endregion

        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            return IndexAttributes.ToContentString();

        }

        #endregion

    }

}
