/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structure.Nodes.DML;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This node is requested in case of an SetRefNode statement.
    /// </summary>
    public sealed class SetRefNode : AStructureNode, IAstNodeInit
    {
        public SetRefDefinition SetRefDefinition { get; private set; }

        #region Data

        public Boolean IsREFUUID = false;

        #endregion

        #region constructor

        public SetRefNode()
        {

        }

        #endregion


        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var grammar = (SonesGQLGrammar)context.Language.Grammar;
            if (parseNode.ChildNodes[0].Term == grammar.S_REFUUID || parseNode.ChildNodes[0].Term == grammar.S_REFERENCEUUID)
            {
                IsREFUUID = true;
            }

            if (parseNode.ChildNodes.Count >3)
            {
                var tupleNode = parseNode.ChildNodes[4].AstNode as TupleNode;

                if (tupleNode == null)
                {
                    throw new NotImplementedQLException("");
                }

                Dictionary<string, object> parameters = null;
                if (parseNode.ChildNodes[5].AstNode is ParametersNode)
                {
                    parameters = (parseNode.ChildNodes[5].AstNode as ParametersNode).ParameterValues;
                }

                String referencedVertexType = parseNode.ChildNodes[2].Token.ValueString;

                SetRefDefinition = new SetRefDefinition(tupleNode.TupleDefinition, IsREFUUID, referencedVertexType, parameters);
            }
            else
            {
                var tupleNode = parseNode.ChildNodes[1].AstNode as TupleNode;

                if (tupleNode == null)
                {
                    throw new NotImplementedQLException("");
                }

                Dictionary<string, object> parameters = null;
                if (parseNode.ChildNodes[2].AstNode is ParametersNode)
                {
                    parameters = (parseNode.ChildNodes[2].AstNode as ParametersNode).ParameterValues;
                }

                SetRefDefinition = new SetRefDefinition(tupleNode.TupleDefinition, IsREFUUID, String.Empty, parameters);
            }

            
        }

        #endregion
    }
}
