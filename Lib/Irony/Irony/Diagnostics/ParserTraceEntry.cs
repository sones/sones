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
using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.Lib.Frameworks.Irony.Diagnostics {
  public class ParserTraceEntry {
    public ParserState State;
    public ParseTreeNode StackTop;
    public ParseTreeNode Input;
    public string Message;
    public ParserState NewState;
    public bool IsError;

    public ParserTraceEntry(ParserState state, ParseTreeNode stackTop, ParseTreeNode input) {
      State = state;
      StackTop = stackTop;
      Input = input;
    }
    public void SetDetails(String message, ParserState newState) {
      Message = message;
      NewState = newState;
    }
  }//class

  public class ParserTrace : List<ParserTraceEntry> { }

  public class ParserTraceEventArgs : EventArgs {
    public ParserTraceEventArgs(ParserTraceEntry entry) {
      Entry = entry; 
    }

    public readonly ParserTraceEntry Entry;

    public override string ToString() {
      return Entry.ToString(); 
    }
  }//class



}//namespace
