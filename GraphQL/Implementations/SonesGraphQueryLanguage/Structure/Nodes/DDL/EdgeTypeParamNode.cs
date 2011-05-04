using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This is one param of an EdgeTypeParamsNode
    /// </summary>
    public sealed class EdgeTypeParamNode : AStructureNode, IAstNodeInit
    {
        public EdgeTypeParamDefinition EdgeTypeParamDefinition { get; set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            ParamType type = ParamType.Value;
            Object param = null;

            if (HasChildNodes(parseNode))
            {

                throw new NotImplementedQLException("TODO");

                ///Das sollte hier jetzt viel einfacher sein, weil die definition einer edge maximal so aussehen kann User(Weighted) oder Set<User (Weighted)>

                //    throw new NotImplementedQLException("TODO");

                //    if (GraphDBTypeMapper.IsBasicType(parseNode.ChildNodes[0].Token.ValueString))
                //    {
                //        param = GraphDBTypeMapper.GetGraphObjectFromTypeName(parseNode.ChildNodes[0].Token.ValueString);
                //        type = ParamType.Type;
                //    }

                //    else
                //    {
                //        param = parseNode.ChildNodes[0].Token.Value;
                //        type = ParamType.Value;
                //    }

                //}

                //EdgeTypeParamDefinition = new EdgeTypeParamDefinition(type, param);

            }
        }

        #endregion
    }
}
