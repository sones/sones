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

/* <id name="GraphDB – BulkType Node" />
 * <copyright file="BulkTypeNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary>This node is requested in case of an BulkType node.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class BulkTypeNode : AStructureNode
    {

        #region Data

        private String _TypeName = ""; //the name of the type that should be created
        private String _Extends = ""; //the name of the type that should be extended
        private String _Comment = ""; //the name of the type that should be extended
        private Dictionary<AttributeDefinition, String> _Attributes = new Dictionary<AttributeDefinition, String>(); //the dictionayry of attribute definitions
        private List<BackwardEdgeDefinition> _BackwardEdgeInformation;
        private List<IndexDefinition> _Indices;    

        #endregion

        #region Accessessors

        public String TypeName { get { return _TypeName; } }
        public String Extends { get { return _Extends; } }
        public String Comment { get { return _Comment; } }
        public Dictionary<AttributeDefinition, String> Attributes { get { return _Attributes; } }
        public List<BackwardEdgeDefinition> BackwardEdges { get { return _BackwardEdgeInformation; } }
        public List<IndexDefinition> Indices { get { return _Indices; } }

        #endregion

        #region constructor

        public BulkTypeNode()
        { }

        #endregion

        public Exceptional GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var retExceptional = new Exceptional();

            #region get Name

            _TypeName = parseNode.ChildNodes[0].Token.ValueString;

            #endregion

            #region get Extends

            if (parseNode.ChildNodes[1].HasChildNodes())
            {
                _Extends = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString;
            }
            else
            {
                //if there is no extend a Type is alwayse inheritated by GraphObject
                _Extends = DBReference.Name;
            }

            #endregion

            #region get myAttributes

            if (parseNode.ChildNodes[2].HasChildNodes())
            {
                _Attributes = GetAttributeList(parseNode.ChildNodes[2].ChildNodes[1]);
            }

            #endregion

            #region get BackwardEdges

            if (parseNode.ChildNodes[3].HasChildNodes())
            {
                _BackwardEdgeInformation = (((BackwardEdgesNode)parseNode.ChildNodes[3].AstNode).BackwardEdgeInformation);
            }

            #endregion


            #region get Optional Unique

            if (((UniqueAttributesOptNode)parseNode.ChildNodes[4].AstNode).UniqueAttributes != null)
            {
                foreach (String uniqueAttr in ((UniqueAttributesOptNode)parseNode.ChildNodes[4].AstNode).UniqueAttributes)
                {
                    if (_Attributes.Any(a => a.Key.AttributeName == uniqueAttr))
                    {
                        var attr = (from a in _Attributes where a.Key.AttributeName == uniqueAttr select a).First();
                        attr.Key.AttributeType.TypeCharacteristics.IsUnique = true;
                    }
                    else
                    {
                        throw new GraphDBException(new Error_AttributeIsNotDefined(uniqueAttr));
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
                        throw new GraphDBException(new Error_AttributeIsNotDefined(mandAttr));
                    }
                }
            }

            #endregion

            #region Get Optional Indices

            if(parseNode.ChildNodes[6].HasChildNodes())
            {
                if (parseNode.ChildNodes[6].ChildNodes[0].HasChildNodes())
                {
                    var idxCreateNode = (IndexOnCreateTypeNode)parseNode.ChildNodes[6].ChildNodes[0].AstNode;
                    ParsingResult.Push(idxCreateNode.ParsingResult);

                    _Indices = new List<IndexDefinition>();
                    
                    foreach(var idx in idxCreateNode.ListOfIndexDefinitions)
                    {
                        _Indices.Add(idx);
                    }
                }
            }

            #endregion

            #region get Comment

            if (parseNode.ChildNodes[7].HasChildNodes())
            {
                _Comment = parseNode.ChildNodes[7].ChildNodes[2].Token.ValueString;
            }

            #endregion

            return retExceptional;

        }


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
                AttributeDefinitionNode aAttrDef = (AttributeDefinitionNode)aAttrDefNode.AstNode;
                if (aAttrDef.AttributeDefinition.DefaultValue != null)
                {
                    if (aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics == null)
                    {
                        aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics = new TypeCharacteristics();
                    }
                    aAttrDef.AttributeDefinition.AttributeType.TypeCharacteristics.IsMandatory = true;
                }

                if (attributes.Exists(item => item.Key.AttributeName == aAttrDef.AttributeDefinition.AttributeName))
                    throw new GraphDBException(new Error_AttributeAlreadyExists(aAttrDef.AttributeDefinition.AttributeName));
                else
                    attributes.Add(aAttrDef.AttributeDefinition, aAttrDef.AttributeDefinition.AttributeType.Name);
            }

            return attributes;
        }

        #endregion

    }

}
