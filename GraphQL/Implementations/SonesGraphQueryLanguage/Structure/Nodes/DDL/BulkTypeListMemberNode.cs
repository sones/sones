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

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is requested in case of an BulkTypeListMember node.
    /// </summary>
    public sealed class BulkTypeListMemberNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String  _TypeName = ""; //the name of the type that should be created
        private String _Extends = ""; //the name of the type that should be extended
        private String _Comment = ""; //the name of the type that should be extended
        private Boolean _IsAbstract = false;
        private Dictionary<AttributeDefinition, String> _Attributes = new Dictionary<AttributeDefinition, String>(); //the dictionayry of attribute definitions
        private List<IncomingEdgeDefinition> _BackwardEdgeInformation;
        private List<IndexDefinition> _Indices;

        #endregion

        #region Accessessors

        public String TypeName { get { return _TypeName; } }
        public String Extends { get { return _Extends; } }
        public String Comment { get { return _Comment; } }
        public Boolean IsAbstract { get { return _IsAbstract; } }
        public Dictionary<AttributeDefinition, String> Attributes { get { return _Attributes; } }
        public List<IncomingEdgeDefinition> BackwardEdges { get { return _BackwardEdgeInformation; } }
        public List<IndexDefinition> Indices { get { return _Indices; } }

        #endregion

        #region constructor

        public BulkTypeListMemberNode()
        {
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Abstract

            if (HasChildNodes(parseNode.ChildNodes[0]))
                _IsAbstract = true;

            #endregion

            var bulkTypeNode = (BulkTypeNode)parseNode.ChildNodes[1].AstNode;

            #region get Name

            _TypeName = bulkTypeNode.TypeName;

            #endregion

            #region get Extends

            _Extends = bulkTypeNode.Extends;

            #endregion

            #region get myAttributes

            _Attributes = bulkTypeNode.Attributes;

            #endregion

            #region get BackwardEdges

            _BackwardEdgeInformation = bulkTypeNode.BackwardEdges;

            #endregion

            #region Get Optional Indices

            _Indices = bulkTypeNode.Indices;

            #endregion

            #region get Comment

            _Comment = bulkTypeNode.Comment;

            #endregion
        }

        #endregion
    }
}
