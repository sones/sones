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
    public sealed class AttributeUpdateOrAssignListNode : AStructureNode, IAstNodeInit
    {
        #region Properties

        public HashSet<AAttributeAssignOrUpdateOrRemove> ListOfUpdate { get; private set; }

        #endregion

        #region constructor

        public AttributeUpdateOrAssignListNode()
        {
            ListOfUpdate = new HashSet<AAttributeAssignOrUpdateOrRemove>();
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            foreach (ParseTreeNode aChild in parseNode.ChildNodes)
            {
                if (aChild.AstNode is AttributeAssignNode)
                {

                    #region attribute assign

                    AttributeAssignNode aAttributeAssignNode = (AttributeAssignNode)aChild.AstNode;
                    ListOfUpdate.Add((aChild.AstNode as AttributeAssignNode).AttributeValue);

                    #endregion

                }
                else
                {
                    if ((aChild.AstNode is AddToListAttrUpdateNode) || (aChild.AstNode is RemoveFromListAttrUpdateNode))
                    {

                        #region list update

                        if (aChild.AstNode is AddToListAttrUpdateNode)
                        {
                            ListOfUpdate.Add((aChild.AstNode as AddToListAttrUpdateNode).AttributeUpdateList);
                        }
                        #endregion

                        if (aChild.AstNode is RemoveFromListAttrUpdateNode)
                        {
                            ListOfUpdate.Add((aChild.AstNode as RemoveFromListAttrUpdateNode).AttributeRemoveList);
                        }
                    }
                    else
                    {
                        if (aChild.AstNode is AttributeRemoveNode)
                        {

                            #region remove attribute

                            ListOfUpdate.Add((aChild.AstNode as AttributeRemoveNode).AttributeRemove);

                            #endregion

                        }
                        else
                        {
                            throw new NotImplementedQLException("Invalid task node \"" + aChild.AstNode.GetType().Name + "\" in update statement");
                        }
                    }
                }
            }
        }

        #endregion
    }
}
