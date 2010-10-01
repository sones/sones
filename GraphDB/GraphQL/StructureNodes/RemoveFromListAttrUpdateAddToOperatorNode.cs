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

/* <id name="GraphDB – RemoveFromListAttrUpdateAddToOperatorNode" />
 * <copyright file="RemoveFromListAttrUpdateAddToOperatorNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class RemoveFromListAttrUpdateAddToOperatorNode : RemoveFromListAttrUpdateNode
    {

        public RemoveFromListAttrUpdateAddToOperatorNode()
        { }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var idChain = ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition;
            var AttrName = parseNode.ChildNodes[0].FirstChild.FirstChild.Token.ValueString;
            var tupleDefinition = ((TupleNode)parseNode.ChildNodes[2].AstNode).TupleDefinition;
            AttributeRemoveList = new Managers.Structures.AttributeRemoveList(idChain, AttrName, tupleDefinition);
        }

    }

}
