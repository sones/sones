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

/* <id name="GraphDB – SetRefNode node" />
 * <copyright file="SetRefNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of SetRefNode statement.</summary>
 */

#region Usings

using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.Frameworks.Irony.Parsing;


#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an SetRefNode statement.
    /// </summary>
    public class SetRefNode : AStructureNode, IAstNodeInit
    {

        public SetRefDefinition SetRefDefinition { get; private set; }

        #region Data

        private Boolean _IsREFUUID = false;

        #endregion

        #region constructor

        public SetRefNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var grammar = GetGraphQLGrammar(context);
            if (parseNode.ChildNodes[0].Term == grammar.S_REFUUID || parseNode.ChildNodes[0].Term == grammar.S_REFERENCEUUID)
            {
                _IsREFUUID = true;
            }

            var tupleNode = parseNode.ChildNodes[1].AstNode as TupleNode;

            if (tupleNode == null)
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            ADBBaseObject[] parameters = null;
            if (parseNode.ChildNodes[2].AstNode is ParametersNode)
            {
                parameters = (parseNode.ChildNodes[2].AstNode as ParametersNode).ParameterValues.ToArray();
            }

            SetRefDefinition = new SetRefDefinition(tupleNode.TupleDefinition, _IsREFUUID, parameters);
        
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
