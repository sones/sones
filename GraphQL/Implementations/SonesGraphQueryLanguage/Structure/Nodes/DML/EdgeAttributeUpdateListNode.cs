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
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of an AttrUpdateOrAssignListNode Node.
    /// </summary>
    public sealed class EdgeAttributeUpdateListNode : AStructureNode, IAstNodeInit
    {
        #region Properties

        public HashSet<AAttributeAssignOrUpdateOrRemove> ListOfUpdate { get; private set; }

        #endregion

        #region constructor

        public EdgeAttributeUpdateListNode()
        {
            ListOfUpdate = new HashSet<AAttributeAssignOrUpdateOrRemove>();
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            foreach (ParseTreeNode aChild in parseNode.ChildNodes)
            {
                if (aChild.AstNode is EdgeAttributeUpdateNode)
                {

                    #region attribute assign

                    EdgeAttributeUpdateNode aAttributeAssignNode = (EdgeAttributeUpdateNode)aChild.AstNode;
                    ListOfUpdate.Add((aChild.AstNode as EdgeAttributeUpdateNode).AttributeValue);

                    #endregion

                }
                else
                {
                    throw new NotImplementedQLException("Invalid task node \"" + aChild.AstNode.GetType().Name + "\" in update statement");
                }
            }
        }

        #endregion
    }
}
