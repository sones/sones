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
using sones.Lib.Frameworks.Irony.Parsing.Construction;

namespace sones.Lib.Frameworks.Irony.Diagnostics {
  public static class DiagnosticUtils {

    public static string PrintStateList(LanguageData language) {
      StringBuilder sb = new StringBuilder();
      foreach (ParserState state in language.ParserData.States) {
        sb.Append("State " + state.Name); 
        if (state.BuilderData.IsInadequate) sb.Append(" (Inadequate)");
        sb.AppendLine();
        var srConflicts = state.BuilderData.GetShiftReduceConflicts();
        if (srConflicts.Count > 0)
          sb.AppendLine("  Shift-reduce conflicts on inputs: " + srConflicts.ToString());
        var ssConflicts = state.BuilderData.GetReduceReduceConflicts();
        if (ssConflicts.Count > 0)
          sb.AppendLine("  Reduce-reduce conflicts on inputs: " + ssConflicts.ToString());
        //LRItems
        if (state.BuilderData.ShiftItems.Count > 0) {
          sb.AppendLine("  Shift items:");
          foreach (var item in state.BuilderData.ShiftItems)
            sb.AppendLine("    " + item.ToString());
        }
        if (state.BuilderData.ReduceItems.Count > 0) {
          sb.AppendLine("  Reduce items:");
          foreach (LRItem item in state.BuilderData.ReduceItems)
            sb.AppendLine("    " + item.ToString(state.BuilderData.JumpLookaheads));
        }
        bool headerPrinted = false;
        foreach (BnfTerm key in state.Actions.Keys) {
          ParserAction action = state.Actions[key];
          if (action.ActionType != ParserActionType.Shift && action.ActionType != ParserActionType.Jump) continue;
          if (!headerPrinted)
            sb.Append("  Shifts: ");
          headerPrinted = true;
          sb.Append(key.ToString());
          if (action.ActionType == ParserActionType.Shift)
            sb.Append("->"); //shift
          sb.Append(action.NewState.Name);
          sb.Append(", ");
        }
        sb.AppendLine();
        if (state.BuilderData.JumpLookaheads.Count > 0) 
          //two spaces between 'state' and state name - important for locating state from parser trace
          sb.AppendLine("  Jump to non-canonical state  " + state.BuilderData.JumpTarget + " on lookaheads: " + state.BuilderData.JumpLookaheads.ToString());
        sb.AppendLine();
      }//foreach
      return sb.ToString();
    }

    public static string PrintTerminals(LanguageData language) {
      StringBuilder sb = new StringBuilder();
      foreach (Terminal term in language.GrammarData.Terminals) {
        sb.Append(term.ToString());
        sb.AppendLine();
      }
      return sb.ToString();
    }

    public static string PrintNonTerminals(LanguageData language) {
      StringBuilder sb = new StringBuilder();
      foreach (NonTerminal nt in language.GrammarData.NonTerminals) {
        sb.Append(nt.Name);
        sb.Append(nt.IsSet(TermOptions.IsNullable) ? "  (Nullable) " : "");
        sb.AppendLine();
        foreach (Production pr in nt.Productions) {
          sb.Append("   ");
          sb.AppendLine(pr.ToString());
        }
      }//foreachc nt
      return sb.ToString();
    }

    public static void NotImplemented(String methodName) {
      throw new ApplicationException("Member " + methodName + " not implemented.");
    }

  }//class
}//namespace
