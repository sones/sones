/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/


#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Errors;
using sones.GraphDB.GraphQL;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// Errors: Error_ArgumentException
    /// Warnings: Warning_ObsoleteGQL
    /// </summary>
    public class IndexOptOnCreateTypeMemberNode : AStructureNode
    {

        #region Data

        private String _IndexName;
        private String _Edition;
        private String _IndexType;
        private List<IndexAttributeDefinition> _IndexAttributeDefinitions { get; set; }
        private UInt16 _AttributeIdxShards;

        #endregion

        #region Properties

        public IndexDefinition IndexDefinition { get; private set; }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var grammar = GetGraphQLGrammar(context);

            if (parseNode.ChildNodes.Count < 1)
            {
                ParsingResult.PushIError(new Error_ArgumentException("No index definitions found!"));
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

                    else if (child.AstNode is ShardsNode)
                    {
                        _AttributeIdxShards = (child.AstNode as ShardsNode).Shards.HasValue ? (child.AstNode as ShardsNode).Shards.Value : DBConstants.AttributeIdxShards;
                    }

                }

            }


            #region Validation

            if (_IndexAttributeDefinitions.IsNullOrEmpty())
            {
                ParsingResult.PushIError(new Error_ArgumentException("No attributes given for index!"));
            }

            #endregion

            IndexDefinition = new IndexDefinition(_IndexName, _Edition, _IndexType, _IndexAttributeDefinitions, _AttributeIdxShards);

            #region Check for obsolete GQL parts and return warning

            // only for a detailed definition
            if (parseNode.ChildNodes.Count > 3 && (parseNode.ChildNodes[4].Token == null || parseNode.ChildNodes[4].Token.AsSymbol != grammar.S_ATTRIBUTES))
            {
                ParsingResult.PushIWarning(new Warnings.Warning_ObsoleteGQL(
                    String.Format("{0} {1}", grammar.S_ON.ToUpperString(), _IndexAttributeDefinitions.ToContentString()),
                    String.Format("{0} {1} {2}", grammar.S_ON.ToUpperString(), grammar.S_ATTRIBUTES.ToUpperString(), _IndexAttributeDefinitions.ToContentString())));
            }

            #endregion

        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
