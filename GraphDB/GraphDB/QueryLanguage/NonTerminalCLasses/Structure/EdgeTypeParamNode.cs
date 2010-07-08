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

/* <id name="sones GraphDB – EdgeTypeParamNode" />
 * <copyright file="EdgeTypeParamNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This is one param of an EdgeTypeParamsNode</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    /// <summary>
    /// This is one param of an EdgeTypeParamsNode
    /// </summary>
    public class EdgeTypeParamNode : AStructureNode, IAstNodeInit
    {

        public enum ParamType
        {
            PandoraType,
            Value,
            DefaultValueDef,
            Sort
        }


        private Object _Param;
        public Object Param
        {
            get { return _Param; }
        }

        public ParamType Type { get; set; }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                if (parseNode.ChildNodes[0].AstNode is DefaultValueDefNode)
                {
                    _Param = (parseNode.ChildNodes[0].AstNode as DefaultValueDefNode).Value;
                    Type = ParamType.DefaultValueDef;
                }
                else if (parseNode.ChildNodes[0].AstNode is EdgeType_SortedNode)
                {
                    _Param = (parseNode.ChildNodes[0].AstNode as EdgeType_SortedNode).SortDirection;
                    Type = ParamType.Sort;
                }
                else
                {
                    if (GraphDBTypeMapper.IsBasicType(parseNode.ChildNodes[0].Token.ValueString))
                    {
                        _Param = GraphDBTypeMapper.GetPandoraObjectFromTypeName(parseNode.ChildNodes[0].Token.ValueString);
                        Type = ParamType.PandoraType;
                    }
                    else
                    {
                        _Param = parseNode.ChildNodes[0].Token.Value;
                        Type = ParamType.Value;
                    }
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
