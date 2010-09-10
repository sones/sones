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


#region Usings

using sones.GraphDB.Exceptions;
using sones.GraphDB.ImportExport;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Dump
{

    public class DumpFormatNode : AStructureNode
    {

        public DumpFormats DumpFormat { get; set; }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            
            var _GraphQL = GetGraphQLGrammar(context);

            if (parseNode.HasChildNodes())
            {

                var _Terminal = parseNode.ChildNodes[1].Token.Terminal;

                if (_Terminal == _GraphQL.S_GQL)
                {
                    DumpFormat = DumpFormats.GQL;
                }
                else
                {
                    throw new GraphDBException(new Errors.Error_InvalidDumpFormat(_Terminal.DisplayName));
                }

            }
            else
            {
                DumpFormat = DumpFormats.GQL;
            }

        }

    }

}
