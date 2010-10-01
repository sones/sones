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

/* <id name="GraphDB – WhereExpressionNode" />
 * <copyright file="WhereExpressionNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Daniel Kirstenpfad
 * <summary>This node is requested in case of where clause.</summary>
 */

#region Usings

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class WhereExpressionNode : AStructureNode
    {

        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        public WhereExpressionNode()
        { }

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            if (myParseTreeNode.HasChildNodes())
            {

                if (myParseTreeNode.ChildNodes[1].AstNode is TupleNode && (myParseTreeNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition.TupleElements.Count == 1)
                {
                    var tuple = (myParseTreeNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition.Simplyfy();
                    BinaryExpressionDefinition = (tuple.TupleElements[0].Value as BinaryExpressionDefinition);
                }
                else if (myParseTreeNode.ChildNodes[1].AstNode is BinaryExpressionNode)
                {
                    BinaryExpressionDefinition = ((BinaryExpressionNode)myParseTreeNode.ChildNodes[1].AstNode).BinaryExpressionDefinition;
                }
                //else
                //{
                //    throw new GraphDBException(new Errors.Error_GqlSyntax("Invalid tuple for where expression"));
                //}
            }
        }

        #endregion

        public override string ToString()
        {
            return "whereClauseOpt";
        }


    }

}
