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

/* <id name="PandoraDB – CreateIndexAttributeList Node" />
 * <copyright file="CreateIndexAttributeListNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary>This node is requested in case of an CreateIndexAttributeList node.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeList node.
    /// </summary>
    public class IndexAttributeListNode : AStructureNode, IAstNodeInit
    {

        #region properties

        private List<IndexAttributeNode> _IndexAttributes = null;

        #endregion

        #region constructor

        public IndexAttributeListNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region Data

            _IndexAttributes = new List<IndexAttributeNode>();

            foreach(ParseTreeNode aNode in parseNode.ChildNodes)
            {
                _IndexAttributes.Add((IndexAttributeNode)aNode.AstNode);
            }
            
            #endregion

        }

        #region Accessessors

        public List<IndexAttributeNode> IndexAttributes { get { return _IndexAttributes; } }

        #endregion

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            String _returnValue = String.Empty;

            for (int i=0;i<_IndexAttributes.Count-1;i++)
                _returnValue = _returnValue + _IndexAttributes[i].ToString() + ", ";

            return _returnValue + _IndexAttributes[_IndexAttributes.Count-1].ToString();

        }

        #endregion

    }//class
}//namespace
