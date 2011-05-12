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
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using System.Collections.Generic;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.Misc
{
    public sealed class VertexTypeListNode : AStructureNode, IAstNodeInit
    {
        #region Properties

        public List<TypeReferenceDefinition> Types { get; private set; }

        #endregion

        #region Constructor

        public VertexTypeListNode()
        {
            Types = new List<TypeReferenceDefinition>();
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                foreach (var child in parseNode.ChildNodes)
                {
                    if (child.AstNode is ATypeNode)
                    {
                        var tr = (child.AstNode as ATypeNode).ReferenceAndType;
                        if (!Types.Contains(tr))
                        {
                            Types.Add(tr);
                        }
                        else
                        {
                            throw new DuplicateReferenceOccurrenceException(tr.TypeName);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
