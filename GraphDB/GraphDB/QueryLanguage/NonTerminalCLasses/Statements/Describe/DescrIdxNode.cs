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


/* <id name="sones GraphDB – DescrIdxNode" />
 * <copyright file="DescrIdxNode.cs"
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
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Structures.Describe;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescribeIndexNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeIndexDefinition; }
        }
        private DescribeIndexDefinition _DescribeIndexDefinition;

        #endregion


        #region AStructureNode

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            string type = parseNode.ChildNodes[1].ChildNodes[0].Token.ValueString;
            string indexName = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString;
            string edition = null;
            if (parseNode.ChildNodes[2].HasChildNodes())
            {
                edition = parseNode.ChildNodes[2].ChildNodes[0].Token.ValueString;
            }

            _DescribeIndexDefinition = new DescribeIndexDefinition(type, indexName, edition);
            
        }       
        #endregion

    }
}
