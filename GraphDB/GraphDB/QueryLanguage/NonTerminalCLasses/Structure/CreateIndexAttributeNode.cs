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


/* <id name="sones GraphDB – CreateIndexAttributeNode Node" />
 * <copyright file="CreateIndexAttributeNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary>This node is requested in case of an CreateIndexAttributeNode node.</summary>
 */

#region usings

using System;
using System.Text;

using sones.Lib.Frameworks.Irony.Parsing;
using System.Collections.Generic;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    /// <summary>
    /// This node is requested in case of an CreateIndexAttributeNode node.
    /// </summary>
    public class CreateIndexAttributeNode : AStructureNode, IAstNodeInit
    {

        #region properties

        private String _IndexAttribute = null;
        private String _OrderDirection = null;
        private String _IndexType = null;

        #endregion

        #region constructor

        public CreateIndexAttributeNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            #region Data            
                                    
            if (parseNode.ChildNodes[0].HasChildNodes())
            {
                if (parseNode.ChildNodes[0].ChildNodes[0].ChildNodes.Count > 1)
                {
                    _IndexType = ((ATypeNode)parseNode.ChildNodes[0].ChildNodes[0].ChildNodes[0].AstNode).DBTypeStream.Name;

                    if (((IDNode)parseNode.ChildNodes[0].ChildNodes[0].ChildNodes[2].AstNode).IsValidated == false)
                        throw new GraphDBException(new Error_IndexTypesOverlap());
                    else
                        _IndexAttribute = ((IDNode)parseNode.ChildNodes[0].ChildNodes[0].ChildNodes[2].AstNode).LastAttribute.Name;                        
                }
                else
                {
                    _IndexAttribute = parseNode.ChildNodes[0].ChildNodes[0].Token.ValueString;
                }
            }

            if(parseNode.ChildNodes.Count > 1 && parseNode.ChildNodes[1].HasChildNodes())
                _OrderDirection = parseNode.ChildNodes[1].FirstChild.Token.ValueString;

            else
                _OrderDirection = String.Empty;

            #endregion

        }

        #region Accessessors

        public String IndexAttribute { get { return _IndexAttribute; } }
        public String OrderDirection { get { return _OrderDirection; } }
        public String IndexTypes { get { return _IndexType; } }

        #endregion


        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion


        #region ToString()

        public override String ToString()
        {

            if (_OrderDirection.Equals(String.Empty))
                return String.Concat(_IndexAttribute);

            else
                return String.Concat(_IndexAttribute, " ", _OrderDirection);

        }

        #endregion

    }//class
}//namespace
