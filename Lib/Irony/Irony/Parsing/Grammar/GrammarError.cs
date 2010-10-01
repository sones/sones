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
  public enum GrammarErrorLevel {
    NoError, //used only for max error level when there are no errors
    Info,
    Warning,
    Conflict, //shift-reduce or reduce-reduce conflict
    Error,    //severe grammar error, parser construction cannot continue
    InternalError,  //internal Irony error
  }

  public class GrammarError {
    public readonly GrammarErrorLevel Level; 
    public readonly string Message;
    public readonly ParserState State; //can be null!
    public GrammarError(GrammarErrorLevel level, ParserState state, string message) {
      Level = level;
      State = state;
      Message = message; 
    }
  }//class

  public class GrammarErrorList : List<GrammarError> {
    public void Add(GrammarErrorLevel level, ParserState state, string message, params object[] args) {
      if (args != null && args.Length > 0)
        message = String.Format(message, args);
      base.Add(new GrammarError(level, state, message));
    }
    public void AddAndThrow(GrammarErrorLevel level, ParserState state, string message, params object[] args) {
      Add(level, state, message, args);
      var error = this[this.Count - 1];
      var exc = new GrammarErrorException(error.Message, error);
      throw exc; 
    }
    public GrammarErrorLevel GetMaxLevel() {
      var max = GrammarErrorLevel.NoError;
      foreach (var err in this)
        if (max < err.Level)
          max = err.Level;
      return max; 
    }
  }

  //Used to cancel parser construction when fatal error is found
  public class GrammarErrorException : Exception {
    public readonly GrammarError Error;
    public GrammarErrorException(String message, GrammarError error) : base(message) {
      Error = error; 
    }

  }//class


}
