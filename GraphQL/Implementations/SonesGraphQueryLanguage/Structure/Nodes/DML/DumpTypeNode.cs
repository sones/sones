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

using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.ErrorHandling;
using sones.Library.DataStructures;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class DumpTypeNode : AStructureNode, IAstNodeInit
    {
        #region data

        public DumpTypes DumpType { get; set; }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var _GraphQL = context.Parser.Language.Grammar;

            if (HasChildNodes(parseNode))
            {

                var _Terminal = parseNode.ChildNodes[0].Token.Terminal;

                if (_Terminal == _GraphQL.ToTerm("ALL"))
                {
                    DumpType = DumpTypes.GDDL | DumpTypes.GDML;
                }
                else if (_Terminal == _GraphQL.ToTerm("GDDL"))
                {
                    DumpType = DumpTypes.GDDL;
                }
                else if (_Terminal == _GraphQL.ToTerm("GDML"))
                {
                    DumpType = DumpTypes.GDML;
                }
                else
                {
                    throw new InvalidDumpTypeException(_Terminal.ToString(), "");
                }

            }
            else
            {
                DumpType = DumpTypes.GDDL | DumpTypes.GDML;
            }
        }

        #endregion

    }
}
