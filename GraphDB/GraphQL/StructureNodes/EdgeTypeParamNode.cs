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

/* <id name="GraphDB – EdgeTypeParamNode" />
 * <copyright file="EdgeTypeParamNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This is one param of an EdgeTypeParamsNode</summary>
 */

#region Usings

using System;

using sones.GraphDB.GraphQL.StructureNodes;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This is one param of an EdgeTypeParamsNode
    /// </summary>
    public class EdgeTypeParamNode : AStructureNode, IAstNodeInit
    {

        public EdgeTypeParamDefinition EdgeTypeParamDefinition { get; set; }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            ParamType type = ParamType.Value;
            Object param = null;

            if (parseNode.HasChildNodes())
            {

                if (parseNode.ChildNodes[0].AstNode is DefaultValueDefNode)
                {
                    param = (parseNode.ChildNodes[0].AstNode as DefaultValueDefNode).Value;
                    type = ParamType.DefaultValueDef;
                }

                else if (parseNode.ChildNodes[0].AstNode is EdgeType_SortedNode)
                {
                    param = (parseNode.ChildNodes[0].AstNode as EdgeType_SortedNode).SortDirection;
                    type = ParamType.Sort;
                }

                else
                {

                    if (GraphDBTypeMapper.IsBasicType(parseNode.ChildNodes[0].Token.ValueString))
                    {
                        param = GraphDBTypeMapper.GetGraphObjectFromTypeName(parseNode.ChildNodes[0].Token.ValueString);
                        type = ParamType.GraphType;
                    }
                    
                    else
                    {
                        param = parseNode.ChildNodes[0].Token.Value;
                        type = ParamType.Value;
                    }

                }

                EdgeTypeParamDefinition = new EdgeTypeParamDefinition(type, param);

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
