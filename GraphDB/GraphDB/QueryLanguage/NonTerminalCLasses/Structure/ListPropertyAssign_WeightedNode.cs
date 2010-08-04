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

/* <id name="sones GraphDB – ListPropertyAssign_WeightedNode" />
 * <copyright file="ListPropertyAssign_WeightedNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The weighted value.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    /// <summary>
    /// The weighted value
    /// </summary>
    public class ListPropertyAssign_WeightedNode : AStructureNode, IAstNodeInit
    {

        private Boolean _IsWeighted = false;
        public Boolean IsWeighted
        {
            get { return _IsWeighted; }
        }

        private DBNumber _WeightedValue = new DBUInt64(1UL);
        public DBNumber WeightedValue
        {
            get { return _WeightedValue; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (!parseNode.HasChildNodes())
                return;

            _IsWeighted = true;

            if (parseNode.ChildNodes[2].HasChildNodes())
            {
                String type = ((GraphDBTypeNode)parseNode.ChildNodes[2].AstNode).DBTypeDefinition.Name;
                _WeightedValue = (DBNumber)GraphDBTypeMapper.GetPandoraObjectFromTypeName(type); //Convert.ToDouble(parseNode.ChildNodes[2].Token.Value);
            }
            else if (parseNode.ChildNodes.Count == 3)
            {
                _WeightedValue.SetValue(parseNode.ChildNodes[2].Token.Value);
            }

            if (parseNode.ChildNodes.Count == 4)
            {
                _WeightedValue.SetValue(parseNode.ChildNodes[2].Token.Value);
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
