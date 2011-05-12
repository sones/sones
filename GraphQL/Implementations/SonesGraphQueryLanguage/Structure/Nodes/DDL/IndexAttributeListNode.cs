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
using System.Collections.Generic;
using sones.Library.LanguageExtensions;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeList node.
    /// </summary>
    public sealed class IndexAttributeListNode : AStructureNode, IAstNodeInit
    {
        #region properties

        public List<IndexAttributeDefinition> IndexAttributes { get; private set; }

        #endregion

        #region constructor

        public IndexAttributeListNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region Data

            IndexAttributes = new List<IndexAttributeDefinition>();

            foreach (ParseTreeNode aNode in parseNode.ChildNodes)
            {
                if ((aNode.AstNode as IndexAttributeNode) != null)
                {
                    IndexAttributes.Add((aNode.AstNode as IndexAttributeNode).IndexAttributeDefinition);
                }
            }

            #endregion
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            return IndexAttributes.ToContentString();

        }

        #endregion
    }
}
