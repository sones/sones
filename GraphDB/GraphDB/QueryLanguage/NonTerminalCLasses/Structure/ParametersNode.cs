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

/* <id name="PandoraDB – ParametersNode node" />
 * <copyright file="ParametersNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A list of parameters passed to a add method of a AListEdgeType.</summary>
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
    /// A list of parameters passed to a add method of a AListEdgeType.
    /// </summary>
    public class ParametersNode : AStructureNode, IAstNodeInit
    {

        private List<ADBBaseObject> _ParameterValues;
        public List<ADBBaseObject> ParameterValues
        {
            get { return _ParameterValues; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _ParameterValues = new List<ADBBaseObject>();
            if (parseNode.HasChildNodes() && parseNode.ChildNodes[1].HasChildNodes())
            {
                foreach (var child in parseNode.ChildNodes[1].ChildNodes)
                {
                    _ParameterValues.Add(GraphDBTypeMapper.GetPandoraObjectFromTypeName(child.Token.Terminal.GetType().Name, child.Token.Value));
                }
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
