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
using sones.GraphQL.Structure.Nodes.Misc;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is the edgeType definition. A simple EdgyTypeList, BackwardEdge (make an implicit backward edge visible to the user)
    /// or a own defined edge derived from AListEdgeType, AListBaseEdgeType, ASingleEdgeType
    /// </summary>
    public sealed class EdgeTypeDefNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public String Type { get; private set; }

        public String Name { get; private set; }

        /// <summary>
        /// The characteristics of an edge (Backward, Mandatory, Unique - valid for a combination of typeattributes
        /// </summary>
        public TypeCharacteristics TypeCharacteristics { get; private set; }

        /// <summary>
        /// An edge type definition - resolved by the name and found in the typeManager EdgeTypesLookup table
        /// </summary>
        public String EdgeType { get; private set; }

        #endregion

        #region constructor

        public EdgeTypeDefNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region TypeEdge class

            EdgeType = parseNode.ChildNodes[3].Token.ValueString;

            #endregion

            if (parseNode.FirstChild.Term is IdentifierTerminal)
            {
                #region simple id

                Type = SonesGQLConstants.SingleType;

                Name = parseNode.ChildNodes[0].Token.ValueString;

                #endregion
            }

            else if (parseNode.FirstChild.Term.Name.ToUpper().Equals(SonesGQLConstants.INCOMINGEDGE.ToUpper()))
            {
                Name = ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition.Reference.Item2.Name;
                TypeCharacteristics = new TypeCharacteristics();
                TypeCharacteristics.IsIncomingEdge = true;
            }

            else
            {

                Name = parseNode.ChildNodes[2].Token.ValueString;

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == SonesGQLGrammar.TERMINAL_SET)
                {
                    Type = SonesGQLGrammar.TERMINAL_SET;
                }

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == SonesGQLGrammar.TERMINAL_LIST)
                {
                    Type = SonesGQLGrammar.TERMINAL_LIST;
                }


            }

            if (this.TypeCharacteristics == null)
            {
                this.TypeCharacteristics = new TypeCharacteristics();
            }
        }

        #endregion
    }
}
