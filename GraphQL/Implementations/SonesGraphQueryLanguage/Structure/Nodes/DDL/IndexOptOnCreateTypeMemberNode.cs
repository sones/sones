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
    public sealed class IndexOptOnCreateTypeMemberNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String _IndexName;
        private String _Edition;
        private String _IndexType;
        private List<IndexAttributeDefinition> _IndexAttributeDefinitions { get; set; }

        #endregion

        #region Properties

        public IndexDefinition IndexDefinition { get; private set; }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes.Count < 1)
            {
                throw new ArgumentException("No index definitions found!");
            }

            foreach (var child in parseNode.ChildNodes)
            {

                if (child.AstNode != null)
                {

                    if (child.AstNode is IndexNameOptNode)
                    {
                        _IndexName = (child.AstNode as IndexNameOptNode).IndexName;
                    }

                    else if (child.AstNode is EditionOptNode)
                    {
                        _Edition = (child.AstNode as EditionOptNode).IndexEdition;
                    }

                    else if (child.AstNode is IndexTypeOptNode)
                    {
                        _IndexType = (child.AstNode as IndexTypeOptNode).IndexType;
                    }

                    else if (child.AstNode is IndexAttributeListNode)
                    {
                        _IndexAttributeDefinitions = (child.AstNode as IndexAttributeListNode).IndexAttributes;
                    }

                }

            }


            #region Validation

            if (_IndexAttributeDefinitions.IsNullOrEmpty())
            {
                throw new ArgumentException("No attributes given for index!");
            }

            #endregion

            IndexDefinition = new IndexDefinition(_IndexName, _Edition, _IndexType, _IndexAttributeDefinitions);
        }

        #endregion
    }
}
