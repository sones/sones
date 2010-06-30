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


/* <id name="GraphDB – AddToListAttrUpdateAddToNode" />
 * <copyright file="AddToListAttrUpdateAddToNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using System;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    public class AddToListAttrUpdateOperatorNode : AddToListAttrUpdateNode
    {
        public AddToListAttrUpdateOperatorNode()
        {

        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (((IDNode)parseNode.ChildNodes[0].AstNode).IsValidated)
            {
                if (((IDNode)parseNode.ChildNodes[0].AstNode).Edges.Count > 1)
                {
                    //there is a IDNOde with edges greater than 1. THat means a user tried to traverse more than one edge. this is not possible during a remove
                    throw new GraphDBException(new Error_InvalidAttribute(((IDNode)parseNode.ChildNodes[0].AstNode).ToString() + " It is not valid to traverse multiple edged while adding members to a list attribute."));
                }

                #region set Data

                _Attribute = ((IDNode)parseNode.ChildNodes[0].AstNode).LastAttribute;
                _AttrName = _Attribute.Name;

                #endregion
            }
            
            _elementsToBeAdded = (CollectionOfDBObjectsNode)parseNode.ChildNodes[2].AstNode;
            _AttrName = parseNode.ChildNodes[0].FirstChild.FirstChild.Token.ValueString;
        }
    }//class
}//namespace
