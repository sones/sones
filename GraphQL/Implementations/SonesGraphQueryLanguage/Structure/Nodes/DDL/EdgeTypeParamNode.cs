/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
            //ParamType type = ParamType.Value;
            //Object param = null;

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
