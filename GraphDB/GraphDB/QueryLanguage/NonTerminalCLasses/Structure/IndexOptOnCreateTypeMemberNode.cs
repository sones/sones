/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.Objects;
using sones.Lib.ErrorHandling;
using sones.Lib;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class IndexOptOnCreateTypeMemberNode : AStructureNode
    {

        #region Properties

        private String _IndexName;
        public String IndexName
        {
            get { return _IndexName; }
        }

        private String _Edition;
        public String Edition
        {
            get { return _Edition; }
        }

        private String _IndexType;
        public String IndexType
        {
            get { return _IndexType; }
        }

        private List<IndexAttributeNode> _IndexAttributeNames;
        public List<IndexAttributeNode> IndexAttributeNames
        {
            get { return _IndexAttributeNames; }
            set { _IndexAttributeNames = value; }
        }

        #endregion

        public Exceptional GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var grammar = GetGraphQLGrammar(context);

            if (parseNode.ChildNodes.Count < 1)
                throw new ArgumentException("No index definitions found!");

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
                        _IndexAttributeNames = (child.AstNode as IndexAttributeListNode).IndexAttributes;
                    }
                }
            }


            #region Validation

            if (_IndexAttributeNames == null || _IndexAttributeNames.Count == 0)
            {
                throw new ArgumentException("No attributes given for index!");
            }

            if (String.IsNullOrEmpty(_IndexName))
            {
                _IndexName = _IndexAttributeNames.Aggregate(new StringBuilder(DBConstants.IndexKeyPrefix), (result, elem) => { result.Append(String.Concat(DBConstants.IndexKeySeperator, elem.IndexAttribute)); return result; }).ToString();
            }

            #endregion

            #region Check for obsolete GQL parts and return warning

            // only for a detailed definition
            if (parseNode.ChildNodes.Count > 3 && (parseNode.ChildNodes[4].Token == null || parseNode.ChildNodes[4].Token.AsSymbol != grammar.S_ATTRIBUTES))
            {
                return new Exceptional(new Warnings.Warning_ObsoleteGQL(
                    String.Format("{0} {1}", grammar.S_ON.ToUpperString(), _IndexAttributeNames.ToContentString()),
                    String.Format("{0} {1} {2}", grammar.S_ON.ToUpperString(), grammar.S_ATTRIBUTES.ToUpperString(), _IndexAttributeNames.ToContentString())));
            }

            #endregion

            return Exceptional.OK;

        }


        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }
}
