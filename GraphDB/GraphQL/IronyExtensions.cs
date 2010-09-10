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
using System.Text;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL
{
    public static class IronyExtensions
    {

        #region Irony: ParseTreeNode, SymbolTerminal

        public static String ToStringChildnodesIncluded(this ParseTreeNode parseTreeNode)
        {

            var retString = new StringBuilder();
            if (parseTreeNode != null)
            {
                retString.Append(parseTreeNode.ToString() + ":");
                if (parseTreeNode.HasChildNodes())
                {
                    foreach (var child in parseTreeNode.ChildNodes)
                    {
                        retString.Append(child.ToString() + ", ");
                    }
                }
            }

            return retString.ToString();

        }

        public static string ToUpperString(this SymbolTerminal symbolTerminal)
        {
            return symbolTerminal.ToString().ToUpper();
        }

        public static string ToString(this SymbolTerminal symbolTerminal)
        {
            return symbolTerminal.ToString().ToUpper();
        }

        #endregion

    }
}
