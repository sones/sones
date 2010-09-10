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

using System;

using sones.GraphDB.ImportExport;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Import
{

    public class VerbosityNode : AStructureNode
    {

        public VerbosityTypes VerbosityType { get; private set; }

        public VerbosityNode()
        {
            VerbosityType = VerbosityTypes.Errors;
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var verbosityType = VerbosityType;

            if (parseNode.HasChildNodes() && Enum.TryParse<VerbosityTypes>(parseNode.ChildNodes[1].Token.Text, true, out verbosityType))
            {
                VerbosityType = verbosityType;
            }

        }

    }

}
