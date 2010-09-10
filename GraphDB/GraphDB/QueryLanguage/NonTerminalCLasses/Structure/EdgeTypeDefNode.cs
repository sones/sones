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

/* <id name="PandoraDB – EdgeTypeDefNode" />
 * <copyright file="EdgeTypeDefNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This node is the edgeType definition. A simple EdgyTypeList, BackwardEdge (make an implicit backward edge visible to the user)
 *          or a own defined edge derived from AListEdgeType, AListBaseEdgeType, ASingleEdgeType
 * </summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Errors;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    /// <summary>
    /// This node is the edgeType definition. A simple EdgyTypeList, BackwardEdge (make an implicit backward edge visible to the user)
    /// or a own defined edge derived from AListEdgeType, AListBaseEdgeType, ASingleEdgeType
    /// </summary>
    public class EdgeTypeDefNode : AStructureNode
    {

        #region Data

        public TypesOfPandoraType Type { get { return _Type; } }
        TypesOfPandoraType _Type;

        public String Name { get { return _Name; } }
        String _Name = null;
        
        /// <summary>
        /// The characteristics of an edge (Backward, Mandatory, Unique - valid for a combination of typeattributes
        /// </summary>
        public TypeCharacteristics TypeCharacteristics { get { return _TypeCharacteristics; } }
        TypeCharacteristics _TypeCharacteristics = null;

        /// <summary>
        /// An edge type definition - resolved by the name and found in the typeManager EdgeTypesLookup table
        /// </summary>
        public AEdgeType EdgeType
        {
            get
            {
                return _EdgeType;
            }
        }
        AEdgeType _EdgeType;

        #endregion

        #region constructor

        public EdgeTypeDefNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            #region TypeEdge class

            if (!dbContext.DBPluginManager.HasEdgeType(parseNode.ChildNodes[2].Token.ValueString))
                throw new GraphDBException(new Error_EdgeTypeDoesNotExist(parseNode.ChildNodes[2].Token.ValueString));

            _EdgeType = dbContext.DBPluginManager.GetEdgeType(parseNode.ChildNodes[2].Token.ValueString);

            if (parseNode.ChildNodes[3].AstNode != null)
            {
                _EdgeType.ApplyParams(((EdgeTypeParamsNode)parseNode.ChildNodes[3].AstNode).Parameters);
            }

            #endregion

            if (parseNode.FirstChild.Term.Name.Equals("id_simple"))
            {
                #region simple id

                _Type = TypesOfPandoraType.Simple;

                _Name = parseNode.ChildNodes[0].Token.ValueString;

                #endregion
            }
            else if (parseNode.FirstChild.Term.Name.ToUpper().Equals(DBConstants.DBBackwardEdge.ToUpper()))
            {

                _Name = ((IDNode)parseNode.ChildNodes[2].AstNode).Reference.Item2.Name;
                _TypeCharacteristics = new TypeCharacteristics();
                _TypeCharacteristics.IsBackwardEdge = true;
            }
            else
            {
                _Name = parseNode.ChildNodes[5].Token.ValueString;

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                {

                    GraphDBType gType = typeManager.GetTypeByName(_Name);

                    if (gType != null)
                    {
                        if (gType.IsUserDefined)
                        {
                            _Type = TypesOfPandoraType.SetOfReferences;
                        }
                        else
                        {
                            _Type = TypesOfPandoraType.SetOfNoneReferences;
                        }
                    }
                    else
                        _Type = TypesOfPandoraType.SetOfReferences;
                }

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.LIST)
                    _Type = TypesOfPandoraType.ListOfNoneReferences;

                #region Verify edge against the type LIST/SET

                switch (_Type)
                {
                    case TypesOfPandoraType.ListOfNoneReferences:
                        if (!(_EdgeType is AListBaseEdgeType))
                        {
                            throw new GraphDBException(new Error_InvalidEdgeType(_EdgeType.GetType(), typeof(AListBaseEdgeType)));
                        }
                        break;
                    case TypesOfPandoraType.SetOfReferences:
                        if (!(_EdgeType is ASetReferenceEdgeType))
                        {
                            throw new GraphDBException(new Error_InvalidEdgeType(_EdgeType.GetType(), typeof(ASetReferenceEdgeType)));
                        }
                        break;
                    case TypesOfPandoraType.SetOfNoneReferences:
                        if (!(_EdgeType is ASetBaseEdgeType))
                        {
                            throw new GraphDBException(new Error_InvalidEdgeType(_EdgeType.GetType(), typeof(ASetBaseEdgeType)));
                        }
                       break;
                    default:
                        break;
                }
                

                #endregion

            }
        }


    }
}
