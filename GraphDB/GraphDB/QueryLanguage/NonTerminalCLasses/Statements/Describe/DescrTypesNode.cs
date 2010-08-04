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

/* <id name="sones GraphDB – DescrTypesNode" />
 * <copyright file="DescrTypesNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Result;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Structures.Describe;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{

    public class DescribeTypesNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeTypeDefinition; }
        }
        private DescribeTypeDefinition _DescribeTypeDefinition;

        #endregion

        #region AStructureNode

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            _DescribeTypeDefinition = new DescribeTypeDefinition();
        }       

        #endregion

    }

}
