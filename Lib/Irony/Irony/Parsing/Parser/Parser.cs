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

namespace sones.Lib.Frameworks.Irony.Parsing {
  //Parser class represents combination of scanner and LALR parser (CoreParser)
  public class Parser { 
    public readonly LanguageData Language; 
    public readonly Scanner Scanner;
    public readonly CoreParser CoreParser;

    public Parser(LanguageData language) {
      Language = language; 
      Scanner = new Scanner(Language.ScannerData);
      CoreParser = new CoreParser(Language.ParserData, Scanner); 
    }

    public ParseTree Parse(CompilerContext context, string sourceText, string fileName) {
      context.CurrentParseTree = new ParseTree(sourceText, fileName);
      Scanner.SetSource(sourceText);
      Scanner.BeginScan(context);
      CoreParser.Parse(context);
      if (context.CurrentParseTree.Errors.Count > 0)
        context.CurrentParseTree.Errors.Sort(SyntaxErrorList.ByLocation);
      return context.CurrentParseTree;
    }
  
  }//class
}//namespace
