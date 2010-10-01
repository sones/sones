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

namespace sones.Lib.Frameworks.Irony.Parsing.Construction {
  internal class LanguageDataBuilder { 

    internal LanguageData Language;
    Grammar _grammar;
    private ParserStateHash _stateHash = new ParserStateHash();

    public LanguageDataBuilder(LanguageData language) {
      Language = language;
      _grammar = Language.Grammar;
    }

    public bool Build() {
      try {
        if (_grammar.Root == null)
          Language.Errors.AddAndThrow(GrammarErrorLevel.Error, null, "Root property of the grammar is not set.");
        var gbld = new GrammarDataBuilder(Language);
        gbld.Build();
        //Just in case grammar author wants to customize something...
        _grammar.OnGrammarDataConstructed(Language);
        var sbld = new ScannerDataBuilder(Language);
        sbld.Build();
        var pbld = new ParserDataBuilder(Language);
        pbld.Build();
        Validate();
        //call grammar method, a chance to tweak the automaton
        _grammar.OnParserDataConstructed(Language);
        return true;
      } catch (GrammarErrorException) {
        return false; //grammar error should be already added to Language.Errors collection
      } finally {
        Language.ErrorLevel = Language.Errors.GetMaxLevel();
      }

    }

    #region Language Data Validation
    private void Validate() {

    }//method
    #endregion

  
  }//class
}
