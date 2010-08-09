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
                        param = GraphDBTypeMapper.GetPandoraObjectFromTypeName(parseNode.ChildNodes[0].Token.ValueString);
                        type = ParamType.PandoraType;
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
