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
  public class LanguageData {
    public readonly Grammar Grammar;
    public readonly GrammarData GrammarData; 
    public readonly ParserData ParserData;
    public readonly ScannerData ScannerData;
    public readonly GrammarErrorList Errors = new GrammarErrorList(); 
    public GrammarErrorLevel ErrorLevel = GrammarErrorLevel.NoError;

    public LanguageData(Grammar grammar) {
      Grammar = grammar;
      GrammarData = new GrammarData(this);
      ParserData = new ParserData(this);
      ScannerData = new ScannerData(this); 
    }
    public bool CanParse() {
      return ErrorLevel < GrammarErrorLevel.Error;
    }
  }//class
}//namespace
