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

/* <id name="PandoraDB – PandoraType node" />
 * <copyright file="PandoraTypeNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of PandoratType statement.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of a PandoraType statement.
    /// </summary>
    public class GraphDBTypeNode : AStructureNode
    {

        #region Data

        private TypesOfPandoraType _Type;
        private String _Name = null;
        private TypeCharacteristics _TypeCharacteristics = null;
        
        public TypeCharacteristics TypeCharacteristics
        {
            get { return _TypeCharacteristics; }
        }

        /// <summary>
        /// The type of the edges.
        /// </summary>
        public AEdgeType EdgeType
        {
            get { return _EdgeType; }
        }
        AEdgeType _EdgeType;

        #endregion

        #region constructor

        public GraphDBTypeNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

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
            else if (parseNode.FirstChild.AstNode is EdgeTypeDefNode)
            {
                _Type = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).Type;
                _Name = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).Name;
                _TypeCharacteristics = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).TypeCharacteristics;
                _EdgeType = ((EdgeTypeDefNode)parseNode.FirstChild.AstNode).EdgeType;
            }
            else if (parseNode.FirstChild.AstNode is SingleEdgeTypeDefNode)
            {
                _Type = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).Type;
                _Name = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).Name;
                _TypeCharacteristics = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).TypeCharacteristics;
                _EdgeType = ((SingleEdgeTypeDefNode)parseNode.FirstChild.AstNode).EdgeType;
            }
            else if (parseNode.ChildNodes.Count >= 2)
            {
                _Name = parseNode.ChildNodes[2].Token.ValueString;
                
                #region Type userdefined               
                
                Boolean isUserDefined = false;

                GraphDBType DBType = typeManager.GetTypeByName(_Name);

                if (DBType != null)
                    isUserDefined = DBType.IsUserDefined;
                else
                    isUserDefined = true; //for the case we have a create types statement, then the type is actual unknown

                #endregion

                #region set
                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                {
                    if (isUserDefined)
                    {
                        _Type = TypesOfPandoraType.SetOfReferences;
                        _EdgeType = new EdgeTypeSetOfReferences();
                    }
                    else
                    {
                        _Type = TypesOfPandoraType.SetOfNoneReferences;
                        _EdgeType = new EdgeTypeSetOfBaseObjects();
                    }
                }
                #endregion

                #region list
                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.LIST)
                {
                    if (!isUserDefined)
                    {
                        _Type = TypesOfPandoraType.ListOfNoneReferences;
                        _EdgeType = new EdgeTypeListOfBaseObjects();
                    }
                    else
                        throw new GraphDBException(new Error_ListAttributeNotAllowed(_Name));
                }
                #endregion

            }
            else
            {
                throw new ArgumentException("Invalid pandora type definition...");
            }
        }

        public TypesOfPandoraType Type { get { return _Type; } }

        public String Name { get { return _Name; } }        

    }
}
