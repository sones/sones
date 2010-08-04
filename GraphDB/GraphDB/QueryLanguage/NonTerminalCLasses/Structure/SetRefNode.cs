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

/* <id name="sones GraphDB – SetRefNode node" />
 * <copyright file="SetRefNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of SetRefNode statement.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;

using sones.Lib.Frameworks.Irony.Parsing;

using sones.Lib.DataStructures;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Objects;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.Lib.Session;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
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
