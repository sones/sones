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
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class IndexDropOnAlterType : AStructureNode
    {

        #region Data

        private Dictionary<String, String> _DropIndexList;
        
        #endregion

        #region constructors

        public IndexDropOnAlterType()
        { }
        
        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _DropIndexList = new Dictionary<String, String>();


            if (parseNode.ChildNodes[1].ChildNodes.Count == 2 && parseNode.ChildNodes[1].ChildNodes.TrueForAll(item => !item.HasChildNodes()))
            {
                var idxName = parseNode.ChildNodes[1].ChildNodes[0].Token.Text;
                var idxEdition = ((EditionOptNode)parseNode.ChildNodes[1].ChildNodes[1].AstNode).IndexEdition;

                if (!_DropIndexList.ContainsKey(idxName))
                {
                    _DropIndexList.Add(idxName, idxEdition);
                }
            }
            else
            {
                foreach (var nodes in parseNode.ChildNodes[1].ChildNodes)
                {
                    var idxName = nodes.ChildNodes[0].Token.Text;
                    var idxEdition = ((EditionOptNode)nodes.ChildNodes[1].AstNode).IndexEdition;

                    if (!_DropIndexList.ContainsKey(idxName))
                    {
                        _DropIndexList.Add(idxName, idxEdition);
                    }
                }
            }

        }

        #region Accessors

        public Dictionary<String, String> DropIndexList
        {
            get { return _DropIndexList; }
        }

        #endregion

    }

}
