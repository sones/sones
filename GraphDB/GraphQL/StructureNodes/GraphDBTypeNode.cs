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

/* <id name="GraphDB – GraphType node" />
 * <copyright file="GraphTypeNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of GraphtType statement.</summary>
 */

#region Usings

using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of a GraphType statement.
    /// </summary>
    public class GraphDBTypeNode : AStructureNode
    {

        #region Data

        public DBTypeOfAttributeDefinition DBTypeDefinition { get; private set; }

        #endregion

        #region constructor

        public GraphDBTypeNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.FirstChild.Term.Name.Equals("id_simple"))
            {

                #region simple id

                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    Type = KindsOfType.UnknownSingle,
                    Name = parseNode.ChildNodes[0].Token.ValueString
                };

                #endregion

            }

            else if (parseNode.FirstChild.Term.Name.ToUpper().Equals(DBConstants.DBBackwardEdge.ToUpper()))
            {

                #region BackwardedgeDefinition

                var _TypeCharacteristics = new TypeCharacteristics();
                _TypeCharacteristics.IsBackwardEdge = true;
                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    TypeCharacteristics = _TypeCharacteristics,
                    Name = parseNode.ChildNodes[0].Token.ValueString
                };

                #endregion

            }

            else if (parseNode.FirstChild.AstNode is EdgeTypeDefNode)
            {

                #region EdgeType definition

                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    Type = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).Type,
                    Name = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).Name,
                    TypeCharacteristics = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).TypeCharacteristics,
                    EdgeType = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).EdgeType,
                    Parameters = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).Parameters,
                };

                #endregion

            }

            else if (parseNode.FirstChild.AstNode is SingleEdgeTypeDefNode)
            {

                #region Single edge type definition

                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    Type = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).Type,
                    Name = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).Name,
                    TypeCharacteristics = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).TypeCharacteristics,
                    EdgeType = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).EdgeType,
                    Parameters = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).Parameters,
                };

                #endregion

            }

            else if (parseNode.ChildNodes.Count >= 2)
            {

                KindsOfType type;

                #region set
                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                {
                    type = KindsOfType.UnknownSet;
                }
                #endregion

                #region list
                else if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.LIST)
                {
                    type = KindsOfType.UnknownList;
                }
                #endregion

                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                DBTypeDefinition = new DBTypeOfAttributeDefinition()
                {
                    Type = type,
                    Name = parseNode.ChildNodes[2].Token.ValueString
                };
            }

            else
            {
                throw new ArgumentException("Invalid Graph type definition...");
            }

        }

    }

}
