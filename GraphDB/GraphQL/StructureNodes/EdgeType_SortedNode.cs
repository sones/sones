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


#region Usings

using System;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.Lib.DataStructures;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class EdgeType_SortedNode : AStructureNode, IAstNodeInit
    {

        public Boolean IsSorted
        {
            get; private set;
        }

        public SortDirection SortDirection
        {
            get; private set;
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (!parseNode.HasChildNodes())
                return;

            IsSorted = true;
            if (parseNode.ChildNodes.Count == 3 && parseNode.ChildNodes[2].Token.Text.ToUpper() == GraphQL.GraphQueryLanguage.TERMINAL_DESC)
                SortDirection = SortDirection.Desc;
            else
                SortDirection = SortDirection.Asc;
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    
    }

}
