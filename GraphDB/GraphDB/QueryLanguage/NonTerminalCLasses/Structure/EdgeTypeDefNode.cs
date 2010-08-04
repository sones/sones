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

/* <id name="sones GraphDB – EdgeTypeDefNode" />
 * <copyright file="EdgeTypeDefNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
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

        public KindsOfType Type { get; private set; }

        public String Name { get; private set; }
        
        /// <summary>
        /// The characteristics of an edge (Backward, Mandatory, Unique - valid for a combination of typeattributes
        /// </summary>
        public TypeCharacteristics TypeCharacteristics { get; private set; }

        /// <summary>
        /// An edge type definition - resolved by the name and found in the typeManager EdgeTypesLookup table
        /// </summary>
        public AEdgeType EdgeType{ get; private set; }

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

            EdgeType = dbContext.DBPluginManager.GetEdgeType(parseNode.ChildNodes[2].Token.ValueString);

            if (parseNode.ChildNodes[3].AstNode != null)
            {
                EdgeType.ApplyParams(((EdgeTypeParamsNode)parseNode.ChildNodes[3].AstNode).Parameters);
            }

            #endregion

            if (parseNode.FirstChild.Term.Name.Equals("id_simple"))
            {
                #region simple id

                Type = KindsOfType.SingleNoneReference;

                Name = parseNode.ChildNodes[0].Token.ValueString;

                #endregion
            }
            else if (parseNode.FirstChild.Term.Name.ToUpper().Equals(DBConstants.DBBackwardEdge.ToUpper()))
            {

                Name = ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition.Reference.Item2.Name;
                TypeCharacteristics = new TypeCharacteristics();
                TypeCharacteristics.IsBackwardEdge = true;
            }
            else
            {
                Name = parseNode.ChildNodes[5].Token.ValueString;

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                {

                    GraphDBType gType = typeManager.GetTypeByName(Name);

                    if (gType != null)
                    {
                        if (gType.IsUserDefined)
                        {
                            Type = KindsOfType.SetOfReferences;
                        }
                        else
                        {
                            Type = KindsOfType.SetOfNoneReferences;
                        }
                    }
                    else
                        Type = KindsOfType.SetOfReferences;
                }

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.LIST)
                    Type = KindsOfType.ListOfNoneReferences;

                #region Verify edge against the type LIST/SET

                switch (Type)
                {
                    case KindsOfType.ListOfNoneReferences:
                        if (!(EdgeType is AListBaseEdgeType))
                        {
                            throw new GraphDBException(new Error_InvalidEdgeType(EdgeType.GetType(), typeof(AListBaseEdgeType)));
                        }
                        break;
                    case KindsOfType.SetOfReferences:
                        if (!(EdgeType is ASetReferenceEdgeType))
                        {
                            throw new GraphDBException(new Error_InvalidEdgeType(EdgeType.GetType(), typeof(ASetReferenceEdgeType)));
                        }
                        break;
                    case KindsOfType.SetOfNoneReferences:
                        if (!(EdgeType is ASetBaseEdgeType))
                        {
                            throw new GraphDBException(new Error_InvalidEdgeType(EdgeType.GetType(), typeof(ASetBaseEdgeType)));
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
