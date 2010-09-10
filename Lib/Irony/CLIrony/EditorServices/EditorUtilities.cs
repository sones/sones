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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.CLIrony.Compiler;

namespace sones.Lib.Frameworks.CLIrony.EditorServices
{

    public static class EditorUtilities
    {
        public static TokenList GetOpeningBraces(TokenList tokens)
        {
            TokenList braces = new TokenList();
            foreach (Token token in tokens)
                if (token.Term.IsSet(TermOptions.IsOpenBrace) && token.OtherBrace != null)
                    braces.Add(token);
            return braces;
        }
    }

}//namespace
