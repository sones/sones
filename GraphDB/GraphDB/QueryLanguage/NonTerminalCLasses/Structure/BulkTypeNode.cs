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


/* <id name="sones GraphDB – BulkType Node" />
 * <copyright file="BulkTypeNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary>This node is requested in case of an BulkType node.</summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.Lib;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class BulkTypeNode : AStructureNode
    {
        #region Data

        private String _TypeName = ""; //the name of the type that should be created
        private String _Extends = ""; //the name of the type that should be extended
        private String _Comment = ""; //the name of the type that should be extended
        private Dictionary<TypeAttribute, String> _Attributes = new Dictionary<TypeAttribute, String>(); //the dictionayry of attribute definitions
        private List<BackwardEdgeNode> _BackwardEdgeInformation;
        private List<Exceptional<IndexOptOnCreateTypeMemberNode>> _Indices;    

        #endregion


        #region constructor

        public BulkTypeNode()
        { }

        #endregion

        public Exceptional GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var retExceptional = new Exceptional();

            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

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
                //if there is no extend a Type is alwayse inheritated by PandoraObject
                _Extends = DBReference.Name;
            }

            #endregion

            #region get myAttributes

            if (parseNode.ChildNodes[2].HasChildNodes())
            {
                _Attributes = GetAttributeList(parseNode.ChildNodes[2].ChildNodes[1], dbContext);
            }

            #endregion

            #region get BackwardEdges

            if (parseNode.ChildNodes[3].HasChildNodes())
            {
                _BackwardEdgeInformation = ((BackwardEdgesNode)parseNode.ChildNodes[3].AstNode).BackwardEdgeInformation;
            }

            #endregion


            #region get Optional Unique

            if (((UniqueAttributesOptNode)parseNode.ChildNodes[4].AstNode).UniqueAttributes != null)
            {
                foreach (String uniqueAttr in ((UniqueAttributesOptNode)parseNode.ChildNodes[4].AstNode).UniqueAttributes)
                {
                    if (_Attributes.Any(a => a.Key.Name == uniqueAttr))
                    {
                        var attr = (from a in _Attributes where a.Key.Name == uniqueAttr select a).First();
                        attr.Key.TypeCharacteristics.IsUnique = true;
                    }
                    else
                    {
                        throw new GraphDBException(new Error_AttributeDoesNotExists(uniqueAttr));
                    }
                }
            }

            #endregion

            #region get Optional Mandatory

            if (((MandatoryOptNode)parseNode.ChildNodes[5].AstNode).MandatoryAttribs != null)
            {
                foreach (String mandAttr in ((MandatoryOptNode)parseNode.ChildNodes[5].AstNode).MandatoryAttribs)
                {
                    if (_Attributes.Any(a => a.Key.Name == mandAttr))
                    {
                        var attr = (from a in _Attributes where a.Key.Name == mandAttr select a).First();
                        attr.Key.TypeCharacteristics.IsMandatory = true;
                    }
                    else
                    {
                        throw new GraphDBException(new Error_AttributeDoesNotExists(mandAttr));
                    }
                }
            }

            #endregion

            #region Get Optional Indices

            if (parseNode.ChildNodes[6].HasChildNodes() && parseNode.ChildNodes[6].ChildNodes[1].HasChildNodes())
            {
                if (parseNode.ChildNodes[6].ChildNodes[1].AstNode is Exceptional<IndexOptOnCreateTypeMemberNode>)
                {
                    #region data
                    _Indices = new List<Exceptional<IndexOptOnCreateTypeMemberNode>>();
                    var aIDX = (Exceptional<IndexOptOnCreateTypeMemberNode>)parseNode.ChildNodes[6].ChildNodes[1].AstNode;
                    if (aIDX.Failed)
                    {
                        return aIDX;
                    }
                    if (!aIDX.Success)
                    {
                        retExceptional.AddErrorsAndWarnings(aIDX);
                    }
                    #endregion

                    foreach (var aAttrInIdx in aIDX.Value.IndexAttributeNames)
                    {
                        #region check attributes of idx

                        bool IsValidIDXAttr = false;
                        foreach (var aAttr in _Attributes)
                        {
                            if (aAttr.Key.Name == aAttrInIdx.IndexAttribute)
                            {
                                IsValidIDXAttr = true;
                                break;
                            }
                        }

                        #region check supertype

                        if (!IsValidIDXAttr)
                        {
                            if (!_Extends.IsNullOrEmpty())
                            {
                                var extendsType = typeManager.GetTypeByName(_Extends);
                                if (extendsType != null)
                                {
                                    if (extendsType.GetTypeAttributeByName(aAttrInIdx.IndexAttribute) != null)
                                    {
                                        IsValidIDXAttr = true;
                                    }
                                }
                            }
                        }

                        #endregion

                        if (!IsValidIDXAttr)
                        {
                            throw new GraphDBException(new Error_AttributeDoesNotExists(_TypeName, aAttrInIdx.IndexAttribute));
                        }

                        #endregion
                    }

                    _Indices.Add(parseNode.ChildNodes[6].ChildNodes[1].AstNode as Exceptional<IndexOptOnCreateTypeMemberNode>);
                }
                else
                {
                    _Indices = new List<Exceptional<IndexOptOnCreateTypeMemberNode>>(parseNode.ChildNodes[6].ChildNodes[1].ChildNodes.Select(child => (Exceptional<IndexOptOnCreateTypeMemberNode>)child.AstNode));
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

        #region Accessessors

        public String TypeName { get { return _TypeName; } }
        public String Extends { get { return _Extends; } }
        public String Comment { get { return _Comment; } }
        public Dictionary<TypeAttribute, String> Attributes { get { return _Attributes; } }
        public List<BackwardEdgeNode> BackwardEdges { get { return _BackwardEdgeInformation; } }
        public List<Exceptional<IndexOptOnCreateTypeMemberNode>> Indices { get { return _Indices; } }

        #endregion

        #region private helper methods

        /// <summary>
        /// Gets an attributeList from a ParseTreeNode.
        /// </summary>
        /// <param name="aChildNode">The interesting ParseTreeNode.</param>
        /// <param name="myTypeManager">the typemanager</param>
        /// <returns>A Dictionary with attribute definitions.</returns>
        private Dictionary<TypeAttribute, String> GetAttributeList(ParseTreeNode aChildNode, DBContext myTypeManager)
        {
            #region Data

            Dictionary<TypeAttribute, String> attributes = new Dictionary<TypeAttribute, String>();

            #endregion

            foreach (ParseTreeNode aAttrDefNode in aChildNode.ChildNodes)
            {
                AttributeDefinitionNode aAttrDef = (AttributeDefinitionNode)aAttrDefNode.AstNode;
                if (aAttrDef.TypeAttribute.DefaultValue != null)
                {
                    aAttrDef.TypeAttribute.TypeCharacteristics.IsMandatory = true;
                }

                if (attributes.Exists(item => item.Key.Name == aAttrDef.TypeAttribute.Name))
                    throw new GraphDBException(new Error_AttributeAlreadyExists(aAttrDef.TypeAttribute.Name));
                else
                    attributes.Add(aAttrDef.TypeAttribute, aAttrDef.Type);
            }

            return attributes;
        }

        #endregion

    }
}
