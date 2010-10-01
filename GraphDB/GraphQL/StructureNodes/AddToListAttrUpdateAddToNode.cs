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

/* <id name="GraphDB – AddToListAttrUpdateAddToNode" />
 * <copyright file="AddToListAttrUpdateAddToNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using sones.GraphDB.Managers.Structures;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// Warnings: Warning_ObsoleteGQL
    /// </summary>
    public class AddToListAttrUpdateAddToNode : AddToListAttrUpdateNode
    {

        public AddToListAttrUpdateAddToNode()
        {

        }

        public new void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var _elementsToBeAdded = (CollectionOfDBObjectsNode)parseNode.ChildNodes[3].AstNode;
            var _AttrName = parseNode.ChildNodes[2].FirstChild.FirstChild.Token.ValueString;

            AttributeUpdateList = new AttributeAssignOrUpdateList(_elementsToBeAdded.CollectionDefinition, ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition, false);

            base.ParsingResult.PushIWarning(new Warnings.Warning_ObsoleteGQL("ADD TO", "+="));
        }

    }

}
