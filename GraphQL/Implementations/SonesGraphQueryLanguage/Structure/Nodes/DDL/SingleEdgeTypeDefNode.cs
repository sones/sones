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
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is the edgeType definition. A simple EdgyTypeList, BackwardEdge (make an implicit backward edge visible to the user)
    /// or a own defined edge derived from AListEdgeType, AListBaseEdgeType, ASingleEdgeType
    /// </summary>
    public sealed class SingleEdgeTypeDefNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public String Type { get { return _Type; } }
        String _Type;

        public String Name { get { return _Name; } }
        String _Name = null;

        /// <summary>
        /// The characteristics of an edge (Backward, Mandatory, Unique - valid for a combination of typeattributes
        /// </summary>
        public TypeCharacteristics TypeCharacteristics { get { return _TypeCharacteristics; } }
        TypeCharacteristics _TypeCharacteristics = null;

        /// <summary>
        /// An edge type definition - resolved by the name and found in the typeManager EdgeTypesLookup table
        /// </summary>
        public String EdgeType { get; private set; }

        public EdgeTypeParamDefinition[] Parameters { get; private set; }

        #endregion

        #region Constructor

        public SingleEdgeTypeDefNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region TypeEdge class

            //if (!dbContext.DBPluginManager.HasEdgeType(myParseTreeNode.ChildNodes[0].Token.ValueString))
            //    throw new GraphDBException(new Error_EdgeTypeDoesNotExist(myParseTreeNode.ChildNodes[0].Token.ValueString));

            //_EdgeType = dbContext.DBPluginManager.GetEdgeType(myParseTreeNode.ChildNodes[0].Token.ValueString);

            EdgeType = parseNode.ChildNodes[0].Token.ValueString;

            if (parseNode.ChildNodes[1].AstNode != null)
            {
                Parameters = ((EdgeTypeParamsNode)parseNode.ChildNodes[1].AstNode).Parameters;
            }

            #endregion

            _Name = parseNode.ChildNodes[3].Token.ValueString;

            _Type = SonesGQLConstants.SingleType;
        }

        #endregion
    }
}
