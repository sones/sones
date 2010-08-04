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

/* <id name="GraphDB – RemoveFromListAttrUpdateAddToOperatorNode" />
 * <copyright file="RemoveFromListAttrUpdateAddToOperatorNode.cs
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
    /// <summary>
    /// 
    /// </summary>
    public class RemoveFromListAttrUpdateAddToOperatorNode : RemoveFromListAttrUpdateNode
    {
        public RemoveFromListAttrUpdateAddToOperatorNode()
        {

        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var idChain = ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition;
            var AttrName = parseNode.ChildNodes[0].FirstChild.FirstChild.Token.ValueString;
            var tupleDefinition = ((TupleNode)parseNode.ChildNodes[2].AstNode).TupleDefinition;
            AttributeRemoveList = new Managers.Structures.AttributeRemoveList(idChain, AttrName, tupleDefinition);

        }
    }//class
}//namespace
