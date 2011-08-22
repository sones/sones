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
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.ErrorHandling;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is requested in case of an BulkType node.
    /// </summary>
    public sealed class BulkEdgeTypeNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String _TypeName = ""; //the name of the type that should be created
        private String _Extends = ""; //the name of the type that should be extended
        private String _Comment = ""; //the name of the type that should be extended
        private Dictionary<AttributeDefinition, String> _Attributes = new Dictionary<AttributeDefinition, String>(); //the dictionayry of attribute definitions

        #endregion

        #region Accessessors

        public String TypeName { get { return _TypeName; } }
        public String Extends { get { return _Extends; } }
        public String Comment { get { return _Comment; } }
        public Dictionary<AttributeDefinition, String> Attributes { get { return _Attributes; } }

        #endregion

        #region constructor

        public BulkEdgeTypeNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Name

            _TypeName = parseNode.ChildNodes[0].Token.ValueString;

            #endregion

            #region get Extends

            if (HasChildNodes(parseNode.ChildNodes[1]))
                _Extends = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString;

            #endregion

            #region get myAttributes

            if (HasChildNodes(parseNode.ChildNodes[2]))
                _Attributes = GetAttributeList(parseNode.ChildNodes[2].ChildNodes[1]);

            #endregion

            #region get Comment

            if (HasChildNodes(parseNode.ChildNodes[3]))
                _Comment = parseNode.ChildNodes[3].ChildNodes[2].Token.ValueString;

            #endregion
        }

        #endregion

        #region private helper methods

        /// <summary>
        /// Gets an attributeList from a ParseTreeNode.
        /// </summary>
        /// <param name="aChildNode">The interesting ParseTreeNode.</param>
        /// <param name="myTypeManager">the typemanager</param>
        /// <returns>A Dictionary with attribute definitions.</returns>
        private Dictionary<AttributeDefinition, String> GetAttributeList(ParseTreeNode aChildNode)
        {
            #region Data

            var attributes = new Dictionary<AttributeDefinition, String>();

            #endregion

            foreach (ParseTreeNode aAttrDefNode in aChildNode.ChildNodes)
            {
                EdgeTypeAttributeDefinitionNode aAttrDef = (EdgeTypeAttributeDefinitionNode)aAttrDefNode.AstNode;
                
                if (aAttrDef.AttributeDefinition.DefaultValue != null)
                {
                    if (aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics == null)
                        aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics = new TypeCharacteristics();

                    aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics.IsMandatory = true;
                }

                if (attributes.Any(item => item.Key.AttributeName == aAttrDef.AttributeDefinition.AttributeName))
                    throw new AttributeAlreadyExistsException(aAttrDef.AttributeDefinition.AttributeName);
                else
                    attributes.Add(aAttrDef.AttributeDefinition, aAttrDef.AttributeDefinition.AttributeType.Name);
            }

            return attributes;
        }

        #endregion
    }
}
