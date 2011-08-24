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
    public sealed class BulkVertexTypeNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String _TypeName = ""; //the name of the type that should be created
        private String _Extends = ""; //the name of the type that should be extended
        private String _Comment = ""; //the name of the type that should be extended
        private Dictionary<AttributeDefinition, String> _Attributes = new Dictionary<AttributeDefinition, String>(); //the dictionayry of attribute definitions
        private List<IncomingEdgeDefinition> _BackwardEdgeInformation;
        private List<IndexDefinition> _Indices;

        #endregion

        #region Accessessors

        public String TypeName { get { return _TypeName; } }
        public String Extends { get { return _Extends; } }
        public String Comment { get { return _Comment; } }
        public Dictionary<AttributeDefinition, String> Attributes { get { return _Attributes; } }
        public List<IncomingEdgeDefinition> BackwardEdges { get { return _BackwardEdgeInformation; } }
        public List<IndexDefinition> Indices { get { return _Indices; } }

        #endregion

        #region constructor

        public BulkVertexTypeNode()
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
            {
                _Extends = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString;
            }

            #endregion

            #region get myAttributes

            if (HasChildNodes(parseNode.ChildNodes[2]))
            {
                _Attributes = GetAttributeList(parseNode.ChildNodes[2].ChildNodes[1]);
            }

            #endregion

            #region get BackwardEdges

            if (HasChildNodes(parseNode.ChildNodes[3]))
            {
                _BackwardEdgeInformation = (((IncomingEdgesNode)parseNode.ChildNodes[3].AstNode).BackwardEdgeInformation);
            }

            #endregion


            #region get Optional Unique

            if (((UniqueAttributesOptNode)parseNode.ChildNodes[4].AstNode).UniqueAttributes != null)
            {
                foreach (String uniqueAttr in ((UniqueAttributesOptNode)parseNode.ChildNodes[4].AstNode).UniqueAttributes)
                {
                    var attr = (from a in _Attributes where a.Key.AttributeName == uniqueAttr select a).FirstOrDefault();
                    if (attr.Key != null)
                    {
                        attr.Key.AttributeType.TypeCharacteristics.IsUnique = true;                        
                    }
                    else
                    {
                        throw new VertexAttributeIsNotDefinedException(uniqueAttr);
                    }
                }
            }

            #endregion

            #region get Optional Mandatory

            if (((MandatoryOptNode)parseNode.ChildNodes[5].AstNode).MandatoryAttribs != null)
            {
                foreach (String mandAttr in ((MandatoryOptNode)parseNode.ChildNodes[5].AstNode).MandatoryAttribs)
                {
                    if (_Attributes.Any(a => a.Key.AttributeName == mandAttr))
                    {
                        var attr = (from a in _Attributes where a.Key.AttributeName == mandAttr select a).First();
                        attr.Key.AttributeType.TypeCharacteristics.IsMandatory = true;
                    }
                    else
                    {
                        throw new VertexAttributeIsNotDefinedException(mandAttr);
                    }
                }
            }

            #endregion

            #region Get Optional Indices

            if (HasChildNodes(parseNode.ChildNodes[6]))
            {
                if (HasChildNodes(parseNode.ChildNodes[6].ChildNodes[0]))
                {
                    var idxCreateNode = (IndexOnCreateTypeNode)parseNode.ChildNodes[6].ChildNodes[0].AstNode;

                    _Indices = new List<IndexDefinition>();

                    foreach (var idx in idxCreateNode.ListOfIndexDefinitions)
                    {
                        _Indices.Add(idx);
                    }
                }
            }

            #endregion

            #region get Comment

            if (HasChildNodes(parseNode.ChildNodes[7]))
            {
                _Comment = parseNode.ChildNodes[7].ChildNodes[2].Token.ValueString;
            }

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
                VertexTypeAttributeDefinitionNode aAttrDef = (VertexTypeAttributeDefinitionNode)aAttrDefNode.AstNode;
                if (aAttrDef.AttributeDefinition.DefaultValue != null)
                {
                    if (aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics == null)
                    {
                        aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics = new TypeCharacteristics();
                    }
                    aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics.IsMandatory = true;
                }

                if (attributes.Any(item => item.Key.AttributeName == aAttrDef.AttributeDefinition.AttributeName))
                    throw new VertexAttributeAlreadyExistsException(aAttrDef.AttributeDefinition.AttributeName);
                else
                    attributes.Add(aAttrDef.AttributeDefinition, aAttrDef.AttributeDefinition.AttributeType.Name);
            }

            return attributes;
        }

        #endregion
    }
}
