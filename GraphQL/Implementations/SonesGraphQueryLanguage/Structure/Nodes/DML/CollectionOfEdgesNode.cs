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
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of CollectionOfDBObjectsNode statement (SETOF or LISTOF).
    /// </summary>
    public sealed class CollectionOfEdgesNode : AStructureNode, IAstNodeInit
    {
        public CollectionDefinition CollectionDefinition { get; private set; }

        #region constructor

        public CollectionOfEdgesNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes[0].AstNode is TupleNode)
            {
                CollectionDefinition = 
                    new CollectionDefinition(CollectionType.Set, 
                                            ((TupleNode)parseNode.ChildNodes[0].AstNode).TupleDefinition);
            }
            else
            {
                CollectionDefinition = 
                    new CollectionDefinition(CollectionType.Set, 
                                            (EdgeTypeEdgeIDCollectionNode)parseNode.ChildNodes[0].AstNode);
            }
        }

        #endregion
    }
}
